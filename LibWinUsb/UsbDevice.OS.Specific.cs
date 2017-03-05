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
using System.IO;
using System.Runtime.InteropServices;
using LibUsbDotNet.Internal.LibUsb;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using LibUsbDotNet.LudnMonoLibUsb;
using LibUsbDotNet.WinUsb;
using MonoLibUsb;

namespace LibUsbDotNet
{
    public abstract partial class UsbDevice
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly bool ForceLegacyLibUsb = IsLinux;

        /// <summary>
        /// Setting this field to <see langword="true"/> will force <see cref="LibUsbDotNet"/> to use the <a href="http://www.libusb.org/wiki/windows_backend">Libusb-1.0 Windows-backend driver.</a>  For platforms other than windows, this setting has no effect.
        /// </summary>
        /// <remarks>
        /// If this is <see langword="true"/>, <see cref="AllDevices"/> will return only <see cref="MonoUsbDevice"/>s in the list.
        /// </remarks>
        public static bool ForceLibUsbWinBack = false;
        

        /// <summary>
        /// Gets a list of all available WinUSB USB devices.
        /// </summary>
        /// <remarks>
        /// On windows, gets a list of WinUSB devices. On linux always returns null.
        /// <para>
        /// Using the <see cref="AllDevices"/> property instead will ensure your source code is platform-independent.
        /// </para>
        /// </remarks>
        public static UsbRegDeviceList AllWinUsbDevices
        {
            get
            {
                UsbRegDeviceList regDevList = new UsbRegDeviceList();
                if (IsLinux || ForceLibUsbWinBack) return regDevList;

                if (HasWinUsbDriver)
                {
                    List<WinUsbRegistry> winUsbRegistry = WinUsbRegistry.DeviceList;
                    foreach (WinUsbRegistry usbRegistry in winUsbRegistry)
                        regDevList.Add(usbRegistry);
                }

                return regDevList;
            }
        }

        /// <summary>
        /// True if the LibUsb driver is found on the system.
        /// </summary>
        [Obsolete("Always returns true")]
        public static bool HasLibUsbDriver
        {
            get
            {
                return true;
            }
        }
        /*
        public static bool HasLibUsbDriver
        {
            get
            {
                if (mHasLibUsbDriver == null)
                {
                    if (IsLinux)
                    {
                        mHasLibUsbDriver = true;
                    }
                    else
                    {
                        mHasLibUsbDriver = false;
                        string sysDriver = Path.Combine(Environment.SystemDirectory, "drivers");
                        DirectoryInfo diSysDriver = new DirectoryInfo(sysDriver);
                        if (diSysDriver.Exists)
                        {
                            FileInfo[] libUsbSysFileInfo = diSysDriver.GetFiles(LIBUSB_SYS);
                            if (libUsbSysFileInfo.Length > 0)
                                mHasLibUsbDriver = true;
                        }
                    }
                }
                return (bool) mHasLibUsbDriver;
            }
        }
        */

        /// <summary>
        /// True if the WinUSB API is available.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static bool HasWinUsbDriver
        {
            get
            {
                if (mHasWinUsbDriver == null)
                {
                    if (IsLinux)
                    {
                        mHasWinUsbDriver = false;
                    }
                    else
                    {
                        try
                        {
                            WinUsb.Internal.WinUsbAPI.WinUsb_Free(IntPtr.Zero);
                            mHasWinUsbDriver = true;
                        }
                        catch (Exception)
                        {
                            mHasWinUsbDriver = false;

                        }
                    }
                }
                return (bool) mHasWinUsbDriver;
            }
        }

        /// <summary>
        /// True if the libusb-1.0 API is available.
        /// </summary>
        public static bool HasLibUsbWinBackDriver
        {
            get
            {
                if (mHasLibUsbWinBackDriver == null)
                {
                    if (IsLinux)
                    {
                        mHasLibUsbWinBackDriver = false;
                    }
                    else
                    {
                        try
                        {
                            MonoUsbApi.StrError(MonoUsbError.Success);
                            mHasLibUsbWinBackDriver = true;
                        }
                        catch(Exception)
                        {
                            mHasLibUsbWinBackDriver = false;
                          
                        }
                    }
                }
                return (bool)mHasLibUsbWinBackDriver;
            }
        }
        ///<summary>
        /// Returns true if the system is a linux/unix-like operating system. 
        ///</summary>
        ///<exception cref="NotSupportedException"></exception>
        public static bool IsLinux
        {
            get
            {
                return Helper.IsLinux;

            }
        }

        /// <summary>
        /// Gets the kernel driver type in use by LibUsbDotNet. 
        /// If <see cref="LibUsbKernelType.NativeLibUsb"/> is returned, LibUsbDotNet using using its
        /// native kernel driver.  Basic usb device information is retreived from the windows registry
        /// which reduces USB IO overhead. 
        /// If <see cref="LibUsbKernelType.LegacyLibUsb"/> is returned, LibUsbDotNet is using the orginal kernel
        /// available at the libusb-win32.sourceforge.net page and true windows registry support
        /// is unavailable.
        /// Under linux, <see cref="LibUsbKernelType.MonoLibUsb"/> is always returned.
        /// </summary>
        public static LibUsbKernelType KernelType
        {
            get
            {
                if (mLibUsbKernelType == LibUsbKernelType.Unknown)
                {
                    if (IsLinux)
                    {
                        mLibUsbKernelType = LibUsbKernelType.MonoLibUsb;
                    }
                    else
                    {
                        UsbKernelVersion libUsbVersion = KernelVersion;
                        if (!libUsbVersion.IsEmpty)
                        {
                            mLibUsbKernelType = libUsbVersion.BcdLibUsbDotNetKernelMod != 0
                                                    ? LibUsbKernelType.NativeLibUsb
                                                    : LibUsbKernelType.LegacyLibUsb;
                        }
                    }
                }

                return mLibUsbKernelType;
            }
        }

        /// <summary>
        /// Gets the kernel driver version in use by LibUsbDotNet.
        /// <alert class="note"><para>
        /// if <see cref="UsbKernelVersion.BcdLibUsbDotNetKernelMod"/> is non-zero then the kernel driver is native.
        /// </para></alert>
        /// </summary>
        public static UsbKernelVersion KernelVersion
        {
            get
            {
                if (mUsbKernelVersion.IsEmpty)
                {
                    if (IsLinux)
                    {
                        mUsbKernelVersion = new UsbKernelVersion(1, 0, 0, 0, 0);
                    }
                    else
                    {
                        for (int i = 1; i < UsbConstants.MAX_DEVICES; i++)
                        {
                            LibUsbDevice newLibUsbDevice;
                            string deviceFileName = LibUsbDriverIO.GetDeviceNameString(i);
                            if (!LibUsbDevice.Open(deviceFileName, out newLibUsbDevice)) continue;
                            LibUsbRequest request = new LibUsbRequest();
                            GCHandle gcReq = GCHandle.Alloc(request, GCHandleType.Pinned);

                            int transferred;
                            bool bSuccess = newLibUsbDevice.UsbIoSync(LibUsbIoCtl.GET_VERSION,
                                                                      request,
                                                                      LibUsbRequest.Size,
                                                                      gcReq.AddrOfPinnedObject(),
                                                                      LibUsbRequest.Size,
                                                                      out transferred);

                            gcReq.Free();
                            newLibUsbDevice.Close();
                            if (bSuccess && transferred == LibUsbRequest.Size)
                            {
                                mUsbKernelVersion = request.Version;
                                break;
                            }
                        }
                    }
                }

                return mUsbKernelVersion;
            }
        }


        ///<summary>
        /// Gets a <see cref="System.OperatingSystem"/> object that contains the current platform identifier and version number.
        ///</summary>
        public static OperatingSystem OSVersion
        {
            get
            {
                return Helper.OSVersion;
            }
        }
    }
}