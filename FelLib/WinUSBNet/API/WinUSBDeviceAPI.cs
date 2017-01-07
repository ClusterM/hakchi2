/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

/* NOTE: Parts of the code in this file are based on the work of Jan Axelson
 * See http://www.lvr.com/winusb.htm for more information
 */

using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;

namespace MadWizard.WinUSBNet.API
{
       [StructLayout(LayoutKind.Sequential)]
		struct USB_DEVICE_DESCRIPTOR
		{ 
            public byte bLength;
            public byte bDescriptorType;
            public ushort bcdUSB;
            public byte bDeviceClass;
            public byte bDeviceSubClass;
            public byte bDeviceProtocol;
            public byte bMaxPacketSize0;
            public ushort idVendor;
            public ushort idProduct;
            public ushort bcdDevice;
            public byte iManufacturer;
            public byte iProduct;
            public byte iSerialNumber;
            public byte bNumConfigurations;
        };

        [StructLayout(LayoutKind.Sequential)]
		struct USB_CONFIGURATION_DESCRIPTOR
		{
            public byte bLength;
            public byte bDescriptorType;
            public ushort wTotalLength;
            public byte bNumInterfaces;
            public byte bConfigurationValue;
            public byte iConfiguration;
            public byte bmAttributes;
            public byte MaxPower;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct USB_INTERFACE_DESCRIPTOR
		{
            public byte bLength;
            public byte bDescriptorType;
            public byte bInterfaceNumber;
            public byte bAlternateSetting;
            public byte bNumEndpoints;
            public byte bInterfaceClass;
            public byte bInterfaceSubClass;
            public byte bInterfaceProtocol;
            public byte iInterface;
		};
        enum USBD_PIPE_TYPE : int
        {
            UsbdPipeTypeControl,
            UsbdPipeTypeIsochronous,
            UsbdPipeTypeBulk,
            UsbdPipeTypeInterrupt,
        }
        [StructLayout(LayoutKind.Sequential)]
        struct WINUSB_PIPE_INFORMATION
        {
            public USBD_PIPE_TYPE PipeType;
            public byte PipeId;
            public ushort MaximumPacketSize;
            public byte Interval;
        }

        enum POLICY_TYPE : int
        {
            SHORT_PACKET_TERMINATE = 1,
            AUTO_CLEAR_STALL,
            PIPE_TRANSFER_TIMEOUT,
            IGNORE_SHORT_PACKETS,
            ALLOW_PARTIAL_READS,
            AUTO_FLUSH,
            RAW_IO,
        }


	partial class WinUSBDevice
	{
		private const UInt32 DEVICE_SPEED = ((UInt32)(1));
		
        private enum USB_DEVICE_SPEED : int
        {
            UsbLowSpeed = 1,
            UsbFullSpeed,
            UsbHighSpeed,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct WINUSB_SETUP_PACKET
        {
            public byte RequestType;
            public byte Request;
            public ushort Value;
            public ushort Index;
            public ushort Length;
        }

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true)]
        private static unsafe extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, NativeOverlapped* pOverlapped);

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_Free(IntPtr InterfaceHandle);

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_Initialize(SafeFileHandle DeviceHandle, ref IntPtr InterfaceHandle);

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_QueryDeviceInformation(IntPtr InterfaceHandle, UInt32 InformationType, ref UInt32 BufferLength, out byte Buffer);

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, Byte AlternateInterfaceNumber, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_QueryPipe(IntPtr InterfaceHandle, Byte AlternateInterfaceNumber, Byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation);

		[DllImport("winusb.dll", SetLastError = true)]
        private static unsafe extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true)]
        private static unsafe extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, NativeOverlapped* pOverlapped);
        
        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID);

		//  Two declarations for WinUsb_SetPipePolicy. 
		//  Use this one when the returned Value is a Byte (all except PIPE_TRANSFER_TIMEOUT):

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, Byte PipeID, UInt32 PolicyType, UInt32 ValueLength, ref byte Value);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, Byte PipeID, UInt32 PolicyType, ref UInt32 ValueLength, out byte Value);

		//  Use this alias when the returned Value is a UInt32 (PIPE_TRANSFER_TIMEOUT only):

		[DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, Byte PipeID, UInt32 PolicyType, UInt32 ValueLength, ref UInt32 Value);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, Byte PipeID, UInt32 PolicyType, ref UInt32 ValueLength, out UInt32 Value);

		[DllImport("winusb.dll", SetLastError = true)]
        private static unsafe extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);
        
        [DllImport("winusb.dll", SetLastError = true)]
        private static unsafe extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, NativeOverlapped* pOverlapped);


        [DllImport("kernel32.dll", SetLastError = true)]
        private static unsafe extern bool CancelIo(IntPtr hFile);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static unsafe extern bool CancelIoEx(IntPtr hFile, NativeOverlapped* pOverlapped);
        
        


        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID);
        
        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType,
                        byte Index, UInt16 LanguageID, byte[] Buffer, UInt32 BufferLength, out UInt32 LengthTransfered);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType,
                        byte Index, UInt16 LanguageID, out USB_DEVICE_DESCRIPTOR deviceDesc, UInt32 BufferLength, out UInt32 LengthTransfered);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType,
                        byte Index, UInt16 LanguageID, out USB_CONFIGURATION_DESCRIPTOR deviceDesc, UInt32 BufferLength, out UInt32 LengthTransfered);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_GetAssociatedInterface(IntPtr InterfaceHandle, byte AssociatedInterfaceIndex,
                    out IntPtr AssociatedInterfaceHandle);
  
        private const int USB_DEVICE_DESCRIPTOR_TYPE = 0x01;
        private const int USB_CONFIGURATION_DESCRIPTOR_TYPE = 0x02;
        private const int USB_STRING_DESCRIPTOR_TYPE = 0x03;

        private const int ERROR_NO_MORE_ITEMS = 259;

	}
}
