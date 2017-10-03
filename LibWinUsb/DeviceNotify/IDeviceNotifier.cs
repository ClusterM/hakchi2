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

namespace LibUsbDotNet.DeviceNotify
{
    /// <summary>
    /// Notifies an application of a change to the hardware Configuration of a device or 
    /// the computer.
    /// </summary>
    /// <remarks>
    /// For devices that offer software-controllable features, such as ejection and locking, 
    /// the system typically sends a <see cref="EventType.DeviceRemovePending"/> message to 
    /// let applications and device drivers end their use of the device gracefully. If the 
    /// system forcibly removes a device, it may not send a 
    /// <see cref="EventType.DeviceQueryRemove"/> message before doing so.
    /// </remarks>
    /// <example>
    /// <code source="..\Test_DeviceNotify\fTestDeviceNotify.cs" lang="cs"/>
    /// </example>
    public interface IDeviceNotifier
    {
        ///<summary>
        /// Enables/Disables notification events.
        ///</summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Main Notify event for all device notifications.
        /// </summary>
        event EventHandler<DeviceNotifyEventArgs> OnDeviceNotify;
    }
}