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

namespace LibUsbDotNet.DeviceNotify.Linux
{
    /// <summary>
    /// Describes the device notify event
    /// </summary> 
    public class LinuxDeviceNotifyEventArgs : DeviceNotifyEventArgs
    {
        internal LinuxDeviceNotifyEventArgs(LinuxDevItem linuxDevItem, DeviceType deviceType, EventType eventType)
        {
            mEventType = eventType;
            mDeviceType = deviceType;
            switch (mDeviceType)
            {
                case DeviceType.Volume:
                    throw new NotImplementedException(mDeviceType.ToString());
                case DeviceType.Port:
                    throw new NotImplementedException(mDeviceType.ToString());
                case DeviceType.DeviceInterface:
                    mDevice = new LinuxUsbDeviceNotifyInfo(linuxDevItem);
                    mObject = mDevice;
                    break;
            }
        }

        //internal LinuxDeviceNotifyEventArgs(DevBroadcastHdr hdr, IntPtr ptrHdr, EventType eventType)
        //{
        //    mBaseHdr = hdr;
        //    mEventType = eventType;
        //    mDeviceType = mBaseHdr.DeviceType;
        //    switch (mDeviceType)
        //    {
        //        case DeviceType.Volume:
        //            mVolume = new VolumeNotifyInfo(ptrHdr);
        //            mObject = mVolume;
        //            break;
        //        case DeviceType.Port:
        //            mPort = new PortNotifyInfo(ptrHdr);
        //            mObject = mPort;
        //            break;
        //        case DeviceType.DeviceInterface:
        //            mDevice = new UsbDeviceNotifyInfo(ptrHdr);
        //            mObject = mDevice;
        //            break;
        //    }
        //}
    }
}