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
namespace LibUsbDotNet.Main
{
    /// <summary> Standard Windows registry properties for USB devices and other hardware.
    /// </summary> 
    public enum DevicePropertyType
    {
        /// <summary>
        /// Requests a string describing the device, such as "Microsoft PS/2 Port Mouse", typically defined by the manufacturer. 
        /// </summary>
        DeviceDesc = 0,
        /// <summary>
        /// Requests the hardware IDs provided by the device that identify the device.
        /// </summary>
        HardwareId = 1,
        /// <summary>
        /// Requests the compatible IDs reported by the device.
        /// </summary>
        CompatibleIds = 2,
        /// <summary>
        /// Requests the name of the device's setup class, in text format. 
        /// </summary>
        Class = 5,
        /// <summary>
        /// Requests the GUID for the device's setup class.
        /// </summary>
        ClassGuid = 6,
        /// <summary>
        /// Requests the name of the driver-specific registry key.
        /// </summary>
        Driver = 7,
        /// <summary>
        /// Requests a string identifying the manufacturer of the device.
        /// </summary>
        Mfg = 8,
        /// <summary>
        /// Requests a string that can be used to distinguish between two similar devices, typically defined by the class installer.
        /// </summary>
        FriendlyName = 9,
        /// <summary>
        /// Requests information about the device's location on the bus; the interpretation of this information is bus-specific. 
        /// </summary>
        LocationInformation = 10,
        /// <summary>
        /// Requests the name of the PDO for this device.
        /// </summary>
        PhysicalDeviceObjectName = 11,
        /// <summary>
        /// Requests the GUID for the bus that the device is connected to.
        /// </summary>
        BusTypeGuid = 12,
        /// <summary>
        /// Requests the bus type, such as PCIBus or PCMCIABus.
        /// </summary>
        LegacyBusType = 13,
        /// <summary>
        /// Requests the legacy bus number of the bus the device is connected to. 
        /// </summary>
        BusNumber = 14,
        /// <summary>
        /// Requests the name of the enumerator for the device, such as "USB".
        /// </summary>
        EnumeratorName = 15,
        /// <summary>
        /// Requests the address of the device on the bus. 
        /// </summary>
        Address = 16,
        /// <summary>
        /// Requests a number associated with the device that can be displayed in the user interface.
        /// </summary>
        UiNumber = 17,
        /// <summary>
        /// Windows XP and later.) Requests the device's installation state.
        /// </summary>
        InstallState = 18,
        /// <summary>
        /// (Windows XP and later.) Requests the device's current removal policy. The operating system uses this value as a hint to determine how the device is normally removed.
        /// </summary>
        RemovalPolicy = 19
    }
}