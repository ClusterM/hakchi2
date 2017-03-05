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
using System.Threading;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;

namespace LibUsbDotNet.Internal.LibUsb
{
    internal partial class LibUsbDriverIO
    {
        public const int ERROR_IO_PENDING = 997;
        public const int FALSE = 0;
        public const int FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const string LIBUSB_DEVICE_NAME = "\\\\.\\libusb0-";
        public const int TRUE = 1;

        private static byte[] _tempCfgBuf;

        internal static byte[] GlobalTempCfgBuffer
        {
            get
            {
                if (ReferenceEquals(null, _tempCfgBuf))
                    _tempCfgBuf = new byte[4096];

                return _tempCfgBuf;
            }
        }


        internal static string GetDeviceNameString(int index) { return String.Format("{0}{1}", LIBUSB_DEVICE_NAME, index.ToString("0000")); }

        internal static SafeFileHandle OpenDevice(String deviceFileName)
        {
            return Kernel32.CreateFile(deviceFileName,
                                       NativeFileAccess.SPECIAL,
                                       NativeFileShare.NONE,
                                       IntPtr.Zero,
                                       NativeFileMode.OPEN_EXISTING,
                                       NativeFileFlag.FILE_FLAG_OVERLAPPED,
                                       IntPtr.Zero);
        }


        internal static bool UsbIOSync(SafeHandle dev, int code, Object inBuffer, int inSize, IntPtr outBuffer, int outSize, out int ret)
        {
            SafeOverlapped deviceIoOverlapped = new SafeOverlapped();
            ManualResetEvent hEvent = new ManualResetEvent(false);
            deviceIoOverlapped.ClearAndSetEvent(hEvent.SafeWaitHandle.DangerousGetHandle());
            ret = 0;

            if (!Kernel32.DeviceIoControlAsObject(dev, code, inBuffer, inSize, outBuffer, outSize, ref ret, deviceIoOverlapped.GlobalOverlapped))
            {
                int iError = Marshal.GetLastWin32Error();
                if (iError != ERROR_IO_PENDING)
                {
                    // Don't log errors for these control codes.
                    do
                    {
                        if (code == LibUsbIoCtl.GET_REG_PROPERTY) break;
                        if (code == LibUsbIoCtl.GET_CUSTOM_REG_PROPERTY) break;
                        UsbError.Error(ErrorCode.Win32Error, iError, String.Format("DeviceIoControl code {0:X8} failed:{1}", code, Kernel32.FormatSystemMessage(iError)), typeof(LibUsbDriverIO));
                    } while (false);

                    hEvent.Close();
                    return false;
                }
            }
            if (Kernel32.GetOverlappedResult(dev, deviceIoOverlapped.GlobalOverlapped, out ret, true))
            {
                hEvent.Close();
                return true;
            }
            UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetOverlappedResult failed.\nIoCtlCode:" + code, typeof(LibUsbDriverIO));
            hEvent.Close();
            return false;
        }
    }
}