// Copyright © 2006-2009 Travis Robinson. All rights reserved.
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
using System.Threading;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;

namespace LibUsbDotNet.Internal
{
    public abstract class TransferContextBase : IDisposable
    {
        private readonly UsbEndpointBase mEndpointBase;

        private IntPtr mBuffer;
        private int mCurrentOffset;
        private int mCurrentRemaining;
        private int mCurrentTransmitted;

        private int mFailRetries;
        protected int mOriginalCount;
        protected int mOriginalOffset;
        private PinnedHandle mPinnedHandle;

        protected int mTimeout;

        protected bool mHasWaitBeenCalled = true;

        protected ManualResetEvent mTransferCancelEvent = new ManualResetEvent(false);
        protected internal ManualResetEvent mTransferCompleteEvent = new ManualResetEvent(true);

        protected TransferContextBase(UsbEndpointBase endpointBase) { mEndpointBase = endpointBase; }

        public UsbEndpointBase EndpointBase
        {
            get { return mEndpointBase; }
        }

        protected int RequestCount
        {
            get { return (mCurrentRemaining > UsbEndpointBase.MaxReadWrite ? UsbEndpointBase.MaxReadWrite : mCurrentRemaining); }
        }

        protected int FailRetries
        {
            get { return mFailRetries; }
        }

        protected IntPtr NextBufPtr
        {
            get { return new IntPtr(mBuffer.ToInt64() + mCurrentOffset); }
        }

        public bool IsCancelled
        {
            get { return mTransferCancelEvent.WaitOne(0, UsbConstants.EXIT_CONTEXT); }
        }

        public bool IsComplete
        {
            get { return mTransferCompleteEvent.WaitOne(0, UsbConstants.EXIT_CONTEXT); }
        }

        public SafeWaitHandle CancelWaitHandle
        {
            get { return mTransferCancelEvent.SafeWaitHandle; }
        }

        public SafeWaitHandle CompleteWaitHandle
        {
            get { return mTransferCompleteEvent.SafeWaitHandle; }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (!IsCancelled) Cancel();

            int dummy;
            if (!mHasWaitBeenCalled) Wait(out dummy);
        }

        #endregion

        public virtual ErrorCode Cancel()
        {
            mTransferCancelEvent.Set();
            return ErrorCode.Success;
        }

        public abstract ErrorCode Submit();

        public abstract ErrorCode Wait(out int transferredCount);


        public virtual void Fill(object buffer, int offset, int count, int timeout)
        {
            if (mPinnedHandle != null) mPinnedHandle.Dispose();
            mPinnedHandle = new PinnedHandle(buffer);
            Fill(mPinnedHandle.Handle, offset, count, timeout);
        }

        public virtual void Fill(IntPtr buffer, int offset, int count, int timeout)
        {
            mBuffer = buffer;

            mOriginalOffset = offset;
            mOriginalCount = count;
            mTimeout = timeout;
            Reset();
        }

        internal static ErrorCode SyncTransfer(TransferContextBase transferContext,
                                               IntPtr buffer,
                                               int offset,
                                               int length,
                                               int timeout,
                                               out int transferLength)
        {
            if (ReferenceEquals(transferContext, null)) throw new NullReferenceException("Invalid transfer context.");
            if (offset < 0) throw new ArgumentException("must be >=0", "offset");

            lock (transferContext)
            {
                transferLength = 0;

                int transferred;
                ErrorCode ec;

                transferContext.Fill(buffer, offset, length, timeout);

                while (true)
                {
                    ec = transferContext.Submit();
                    if (ec == ErrorCode.IoEndpointGlobalCancelRedo) continue;
                    if (ec != ErrorCode.Success) return ec;

                    ec = transferContext.Wait(out transferred);
                    if (ec == ErrorCode.IoEndpointGlobalCancelRedo) continue;
                    if (ec != ErrorCode.Success) return ec;

                    transferLength += transferred;

                    if ((ec != ErrorCode.None || transferred != UsbEndpointBase.MaxReadWrite) ||
                        !transferContext.IncrementTransfer(transferred))
                        break;
                }

                return ec;
            }
        }

        public bool IncrementTransfer(int amount)
        {
            mCurrentTransmitted += amount;
            mCurrentOffset += amount;
            mCurrentRemaining -= amount;

            if (mCurrentRemaining <= 0) return false;

            return true;
        }

        protected void IncFailRetries() { mFailRetries++; }

        public void Reset()
        {
            mCurrentOffset = mOriginalOffset;
            mCurrentRemaining = mOriginalCount;
            mCurrentTransmitted = 0;
            mFailRetries = 0;

            mTransferCancelEvent.Reset();
        }
    }
}