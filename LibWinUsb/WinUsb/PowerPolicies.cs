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

namespace LibUsbDotNet.WinUsb
{
    /// <summary> 
    /// power policy for a <see cref="WinUsbDevice"/>.
    /// </summary> 
    public class PowerPolicies
    {
        private const int MAX_SIZE = 4;
        private readonly WinUsbDevice mUsbDevice;
        private IntPtr mBufferPtr = IntPtr.Zero;

        internal PowerPolicies(WinUsbDevice usbDevice)
        {
            mBufferPtr = Marshal.AllocCoTaskMem(MAX_SIZE);
            mUsbDevice = usbDevice;
        }

        /// <summary>
        /// If the auto suspend policy parameter is TRUE (that is, nonzero), the USB stack suspends the device when no transfers are pending. The default value for the AutoSuspend policy parameter is TRUE.
        /// </summary>
        public bool AutoSuspend
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = mUsbDevice.GetPowerPolicy(PowerPolicyType.AutoSuspend, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPowerPolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPowerPolicyValue);
                mUsbDevice.SetPowerPolicy(PowerPolicyType.AutoSuspend, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// The suspend delay policy parameter specifies the minimum amount of time, in milliseconds, that the WinUSB driver must wait after any transfer before it can suspend the device. 
        /// </summary>
        public int SuspendDelay
        {
            get
            {
                int iValueLength = Marshal.SizeOf(typeof (int));
                Marshal.WriteInt32(mBufferPtr, 0);
                bool bSuccess = mUsbDevice.GetPowerPolicy(PowerPolicyType.SuspendDelay, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadInt32(mBufferPtr);
                return -1;
            }
            set
            {
                int iValueLength = Marshal.SizeOf(typeof (int));
                Marshal.WriteInt32(mBufferPtr, value);
                mUsbDevice.SetPowerPolicy(PowerPolicyType.SuspendDelay, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// Frees instance resources.
        /// </summary>
        ~PowerPolicies()
        {
            if (mBufferPtr != IntPtr.Zero)
                Marshal.FreeCoTaskMem(mBufferPtr);

            mBufferPtr = IntPtr.Zero;
        }
    }
}