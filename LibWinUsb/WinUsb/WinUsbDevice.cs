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
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Internal;
using LibUsbDotNet.Internal.WinUsb;
using LibUsbDotNet.Main;
using LibUsbDotNet.WinUsb.Internal;
using Microsoft.Win32.SafeHandles;

namespace LibUsbDotNet.WinUsb
{
    /// <summary> 
    /// Contains members specific to Microsofts WinUSB driver.
    /// </summary> 
    /// <remarks>
    /// A <see cref="WinUsbDevice"/> should be thought of as a part of, or an interface of a USB device.
    /// The <see cref="WinUsbDevice"/> class does not have members for selecting configurations and
    /// intefaces.  This is done at a lower level by the winusb driver depending on which interface the
    /// <see cref="WinUsbDevice"/> belongs to.
    /// </remarks> 
    public class WinUsbDevice : UsbDevice, IUsbInterface
    {
        private readonly string mDevicePath;
        private PowerPolicies mPowerPolicies;
        private SafeFileHandle mSafeDevHandle;

        internal WinUsbDevice(UsbApiBase usbApi,
                              SafeFileHandle usbHandle,
                              SafeHandle handle,
                              string devicePath)
            : base(usbApi, handle)
        {
            mDevicePath = devicePath;
            mSafeDevHandle = usbHandle;
            mPowerPolicies = new PowerPolicies(this);
        }

        /// <summary>
        /// Gets the power policies for this <see cref="WinUsbDevice"/>.
        /// </summary>
        public PowerPolicies PowerPolicy
        {
            get { return mPowerPolicies; }
        }

        /// <summary>
        /// Gets the device path used to open this <see cref="WinUsbDevice"/>.
        /// </summary>
        public string DevicePath
        {
            get { return mDevicePath; }
        }

        #region IUsbInterface Members

        /// <summary>
        /// Returns the DriverMode this USB device is using.
        /// </summary>
        public override DriverModeType DriverMode
        {
            get { return DriverModeType.WinUsb; }
        }

        /// <summary>
        /// Closes the <see cref="UsbDevice"/> and disposes any <see cref="UsbDevice.ActiveEndpoints"/>.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Close()
        {
            if (IsOpen)
            {
                ActiveEndpoints.Clear();
                mUsbHandle.Close();

                if (mSafeDevHandle != null)
                    if (!mSafeDevHandle.IsClosed)
                        mSafeDevHandle.Close();
            }
            return true;
        }

        ///<summary>
        /// Opens the USB device handle.
        ///</summary>
        ///<returns>
        ///True if the device is already opened or was opened successfully.
        ///False if the device does not exists or is no longer valid.  
        ///</returns>
        public override bool Open()
        {
            if (IsOpen) return true;

            SafeFileHandle sfhDev;

            bool bSuccess = WinUsbAPI.OpenDevice(out sfhDev, mDevicePath);
            if (bSuccess)
            {
                SafeWinUsbInterfaceHandle handle = new SafeWinUsbInterfaceHandle();
                if ((bSuccess = WinUsbAPI.WinUsb_Initialize(sfhDev, ref handle)))
                {
                    mSafeDevHandle = sfhDev;
                    mUsbHandle = handle;
                    mPowerPolicies = new PowerPolicies(this);
                }
                else
                    UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open:Initialize", typeof (UsbDevice));
            }
            else
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open", typeof(UsbDevice));


            return bSuccess;
        }

        #endregion

        /// <summary>
        /// Opens a WinUsb directly from the user supplied device path. 
        /// </summary>
        /// <param name="devicePath">Device path (symbolic link) of the WinUsb device to open.</param>
        /// <param name="usbDevice">Returns an opened WinUsb device on success, null on failure.</param>
        /// <returns>True on success.</returns>
        public static bool Open(string devicePath, out WinUsbDevice usbDevice)
        {
            usbDevice = null;

            SafeFileHandle sfhDev;

            bool bSuccess = WinUsbAPI.OpenDevice(out sfhDev, devicePath);
            if (bSuccess)
            {
                SafeWinUsbInterfaceHandle handle = new SafeWinUsbInterfaceHandle();
                bSuccess = WinUsbAPI.WinUsb_Initialize(sfhDev, ref handle);
                if (bSuccess)
                {
                    usbDevice = new WinUsbDevice(WinUsbApi, sfhDev, handle, devicePath);
                }
                else
                    UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open:Initialize", typeof(UsbDevice));
            }
            else
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open", typeof(UsbDevice));


            return bSuccess;
        }

        /// <summary>
        /// Gets endpoint policies for the specified endpoint id.
        /// </summary>
        /// <param name="epNum">The endpoint ID to retrieve <see cref="PipePolicies"/> for.</param>
        /// <returns>A <see cref="PipePolicies"/> class.</returns>
        public PipePolicies EndpointPolicies(ReadEndpointID epNum) { return new PipePolicies(mUsbHandle, (byte) epNum); }

        /// <summary>
        /// Gets endpoint policies for the specified endpoint id.
        /// </summary>
        /// <param name="epNum">The endpoint ID to retrieve <see cref="PipePolicies"/> for.</param>
        /// <returns>A <see cref="PipePolicies"/> class.</returns>
        public PipePolicies EndpointPolicies(WriteEndpointID epNum) { return new PipePolicies(mUsbHandle, (byte) epNum); }

        /// <summary>
        /// Gets an interface associated with this <see cref="WinUsbDevice"/>.
        /// </summary>
        /// <param name="associatedInterfaceIndex">The index to retrieve. (0 = next interface, 1= interface after next, etc.).</param>
        /// <param name="usbDevice">A new <see cref="WinUsbDevice"/> class for the specified AssociatedInterfaceIndex.</param>
        /// <returns>True on success.</returns>
        public bool GetAssociatedInterface(byte associatedInterfaceIndex, out WinUsbDevice usbDevice)
        {
            usbDevice = null;
            IntPtr pHandle = IntPtr.Zero;
            bool bSuccess = WinUsbAPI.WinUsb_GetAssociatedInterface(mUsbHandle, associatedInterfaceIndex, ref pHandle);
            if (bSuccess)
            {
                SafeWinUsbInterfaceHandle tempHandle = new SafeWinUsbInterfaceHandle(pHandle);

                usbDevice = new WinUsbDevice(mUsbApi, null, tempHandle, mDevicePath);
            }
            if (!bSuccess)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetAssociatedInterface", this);

            return bSuccess;
        }

        /// <summary>
        /// Gets the currently selected alternate settings number for the selected inteface.
        /// </summary>
        /// <param name="settingNumber">The selected AlternateSetting number.</param>
        /// <returns>True on success.</returns>
        public bool GetCurrentAlternateSetting(out byte settingNumber)
        {
            bool bSuccess;
            //settingNumber = 0;
            //if (LockDevice() != ErrorCode.None) return false;

            //try
            //{
            bSuccess = WinUsbAPI.WinUsb_GetCurrentAlternateSetting(mUsbHandle, out settingNumber);

            if (!bSuccess)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetCurrentAlternateSetting", this);
            //}
            //finally
            //{
            //    UnlockDevice();
            //}


            return bSuccess;
        }

        /// <summary>
        /// Gets the device speed.
        /// </summary>
        /// <param name="deviceSpeed">The device speed.</param>
        /// <returns>True on success.</returns>
        public bool QueryDeviceSpeed(out DeviceSpeedTypes deviceSpeed)
        {
            deviceSpeed = DeviceSpeedTypes.Undefined;
            byte[] buf = new byte[1];
            int uTransferLength = 1;
            bool bSuccess = WinUsbAPI.WinUsb_QueryDeviceInformation(mUsbHandle, DeviceInformationTypes.DeviceSpeed, ref uTransferLength, buf);

            if (bSuccess)
            {
                deviceSpeed = (DeviceSpeedTypes) buf[0];
            }
            else
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "QueryDeviceInformation:QueryDeviceSpeed", this);

            return bSuccess;
        }

        /// <summary>
        /// Gets a <see cref="UsbInterfaceDescriptor"/> for the specified AlternateInterfaceNumber,
        /// </summary>
        /// <param name="alternateInterfaceNumber">The alternate interface index for the <see cref="UsbInterfaceDescriptor"/> to retrieve. </param>
        /// <param name="usbAltInterfaceDescriptor">The <see cref="UsbInterfaceDescriptor"/> for the specified AlternateInterfaceNumber.</param>
        /// <returns>True on success.</returns>
        public bool QueryInterfaceSettings(byte alternateInterfaceNumber, ref UsbInterfaceDescriptor usbAltInterfaceDescriptor)
        {
            bool bSuccess;
            //if (mSemDeviceLock != null)
            //{
            //    if (LockDevice() != ErrorCode.None) return false;
            //}

            //try
            //{
            bSuccess = WinUsbAPI.WinUsb_QueryInterfaceSettings(Handle, alternateInterfaceNumber, usbAltInterfaceDescriptor);
            if (!bSuccess)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "QueryInterfaceSettings", this);
            //}
            //finally
            //{
            //    if (mSemDeviceLock != null) UnlockDevice();
            //}

            return bSuccess;
        }

        internal bool GetPowerPolicy(PowerPolicyType policyType, ref int valueLength, IntPtr pBuffer)
        {
            bool bSuccess = WinUsbAPI.WinUsb_GetPowerPolicy(mUsbHandle, policyType, ref valueLength, pBuffer);

            if (!bSuccess)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "GetPowerPolicy", this);

            return bSuccess;
        }

        /// <summary>
        /// Gets a list a valid, connected WinUSB device inteface paths for the a given WinUSB device interface guid.
        /// </summary>
        /// <param name="interfaceGuid">A WinUSB DeviceInterfaceGUID.  This is set in the usb devices inf file when the drivers for it are installed.</param>
        /// <param name="devicePathList">A list of connected WinUSB device inteface paths.</param>
        /// <returns>True if one or more device paths were found.  False if no devices are found or an error occured. <see cref="UsbDevice.UsbErrorEvent"/> </returns>
        public static bool GetDevicePathList(Guid interfaceGuid, out List<String> devicePathList) { return WinUsbRegistry.GetDevicePathList(interfaceGuid, out devicePathList); }

        internal bool SetPowerPolicy(PowerPolicyType policyType, int valueLength, IntPtr pBuffer)
        {
            bool bSuccess = WinUsbAPI.WinUsb_SetPowerPolicy(mUsbHandle, policyType, valueLength, pBuffer);

            if (!bSuccess)
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "SetPowerPolicy", this);

            return bSuccess;
        }

        /// <summary>
        /// Closes the device. <see cref="WinUsbDevice.Close"/>.
        /// </summary>
        ~WinUsbDevice() { Close(); }
    }
}