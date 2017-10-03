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
using System.Text;

namespace LibUsbDotNet.Descriptors
{
    internal abstract class UsbMemChunk
    {
        private readonly int mMaxSize;

        private IntPtr mMemPointer = IntPtr.Zero;

        protected UsbMemChunk(int maxSize)
        {
            mMaxSize = maxSize;
            mMemPointer = Marshal.AllocHGlobal(maxSize);
        }

        public int MaxSize
        {
            get { return mMaxSize; }
        }

        public IntPtr Ptr
        {
            get { return mMemPointer; }
        }

        public void Free()
        {
            if (mMemPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(mMemPointer);
                mMemPointer = IntPtr.Zero;
            }
        }

        ~UsbMemChunk() { Free(); }
    }

    internal class LangStringDescriptor : UsbMemChunk
    {
        #region FIELD_OFFSETS

        private static readonly int OfsDescriptorType = Marshal.OffsetOf(typeof (UsbDescriptor), "DescriptorType").ToInt32();
        private static readonly int OfsLength = Marshal.OffsetOf(typeof (UsbDescriptor), "Length").ToInt32();

        #endregion

        public LangStringDescriptor(int maxSize)
            : base(maxSize) { }

        public DescriptorType DescriptorType
        {
            get { return (DescriptorType) Marshal.ReadByte(Ptr, OfsDescriptorType); }
            set { Marshal.WriteByte(Ptr, OfsDescriptorType, (byte) value); }
        }

        public byte Length
        {
            get { return Marshal.ReadByte(Ptr, OfsLength); }
            set { Marshal.WriteByte(Ptr, OfsLength, value); }
        }

        public bool Get(out short[] langIds)
        {
            langIds = new short[0];
            int totalLength = Length;
            if (totalLength <= 2) return false;

            int elementCount = (totalLength - 2)/2;
            langIds = new short[elementCount];

            int startOffset = UsbDescriptor.Size;
            for (int iElement = 0; iElement < langIds.Length; iElement++)
            {
                langIds[iElement] = Marshal.ReadInt16(Ptr, startOffset + (sizeof (ushort)*iElement));
            }
            return true;
        }

        public bool Get(out byte[] bytes)
        {
            bytes = new byte[Length];
            Marshal.Copy(Ptr, bytes, 0, bytes.Length);
            return true;
        }

        public bool Get(out string str)
        {
            str = string.Empty;

            byte[] bytes;
            if (Get(out bytes))
            {
                if (bytes.Length <= UsbDescriptor.Size)
                {
                    str = string.Empty;
                }
                else
                {
                    str = Encoding.Unicode.GetString(bytes, UsbDescriptor.Size, bytes.Length - UsbDescriptor.Size);
                }
                return true;
            }

            return false;
        }
    }
}