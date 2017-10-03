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
using LibUsbDotNet.Internal;
using LibUsbDotNet.Main;

namespace LibUsbDotNet
{
    /// <summary>Contains methods for writing data to a <see cref="EndpointType.Bulk"/> or <see cref="EndpointType.Interrupt"/> endpoint using the overloaded <see cref="Write(byte[],int,out int)"/> functions.
    /// </summary> 
    public class UsbEndpointWriter : UsbEndpointBase
    {
        internal UsbEndpointWriter(UsbDevice usbDevice, WriteEndpointID writeEndpointID, EndpointType endpointType)
            : base(usbDevice, (byte)writeEndpointID, endpointType) { }


        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Write(byte[] buffer, int timeout, out int transferLength) { return Write(buffer, 0, buffer.Length, timeout, out transferLength); }

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="pBuffer">The buffer storing the data to write.</param>
        /// <param name="offset">The position in buffer to start writing the data from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Write(IntPtr pBuffer, int offset, int count, int timeout, out int transferLength) { return Transfer(pBuffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="offset">The position in buffer to start writing the data from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Write(byte[] buffer, int offset, int count, int timeout, out int transferLength) { return Transfer(buffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="offset">The position in buffer to start writing the data from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Write(object buffer, int offset, int count, int timeout, out int transferLength) { return Transfer(buffer, offset, count, timeout, out transferLength); }

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <param name="transferLength">Number of bytes actually transferred.</param>
        /// <returns>
        /// <see cref="ErrorCode"/>.<see cref="ErrorCode.None"/> on success.
        /// </returns>
        public virtual ErrorCode Write(object buffer, int timeout, out int transferLength) { return Write(buffer, 0, Marshal.SizeOf(buffer), timeout, out transferLength); }

        internal override UsbTransfer CreateTransferContext() { return new OverlappedTransferContext(this); }
    }
}