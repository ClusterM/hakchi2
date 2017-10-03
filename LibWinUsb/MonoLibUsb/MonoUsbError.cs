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

namespace MonoLibUsb
{
    /// <summary>
    /// Error codes.
    /// Most libusb functions return 0 on success or one of these codes on failure. 
    /// </summary>
    public enum MonoUsbError
    {
        /// <summary>
        /// Success (no error) 
        /// </summary>
        Success = 0,

        /// <summary>
        /// Input/output error 
        /// </summary>
        ErrorIO = -1,

        /// <summary>
        /// Invalid parameter 
        /// </summary>
        ErrorInvalidParam = -2,

        /// <summary>
        /// Access denied (insufficient permissions) 
        /// </summary>
        ErrorAccess = -3,

        /// <summary>
        /// No such device (it may have been disconnected) 
        /// </summary>
        ErrorNoDevice = -4,

        /// <summary>
        /// Entity not found 
        /// </summary>
        ErrorNotFound = -5,

        /// <summary>
        /// Resource busy 
        /// </summary>
        ErrorBusy = -6,

        /// <summary>
        /// Operation timed out 
        /// </summary>
        ErrorTimeout = -7,

        /// <summary>
        /// Overflow 
        /// </summary>
        ErrorOverflow = -8,

        /// <summary>
        /// Pipe error 
        /// </summary>
        ErrorPipe = -9,

        /// <summary>
        /// System call interrupted (perhaps due to signal) 
        /// </summary>
        ErrorInterrupted = -10,

        /// <summary>
        /// Insufficient memory 
        /// </summary>
        ErrorNoMem = -11,

        /// <summary>
        /// Operation not supported or unimplemented on this platform 
        /// </summary>
        ErrorNotSupported = -12,

        /// <summary>
        /// Cancel IO failed.
        /// </summary>
        ErrorIOCancelled = -13,

        /// <summary>
        /// Other error 
        /// </summary>
        ErrorOther = -99,
    } ;
}