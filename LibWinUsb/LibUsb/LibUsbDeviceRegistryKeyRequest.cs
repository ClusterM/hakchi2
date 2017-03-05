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
using System.Text;
using LibUsbDotNet.Internal.LibUsb;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.LibUsb
{
    internal static class LibUsbDeviceRegistryKeyRequest
    {
        public static byte[] RegGetRequest(string name, int valueBufferSize)
        {
            if (valueBufferSize < 1 || name.Trim().Length == 0) throw new UsbException("Global", "Invalid DeviceRegistry het parameter.");

            LibUsbRequest req = new LibUsbRequest();
            int uOffset = LibUsbRequest.Size;
            req.DeviceRegKey.KeyType = (int) KeyType.RegBinary;

            byte[] bytesName = Encoding.Unicode.GetBytes(name + "\0");

            req.DeviceRegKey.NameOffset = uOffset;
            uOffset += bytesName.Length;
            req.DeviceRegKey.ValueOffset = uOffset;
            req.DeviceRegKey.ValueLength = (valueBufferSize);

            uOffset += Math.Max(uOffset + 1, valueBufferSize - (LibUsbRequest.Size + bytesName.Length));
            byte[] buffer = new byte[uOffset];
            byte[] regBytes = req.Bytes;

            Array.Copy(regBytes, buffer, regBytes.Length);
            Array.Copy(bytesName, 0, buffer, req.DeviceRegKey.NameOffset, bytesName.Length);

            return buffer;
        }

        public static byte[] RegSetBinaryRequest(string name, byte[] value)
        {
            LibUsbRequest req = new LibUsbRequest();
            int uOffset = LibUsbRequest.Size;
            req.DeviceRegKey.KeyType = (int) KeyType.RegBinary;

            byte[] bytesName = Encoding.Unicode.GetBytes(name + "\0");
            byte[] bytesValue = value;

            req.DeviceRegKey.NameOffset = uOffset;
            uOffset += bytesName.Length;
            req.DeviceRegKey.ValueOffset = uOffset;
            req.DeviceRegKey.ValueLength = bytesValue.Length;

            uOffset += bytesValue.Length;
            byte[] buffer = new byte[uOffset];
            byte[] regBytes = req.Bytes;

            Array.Copy(regBytes, buffer, regBytes.Length);
            Array.Copy(bytesName, 0, buffer, req.DeviceRegKey.NameOffset, bytesName.Length);
            Array.Copy(bytesValue, 0, buffer, req.DeviceRegKey.ValueOffset, bytesValue.Length);

            return buffer;
        }

        public static byte[] RegSetStringRequest(string name, string value)
        {
            LibUsbRequest req = new LibUsbRequest();
            int uOffset = LibUsbRequest.Size;
            req.DeviceRegKey.KeyType = (int) KeyType.RegSz;

            byte[] bytesName = Encoding.Unicode.GetBytes(name + "\0");
            byte[] bytesValue = Encoding.Unicode.GetBytes(value + "\0");

            req.DeviceRegKey.NameOffset = uOffset;
            uOffset += bytesName.Length;
            req.DeviceRegKey.ValueOffset = uOffset;
            req.DeviceRegKey.ValueLength = bytesValue.Length;

            uOffset += bytesValue.Length;
            byte[] buffer = new byte[uOffset];
            byte[] regBytes = req.Bytes;

            Array.Copy(regBytes, buffer, regBytes.Length);
            Array.Copy(bytesName, 0, buffer, req.DeviceRegKey.NameOffset, bytesName.Length);
            Array.Copy(bytesValue, 0, buffer, req.DeviceRegKey.ValueOffset, bytesValue.Length);

            return buffer;
        }

        #region Nested Types

        private enum KeyType
        {
            RegSz = 1, // Unicode nul terminated string
            RegBinary = 3, // Free form binary
        }

        #endregion
    }
}