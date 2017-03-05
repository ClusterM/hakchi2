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
namespace LibUsbDotNet.Internal.LibUsb
{
    internal static class LibUsbIoCtl
    {
        private const int FILE_ANY_ACCESS = 0;
        private const int FILE_DEVICE_UNKNOWN = 0x00000022;

        private const int METHOD_BUFFERED = 0;
        private const int METHOD_IN_DIRECT = 1;
        private const int METHOD_NEITHER = 3;
        private const int METHOD_OUT_DIRECT = 2;

        public static readonly int ABORT_ENDPOINT = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80F, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int CLAIM_INTERFACE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x815, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int CLEAR_FEATURE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x806, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int CONTROL_TRANSFER = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x903, METHOD_BUFFERED, FILE_ANY_ACCESS);


        public static readonly int GET_CONFIGURATION = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_CUSTOM_REG_PROPERTY = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x901, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_DESCRIPTOR = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x809, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_INTERFACE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x804, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_STATUS = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x807, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_VERSION = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x812, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int GET_REG_PROPERTY = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x900, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int INTERRUPT_OR_BULK_READ = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80B, METHOD_OUT_DIRECT, FILE_ANY_ACCESS);
        public static readonly int INTERRUPT_OR_BULK_WRITE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80A, METHOD_IN_DIRECT, FILE_ANY_ACCESS);
        public static readonly int ISOCHRONOUS_READ = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x814, METHOD_OUT_DIRECT, FILE_ANY_ACCESS);
        public static readonly int ISOCHRONOUS_WRITE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x813, METHOD_IN_DIRECT, FILE_ANY_ACCESS);
        public static readonly int RELEASE_INTERFACE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x816, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int RESET_DEVICE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x810, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int RESET_ENDPOINT = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80E, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int SET_CONFIGURATION = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int SET_DEBUG_LEVEL = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x811, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int SET_DESCRIPTOR = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x808, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int SET_FEATURE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x805, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int SET_INTERFACE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x803, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int VENDOR_READ = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80D, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static readonly int VENDOR_WRITE = CTL_CODE(FILE_DEVICE_UNKNOWN, 0x80C, METHOD_BUFFERED, FILE_ANY_ACCESS);


        private static int CTL_CODE(int DeviceType, int Function, int Method, int Access) { return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method); }
    }
}