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
using LibUsbDotNet.Internal.LibUsb;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;
using Debug=System.Diagnostics.Debug;

namespace LibUsbDotNet.LibUsb
{
    /// <summary> LibUsb specific members for device registry settings.
    /// </summary> 
    public class LibUsbRegistry : UsbRegistry
    {
        private readonly string mDeviceFilename;
        private readonly int mDeviceIndex;

        private LibUsbRegistry(SafeFileHandle usbHandle, string deviceFileName, int deviceIndex)
        {
            mDeviceFilename = deviceFileName;
            mDeviceIndex = deviceIndex;
            GetPropertiesSPDRP(usbHandle);
            string symbolicName;

            if (GetCustomDeviceKeyValue(usbHandle, SYMBOLIC_NAME_KEY, out symbolicName, 512) == ErrorCode.None)
            {
                mDeviceProperties.Add(SYMBOLIC_NAME_KEY, symbolicName);
            }

            // If the SymbolicName key does not exists, use the first HardwareID string.
            if (!mDeviceProperties.ContainsKey(SYMBOLIC_NAME_KEY) || String.IsNullOrEmpty(symbolicName))
            {
                string[] hwIDs = mDeviceProperties[SPDRP.HardwareId.ToString()] as string[];

                if ((hwIDs == null) || hwIDs.Length==0)
                {
                    LibUsbDevice usbDevice = new LibUsbDevice(UsbDevice.LibUsbApi, usbHandle, deviceFileName);
                    LegacyUsbRegistry.GetPropertiesSPDRP(usbDevice, mDeviceProperties);
                    return;
                }

                if (hwIDs.Length > 0)
                {
                    mDeviceProperties.Add(SYMBOLIC_NAME_KEY, hwIDs[0]);
                }
            }

            string deviceInterfaceGuids;
            if (GetCustomDeviceKeyValue(usbHandle, LIBUSB_INTERFACE_GUIDS, out deviceInterfaceGuids, 512) == ErrorCode.None)
            {
                string[] deviceInterfaceGuidsArray = deviceInterfaceGuids.Split(new char[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);

                mDeviceProperties.Add(LIBUSB_INTERFACE_GUIDS, deviceInterfaceGuidsArray);
            }
        }

        /// <summary>
        /// Gets the 0 based index of this libusb device
        /// </summary>
        public int DeviceIndex
        {
            get { return mDeviceIndex; }
        }

        /// <summary>
        /// Gets a list of available LibUsb devices.
        /// </summary>
        public static List<LibUsbRegistry> DeviceList
        {
            get
            {
                List<LibUsbRegistry> deviceList = new List<LibUsbRegistry>();
                for (int i = 1; i < UsbConstants.MAX_DEVICES; i++)
                {
                    string deviceFileName = LibUsbDriverIO.GetDeviceNameString(i);

                    SafeFileHandle deviceHandle = LibUsbDriverIO.OpenDevice(deviceFileName);
                    if (deviceHandle != null && !deviceHandle.IsInvalid && !deviceHandle.IsClosed)
                    {
                        try
                        {
                            LibUsbRegistry regInfo = new LibUsbRegistry(deviceHandle, deviceFileName, i);
                            //System.Diagnostics.Debug.Print("Address:{0}, Index:{1}, {2}", regInfo[SPDRP.Address], i, regInfo[SPDRP.DeviceDesc]);
                            deviceList.Add(regInfo);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.Message);
                        }
                    }
                    if (deviceHandle != null && !deviceHandle.IsClosed) deviceHandle.Close();
                }

                return deviceList;
            }
        }

        /// <summary>
        /// Check this value to determine if the usb device is still connected to the bus and ready to open.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (String.IsNullOrEmpty(SymbolicName)) throw new UsbException(this, "A symbolic name is required for this property.");
                List<LibUsbRegistry> deviceList = DeviceList;
                foreach (LibUsbRegistry registry in deviceList)
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
                LibUsbDevice libUsbDevice;
                Open(out libUsbDevice);
                return libUsbDevice;
            }
        }

        /// <summary>
        /// Gets the DeviceInterfaceGuids for the WinUsb device.
        /// </summary>
        public override Guid[] DeviceInterfaceGuids
        {
            get
            {
                if (ReferenceEquals(mDeviceInterfaceGuids, null))
                {
                    if (!mDeviceProperties.ContainsKey(LIBUSB_INTERFACE_GUIDS)) return new Guid[0];

                    string[] saDeviceInterfaceGuids = mDeviceProperties[LIBUSB_INTERFACE_GUIDS] as string[];
                    if (ReferenceEquals(saDeviceInterfaceGuids, null)) return new Guid[0];

                    mDeviceInterfaceGuids = new Guid[saDeviceInterfaceGuids.Length];

                    for (int i = 0; i < saDeviceInterfaceGuids.Length; i++)
                    {
                        string sGuid = saDeviceInterfaceGuids[i].Trim(new char[] {' ', '{', '}', '[', ']', '\0'});
                        mDeviceInterfaceGuids[i] = new Guid(sGuid);
                    }
                }
                return mDeviceInterfaceGuids;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public bool Open(out LibUsbDevice usbDevice)
        {
            bool bSuccess = LibUsbDevice.Open(mDeviceFilename, out usbDevice);
            if (bSuccess)
            {
                usbDevice.mUsbRegistry = this;
            }

            return bSuccess;
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public override bool Open(out UsbDevice usbDevice)
        {
            usbDevice = null;
            LibUsbDevice libUsbDevice;
            bool bSuccess = Open(out libUsbDevice);
            if (bSuccess)
                usbDevice = libUsbDevice;
            return bSuccess;
        }

        internal ErrorCode GetCustomDeviceKeyValue(SafeFileHandle usbHandle, string key, out string propData, int maxDataLength)
        {
            byte[] propDataBytes;
            ErrorCode eReturn = GetCustomDeviceKeyValue(usbHandle, key, out propDataBytes, maxDataLength);
            if (eReturn == ErrorCode.None)
            {
                propData = Encoding.Unicode.GetString(propDataBytes);
                propData.TrimEnd(new char[] {'\0'});
            }
            else
            {
                propData = null;
            }

            return eReturn;
        }

        internal ErrorCode GetCustomDeviceKeyValue(SafeFileHandle usbHandle, string key, out byte[] propData, int maxDataLength)
        {
            ErrorCode eReturn = ErrorCode.None;
            int iReturnBytes;
            propData = null;
            byte[] bytesReq = LibUsbDeviceRegistryKeyRequest.RegGetRequest(key, maxDataLength);
            GCHandle gcbytesReq = GCHandle.Alloc(bytesReq, GCHandleType.Pinned);

            bool bSuccess = LibUsbDriverIO.UsbIOSync(usbHandle,
                                                     LibUsbIoCtl.GET_CUSTOM_REG_PROPERTY,
                                                     bytesReq,
                                                     bytesReq.Length,
                                                     gcbytesReq.AddrOfPinnedObject(),
                                                     bytesReq.Length,
                                                     out iReturnBytes);
            gcbytesReq.Free();
            if (bSuccess)
            {
                propData = new byte[iReturnBytes];
                Array.Copy(bytesReq, propData, iReturnBytes);
            }
            else
            {
                eReturn = ErrorCode.GetDeviceKeyValueFailed;
                // dont log this as an error; 
                //UsbError.Error(eReturn,0, "Failed getting device registry Key:" + key, this);
            }
            return eReturn;
        }

        private void GetPropertiesSPDRP(SafeHandle usbHandle)
        {
            byte[] propBuffer = new byte[1024];
            GCHandle gcPropBuffer = GCHandle.Alloc(propBuffer, GCHandleType.Pinned);

            LibUsbRequest req = new LibUsbRequest();
            Dictionary<string, int> allProps = Helper.GetEnumData(typeof (DevicePropertyType));
            foreach (KeyValuePair<string, int> prop in allProps)
            {
                object oValue = String.Empty;

                req.DeviceProperty.ID = prop.Value;
                int iReturnBytes;
                bool bSuccess = LibUsbDriverIO.UsbIOSync(usbHandle,
                                                         LibUsbIoCtl.GET_REG_PROPERTY,
                                                         req,
                                                         LibUsbRequest.Size,
                                                         gcPropBuffer.AddrOfPinnedObject(),
                                                         propBuffer.Length,
                                                         out iReturnBytes);
                if (bSuccess)
                {
                    switch ((DevicePropertyType) prop.Value)
                    {
                        case DevicePropertyType.PhysicalDeviceObjectName:
                        case DevicePropertyType.LocationInformation:
                        case DevicePropertyType.Class:
                        case DevicePropertyType.Mfg:
                        case DevicePropertyType.DeviceDesc:
                        case DevicePropertyType.Driver:
                        case DevicePropertyType.EnumeratorName:
                        case DevicePropertyType.FriendlyName:
                        case DevicePropertyType.ClassGuid:
                            oValue = GetAsString(propBuffer, iReturnBytes);
                            break;
                        case DevicePropertyType.HardwareId:
                        case DevicePropertyType.CompatibleIds:
                            oValue = GetAsStringArray(propBuffer, iReturnBytes);
                            break;
                        case DevicePropertyType.BusNumber:
                        case DevicePropertyType.InstallState:
                        case DevicePropertyType.LegacyBusType:
                        case DevicePropertyType.RemovalPolicy:
                        case DevicePropertyType.UiNumber:
                        case DevicePropertyType.Address:
                            oValue = GetAsStringInt32(propBuffer, iReturnBytes);
                            break;
                        case DevicePropertyType.BusTypeGuid:
                            oValue = GetAsGuid(propBuffer, iReturnBytes);
                            break;
                    }
                }
                else
                    oValue = String.Empty;

                mDeviceProperties.Add(prop.Key, oValue);
            }
            gcPropBuffer.Free();
        }
    }
}