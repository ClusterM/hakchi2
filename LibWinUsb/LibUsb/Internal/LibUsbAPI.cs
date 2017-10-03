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
    internal class LibUsbAPI : UsbApiBase
    {
        public override bool AbortPipe(SafeHandle interfaceHandle, byte pipeID)
        {
            LibUsbRequest req = new LibUsbRequest();

            int ret;
            req.Endpoint.ID = pipeID;
            req.Timeout = UsbConstants.DEFAULT_TIMEOUT;
            return LibUsbDriverIO.UsbIOSync(interfaceHandle, LibUsbIoCtl.ABORT_ENDPOINT, req, LibUsbRequest.Size, IntPtr.Zero, 0, out ret);
        }

        public bool ResetDevice(SafeHandle interfaceHandle)
        {
            LibUsbRequest req = new LibUsbRequest();

            int ret;
            req.Timeout = UsbConstants.DEFAULT_TIMEOUT;
            return LibUsbDriverIO.UsbIOSync(interfaceHandle, LibUsbIoCtl.RESET_DEVICE, req, LibUsbRequest.Size, IntPtr.Zero, 0, out ret);
        }
        
        public override bool ControlTransfer(SafeHandle interfaceHandle,
                                             UsbSetupPacket setupPacket,
                                             IntPtr buffer,
                                             int bufferLength,
                                             out int lengthTransferred)
        {
            return LibUsbDriverIO.ControlTransfer(interfaceHandle,
                                                  setupPacket,
                                                  buffer,
                                                  bufferLength,
                                                  out lengthTransferred,
                                                  UsbConstants.DEFAULT_TIMEOUT);
        }

        public override bool FlushPipe(SafeHandle interfaceHandle, byte pipeID) { return true; }


        public override bool GetDescriptor(SafeHandle interfaceHandle,
                                           byte descriptorType,
                                           byte index,
                                           ushort languageID,
                                           IntPtr buffer,
                                           int bufferLength,
                                           out int lengthTransferred)
        {
            LibUsbRequest req = new LibUsbRequest();
            req.Descriptor.Index = index;
            req.Descriptor.LangID = languageID;
            req.Descriptor.Recipient = (byte) UsbEndpointDirection.EndpointIn & 0x1F;
            req.Descriptor.Type = descriptorType;
            return LibUsbDriverIO.UsbIOSync(interfaceHandle,
                                            LibUsbIoCtl.GET_DESCRIPTOR,
                                            req,
                                            LibUsbRequest.Size,
                                            buffer,
                                            bufferLength,
                                            out lengthTransferred);
        }


        public override bool GetOverlappedResult(SafeHandle interfaceHandle, IntPtr pOverlapped, out int numberOfBytesTransferred, bool wait) { return Kernel32.GetOverlappedResult(interfaceHandle, pOverlapped, out numberOfBytesTransferred, wait); }


        public override bool ReadPipe(UsbEndpointBase endPointBase,
                                      IntPtr buffer,
                                      int bufferLength,
                                      out int lengthTransferred,
                                      int isoPacketSize,
                                      IntPtr pOverlapped)
        {
            LibUsbRequest req = new LibUsbRequest();
            req.Endpoint.ID = endPointBase.EpNum;
            req.Endpoint.PacketSize = isoPacketSize;
            req.Timeout = UsbConstants.DEFAULT_TIMEOUT;

            int cltCode = endPointBase.Type == EndpointType.Isochronous ? LibUsbIoCtl.ISOCHRONOUS_READ : LibUsbIoCtl.INTERRUPT_OR_BULK_READ; 


            return Kernel32.DeviceIoControl(endPointBase.Device.Handle,
                                            cltCode,
                                            req,
                                            LibUsbRequest.Size,
                                            buffer,
                                            bufferLength,
                                            out lengthTransferred,
                                            pOverlapped);
        }

        public override bool ResetPipe(SafeHandle interfaceHandle, byte pipeID)
        {
            LibUsbRequest req = new LibUsbRequest();

            int ret;
            req.Endpoint.ID = pipeID;
            req.Timeout = UsbConstants.DEFAULT_TIMEOUT;
            return LibUsbDriverIO.UsbIOSync(interfaceHandle, LibUsbIoCtl.RESET_ENDPOINT, req, LibUsbRequest.Size, IntPtr.Zero, 0, out ret);
        }

  
        public override bool WritePipe(UsbEndpointBase endPointBase,
                                       IntPtr buffer,
                                       int bufferLength,
                                       out int lengthTransferred,
                                       int isoPacketSize,
                                       IntPtr pOverlapped)
        {
            LibUsbRequest req = new LibUsbRequest();
            req.Endpoint.ID = endPointBase.EpNum;
            req.Endpoint.PacketSize = isoPacketSize;
            req.Timeout = UsbConstants.DEFAULT_TIMEOUT;
            int cltCode = endPointBase.Type == EndpointType.Isochronous ? LibUsbIoCtl.ISOCHRONOUS_WRITE : LibUsbIoCtl.INTERRUPT_OR_BULK_WRITE; 

            return Kernel32.DeviceIoControl(endPointBase.Handle,
                                            cltCode,
                                            req,
                                            LibUsbRequest.Size,
                                            buffer,
                                            bufferLength,
                                            out lengthTransferred,
                                            pOverlapped);
        }
    }
}