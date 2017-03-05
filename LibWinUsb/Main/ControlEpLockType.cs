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
namespace LibUsbDotNet.Main
{
    /// <summary> The locking strategy for endpoint 0 operations in regards to any pending endpoint data operations.
    /// </summary> 
    /// <remarks> Some USB devices have problems handling endpoint 0 control messages when other endpoints on the device have pending IO operations.
    /// Must be combined with <see cref="LibUsbDotNet.Main.DeviceLockType.Locked"/>.
    /// </remarks> 
    public enum ControlEpLockType
    {
        /// <summary>
        /// Endpoint 0 transfers do not interfere with data endpoint transfers in any way. This is the default.
        /// </summary>
        None,
        /// <summary>
        /// If data endpoints for the device remain busy for longer than <see cref="UsbDevice.DeviceLockTimeout"/>, then force an IO cancel operation for the busy endpoints.
        /// </summary>
        /// <remarks>
        /// There is a potential for loss of data when using this option.
        /// This will immediately cancel pending I/O transfers on all data endpoints when a enpoint 0 control transfer is initiated.
        /// </remarks>
        CancelIoOnLockTimeout,
        /// <summary>
        /// If data endpoints for the device remain busy for longer than <see cref="UsbDevice.DeviceLockTimeout"/>, then consider the remaining endpoints with IO pending operation as idle and continue with operation.
        /// This option is the same as ErrorOnLockTimeout only instead of returning with an error after the lock timeout period, an attempt is made to transfer data anyways. (Ignoring the lock)
        /// </summary>
        ContinueOnLockTimeout,
        /// <summary>
        /// If data endpoints for the device remain busy for longer than <see cref="UsbDevice.DeviceLockTimeout"/>, then return a LockTimeout error code.
        /// Setting this value will lock ALL the devices data endpoints (1-15) before transfering data to the control enpoint 0.
        /// </summary>
        ErrorOnLockTimeout
    }
}