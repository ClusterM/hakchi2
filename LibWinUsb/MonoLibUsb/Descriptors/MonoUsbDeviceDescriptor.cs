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
using LibUsbDotNet.Main;

namespace MonoLibUsb.Descriptors
{
    ///<summary>A structure representing the standard USB device descriptor. 
    ///This descriptor is documented in section 9.6.1 of the USB 2.0 specification. 
    ///All multiple-byte fields are represented in host-endian format.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = MonoUsbApi.LIBUSB_PACK)]
    public class MonoUsbDeviceDescriptor
    {
        /// <summary>
        /// Total size of this structure in bytes.
        /// </summary>
        public static readonly int Size = Marshal.SizeOf(typeof (MonoUsbDeviceDescriptor));

        /// <summary>
        /// Length of structure reported by the associated usb device.
        /// </summary>
        public byte Length;

        /// <summary>
        /// Type of structure reported by the associated usb device.
        /// </summary>
        public DescriptorType DescriptorType;

        /// <summary>
        /// USB Specification Number which device complies too.
        /// </summary>
        public readonly short BcdUsb;

        /// <summary>
        /// Class Code (Assigned by USB Org)
        /// If equal to Zero, each interface specifies it’s own class code; If equal to 0xFF, the class code is vendor specified; Otherwise field is valid Class Code.
        /// </summary>
        public readonly ClassCodeType Class;

        /// <summary>
        /// Subclass Code (Assigned by USB Org)
        /// </summary>
        public readonly byte SubClass;

        /// <summary>
        /// Protocol Code (Assigned by USB Org)
        /// </summary>
        public readonly byte Protocol;

        /// <summary>
        /// Maximum Packet Size for Zero Endpoint. Valid Sizes are 8, 16, 32, 64
        /// </summary>
        public readonly byte MaxPacketSize0;

        /// <summary>
        /// Vendor ID (Assigned by USB Org)
        /// </summary>
        public readonly short VendorID;

        /// <summary>
        /// Product ID (Assigned by Manufacturer)
        /// </summary>
        public readonly short ProductID;

        /// <summary>
        /// Device Release Number
        /// </summary>
        public readonly short BcdDevice;

        /// <summary>
        /// Index of Manufacturer String Descriptor
        /// </summary>
        public readonly byte ManufacturerStringIndex;

        /// <summary>
        /// Index of Product String Descriptor
        /// </summary>
        public readonly byte ProductStringIndex;

        /// <summary>
        /// Index of Serial Number String Descriptor
        /// </summary>
        public readonly byte SerialStringIndex;

        /// <summary>
        /// Number of Possible Configurations
        /// </summary>
        public readonly byte ConfigurationCount;

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="MonoUsbDeviceDescriptor"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="MonoUsbDeviceDescriptor"/>.
        ///</returns>
        public override string ToString()
        {
            Object[] values = {
                                  Length, DescriptorType, "0x" + BcdUsb.ToString("X4"), Class, SubClass, Protocol, MaxPacketSize0,
                                  "0x" + VendorID.ToString("X4"), "0x" + ProductID.ToString("X4"), BcdDevice,
                                  ManufacturerStringIndex, ProductStringIndex, SerialStringIndex, ConfigurationCount
                              };
            string[] names = {
                                 "Length", "DescriptorType", "BcdUsb", "Class", "SubClass", "Protocol", "MaxPacketSize0", "VendorID", "ProductID",
                                 "BcdDevice",
                                 "ManufacturerStringIndex", "ProductStringIndex", "SerialStringIndex", "ConfigurationCount"
                             };
            return Helper.ToString("", names, ":", values, "\r\n");

        }
    }
}