// Copyright © 2006-2009 Travis Robinson. All rights reserved.
// 
// website: sourceforge.net/projects/libusbdotnet/
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

namespace LibUsbDotNet.Main
{
    /// <summary> The locking strategy for endpoint 0 operations per proccess/thread
    /// </summary> 
    [Flags]
    public enum DeviceLockType
    {
        /// <summary>
        /// Don't lock the device IO functions.
        /// </summary>
        /// <remarks>
        /// This option is not thread/proccess safe.
        /// </remarks>
        None,
        /// <summary>
        /// Use a global semapore to lock endpoint I/O operations.  This prevents multiple threads/proccesses from accessing
        /// the <see cref="UsbDevice"/> IO functions (IE <see cref="UsbDevice.ControlTransfer(ref UsbSetupPacket,object,int,out int)"/><see cref="UsbDevice.GetString"/>, etc..) at the same time. 
        /// </summary>
        /// <remarks>
        /// Using this option will allow your LibUsbDotNet applications to comunicate cooperatively with the same USB device.
        /// NOTE: Usb devices using the WinUsb.dll driver can't be shared by multiple processes.
        /// </remarks>
        Locked,
    }
}