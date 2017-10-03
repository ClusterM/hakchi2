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
using System.Threading;

namespace LibUsbDotNet.Internal
{
    internal class SafeOverlapped : IDisposable
    {
        // Find the structural starting positions in the NativeOverlapped structure.
        private static readonly int FieldOffsetEventHandle = Marshal.OffsetOf(typeof (NativeOverlapped), "EventHandle").ToInt32();
        private static readonly int FieldOffsetInternalHigh = Marshal.OffsetOf(typeof (NativeOverlapped), "InternalHigh").ToInt32();
        private static readonly int FieldOffsetInternalLow = Marshal.OffsetOf(typeof (NativeOverlapped), "InternalLow").ToInt32();
        private static readonly int FieldOffsetOffsetHigh = Marshal.OffsetOf(typeof (NativeOverlapped), "OffsetHigh").ToInt32();
        private static readonly int FieldOffsetOffsetLow = Marshal.OffsetOf(typeof (NativeOverlapped), "OffsetLow").ToInt32();
        private IntPtr mPtrOverlapped = IntPtr.Zero;

        public SafeOverlapped()
        {
            // Globally allocated the memory for the overlapped structure
            mPtrOverlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (NativeOverlapped)));
        }

        public IntPtr InternalLow
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, FieldOffsetInternalLow); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, FieldOffsetInternalLow, value); }
        }

        public IntPtr InternalHigh
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, FieldOffsetInternalHigh); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, FieldOffsetInternalHigh, value); }
        }

        public int OffsetLow
        {
            get { return Marshal.ReadInt32(mPtrOverlapped, FieldOffsetOffsetLow); }
            set { Marshal.WriteInt32(mPtrOverlapped, FieldOffsetOffsetLow, value); }
        }

        public int OffsetHigh
        {
            get { return Marshal.ReadInt32(mPtrOverlapped, FieldOffsetOffsetHigh); }
            set { Marshal.WriteInt32(mPtrOverlapped, FieldOffsetOffsetHigh, value); }
        }

        /// <summary>
        /// The overlapped event wait hande.
        /// </summary>
        public IntPtr EventHandle
        {
            get { return Marshal.ReadIntPtr(mPtrOverlapped, FieldOffsetEventHandle); }
            set { Marshal.WriteIntPtr(mPtrOverlapped, FieldOffsetEventHandle, value); }
        }

        /// <summary>
        /// Pass this into the DeviceIoControl and GetOverlappedResult APIs
        /// </summary>
        public IntPtr GlobalOverlapped
        {
            get { return mPtrOverlapped; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (mPtrOverlapped != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mPtrOverlapped);
                mPtrOverlapped = IntPtr.Zero;
            }
        }

        #endregion

        /// <summary>
        /// Set the overlapped wait handle and clear out the rest of the structure.
        /// </summary>
        /// <param name="hEventOverlapped"></param>
        public void ClearAndSetEvent(IntPtr hEventOverlapped)
        {
            EventHandle = hEventOverlapped;
            InternalLow = IntPtr.Zero;
            InternalHigh = IntPtr.Zero;
            OffsetLow = 0;
            OffsetHigh = 0;
        }


        // Clean up the globally allocated memory. 
        ~SafeOverlapped() { Dispose(); }
    }
}