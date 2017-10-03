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
using LibUsbDotNet.Main;

namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary> Common inteface describing USB DEVICE INTERFACE arrival and removal events.
    /// </summary> 
    public interface IUsbDeviceNotifyInfo
    {
        /// <summary>
        /// The symbolc name class for this device.  For more information, see <see cref="UsbSymbolicName"/>.
        /// </summary>
        UsbSymbolicName SymbolicName { get; }

        /// <summary>
        /// Gets the full name of the USB device that caused the notification.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Class Guid of the USB device that caused the notification.
        /// </summary>
        Guid ClassGuid { get; }

        /// <summary>
        /// Parses and returns the VID from the <see cref="Name"/> property.
        /// </summary>
        int IdVendor { get; }

        /// <summary>
        /// Parses and returns the PID from the <see cref="Name"/> property.
        /// </summary>
        int IdProduct { get; }

        /// <summary>
        /// Parses and returns the serial number from the <see cref="Name"/> property.
        /// </summary>
        string SerialNumber { get; }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</returns>
        string ToString();
    }
}