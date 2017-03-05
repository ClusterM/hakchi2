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
using System.Diagnostics;
using System.Threading;

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// Base class for async transfer context.
    /// </summary>
    public abstract class UsbTransfer : IDisposable,IAsyncResult
    {
        private readonly UsbEndpointBase mEndpointBase;

        private IntPtr mBuffer;
        private int mCurrentOffset;
        private int mCurrentRemaining;
        private int mCurrentTransmitted;

        /// <summary></summary>
        protected int mIsoPacketSize;

        /// <summary></summary>
        protected int mOriginalCount;
        /// <summary></summary>
        protected int mOriginalOffset;
        private PinnedHandle mPinnedHandle;

        /// <summary></summary>
        protected int mTimeout;

        /// <summary></summary>
        protected bool mHasWaitBeenCalled = true;

        /// <summary></summary>
        protected readonly object mTransferLOCK = new object();

        /// <summary></summary>
        protected ManualResetEvent mTransferCancelEvent = new ManualResetEvent(false);
        /// <summary></summary>
        protected internal ManualResetEvent mTransferCompleteEvent = new ManualResetEvent(true);

        /// <summary></summary>
        protected UsbTransfer(UsbEndpointBase endpointBase) { mEndpointBase = endpointBase; }

        /// <summary>
        /// Returns the <see cref="UsbEndpointReader"/> or <see cref="UsbEndpointWriter"/> this transfer context is associated with.
        /// </summary>
        public UsbEndpointBase EndpointBase
        {
            get { return mEndpointBase; }
        }

        /// <summary>
        /// Number of bytes that will be requested for the next transfer.
        /// </summary>
        protected int RequestCount
        {
            get { return (mCurrentRemaining > UsbEndpointBase.MaxReadWrite ? UsbEndpointBase.MaxReadWrite : mCurrentRemaining); }
        }

        ///// <summary></summary>
        //protected int FailRetries
        //{
        //    get { return mFailRetries; }
        //}

        /// <summary></summary>
        protected IntPtr NextBufPtr
        {
            get { return new IntPtr(mBuffer.ToInt64() + mCurrentOffset); }
        }

        ///<summary>
        /// True if the transfer has been cacelled with <see cref="Cancel"/>.
        ///</summary>
        public bool IsCancelled
        {
            get { return mTransferCancelEvent.WaitOne(0, UsbConstants.EXIT_CONTEXT); }
        }

        /// <summary>
        /// Gets the <see cref="WaitHandle"/> for the cancel event.
        /// </summary>
        public WaitHandle CancelWaitHandle
        {
            get { return mTransferCancelEvent; }
        }

        /// <summary>
        /// Gets the size of each isochronous packet.
        /// </summary>
        /// <remarks>
        /// To change the packet size see <see cref="Fill(System.IntPtr,int,int,int,int)"/>
        /// </remarks>
        public int IsoPacketSize
        {
            get { return mIsoPacketSize; }
        }


        #region IDisposable Members

        /// <summary>
        /// Cancels any pending transfer and frees resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsCancelled) Cancel();

            int dummy;
            if (!mHasWaitBeenCalled) Wait(out dummy);
            if (mPinnedHandle != null) mPinnedHandle.Dispose();
            mPinnedHandle = null;
        }

        #endregion

        ~UsbTransfer() { Dispose(); }

        /// <summary>
        /// Cancels a pending transfer that was previously submitted with <see cref="Submit"/>.
        /// </summary>
        /// <returns></returns>
        public virtual ErrorCode Cancel()
        {
            mTransferCancelEvent.Set();
            mTransferCompleteEvent.WaitOne(5000, false);

            return ErrorCode.Success;
        }

        /// <summary>
        /// Submits the transfer.
        /// </summary>
        /// <remarks>
        /// This functions submits the USB transfer and return immediately.
        /// </remarks>
        /// <returns>
        /// <see cref="ErrorCode.Success"/> if the submit succeeds, 
        /// otherwise one of the other <see cref="ErrorCode"/> codes.
        /// </returns>
        public abstract ErrorCode Submit();

        /// <summary>
        /// Wait for the transfer to complete, timeout, or get cancelled.
        /// </summary>
        /// <param name="transferredCount">The number of bytes transferred on <see cref="ErrorCode.Success"/>.</param>
        /// <param name="cancel">If true, the transfer is cancelled if it does not complete within the time specified in <see cref="Timeout"/>.</param>
        /// <returns><see cref="ErrorCode.Success"/> if the transfer completes successfully, otherwise one of the other <see cref="ErrorCode"/> codes.</returns>
        public abstract ErrorCode Wait(out int transferredCount, bool cancel);

        /// <summary>
        /// Wait for the transfer to complete, timeout, or get cancelled.
        /// </summary>
        /// <param name="transferredCount">The number of bytes transferred on <see cref="ErrorCode.Success"/>.</param>
        /// <returns><see cref="ErrorCode.Success"/> if the transfer completes successfully, otherwise one of the other <see cref="ErrorCode"/> codes.</returns>
        public ErrorCode Wait(out int transferredCount) { return Wait(out transferredCount, true); }

        /// <summary>
        /// Fills the transfer with the data to <see cref="Submit"/>.
        /// </summary>
        /// <param name="buffer">The buffer; See <see cref="PinnedHandle"/> for more details.</param>
        /// <param name="offset">The offset on the buffer where the transfer should read/write.</param>
        /// <param name="count">The number of bytes to transfer.</param>
        /// <param name="timeout">Time (milliseconds) to wait before the transfer times out.</param>
        public virtual void Fill(object buffer, int offset, int count, int timeout)
        {
            if (mPinnedHandle != null) mPinnedHandle.Dispose();
            mPinnedHandle = new PinnedHandle(buffer);
            Fill(mPinnedHandle.Handle, offset, count, timeout);
        }
        /// <summary>
        /// Fills the transfer with the data to <see cref="Submit"/> an isochronous transfer.
        /// </summary>
        /// <param name="buffer">The buffer; See <see cref="PinnedHandle"/> for more details.</param>
        /// <param name="offset">The offset on the buffer where the transfer should read/write.</param>
        /// <param name="count">The number of bytes to transfer.</param>
        /// <param name="timeout">Time (milliseconds) to wait before the transfer times out.</param>
        /// <param name="isoPacketSize">Size of each isochronous packet.</param>
        public virtual void Fill(object buffer, int offset, int count, int timeout, int isoPacketSize)
        {
            if (mPinnedHandle != null) mPinnedHandle.Dispose();
            mPinnedHandle = new PinnedHandle(buffer);
            Fill(mPinnedHandle.Handle, offset, count, timeout, isoPacketSize);
        }
        /// <summary>
        /// Fills the transfer with the data to <see cref="Submit"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset on the buffer where the transfer should read/write.</param>
        /// <param name="count">The number of bytes to transfer.</param>
        /// <param name="timeout">Time (milliseconds) to wait before the transfer times out.</param>
        public virtual void Fill(IntPtr buffer, int offset, int count, int timeout)
        {
            mBuffer = buffer;

            mOriginalOffset = offset;
            mOriginalCount = count;
            mTimeout = timeout;
            Reset();
        }
        /// <summary>
        /// Fills the transfer with the data to <see cref="Submit"/> an isochronous transfer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset on the buffer where the transfer should read/write.</param>
        /// <param name="count">The number of bytes to transfer.</param>
        /// <param name="timeout">Time (milliseconds) to wait before the transfer times out.</param>
        /// <param name="isoPacketSize">Size of each isochronous packet.</param>
        public virtual void Fill(IntPtr buffer, int offset, int count, int timeout, int isoPacketSize)
        {
            mBuffer = buffer;

            mOriginalOffset = offset;
            mOriginalCount = count;
            mTimeout = timeout;
            mIsoPacketSize = isoPacketSize;
            Reset();
        }
        internal static ErrorCode SyncTransfer(UsbTransfer transferContext,
                                       IntPtr buffer,
                                       int offset,
                                       int length,
                                       int timeout,
                                       out int transferLength)
        {
            return SyncTransfer(transferContext, buffer, offset, length, timeout, 0, out transferLength);
        }
        internal static ErrorCode SyncTransfer(UsbTransfer transferContext,
                                               IntPtr buffer,
                                               int offset,
                                               int length,
                                               int timeout,
                                               int isoPacketSize,
                                               out int transferLength)
        {
            if (ReferenceEquals(transferContext, null)) throw new NullReferenceException("Invalid transfer context.");
            if (offset < 0) throw new ArgumentException("must be >=0", "offset");
            if (isoPacketSize == 0 && transferContext.EndpointBase.Type == EndpointType.Isochronous)
            {
                Info.UsbEndpointInfo endpointInfo = transferContext.EndpointBase.EndpointInfo;
                if (endpointInfo!=null)
                    isoPacketSize = endpointInfo.Descriptor.MaxPacketSize;
            }
            lock (transferContext.mTransferLOCK)
            {
                transferLength = 0;

                int transferred;
                ErrorCode ec;
                transferContext.Fill(buffer, offset, length, timeout, isoPacketSize);

                while (true)
                {
                    ec = transferContext.Submit();
                    if (ec != ErrorCode.Success) return ec;

                    ec = transferContext.Wait(out transferred);
                    if (ec != ErrorCode.Success) return ec;

                    transferLength += transferred;

                    if ((ec != ErrorCode.None || transferred != UsbEndpointBase.MaxReadWrite) ||
                        !transferContext.IncrementTransfer(transferred))
                        break;
                }

                return ec;
            }
        }

        /// <summary>
        /// Increments the internal counters to the next transfer batch (for transfers greater than <see cref="UsbEndpointBase.MaxReadWrite"/>)
        /// </summary>
        /// <param name="amount">This will usually be the total transferred on the previous batch.</param>
        /// <returns>True if the buffer still has data available and internal counters were successfully incremented.</returns>
        public bool IncrementTransfer(int amount)
        {
            mCurrentTransmitted += amount;
            mCurrentRemaining -= amount;
            mCurrentOffset += amount;

            if ((mCurrentRemaining) <= 0)
            {
                Debug.Assert(mCurrentRemaining == 0);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Totoal number of bytes transferred.
        /// </summary>
        public int Transmitted
        {
            get
            {
                return mCurrentTransmitted;
            }
        }

        /// <summary>
        /// Remaining bytes in the transfer data buffer.
        /// </summary>
        public int Remaining
        {
            get
            {
                return mCurrentRemaining;
            }
        }
        /// <summary>
        /// Resets the transfer to its orignal state.
        /// </summary>
        /// <remarks>
        /// Prepares a <see cref="UsbTransfer"/> to be resubmitted.
        /// </remarks>
        public void Reset()
        {
            mCurrentOffset = mOriginalOffset;
            mCurrentRemaining = mOriginalCount;
            mCurrentTransmitted = 0;

            mTransferCancelEvent.Reset();
        }

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        /// <returns>
        /// true if the operation is complete; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCompleted
        {
            get { return mTransferCompleteEvent.WaitOne(0, UsbConstants.EXIT_CONTEXT); }
        }


        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle
        {
            get { return mTransferCompleteEvent; }
        }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object AsyncState
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>
        /// true if the asynchronous operation completed synchronously; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool CompletedSynchronously
        {
            get { return false; }
        }
    }

    /// <summary>
    /// Helper class for maintaining a user defined number of outstanding aync transfers on an endpoint.
    /// </summary>
    public class UsbTransferQueue
    {
        /// <summary>
        /// Creates a new transfer queue instance.
        /// </summary>
        /// <param name="endpointBase">The endpoint to transfer data to/from.</param>
        /// <param name="maxOutstandingIO">The number of transfers to <see cref="UsbTransfer.Submit"/> before waiting for a completion.</param>
        /// <param name="bufferSize">The size of each data buffer.</param>
        /// <param name="timeout">The maximum time to wait for each transfer.</param>
        /// <param name="isoPacketSize">For isochronous use only.  The iso packet size.  If 0, the endpoints max packet size is used.</param>
        public UsbTransferQueue(UsbEndpointBase endpointBase, int maxOutstandingIO, int bufferSize, int timeout, int isoPacketSize)
        {
            EndpointBase = endpointBase;
            IsoPacketSize = isoPacketSize;
            Timeout = timeout;
            BufferSize = bufferSize;
            MaxOutstandingIO = maxOutstandingIO;

            mTransferHandles = new Handle[maxOutstandingIO];

            mBuffer = new byte[maxOutstandingIO][];
            for(int i=0; i < maxOutstandingIO; i++) 
                mBuffer[i] = new byte[bufferSize];

            IsoPacketSize = isoPacketSize > 0 ? isoPacketSize : endpointBase.EndpointInfo.Descriptor.MaxPacketSize;
        }

        /// <summary>
        /// Endpoint for I/O operations.
        /// </summary>
        public readonly UsbEndpointBase EndpointBase;

        /// <summary>
        /// Maximum outstanding I/O operations before waiting for a completion.
        /// This is also the number of data buffers allocated for this transfer queue.
        /// </summary>
        public readonly int MaxOutstandingIO;

        /// <summary>
        /// Size (in bytes) of each data buffer in this transfer queue.
        /// </summary>
        public readonly int BufferSize;

        /// <summary>
        /// Time (in milliseconds) to wait for a transfer to complete before returning <see cref="ErrorCode.IoTimedOut"/>.
        /// </summary>
        public readonly int Timeout;

        /// <summary>
        /// For isochronous use only.  The iso packet size.
        /// </summary>
        public readonly int IsoPacketSize;

        private int mOutstandingTransferCount;
        private readonly Handle[] mTransferHandles;
        private readonly byte[][] mBuffer;
        private int mTransferHandleNextIndex;
        private int mTransferHandleWaitIndex;

        /// <summary>
        /// A transfer queue handle.
        /// </summary>
        public class Handle
        {
            internal Handle(UsbTransfer context, byte[] data)
            {
                Context = context;
                Data = data;

            }

            /// <summary>
            /// Transfer context.
            /// </summary>
            public readonly UsbTransfer Context;

            /// <summary>
            /// Data buffer.
            /// </summary>
            public readonly byte[] Data;

            /// <summary>
            /// Number of bytes sent/received.
            /// </summary>
            public int Transferred;

            internal bool InUse;

        }

        /// <summary>
        /// Gets the transfer data buffer at the specified index.
        /// </summary>
        /// <param name="index">The index of the buffer to retrieve.</param>
        /// <returns>The byte array for a transfer.</returns>
        public byte[] this[int index]
        {
            get{ return mBuffer[index]; }
        }

        /// <summary>
        /// Gets a two dimensional array of data buffers. The first index represents the transfer the second represents the data buffer.
        /// </summary>
        public byte[][] Buffer
        {
            get { return mBuffer; }
        }

        private static void IncWithRoll(ref int incField, int rollOverValue)
        {
            if ((++incField) >= rollOverValue)
                incField = 0;
        }

        /// <summary>
        /// Submits transfers until <see cref="MaxOutstandingIO"/> is reached then waits for the oldest transfer to complete.  
        /// </summary>
        /// <param name="handle">The queue handle to the <see cref="UsbTransfer"/> that completed.</param>
        /// <returns><see cref="ErrorCode.Success"/> if data was transferred, or another <see cref="ErrorCode"/> on error.</returns>
        public ErrorCode Transfer(out Handle handle)
        {
            return transfer(this, out handle);
        }
        private static ErrorCode transfer(UsbTransferQueue transferParam, out Handle handle)
        {
            handle = null;
            ErrorCode ret = ErrorCode.Success;

            // Submit transfers until the maximum number of outstanding transfer(s) is reached.
            while (transferParam.mOutstandingTransferCount < transferParam.MaxOutstandingIO)
            {
                if (ReferenceEquals(transferParam.mTransferHandles[transferParam.mTransferHandleNextIndex], null))
                {
                    handle = transferParam.mTransferHandles[transferParam.mTransferHandleNextIndex] =
                        new Handle(transferParam.EndpointBase.NewAsyncTransfer(), transferParam.mBuffer[transferParam.mTransferHandleNextIndex]);

                    // Get the next available benchmark transfer handle.
                    handle.Context.Fill(handle.Data, 0, handle.Data.Length, transferParam.Timeout, transferParam.IsoPacketSize);
                }
                else
                {
                    // Get the next available benchmark transfer handle.
                    handle = transferParam.mTransferHandles[transferParam.mTransferHandleNextIndex];

                }

                handle.Transferred = 0;

                // Submit this transfer now.
                handle.Context.Reset();
                ret = handle.Context.Submit();
                if (ret != ErrorCode.Success) goto Done;

                // Mark this handle has InUse.
                handle.InUse = true;

                // When transfers ir successfully submitted, OutstandingTransferCount goes up; when
                // they are completed it goes down.
                //
                transferParam.mOutstandingTransferCount++;

                // Move TransferHandleNextIndex to the next available transfer.
                IncWithRoll(ref transferParam.mTransferHandleNextIndex, transferParam.MaxOutstandingIO);
            }

            // If the number of outstanding transfers has reached the limit, wait for the 
            // oldest outstanding transfer to complete.
            //
            if (transferParam.mOutstandingTransferCount == transferParam.MaxOutstandingIO)
            {
                // TransferHandleWaitIndex is the index of the oldest outstanding transfer.
                handle = transferParam.mTransferHandles[transferParam.mTransferHandleWaitIndex];
                ret = handle.Context.Wait(out handle.Transferred, false);
                if (ret != ErrorCode.Success)
                    goto Done;

                // Mark this handle has no longer InUse.
                handle.InUse = false;

                // When transfers ir successfully submitted, OutstandingTransferCount goes up; when
                // they are completed it goes down.
                //
                transferParam.mOutstandingTransferCount--;

                // Move TransferHandleWaitIndex to the oldest outstanding transfer.
                IncWithRoll(ref transferParam.mTransferHandleWaitIndex, transferParam.MaxOutstandingIO);

                return ErrorCode.Success;
            }

        Done:
            return ret;
        }

        /// <summary>
        /// Cancels and frees all oustanding transfers.
        /// </summary>
        public void Free()
        {
            free(this);
        }

        private static void free(UsbTransferQueue transferParam)
        {
            for (int i = 0; i < transferParam.MaxOutstandingIO; i++)
            {
                if (!ReferenceEquals(transferParam.mTransferHandles[i], null))
                {
                    if (transferParam.mTransferHandles[i].InUse)
                    {
                        if (!transferParam.mTransferHandles[i].Context.IsCompleted)
                        {
                            transferParam.EndpointBase.Abort();
                            Thread.Sleep(1);
                        }

                        transferParam.mTransferHandles[i].InUse = false;
                        transferParam.mTransferHandles[i].Context.Dispose();
                    }
                    transferParam.mTransferHandles[i] = null;
                }
            }
            transferParam.mOutstandingTransferCount = 0;
            transferParam.mTransferHandleNextIndex = 0;
            transferParam.mTransferHandleWaitIndex = 0;
        }
    }
}