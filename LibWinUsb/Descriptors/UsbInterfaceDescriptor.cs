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
    /// <summary> Usb Interface Descriptor.
    /// </summary> 
    /// <remarks> This is the actual descriptor as described in the USB 2.0 Specifications.
    /// </remarks> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class UsbInterfaceDescriptor : UsbDescriptor
    {
        /// <summary>
        /// Total size of this structure in bytes.
        /// </summary>
        public new static readonly int Size = Marshal.SizeOf(typeof (UsbInterfaceDescriptor));

        /// <summary>
        /// Number of Interface
        /// </summary>
        public readonly byte InterfaceID;

        /// <summary>
        /// Value used to select alternative setting
        /// </summary>
        public readonly byte AlternateID;

        /// <summary>
        /// Number of Endpoints used for this interface
        /// </summary>
        public readonly byte EndpointCount;

        /// <summary>
        /// Class Code (Assigned by USB Org)
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
        /// Index of String Descriptor Describing this interface
        /// </summary>
        public readonly byte StringIndex;

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbInterfaceDescriptor"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbInterfaceDescriptor"/>.
        ///</returns>
        public override string ToString() { return ToString("", ToStringParamValueSeperator, ToStringFieldSeperator); }


        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbInterfaceDescriptor"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbInterfaceDescriptor"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator)
        {
            Object[] values = {
                                  Length, DescriptorType, InterfaceID, AlternateID, EndpointCount, Class, "0x" + SubClass.ToString("X2"),
                                  "0x" + Protocol.ToString("X2"), StringIndex
                              };
            string[] names = {
                                 "Length", "DescriptorType", "InterfaceID", "AlternateID", "EndpointCount", "Class", "SubClass", "Protocol",
                                 "StringIndex"
                             };

            return Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }


        internal UsbInterfaceDescriptor() { }

        internal UsbInterfaceDescriptor(MonoUsbAltInterfaceDescriptor altInterfaceDescriptor)
        {
            AlternateID = altInterfaceDescriptor.bAlternateSetting;
            Class = altInterfaceDescriptor.bInterfaceClass;
            DescriptorType = altInterfaceDescriptor.bDescriptorType;
            EndpointCount = altInterfaceDescriptor.bNumEndpoints;
            InterfaceID = altInterfaceDescriptor.bInterfaceNumber;
            Length = altInterfaceDescriptor.bLength;
            Protocol = altInterfaceDescriptor.bInterfaceProtocol;
            StringIndex = altInterfaceDescriptor.iInterface;
            SubClass = altInterfaceDescriptor.bInterfaceSubClass;
        }
    }
}