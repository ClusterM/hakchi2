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
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.Internal.LibUsb
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = sizeof (int)*6)]
    internal class LibUsbRequest
    {
        public static int Size = Marshal.SizeOf(typeof (LibUsbRequest));
        [FieldOffset(0)] public int Timeout = UsbConstants.DEFAULT_TIMEOUT;

        #region Union Struct

        [FieldOffset(sizeof (int))] public Control Control;

        [FieldOffset(sizeof (int))] public Config Config;

        [FieldOffset(sizeof (int))] public Debug Debug;

        [FieldOffset(sizeof (int))] public Descriptor Descriptor;

        [FieldOffset(sizeof (int))] public Endpoint Endpoint;

        [FieldOffset(sizeof (int))] public Feature Feature;

        [FieldOffset(sizeof (int))] public Iface Iface;

        [FieldOffset(sizeof (int))] public Status Status;

        [FieldOffset(sizeof (int))] public Vendor Vendor;

        [FieldOffset(sizeof (int))] public UsbKernelVersion Version;

        [FieldOffset(sizeof (int))] public DeviceProperty DeviceProperty;

        [FieldOffset(sizeof (int))] public DeviceRegKey DeviceRegKey;

        [FieldOffset(sizeof (int))] public BusQueryID BusQueryID;
        #endregion

        public Byte[] Bytes
        {
            get
            {
                Byte[] rtn = new byte[Size];

                for (int i = 0; i < Size; i++)
                    rtn[i] = Marshal.ReadByte(this, i);

                return rtn;
            }
        }


        public void RequestConfigDescriptor(int index)
        {
            Timeout = UsbConstants.DEFAULT_TIMEOUT;

            int value = ((int) DescriptorType.Configuration << 8) + index;

            Descriptor.Recipient = (byte)UsbEndpointDirection.EndpointIn & 0x1F;
            Descriptor.Type = (value >> 8) & 0xFF;
            Descriptor.Index = value & 0xFF;
            Descriptor.LangID = 0;
        }

        public void RequestStringDescriptor(int index, short langid)
        {
            Timeout = UsbConstants.DEFAULT_TIMEOUT;

            int value = ((int) DescriptorType.String << 8) + index;

            Descriptor.Recipient = (byte)UsbEndpointDirection.EndpointIn & 0x1F;
            Descriptor.Type = value >> 8 & 0xFF;
            Descriptor.Index = value & 0xFF;
            Descriptor.LangID = langid;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Descriptor
    {
        public int Type;
        public int Index;
        public int LangID;
        public int Recipient;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Config
    {
        public int ID;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Control
    {
        public byte RequestType;
        public byte Request;
        public ushort Value;
        public ushort Index;
        public ushort Length;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DeviceProperty
    {
        public int ID;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Iface
    {
        public int ID;
        public int AlternateID;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Endpoint
    {
        public int ID;
        public int PacketSize;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Vendor
    {
        public int Type;
        public int Recipient;
        public int Request;
        public int ID;
        public int Index;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Feature
    {
        public int Recipient;
        public int ID;
        public int Index;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Status
    {
        public int Recipient;
        public int Index;
        public int ID;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Debug
    {
        public int Level;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DeviceRegKey
    {
        public int KeyType;
        public int NameOffset;
        public int ValueOffset;
        public int ValueLength;
    } ;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BusQueryID
    {
        public ushort IDType;
    } ;
    
}