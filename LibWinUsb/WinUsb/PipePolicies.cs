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
using LibUsbDotNet.WinUsb.Internal;

namespace LibUsbDotNet.WinUsb
{
    /// <summary> Endpoint specific policies. <see cref="WinUsbDevice.EndpointPolicies(ReadEndpointID)"/>.
    /// </summary> 
    public class PipePolicies
    {
        private const int MAX_SIZE = 4;
        private readonly byte mEpNum;
        private readonly SafeHandle mUsbHandle;

        private IntPtr mBufferPtr = IntPtr.Zero;

        internal PipePolicies(SafeHandle usbHandle, byte epNum)
        {
            mBufferPtr = Marshal.AllocCoTaskMem(MAX_SIZE);
            mEpNum = epNum;
            mUsbHandle = usbHandle;
        }

        /// <summary>
        /// If the allow partial reads policy parameter is FALSE (that is, zero), the read request fails whenever the device returns more data than the client requested. 
        /// If the allow partial reads policy parameter is TRUE, the WinUSB driver saves the extra data and sends the extra data to the client during the client's next read. 
        /// The default value of the allow partial reads policy parameter is TRUE.
        /// </summary>
        public bool AllowPartialReads
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.AllowPartialReads, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.AllowPartialReads, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// If the short packet terminate policy parameter is TRUE (that is, nonzero), every write request that is a multiple of the maximum packet size for the endpoint is terminated with a zero-length packet. 
        /// The default value of the short packet terminate policy parameter is FALSE.
        /// </summary>
        public bool ShortPacketTerminate
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.ShortPacketTerminate, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.ShortPacketTerminate, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// If the auto clear stall policy parameter is TRUE (that is, nonzero), the driver fails stalled data transfers, but the driver clears the stall condition automatically, and data continues to flow on the pipe. This policy parameter does not affect control pipes. 
        /// The default value for the auto clear stall policy parameter is FALSE.
        /// </summary>
        public bool AutoClearStall
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.AutoClearStall, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.AutoClearStall, iValueLength, mBufferPtr);
            }
        }


        ///<summary>
        /// The auto flush policy parameter works with allow partial reads. If allow partial reads is FALSE, the WinUSB driver ignores the value of auto flush. If allow partial reads is TRUE, the value of auto flush determines what the WinUSB driver does when the device returns more data than the client requested. 
        /// If both allow partial reads and auto flush policy parameters are TRUE (that is, nonzero) and the device returns more data than the client requested, the remaining data is discarded. If allow partial reads is TRUE, but auto flush is FALSE, the WinUSB driver caches the extra data and sends it to the client in the next read operation. 
        /// The default value of the auto flush policy parameter is FALSE.
        ///</summary>
        public bool AutoFlush
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.AutoFlush, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.AutoFlush, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// If the ignore short packets policy parameter is TRUE (that is, nonzero), the host does not complete a read operation after it receives a short packet. Instead, the the host completes the operation only after the host has read the specified number of bytes. 
        /// If the ignore short packets policy parameter is FALSE, the host completes a read operation when either the host has read the specified number of bytes or the host has received a short packet. 
        /// The default value of the ignore short packets policy parameter is FALSE.
        /// </summary>
        public bool IgnoreShortPackets
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.IgnoreShortPackets, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.IgnoreShortPackets, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// If the raw i/o policy parameter is TRUE (that is, nonzero), calls to WinUsb_ReadPipe and WinUsb_WritePipe for the specified endpoint must satisfy the following conditions:
        /// The buffer length must be a multiple of the maximum endpoint packet size. 
        /// The length must be less than what the host controller supports. 
        /// If the preceding conditions are met, WinUSB sends data directly to the USB driver stack, bypassing WinUSB's queuing and error handling. 
        /// If the raw i/o policy parameter is FALSE, no restrictions are imposed on the buffers that are passed to WinUsb_ReadPipe and WinUsb_WritePipe. 
        /// The default value of the raw i/o policy parameter is FALSE.
        /// </summary>
        public bool RawIo
        {
            get
            {
                int iValueLength = 1;
                Marshal.WriteByte(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.RawIo, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadByte(mBufferPtr) == 0 ? false : true;
                return false;
            }
            set
            {
                int iValueLength = 1;
                byte bPipePolicyValue = (value) ? (byte) 1 : (byte) 0;
                Marshal.WriteByte(mBufferPtr, bPipePolicyValue);
                SetPipePolicy(PipePolicyType.RawIo, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// The pipe transfer timeout policy parameter specifies the time-out interval, in milliseconds. The host cancels transfers that do not complete within the time-out interval. A value of zero means that transfers do not time out. 
        /// By default, the time-out value is zero, and the host never cancels a transfer because of a time-out.
        /// </summary>
        public int PipeTransferTimeout
        {
            get
            {
                int iValueLength = 4;
                Marshal.WriteInt32(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.PipeTransferTimeout, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadInt32(mBufferPtr);
                return -1;
            }
            set
            {
                int iValueLength = 4;
                Marshal.WriteInt32(mBufferPtr, value);
                SetPipePolicy(PipePolicyType.PipeTransferTimeout, iValueLength, mBufferPtr);
            }
        }

        /// <summary>
        /// The maximum number of bytes that can be transferred at once.
        /// </summary>
        public int MaxTransferSize
        {
            get
            {
                int iValueLength = 4;
                Marshal.WriteInt32(mBufferPtr, 0);
                bool bSuccess = GetPipePolicy(PipePolicyType.MaximumTransferSize, ref iValueLength, mBufferPtr);
                if (bSuccess)
                    return Marshal.ReadInt32(mBufferPtr);
                return -1;
            }
        }


        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="T:System.Object"/>.
        ///</returns>
        public override string ToString()
        {
            object[] o = new object[]
                             {
                                 AllowPartialReads, ShortPacketTerminate, AutoClearStall, AutoFlush, IgnoreShortPackets, RawIo, PipeTransferTimeout,
                                 MaxTransferSize
                             };
            return
                string.Format(
                    "AllowPartialReads:{0}\r\nShortPacketTerminate:{1}\r\nAutoClearStall:{2}\r\nAutoFlush:{3}\r\nIgnoreShortPackets:{4}\r\nRawIO:{5}\r\nPipeTransferTimeout:{6}\r\nMaxTransferSize:{7}\r\n",
                    o);
        }


        internal bool GetPipePolicy(PipePolicyType policyType, ref int valueLength, IntPtr pBuffer)
        {
            bool bSuccess = WinUsbAPI.WinUsb_GetPipePolicy(mUsbHandle, mEpNum, policyType, ref valueLength, pBuffer);

            if (!bSuccess) UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetPipePolicy", this);

            return bSuccess;
        }

        internal bool SetPipePolicy(PipePolicyType policyType, int valueLength, IntPtr pBuffer)
        {
            bool bSuccess = WinUsbAPI.WinUsb_SetPipePolicy(mUsbHandle, mEpNum, policyType, valueLength, pBuffer);

            if (!bSuccess) UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "SetPipePolicy", this);

            return bSuccess;
        }

        /// <summary>
        /// Frees instance resources.
        /// </summary>
        ~PipePolicies()
        {
            if (mBufferPtr != IntPtr.Zero)
                Marshal.FreeCoTaskMem(mBufferPtr);

            mBufferPtr = IntPtr.Zero;
        }
    }
}