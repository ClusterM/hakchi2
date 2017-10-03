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
using LibUsbDotNet.Main;
using MonoLibUsb.Descriptors;

#pragma warning disable 649

namespace LibUsbDotNet.Descriptors
{
    /// <summary> Usb Endpoint Descriptor
    /// </summary> 
    /// <remarks> This is the actual descriptor as described in the USB 2.0 Specifications.
    /// </remarks> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class UsbEndpointDescriptor : UsbDescriptor
    {
        /// <summary>
        /// Total size of this structure in bytes.
        /// </summary>
        public new static readonly int Size = Marshal.SizeOf(typeof (UsbEndpointDescriptor));

        /// <summary>
        /// Endpoint Address
        /// Bits 0..3b Endpoint Number.
        /// Bits 4..6b Reserved. Set to Zero
        /// Bits 7 Direction 0 = Out, 1 = In (Ignored for Control Endpoints)
        /// </summary>
        public readonly byte EndpointID;

        /// <summary>
        /// Bits 0..1 Transfer Type 
        /// 00 = Control
        /// 01 = Isochronous
        /// 10 = Bulk
        /// 11 = Interrupt
        /// 
        /// Bits 2..7 are reserved. If Isochronous endpoint, 
        /// Bits 3..2 = Synchronisation Type (Iso Mode) 
        /// 00 = No Synchonisation
        /// 01 = Asynchronous
        /// 10 = Adaptive
        /// 11 = Synchronous
        /// 
        /// Bits 5..4 = Usage Type (Iso Mode) 
        /// 00 = Data Endpoint
        /// 01 = Feedback Endpoint
        /// 10 = Explicit Feedback Data Endpoint
        /// 11 = Reserved
        /// </summary>
        public readonly byte Attributes;

        /// <summary>
        /// Maximum Packet Size this endpoint is capable of sending or receiving
        /// </summary>
        public readonly short MaxPacketSize;

        /// <summary>
        /// Interval for polling endpoint data transfers. Value in frame counts. Ignored for Bulk and Control Endpoints. Isochronous must equal 1 and field may range from 1 to 255 for interrupt endpoints.
        /// </summary>
        public readonly byte Interval;

        /// <summary>
        /// Audio endpoint specific.
        /// </summary>
        public readonly byte Refresh;

        /// <summary>
        /// Audio endpoint specific.
        /// </summary>
        public readonly byte SynchAddress;

        internal UsbEndpointDescriptor() { }

        internal UsbEndpointDescriptor(MonoUsbEndpointDescriptor descriptor)
        {
            Attributes = descriptor.bmAttributes;
            DescriptorType = descriptor.bDescriptorType;
            EndpointID = descriptor.bEndpointAddress;
            Interval = descriptor.bInterval;
            Length = descriptor.bLength;
            MaxPacketSize = (short) descriptor.wMaxPacketSize;
            SynchAddress = descriptor.bSynchAddress;
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbEndpointDescriptor"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbEndpointDescriptor"/>.
        ///</returns>
        public override string ToString() { return ToString("", ToStringParamValueSeperator, ToStringFieldSeperator); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbEndpointDescriptor"/>.
        ///</summary>
        ///
        ///<param name="prefixSeperator">The field prefix string.</param>
        ///<param name="entitySperator">The field/value seperator string.</param>
        ///<param name="suffixSeperator">The value suffix string.</param>
        ///<returns>A formatted representation of the <see cref="UsbEndpointDescriptor"/>.</returns>
        public string ToString(string prefixSeperator, string entitySperator, string suffixSeperator)
        {
            Object[] values = {
                                  Length, DescriptorType, "0x" + EndpointID.ToString("X2"), "0x" + Attributes.ToString("X2"), MaxPacketSize, Interval,
                                  Refresh, "0x" + SynchAddress.ToString("X2")
                              };
            string[] names = {"Length", "DescriptorType", "EndpointID", "Attributes", "MaxPacketSize", "Interval", "Refresh", "SynchAddress"};

            return Helper.ToString(prefixSeperator, names, entitySperator, values, suffixSeperator);
        }
    }
}