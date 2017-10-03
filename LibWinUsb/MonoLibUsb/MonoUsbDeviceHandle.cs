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
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;
using MonoLibUsb.Profile;

namespace MonoLibUsb
{
    /// <summary>
    /// Represents a Libusb-1.0 device handle.
    /// </summary>
    /// <remarks>
    /// <para>To close a device, see the <see cref="Close"/> method.</para>
    /// <note title="Libusb-1.0 API Note:" type="cpp">A <see cref="MonoUsbDeviceHandle"/> is roughly equivalent to a <a href="http://libusb.sourceforge.net/api-1.0/group__dev.html#ga7df95821d20d27b5597f1d783749d6a4">libusb_device_handle</a>.</note>
    /// </remarks>
    /// <code>
    /// MonoUsbDeviceHandle deviceHandle = new MonoUsbDeviceHandle(profileHandle);
    /// if (deviceHandle.IsInvalid) throw new Exception("Invalid device context.");
    /// </code>
    public class MonoUsbDeviceHandle : SafeContextHandle
    {
        private static Object handleLOCK = new object();
        private static MonoUsbError mLastReturnCode;
        private static String mLastReturnString = String.Empty;

        /// <summary>
        /// If the device handle is <see cref="SafeContextHandle.IsInvalid"/>, gets a descriptive string for the <see cref="LastErrorCode"/>.
        /// </summary>
        public static string LastErrorString
        {
            get
            {
                lock (handleLOCK)
                {
                    return mLastReturnString;
                }
            }
        }
        /// <summary>
        /// If the device handle is <see cref="SafeContextHandle.IsInvalid"/>, gets the <see cref="MonoUsbError"/> status code indicating the reason.
        /// </summary>
        public static MonoUsbError LastErrorCode
        {
            get
            {
                lock (handleLOCK)
                {
                    return mLastReturnCode;
                }
            }
        }
        /// <summary>Open a device handle from <paramref name="profileHandle"/>.</summary>
        /// <remarks>
        /// <para>A handle allows you to perform I/O on the device in question.</para>
        /// <para>To close a device handle call its <see cref="SafeHandle.Close"/> method.</para>
        /// <para>This is a non-blocking function; no requests are sent over the bus.</para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">The <see cref="MonoUsbDeviceHandle(MonoUsbProfileHandle)"/> constructor is roughly equivalent to <a href="http://libusb.sourceforge.net/api-1.0/group__dev.html#ga8163100afdf933fabed0db7fa81c89d1">libusb_open()</a>.</note>
        /// </remarks>
        /// <param name="profileHandle">A device profile handle.</param>
        public MonoUsbDeviceHandle(MonoUsbProfileHandle profileHandle)
            : base(IntPtr.Zero) 
        {
            IntPtr pDeviceHandle = IntPtr.Zero;
            int ret = MonoUsbApi.Open(profileHandle, ref pDeviceHandle);
            if (ret < 0 || pDeviceHandle==IntPtr.Zero)
            {
                lock (handleLOCK)
                {
                    mLastReturnCode = (MonoUsbError) ret;
                    mLastReturnString = MonoUsbApi.StrError(mLastReturnCode);
                }
                SetHandleAsInvalid();
            }
            else
            {
                SetHandle(pDeviceHandle);
            }

        }

        internal MonoUsbDeviceHandle(IntPtr pDeviceHandle)
            : base(pDeviceHandle)
        {
        }
        ///<summary>
        ///Closes the <see cref="MonoUsbDeviceHandle"/>.
        ///</summary>
        ///<returns>
        ///true if the <see cref="MonoUsbDeviceHandle"/> is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        ///</returns>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                Debug.WriteLine(GetType().Name + ".ReleaseHandle() Before", "Libusb-1.0");
                MonoUsbApi.Close(handle);
                Debug.WriteLine(GetType().Name + ".ReleaseHandle() After", "Libusb-1.0");
                SetHandleAsInvalid();
            }
            return true;
        }

        /// <summary>
        /// Closes the <see cref="MonoUsbDeviceHandle"/> reference.  When all references are no longer is use, the device
        /// is closed in the <see cref="ReleaseHandle"/> finalizer.
        /// </summary>
        /// <remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">The <see cref="Close"/> method is roughly equivalent to <a href="http://libusb.sourceforge.net/api-1.0/group__dev.html#ga779bc4f1316bdb0ac383bddbd538620e">libusb_close()</a>.</note>
        /// </remarks>
        public new void Close()
        {
            base.Close();
        }
    }
}