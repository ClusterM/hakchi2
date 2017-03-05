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

namespace LibUsbDotNet.Main
{
    /// <summary> Transfers data to the main control endpoint (Endpoint 0).
    /// </summary> 
    /// <remarks> All USB devices respond to requests from the host on the device’s Default Control Pipe. These requests are made using control transfers. The request and the request’s parameters are sent to the device in the Setup packet. The host is responsible for establishing the values passed in the fields. Every Setup packet has eight bytes.
    /// </remarks> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UsbSetupPacket
    {
        /// <summary>
        /// This bitmapped field identifies the characteristics of the specific request. In particular, this field identifies the direction of data transfer in the second phase of the control transfer. The state of the Direction bit is ignored if the wLength field is zero, signifying there is no Data stage.
        /// The USB Specification defines a series of standard requests that all devices must support. In addition, a device class may define additional requests. A device vendor may also define requests supported by the device.
        /// Requests may be directed to the device, an interface on the device, or a specific endpoint on a device. This field also specifies the intended recipient of the request. When an interface or endpoint is specified, the wIndex field identifies the interface or endpoint.
        /// </summary>
        /// <remarks>
        /// <ul>Characteristics of request:
        /// <li>D7: Data transfer direction</li>
        /// <li>0 = Host-to-device</li>
        /// <li>1 = Device-to-host</li>
        /// <li>D6...5: Type</li>
        /// <li>0 = Standard</li>
        /// <li>1 = Class</li>
        /// <li>2 = Vendor</li>
        /// <li>3 = Reserved</li>
        /// <li>D4...0: Recipient</li>
        /// <li>0 = Device</li>
        /// <li>1 = Interface</li>
        /// <li>2 = Endpoint</li>
        /// <li>3 = Other</li>
        /// <li>4...31 = Reserved</li>
        /// </ul>
        /// </remarks>
        public byte RequestType;

        /// <summary>
        /// This field specifies the particular request. The Type bits in the bmRequestType field modify the meaning of this field. This specification defines values for the bRequest field only when the bits are reset to zero, indicating a standard request.
        /// </summary>
        public byte Request;

        /// <summary>
        /// The contents of this field vary according to the request. It is used to pass a parameter to the device, specific to the request.
        /// </summary>
        public short Value;

        /// <summary>
        /// The contents of this field vary according to the request. It is used to pass a parameter to the device, specific to the request.
        /// </summary>
        public short Index;

        /// <summary>
        /// This field specifies the length of the data transferred during the second phase of the control transfer. The direction of data transfer (host-to-device or device-to-host) is indicated by the Direction bit of the <see cref="RequestType"/> field. If this field is zero, there is no data transfer phase. On an input request, a device must never return more data than is indicated by the wLength value; it may return less. On an output request, wLength will always indicate the exact amount of data to be sent by the host. Device behavior is undefined if the host should send more data than is specified in wLength.
        /// </summary>
        public short Length;

        /// <summary>
        /// Creates a new instance of a <see cref="UsbSetupPacket"/> and initializes all the fields with the following parameters.
        /// </summary>
        /// <param name="requestType">See <see cref="UsbSetupPacket.RequestType"/>.</param>
        /// <param name="request">See <see cref="UsbSetupPacket.Request"/>.</param>
        /// <param name="value">See <see cref="UsbSetupPacket.Value"/>.</param>
        /// <param name="index">See <see cref="UsbSetupPacket.Index"/>.</param>
        /// <param name="length">See <see cref="UsbSetupPacket.Length"/>.</param>
        public UsbSetupPacket(byte requestType, byte request, short value, short index, short length)
        {
            RequestType = requestType;
            Request = request;
            Value = value;
            Index = index;
            Length = length;
        }
    }
}