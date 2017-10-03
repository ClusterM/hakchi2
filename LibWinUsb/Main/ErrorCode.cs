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

using MonoLibUsb;

namespace LibUsbDotNet.Main
{
    /// <summary> 
    /// General area in which the failure occurred. See the <see cref="UsbError"/> class.
    /// </summary> 
    public enum ErrorCode
    {
        /// <summary>
        /// No error. (None, Success, and Ok)
        /// </summary>
        None = 0,
        /// <summary>
        /// No error.
        /// </summary>
        Success = 0,
        /// <summary>
        /// No error.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// The USB device has errors in its Configuration descriptor.
        /// </summary>
        InvalidConfig = -16384,
        /// <summary>
        /// A synchronous device IO operation failed.
        /// </summary>
        IoSyncFailed,
        /// <summary>
        /// A request for a string failed.
        /// </summary>
        GetString,
        /// <summary>
        /// A specified endpoint is invalid for the operation.
        /// </summary>
        InvalidEndpoint,
        /// <summary>
        /// A request to cancel IO operation failed.
        /// </summary>
        AbortEndpoint,
        /// <summary>
        /// A call to the core Win32 API DeviceIOControl failed.
        /// </summary>
        DeviceIoControl,
        /// <summary>
        /// A call to the core Win32 API GetOverlappedResult failed.
        /// </summary>
        GetOverlappedResult,
        /// <summary>
        /// An Endpoints receive thread was dangerously terminated.
        /// </summary>
        ReceiveThreadTerminated,
        /// <summary>
        /// A write operation failed.
        /// </summary>
        WriteFailed,
        /// <summary>
        /// A read operation failed.
        /// </summary>
        ReadFailed,
        /// <summary>
        /// An endpoint 0 IO control message failed.
        /// </summary>
        IoControlMessage,
        /// <summary>
        /// The action of cancelling the IO operation failed.
        /// </summary>
        CancelIoFailed,
        /// <summary>
        /// An IO operation was cancelled by the user before it completed.
        /// </summary>
        /// <remarks>
        /// IoCancelled errors may occur as normal operation; for this reason they are not logged as a <see cref="UsbError"/>.
        /// </remarks>
        IoCancelled,
        /// <summary>
        /// An IO operation timed out before it completed.
        /// </summary>
        /// <remarks>
        /// IoTimedOut errors may occur as normal operation; for this reason they are not logged as a <see cref="UsbError"/>.
        /// </remarks>
        IoTimedOut,
        /// <summary>
        /// An IO operation was cancelled and will be re-submiited when ready.
        /// </summary>
        /// <remarks>
        /// IoEndpointGlobalCancelRedo errors may occur as normal operation; for this reason they are not logged as a <see cref="UsbError"/>.
        /// </remarks>
        IoEndpointGlobalCancelRedo,
        /// <summary>
        /// Failed retrieving a custom USB device key value.
        /// </summary>
        GetDeviceKeyValueFailed,
        /// <summary>
        /// Failed setting a custom USB device key value.
        /// </summary>
        SetDeviceKeyValueFailed,
        /// <summary>
        /// The error is a standard windows error.
        /// </summary>
        Win32Error,
        /// <summary>
        /// An attempt was made to lock a device that is already locked.
        /// </summary>
        DeviceAllreadyLocked,
        /// <summary>
        /// An attempt was made to lock an endpoint that is already locked.
        /// </summary>
        EndpointAllreadyLocked,
        /// <summary>
        /// The USB device request failed because the USB device was not found.
        /// </summary>
        DeviceNotFound,
        /// <summary>
        /// Operation was intentionally cancelled by the user or application.
        /// </summary>
        UserAborted,
        /// <summary>
        /// Invalid parameter.
        /// </summary>
        InvalidParam,

        /// <summary>
        /// Access denied (insufficient permissions).
        /// </summary>
        AccessDenied,
        /// <summary>
        /// Resource Busy.
        /// </summary>
        ResourceBusy,
        /// <summary>
        /// Overflow.
        /// </summary>
        Overflow,
        /// <summary>
        /// Pipe error or endpoint halted.
        /// </summary>
        PipeError,
        /// <summary>
        /// System call interrupted (perhaps due to signal).
        /// </summary>
        Interrupted,

        /// <summary>
        /// Insufficient memory.
        /// </summary>
        InsufficientMemory,
        /// <summary>
        /// Operation not supported or unimplemented on this platform.
        /// </summary>
        NotSupported,
        /// <summary>
        /// Unknown or other error.
        /// </summary>
        UnknownError,
        /// <summary>
        /// The error is one of the <see cref="MonoUsbError"/>
        /// </summary>
        MonoApiError
    }
}