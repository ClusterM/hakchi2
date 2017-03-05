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
using System.Runtime.InteropServices;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// A forward-only enumerator for iterating a device lists.
    /// </summary>
    internal class MonoUsbProfileHandleEnumerator : IEnumerator<MonoUsbProfileHandle>
    {
        private readonly MonoUsbProfileListHandle mProfileListHandle;
        private MonoUsbProfileHandle mCurrentProfile;
        private int mNextDeviceProfilePos;

        
        internal MonoUsbProfileHandleEnumerator(MonoUsbProfileListHandle profileListHandle)
        {
            mProfileListHandle = profileListHandle;
            Reset();
        }

        #region IEnumerator<MonoUsbProfileHandle> Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() { Reset(); }


        /// <summary>
        /// Advances the enumerator to the next <see cref="MonoUsbProfileHandle"/> element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            IntPtr pNextProfileHandle =
                Marshal.ReadIntPtr(new IntPtr(mProfileListHandle.DangerousGetHandle().ToInt64() + (mNextDeviceProfilePos*IntPtr.Size)));
            if (pNextProfileHandle != IntPtr.Zero)
            {
                mCurrentProfile = new MonoUsbProfileHandle(pNextProfileHandle);
                mNextDeviceProfilePos++;
                return true;
            }
            mCurrentProfile = null;
            return false;
        }


        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public void Reset()
        {
            mNextDeviceProfilePos = 0;
            mCurrentProfile = null;
        }

        /// <summary>
        /// Gets the element in the <see cref="MonoUsbProfileHandle"/> collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The current <see cref="MonoUsbProfileHandle"/> element.
        /// </returns>
        public MonoUsbProfileHandle Current
        {
            get { return mCurrentProfile; }
        }

        /// <summary>
        /// Gets the current element in the <see cref="MonoUsbProfileHandle"/> collection.
        /// </summary>
        /// <returns>
        /// The current <see cref="MonoUsbProfileHandle"/> element.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception><filterpriority>2</filterpriority>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion
    }
}