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
using LibUsbDotNet.Internal.LibUsb;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.LudnMonoLibUsb;
using Microsoft.Win32.SafeHandles;
using Debug=System.Diagnostics.Debug;

namespace LibUsbDotNet.Main
{
    /// <summary> 
    /// LibUsb specific members for device registry settings.  
    /// This legacy class does not actually query the windows registry as the other Registry classes do. 
    /// Instead, it wraps a <see cref="LibUsbDevice"/> and queries descriptors directly from the device 
    /// using usb IO control messages.
    /// </summary> 
    public class LegacyUsbRegistry : UsbRegistry
    {

        private readonly UsbDevice mUSBDevice;
        internal LegacyUsbRegistry(UsbDevice usbDevice)
        {
            mUSBDevice = usbDevice;
            GetPropertiesSPDRP(mUSBDevice, mDeviceProperties);
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
                UsbDevice libUsbDevice;
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
                List<LegacyUsbRegistry> deviceList = DeviceList;
                foreach (LegacyUsbRegistry registry in deviceList)
                {
                    if (String.IsNullOrEmpty(registry.SymbolicName)) continue;

                    if (registry.SymbolicName == SymbolicName)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a list of available LibUsb devices.
        /// </summary>
        ///
        public static List<LegacyUsbRegistry> DeviceList
        {
            get
            {
                List<LegacyUsbRegistry> deviceList = new List<LegacyUsbRegistry>();
                if (UsbDevice.IsLinux)
                {
                    List<MonoUsbDevice> legacyLibUsbDeviceList = MonoUsbDevice.MonoUsbDeviceList;
                    foreach (MonoUsbDevice usbDevice in legacyLibUsbDeviceList)
                    {
                        deviceList.Add(new LegacyUsbRegistry(usbDevice));
                    }
                }
                else
                {
                    for (int i = 1; i < UsbConstants.MAX_DEVICES; i++)
                    {
                        string deviceFileName = LibUsbDriverIO.GetDeviceNameString(i);

                        SafeFileHandle deviceHandle = LibUsbDriverIO.OpenDevice(deviceFileName);
                        if (deviceHandle != null && !deviceHandle.IsInvalid && !deviceHandle.IsClosed)
                        {
                            try
                            {
                                LibUsbDevice newUsbDevice = new LibUsbDevice(UsbDevice.LibUsbApi, deviceHandle, deviceFileName);

                                LegacyUsbRegistry regInfo = new LegacyUsbRegistry(newUsbDevice);

                                deviceList.Add(regInfo);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                            }
                        }
                        if (deviceHandle != null && !deviceHandle.IsClosed) deviceHandle.Close();
                    }
                }
                return deviceList;
            }
        }

        ///// <summary>
        ///// ProductID
        ///// </summary>
        //public override int Pid
        //{
        //    get { return (int)((ushort)mUSBDevice.Info.Descriptor.ProductID); }
        //}

        ///// <summary>
        ///// VendorID
        ///// </summary>
        //public override int Vid
        //{
        //    get { return (int)((ushort)mUSBDevice.Info.Descriptor.VendorID); }
        //}

        /// <summary>
        /// Usb device revision number.
        /// </summary>
        public override int Rev
        {
            get
            {
                int bcdRev;
                string s = mUSBDevice.Info.Descriptor.BcdDevice.ToString("X4");
                if (int.TryParse(s, out bcdRev))
                {
                    return bcdRev;
                }
                return (int)((ushort)mUSBDevice.Info.Descriptor.BcdDevice);
            }
        }

        internal static string GetRegistryHardwareID(ushort vid, ushort pid, ushort rev)
        {
            return string.Format("Vid_{0:X4}&Pid_{1:X4}&Rev_{2}", vid, pid, rev.ToString("0000"));
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public override bool Open(out UsbDevice usbDevice)
        {
            usbDevice = null;
            bool bSuccess = mUSBDevice.Open();
            if (bSuccess)
            {
                usbDevice = mUSBDevice;
                usbDevice.mUsbRegistry = this;
            }

            return bSuccess;
        }

        internal static void GetPropertiesSPDRP(UsbDevice usbDevice, Dictionary<string, object> deviceProperties)
        {


            deviceProperties.Add(DevicePropertyType.Mfg.ToString(),
                                  usbDevice.Info.Descriptor.ManufacturerStringIndex > 0 ? usbDevice.Info.ManufacturerString : string.Empty);

            deviceProperties.Add(DevicePropertyType.DeviceDesc.ToString(),
                                  usbDevice.Info.Descriptor.ProductStringIndex > 0 ? usbDevice.Info.ProductString : string.Empty);

            deviceProperties.Add("SerialNumber",
                                  usbDevice.Info.Descriptor.SerialStringIndex > 0 ? usbDevice.Info.SerialString : string.Empty);

            string fakeHardwareIds = GetRegistryHardwareID((ushort)usbDevice.Info.Descriptor.VendorID,
                                                           (ushort)usbDevice.Info.Descriptor.ProductID,
                                                           (ushort)usbDevice.Info.Descriptor.BcdDevice);

            deviceProperties.Add(DevicePropertyType.HardwareId.ToString(), new string[] { fakeHardwareIds });

            string fakeSymbolicName = fakeHardwareIds + "{" + Guid.Empty + " }";

            if (usbDevice.Info.Descriptor.SerialStringIndex > 0)
            {
                fakeSymbolicName += "#" + deviceProperties["SerialNumber"] + "#";
            }
            deviceProperties.Add(SYMBOLIC_NAME_KEY, fakeSymbolicName);
        }
    }
}