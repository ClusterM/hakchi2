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
    ///<summary> Device and/or Interface Class codes</summary>
    [Flags]
    public enum ClassCodeType : byte
    {
        ///<summary>In the context of a "device descriptor", this bDeviceClass value indicates that each interface specifies its own class information and all interfaces operate independently.</summary>
        PerInterface = 0,

        ///<summary>Audio class</summary>
        Audio = 1,

        ///<summary> Communications class</summary>
        Comm = 2,

        ///<summary> Human Interface Device class</summary>
        Hid = 3,

        ///<summary> Printer dclass</summary>
        Printer = 7,

        ///<summary> Picture transfer protocol class</summary>
        Ptp = 6,

        ///<summary> Mass storage class</summary>
        MassStorage = 8,

        ///<summary> Hub class</summary>
        Hub = 9,

        ///<summary> Data class</summary>
        Data = 10,

        ///<summary> Class is vendor-specific</summary>
        VendorSpec = 0xff
    }
}