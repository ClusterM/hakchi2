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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// General utilities class used by LudnLite and exposed publicly for your convience.
    /// </summary>
    public static class Helper
    {
        private static object mIsLinux;
        private static OperatingSystem mOs;

        /// <summary>
        /// Gets the  <see cref="OperatingSystem"/> class.
        /// </summary>
        public static OperatingSystem OSVersion
        {
            get
            {
                if (ReferenceEquals(mOs, null))
                    mOs = Environment.OSVersion;

                return mOs;
            }
        }

        /// <summary>
        /// True if running on a unix-like operating system.
        /// False if using the Libusb-1.0 windows backend driver.
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                if (ReferenceEquals(mIsLinux, null))
                {
                    switch (OSVersion.Platform.ToString())
                    {
                        case "Win32S":
                        case "Win32Windows":
                        case "Win32NT":
                        case "WinCE":
                        case "Xbox":
                            mIsLinux = false;
                            break;
                        case "Unix":
                        case "MacOSX":
                            mIsLinux = true;
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Operating System:{0} not supported.", OSVersion));
                    }
                }
                return (bool)mIsLinux;
            }
        }

        /// <summary>
        /// Copies bytes to a blittable object.
        /// </summary>
        /// <param name="sourceBytes">bytes to copy</param>
        /// <param name="iStartIndex">Start index</param>
        /// <param name="iLength">number of bytes to copy</param>
        /// <param name="destObject">blittable destination object</param>
        public static void BytesToObject(byte[] sourceBytes, int iStartIndex, int iLength, object destObject)
        {
            GCHandle gch = GCHandle.Alloc(destObject, GCHandleType.Pinned);

            IntPtr ptrDestObject = gch.AddrOfPinnedObject();
            Marshal.Copy(sourceBytes, iStartIndex, ptrDestObject, iLength);

            gch.Free();
        }

        /// <summary>
        /// Returns a dictionary object of enumeration names and values.
        /// </summary>
        /// <param name="type">They <see cref="Type"/> of enumeration.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> enumeration of names and values.</returns>
        public static Dictionary<string, int> GetEnumData(Type type)
        {
            Dictionary<string, int> dictEnum = new Dictionary<string, int>();
            FieldInfo[] enumFields = type.GetFields();
            for (int iField = 1; iField < enumFields.Length; iField++)
            {
                object oValue = enumFields[iField].GetRawConstantValue();
                dictEnum.Add(enumFields[iField].Name, Convert.ToInt32(oValue));
            }

            return dictEnum;
        }


        /// <summary>
        /// Swaps low and high bytes on big endian systems.  Has no effect on little endian systems.
        /// </summary>
        /// <param name="swapValue">The value to convert.</param>
        /// <returns>a swapped value an big endian system, the same value on little endian systems</returns>
        public static short HostEndianToLE16(short swapValue)
        {
            HostEndian16BitValue rtn = new HostEndian16BitValue(swapValue);
            return (short)rtn.U16;
        }

        /// <summary>
        /// Converts standard values to decorated hex string values.
        /// </summary>
        /// <param name="standardValue">The value to convert.</param>
        /// <returns>A string representing <paramref name="standardValue"/> in hex display format.</returns>
        public static string ShowAsHex(object standardValue)
        {
            if (standardValue == null) return "";
            if (standardValue is Int64) return "0x" + ((Int64)standardValue).ToString("X16");
            if (standardValue is UInt32) return "0x" + ((UInt32)standardValue).ToString("X8");
            if (standardValue is Int32) return "0x" + ((Int32)standardValue).ToString("X8");
            if (standardValue is UInt16) return "0x" + ((UInt16)standardValue).ToString("X4");
            if (standardValue is Int16) return "0x" + ((Int16)standardValue).ToString("X4");
            if (standardValue is Byte) return "0x" + ((Byte)standardValue).ToString("X2");
            if (standardValue is String) return HexString(standardValue as byte[], "", " ");

            return "";
        }

        /// <summary>
        /// Builds a delimited string of names and values.
        /// </summary>
        /// <param name="sep0">Inserted and the begining of the entity.</param>
        /// <param name="names">The list of names for the object values.</param>
        /// <param name="sep1">Inserted between the name and value.</param>
        /// <param name="values">The values for the names.</param>
        /// <param name="sep2">Inserted and the end of the entity.</param>
        /// <returns>The formatted string.</returns>
        public static string ToString(string sep0, string[] names, string sep1, object[] values, string sep2)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < names.Length; i++)
                sb.Append(sep0 + names[i] + sep1 + values[i] + sep2);

            return sb.ToString();
        }

        #region Nested Types

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        internal struct HostEndian16BitValue
        {
            public HostEndian16BitValue(short x)
            {
                U16 = 0;
                B1 = (byte)(x >> 8);
                B0 = (byte)(x & 0xff);
            }

            [FieldOffset(0)]
            public readonly ushort U16;

            [FieldOffset(0)]
            public readonly byte B0;

            [FieldOffset(1)]
            public readonly byte B1;
        }

        #endregion

        /// <summary>
        /// Builds a formatted hexidecimal string from an array of bytes. 
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <param name="prefix">string to place before each byte</param>
        /// <param name="suffix">string to place after each byte</param>
        /// <returns>a formatted hex string</returns>
        public static string HexString(byte[] data, string prefix, string suffix)
        {
            if (prefix == null) prefix = String.Empty;
            if (suffix == null) suffix = String.Empty;

            StringBuilder sb = new StringBuilder((data.Length * 2) + (data.Length * prefix.Length) + (data.Length * suffix.Length));
            foreach (byte b in data)
                sb.Append(prefix + b.ToString("X2") + suffix);

            return sb.ToString();
        }

    }
    /// <summary>
    /// Used for allocating a <see cref="GCHandle"/> to access the underlying pointer of an object.
    /// </summary>
    public class PinnedHandle : IDisposable
    {
        private readonly IntPtr mPtr = IntPtr.Zero;
        private bool mFreeGcBuffer;
        private GCHandle mGcObject;


        /// <summary>
        /// Creates a pinned object.
        /// </summary>
        /// <param name="objectToPin">
        /// The object can be any blittable class, or array.  If a <see cref="GCHandle"/> is passed it will be used "as-is" and no pinning will take place. 
        /// </param>
        public PinnedHandle(object objectToPin)
        {
            if (!ReferenceEquals(objectToPin, null))
            {
                mFreeGcBuffer = GetPinnedObjectHandle(objectToPin, out mGcObject);
                mPtr = mGcObject.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// The raw pointer in memory of the pinned object.
        /// </summary>
        public IntPtr Handle
        {
            get { return mPtr; }
        }

        #region IDisposable Members

        /// <summary>
        /// Frees and disposes the <see cref="GCHandle"/> for this pinned object.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if ((mFreeGcBuffer) && mGcObject.IsAllocated)
            {
                mFreeGcBuffer = false;
                mGcObject.Free();
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Disposes the gchande for the object if ibe is allocated.
        /// </summary>
        ~PinnedHandle() { Dispose(); }

        private static bool GetPinnedObjectHandle(object objectToPin, out GCHandle pinnedObject)
        {
            bool bFreeGcBuffer = false;

            if (objectToPin is GCHandle)
                pinnedObject = (GCHandle)objectToPin;
            else
            {
                pinnedObject = GCHandle.Alloc(objectToPin, GCHandleType.Pinned);
                bFreeGcBuffer = true;
            }

            return bFreeGcBuffer;
        }
    }
}