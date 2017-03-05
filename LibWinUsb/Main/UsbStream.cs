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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LibUsbDotNet.Main
{
    public class IOCancelledException : IOException
    {
        public IOCancelledException(string message) : base(message) { }
    }

    public class UsbStreamAsyncTransfer : IAsyncResult
    {
        internal readonly int mCount;
        internal readonly int mOffset;
        internal readonly object mState;
        private readonly int mTimeout;
        internal AsyncCallback mCallback;
        internal ManualResetEvent mCompleteEvent = new ManualResetEvent(false);
        internal GCHandle mGCBuffer;
        internal bool mIsComplete;
        private ErrorCode mResult;
        private int mTrasferredLength;
        internal UsbEndpointBase mUsbEndpoint;

        public UsbStreamAsyncTransfer(UsbEndpointBase usbEndpoint,
                                      byte[] buffer,
                                      int offset,
                                      int count,
                                      AsyncCallback callback,
                                      object state,
                                      int timeout)
        {
            mUsbEndpoint = usbEndpoint;
            mOffset = offset;
            mCount = count;
            mState = state;
            mTimeout = timeout;
            mCallback = callback;
            mGCBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        }

        public ErrorCode Result
        {
            get { return mResult; }
        }

        public int TransferredLength
        {
            get { return mTrasferredLength; }
        }

        #region IAsyncResult Members

        public bool IsCompleted
        {
            get { return mIsComplete; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return mCompleteEvent; }
        }

        public object AsyncState
        {
            get { return mState; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        #endregion

        public ErrorCode SyncTransfer()
        {
            mResult = mUsbEndpoint.Transfer(mGCBuffer.AddrOfPinnedObject(), mOffset, mCount, mTimeout, out mTrasferredLength);
            mGCBuffer.Free();
            mIsComplete = true;
            if (mCallback != null) mCallback(this as IAsyncResult);
            mCompleteEvent.Set();
            return mResult;
        }
    }

    public class UsbStream : Stream
    {
        private readonly UsbEndpointBase mUsbEndpoint;
        private int mTimeout = UsbConstants.DEFAULT_TIMEOUT;
        private Thread mWaitThread;

        public UsbStream(UsbEndpointBase usbEndpoint) { mUsbEndpoint = usbEndpoint; }

        #region NOT SUPPORTED

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }


        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        public override void SetLength(long value) { throw new NotSupportedException(); }

        #endregion

        #region Overridden Members

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            UsbStreamAsyncTransfer asyncTransfer = new UsbStreamAsyncTransfer(mUsbEndpoint, buffer, offset, count, callback, state, ReadTimeout);
            WaitThread.Start(asyncTransfer);
            return asyncTransfer;
        }
        private Thread WaitThread
        {
            get
            {
                if (ReferenceEquals(mWaitThread,null))
                    mWaitThread=new Thread(AsyncTransferFn);
                
                while (mWaitThread.IsAlive)Application.DoEvents();
                
                return mWaitThread;
            }
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            UsbStreamAsyncTransfer asyncTransfer = new UsbStreamAsyncTransfer(mUsbEndpoint, buffer, offset, count, callback, state, WriteTimeout);
            WaitThread.Start(asyncTransfer);
            return asyncTransfer;
        }

        public override bool CanRead
        {
            get { return (mUsbEndpoint.EpNum & 0x80) == 0x80; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return (mUsbEndpoint.EpNum & 0x80) == 0; }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            UsbStreamAsyncTransfer asyncTransfer = (UsbStreamAsyncTransfer) asyncResult;
            asyncTransfer.mCompleteEvent.WaitOne();

            if (asyncTransfer.Result == ErrorCode.Success) return asyncTransfer.TransferredLength;

            if (asyncTransfer.Result == ErrorCode.IoTimedOut)
                throw new TimeoutException(String.Format("{0}:Endpoint 0x{1:X2} IO timed out.", asyncTransfer.Result, mUsbEndpoint.EpNum));
            if (asyncTransfer.Result == ErrorCode.IoCancelled)
                throw new IOCancelledException(String.Format("{0}:Endpoint 0x{1:X2} IO was cancelled.", asyncTransfer.Result, mUsbEndpoint.EpNum));

            throw new IOException(string.Format("{0}:Failed reading from endpoint:{1}", asyncTransfer.Result, mUsbEndpoint.EpNum));
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            UsbStreamAsyncTransfer asyncTransfer = (UsbStreamAsyncTransfer) asyncResult;
            asyncTransfer.mCompleteEvent.WaitOne();

            if (asyncTransfer.Result == ErrorCode.Success && asyncTransfer.mCount == asyncTransfer.TransferredLength) return;

            if (asyncTransfer.Result == ErrorCode.IoTimedOut)
                throw new TimeoutException(String.Format("{0}:Endpoint 0x{1:X2} IO timed out.", asyncTransfer.Result, mUsbEndpoint.EpNum));
            if (asyncTransfer.Result == ErrorCode.IoCancelled)
                throw new IOCancelledException(String.Format("{0}:Endpoint 0x{1:X2} IO was cancelled.", asyncTransfer.Result, mUsbEndpoint.EpNum));
            if (asyncTransfer.mCount != asyncTransfer.TransferredLength)
                throw new IOException(String.Format("{0}:Failed writing {1} byte(s) to endpoint 0x{2:X2}.",
                                                    asyncTransfer.Result,
                                                    asyncTransfer.mCount - asyncTransfer.TransferredLength,
                                                    mUsbEndpoint.EpNum));

            throw new IOException(String.Format("{0}:Failed writing to endpoint 0x{1:X2}", asyncTransfer.Result, mUsbEndpoint.EpNum));
        }

        public override void Flush() { return; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new InvalidOperationException(String.Format("Cannot read from WriteEndpoint {0}.", (WriteEndpointID) mUsbEndpoint.EpNum));

            int transferred;
            ErrorCode ec = mUsbEndpoint.Transfer(buffer, offset, count, ReadTimeout, out transferred);

            if (ec == ErrorCode.Success) return transferred;

            if (ec == ErrorCode.IoTimedOut) throw new TimeoutException(String.Format("{0}:Endpoint 0x{1:X2} IO timed out.", ec, mUsbEndpoint.EpNum));
            if (ec == ErrorCode.IoCancelled)
                throw new IOCancelledException(String.Format("{0}:Endpoint 0x{1:X2} IO was cancelled.", ec, mUsbEndpoint.EpNum));

            throw new IOException(string.Format("{0}:Failed reading from endpoint:{1}", ec, mUsbEndpoint.EpNum));
        }

        public override int ReadTimeout
        {
            get { return mTimeout; }
            set { mTimeout = value; }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new InvalidOperationException(String.Format("Cannot write to ReadEndpoint {0}.", (ReadEndpointID) mUsbEndpoint.EpNum));

            int transferred;
            ErrorCode ec = mUsbEndpoint.Transfer(buffer, offset, count, WriteTimeout, out transferred);

            if (ec == ErrorCode.Success && count == transferred) return;

            if (ec == ErrorCode.IoTimedOut) throw new TimeoutException(String.Format("{0}:Endpoint 0x{1:X2} IO timed out.", ec, mUsbEndpoint.EpNum));
            if (ec == ErrorCode.IoCancelled)
                throw new IOCancelledException(String.Format("{0}:Endpoint 0x{1:X2} IO was cancelled.", ec, mUsbEndpoint.EpNum));
            if (count != transferred)
                throw new IOException(String.Format("{0}:Failed writing {1} byte(s) to endpoint 0x{2:X2}.",
                                                    ec,
                                                    count - transferred,
                                                    mUsbEndpoint.EpNum));

            throw new IOException(String.Format("{0}:Failed writing to endpoint 0x{1:X2}", ec, mUsbEndpoint.EpNum));
        }

        public override int WriteTimeout
        {
            get { return mTimeout; }
            set { mTimeout = value; }
        }

        #endregion

        #region STATIC Members

        private static void AsyncTransferFn(object oContext)
        {
            UsbStreamAsyncTransfer context = oContext as UsbStreamAsyncTransfer;
            context.SyncTransfer();
        }

        #endregion
    }
}