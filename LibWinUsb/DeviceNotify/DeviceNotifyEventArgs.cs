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
using LibUsbDotNet.DeviceNotify.Info;

namespace LibUsbDotNet.DeviceNotify
{
    /// <summary>
    /// Describes the device notify event
    /// </summary> 
    public abstract class DeviceNotifyEventArgs : EventArgs
    {
        internal IUsbDeviceNotifyInfo mDevice;
        internal DeviceType mDeviceType;
        internal EventType mEventType;
        internal object mObject;
        internal IPortNotifyInfo mPort;
        internal IVolumeNotifyInfo mVolume;

        /// <summary>
        /// Gets the <see cref="VolumeNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="DeviceNotify.DeviceType.Volume"/>
        /// </remarks>
        public IVolumeNotifyInfo Volume
        {
            get { return mVolume; }
        }

        /// <summary>
        /// Gets the <see cref="PortNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="DeviceNotify.DeviceType.Port"/>
        /// </remarks>
        public IPortNotifyInfo Port
        {
            get { return mPort; }
        }

        /// <summary>
        /// Gets the <see cref="UsbDeviceNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="DeviceNotify.DeviceType.DeviceInterface"/>
        /// </remarks>
        public IUsbDeviceNotifyInfo Device
        {
            get { return mDevice; }
        }

        /// <summary>
        /// Gets the <see cref="EventType"/> for this notification.
        /// </summary>
        public EventType EventType
        {
            get { return mEventType; }
        }

        /// <summary>
        /// Gets the <see cref="DeviceType"/> for this notification.
        /// </summary>
        public DeviceType DeviceType
        {
            get { return mDeviceType; }
        }

        /// <summary>
        /// Gets the notification class as an object.
        /// </summary>
        /// <remarks>
        /// This value is never null.
        /// </remarks>
        public object Object
        {
            get { return mObject; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="DeviceNotifyEventArgs"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="DeviceNotifyEventArgs"/>.
        ///</returns>
        public override string ToString()
        {
            object[] o = {DeviceType, EventType, mObject.ToString()};
            return string.Format("[DeviceType:{0}] [EventType:{1}] {2}", o);
        }
    }
}