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
using System.Security;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Internal;
using LibUsbDotNet.Internal.WinUsb;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;

// ReSharper disable InconsistentNaming

namespace LibUsbDotNet.WinUsb.Internal
{
    [SuppressUnmanagedCodeSecurity]
    internal class WinUsbAPI : UsbApiBase
    {
        internal const string WIN_USB_DLL = "winusb.dll";


        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_AbortPipe", SetLastError = true)]
        private static extern bool WinUsb_AbortPipe([In] SafeHandle InterfaceHandle, byte PipeID);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
        private static extern bool WinUsb_ControlTransfer([In] SafeHandle InterfaceHandle,
                                                          [In] UsbSetupPacket SetupPacket,
                                                          IntPtr Buffer,
                                                          int BufferLength,
                                                          out int LengthTransferred,
                                                          IntPtr pOVERLAPPED);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_FlushPipe", SetLastError = true)]
        private static extern bool WinUsb_FlushPipe([In] SafeHandle InterfaceHandle, byte PipeID);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_Free", SetLastError = true)]
        internal static extern bool WinUsb_Free([In] IntPtr InterfaceHandle);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetAssociatedInterface", SetLastError = true)]
        internal static extern bool WinUsb_GetAssociatedInterface([In] SafeHandle InterfaceHandle,
                                                                  byte AssociatedInterfaceIndex,
                                                                  ref IntPtr AssociatedInterfaceHandle);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetCurrentAlternateSetting", SetLastError = true)]
        internal static extern bool WinUsb_GetCurrentAlternateSetting([In] SafeHandle InterfaceHandle, out byte SettingNumber);


        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetDescriptor", SetLastError = true)]
        private static extern bool WinUsb_GetDescriptor([In] SafeHandle InterfaceHandle,
                                                        byte DescriptorType,
                                                        byte Index,
                                                        ushort LanguageID,
                                                        IntPtr Buffer,
                                                        int BufferLength,
                                                        out int LengthTransferred);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetOverlappedResult", SetLastError = true)]
        private static extern bool WinUsb_GetOverlappedResult([In] SafeHandle InterfaceHandle,
                                                              IntPtr pOVERLAPPED,
                                                              out int lpNumberOfBytesTransferred,
                                                              bool Wait);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetPipePolicy", SetLastError = true)]
        internal static extern bool WinUsb_GetPipePolicy([In] SafeHandle InterfaceHandle,
                                                         byte PipeID,
                                                         PipePolicyType policyType,
                                                         ref int ValueLength,
                                                         IntPtr Value);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetPowerPolicy", SetLastError = true)]
        internal static extern bool WinUsb_GetPowerPolicy([In] SafeHandle InterfaceHandle,
                                                          PowerPolicyType policyType,
                                                          ref int ValueLength,
                                                          IntPtr Value);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_Initialize", SetLastError = true)]
        internal static extern bool WinUsb_Initialize([In] SafeHandle DeviceHandle, [Out, In] ref SafeWinUsbInterfaceHandle InterfaceHandle);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryDeviceInformation", SetLastError = true)]
        internal static extern bool WinUsb_QueryDeviceInformation([In] SafeHandle InterfaceHandle,
                                                                  DeviceInformationTypes InformationType,
                                                                  ref int BufferLength,
                                                                  [MarshalAs(UnmanagedType.AsAny), In, Out] object Buffer);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryInterfaceSettings", SetLastError = true)]
        internal static extern bool WinUsb_QueryInterfaceSettings([In] SafeHandle InterfaceHandle,
                                                                  byte AlternateInterfaceNumber,
                                                                  [MarshalAs(UnmanagedType.LPStruct), In, Out] UsbInterfaceDescriptor
                                                                      UsbAltInterfaceDescriptor);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryPipe", SetLastError = true)]
        internal static extern bool WinUsb_QueryPipe([In] SafeHandle InterfaceHandle,
                                                     byte AlternateInterfaceNumber,
                                                     byte PipeIndex,
                                                     [MarshalAs(UnmanagedType.LPStruct), In, Out] PipeInformation PipeInformation);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
        private static extern bool WinUsb_ReadPipe([In] SafeHandle InterfaceHandle,
                                                   byte PipeID,
                                                   Byte[] Buffer,
                                                   int BufferLength,
                                                   out int LengthTransferred,
                                                   IntPtr pOVERLAPPED);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
        private static extern bool WinUsb_ReadPipe([In] SafeHandle InterfaceHandle,
                                                   byte PipeID,
                                                   IntPtr pBuffer,
                                                   int BufferLength,
                                                   out int LengthTransferred,
                                                   IntPtr pOVERLAPPED);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ResetPipe", SetLastError = true)]
        private static extern bool WinUsb_ResetPipe([In] SafeHandle InterfaceHandle, byte PipeID);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
        internal static extern bool WinUsb_SetPipePolicy([In] SafeHandle InterfaceHandle,
                                                         byte PipeID,
                                                         PipePolicyType policyType,
                                                         int ValueLength,
                                                         IntPtr Value);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_SetPowerPolicy", SetLastError = true)]
        internal static extern bool WinUsb_SetPowerPolicy([In] SafeHandle InterfaceHandle, PowerPolicyType policyType, int ValueLength, IntPtr Value);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
        private static extern bool WinUsb_WritePipe([In] SafeHandle InterfaceHandle,
                                                    byte PipeID,
                                                    Byte[] Buffer,
                                                    int BufferLength,
                                                    out int LengthTransferred,
                                                    IntPtr pOVERLAPPED);

        [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
        private static extern bool WinUsb_WritePipe([In] SafeHandle InterfaceHandle,
                                                    byte PipeID,
                                                    IntPtr pBuffer,
                                                    int BufferLength,
                                                    out int LengthTransferred,
                                                    IntPtr pOVERLAPPED);


        public override bool AbortPipe(SafeHandle InterfaceHandle, byte PipeID) { return WinUsb_AbortPipe(InterfaceHandle, PipeID); }

        public override bool ControlTransfer(SafeHandle InterfaceHandle,
                                             UsbSetupPacket SetupPacket,
                                             IntPtr Buffer,
                                             int BufferLength,
                                             out int LengthTransferred) { return WinUsb_ControlTransfer(InterfaceHandle, SetupPacket, Buffer, BufferLength, out LengthTransferred, IntPtr.Zero); }

        public override bool FlushPipe(SafeHandle InterfaceHandle, byte PipeID) { return WinUsb_FlushPipe(InterfaceHandle, PipeID); }

        public override bool GetDescriptor(SafeHandle InterfaceHandle,
                                           byte DescriptorType,
                                           byte Index,
                                           ushort LanguageID,
                                           IntPtr Buffer,
                                           int BufferLength,
                                           out int LengthTransferred) { return WinUsb_GetDescriptor(InterfaceHandle, DescriptorType, Index, LanguageID, Buffer, BufferLength, out LengthTransferred); }

        public override bool GetOverlappedResult(SafeHandle InterfaceHandle, IntPtr pOVERLAPPED, out int numberOfBytesTransferred, bool Wait) { return WinUsb_GetOverlappedResult(InterfaceHandle, pOVERLAPPED, out numberOfBytesTransferred, Wait); }

        //public override bool ReadPipe(UsbEndpointBase endPointBase,
        //                              byte[] Buffer,
        //                              int BufferLength,
        //                              out int LengthTransferred,
        //                              int isoPacketSize,
        //                              IntPtr pOVERLAPPED) { return WinUsb_ReadPipe(endPointBase.Device.Handle, endPointBase.EpNum, Buffer, BufferLength, out LengthTransferred, pOVERLAPPED); }

        public override bool ReadPipe(UsbEndpointBase endPointBase,
                                      IntPtr pBuffer,
                                      int BufferLength,
                                      out int LengthTransferred,
                                      int isoPacketSize,
                                      IntPtr pOVERLAPPED) { return WinUsb_ReadPipe(endPointBase.Device.Handle, endPointBase.EpNum, pBuffer, BufferLength, out LengthTransferred, pOVERLAPPED); }

        public override bool ResetPipe(SafeHandle InterfaceHandle, byte PipeID) { return WinUsb_ResetPipe(InterfaceHandle, PipeID); }

        //public override bool WritePipe(UsbEndpointBase endPointBase,
        //                               byte[] Buffer,
        //                               int BufferLength,
        //                               out int LengthTransferred,
        //                               int isoPacketSize,
        //                               IntPtr pOVERLAPPED) { return WinUsb_WritePipe(endPointBase.Device.Handle, endPointBase.EpNum, Buffer, BufferLength, out LengthTransferred, pOVERLAPPED); }

        public override bool WritePipe(UsbEndpointBase endPointBase,
                                       IntPtr pBuffer,
                                       int BufferLength,
                                       out int LengthTransferred,
                                       int isoPacketSize,
                                       IntPtr pOVERLAPPED) { return WinUsb_WritePipe(endPointBase.Device.Handle, endPointBase.EpNum, pBuffer, BufferLength, out LengthTransferred, pOVERLAPPED); }

        internal static bool OpenDevice(out SafeFileHandle sfhDevice, string DevicePath)
        {
            sfhDevice =
                Kernel32.CreateFile(DevicePath,
                                    NativeFileAccess.FILE_GENERIC_WRITE | NativeFileAccess.FILE_GENERIC_READ,
                                    NativeFileShare.FILE_SHARE_WRITE | NativeFileShare.FILE_SHARE_READ,
                                    IntPtr.Zero,
                                    NativeFileMode.OPEN_EXISTING,
                                    NativeFileFlag.FILE_ATTRIBUTE_NORMAL | NativeFileFlag.FILE_FLAG_OVERLAPPED,
                                    IntPtr.Zero);

            return (!sfhDevice.IsInvalid && !sfhDevice.IsClosed);
        }
    }
}