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
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

#pragma warning disable 649

namespace LibUsbDotNet.Descriptors
{
    /// <summary> Usb Device Descriptor
    /// </summary> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class UsbDeviceDescriptor : UsbDescriptor
    {
        /// <summary>
        /// Total size of this structure in bytes.
        /// </summary>
        public new static readonly int Size = Marshal.SizeOf(typeof (UsbDeviceDescriptor));

        /// <summary>
        /// USB Specification Number which device complies too.
        /// </summary>
        public short BcdUsb;

        /// <summary>
        /// Class Code (Assigned by USB Org)
        /// If equal to Zero, each interface specifies it’s own class code; If equal to 0xFF, the class code is vendor specified; Otherwise field is valid Class Code.
        /// </summary>
        public ClassCodeType Class;

        /// <summary>
        /// Subclass Code (Assigned by USB Org)
        /// </summary>
        public byte SubClass;

        /// <summary>
        /// Protocol Code (Assigned by USB Org)
        /// </summary>
        public byte Protocol;

        /// <summary>
        /// Maximum Packet Size for Zero Endpoint. Valid Sizes are 8, 16, 32, 64
        /// </summary>
        public byte MaxPacketSize0;

        /// <summary>
        /// Vendor ID (Assigned by USB Org)
        /// </summary>
        public short VendorID;

        /// <summary>
        /// Product ID (Assigned by Manufacturer)
        /// </summary>
        public short ProductID;

        /// <summary>
        /// Device Release Number
        /// </summary>
        public short BcdDevice;

        /// <summary>
        /// Index of Manufacturer String Descriptor
        /// </summary>
        public byte ManufacturerStringIndex;

        /// <summary>
        /// Index of Product String Descriptor
        /// </summary>
        public byte ProductStringIndex;

        /// <summary>
        /// Index of Serial Number String Descriptor
        /// </summary>
        public byte SerialStringIndex;

        /// <summary>
        /// Number of Possible Configurations
        /// </summary>
        public byte ConfigurationCount;

        internal UsbDeviceDescriptor() { }

        internal UsbDeviceDescriptor(MonoUsbDeviceDescriptor usbDeviceDescriptor)
        {
            BcdDevice = usbDeviceDescriptor.BcdDevice;
            BcdUsb = usbDeviceDescriptor.BcdUsb;
            Class = usbDeviceDescriptor.Class;
            ConfigurationCount = usbDeviceDescriptor.ConfigurationCount;
            DescriptorType = usbDeviceDescriptor.DescriptorType;
            Length = usbDeviceDescriptor.Length;
            ManufacturerStringIndex = usbDeviceDescriptor.ManufacturerStringIndex;
            MaxPacketSize0 = usbDeviceDescriptor.MaxPacketSize0;
            ProductID = usbDeviceDescriptor.ProductID;
            ProductStringIndex = usbDeviceDescriptor.ProductStringIndex;
            Protocol = usbDeviceDescriptor.Protocol;
            SerialStringIndex = usbDeviceDescriptor.SerialStringIndex;
            SubClass = usbDeviceDescriptor.SubClass;
            VendorID = usbDeviceDescriptor.VendorID;
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbDeviceDescriptor"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbDeviceDescriptor"/>.
        ///</returns>
        public override string ToString() { return ToString("", ToStringParamValueSeperator, ToStringFieldSeperator); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbDeviceDescriptor"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbDeviceDescriptor"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator)
        {
            Object[] values = {
                                  Length, DescriptorType, "0x" + BcdUsb.ToString("X4"), Class, "0x" + SubClass.ToString("X2"),
                                  "0x" + Protocol.ToString("X2"), MaxPacketSize0,
                                  "0x" + VendorID.ToString("X4"), "0x" + ProductID.ToString("X4"), "0x" + BcdDevice.ToString("X4"),
                                  ManufacturerStringIndex, ProductStringIndex, SerialStringIndex, ConfigurationCount
                              };
            string[] names = {
                                 "Length", "DescriptorType", "BcdUsb", "Class", "SubClass", "Protocol", "MaxPacketSize0", "VendorID", "ProductID",
                                 "BcdDevice",
                                 "ManufacturerStringIndex", "ProductStringIndex", "SerialStringIndex", "ConfigurationCount"
                             };

            return Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }

        /// <summary>
        /// Determines whether the specified <see cref="UsbDeviceDescriptor"/> is equal to the current <see cref="UsbDeviceDescriptor"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="UsbDeviceDescriptor"/> is equal to the current <see cref="UsbDeviceDescriptor"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The <see cref="UsbDeviceDescriptor"/> to compare with the current <see cref="UsbDeviceDescriptor"/>. </param><exception cref="T:System.NullReferenceException">The <paramref name="other"/> parameter is null.</exception><filterpriority>2</filterpriority>
        public bool Equals(UsbDeviceDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.BcdUsb == BcdUsb && other.Class == Class && other.SubClass == SubClass && other.Protocol == Protocol &&
                   other.MaxPacketSize0 == MaxPacketSize0 && other.VendorID == VendorID && other.ProductID == ProductID &&
                   other.BcdDevice == BcdDevice && other.ManufacturerStringIndex == ManufacturerStringIndex &&
                   other.ProductStringIndex == ProductStringIndex && other.SerialStringIndex == SerialStringIndex &&
                   other.ConfigurationCount == ConfigurationCount;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (UsbDeviceDescriptor)) return false;
            return Equals((UsbDeviceDescriptor) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = BcdUsb.GetHashCode();
                result = (result*397) ^ Class.GetHashCode();
                result = (result*397) ^ SubClass.GetHashCode();
                result = (result*397) ^ Protocol.GetHashCode();
                result = (result*397) ^ MaxPacketSize0.GetHashCode();
                result = (result*397) ^ VendorID.GetHashCode();
                result = (result*397) ^ ProductID.GetHashCode();
                result = (result*397) ^ BcdDevice.GetHashCode();
                result = (result*397) ^ ManufacturerStringIndex.GetHashCode();
                result = (result*397) ^ ProductStringIndex.GetHashCode();
                result = (result*397) ^ SerialStringIndex.GetHashCode();
                result = (result*397) ^ ConfigurationCount.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(UsbDeviceDescriptor left, UsbDeviceDescriptor right) { return Equals(left, right); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UsbDeviceDescriptor left, UsbDeviceDescriptor right) { return !Equals(left, right); }
    }
}