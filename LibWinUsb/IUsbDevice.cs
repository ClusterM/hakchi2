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
using LibUsbDotNet.LudnMonoLibUsb;
using LibUsbDotNet.WinUsb;

namespace LibUsbDotNet
{
    /// <summary>
    /// The <see cref="IUsbDevice"/> interface contains members needed to configure a USB device for use. 
    /// </summary>
    /// <remarks>
    /// Only "whole" usb devices have a <see cref="IUsbDevice"/> interface such as a 
    /// <see cref="LibUsb.LibUsbDevice"/> or a <see cref="MonoUsbDevice"/>. This indicates
    /// the USB device must be properly configured by the user before it can be used.
    /// Partial or interfaces of devices such as a <see cref="WinUsbDevice"/> do not have an <see cref="IUsbDevice"/> 
    /// interface. This indicates that the driver is handling device configuration.
    /// </remarks>
    /// <example>
    /// This example uses the <see cref="IUsbDevice"/> interface to select the desired configuration and interface
    /// for usb devices that require it.
    /// <code source="..\Examples\Read.Write\ReadWrite.cs" lang="cs"/>
    /// </example>
    public interface IUsbDevice : IUsbInterface
    {
        /// <summary>
        /// Sets the USB devices active configuration value. 
        /// </summary>
        /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
        /// <returns>True on success.</returns>
        /// <remarks>
        /// A USB device can have several different configurations, but only one active configuration.
        /// </remarks>
        bool SetConfiguration(byte config);

        /// <summary>
        /// Gets the USB devices active configuration value. 
        /// </summary>
        /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
        /// <returns>True on success.</returns>
        bool GetConfiguration(out byte config);

        /// <summary>
        /// Sets an alternate interface for the most recent claimed interface.
        /// </summary>
        /// <param name="alternateID">The alternate interface to select for the most recent claimed interface See <see cref="MonoUsbDevice.ClaimInterface"/>.</param>
        /// <returns>True on success.</returns>
        bool SetAltInterface(int alternateID);

        /// <summary>
        /// Gets the selected alternate interface of the specified interface.
        /// </summary>
        /// <param name="interfaceID">The interface settings number (index) to retrieve the selected alternate interface setting for.</param>
        /// <param name="selectedAltInterfaceID">The alternate interface setting selected for use with the specified interface.</param>
        /// <returns>True on success.</returns>
        bool GetAltInterfaceSetting(byte interfaceID, out byte selectedAltInterfaceID);

        /// <summary>
        /// Claims the specified interface of the device.
        /// </summary>
        /// <param name="interfaceID">The interface to claim.</param>
        /// <returns>True on success.</returns>
        bool ClaimInterface(int interfaceID);

        /// <summary>
        /// Releases an interface that was previously claimed with <see cref="ClaimInterface"/>.
        /// </summary>
        /// <param name="interfaceID">The interface to release.</param>
        /// <returns>True on success.</returns>
        bool ReleaseInterface(int interfaceID);

        /// <summary>
        /// Sends a usb device reset command.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="ResetDevice"/>, the <see cref="UsbDevice"/> instance is disposed and
        /// no longer usable.  A new <see cref="UsbDevice"/> instance must be obtained from the device list.
        /// </remarks>
        /// <returns>True on success.</returns>
        bool ResetDevice();
    }
}