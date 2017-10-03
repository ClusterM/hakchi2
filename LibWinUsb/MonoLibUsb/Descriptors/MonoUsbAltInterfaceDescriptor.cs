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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibUsbDotNet.Descriptors;

namespace MonoLibUsb.Descriptors
{
    /// <summary>
    /// A structure representing the standard USB interface descriptor. This
    /// descriptor is documented in section 9.6.5 of the USB 2.0 specification.
    /// All multiple-byte fields are represented in host-endian format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = MonoUsbApi.LIBUSB_PACK)]
    public class MonoUsbAltInterfaceDescriptor
    {
        ///<summary>Size of this descriptor (in bytes)</summary>
        public readonly Byte bLength;

        ///<summary>Descriptor type. Will have value LIBUSB_DT_INTERFACE in this context.</summary>
        public readonly DescriptorType bDescriptorType;

        ///<summary>Number of this interface</summary>
        public readonly Byte bInterfaceNumber;

        ///<summary>Value used to select this alternate setting for this interface</summary>
        public readonly Byte bAlternateSetting;

        ///<summary> Number of endpoints used by this interface (excluding the control endpoint).</summary>
        public readonly Byte bNumEndpoints;

        ///<summary> USB-IF class code for this interface. See ClassCodeType.</summary>
        public readonly ClassCodeType bInterfaceClass;

        ///<summary> USB-IF subclass code for this interface, qualified by the bInterfaceClass value</summary>
        public readonly Byte bInterfaceSubClass;

        ///<summary> USB-IF protocol code for this interface, qualified by the bInterfaceClass and bInterfaceSubClass values</summary>
        public readonly Byte bInterfaceProtocol;

        ///<summary> Index of string descriptor describing this interface</summary>
        public readonly Byte iInterface;

        ///<summary> Array of endpoint descriptors. This length of this array is determined by the bNumEndpoints field.</summary>
        private readonly IntPtr pEndpointDescriptors;

        ///<summary> Extra descriptors. If libusb encounters unknown interface descriptors, it will store them here, should you wish to parse them.</summary>
        private IntPtr pExtraBytes;

        ///<summary> Length of the extra descriptors, in bytes.</summary>
        public readonly int ExtraLength;

        ///<summary> Extra descriptors. If libusb encounters unknown interface descriptors, it will store them here, should you wish to parse them.</summary>
        public byte[] ExtraBytes
        {
            get
            {
                byte[] bytes = new byte[ExtraLength];
                Marshal.Copy(pExtraBytes, bytes, 0, bytes.Length);
                return bytes;
            }
        }

        ///<summary> Array of endpoint descriptors. This length of this array is determined by the bNumEndpoints field.</summary>
        public List<MonoUsbEndpointDescriptor> EndpointList
        {
            get
            {
                List<MonoUsbEndpointDescriptor> endpointList = new List<MonoUsbEndpointDescriptor>();
                int iEndpoint;
                for (iEndpoint = 0; iEndpoint < bNumEndpoints; iEndpoint++)
                {
                    IntPtr pNextInterface = new IntPtr(pEndpointDescriptors.ToInt64() + (Marshal.SizeOf(typeof (MonoUsbEndpointDescriptor))*iEndpoint));
                    MonoUsbEndpointDescriptor monoUsbEndpoint = new MonoUsbEndpointDescriptor();
                    Marshal.PtrToStructure(pNextInterface, monoUsbEndpoint);
                    endpointList.Add(monoUsbEndpoint);
                }

                return endpointList;
            }
        }
    }
}