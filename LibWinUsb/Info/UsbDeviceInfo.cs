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
using System.Globalization;
using System.Runtime.InteropServices;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

namespace LibUsbDotNet.Info
{
    /// <summary> Contains USB device descriptor information.
    /// </summary> 
    public class UsbDeviceInfo
    {
        private const short NO_LANG = short.MaxValue;
        private readonly UsbDeviceDescriptor mDeviceDescriptor;
        private short mCurrentCultureLangID = NO_LANG;
        private String mManufacturerString;
        private String mProductString;
        private String mSerialString;
        internal UsbDevice mUsbDevice;

        internal UsbDeviceInfo(UsbDevice usbDevice)
        {
            mUsbDevice = usbDevice;
            GetDeviceDescriptor(mUsbDevice, out mDeviceDescriptor);
        }

        internal UsbDeviceInfo(UsbDevice usbDevice, MonoUsbDeviceDescriptor usbDeviceDescriptor)
        {
            mUsbDevice = usbDevice;

            mDeviceDescriptor = new UsbDeviceDescriptor();
            mDeviceDescriptor.BcdDevice = usbDeviceDescriptor.BcdDevice;
            mDeviceDescriptor.BcdUsb = usbDeviceDescriptor.BcdUsb;
            mDeviceDescriptor.Class = usbDeviceDescriptor.Class;
            mDeviceDescriptor.ConfigurationCount = usbDeviceDescriptor.ConfigurationCount;
            mDeviceDescriptor.DescriptorType = usbDeviceDescriptor.DescriptorType;
            mDeviceDescriptor.Length = usbDeviceDescriptor.Length;
            mDeviceDescriptor.ManufacturerStringIndex = usbDeviceDescriptor.ManufacturerStringIndex;
            mDeviceDescriptor.MaxPacketSize0 = usbDeviceDescriptor.MaxPacketSize0;
            mDeviceDescriptor.ProductID = usbDeviceDescriptor.ProductID;
            mDeviceDescriptor.ProductStringIndex = usbDeviceDescriptor.ProductStringIndex;
            mDeviceDescriptor.Protocol = usbDeviceDescriptor.Protocol;
            mDeviceDescriptor.SerialStringIndex = usbDeviceDescriptor.SerialStringIndex;
            mDeviceDescriptor.SubClass = usbDeviceDescriptor.SubClass;
            mDeviceDescriptor.VendorID = usbDeviceDescriptor.VendorID;
        }

        /// <summary>
        /// The raw <see cref="UsbDeviceDescriptor"/> for the current <see cref="UsbDevice"/>.
        /// </summary>
        public UsbDeviceDescriptor Descriptor
        {
            get { return mDeviceDescriptor; }
        }

        /// <summary>
        /// Request all available languages from the USB device (string index 0) and return the most appropriate LCID given the current operating systems locale settings. See System.Globalization.CultureInfo.CurrentCulture.LCID.
        /// </summary>
        /// <remarks>
        /// Once the USB devices CurrentCultureLangID has been retreived, subsequent request will return a cached copy of the LCID.
        /// </remarks>
        public short CurrentCultureLangID
        {
            get
            {
                if (mCurrentCultureLangID == NO_LANG)
                {
                    short currentCultureLangID = (short) CultureInfo.CurrentCulture.LCID;
                    short[] deviceLangIDs;
                    if (mUsbDevice.GetLangIDs(out deviceLangIDs))
                    {
                        foreach (short deviceLangID in deviceLangIDs)
                        {
                            if (deviceLangID == currentCultureLangID)
                            {
                                mCurrentCultureLangID = deviceLangID;
                                return mCurrentCultureLangID;
                            }
                        }
                    }
                    mCurrentCultureLangID = deviceLangIDs.Length > 0 ? deviceLangIDs[0] : (short) 0;
                }
                return mCurrentCultureLangID;
            }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="UsbDeviceDescriptor.ManufacturerStringIndex"/> string index.
        /// </summary>
        public String ManufacturerString
        {
            get
            {
                if (ReferenceEquals(mManufacturerString, null))
                {
                    mManufacturerString = String.Empty;
                    if (Descriptor.ManufacturerStringIndex > 0)
                    {
                        mUsbDevice.GetString(out mManufacturerString, CurrentCultureLangID, Descriptor.ManufacturerStringIndex);
                    }
                }
                return mManufacturerString;
            }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="UsbDeviceDescriptor.ProductStringIndex"/> string index.
        /// </summary>
        public String ProductString
        {
            get
            {
                if (ReferenceEquals(mProductString, null))
                {
                    mProductString = String.Empty;
                    if (Descriptor.ProductStringIndex > 0)
                    {
                        mUsbDevice.GetString(out mProductString, CurrentCultureLangID, Descriptor.ProductStringIndex);
                    }
                }
                return mProductString;
            }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="UsbDeviceDescriptor.SerialStringIndex"/> string index.
        /// </summary>
        public String SerialString
        {
            get
            {
                if (ReferenceEquals(mSerialString, null))
                {
                    mSerialString = String.Empty;
                    if (Descriptor.SerialStringIndex > 0)
                    {
                        mUsbDevice.GetString(out mSerialString, 0x0409, Descriptor.SerialStringIndex);
                    }
                }
                return mSerialString;
            }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbInterfaceInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbInterfaceInfo"/>.
        ///</returns>
        public override string ToString() { return ToString("", UsbDescriptor.ToStringParamValueSeperator, UsbDescriptor.ToStringFieldSeperator); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbInterfaceInfo"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbInterfaceInfo"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator)
        {
            string[] names = {"ManufacturerString", "ProductString", "SerialString"};
            Object[] values = {ManufacturerString, ProductString, SerialString};
            return Descriptor.ToString(prefixSeperator, entitySperator, suffixSeperator) +
                   Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }

        internal static bool GetDeviceDescriptor(UsbDevice usbDevice, out UsbDeviceDescriptor deviceDescriptor)
        {
            if (usbDevice.mCachedDeviceDescriptor!=null)
            {
                deviceDescriptor = usbDevice.mCachedDeviceDescriptor;
                return true;
            }
            deviceDescriptor = new UsbDeviceDescriptor();

            GCHandle gcDeviceDescriptor = GCHandle.Alloc(deviceDescriptor, GCHandleType.Pinned);
            int ret;
            bool bSuccess = usbDevice.GetDescriptor((byte) DescriptorType.Device,
                                                    0,
                                                    0,
                                                    gcDeviceDescriptor.AddrOfPinnedObject(),
                                                    UsbDeviceDescriptor.Size,
                                                    out ret);
            gcDeviceDescriptor.Free();

            if (bSuccess) return true;

            return false;
        }
    }
}