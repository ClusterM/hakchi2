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
using System.Windows.Forms;
using LibUsbDotNet.Internal;
using LibUsbDotNet.Main;

namespace LibUsbDotNet
{
    /// <summary>
    /// Contains methods for retrieving data from a <see cref="EndpointType.Bulk"/> or <see cref="EndpointType.Interrupt"/> endpoint using the overloaded <see cref="Read(byte[],int,out int)"/> functions or a <see cref="DataReceived"/> event.
    /// </summary> 
    /// <remarks>
    /// <list type="bullet">
    /// <item>Before using the <see cref="DataReceived"/> event, the <see cref="DataReceivedEnabled"/> property must be set to true.</item>
    /// <item>While the <see cref="DataReceivedEnabled"/> property is True, the overloaded <see cref="Read(byte[],int,out int)"/> functions cannot be used.</item>
    /// </list>
    /// </remarks>
    public class UsbEndpointReader : UsbEndpointBase
    {
        private static int mDefReadBufferSize = 4096;

        private bool mDataReceivedEnabled;
        private int mReadBufferSize;
        private Thread mReadThread;
        private ThreadPriority mReadThreadPriority = ThreadPriority.Normal;

        internal UsbEndpointReader(UsbDevice usbDevice, int readBufferSize, ReadEndpointID readEndpointID, EndpointType endpointType)
            : base(usbDevice, (Byte) readEndpointID, endpointType) { mReadBufferSize = readBufferSize; }

        /// <summary>
        /// Default read buffer size when using the <see cref="DataReceived"/> event.
        /// </summary>
        /// <remarks>
        /// This value can be bypassed using the second parameter of the <see cref="UsbDevice.OpenEndpointReader(LibUsbDotNet.Main.ReadEndpointID,int)"/> method.
        /// The default is 4096.
        /// </remarks>
        public static int DefReadBufferSize
        {
            get { return mDefReadBufferSize; }
            set { mDefReadBufferSize = value; }
        }

        /// <summary>
        /// Gets/Sets a value indicating if the <see cref="UsbEndpointReader.DataReceived"/> event should be used.
        /// </summary>
        /// <remarks>
        /// If DataReceivedEnabled is true the <see cref="Read(byte[] , int , int , int, out int )"/> functions cannot be used.
        /// </remarks>
        public virtual bool DataReceivedEnabled
        {
            get { return mDataReceivedEnabled; }
            set
            {
                if (value != mDataReceivedEnabled)
                {
                    StartStopReadThread();
                }
            }
        }


        /// <summary>
        /// Size of the read buffer in bytes for the <see cref="UsbEndpointReader.DataReceived"/> event.
        /// </summary>
        /// <remarks>
        /// Setting a large values, for example 64K will yield a lower number of <see cref="UsbEndpointReader.DataReceived"/> and a higher data rate. 
        /// </remarks>
        public int ReadBufferSize
        {
            get { return mReadBufferSize; }
            set { mReadBufferSize = value; }
        }

        /// <summary>
        /// Gets/Sets the Priority level for the read thread when <see cref="DataReceivedEnabled"/> is true.
        /// </summary>
        public ThreadPriority ReadThreadPriority
        {
            get { return mReadThreadPriority; }
            set { mReadThreadPriority = value; }
        }


        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Read(byte[] buffer, int timeout, out int transferLength) { return Read(buffer, 0, buffer.Length, timeout, out transferLength); }

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="count">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Read(IntPtr buffer, int offset, int count, int timeout, out int transferLength) { return Transfer(buffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="count">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Read(byte[] buffer, int offset, int count, int timeout, out int transferLength) { return Transfer(buffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="count">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Read(object buffer, int offset, int count, int timeout, out int transferLength) { return Transfer(buffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Read(object buffer, int timeout, out int transferLength) { return Transfer(buffer, 0, Marshal.SizeOf(buffer), timeout, out transferLength); }

        /// <summary>
        /// Reads/discards data from the enpoint until no more data is available.
        /// </summary>
        /// <returns>Alwats returns <see cref="ErrorCode.None"/> </returns>
        public virtual ErrorCode ReadFlush()
        {
            byte[] bufDummy = new byte[64];
            int iTransferred;
            int iBufCount = 0;
            while (Read(bufDummy, 10, out iTransferred) == ErrorCode.None && iBufCount < 128)
            {
                iBufCount++;
            }

            return ErrorCode.None;
        }


        private static void ReadData(object context)
        {
            UsbTransfer overlappedTransferContext = (UsbTransfer) context;
            UsbEndpointReader reader = (UsbEndpointReader) overlappedTransferContext.EndpointBase;
            reader.mDataReceivedEnabled = true;
            EventHandler<DataReceivedEnabledChangedEventArgs> dataReceivedEnabledChangedEvent;

            dataReceivedEnabledChangedEvent = reader.DataReceivedEnabledChanged;
            if (!ReferenceEquals(dataReceivedEnabledChangedEvent,null))
                dataReceivedEnabledChangedEvent(reader, new DataReceivedEnabledChangedEventArgs(reader.mDataReceivedEnabled));

            overlappedTransferContext.Reset();

            byte[] buf = new byte[reader.mReadBufferSize];
            try
            {
                while (!overlappedTransferContext.IsCancelled)
                {
                    int iTransferLength;
                    ErrorCode eReturn = reader.Transfer(buf, 0, buf.Length, Timeout.Infinite, out iTransferLength);
                    if (eReturn == ErrorCode.None)
                    {
                        EventHandler<EndpointDataEventArgs> temp = reader.DataReceived;
                        if (!ReferenceEquals(temp, null) && !overlappedTransferContext.IsCancelled)
                        {
                            temp(reader, new EndpointDataEventArgs(buf, iTransferLength));
                        }
                        continue;
                    }
                    if (eReturn != ErrorCode.IoTimedOut) break;
                }
            }
            catch (ThreadAbortException)
            {
                UsbError.Error(ErrorCode.ReceiveThreadTerminated,0, "ReadData:Read thread aborted.", reader);
            }
            finally
            {
                reader.Abort();
                reader.mDataReceivedEnabled = false;

                dataReceivedEnabledChangedEvent = reader.DataReceivedEnabledChanged;
                if (!ReferenceEquals(dataReceivedEnabledChangedEvent, null))
                    dataReceivedEnabledChangedEvent(reader, new DataReceivedEnabledChangedEventArgs(reader.mDataReceivedEnabled));

            }
        }

        private void StartReadThread()
        {
            mReadThread = new Thread(ReadData);
            mReadThread.Priority = ReadThreadPriority;
            mReadThread.Start(TransferContext);
            Thread.Sleep(1);
            Application.DoEvents();
        }

        private bool StopReadThread()
        {
            Abort();
            Thread.Sleep(1);
            Application.DoEvents();
            DateTime dtStart = DateTime.Now;
            while (mReadThread.IsAlive && ((DateTime.Now - dtStart).TotalSeconds < 5)) // 5 sec fail-safe
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            if (mReadThread.IsAlive)
            {
                UsbError.Error(ErrorCode.ReceiveThreadTerminated,0, "Failed stopping read thread.", this);
                mReadThread.Abort();
                return false;
            }
            return true;
        }

        private void StartStopReadThread()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().FullName);

            if (mDataReceivedEnabled)
            {
                StopReadThread();
            }
            else
            {
                StartReadThread();
            }
        }


        /// <summary>
        /// The DataReceived Event is fired when new data arrives for the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <remarks>To use the DataReceived event, <see cref="DataReceivedEnabled"/> must be set to truw.</remarks>
        public virtual event EventHandler<EndpointDataEventArgs> DataReceived;
       
        /// <summary>
        /// The <see cref="DataReceivedEnabledChanged"/> Event is fired when the <see cref="DataReceived"/> event is started or stopped.
        /// </summary>
        public virtual event EventHandler<DataReceivedEnabledChangedEventArgs> DataReceivedEnabledChanged;

        internal override UsbTransfer CreateTransferContext() { return new OverlappedTransferContext(this); }
    }
}