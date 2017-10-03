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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

// ReSharper disable InconsistentNaming

namespace LibUsbDotNet.Internal
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Kernel32
    {
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        private static readonly StringBuilder m_sbSysMsg = new StringBuilder(1024);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(string fileName,
                                                       [MarshalAs(UnmanagedType.U4)] NativeFileAccess fileAccess,
                                                       [MarshalAs(UnmanagedType.U4)] NativeFileShare fileShare,
                                                       IntPtr securityAttributes,
                                                       [MarshalAs(UnmanagedType.U4)] NativeFileMode creationDisposition,
                                                       NativeFileFlag flags,
                                                       IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int FormatMessage(int dwFlags,
                                                IntPtr lpSource,
                                                int dwMessageId,
                                                int dwLanguageId,
                                                [Out] StringBuilder lpBuffer,
                                                int nSize,
                                                IntPtr lpArguments);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool GetOverlappedResult(SafeHandle hDevice, IntPtr lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait);


        public static string FormatSystemMessage(int dwMessageId)
        {
            lock (m_sbSysMsg)
            {
                int ret = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,
                                        IntPtr.Zero,
                                        dwMessageId,
                                        CultureInfo.CurrentCulture.LCID,
                                        m_sbSysMsg,
                                        m_sbSysMsg.Capacity - 1,
                                        IntPtr.Zero);

                if (ret > 0) return m_sbSysMsg.ToString(0, ret);
                return null;
            }
        }

        #region DeviceIoControl

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool DeviceIoControl(SafeHandle hDevice,
                                                  int IoControlCode,
                                                  [MarshalAs(UnmanagedType.AsAny), In] object InBuffer,
                                                  int nInBufferSize,
                                                  IntPtr OutBuffer,
                                                  int nOutBufferSize,
                                                  out int pBytesReturned,
                                                  IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool DeviceIoControl(SafeHandle hDevice,
                                                  int IoControlCode,
                                                  [MarshalAs(UnmanagedType.AsAny), In] object InBuffer,
                                                  int nInBufferSize,
                                                  [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), Out] byte[] OutBuffer,
                                                  int nOutBufferSize,
                                                  out int pBytesReturned,
                                                  IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool DeviceIoControl(SafeHandle hDevice,
                                                  int IoControlCode,
                                                  IntPtr InBuffer,
                                                  int nInBufferSize,
                                                  IntPtr OutBuffer,
                                                  int nOutBufferSize,
                                                  out int pBytesReturned,
                                                  IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, EntryPoint = "DeviceIoControl")]
        public static extern bool DeviceIoControlAsObject(SafeHandle hDevice,
                                                          int IoControlCode,
                                                          [MarshalAs(UnmanagedType.AsAny), In] object InBuffer,
                                                          int nInBufferSize,
                                                          IntPtr OutBuffer,
                                                          int nOutBufferSize,
                                                          ref int pBytesReturned,
                                                          IntPtr Overlapped);

        #endregion
    }

    [Flags]
    internal enum NativeFileAccess : uint
    {
        FILE_SPECIAL = 0,
        FILE_APPEND_DATA = (0x0004), // file
        FILE_READ_DATA = (0x0001), // file & pipe
        FILE_WRITE_DATA = (0x0002), // file & pipe
        FILE_READ_EA = (0x0008), // file & directory
        FILE_WRITE_EA = (0x0010), // file & directory
        FILE_READ_ATTRIBUTES = (0x0080), // all
        FILE_WRITE_ATTRIBUTES = (0x0100), // all
        DELETE = 0x00010000,
        READ_CONTROL = (0x00020000),
        WRITE_DAC = (0x00040000),
        WRITE_OWNER = (0x00080000),
        SYNCHRONIZE = (0x00100000),
        STANDARD_RIGHTS_REQUIRED = (0x000F0000),
        STANDARD_RIGHTS_READ = (READ_CONTROL),
        STANDARD_RIGHTS_WRITE = (READ_CONTROL),
        STANDARD_RIGHTS_EXECUTE = (READ_CONTROL),
        STANDARD_RIGHTS_ALL = (0x001F0000),
        SPECIFIC_RIGHTS_ALL = (0x0000FFFF),
        FILE_GENERIC_READ = (STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE),
        FILE_GENERIC_WRITE = (STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE),
        SPECIAL = 0
    }

    internal enum NativeFileMode : uint
    {
        CREATE_NEW = 1,
        CREATE_ALWAYS = 2,
        OPEN_EXISTING = 3,
        OPEN_ALWAYS = 4,
        TRUNCATE_EXISTING = 5,
    }

    [Flags]
    internal enum NativeFileShare : uint
    {
        NONE = 0,
        FILE_SHARE_READ = 0x00000001,
        FILE_SHARE_WRITE = 0x00000002,
        FILE_SHARE_DEELETE = 0x00000004,
    }

    [Flags]
    internal enum NativeFileFlag : uint
    {
        FILE_ATTRIBUTE_READONLY = 0x00000001,
        FILE_ATTRIBUTE_HIDDEN = 0x00000002,
        FILE_ATTRIBUTE_SYSTEM = 0x00000004,
        FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
        FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
        FILE_ATTRIBUTE_DEVICE = 0x00000040,
        FILE_ATTRIBUTE_NORMAL = 0x00000080,
        FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
        FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
        FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
        FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
        FILE_ATTRIBUTE_OFFLINE = 0x00001000,
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
        FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
        FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
        FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
        FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
        FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
        FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000,
    }
}