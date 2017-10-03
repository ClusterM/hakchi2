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
using System.Collections;
using System.Collections.Generic;
using LibUsbDotNet.Main;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// Used to iterate through the <see cref="MonoUsbProfileHandle"/> collection contained in the <see cref="MonoUsbProfileListHandle"/>.
    /// </summary>
    /// <remarks>
    /// <para>Wraps a device list handle into a <see cref="System.Runtime.ConstrainedExecution.CriticalFinalizerObject"/></para>
    /// </remarks>
    /// <seealso cref="MonoUsbProfileList"/>
    public class MonoUsbProfileListHandle : SafeContextHandle, IEnumerable<MonoUsbProfileHandle>
    {
        private MonoUsbProfileListHandle()
            : base(IntPtr.Zero) { }

        internal MonoUsbProfileListHandle(IntPtr pHandleToOwn)
            : base(pHandleToOwn) { }

        #region IEnumerable<MonoUsbProfileHandle> Members

        /// <summary>
        /// Gets a forward-only device list enumerator.
        /// </summary>
        /// <returns>A profile handle enumerator used iterating through the <see cref="MonoUsbProfileHandle"/> classes.</returns>
        public IEnumerator<MonoUsbProfileHandle> GetEnumerator() { return new MonoUsbProfileHandleEnumerator(this); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                MonoUsbApi.FreeDeviceList(handle, 1);
                //Console.WriteLine("FreeDeviceList:{0}", handle);
                SetHandleAsInvalid();
            }

            return true;
        }
    }
}