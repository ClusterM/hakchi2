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
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

namespace LibUsbDotNet.Info
{
    /// <summary> Contains Endpoint information for the current <see cref="T:LibUsbDotNet.Info.UsbConfigInfo"/>.
    /// </summary> 
    public class UsbEndpointInfo : UsbBaseInfo
    {
        internal UsbEndpointDescriptor mUsbEndpointDescriptor;

        internal UsbEndpointInfo(byte[] descriptor)
        {
            mUsbEndpointDescriptor = new UsbEndpointDescriptor();
            Helper.BytesToObject(descriptor, 0, Math.Min(UsbEndpointDescriptor.Size, descriptor[0]), mUsbEndpointDescriptor);
        }

        internal UsbEndpointInfo(MonoUsbEndpointDescriptor monoUsbEndpointDescriptor) { mUsbEndpointDescriptor = new UsbEndpointDescriptor(monoUsbEndpointDescriptor); }

        /// <summary>
        /// Gets the <see cref="UsbEndpointDescriptor"/> information.
        /// </summary>
        public UsbEndpointDescriptor Descriptor
        {
            get { return mUsbEndpointDescriptor; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbEndpointInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbEndpointInfo"/>.
        ///</returns>
        public override string ToString() { return Descriptor.ToString(); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbEndpointInfo"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbEndpointInfo"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator) { return Descriptor.ToString(prefixSeperator, entitySperator, suffixSeperator); }
    }
}