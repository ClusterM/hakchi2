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
using LibUsbDotNet.Internal;
using LibUsbDotNet.Main;
using LibUsbDotNet.LudnMonoLibUsb.Internal;
using MonoLibUsb;

namespace LibUsbDotNet.LudnMonoLibUsb
{
    /// <summary>
    /// Implements mono-linux libusb 1.x methods for reading data from a <see cref="EndpointType.Bulk"/> or <see cref="EndpointType.Interrupt"/> endpoints.
    /// </summary> 
    public class MonoUsbEndpointReader : UsbEndpointReader
    {
        private MonoUsbTransferContext mMonoTransferContext;

        internal MonoUsbEndpointReader(UsbDevice usbDevice, int readBufferSize, ReadEndpointID readEndpointID, EndpointType endpointType)
            : base(usbDevice, readBufferSize, readEndpointID, endpointType) { }

        /// <summary>
        /// Frees resources associated with the endpoint.  Once disposed this class cannot be used.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (ReferenceEquals(mMonoTransferContext, null)) return;
            mMonoTransferContext.Dispose();
            mMonoTransferContext = null;
        }

        /// <summary>
        /// Calling this methods is that same as calling <see cref="UsbEndpointReader.ReadFlush"/>
        /// </summary>
        /// <returns>True an success.</returns>
        public override bool Flush()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().Name);
            return ReadFlush() == ErrorCode.Success;
        }

        /// <summary>
        /// Cancels pending transfers and clears the halt condition on an enpoint.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Reset()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().Name);
            Abort();
            int ret = MonoUsbApi.ClearHalt((MonoUsbDeviceHandle) Device.Handle, EpNum);
            if (ret < 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "Endpoint Reset Failed", this);
                return false;
            }
            return true;
        }


        internal override UsbTransfer CreateTransferContext() { return new MonoUsbTransferContext(this); }
    }
}