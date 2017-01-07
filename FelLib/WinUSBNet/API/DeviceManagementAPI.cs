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
using System.Runtime.InteropServices;

namespace MadWizard.WinUSBNet.API
{
	/// <summary>
	/// API declarations relating to device management (SetupDixxx and 
	/// RegisterDeviceNotification functions).   
	/// </summary>

	internal static partial class DeviceManagement
	{
		// from dbt.h

        internal const Int32 DBT_DEVICEARRIVAL = 0X8000;
        internal const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
        private const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        private const Int32 DBT_DEVTYP_HANDLE = 6;
        private const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        private const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        private const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        internal const Int32 WM_DEVICECHANGE = 0X219;

		// from setupapi.h

        private const Int32 DIGCF_PRESENT = 2;
        private const Int32 DIGCF_DEVICEINTERFACE = 0X10;
        private const Int32 DIGCF_ALLCLASSES = 0X04;

		// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

		// Use this one in the call to RegisterDeviceNotification() and
		// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:

		[StructLayout(LayoutKind.Sequential)]
        private class DEV_BROADCAST_DEVICEINTERFACE
		{
			internal Int32 dbcc_size;
			internal Int32 dbcc_devicetype;
			internal Int32 dbcc_reserved;
			internal Guid dbcc_classguid;
			internal Int16 dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DEV_BROADCAST_DEVICEINTERFACE_1
		{
			internal Int32 dbcc_size;
			internal Int32 dbcc_devicetype;
			internal Int32 dbcc_reserved;
            internal Guid dbcc_classguid; 
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			internal Char[] dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential)]
        private class DEV_BROADCAST_HDR
		{
			internal Int32 dbch_size;
			internal Int32 dbch_devicetype;
			internal Int32 dbch_reserved;
		}

        private struct SP_DEVICE_INTERFACE_DATA
		{
			internal Int32 cbSize;
			internal System.Guid InterfaceClassGuid;
			internal Int32 Flags;
			internal IntPtr Reserved;
		}
        private struct SP_DEVINFO_DATA
        {
            internal Int32 cbSize;
            internal System.Guid ClassGuid;
            internal Int32 DevInst;
            internal IntPtr Reserved;
        }

        // from pinvoke.net
        private enum SPDRP : uint
        {
            SPDRP_DEVICEDESC = 0x00000000,
            SPDRP_HARDWAREID = 0x00000001,
            SPDRP_COMPATIBLEIDS = 0x00000002,
            SPDRP_NTDEVICEPATHS = 0x00000003,
            SPDRP_SERVICE = 0x00000004,
            SPDRP_CONFIGURATION = 0x00000005,
            SPDRP_CONFIGURATIONVECTOR = 0x00000006,
            SPDRP_CLASS = 0x00000007,
            SPDRP_CLASSGUID = 0x00000008,
            SPDRP_DRIVER = 0x00000009,
            SPDRP_CONFIGFLAGS = 0x0000000A,
            SPDRP_MFG = 0x0000000B,
            SPDRP_FRIENDLYNAME = 0x0000000C,
            SPDRP_LOCATION_INFORMATION = 0x0000000D,
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E,
            SPDRP_CAPABILITIES = 0x0000000F,
            SPDRP_UI_NUMBER = 0x00000010,
            SPDRP_UPPERFILTERS = 0x00000011,
            SPDRP_LOWERFILTERS = 0x00000012,
            SPDRP_MAXIMUM_PROPERTY = 0x00000013,

            SPDRP_ENUMERATOR_NAME = 0x16,
        };


        private enum RegTypes : int
        {
            // incomplete list, these are just the ones used.
            REG_SZ = 1,
            REG_MULTI_SZ = 7
        }


        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

		//[DllImport("setupapi.dll", SetLastError = true)]
		//internal static extern Int32 SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, Int32 hwndParent);

		[DllImport("setupapi.dll", SetLastError = true)]
		private static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		[DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, out int PropertyRegDataType, byte[] PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, IntPtr PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out UInt32 RequiredSize);
    
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterDeviceNotification(IntPtr Handle);

        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
	}
}
