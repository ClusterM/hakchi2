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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Main;
using LibUsbDotNet.LudnMonoLibUsb;
using MonoLibUsb.Descriptors;

namespace LibUsbDotNet.Info
{
    /// <summary> Contains all Configuration information for the current <see cref="T:LibUsbDotNet.UsbDevice"/>.
    /// </summary> 
    public class UsbConfigInfo : UsbBaseInfo
    {
        private readonly List<UsbInterfaceInfo> mInterfaceList = new List<UsbInterfaceInfo>();
        internal readonly UsbConfigDescriptor mUsbConfigDescriptor;
        private String mConfigString;
        internal UsbDevice mUsbDevice;

        internal UsbConfigInfo(UsbDevice usbDevice, UsbConfigDescriptor descriptor, ref List<byte[]> rawDescriptors)
        {
            mUsbDevice = usbDevice;
            mUsbConfigDescriptor = descriptor;
            mRawDescriptors = rawDescriptors;

            UsbInterfaceInfo currentInterface = null;
            for (int iRawDescriptor = 0; iRawDescriptor < rawDescriptors.Count; iRawDescriptor++)
            {
                byte[] bytesRawDescriptor = rawDescriptors[iRawDescriptor];

                switch (bytesRawDescriptor[1])
                {
                    case (byte) DescriptorType.Interface:
                        currentInterface = new UsbInterfaceInfo(usbDevice, bytesRawDescriptor);
                        mRawDescriptors.RemoveAt(iRawDescriptor);
                        mInterfaceList.Add(currentInterface);
                        iRawDescriptor--;
                        break;
                    case (byte) DescriptorType.Endpoint:
                        if (currentInterface == null)
                            throw new UsbException(this, "Recieved and endpoint descriptor before receiving an interface descriptor.");

                        currentInterface.mEndpointInfo.Add(new UsbEndpointInfo(bytesRawDescriptor));
                        mRawDescriptors.RemoveAt(iRawDescriptor);
                        iRawDescriptor--;
                        break;
                    default:
                        if (currentInterface != null)
                        {
                            currentInterface.mRawDescriptors.Add(bytesRawDescriptor);
                            mRawDescriptors.RemoveAt(iRawDescriptor);
                            iRawDescriptor--;
                        }
                        break;
                }
            }
        }

        internal UsbConfigInfo(MonoUsbDevice usbDevice, MonoUsbConfigDescriptor configDescriptor)
        {
            mUsbDevice = usbDevice;

            mUsbConfigDescriptor = new UsbConfigDescriptor(configDescriptor);

            List<MonoUsbInterface> monoUSBInterfaces = configDescriptor.InterfaceList;
            foreach (MonoUsbInterface usbInterface in monoUSBInterfaces)
            {
                List<MonoUsbAltInterfaceDescriptor> monoUSBAltInterfaces = usbInterface.AltInterfaceList;
                foreach (MonoUsbAltInterfaceDescriptor monoUSBAltInterface in monoUSBAltInterfaces)
                {
                    UsbInterfaceInfo usbInterfaceInfo = new UsbInterfaceInfo(mUsbDevice, monoUSBAltInterface);
                    mInterfaceList.Add(usbInterfaceInfo);
                }
            }
        }

        /// <summary>
        /// Gets the actual <see cref="UsbConfigDescriptor"/> for the current config.
        /// </summary>
        public UsbConfigDescriptor Descriptor
        {
            get { return mUsbConfigDescriptor; }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="UsbConfigDescriptor.StringIndex"/> string index.
        /// </summary>
        public String ConfigString
        {
            get
            {
                if (ReferenceEquals(mConfigString, null))
                {
                    mConfigString = String.Empty;
                    if (Descriptor.StringIndex > 0)
                    {
                        mUsbDevice.GetString(out mConfigString, mUsbDevice.Info.CurrentCultureLangID, Descriptor.StringIndex);
                    }
                }
                return mConfigString;
            }
        }

        /// <summary>
        /// Gets the collection of USB device interfaces associated with this <see cref="UsbConfigInfo"/> instance.
        /// </summary>
        public ReadOnlyCollection<UsbInterfaceInfo> InterfaceInfoList
        {
            get { return mInterfaceList.AsReadOnly(); }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbConfigInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbConfigInfo"/>.
        ///</returns>
        public override string ToString() { return ToString("", UsbDescriptor.ToStringParamValueSeperator, UsbDescriptor.ToStringFieldSeperator); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbConfigInfo"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbConfigInfo"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator)
        {
            Object[] values = {ConfigString};
            string[] names = {"ConfigString"};
            return Descriptor.ToString(prefixSeperator, entitySperator, suffixSeperator) +
                   Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }
    }
}