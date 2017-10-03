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
using LibUsbDotNet.Descriptors;

namespace MonoLibUsb.Descriptors
{
    /// <summary>
    /// A structure representing the standard USB endpoint descriptor. This
    /// descriptor is documented in section 9.6.3 of the USB 2.0 specification.
    /// All multiple-byte fields are represented in host-endian format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = MonoUsbApi.LIBUSB_PACK)]
    public class MonoUsbEndpointDescriptor
    {
        ///<summary> Size of this descriptor (in bytes)</summary>
        public readonly Byte bLength;

        ///<summary> Descriptor type. Will have value LIBUSB_DT_ENDPOINT in this context.</summary>
        public readonly DescriptorType bDescriptorType;

        ///<summary> The address of the endpoint described by this descriptor. Bits 0:3 are the endpoint number. Bits 4:6 are reserved. Bit 7 indicates direction, see \ref libusb_endpoint_direction.</summary>
        public readonly Byte bEndpointAddress;

        ///<summary> Attributes which apply to the endpoint when it is configured using the bConfigurationValue. Bits 0:1 determine the transfer type and correspond to \ref libusb_transfer_type. Bits 2:3 are only used for isochronous endpoints and correspond to \ref libusb_iso_sync_type. Bits 4:5 are also only used for isochronous endpoints and correspond to \ref libusb_iso_usage_type. Bits 6:7 are reserved.</summary>
        public readonly Byte bmAttributes;

        ///<summary> Maximum packet size this endpoint is capable of sending/receiving.</summary>
        public readonly short wMaxPacketSize;

        ///<summary> Interval for polling endpoint for data transfers.</summary>
        public readonly Byte bInterval;

        ///<summary> For audio devices only: the rate at which synchronization feedback is provided.</summary>
        public readonly Byte bRefresh;

        ///<summary> For audio devices only: the address if the synch endpoint</summary>
        public readonly Byte bSynchAddress;

        ///<summary> Extra descriptors. If libusb encounters unknown endpoint descriptors, it will store them here, should you wish to parse them.</summary>
        private readonly IntPtr pExtraBytes;

        ///<summary> Length of the extra descriptors, in bytes.</summary>
        public readonly int ExtraLength;

        ///<summary> Extra descriptors. If libusb encounters unknown endpoint descriptors, it will store them here, should you wish to parse them.</summary>
        public byte[] ExtraBytes
        {
            get
            {
                byte[] bytes = new byte[ExtraLength];
                Marshal.Copy(pExtraBytes, bytes, 0, bytes.Length);
                return bytes;
            }
        }
    }
}