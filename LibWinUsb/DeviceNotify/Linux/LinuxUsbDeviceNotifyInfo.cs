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
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.DeviceNotify.Info;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.DeviceNotify.Linux
{
    /// <summary> Describes the USB device that caused the notification.
    /// see the <see cref="IUsbDeviceNotifyInfo"/> inteface for more information.
    /// </summary> 
    public class LinuxUsbDeviceNotifyInfo : IUsbDeviceNotifyInfo
    {
        private readonly LinuxDevItem mLinuxDevItem;

        internal LinuxUsbDeviceNotifyInfo(LinuxDevItem linuxDevItem) { mLinuxDevItem = linuxDevItem; }

        ///<summary>
        /// Gets the <see cref="UsbDeviceDescriptor"/> for the device that caused the event.
        ///</summary>
        public UsbDeviceDescriptor DeviceDescriptor
        {
            get { return mLinuxDevItem.DeviceDescriptor; }
        }

        /// <summary>
        /// Gets the bus number the device is connected to.
        /// </summary>
        public byte BusNumber
        {
            get { return mLinuxDevItem.BusNumber; }
        }

        /// <summary>
        /// Get the device instance address.
        /// </summary>
        public byte DeviceAddress
        {
            get { return mLinuxDevItem.DeviceAddress; }
        }

        #region IUsbDeviceNotifyInfo Members

        /// <summary>
        /// Not supported.  Always returns null.
        /// </summary>
        public UsbSymbolicName SymbolicName
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the name of the USB device file descriptor that caused the notification.
        /// </summary>
        public string Name
        {
            get { return mLinuxDevItem.DeviceFileName; }
        }

        /// <summary>
        /// Not supported. Always returs Guid.Empty.
        /// </summary>
        public Guid ClassGuid
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// Parses and returns the VID from the <see cref="IUsbDeviceNotifyInfo.Name"/> property.
        /// </summary>
        public int IdVendor
        {
            get { return (int)((ushort)mLinuxDevItem.DeviceDescriptor.VendorID); }
        }

        /// <summary>
        /// Parses and returns the PID from the <see cref="IUsbDeviceNotifyInfo.Name"/> property.
        /// </summary>
        public int IdProduct
        {
            get { return (int)((ushort)mLinuxDevItem.DeviceDescriptor.ProductID); }
        }

        /// <summary>
        /// Not supported.  Always returns String.Empty.
        /// </summary>
        public string SerialNumber
        {
            get { return string.Empty; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</returns>
        public override string ToString()
        {
            object[] values = new object[] {Name, BusNumber, DeviceAddress, DeviceDescriptor.ToString()};
            return string.Format("Name:{0} BusNumber:{1} DeviceAddress:{2}\n{3}", values);
        }

        #endregion
    }
}