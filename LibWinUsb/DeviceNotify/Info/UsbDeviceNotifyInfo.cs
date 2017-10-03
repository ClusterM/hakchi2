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
using LibUsbDotNet.DeviceNotify.Internal;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary> Describes the USB device that caused the notification.
    /// See the <see cref="IUsbDeviceNotifyInfo"/> inteface for more information.
    /// </summary>  
    public class UsbDeviceNotifyInfo : IUsbDeviceNotifyInfo
    {
        private readonly DevBroadcastDeviceinterface mBaseHdr = new DevBroadcastDeviceinterface();
        private readonly string mName;
        private UsbSymbolicName mSymbolicName;

        internal UsbDeviceNotifyInfo(IntPtr lParam)
        {
            Marshal.PtrToStructure(lParam, mBaseHdr);
            IntPtr pName = new IntPtr(lParam.ToInt64() + Marshal.OffsetOf(typeof (DevBroadcastDeviceinterface), "mNameHolder").ToInt64());
            mName = Marshal.PtrToStringAuto(pName);
        }

        #region IUsbDeviceNotifyInfo Members

        /// <summary>
        /// The symbolc name class for this device.  For more information, see <see cref="UsbSymbolicName"/>.
        /// </summary>
        public UsbSymbolicName SymbolicName
        {
            get
            {
                if (ReferenceEquals(mSymbolicName, null))
                    mSymbolicName = new UsbSymbolicName(mName);

                return mSymbolicName;
            }
        }

        /// <summary>
        /// Gets the full name of the USB device that caused the notification.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the Class Guid of the USB device that caused the notification.
        /// </summary>
        public Guid ClassGuid
        {
            get { return SymbolicName.ClassGuid; }
        }

        /// <summary>
        /// Parses and returns the VID from the <see cref="Name"/> property.
        /// </summary>
        public int IdVendor
        {
            get { return SymbolicName.Vid; }
        }

        /// <summary>
        /// Parses and returns the PID from the <see cref="Name"/> property.
        /// </summary>
        public int IdProduct
        {
            get { return SymbolicName.Pid; }
        }

        /// <summary>
        /// Parses and returns the serial number from the <see cref="Name"/> property.
        /// </summary>
        public string SerialNumber
        {
            get { return SymbolicName.SerialNumber; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbDeviceNotifyInfo"/>.
        ///</returns>
        public override string ToString() { return SymbolicName.ToString(); }

        #endregion
    }
}