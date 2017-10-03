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
    /// Type of notification event.
    /// </summary> 
    public enum EventType
    {
        /// <summary>
        /// A custom event has occurred.
        /// </summary>
        CustomEvent = 0x8006,
        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        DeviceArrival = 0x8000,
        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        DeviceQueryRemove = 0x8001,
        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        DeviceQueryRemoveFailed = 0x8002,
        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        DeviceRemoveComplete = 0x8004,
        /// <summary>
        /// A device or piece of media is about to be removed. Cannot be denied.
        /// </summary>
        DeviceRemovePending = 0x8003,
        /// <summary>
        /// A device-specific event has occurred.
        /// </summary>
        DeviceTypeSpecific = 0x8005
    }
}