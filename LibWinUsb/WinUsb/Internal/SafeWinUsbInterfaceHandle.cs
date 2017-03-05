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
using LibUsbDotNet.WinUsb.Internal;

namespace LibUsbDotNet.Internal.WinUsb
{
    internal class SafeWinUsbInterfaceHandle : SafeHandle
    {
        public SafeWinUsbInterfaceHandle()
            : base(IntPtr.Zero, true) { }

        public SafeWinUsbInterfaceHandle(IntPtr handle)
            : base(handle, true) { }

        ///<summary>
        ///Gets a value indicating whether the <see cref="SafeWinUsbInterfaceHandle"/> value is invalid.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="SafeWinUsbInterfaceHandle"/> is valid; otherwise, false.
        ///</returns>
        public override bool IsInvalid
        {
            get { return (handle == IntPtr.Zero || handle.ToInt64() == -1); }
        }

        ///<summary>
        ///Executes the code required to free the <see cref="SafeWinUsbInterfaceHandle"/>.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="SafeWinUsbInterfaceHandle"/> is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        ///</returns>
        ///
        protected override bool ReleaseHandle()
        {
            bool bSuccess = true;
            if (!IsInvalid)
            {
                bSuccess = WinUsbAPI.WinUsb_Free(handle);
                handle = IntPtr.Zero;
            }
            return bSuccess;
        }
    }
}