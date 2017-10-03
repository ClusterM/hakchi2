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
namespace LibUsbDotNet.DeviceNotify
{
    /// <summary> 
    /// Type of notification device.
    /// </summary> 
    public enum DeviceType
    {
        /// <summary>
        /// oem-defined device type.
        /// </summary>
        Oem = 0x00000000,
        /// <summary>
        /// devnode number.
        /// </summary>
        DevNode = 0x00000001,
        /// <summary>
        /// logical volume.
        /// </summary>
        Volume = 0x00000002,
        /// <summary>
        /// serial, parallel.
        /// </summary>
        Port = 0x00000003,
        /// <summary>
        /// network resource.
        /// </summary>
        Net = 0x00000004,
        /// <summary>
        /// device interface class
        /// </summary>
        DeviceInterface = 0x00000005,
        /// <summary>
        /// file system handle.
        /// </summary>
        Handle = 0x00000006
    }
}