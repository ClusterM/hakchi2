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

#pragma warning disable 649

namespace LibUsbDotNet.Descriptors
{
    /// <summary> Base class for all usb descriptors structures.
    /// </summary> 
    /// <remarks> This is the actual descriptor as described in the USB 2.0 Specifications.
    /// </remarks> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class UsbDescriptor
    {
        /// <summary>
        /// String value used to seperate the name/value pairs for all ToString overloads of the descriptor classes.
        /// </summary>
        public static string ToStringParamValueSeperator = ":";

        /// <summary>
        /// String value used to seperate the name/value groups for all ToString overloads of the descriptor classes.
        /// </summary>
        public static string ToStringFieldSeperator = "\r\n";

        /// <summary>
        /// Total size of this structure in bytes.
        /// </summary>
        public static readonly int Size = Marshal.SizeOf(typeof (UsbDescriptor));

        /// <summary>
        /// Length of structure reported by the associated usb device.
        /// </summary>
        public byte Length;

        /// <summary>
        /// Type of structure reported by the associated usb device.
        /// </summary>
        public DescriptorType DescriptorType;

        /// <summary>
        /// String representation of the UsbDescriptor class.
        /// </summary>
        public override string ToString()
        {
            Object[] values = {Length, DescriptorType};
            string[] names = {"Length", "DescriptorType"};

            return Helper.ToString("", names, ToStringParamValueSeperator, values, ToStringFieldSeperator);
        }
    }
}