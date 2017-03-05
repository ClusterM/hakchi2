// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// 
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
// 
using System;
using System.Runtime.InteropServices;
using System.Threading;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.Internal
{
    internal class OverlappedTransferContext : UsbTransfer
    {
        private readonly SafeOverlapped mOverlapped = new SafeOverlapped();

        public OverlappedTransferContext(UsbEndpointBase endpointBase)
            : base(endpointBase) { }

        public SafeOverlapped Overlapped
        {
            get { return mOverlapped; }
        }

        public override ErrorCode Submit()
        {
            int iTransferred;
            ErrorCode eReturn = ErrorCode.Success;

            if (mTransferCancelEvent.WaitOne(0, false)) return ErrorCode.IoCancelled;
            if (!mTransferCompleteEvent.WaitOne(0, UsbConstants.EXIT_CONTEXT)) return ErrorCode.ResourceBusy;

            mHasWaitBeenCalled = false;
            mTransferCompleteEvent.Reset();
            Overlapped.ClearAndSetEvent(mTransferCompleteEvent.SafeWaitHandle.DangerousGetHandle());

            int ret = EndpointBase.PipeTransferSubmit(NextBufPtr,
                                                      RequestCount,
                                                      out iTransferred,
                                                      mIsoPacketSize,
                                                      Overlapped.GlobalOverlapped);
            if (ret != 0 && ret != (int) UsbStatusClodes.ErrorIoPending)
            {
                mTransferCompleteEvent.Set();
                UsbError usbErr = UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "PipeTransferSubmit", EndpointBase);

                eReturn = usbErr.ErrorCode;
            }
            return eReturn;
        }

        public override ErrorCode Wait(out int transferredCount, bool cancel) 
        {
            if (mHasWaitBeenCalled) throw new UsbException(this, "Repeated calls to wait with a submit is not allowed.");

            transferredCount = 0;
            bool bSuccess;
            // Temporarily release the transfer lock while we wait for something to happen.
            int iWait = WaitHandle.WaitAny(new WaitHandle[] { mTransferCompleteEvent, mTransferCancelEvent }, mTimeout, UsbConstants.EXIT_CONTEXT);
            if (iWait == WaitHandle.WaitTimeout && !cancel)
            {
                return ErrorCode.IoTimedOut;
            }
            mHasWaitBeenCalled = true;

            if (iWait != 0)
            {
                bSuccess = EndpointBase.mUsbApi.AbortPipe(EndpointBase.Handle, EndpointBase.EpNum);
                bool bTransferComplete = mTransferCompleteEvent.WaitOne(100, UsbConstants.EXIT_CONTEXT);
                mTransferCompleteEvent.Set();
                if (!bSuccess || !bTransferComplete)
                {
                    ErrorCode ec = bSuccess ? ErrorCode.Win32Error : ErrorCode.CancelIoFailed;
                    UsbError.Error(ec, Marshal.GetLastWin32Error(), "Wait:AbortPipe Failed", this);
                    return ec;
                }
                if (iWait == WaitHandle.WaitTimeout) return ErrorCode.IoTimedOut;
                return ErrorCode.IoCancelled;
            }

            try
            {
                bSuccess = EndpointBase.mUsbApi.GetOverlappedResult(EndpointBase.Handle, Overlapped.GlobalOverlapped, out transferredCount, true);
                if (!bSuccess)
                {
                    UsbError usbErr = UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetOverlappedResult", EndpointBase);
                    return usbErr.ErrorCode;
                }
                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.UnknownError;
            }
        }
    }
}