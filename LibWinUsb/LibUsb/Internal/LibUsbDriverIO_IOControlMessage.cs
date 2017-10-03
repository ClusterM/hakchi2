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

namespace LibUsbDotNet.Internal.LibUsb
{
    internal partial class LibUsbDriverIO
    {
        public const int EINVAL = 22;

        internal static bool ControlTransferEx(SafeHandle interfaceHandle,
                                               UsbSetupPacket setupPacket,
                                               IntPtr buffer,
                                               int bufferLength,
                                               out int lengthTransferred,
                                               int timeout)
        {
            lengthTransferred = 0;
            LibUsbRequest req = new LibUsbRequest();

            req.Timeout = timeout;
            req.Control.RequestType = (byte) setupPacket.RequestType;
            req.Control.Request = (byte) setupPacket.Request;
            req.Control.Value = (ushort) setupPacket.Value;
            req.Control.Index = (ushort) setupPacket.Index;
            req.Control.Length = (ushort) setupPacket.Length;

            /* in request? */
            Byte[] reqBytes = req.Bytes;
            Byte[] inBytes = reqBytes;
            if ((setupPacket.RequestType & (byte) UsbEndpointDirection.EndpointIn) == 0)
            {
                inBytes = new byte[LibUsbRequest.Size + bufferLength];
                reqBytes.CopyTo(inBytes, 0);
                if (buffer != IntPtr.Zero) Marshal.Copy(buffer, inBytes, LibUsbRequest.Size, bufferLength);

                buffer = IntPtr.Zero;
                bufferLength = 0;
            }

            if (UsbIOSync(interfaceHandle, LibUsbIoCtl.CONTROL_TRANSFER, inBytes, inBytes.Length, buffer, bufferLength, out lengthTransferred))
            {
                /* in request? */
                if ((setupPacket.RequestType & (byte)UsbEndpointDirection.EndpointIn) == 0)
                    lengthTransferred = (inBytes.Length - reqBytes.Length);

                return true;
            }
            return false;
        }

        internal static bool ControlTransfer(SafeHandle interfaceHandle,
                                             UsbSetupPacket setupPacket,
                                             IntPtr buffer,
                                             int bufferLength,
                                             out int lengthTransferred,
                                             int timeout)
        {
            lengthTransferred = 0;
            LibUsbRequest req = new LibUsbRequest();
            int code;

            req.Timeout = timeout;

            switch ((UsbRequestType)(setupPacket.RequestType &  (0x03 << 5)))
            {
                case UsbRequestType.TypeStandard:
                    switch ((UsbStandardRequest)setupPacket.Request)
                    {
                        case UsbStandardRequest.GetStatus:
                            req.Status.Recipient = (int) setupPacket.RequestType & 0x1F;
                            req.Status.Index = setupPacket.Index;
                            code = LibUsbIoCtl.GET_STATUS;
                            break;

                        case UsbStandardRequest.ClearFeature:
                            req.Feature.Recipient = (int) setupPacket.RequestType & 0x1F;
                            req.Feature.ID = setupPacket.Value;
                            req.Feature.Index = setupPacket.Index;
                            code = LibUsbIoCtl.CLEAR_FEATURE;
                            break;

                        case UsbStandardRequest.SetFeature:
                            req.Feature.Recipient = (int) setupPacket.RequestType & 0x1F;
                            req.Feature.ID = setupPacket.Value;
                            req.Feature.Index = setupPacket.Index;
                            code = LibUsbIoCtl.SET_FEATURE;
                            break;

                        case UsbStandardRequest.GetDescriptor:
                            req.Descriptor.Recipient = (int) setupPacket.RequestType & 0x1F;
                            req.Descriptor.Type = (setupPacket.Value >> 8) & 0xFF;
                            req.Descriptor.Index = setupPacket.Value & 0xFF;
                            req.Descriptor.LangID = setupPacket.Index;
                            code = LibUsbIoCtl.GET_DESCRIPTOR;
                            break;

                        case UsbStandardRequest.SetDescriptor:
                            req.Descriptor.Recipient = (int) setupPacket.RequestType & 0x1F;
                            req.Descriptor.Type = (setupPacket.Value >> 8) & 0xFF;
                            req.Descriptor.Index = setupPacket.Value & 0xFF;
                            req.Descriptor.LangID = setupPacket.Index;
                            code = LibUsbIoCtl.SET_DESCRIPTOR;
                            break;

                        case UsbStandardRequest.GetConfiguration:
                            code = LibUsbIoCtl.GET_CONFIGURATION;
                            break;

                        case UsbStandardRequest.SetConfiguration:
                            req.Config.ID = setupPacket.Value;
                            code = LibUsbIoCtl.SET_CONFIGURATION;
                            break;

                        case UsbStandardRequest.GetInterface:
                            req.Iface.ID = setupPacket.Index;
                            code = LibUsbIoCtl.GET_INTERFACE;
                            break;

                        case UsbStandardRequest.SetInterface:
                            req.Iface.ID = setupPacket.Index;
                            req.Iface.AlternateID = setupPacket.Value;
                            code = LibUsbIoCtl.SET_INTERFACE;
                            break;

                        default:
                            UsbError.Error(ErrorCode.IoControlMessage,0,
                                            String.Format("Invalid request: 0x{0:X8}", setupPacket.Request),
                                            typeof(LibUsbDriverIO));
                            return false;
                    }
                    break;

                case UsbRequestType.TypeVendor:
                case UsbRequestType.TypeClass:

                    req.Vendor.Type = ((byte) setupPacket.RequestType >> 5) & 0x03;
                    req.Vendor.Recipient = (int) setupPacket.RequestType & 0x1F;
                    req.Vendor.Request = (int) setupPacket.Request;
                    req.Vendor.ID = setupPacket.Value;
                    req.Vendor.Index = setupPacket.Index;

                    code = ((byte) setupPacket.RequestType & 0x80) > 0 ? LibUsbIoCtl.VENDOR_READ : LibUsbIoCtl.VENDOR_WRITE;
                    break;

                case UsbRequestType.TypeReserved:
                default:
                    UsbError.Error(ErrorCode.IoControlMessage,0,
                                    String.Format("invalid or unsupported request type: 0x{0:X8}", setupPacket.RequestType),
                                    typeof(LibUsbDriverIO));
                    return false;
            }

            /* in request? */
            Byte[] reqBytes = req.Bytes;
            Byte[] inBytes = reqBytes;
            if ((setupPacket.RequestType & (byte)UsbEndpointDirection.EndpointIn) == 0)
            {
                inBytes = new byte[LibUsbRequest.Size + bufferLength];
                reqBytes.CopyTo(inBytes, 0);
                if (buffer != IntPtr.Zero) Marshal.Copy(buffer, inBytes, LibUsbRequest.Size, bufferLength);

                buffer = IntPtr.Zero;
                bufferLength = 0;
            }

            if (UsbIOSync(interfaceHandle, code, inBytes, inBytes.Length, buffer, bufferLength, out lengthTransferred))
            {
                /* in request? */
                if ((setupPacket.RequestType & (byte)UsbEndpointDirection.EndpointIn) == 0)
                    lengthTransferred = (inBytes.Length - reqBytes.Length);

                return true;
            }
            return false;
        }
    }
}