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

#pragma warning disable 169

namespace LibUsbDotNet.DeviceNotify.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal class DevBroadcastDeviceinterface : DevBroadcastHdr
    {
        public Guid ClassGuid = Guid.Empty;
        private char mNameHolder;

        public DevBroadcastDeviceinterface()
        {
            Size = Marshal.SizeOf(this);
            DeviceType = DeviceType.DeviceInterface;
        }

        public DevBroadcastDeviceinterface(Guid guid)
            : this() { ClassGuid = guid; }
    }
}