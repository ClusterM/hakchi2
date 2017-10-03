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
namespace LibUsbDotNet.LibUsb
{
    /// <summary>
    /// Kernel types supported by LibUsbDotNet.  See <see cref="UsbDevice.KernelType"/> for more details.
    /// </summary>
    public enum LibUsbKernelType
    {
        /// <summary>
        /// LibUsb support us unavailable.
        /// </summary>
        Unknown,
        /// <summary>
        /// LibUsbDotNet native kernel driver detected.
        /// </summary>
        NativeLibUsb,
        /// <summary>
        /// Original libusb-win32 kernel driver detected.
        /// </summary>
        LegacyLibUsb,
        /// <summary>
        /// mono-linux libusb 1.x driver detected.
        /// </summary>
        MonoLibUsb
    }
}