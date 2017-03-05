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
using System.Text;
using LibUsbDotNet.Descriptors;

namespace LibUsbDotNet.Main
{
    /// <summary> USB device registry members common to both LibUsb and WinUsb devices.
    /// </summary> 
    public abstract class UsbRegistry //: IEquatable<UsbRegistry>
    {
        internal const string DEVICE_INTERFACE_GUIDS = "DeviceInterfaceGuids";
        internal const string LIBUSB_INTERFACE_GUIDS = "LibUsbInterfaceGUIDs";

        internal const string SYMBOLIC_NAME_KEY = "SymbolicName";
        internal const string DEVICE_ID_KEY = "DeviceID";

        private static readonly char[] ChNull = new char[] {'\0'};

        /// <summary>
        /// If true, LibUsbDotNet will use the vid, pid and revision of the <see cref="UsbDevice.Info"/> 
        /// descriptor to lookup additional device information in the windows registry via the setupapi.
        /// Setting this field to false will cause all device information to come directly from the 
        /// device descriptors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If using WinUSB or the LibUsbDotNet-libusb-win32 native driver, information provided by 
        /// the <see cref="UsbRegistry"/> class will come from the registry regardless of this setting
        /// because these drivers have direct support for this.
        /// </para>
        /// <para>
        /// The Libusb-1.0 windows-backend driver and the legacy libusb-win32 driver have their own methods
        /// for listing, finding, and opening devices.  For these drivers, the <see cref="ForceSetupApi"/> can
        /// be set to do a "reverse lookup" via the setupapi using only the vid, pid and revision of the 
        /// <see cref="UsbDeviceDescriptor"/>.  The <see cref="UsbRegistry"/> class is then populated with
        /// all available <see cref="SPDRP"/> properties, device interface guids, winusb device paths, etc.
        /// </para>
        /// </remarks>
        public static bool ForceSetupApi = true;

        /// <summary>
        /// Guid array of all <see cref="DeviceInterfaceGuids"/> assigned to this device.
        /// </summary>
        internal Guid[] mDeviceInterfaceGuids=new Guid[0];

        internal Dictionary<string, object> mDeviceProperties = new Dictionary<string, object>();

        private UsbSymbolicName mSymHardwareId;

        /// <summary>
        /// Collection of known usb device properties (from the registry).
        /// </summary>
        public Dictionary<string, object> DeviceProperties
        {
            get { return mDeviceProperties; }
        }

        /// <summary>
        /// Check this value to determine if the usb device is still connected to the bus and ready to open.
        /// </summary>
        /// <remarks>
        /// Uses the symbolic name as a unique id to determine if this device instance is still attached.
        /// </remarks>
        /// <exception cref="UsbException">An exception is thrown if the <see cref="UsbRegistry.SymbolicName"/> property is null or empty.</exception>
        public abstract bool IsAlive { get; }

        /// <summary>
        /// The unique "SymbolicName" of the device.
        /// </summary>
        public string SymbolicName
        {
            get
            {
                if (mDeviceProperties.ContainsKey(SYMBOLIC_NAME_KEY))
                    return (string) mDeviceProperties[SYMBOLIC_NAME_KEY];
                return null;
            }
        }

        /// <summary>
        /// The unique "SymbolicName" of the device.
        /// </summary>
        public abstract Guid[] DeviceInterfaceGuids { get; }

        /// <summary>
        /// VendorID
        /// </summary>
        public virtual int Vid
        {
            get
            {
                if (ReferenceEquals(mSymHardwareId, null))
                {
                    string[] saHardwareIds = mDeviceProperties[SPDRP.HardwareId.ToString()] as string[];
                    if (saHardwareIds != null && saHardwareIds.Length > 0)
                    {
                        mSymHardwareId = UsbSymbolicName.Parse(saHardwareIds[0]);
                    }
                }
                if (!ReferenceEquals(mSymHardwareId, null))
                {
                    return mSymHardwareId.Vid;
                }

                return 0;
            }
        }

        /// <summary>
        /// ProductID
        /// </summary>
        public virtual int Pid
        {
            get
            {
                if (ReferenceEquals(mSymHardwareId, null))
                {
                    string[] saHardwareIds = mDeviceProperties[SPDRP.HardwareId.ToString()] as string[];
                    if (saHardwareIds != null && saHardwareIds.Length > 0)
                    {
                        mSymHardwareId = UsbSymbolicName.Parse(saHardwareIds[0]);
                    }
                }
                if (!ReferenceEquals(mSymHardwareId, null))
                {
                    return mSymHardwareId.Pid;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a device property/key from the registry.
        /// </summary>
        /// <param name="name">The name of the property to retrieve.</param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                object temp;
                mDeviceProperties.TryGetValue(name, out temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets a device property/key from the registry.  See the <see cref="SPDRP"/> enumeration for more information.
        /// </summary>
        /// <param name="spdrp">The name of the property to retrieve.</param>
        /// <returns></returns>
        public object this[SPDRP spdrp]
        {
            get
            {
                object temp;
                mDeviceProperties.TryGetValue(spdrp.ToString(), out temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets a property from the registry.  See the <see cref="DevicePropertyType"/> enumeration for more information.
        /// </summary>
        /// <param name="devicePropertyType">The name of the property to retrieve.</param>
        /// <returns></returns>
        public object this[DevicePropertyType devicePropertyType]
        {
            get
            {
                object temp;
                mDeviceProperties.TryGetValue(devicePropertyType.ToString(), out temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets the short name of the usb device.
        /// </summary>
        /// <remarks>This is the device decription as it is defined in the setup/inf file.</remarks>
        public string Name
        {
            get
            {
                string deviceDesc = this[SPDRP.DeviceDesc] as string;
                if (String.IsNullOrEmpty(deviceDesc)) return string.Empty;
                return deviceDesc;
            }
        }

        /// <summary>
        /// Gets the manufacturer followed by the device decription in the format 'Mfu - Description'
        /// </summary>
        /// <remarks>This property works best for a display name.  It does additional proccessing on the manufacturer and device description that make it more user readable.</remarks>
        public string FullName
        {
            get
            {
                string deviceDesc = Name;
                string mfg = this[SPDRP.Mfg] as string;
                if (mfg == null) mfg = String.Empty;
                deviceDesc = deviceDesc.Trim();
                mfg = mfg.Trim();

                int firstMfuSpace = mfg.IndexOf(' ');
                int firstDescSpace = deviceDesc.IndexOf(' ');
                while (firstMfuSpace == firstDescSpace && firstDescSpace != -1)
                {
                    if (mfg.Substring(0, firstMfuSpace).Equals(deviceDesc.Substring(0, firstDescSpace)))
                    {
                        deviceDesc = deviceDesc.Remove(0, firstDescSpace + 1);
                        firstMfuSpace = mfg.IndexOf(' ');
                        firstDescSpace = deviceDesc.IndexOf(' ');
                    }
                    else
                    {
                        break;
                    }
                }

                if (deviceDesc.ToLower().Contains(mfg.ToLower()))
                    return deviceDesc;

                if (mfg == string.Empty) mfg = "[Not Provided]";
                if (deviceDesc == string.Empty) deviceDesc = "[Not Provided]";

                return (mfg + " - " + deviceDesc);
            }
        }

        /// <summary>
        /// Number of properties in the array.
        /// </summary>
        public int Count
        {
            get { return mDeviceProperties.Count; }
        }

        /// <summary>
        /// Usb device revision number.
        /// </summary>
        public virtual int Rev
        {
            get
            {
                if (ReferenceEquals(mSymHardwareId, null))
                {
                    string[] saHardwareIds = mDeviceProperties[SPDRP.HardwareId.ToString()] as string[];
                    if (saHardwareIds != null && saHardwareIds.Length > 0)
                    {
                        mSymHardwareId = UsbSymbolicName.Parse(saHardwareIds[0]);
                    }
                }
                if (!ReferenceEquals(mSymHardwareId, null))
                {
                    return mSymHardwareId.Rev;
                }

                return 0;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <returns>Return a new instance of the <see cref="UsbDevice"/> class.
        /// If the device fails to open a null refrence is return. For extended error
        /// information see the <see cref="UsbDevice.UsbErrorEvent"/>.
        ///  </returns>
        public abstract UsbDevice Device { get; }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public abstract bool Open(out UsbDevice usbDevice);

        internal static Guid GetAsGuid(byte[] buffer, int len)
        {
            Guid rtn = Guid.Empty;
            if (len == 16)
            {
                byte[] guidBytes = new byte[len];
                Array.Copy(buffer, guidBytes, guidBytes.Length);
                rtn = new Guid(guidBytes);
            }

            return rtn;
        }

        internal static string GetAsString(byte[] buffer, int len)
        {
            if (len > 2) return Encoding.Unicode.GetString(buffer, 0, len).TrimEnd(ChNull);

            return "";
        }

        internal static string[] GetAsStringArray(byte[] buffer, int len) { return GetAsString(buffer, len).Split(new char[] {'\0'}, StringSplitOptions.RemoveEmptyEntries); }

        internal static Int32 GetAsStringInt32(byte[] buffer, int len)
        {
            Int32 iRtn = 0;
            if (len == 4)
                iRtn = buffer[0] | ((buffer[1]) << 8) | ((buffer[2]) << 16) | ((buffer[3]) << 24);
            return iRtn;
        }
    }
}