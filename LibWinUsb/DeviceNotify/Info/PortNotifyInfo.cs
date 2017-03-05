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

namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary> Notify information for a communication port
    /// </summary> 
    public class PortNotifyInfo : IPortNotifyInfo
    {
        private readonly DevBroadcastPort mBaseHdr = new DevBroadcastPort();
        private readonly string mName;

        internal PortNotifyInfo(IntPtr lParam)
        {
            Marshal.PtrToStructure(lParam, mBaseHdr);
            IntPtr pName = new IntPtr(lParam.ToInt64() + Marshal.OffsetOf(typeof (DevBroadcastPort), "mNameHolder").ToInt64());
            mName = Marshal.PtrToStringAuto(pName);
        }

        #region IPortNotifyInfo Members

        /// <summary>
        /// Gets the name of the port that caused the event.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="PortNotifyInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="PortNotifyInfo"/>.
        ///</returns>
        public override string ToString() { return string.Format("[Port Name:{0}] ", Name); }

        #endregion
    }
}