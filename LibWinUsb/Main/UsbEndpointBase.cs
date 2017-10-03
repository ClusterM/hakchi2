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
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using LibUsbDotNet.Info;
using LibUsbDotNet.Internal;

namespace LibUsbDotNet.Main
{
    /// <summary> 
    /// Endpoint members common to Read, Write, Bulk, and Interrupt <see cref="T:LibUsbDotNet.Main.EndpointType"/>.
    /// </summary> 
    public abstract class UsbEndpointBase : IDisposable
    {
        /// <summary>
        /// The maximum transfer payload size for all usb endpoints.
        /// </summary>
        /// <remarks>
        /// Transfers greater than this amount are automatically split into
        /// multiple transfers.  This applies to all endpoint transfer methods
        /// (reads and writes). The default is 4megs (4,194,304 bytes)
        /// </remarks>
        public static int MaxReadWrite = 65536;

        internal readonly byte mEpNum;
        internal readonly UsbApiBase mUsbApi;
        private readonly UsbDevice mUsbDevice;
        private readonly SafeHandle mUsbHandle;
        private bool mIsDisposed;
        internal TransferDelegate mPipeTransferSubmit;
        private UsbTransfer mTransferContext;
        private UsbEndpointInfo mUsbEndpointInfo;
        private EndpointType mEndpointType;
        private UsbInterfaceInfo mUsbInterfacetInfo;

        internal UsbEndpointBase(UsbDevice usbDevice, byte epNum, EndpointType endpointType)
        {
            mUsbDevice = usbDevice;
            mUsbApi = mUsbDevice.mUsbApi;
            mUsbHandle = mUsbDevice.Handle;
            mEpNum = epNum;
            mEndpointType = endpointType;
            if ((mEpNum & 0x80) > 0)
            {
                mPipeTransferSubmit = ReadPipe;
            }
            else
                mPipeTransferSubmit = WritePipe;
        }


        internal virtual TransferDelegate PipeTransferSubmit
        {
            get { return mPipeTransferSubmit; }
        }

        internal UsbTransfer TransferContext
        {
            get
            {
                if (ReferenceEquals(mTransferContext, null))
                {
                    mTransferContext = CreateTransferContext();
                }
                return mTransferContext;
            }
        }

        /// <summary>
        /// Gets a value indicating if the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return mIsDisposed; }
        }

        /// <summary>
        /// Gets the <see cref="UsbDevice"/> class this endpoint belongs to.
        /// </summary>
        public UsbDevice Device
        {
            get { return mUsbDevice; }
        }


        internal SafeHandle Handle
        {
            get { return mUsbHandle; }
        }

        /// <summary>
        /// Gets the endpoint ID for this <see cref="UsbEndpointBase"/> class.
        /// </summary>
        public byte EpNum
        {
            get
            {
                return mEpNum;
            }
        }

        /// <summary>
        /// Returns the <see cref="EndpointType"/> for this endpoint.
        /// </summary>
        public EndpointType Type
        {
            get { return mEndpointType; }
        }

        /// <summary>
        /// Returns the <see cref="UsbEndpointInfo"/> descriptor for this endpoint.
        /// </summary>
        public UsbEndpointInfo EndpointInfo
        {
            get
            {
                if (ReferenceEquals(mUsbEndpointInfo, null))
                {
                    if (!LookupEndpointInfo(Device.Configs[0], mEpNum, out mUsbInterfacetInfo, out mUsbEndpointInfo))
                    {
                        // throw new UsbException(this, String.Format("Failed locating endpoint {0} for the current usb configuration.", mEpNum));
                        return null;
                    }
                }
                return mUsbEndpointInfo;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Frees resources associated with the endpoint.  Once disposed this <see cref="UsbEndpointBase"/> cannot be used.
        /// </summary>
        public virtual void Dispose() { DisposeAndRemoveFromList(); }

        #endregion

        internal abstract UsbTransfer CreateTransferContext();

        /// <summary>
        /// Aborts pending IO operation on this enpoint of one exists.
        /// </summary>
        /// <returns>True on success or if no pending IO operation exits.</returns>
        public virtual bool Abort()
        {
            if (mIsDisposed) throw new ObjectDisposedException(GetType().Name);
            bool bSuccess = TransferContext.Cancel() == ErrorCode.Success;

            return bSuccess;
        }

        /// <summary>
        /// Discards any data that is cached in this endpoint.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Flush()
        {
            if (mIsDisposed) throw new ObjectDisposedException(GetType().Name);

            bool bSuccess = mUsbApi.FlushPipe(mUsbHandle, EpNum);

            if (!bSuccess) UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "FlushPipe", this);

            return bSuccess;
        }

        /// <summary>
        /// Resets the data toggle and clears the stall condition on an enpoint.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Reset()
        {
            if (mIsDisposed) throw new ObjectDisposedException(GetType().Name);

            bool bSuccess = mUsbApi.ResetPipe(mUsbHandle, EpNum);

            if (!bSuccess) UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "ResetPipe", this);

            return bSuccess;
        }

        /// <summary>
        /// Synchronous bulk/interrupt transfer function.
        /// </summary>
        /// <param name="buffer">An <see cref="IntPtr"/> to a caller-allocated buffer.</param>
        /// <param name="offset">Position in buffer that transferring begins.</param>
        /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>True on success.</returns>
        public virtual ErrorCode Transfer(IntPtr buffer, int offset, int length, int timeout, out int transferLength) { return UsbTransfer.SyncTransfer(TransferContext, buffer, offset, length, timeout, out transferLength); }

        /// <summary>
        /// Creates, fills and submits an asynchronous <see cref="UsbTransfer"/> context.
        /// </summary>
        /// <remarks>
        /// <note type="tip">This is a non-blocking asynchronous transfer function. This function returns immediately after the context is created and submitted.</note>
        /// </remarks>
        /// <param name="buffer">A caller-allocated buffer for the data that is transferred.</param>
        /// <param name="offset">Position in buffer that transferring begins.</param>
        /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
        /// <param name="transferContext">On <see cref="ErrorCode.Success"/>, a new transfer context.</param>
        /// <returns><see cref="ErrorCode.Success"/> if the transfer context was created and <see cref="UsbTransfer.Submit"/> succeeded.</returns>
        /// <seealso cref="SubmitAsyncTransfer(System.IntPtr,int,int,int,out LibUsbDotNet.Main.UsbTransfer)"/>
        /// <seealso cref="NewAsyncTransfer"/>
        public virtual ErrorCode SubmitAsyncTransfer(object buffer, int offset, int length, int timeout, out UsbTransfer transferContext)
        {
            transferContext = CreateTransferContext();
            transferContext.Fill(buffer, offset, length, timeout);

            ErrorCode ec = transferContext.Submit();
            if (ec != ErrorCode.None)
            {
                transferContext.Dispose();
                transferContext = null;
                UsbError.Error(ec, 0, "SubmitAsyncTransfer Failed", this);
            }

            return ec;
        }

        /// <summary>
        /// Creates, fills and submits an asynchronous <see cref="UsbTransfer"/> context.
        /// </summary>
        /// <remarks>
        /// <note type="tip">This is a non-blocking asynchronous transfer function. This function returns immediately after the context is created and submitted.</note>
        /// </remarks>
        /// <param name="buffer">A caller-allocated buffer for the data that is transferred.</param>
        /// <param name="offset">Position in buffer that transferring begins.</param>
        /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
        /// <param name="transferContext">On <see cref="ErrorCode.Success"/>, a new transfer context.</param>
        /// <returns><see cref="ErrorCode.Success"/> if the transfer context was created and <see cref="UsbTransfer.Submit"/> succeeded.</returns>
        /// <seealso cref="SubmitAsyncTransfer(object,int,int,int,out LibUsbDotNet.Main.UsbTransfer)"/>
        /// <seealso cref="NewAsyncTransfer"/>
        public virtual ErrorCode SubmitAsyncTransfer(IntPtr buffer, int offset, int length, int timeout, out UsbTransfer transferContext)
        {
            transferContext = CreateTransferContext();
            transferContext.Fill(buffer, offset, length, timeout);

            ErrorCode ec = transferContext.Submit();
            if (ec != ErrorCode.None)
            {
                transferContext.Dispose();
                transferContext = null;
                UsbError.Error(ec, 0, "SubmitAsyncTransfer Failed", this);
            }

            return ec;
        }
        /// <summary>
        /// Creates a <see cref="UsbTransfer"/> context for asynchronous transfers.
        /// </summary>
        /// <remarks>
        /// <para> This method returns a new, empty transfer context.  Unlike <see cref="SubmitAsyncTransfer(object,int,int,int,out LibUsbDotNet.Main.UsbTransfer)">SubmitAsyncTransfer</see>, this context is <c>not</c> filled and submitted.</para>
        /// <note type="tip">This is a non-blocking asynchronous transfer function. This function returns immediately after the context created.</note>
        /// </remarks>
        /// <returns>A new <see cref="UsbTransfer"/> context.</returns>
        /// <seealso cref="SubmitAsyncTransfer(System.IntPtr,int,int,int,out LibUsbDotNet.Main.UsbTransfer)"/>
        /// <seealso cref="SubmitAsyncTransfer(object,int,int,int,out LibUsbDotNet.Main.UsbTransfer)"/>
        public UsbTransfer NewAsyncTransfer()
        {
            UsbTransfer transfer = CreateTransferContext();
            return transfer;
        }

        /// <summary>
        /// Looks up endpoint/interface information in a configuration.
        /// </summary>
        /// <param name="currentConfigInfo">The config to seach.</param>
        /// <param name="endpointAddress">The endpoint address to look for.</param>
        /// <param name="usbInterfaceInfo">On success, the <see cref="UsbInterfaceInfo"/> class for this endpoint.</param>
        /// <param name="usbEndpointInfo">On success, the <see cref="UsbEndpointInfo"/> class for this endpoint.</param>
        /// <returns>True of the endpoint was found, otherwise false.</returns>
        public static bool LookupEndpointInfo(UsbConfigInfo currentConfigInfo, byte endpointAddress, out UsbInterfaceInfo usbInterfaceInfo, out UsbEndpointInfo usbEndpointInfo)
        {
            bool found = false;

            usbInterfaceInfo = null;
            usbEndpointInfo = null;
            foreach (UsbInterfaceInfo interfaceInfo in currentConfigInfo.InterfaceInfoList)
            {
                foreach (UsbEndpointInfo endpointInfo in interfaceInfo.EndpointInfoList)
                {
                    if ((endpointAddress & UsbConstants.ENDPOINT_NUMBER_MASK) == 0)
                    {
                        // find first read/write endpoint
                        if ((endpointAddress & UsbConstants.ENDPOINT_DIR_MASK) == 0 && 
                            (endpointInfo.Descriptor.EndpointID & UsbConstants.ENDPOINT_DIR_MASK) == 0)
                        {
                            // first write endpoint
                            found = true;
                        }
                        if ((endpointAddress & UsbConstants.ENDPOINT_DIR_MASK) != 0 && 
                            (endpointInfo.Descriptor.EndpointID & UsbConstants.ENDPOINT_DIR_MASK) != 0)
                        {
                            // first read endpoint
                            found = true;
                        }
                    }
                    else if (endpointInfo.Descriptor.EndpointID == endpointAddress)
                    {
                        found = true;
                    }

                    if (found)
                    {
                        usbInterfaceInfo = interfaceInfo;
                        usbEndpointInfo = endpointInfo;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Synchronous bulk/interrupt transfer function.
        /// </summary>
        /// <param name="buffer">A caller-allocated buffer for the transfer data. This object is pinned using <see cref="PinnedHandle"/>.</param>
        /// <param name="offset">Position in buffer that transferring begins.</param>
        /// <param name="length">Number of bytes, starting from thr offset parameter to transfer.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>True on success.</returns>
        public ErrorCode Transfer(object buffer, int offset, int length, int timeout, out int transferLength)
        {
            PinnedHandle pinned = new PinnedHandle(buffer);
            ErrorCode eReturn = Transfer(pinned.Handle, offset, length, timeout, out transferLength);
            pinned.Dispose();
            return eReturn;
        }

        private void DisposeAndRemoveFromList()
        {
            if (!mIsDisposed)
            {
                UsbEndpointReader epReader = this as UsbEndpointReader;
                if (!ReferenceEquals(epReader, null))
                {
                    if (epReader.DataReceivedEnabled) epReader.DataReceivedEnabled = false;
                }
                Abort();
                mUsbDevice.ActiveEndpoints.RemoveFromList(this);
            }
            mIsDisposed = true;
        }

        private int ReadPipe(IntPtr pBuffer, int bufferLength, out int lengthTransferred, int isoPacketSize, IntPtr pOverlapped)
        {
            bool bSuccess = mUsbApi.ReadPipe(this, pBuffer, bufferLength, out lengthTransferred, isoPacketSize, pOverlapped);
            if (!bSuccess) return Marshal.GetLastWin32Error();
            return 0;
        }


        private int WritePipe(IntPtr pBuffer, int bufferLength, out int lengthTransferred, int isoPacketSize, IntPtr pOverlapped)
        {
            bool bSuccess = mUsbApi.WritePipe(this, pBuffer, bufferLength, out lengthTransferred, isoPacketSize, pOverlapped);
            if (!bSuccess) return Marshal.GetLastWin32Error();
            return 0;
        }

        #region Nested type: TransferDelegate

        internal delegate int TransferDelegate(IntPtr pBuffer, int bufferLength, out int lengthTransferred, int isoPacketSize, IntPtr pOverlapped);

        #endregion
    }
}