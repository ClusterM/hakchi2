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
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet.Main;
using LibUsbDotNet.WinUsb;
using Microsoft.Win32;

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable UnusedParameter.Local

namespace LibUsbDotNet.Internal
{
    internal class SetupApi
    {
        #region Delegates

        public delegate bool ClassEnumeratorDelegate(
            IntPtr DeviceInfoSet, int deviceIndex, ref SP_DEVINFO_DATA DeviceInfoData, object classEnumeratorCallbackParam1);

        #endregion

        #region Enumerations

        #region CR enum

        public enum CR
        {
            SUCCESS = (0x00000000),
            DEFAULT = (0x00000001),
            OUT_OF_MEMORY = (0x00000002),
            INVALID_POINTER = (0x00000003),
            INVALID_FLAG = (0x00000004),
            INVALID_DEVNODE = (0x00000005),
            INVALID_DEVINST = INVALID_DEVNODE,
            INVALID_RES_DES = (0x00000006),
            INVALID_LOG_CONF = (0x00000007),
            INVALID_ARBITRATOR = (0x00000008),
            INVALID_NODELIST = (0x00000009),
            DEVNODE_HAS_REQS = (0x0000000A),
            DEVINST_HAS_REQS = DEVNODE_HAS_REQS,
            INVALID_RESOURCEID = (0x0000000B),
            DLVXD_NOT_FOUND = (0x0000000C), // WIN 95 ONLY
            NO_SUCH_DEVNODE = (0x0000000D),
            NO_SUCH_DEVINST = NO_SUCH_DEVNODE,
            NO_MORE_LOG_CONF = (0x0000000E),
            NO_MORE_RES_DES = (0x0000000F),
            ALREADY_SUCH_DEVNODE = (0x00000010),
            ALREADY_SUCH_DEVINST = ALREADY_SUCH_DEVNODE,
            INVALID_RANGE_LIST = (0x00000011),
            INVALID_RANGE = (0x00000012),
            FAILURE = (0x00000013),
            NO_SUCH_LOGICAL_DEV = (0x00000014),
            CREATE_BLOCKED = (0x00000015),
            NOT_SYSTEM_VM = (0x00000016), // WIN 95 ONLY
            REMOVE_VETOED = (0x00000017),
            APM_VETOED = (0x00000018),
            INVALID_LOAD_TYPE = (0x00000019),
            BUFFER_SMALL = (0x0000001A),
            NO_ARBITRATOR = (0x0000001B),
            NO_REGISTRY_HANDLE = (0x0000001C),
            REGISTRY_ERROR = (0x0000001D),
            INVALID_DEVICE_ID = (0x0000001E),
            INVALID_DATA = (0x0000001F),
            INVALID_API = (0x00000020),
            DEVLOADER_NOT_READY = (0x00000021),
            NEED_RESTART = (0x00000022),
            NO_MORE_HW_PROFILES = (0x00000023),
            DEVICE_NOT_THERE = (0x00000024),
            NO_SUCH_VALUE = (0x00000025),
            WRONG_TYPE = (0x00000026),
            INVALID_PRIORITY = (0x00000027),
            NOT_DISABLEABLE = (0x00000028),
            FREE_RESOURCES = (0x00000029),
            QUERY_VETOED = (0x0000002A),
            CANT_SHARE_IRQ = (0x0000002B),
            NO_DEPENDENT = (0x0000002C),
            SAME_RESOURCES = (0x0000002D),
            NO_SUCH_REGISTRY_KEY = (0x0000002E),
            INVALID_MACHINENAME = (0x0000002F), // NT ONLY
            REMOTE_COMM_FAILURE = (0x00000030), // NT ONLY
            MACHINE_UNAVAILABLE = (0x00000031), // NT ONLY
            NO_CM_SERVICES = (0x00000032), // NT ONLY
            ACCESS_DENIED = (0x00000033), // NT ONLY
            CALL_NOT_IMPLEMENTED = (0x00000034),
            INVALID_PROPERTY = (0x00000035),
            DEVICE_INTERFACE_ACTIVE = (0x00000036),
            NO_SUCH_DEVICE_INTERFACE = (0x00000037),
            INVALID_REFERENCE_STRING = (0x00000038),
            INVALID_CONFLICT_LIST = (0x00000039),
            INVALID_INDEX = (0x0000003A),
            INVALID_STRUCTURE_SIZE = (0x0000003B),
            NUM_CR_RESULTS = (0x0000003C)
        }

        #endregion

        #region DeviceInterfaceDataFlags enum

        public enum DeviceInterfaceDataFlags : uint
        {
            Active = 0x00000001,
            Default = 0x00000002,
            Removed = 0x00000004
        }

        #endregion

        #region DICFG enum

        [Flags]
        public enum DICFG
        {
            /// <summary>
            /// Return only the device that is associated with the system default device interface, if one is set, for the specified device interface classes. 
            ///  only valid with <see cref="DEVICEINTERFACE"/>.
            /// </summary>
            DEFAULT = 0x00000001,
            /// <summary>
            /// Return only devices that are currently present in a system. 
            /// </summary>
            PRESENT = 0x00000002,
            /// <summary>
            /// Return a list of installed devices for all device setup classes or all device interface classes. 
            /// </summary>
            ALLCLASSES = 0x00000004,
            /// <summary>
            /// Return only devices that are a part of the current hardware profile. 
            /// </summary>
            PROFILE = 0x00000008,
            /// <summary>
            /// Return devices that support device interfaces for the specified device interface classes. 
            /// </summary>
            DEVICEINTERFACE = 0x00000010,
        }

        #endregion

        #region DICUSTOMDEVPROP enum

        public enum DICUSTOMDEVPROP
        {
            NONE = 0,
            MERGE_MULTISZ = 0x00000001,
        }

        #endregion

        [Flags]
        public enum DevKeyType
        {
            DEV = 0x00000001,         // Open/Create/Delete device key
            DRV = 0x00000002,         // Open/Create/Delete driver key
            BOTH = 0x00000004,         // Delete both driver and Device key

        }
        #endregion

        private const string STRUCT_END_MARK = "STRUCT_END_MARK";

        public static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("f18a0e88-c30c-11d0-8815-00a0c906bed8");
        
        public static bool Is64Bit
        {
            get { return (IntPtr.Size == 8); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dnDevInst">Caller-supplied device instance handle that is bound to the local machine.</param>
        /// <param name="Buffer">Address of a buffer to receive a device instance ID string. The required buffer size can be obtained by calling CM_Get_Device_ID_Size, then incrementing the received value to allow room for the string's terminating NULL. </param>
        /// <param name="BufferLen">Caller-supplied length, in characters, of the buffer specified by Buffer. </param>
        /// <param name="ulFlags">Not used. set to 0.</param>
        /// <returns>If the operation succeeds, the function returns CR_SUCCESS. Otherwise, it returns one of the CR_-prefixed error codes defined in cfgmgr32.h.</returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern CR CM_Get_Device_ID(IntPtr dnDevInst, IntPtr Buffer, int BufferLen, int ulFlags);


        /// <summary>
        /// The CM_Get_Parent function obtains a device instance handle to the parent node of a specified device node, in the local machine's device tree.
        /// </summary>
        /// <param name="pdnDevInst">Caller-supplied pointer to the device instance handle to the parent node that this function retrieves. The retrieved handle is bound to the local machine.</param>
        /// <param name="dnDevInst">Caller-supplied device instance handle that is bound to the local machine. </param>
        /// <param name="ulFlags">Not used. set to 0.</param>
        /// <returns>If the operation succeeds, the function returns CR_SUCCESS. Otherwise, it returns one of the CR_-prefixed error codes defined in cfgmgr32.h.</returns>
        [DllImport("setupapi.dll")]
        public static extern CR CM_Get_Parent(out IntPtr pdnDevInst, IntPtr dnDevInst, int ulFlags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto /*, SetLastError = true*/)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        /// <summary>
        /// The SetupDiEnumDeviceInterfaces function enumerates the device interfaces that are contained in a device information set. 
        /// </summary>
        /// <param name="hDevInfo">A pointer to a device information set that contains the device interfaces for which to return information. This handle is typically returned by SetupDiGetClassDevs. </param>
        /// <param name="devInfo">A pointer to an SP_DEVINFO_DATA structure that specifies a device information element in DeviceInfoSet. This parameter is optional and can be NULL. If this parameter is specified, SetupDiEnumDeviceInterfaces constrains the enumeration to the interfaces that are supported by the specified device. If this parameter is NULL, repeated calls to SetupDiEnumDeviceInterfaces return information about the interfaces that are associated with all the device information elements in DeviceInfoSet. This pointer is typically returned by SetupDiEnumDeviceInfo. </param>
        /// <param name="interfaceClassGuid">A pointer to a GUID that specifies the device interface class for the requested interface. </param>
        /// <param name="memberIndex">A zero-based index into the list of interfaces in the device information set. The caller should call this function first with MemberIndex set to zero to obtain the first interface. Then, repeatedly increment MemberIndex and retrieve an interface until this function fails and GetLastError returns ERROR_NO_MORE_ITEMS.  If DeviceInfoData specifies a particular device, the MemberIndex is relative to only the interfaces exposed by that device.</param>
        /// <param name="deviceInterfaceData">A pointer to a caller-allocated buffer that contains, on successful return, a completed SP_DEVICE_INTERFACE_DATA structure that identifies an interface that meets the search parameters. The caller must set DeviceInterfaceData.cbSize to sizeof(SP_DEVICE_INTERFACE_DATA) before calling this function. </param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo,
                                                                 ref SP_DEVINFO_DATA devInfo,
                                                                 ref Guid interfaceClassGuid,
                                                                 int memberIndex,
                                                                 ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo,
                                                                 [MarshalAs(UnmanagedType.AsAny)] object devInfo,
                                                                 ref Guid interfaceClassGuid,
                                                                 int memberIndex,
                                                                 ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        /// <summary>
        /// The SetupDiGetClassDevs function returns a handle to a device information set that contains requested device information elements for a local machine. 
        /// </summary>
        /// <param name="ClassGuid">A pointer to the GUID for a device setup class or a device interface class. This pointer is optional and can be NULL. For more information about how to set ClassGuid, see the following Comments section. </param>
        /// <param name="Enumerator">A pointer to a NULL-terminated string that supplies the name of a Plug and Play (PnP) enumerator or a PnP device instance identifier. This pointer is optional and can be NULL. For more information about how to set the Enumerator value, see the following Comments section. </param>
        /// <param name="hwndParent">A handle of the top-level window to be used for a user interface that is associated with installing a device instance in the device information set. This handle is optional and can be NULL. </param>
        /// <param name="Flags">A variable of type DWORD that specifies control options that filter the device information elements that are added to the device information set. This parameter can be a bitwise OR of zero or more of the following flags.</param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Ansi, EntryPoint = "SetupDiGetClassDevsA")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid,
                                                        [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
                                                        IntPtr hwndParent,
                                                        DICFG Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Ansi, EntryPoint = "SetupDiGetClassDevsA")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, int Enumerator, IntPtr hwndParent, DICFG Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Ansi, EntryPoint = "SetupDiGetClassDevsA")]
        public static extern IntPtr SetupDiGetClassDevs(int ClassGuid, string Enumerator, IntPtr hwndParent, DICFG Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetCustomDeviceProperty(IntPtr DeviceInfoSet,
                                                                 ref SP_DEVINFO_DATA DeviceInfoData,
                                                                 string CustomPropertyName,
                                                                 DICUSTOMDEVPROP Flags,
                                                                 out RegistryValueKind PropertyRegDataType,
                                                                 Byte[] PropertyBuffer,
                                                                 int PropertyBufferSize,
                                                                 out int RequiredSize);

        /// <summary>
        /// The SetupDiGetDeviceInstanceId function retrieves the device instance ID that is associated with a device information element.
        /// </summary>
        /// <param name="DeviceInfoSet">A handle to the device information set that contains the device information element that represents the device for which to retrieve a device instance ID. </param>
        /// <param name="DeviceInfoData">A pointer to an SP_DEVINFO_DATA structure that specifies the device information element in DeviceInfoSet. </param>
        /// <param name="DeviceInstanceId">A pointer to the character buffer that will receive the NULL-terminated device instance ID for the specified device information element. For information about device instance IDs, see Device Identification Strings.</param>
        /// <param name="DeviceInstanceIdSize">The size, in characters, of the DeviceInstanceId buffer. </param>
        /// <param name="RequiredSize">A pointer to the variable that receives the number of characters required to store the device instance ID.</param>
        /// <returns>The function returns TRUE if it is successful. Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.</returns>
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Ansi, EntryPoint = "SetupDiGetDeviceInstanceIdA")]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet,
                                                             ref SP_DEVINFO_DATA DeviceInfoData,
                                                             StringBuilder DeviceInstanceId,
                                                             int DeviceInstanceIdSize,
                                                             out int RequiredSize);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo,
                                                                     ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                                     DEVICE_INTERFACE_DETAIL_HANDLE deviceInterfaceDetailData,
                                                                     int deviceInterfaceDetailDataSize,
                                                                     out int requiredSize,
                                                                     [MarshalAs(UnmanagedType.AsAny)] object deviceInfoData);
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo,
                                                                     ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                                     DEVICE_INTERFACE_DETAIL_HANDLE deviceInterfaceDetailData,
                                                                     int deviceInterfaceDetailDataSize,
                                                                     out int requiredSize,
                                                                     ref SP_DEVINFO_DATA deviceInfoData);
        
        
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfacePropertyKeys(IntPtr DeviceInfoSet,
                                                                        ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
                                                                        byte[] propKeyBuffer,
                                                                        int propKeyBufferElements,
                                                                        out int RequiredPropertyKeyCount,
                                                                        int Flags);

        /// <summary>
        /// The SetupDiGetDeviceRegistryProperty function retrieves the specified device property.
        /// This handle is typically returned by the SetupDiGetClassDevs or SetupDiGetClassDevsEx function.
        /// </summary>
        /// <param Name="DeviceInfoSet">Handle to the device information set that contains the interface and its underlying device.</param>
        /// <param Name="DeviceInfoData">Pointer to an SP_DEVINFO_DATA structure that defines the device instance.</param>
        /// <param Name="Property">Device property to be retrieved. SEE MSDN</param>
        /// <param Name="PropertyRegDataType">Pointer to a variable that receives the registry data Type. This parameter can be NULL.</param>
        /// <param Name="PropertyBuffer">Pointer to a buffer that receives the requested device property.</param>
        /// <param Name="PropertyBufferSize">Size of the buffer, in bytes.</param>
        /// <param Name="RequiredSize">Pointer to a variable that receives the required buffer size, in bytes. This parameter can be NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet,
                                                                   ref SP_DEVINFO_DATA DeviceInfoData,
                                                                   SPDRP Property,
                                                                   out RegistryValueKind PropertyRegDataType,
                                                                   byte[] PropertyBuffer,
                                                                   int PropertyBufferSize,
                                                                   out int RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern CR CM_Get_Device_ID(uint dnDevInst, StringBuilder Buffer, int BufferLen, int ulFlags);

        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int scope, int hwProfile, DevKeyType keyType, RegistryKeyPermissionCheck samDesired);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegEnumValue(IntPtr hKey, int index, StringBuilder lpValueName, ref int lpcValueName, IntPtr lpReserved, out RegistryValueKind lpType, byte[] data, ref int dataLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegEnumValue(IntPtr hKey, int index, StringBuilder lpValueName, ref int lpcValueName, IntPtr lpReserved, out RegistryValueKind lpType, StringBuilder data, ref int dataLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegCloseKey(IntPtr hKey);

        public static bool EnumClassDevs(string enumerator,
                                         DICFG flags,
                                         ClassEnumeratorDelegate classEnumeratorCallback,
                                         object classEnumeratorCallbackParam1)
        {
            SP_DEVINFO_DATA dev_info_data = SP_DEVINFO_DATA.Empty;

            int dev_index = 0;

            IntPtr dev_info = SetupDiGetClassDevs(0, enumerator, IntPtr.Zero, flags);

            if (dev_info == IntPtr.Zero || dev_info.ToInt64() == -1) return false;
            bool bSuccess = false;
            while (SetupDiEnumDeviceInfo(dev_info, dev_index, ref dev_info_data))
            {
                if (classEnumeratorCallback(dev_info, dev_index, ref dev_info_data, classEnumeratorCallbackParam1))
                {
                    bSuccess = true;
                    break;
                }

                dev_index++;
            }

            SetupDiDestroyDeviceInfoList(dev_info);

            return bSuccess;
        }

        public static void getSPDRPProperties(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, Dictionary<string, object> deviceProperties)
        {
            byte[] propBuffer = new byte[1024];
            Dictionary<string, int> allProps = Helper.GetEnumData(typeof(SPDRP));
            foreach (KeyValuePair<string, int> prop in allProps)
            {
                object oValue = String.Empty;
                int iReturnBytes;
                RegistryValueKind regPropType;
                bool bSuccess = SetupDiGetDeviceRegistryProperty(deviceInfoSet,
                                                                 ref deviceInfoData,
                                                                 (SPDRP)prop.Value,
                                                                 out regPropType,
                                                                 propBuffer,
                                                                 propBuffer.Length,
                                                                 out iReturnBytes);
                if (bSuccess)
                {
                    switch ((SPDRP)prop.Value)
                    {
                        case SPDRP.PhysicalDeviceObjectName:
                        case SPDRP.LocationInformation:
                        case SPDRP.Class:
                        case SPDRP.Mfg:
                        case SPDRP.DeviceDesc:
                        case SPDRP.Driver:
                        case SPDRP.EnumeratorName:
                        case SPDRP.FriendlyName:
                        case SPDRP.ClassGuid:
                            oValue = UsbRegistry.GetAsString(propBuffer, iReturnBytes);
                            break;
                        case SPDRP.HardwareId:
                        case SPDRP.CompatibleIds:
                        case SPDRP.LocationPaths:
                            oValue = UsbRegistry.GetAsStringArray(propBuffer, iReturnBytes);
                            break;
                        case SPDRP.BusNumber:
                        case SPDRP.InstallState:
                        case SPDRP.LegacyBusType:
                        case SPDRP.RemovalPolicy:
                        case SPDRP.UiNumber:
                        case SPDRP.Address:
                            oValue = UsbRegistry.GetAsStringInt32(propBuffer, iReturnBytes);
                            break;
                        case SPDRP.BusTypeGuid:
                            oValue = UsbRegistry.GetAsGuid(propBuffer, iReturnBytes);
                            break;
                    }
                }
                else
                    oValue = String.Empty;

                deviceProperties.Add(prop.Key, oValue);
            }
        }

        public static bool SetupDiGetDeviceInterfaceDetailLength(IntPtr hDevInfo,
                                                                 ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                                 out int requiredLength)
        {
            DEVICE_INTERFACE_DETAIL_HANDLE tmp = new DEVICE_INTERFACE_DETAIL_HANDLE();
            return SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, tmp, 0, out requiredLength, null);
        }

        public static bool SetupDiGetDeviceRegistryProperty(out byte[] regBytes,
                                                            IntPtr DeviceInfoSet,
                                                            ref SP_DEVINFO_DATA DeviceInfoData,
                                                            SPDRP Property)
        {
            regBytes = null;
            byte[] tmp = new byte[1024];
            int iReqSize;
            RegistryValueKind regValueType;
            if (!SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out regValueType, tmp, tmp.Length, out iReqSize))
            {
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "SetupDiGetDeviceRegistryProperty", typeof(SetupApi));
                return false;
            }
            regBytes = new byte[iReqSize];
            Array.Copy(tmp, regBytes, regBytes.Length);
            return true;
        }

        public static bool SetupDiGetDeviceRegistryProperty(out string regSZ, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property)
        {
            regSZ = null;
            byte[] tmp;
            if (SetupDiGetDeviceRegistryProperty(out tmp, DeviceInfoSet, ref DeviceInfoData, Property))
            {
                regSZ = Encoding.Unicode.GetString(tmp).TrimEnd(new char[] {'\0'});
                return true;
            }
            return false;
        }

        public static bool SetupDiGetDeviceRegistryProperty(out string[] regMultiSZ,
                                                            IntPtr DeviceInfoSet,
                                                            ref SP_DEVINFO_DATA DeviceInfoData,
                                                            SPDRP Property)
        {
            regMultiSZ = null;
            string tmp;
            if (SetupDiGetDeviceRegistryProperty(out tmp, DeviceInfoSet, ref DeviceInfoData, Property))
            {
                regMultiSZ = tmp.Split(new char[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
                return true;
            }
            return false;
        }

        private static bool cbHasDeviceInterfaceGUID(IntPtr DeviceInfoSet,
                                                     int deviceIndex,
                                                     ref SP_DEVINFO_DATA DeviceInfoData,
                                                     object devInterfaceGuid)
        {
            RegistryValueKind propertyType;
            byte[] propBuffer = new byte[256];
            int requiredSize;
            bool bSuccess = SetupDiGetCustomDeviceProperty(DeviceInfoSet,
                                                           ref DeviceInfoData,
                                                           "DeviceInterfaceGuids",
                                                           DICUSTOMDEVPROP.NONE,
                                                           out propertyType,
                                                           propBuffer,
                                                           propBuffer.Length,
                                                           out requiredSize);
            if (bSuccess)
            {
                Guid devGuid = (Guid) devInterfaceGuid;
                string[] stemp = Encoding.Unicode.GetString(propBuffer, 0, requiredSize).Split(new char[] {'\0'},
                                                                                               StringSplitOptions.RemoveEmptyEntries);
                Guid findGuid = new Guid(stemp[0]);
                return (devGuid == findGuid);
            }
            return false;
        }

        #region Nested Types

        #region Nested type: DEVICE_INTERFACE_DETAIL_HANDLE

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE_INTERFACE_DETAIL_HANDLE
        {
            private IntPtr mPtr;

            internal DEVICE_INTERFACE_DETAIL_HANDLE(IntPtr ptrInit) { mPtr = ptrInit; }
        }

        #endregion

        #region Nested type: DeviceInterfaceDetailHelper

        public class DeviceInterfaceDetailHelper
        {
            public static readonly int SIZE = Is64Bit ? 8 : 6;
            private IntPtr mpDevicePath;
            private IntPtr mpStructure;

            public DeviceInterfaceDetailHelper(int maximumLength)
            {
                mpStructure = Marshal.AllocHGlobal(maximumLength);
                mpDevicePath = new IntPtr(mpStructure.ToInt64() + Marshal.SizeOf(typeof (int)));
            }

            public DEVICE_INTERFACE_DETAIL_HANDLE Handle
            {
                get
                {
                    Marshal.WriteInt32(mpStructure, SIZE);
                    return new DEVICE_INTERFACE_DETAIL_HANDLE(mpStructure);
                }
            }

            public string DevicePath
            {
                get { return Marshal.PtrToStringAuto(mpDevicePath); }
            }


            public void Free()
            {
                if (mpStructure != IntPtr.Zero)
                    Marshal.FreeHGlobal(mpStructure);

                mpDevicePath = IntPtr.Zero;
                mpStructure = IntPtr.Zero;
            }


            ~DeviceInterfaceDetailHelper() { Free(); }
        }

        #endregion

        #region Nested type: MaxStructSizes

        private class MaxStructSizes
        {
            public const int SP_DEVINFO_DATA = 40;
        }

        #endregion

        #region Nested type: SP_DEVICE_INTERFACE_DATA

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public static readonly SP_DEVICE_INTERFACE_DATA Empty = new SP_DEVICE_INTERFACE_DATA(Marshal.SizeOf(typeof (SP_DEVICE_INTERFACE_DATA)));

            public UInt32 cbSize;
            public Guid interfaceClassGuid;
            public UInt32 flags;
            private UIntPtr reserved;

            private SP_DEVICE_INTERFACE_DATA(int size)
            {
                cbSize = (uint) size;
                reserved = UIntPtr.Zero;
                flags = 0;
                interfaceClassGuid = Guid.Empty;
            }
        }

        #endregion

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct SP_DEVICE_INTERFACE_DATA
        //{
        //    public static readonly SP_DEVICE_INTERFACE_DATA Empty = new SP_DEVICE_INTERFACE_DATA(GetSetupApiSize(typeof (SP_DEVICE_INTERFACE_DATA)));

        //    public readonly uint cbSize;
        //    public Guid interfaceClassGuid;
        //    public DeviceInterfaceDataFlags flags;
        //    private IntPtr reserved;

        //    private SP_DEVICE_INTERFACE_DATA(int size)
        //    {
        //        reserved = new IntPtr();
        //        flags = 0;
        //        interfaceClassGuid = Guid.Empty;
        //        cbSize = size;
        //    }
        //}

        #region Nested type: SP_DEVINFO_DATA

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public static readonly SP_DEVINFO_DATA Empty = new SP_DEVINFO_DATA(Marshal.SizeOf(typeof (SP_DEVINFO_DATA)));

            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public IntPtr Reserved;

            private SP_DEVINFO_DATA(int size)
            {
                cbSize = (uint) size;
                ClassGuid = Guid.Empty;
                DevInst = 0;
                Reserved = IntPtr.Zero;
            }
        }

        #endregion

        //[StructLayout(LayoutKind.Sequential, Pack = IntPtr.Size==8?8:1, Size = MaxStructSizes.SP_DEVINFO_DATA)]
        //public struct SP_DEVINFO_DATA
        //{
        //    public static readonly SP_DEVINFO_DATA Empty = new SP_DEVINFO_DATA(Marshal.SizeOf(typeof(SP_DEVINFO_DATA)));

        //    public readonly int cbSize;
        //    public Guid ClassGuid;
        //    public IntPtr DevInst;
        //    public IntPtr Reserved;
        //    private SP_DEVINFO_DATA(int size)
        //    {
        //        cbSize = size;
        //        ClassGuid = Guid.Empty;
        //        DevInst = IntPtr.Zero;
        //        Reserved = IntPtr.Zero;

        //        STRUCT_END_MARK = 0;
        //    }
        //}

        #endregion
    }
}