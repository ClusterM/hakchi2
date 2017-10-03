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
using MonoLibUsb.Descriptors;

namespace LibUsbDotNet.Info
{
    /// <summary> Describes a USB device interface.
    /// </summary> 
    public class UsbInterfaceInfo : UsbBaseInfo
    {
        internal readonly UsbInterfaceDescriptor mUsbInterfaceDescriptor;
        internal List<UsbEndpointInfo> mEndpointInfo = new List<UsbEndpointInfo>();
        private String mInterfaceString;
        internal UsbDevice mUsbDevice;

        internal UsbInterfaceInfo(UsbDevice usbDevice, byte[] descriptor)
        {
            mUsbDevice = usbDevice;
            mUsbInterfaceDescriptor = new UsbInterfaceDescriptor();
            Helper.BytesToObject(descriptor, 0, Math.Min(UsbInterfaceDescriptor.Size, descriptor[0]), mUsbInterfaceDescriptor);
        }

        internal UsbInterfaceInfo(UsbDevice usbDevice, MonoUsbAltInterfaceDescriptor monoUSBAltInterfaceDescriptor)
        {
            mUsbDevice = usbDevice;

            mUsbInterfaceDescriptor = new UsbInterfaceDescriptor(monoUSBAltInterfaceDescriptor);
            List<MonoUsbEndpointDescriptor> monoUsbEndpoints = monoUSBAltInterfaceDescriptor.EndpointList;
            foreach (MonoUsbEndpointDescriptor monoUSBEndpoint in monoUsbEndpoints)
            {
                mEndpointInfo.Add(new UsbEndpointInfo(monoUSBEndpoint));
            }
        }

        /// <summary>
        /// Gets the actual interface descriptor.
        /// </summary>
        public UsbInterfaceDescriptor Descriptor
        {
            get { return mUsbInterfaceDescriptor; }
        }

        /// <summary>
        /// Gets the collection of endpoint descriptors associated with this interface.
        /// </summary>
        public ReadOnlyCollection<UsbEndpointInfo> EndpointInfoList
        {
            get { return mEndpointInfo.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the string representation of the <see cref="UsbInterfaceDescriptor.StringIndex"/> string index.
        /// </summary>
        public String InterfaceString
        {
            get
            {
                if (ReferenceEquals(mInterfaceString, null))
                {
                    mInterfaceString = String.Empty;
                    if (Descriptor.StringIndex > 0)
                    {
                        mUsbDevice.GetString(out mInterfaceString, mUsbDevice.Info.CurrentCultureLangID, Descriptor.StringIndex);
                    }
                }
                return mInterfaceString;
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
            Object[] values = {InterfaceString};
            string[] names = {"InterfaceString"};
            return Descriptor.ToString(prefixSeperator, entitySperator, suffixSeperator) +
                   Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }
    }
}