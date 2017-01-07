/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

/* NOTE: Parts of the code in this file are based on the work of Jan Axelson
 * See http://www.lvr.com/winusb.htm for more information
 */

using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace MadWizard.WinUSBNet.API
{
	///  <summary>
	///  API declarations relating to file I/O (and used by WinUsb).
	///  </summary>

	sealed internal class FileIO
	{
		public const Int32 FILE_ATTRIBUTE_NORMAL = 0X80;
        public const Int32 FILE_FLAG_OVERLAPPED = 0X40000000;
        public const Int32 FILE_SHARE_READ = 1;
        public const Int32 FILE_SHARE_WRITE = 2;
        public const UInt32 GENERIC_READ = 0X80000000;
        public const UInt32 GENERIC_WRITE = 0X40000000;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public const Int32 OPEN_EXISTING = 3;

        public const Int32 ERROR_IO_PENDING = 997;

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);
	}

}
