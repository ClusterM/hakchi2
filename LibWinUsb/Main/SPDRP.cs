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

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// Device registry property codes
    /// </summary> 
    [Flags]
    public enum SPDRP
    {
        /// <summary>
        /// Requests a string describing the device, such as "Microsoft PS/2 Port Mouse", typically defined by the manufacturer. 
        /// </summary>
        DeviceDesc = (0x00000000),
        /// <summary>
        /// Requests the hardware IDs provided by the device that identify the device.
        /// </summary>
        HardwareId = (0x00000001),
        /// <summary>
        /// Requests the compatible IDs reported by the device.
        /// </summary>
        CompatibleIds = (0x00000002),
        /// <summary>
        /// Requests the name of the device's setup class, in text format. 
        /// </summary>
        Class = (0x00000007),
        /// <summary>
        /// Requests the GUID for the device's setup class.
        /// </summary>
        ClassGuid = (0x00000008),
        /// <summary>
        /// Requests the name of the driver-specific registry key.
        /// </summary>
        Driver = (0x00000009),
        /// <summary>
        /// Requests a string identifying the manufacturer of the device.
        /// </summary>
        Mfg = (0x0000000B),
        /// <summary>
        /// Requests a string that can be used to distinguish between two similar devices, typically defined by the class installer.
        /// </summary>
        FriendlyName = (0x0000000C),
        /// <summary>
        /// Requests information about the device's location on the bus; the interpretation of this information is bus-specific. 
        /// </summary>
        LocationInformation = (0x0000000D),
        /// <summary>
        /// Requests the name of the PDO for this device.
        /// </summary>
        PhysicalDeviceObjectName = (0x0000000E),
        /// <summary>
        /// Requests a number associated with the device that can be displayed in the user interface.
        /// </summary>
        UiNumber = (0x00000010),
        /// <summary>
        /// Requests the GUID for the bus that the device is connected to.
        /// </summary>
        BusTypeGuid = (0x00000013),
        /// <summary>
        /// Requests the bus type, such as PCIBus or PCMCIABus.
        /// </summary>
        LegacyBusType = (0x00000014),
        /// <summary>
        /// Requests the legacy bus number of the bus the device is connected to. 
        /// </summary>
        BusNumber = (0x00000015),
        /// <summary>
        /// Requests the name of the enumerator for the device, such as "USB".
        /// </summary>
        EnumeratorName = (0x00000016),
        /// <summary>
        /// Requests the address of the device on the bus. 
        /// </summary>
        Address = (0x0000001C),
        /// <summary>
        /// (Windows XP and later.) Requests the device's current removal policy. The operating system uses this value as a hint to determine how the device is normally removed.
        /// </summary>
        RemovalPolicy = (0x0000001F),
        /// <summary>
        /// Windows XP and later.) Requests the device's installation state.
        /// </summary>
        InstallState = (0x00000022),
        /// <summary>
        /// Device Location Paths (R)
        /// </summary>
        LocationPaths=(0x00000023),

    }
}