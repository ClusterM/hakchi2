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

namespace LibUsbDotNet.Descriptors
{
    /// <summary> Standard USB descriptor types.
    /// </summary> 
    [Flags]
    public enum DescriptorType : byte
    {
        /// <summary>
        /// Device descriptor type.
        /// </summary>
        Device = 1,
        /// <summary>
        /// Configuration descriptor type.
        /// </summary>
        Configuration = 2,
        /// <summary>
        /// String descriptor type.
        /// </summary>
        String = 3,
        /// <summary>
        /// Interface descriptor type.
        /// </summary>
        Interface = 4,
        /// <summary>
        /// Endpoint descriptor type.
        /// </summary>
        Endpoint = 5,
        /// <summary>
        /// Device Qualifier descriptor type.
        /// </summary>
        DeviceQualifier = 6,
        /// <summary>
        /// Other Speed Configuration descriptor type.
        /// </summary>
        OtherSpeedConfiguration = 7,
        /// <summary>
        /// Interface Power descriptor type.
        /// </summary>
        InterfacePower = 8,
        /// <summary>
        /// OTG descriptor type.
        /// </summary>
        OTG = 9,
        /// <summary>
        /// Debug descriptor type.
        /// </summary>
        Debug = 10,
        /// <summary>
        /// Interface Association descriptor type.
        /// </summary>
        InterfaceAssociation = 11,

        ///<summary> HID descriptor</summary>
        Hid = 0x21,

        ///<summary> HID report descriptor</summary>
        HidReport = 0x22,

        ///<summary> Physical descriptor</summary>
        Physical = 0x23,

        ///<summary> Hub descriptor</summary>
        Hub = 0x29
    }
}