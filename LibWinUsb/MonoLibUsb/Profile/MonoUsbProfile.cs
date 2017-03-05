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

using LibUsbDotNet;
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// Representing a USB device that can be opened and used by Libusb-1.0.
    /// </summary>
    public class MonoUsbProfile
    {
        private readonly byte mBusNumber;
        private readonly byte mDeviceAddress;
        private readonly MonoUsbDeviceDescriptor mMonoUsbDeviceDescriptor = new MonoUsbDeviceDescriptor();
        private readonly MonoUsbProfileHandle mMonoUSBProfileHandle;
        internal bool mDiscovered;

        internal MonoUsbProfile(MonoUsbProfileHandle monoUSBProfileHandle)
        {
            mMonoUSBProfileHandle = monoUSBProfileHandle;
            mBusNumber = MonoUsbApi.GetBusNumber(mMonoUSBProfileHandle);
            mDeviceAddress = MonoUsbApi.GetDeviceAddress(mMonoUSBProfileHandle);
            GetDeviceDescriptor(out mMonoUsbDeviceDescriptor);

//#if DEBUG
//            Console.WriteLine("Vid:{0:X4} Pid:{1:X4} BusNumber:{2} DeviceAddress:{3}",
//                              mMonoUsbDeviceDescriptor.VendorID,
//                              mMonoUsbDeviceDescriptor.ProductID,
//                              mBusNumber,
//                              mDeviceAddress);
//#endif
        }

        /// <summary>
        /// Gets the standard usb device descriptor.
        /// </summary>
        public MonoUsbDeviceDescriptor DeviceDescriptor
        {
            get { return mMonoUsbDeviceDescriptor; }
        }

        /// <summary>
        /// Gets the bus number the is resides on.
        /// </summary>
        public byte BusNumber
        {
            get { return mBusNumber; }
        }

        /// <summary>
        /// Gets the device address that belongs to the usb device this profile represents.
        /// </summary>
        public byte DeviceAddress
        {
            get { return mDeviceAddress; }
        }

        /// <summary>
        /// Gets the internal profile handle need for some api calls.
        /// </summary>
        public MonoUsbProfileHandle ProfileHandle
        {
            get { return mMonoUSBProfileHandle; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MonoUsbProfile)) return false;
            return Equals((MonoUsbProfile) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (mBusNumber.GetHashCode()*397) ^ mDeviceAddress.GetHashCode();
            }
        }

        ///<summary>
        /// <c>true</c> if the <see cref="MonoUsbProfile"/> types are equal.
        ///</summary>
        /// <remarks>
        /// <see cref="MonoUsbProfile"/> types are considered equal they have the same <see cref="BusNumber"/> and <see cref="DeviceAddress"/>.
        /// </remarks>
        ///<param name="left"><see cref="MonoUsbProfile"/> on the left.</param>
        ///<param name="right"><see cref="MonoUsbProfile"/> on the right.</param>
        ///<returns>True if the <see cref="MonoUsbProfile"/> types are equal.</returns>
        public static bool operator ==(MonoUsbProfile left, MonoUsbProfile right) { return Equals(left, right); }

        ///<summary>
        /// <c>true</c> if the <see cref="MonoUsbProfile"/> types are not equal.
        ///</summary>
        /// <remarks>
        /// <see cref="MonoUsbProfile"/> types are considered equal they have the same <see cref="BusNumber"/> and <see cref="DeviceAddress"/>.
        /// </remarks>
        ///<param name="left"><see cref="MonoUsbProfile"/> on the left.</param>
        ///<param name="right"><see cref="MonoUsbProfile"/> on the right.</param>
        ///<returns>True if the <see cref="MonoUsbProfile"/> types are not equal.</returns>
        public static bool operator !=(MonoUsbProfile left, MonoUsbProfile right) { return !Equals(left, right); }

        private MonoUsbError GetDeviceDescriptor(out MonoUsbDeviceDescriptor monoUsbDeviceDescriptor)
        {
            MonoUsbError ec = MonoUsbError.Success;

            monoUsbDeviceDescriptor = new MonoUsbDeviceDescriptor();
            //Console.WriteLine("MonoUsbProfile:GetDeviceDescriptor");
            ec = (MonoUsbError) MonoUsbApi.GetDeviceDescriptor(mMonoUSBProfileHandle, monoUsbDeviceDescriptor);
            if (ec != MonoUsbError.Success)
            {
#if LIBUSBDOTNET
                UsbError.Error(ErrorCode.MonoApiError, (int) ec, "GetDeviceDescriptor Failed", this);
#endif
                monoUsbDeviceDescriptor = null;
            }
            return ec;
        }

        /// <summary>
        /// Closes the internal <see cref="MonoUsbProfileHandle"/>.
        /// </summary>
        public void Close()
        {
            if (!mMonoUSBProfileHandle.IsClosed)
                mMonoUSBProfileHandle.Close();
        }

        /// <summary>
        /// Convenience function to open the device handle this profile handle represents.
        /// </summary>
        /// <returns>
        /// A new <see cref="MonoUsbDeviceHandle"/> instance. Created with <see cref="MonoUsbDeviceHandle(MonoUsbProfileHandle)"/> constructor.
        /// </returns>
        public MonoUsbDeviceHandle OpenDeviceHandle() { return new MonoUsbDeviceHandle(ProfileHandle); }

        /// <summary>
        /// Compares a <see cref="MonoUsbProfile"/> with this one.
        /// </summary>
        /// <param name="other">The other <see cref="MonoUsbProfile"/>.</param>
        /// <returns>True if the <see cref="BusNumber"/> and <see cref="DeviceAddress"/> are equal.</returns>
        public bool Equals(MonoUsbProfile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.mBusNumber == mBusNumber && other.mDeviceAddress == mDeviceAddress;
        }
    }
}