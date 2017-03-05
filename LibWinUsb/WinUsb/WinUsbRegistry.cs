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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet.Internal;
using LibUsbDotNet.Internal.UsbRegex;
using LibUsbDotNet.Main;
using Microsoft.Win32;

namespace LibUsbDotNet.WinUsb
{
    /// <summary> WinUsb specific members for device registry settings.
    /// </summary> 
    public class WinUsbRegistry : UsbRegistry
    {
        private bool mIsDeviceIDParsed;

        private string mDeviceID;

        // Parsed out of the device ID
        private byte mInterfaceID;
        private ushort mVid;
        private ushort mPid;

        /// <summary>
        /// Gets a list of WinUSB device paths for the specified interface guid.
        /// </summary>
        /// <param name="deviceInterfaceGuid">The DeviceInterfaceGUID to search for.</param>
        /// <param name="devicePathList">A list of device paths associated with the <paramref name="deviceInterfaceGuid"/>.</param>
        /// <returns>True of one or more device paths was found.</returns>
        /// <remarks>
        /// Each device path string in the <paramref name="devicePathList"/> represents a seperate WinUSB device (interface).
        /// </remarks>
        /// <seealso cref="GetWinUsbRegistryList"/>
        public static bool GetDevicePathList(Guid deviceInterfaceGuid, out List<String> devicePathList)
        {
            devicePathList = new List<string>();
            int devicePathIndex = 0;
            SetupApi.SP_DEVICE_INTERFACE_DATA interfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Empty;
            SetupApi.DeviceInterfaceDetailHelper detailHelper;

            IntPtr deviceInfo = SetupApi.SetupDiGetClassDevs(ref deviceInterfaceGuid, null, IntPtr.Zero, SetupApi.DICFG.PRESENT | SetupApi.DICFG.DEVICEINTERFACE);
            if (deviceInfo != IntPtr.Zero)
            {
                while ((SetupApi.SetupDiEnumDeviceInterfaces(deviceInfo, null, ref deviceInterfaceGuid, devicePathIndex, ref interfaceData)))
                {
                    int length = 1024;
                    detailHelper = new SetupApi.DeviceInterfaceDetailHelper(length);
                    bool bResult = SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref interfaceData, detailHelper.Handle, length, out length, null);
                    if (bResult) devicePathList.Add(detailHelper.DevicePath);

                    devicePathIndex++;
                }
            }
            if (devicePathIndex == 0)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetDevicePathList", typeof(SetupApi));

            if (deviceInfo != IntPtr.Zero)
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);

            return (devicePathIndex > 0);
        }

        /// <summary>
        /// Gets a list of <see cref="WinUsbRegistry"/> classes for the specified interface guid.
        /// </summary>
        /// <param name="deviceInterfaceGuid">The DeviceInterfaceGUID to search for.</param>
        /// <param name="deviceRegistryList">A list of device paths associated with the <paramref name="deviceInterfaceGuid"/>.</param>
        /// <returns>True of one or more device paths was found.</returns>
        /// <remarks>
        /// Each <see cref="WinUsbRegistry"/> in the <paramref name="deviceRegistryList"/> represents a seperate WinUSB device (interface).
        /// </remarks>
        public static bool GetWinUsbRegistryList(Guid deviceInterfaceGuid, out List<WinUsbRegistry> deviceRegistryList)
        {
            deviceRegistryList = new List<WinUsbRegistry>();

            int devicePathIndex = 0;
            SetupApi.SP_DEVICE_INTERFACE_DATA interfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Empty;
            SetupApi.DeviceInterfaceDetailHelper detailHelper;

            SetupApi.SP_DEVINFO_DATA devInfoData = SetupApi.SP_DEVINFO_DATA.Empty;

            // [1]
            IntPtr deviceInfo = SetupApi.SetupDiGetClassDevs(ref deviceInterfaceGuid, null, IntPtr.Zero, SetupApi.DICFG.PRESENT | SetupApi.DICFG.DEVICEINTERFACE);
            if (deviceInfo != IntPtr.Zero)
            {
                while ((SetupApi.SetupDiEnumDeviceInterfaces(deviceInfo, null, ref deviceInterfaceGuid, devicePathIndex, ref interfaceData)))
                {
                    int length = 1024;
                    detailHelper = new SetupApi.DeviceInterfaceDetailHelper(length);
                    bool bResult = SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref interfaceData, detailHelper.Handle, length, out length, ref devInfoData);
                    if (bResult)
                    {
                        WinUsbRegistry regInfo = new WinUsbRegistry();

                        SetupApi.getSPDRPProperties(deviceInfo, ref devInfoData, regInfo.mDeviceProperties);

                        // Use the actual winusb device path for SYMBOLIC_NAME_KEY. This will be used to open the device.
                        regInfo.mDeviceProperties.Add(SYMBOLIC_NAME_KEY, detailHelper.DevicePath);

                        //Debug.WriteLine(detailHelper.DevicePath);

                        regInfo.mDeviceInterfaceGuids = new Guid[] { deviceInterfaceGuid };

                        StringBuilder sbDeviceID=new StringBuilder(1024);
                        if (SetupApi.CM_Get_Device_ID(devInfoData.DevInst,sbDeviceID,sbDeviceID.Capacity,0)==SetupApi.CR.SUCCESS)
                        {
                            regInfo.mDeviceProperties[DEVICE_ID_KEY] = sbDeviceID.ToString();
                        }
                        deviceRegistryList.Add(regInfo);
                    }

                    devicePathIndex++;
                }
            }
            if (devicePathIndex == 0)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetDevicePathList", typeof(SetupApi));

            if (deviceInfo != IntPtr.Zero)
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);

            return (devicePathIndex > 0);
        }

        internal WinUsbRegistry() { }

        /// <summary>
        /// Gets a list of available LibUsb devices.
        /// </summary>
        public static List<WinUsbRegistry> DeviceList
        {
            get
            {
                List<WinUsbRegistry> deviceList = new List<WinUsbRegistry>();
                SetupApi.EnumClassDevs(null, SetupApi.DICFG.ALLCLASSES | SetupApi.DICFG.PRESENT, WinUsbRegistryCallBack, deviceList);
                return deviceList;
            }
        }

        /// <summary>
        /// Gets a collection of DeviceInterfaceGuids that are associated with this WinUSB device.
        /// </summary>
        public override Guid[] DeviceInterfaceGuids
        {
            get
            {
                return mDeviceInterfaceGuids;
            }
        }

        /// <summary>
        /// Check this value to determine if the usb device is still connected to the bus and ready to open.
        /// </summary>
        /// <remarks>
        /// Uses the symbolic name as a unique id to determine if this device instance is still attached.
        /// </remarks>
        /// <exception cref="UsbException">An exception is thrown if the <see cref="UsbRegistry.SymbolicName"/> property is null or empty.</exception>
        public override bool IsAlive
        {
            get
            {
                if (String.IsNullOrEmpty(SymbolicName)) throw new UsbException(this, "A symbolic name is required for this property.");

                List<WinUsbRegistry> deviceList = DeviceList;
                foreach (WinUsbRegistry registry in deviceList)
                {
                    if (String.IsNullOrEmpty(registry.SymbolicName)) continue;

                    if (registry.SymbolicName == SymbolicName)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <returns>Return a new instance of the <see cref="UsbDevice"/> class.
        /// If the device fails to open a null refrence is return. For extended error
        /// information use the <see cref="UsbDevice.UsbErrorEvent"/>.
        ///  </returns>
        public override UsbDevice Device
        {
            get
            {
                WinUsbDevice winUsbDevice;
                Open(out winUsbDevice);
                return winUsbDevice;
            }
        }


        private void parseDeviceID()
        {
            if (mIsDeviceIDParsed) return;
            
            mIsDeviceIDParsed = true;

            byte bTemp;
            ushort uTemp;

            MatchCollection matches = RegHardwareID.GlobalInstance.Matches(DeviceID);
            foreach (Match match in matches)
            {
                foreach (NamedGroup namedGroup in RegHardwareID.NAMED_GROUPS)
                {
                    Group g = match.Groups[namedGroup.GroupNumber];
                   if (g.Success)
                   {
                       switch ((RegHardwareID.ENamedGroups)namedGroup.GroupNumber)
                       {
                           case RegHardwareID.ENamedGroups.Vid:
                               if (ushort.TryParse(g.Value, NumberStyles.HexNumber, null, out uTemp))
                               {
                                   mVid = uTemp;
                                   break;
                               }
                               break;
                           case RegHardwareID.ENamedGroups.Pid:
                               if (ushort.TryParse(g.Value, NumberStyles.HexNumber, null, out uTemp))
                               {
                                   mPid = uTemp;
                                   break;
                               } 
                               break;
                           case RegHardwareID.ENamedGroups.Rev:
                               break;
                           case RegHardwareID.ENamedGroups.MI:
                               if (Byte.TryParse(g.Value, NumberStyles.HexNumber, null, out bTemp))
                               {
                                   mInterfaceID = bTemp;
                                   break;
                               }
                               break;
                           default:
                               throw new ArgumentOutOfRangeException();
                       }
                   }
                }

            }
        }

        /// <summary>
        /// Gets the device instance id.
        /// </summary>
        /// <remarks>
        /// For more information on device instance ids, see the <a href="http://msdn.microsoft.com/en-us/library/ff538405%28v=VS.85%29.aspx">CM_Get_Device_ID Function</a> at MSDN.
        /// </remarks>
        public string DeviceID
        {
            get
            {
                if (ReferenceEquals(mDeviceID,null))
                {
                    object oDeviceID;
                    if (mDeviceProperties.TryGetValue(DEVICE_ID_KEY, out oDeviceID))
                    {
                        mDeviceID = oDeviceID.ToString();
                    }
                    else
                    {
                        mDeviceID = string.Empty;
                    }
                }
                return mDeviceID;
            }
        }

        /// <summary>
        /// VendorID
        /// </summary>
        /// <remarks>This value is parsed out of the <see cref="DeviceID"/> field.</remarks>
        public override int Vid
        {
            get
            {
                parseDeviceID();
                return mVid;
            }
        }

        /// <summary>
        /// ProductID
        /// </summary>
        /// <remarks>This value is parsed out of the <see cref="DeviceID"/> field.</remarks>
        public override int Pid
        {
            get
            {
                parseDeviceID();
                return mPid;
            }
        }


        ///<summary>
        /// Gets the interface ID this WinUSB device (interface) is associated with.
        ///</summary>
        /// <remarks>This value is parsed out of the <see cref="DeviceID"/> field.</remarks>
        public byte InterfaceID
        {
            get
            {
                parseDeviceID();
                return (byte) mInterfaceID;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public override bool Open(out UsbDevice usbDevice)
        {
            usbDevice = null;
            WinUsbDevice winUsbDevice;
            bool bSuccess = Open(out winUsbDevice);
            if (bSuccess)
                usbDevice = winUsbDevice;
            return bSuccess;
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">Returns an opened WinUsb device on success, null on failure.</param>
        /// <returns>True on success.</returns>
        public bool Open(out WinUsbDevice usbDevice)
        {
            usbDevice = null;

            if (String.IsNullOrEmpty(SymbolicName)) return false;
            if (WinUsbDevice.Open(SymbolicName, out usbDevice))
            {
                usbDevice.mUsbRegistry = this;
                return true;
            }
            return false;
        }

        /*
        private static bool WinUsbRegistryCallBack(IntPtr deviceInfoSet,
                                                   int deviceIndex,
                                                   ref SetupApi.SP_DEVINFO_DATA deviceInfoData,
                                                   object classEnumeratorCallbackParam1)
        {

            List<WinUsbRegistry> deviceList = (List<WinUsbRegistry>) classEnumeratorCallbackParam1;

            RegistryValueKind propertyType;
            byte[] propBuffer = new byte[256];
            int requiredSize;
            bool isNew = true;
            bool bSuccess;

            bSuccess = SetupApi.SetupDiGetCustomDeviceProperty(deviceInfoSet,
                                                                    ref deviceInfoData,
                                                                    DEVICE_INTERFACE_GUIDS,
                                                                    SetupApi.DICUSTOMDEVPROP.NONE,
                                                                    out propertyType,
                                                                    propBuffer,
                                                                    propBuffer.Length,
                                                                    out requiredSize);
            if (bSuccess)
            {
                string[] devInterfaceGuids = GetAsStringArray(propBuffer, requiredSize);

                foreach (String devInterfaceGuid in devInterfaceGuids)
                {
                    Guid g = new Guid(devInterfaceGuid);
                    List<string> devicePaths;
                    if (SetupApi.GetDevicePathList(g, out devicePaths))
                    {
                        foreach (string devicePath in devicePaths)
                        {
                            WinUsbRegistry regInfo = new WinUsbRegistry();

                            SetupApi.getSPDRPProperties(deviceInfoSet, ref deviceInfoData, regInfo.mDeviceProperties);

                            // Use the actual winusb device path for SYMBOLIC_NAME_KEY. This will be used to open the device.
                            regInfo.mDeviceProperties.Add(SYMBOLIC_NAME_KEY, devicePath);

                            regInfo.mDeviceInterfaceGuids = new Guid[] { g };

                            // Don't add duplicate devices (with the same device path)
                            WinUsbRegistry foundRegistry=null;
                            foreach (WinUsbRegistry usbRegistry in deviceList)
                            {
                                if (usbRegistry.SymbolicName == regInfo.SymbolicName)
                                {
                                    foundRegistry = usbRegistry;
                                    break;
                                }
                            }
                            if (foundRegistry == null)
                                deviceList.Add(regInfo);
                            else
                            {
                                if (isNew)
                                {
                                    deviceList.Remove(foundRegistry);
                                    deviceList.Add(regInfo);
                                }
                                else
                                {

                                    // If the device path already exists, add this compatible guid 
                                    // to the foundRegstry guid list.
                                    List<Guid> newGuidList = new List<Guid>(foundRegistry.mDeviceInterfaceGuids);
                                    if (!newGuidList.Contains(g))
                                    {
                                        newGuidList.Add(g);
                                        foundRegistry.mDeviceInterfaceGuids = newGuidList.ToArray();
                                    }
                                }
                            }
                            isNew = false;
                        }
                    }
                }
            }

            return false;
        }
        */
        private static bool WinUsbRegistryCallBack(IntPtr deviceInfoSet,
                                           int deviceIndex,
                                           ref SetupApi.SP_DEVINFO_DATA deviceInfoData,
                                           object classEnumeratorCallbackParam1)
        {

            List<WinUsbRegistry> deviceList = (List<WinUsbRegistry>)classEnumeratorCallbackParam1;

            RegistryValueKind propertyType;
            byte[] propBuffer = new byte[256];
            int requiredSize;
            bool bSuccess;

            bSuccess = SetupApi.SetupDiGetCustomDeviceProperty(deviceInfoSet,
                                                                    ref deviceInfoData,
                                                                    DEVICE_INTERFACE_GUIDS,
                                                                    SetupApi.DICUSTOMDEVPROP.NONE,
                                                                    out propertyType,
                                                                    propBuffer,
                                                                    propBuffer.Length,
                                                                    out requiredSize);
            if (bSuccess)
            {
                string[] devInterfaceGuids = GetAsStringArray(propBuffer, requiredSize);

                foreach (String devInterfaceGuid in devInterfaceGuids)
                {
                    Guid g = new Guid(devInterfaceGuid);
                    List<WinUsbRegistry> tempList;
                    if (GetWinUsbRegistryList(g, out tempList))
                    {
                        foreach (WinUsbRegistry regInfo in tempList)
                        {
                            // Don't add duplicate devices (with the same device path)
                            WinUsbRegistry foundRegistry = null;
                            foreach (WinUsbRegistry usbRegistry in deviceList)
                            {
                                if (usbRegistry.SymbolicName == regInfo.SymbolicName)
                                {
                                    foundRegistry = usbRegistry;
                                    break;
                                }
                            }
                            if (foundRegistry == null)
                                deviceList.Add(regInfo);
                            else
                            {
                                // If the device path already exists, add this compatible guid 
                                // to the foundRegstry guid list.
                                List<Guid> newGuidList = new List<Guid>(foundRegistry.mDeviceInterfaceGuids);
                                if (!newGuidList.Contains(g))
                                {
                                    newGuidList.Add(g);
                                    foundRegistry.mDeviceInterfaceGuids = newGuidList.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

    }
}