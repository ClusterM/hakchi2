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
using System.Runtime.InteropServices;

namespace LibUsbDotNet.Main
{
    ///<summary>
    /// Contains version information for the LibUsb Sys driver.
    ///</summary>
    /// <remarks>
    /// This version is not related to LibUsbDotNet.  TO get the LibUsbDotNet version use .NET reflections.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UsbKernelVersion
    {
        /// <summary>
        /// True if Major == 0 and Minor == 0 and Micro == 0 and Nano == 0.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (Major == 0 && Minor == 0 && Micro == 0 && Nano == 0) return true;
                return false;
            }
        }

        internal UsbKernelVersion(int major, int minor, int micro, int nano, int bcdLibUsbDotNetKernelMod)
        {
            Major = major;
            Minor = minor;
            Micro = micro;
            Nano = nano;
            BcdLibUsbDotNetKernelMod = bcdLibUsbDotNetKernelMod;
        }

        /// <summary>
        /// LibUsb-Win32 Major version
        /// </summary>
        public readonly int Major;

        /// <summary>
        /// LibUsb-Win32 Minor version
        /// </summary>
        public readonly int Minor;

        /// <summary>
        /// LibUsb-Win32 Micro version
        /// </summary>
        public readonly int Micro;

        /// <summary>
        /// LibUsb-Win32 Nano version
        /// </summary>
        public readonly int Nano;

        /// <summary>
        /// The LibUsbDotNet - LibUsb-Win32 binary mod code. if not running the LibUsbDotNet LibUsb-Win32 modified kernel driver, this value is 0.
        /// </summary>
        public readonly int BcdLibUsbDotNetKernelMod;


        ///<summary>
        ///The full LibUsb-Win32 kernel driver version (libusb0.sys).
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> containing the full LibUsb-Win32 version.
        ///</returns>
        public override string ToString() { return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Micro, Nano); }
    }
}