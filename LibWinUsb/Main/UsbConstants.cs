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
namespace LibUsbDotNet.Main
{
    /// <summary> Various USB constants.
    /// </summary> 
    public static class UsbConstants
    {
        /// <summary>
        /// Default timeout for all USB IO operations.
        /// </summary>
        public const int DEFAULT_TIMEOUT = 1000;

        internal const bool EXIT_CONTEXT = false;

        /// <summary>
        /// Maximum size of a config descriptor.
        /// </summary>
        public const int MAX_CONFIG_SIZE = 4096;

        /// <summary>
        /// Maximum number of USB devices.
        /// </summary>
        public const int MAX_DEVICES = 128;

        /// <summary>
        /// Maximum number of endpoints per device.
        /// </summary>
        public const int MAX_ENDPOINTS = 32;

        /// <summary>
        /// Endpoint direction mask.
        /// </summary>
        public const byte ENDPOINT_DIR_MASK = 0x80;

        /// <summary>
        /// Endpoint number mask.
        /// </summary>
        public const byte ENDPOINT_NUMBER_MASK = 0xf;

        ///// <summary>
        ///// See <see cref="UsbError.Handled"/>.  Number of RETRIES before failed regardless of the handled field value.
        ///// </summary>
        //public const int MAX_FAIL_RETRIES_ON_HANDLED_ERROR = 4;

    }
}