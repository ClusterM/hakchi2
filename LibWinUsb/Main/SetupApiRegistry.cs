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
using System.Text;
using LibUsbDotNet.Internal;
using LibUsbDotNet.LudnMonoLibUsb;
using Microsoft.Win32;
using MonoLibUsb;
using System.Runtime.InteropServices;

namespace LibUsbDotNet.Main
{
    internal static class SetupApiRegistry
    {
        private static readonly Object mLockSetupApiRegistry = new object();
        private static readonly MasterList mMasterSetupApiDeviceList = new MasterList();
        private static DateTime mLastRefreshTime = DateTime.MinValue;
        public const string DEVICE_ID_KEY = "DeviceInstanceID";

        public static bool NeedsRefresh
        {
            get
            {
                lock (mLockSetupApiRegistry)
                {
                    if ((DateTime.Now - mLastRefreshTime).TotalMilliseconds >= 1000)
                        return true;
                    return false;
                }
            }
        }

        private class MasterItem : Dictionary<string, object>
        {
            public Dictionary<Guid, List<string>> DevicePaths = new Dictionary<Guid, List<string>>();
        }

        private class MasterList : List<MasterItem>
        {
        }

        public static bool FillDeviceProperties(UsbRegistry usbRegistry, UsbDevice usbDevice)
        {

            lock (mLockSetupApiRegistry)
            {
                if (NeedsRefresh) BuildMasterList();
                if (usbDevice is MonoUsbDevice && !Helper.IsLinux)
                    return FillWindowsMonoUsbDeviceRegistry(usbRegistry, (MonoUsbDevice) usbDevice);
                
                string fakeHwId = LegacyUsbRegistry.GetRegistryHardwareID((ushort) usbDevice.Info.Descriptor.VendorID,
                                                                          (ushort) usbDevice.Info.Descriptor.ProductID,
                                                                          (ushort) usbDevice.Info.Descriptor.BcdDevice);
                bool bFound = false;
                string hwIdToFind = fakeHwId.ToLower();
                foreach (MasterItem masterItem in mMasterSetupApiDeviceList)
                {
                    string[] hwIds = masterItem[SPDRP.HardwareId.ToString()] as string[];
                    if (ReferenceEquals(hwIds, null)) continue;
                    foreach (string hwID in hwIds)
                    {
                        if (hwID.ToLower().Contains(hwIdToFind))
                        {
                            usbRegistry.mDeviceProperties = masterItem;
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound) break;
                }
                return bFound;
            }
        }

        private static bool FillWindowsMonoUsbDeviceRegistry(UsbRegistry usbRegistry, MonoUsbDevice usbDevice) 
        {
            MonoLibUsb.MonoUsbApi.internal_windows_device_priv priv = MonoLibUsb.MonoUsbApi.GetWindowsPriv(usbDevice.Profile.ProfileHandle);
            string path;
            for (int i = 0; i < 32; i++)
            {
                if (priv.usb_interfaces[i].path == IntPtr.Zero) break;
                path = Marshal.PtrToStringAnsi(priv.usb_interfaces[i].path);
                //Debug.Print("Intf:{0} Path:{1}",i,path);
            }
            path = Marshal.PtrToStringAnsi(priv.path);

            bool bFound = false;

            //System.Diagnostics.Debug.WriteLine(sb.ToString());
            path = path.ToString().ToLower().Replace("#", "\\");
            foreach (MasterItem masterItem in mMasterSetupApiDeviceList)
            {
                if (path.Contains(masterItem[DEVICE_ID_KEY].ToString().ToLower()))
                {
                    usbRegistry.mDeviceProperties = masterItem;
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }

        public static void BuildMasterList()
        {
            lock (mLockSetupApiRegistry)
            {
                mMasterSetupApiDeviceList.Clear();
                SetupApi.EnumClassDevs(null, SetupApi.DICFG.PRESENT | SetupApi.DICFG.ALLCLASSES, BuildMasterCallback, mMasterSetupApiDeviceList);
                mLastRefreshTime = DateTime.Now;
            }
        }

        private static bool BuildMasterCallback(IntPtr deviceInfoSet, int deviceindex, ref SetupApi.SP_DEVINFO_DATA deviceInfoData, object userData)
        {
            MasterList deviceList = userData as MasterList;
            MasterItem deviceItem = new MasterItem();
            StringBuilder sb=new StringBuilder(256);

            if (SetupApi.CM_Get_Device_ID(deviceInfoData.DevInst, sb, sb.Capacity, 0)!=SetupApi.CR.SUCCESS)
                return false;

            deviceItem.Add(DEVICE_ID_KEY,sb.ToString());
            deviceList.Add(deviceItem);


            RegistryValueKind propertyType;
            byte[] propBuffer = new byte[256];
            int requiredSize;
            bool bSuccess = SetupApi.SetupDiGetCustomDeviceProperty(deviceInfoSet,
                                                                    ref deviceInfoData,
                                                                    UsbRegistry.DEVICE_INTERFACE_GUIDS,
                                                                    SetupApi.DICUSTOMDEVPROP.NONE,
                                                                    out propertyType,
                                                                    propBuffer,
                                                                    propBuffer.Length,
                                                                    out requiredSize);
            if (bSuccess)
            {
                string[] devInterfaceGuids = UsbRegistry.GetAsStringArray(propBuffer, requiredSize);

                deviceItem.Add(UsbRegistry.DEVICE_INTERFACE_GUIDS, devInterfaceGuids);
                foreach (string s in devInterfaceGuids)
                {
                    Guid g = new Guid(s);
                    List<string> devicePathList;
                    if (WinUsb.WinUsbRegistry.GetDevicePathList(g, out devicePathList))
                    {
                        deviceItem.DevicePaths.Add(g, devicePathList);
                    }
                }
            }
            else
            {
                bSuccess = SetupApi.SetupDiGetCustomDeviceProperty(deviceInfoSet,
                                                                   ref deviceInfoData,
                                                                   UsbRegistry.LIBUSB_INTERFACE_GUIDS,
                                                                   SetupApi.DICUSTOMDEVPROP.NONE,
                                                                   out propertyType,
                                                                   propBuffer,
                                                                   propBuffer.Length,
                                                                   out requiredSize);
                if (bSuccess)
                {
                    string[] devInterfaceGuids = UsbRegistry.GetAsStringArray(propBuffer, requiredSize);

                    deviceItem.Add(UsbRegistry.LIBUSB_INTERFACE_GUIDS, devInterfaceGuids);
                }
            }

            bSuccess =
                SetupApi.SetupDiGetCustomDeviceProperty(deviceInfoSet,
                                                        ref deviceInfoData,
                                                        UsbRegistry.SYMBOLIC_NAME_KEY,
                                                        SetupApi.DICUSTOMDEVPROP.NONE,
                                                        out propertyType,
                                                        propBuffer,
                                                        propBuffer.Length,
                                                        out requiredSize);
            if (bSuccess)
            {
                string symbolicName = UsbRegistry.GetAsString(propBuffer, requiredSize);
                deviceItem.Add(UsbRegistry.SYMBOLIC_NAME_KEY, symbolicName);
            }
            SetupApi.getSPDRPProperties(deviceInfoSet, ref deviceInfoData, deviceItem);

            return false;
        }
    }
}