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
using System.Runtime.InteropServices;
using LibUsbDotNet.Descriptors;
using MonoLibUsb.Descriptors;

namespace LibUsbDotNet.DeviceNotify.Linux
{
    internal class LinuxDevItem
    {
        public readonly byte BusNumber;
        public readonly byte DeviceAddress;
        public readonly UsbDeviceDescriptor DeviceDescriptor;
        public readonly string DeviceFileName;

        public LinuxDevItem(string deviceFileName, byte busNumber, byte deviceAddress, byte[] fileDescriptor)
        {
            DeviceFileName = deviceFileName;
            BusNumber = busNumber;
            DeviceAddress = deviceAddress;


            DeviceDescriptor = new UsbDeviceDescriptor();
            GCHandle gcFileDescriptor = GCHandle.Alloc(DeviceDescriptor, GCHandleType.Pinned);
            Marshal.Copy(fileDescriptor, 0, gcFileDescriptor.AddrOfPinnedObject(), Marshal.SizeOf(DeviceDescriptor));

            gcFileDescriptor.Free();
        }

        public LinuxDevItem(string deviceFileName, byte busNumber, byte deviceAddress, MonoUsbDeviceDescriptor monoUsbDeviceDescriptor)
        {
            DeviceFileName = deviceFileName;
            BusNumber = busNumber;
            DeviceAddress = deviceAddress;


            DeviceDescriptor = new UsbDeviceDescriptor(monoUsbDeviceDescriptor);
        }

        public bool Equals(LinuxDevItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DeviceFileName, DeviceFileName) && other.BusNumber == BusNumber && other.DeviceAddress == DeviceAddress &&
                   Equals(other.DeviceDescriptor, DeviceDescriptor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LinuxDevItem)) return false;
            return Equals((LinuxDevItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (DeviceFileName != null ? DeviceFileName.GetHashCode() : 0);
                result = (result*397) ^ BusNumber.GetHashCode();
                result = (result*397) ^ DeviceAddress.GetHashCode();
                result = (result*397) ^ (DeviceDescriptor != null ? DeviceDescriptor.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(LinuxDevItem left, LinuxDevItem right) { return Equals(left, right); }
        public static bool operator !=(LinuxDevItem left, LinuxDevItem right) { return !Equals(left, right); }
    }
}