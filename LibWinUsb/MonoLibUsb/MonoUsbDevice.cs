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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using MonoLibUsb;
using MonoLibUsb.Descriptors;
using MonoLibUsb.Profile;

namespace LibUsbDotNet.LudnMonoLibUsb
{
    /// <summary>This is the LibUsbDotNet Libusb-1.0 implementation of a <see cref="UsbDevice"/>.
    /// </summary> 
    /// <remarks>
    /// <para>This class is used for perform I/O and other operations on Libusb-1.0 devices using with LibUsbDotNet.</para>
    /// <para>This class is not a part of the low-level MonLibUsb API.  This is <see cref="UsbDevice"/> class LibUsbDotNet uses to implement the low-level MonoLibUsb API.</para>
    /// </remarks> 
    public class MonoUsbDevice : UsbDevice, IUsbDevice
    {
        internal static readonly object OLockDeviceList = new object();
        internal static MonoUsbProfileList mMonoUSBProfileList;
        private readonly MonoUsbProfile mMonoUSBProfile;

        private int mClaimedInteface;

        internal MonoUsbDevice(ref MonoUsbProfile monoUSBProfile)
            : base(null, null)
        {
            mMonoUSBProfile = monoUSBProfile;
            mCachedDeviceDescriptor = new UsbDeviceDescriptor(monoUSBProfile.DeviceDescriptor);
        }

        internal static MonoUsbProfileList ProfileList
        {
            get
            {
                lock (OLockDeviceList)
                {
                    MonoUsbApi.InitAndStart();
                    if (mMonoUSBProfileList == null)
                    {
                        mMonoUSBProfileList = new MonoUsbProfileList();
                    }
                    return mMonoUSBProfileList;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="MonoUsbDevice"/> list of Libusb-1.0 devices.  
        /// </summary>
        /// <remarks>
        /// <para>Using the <see cref="MonoUsbDeviceList"/> property will request a device list directly from the <a href="http://www.libusb.org/">Libusb-1.0</a> driver.</para> 
        /// <para><a href="http://www.libusb.org/">Libusb-1.0</a> is compatible with several platforms including windows.</para>
        /// <para>You can force LibUsbDotNet to always use <a href="http://www.libusb.org/">Libusb-1.0</a> with the <see cref="UsbDevice.ForceLibUsbWinBack"/> member.</para>
        /// <seealso cref="UsbDevice.AllDevices"/>
        /// <seealso cref="UsbDevice.AllLibUsbDevices"/>
        /// </remarks>
        public static List<MonoUsbDevice> MonoUsbDeviceList
        {
            get
            {
                lock (OLockDeviceList)
                {
                    MonoUsbApi.InitAndStart();
                    if (mMonoUSBProfileList == null)
                    {
                        mMonoUSBProfileList = new MonoUsbProfileList();
                    }
                    int ret = (int) mMonoUSBProfileList.Refresh(MonoUsbEventHandler.SessionHandle);
                    if (ret < 0) return null;
                    List<MonoUsbDevice> rtnList = new List<MonoUsbDevice>();
                    for (int iProfile = 0; iProfile < mMonoUSBProfileList.Count; iProfile++)
                    {
                        MonoUsbProfile monoUSBProfile = mMonoUSBProfileList[iProfile];
                        if (monoUSBProfile.DeviceDescriptor.BcdUsb == 0) continue;
                        MonoUsbDevice newDevice = new MonoUsbDevice(ref monoUSBProfile);
                        rtnList.Add(newDevice);
                    }

                    return rtnList;
                }
            }
        }

        /// <summary>
        /// Gets the instance address the device is using.
        /// </summary>
        public byte DeviceAddress
        {
            get { return mMonoUSBProfile.DeviceAddress; }
        }

        /// <summary>
        /// Gets the bus number the device is connected to.
        /// </summary>
        public byte BusNumber
        {
            get { return mMonoUSBProfile.BusNumber; }
        }

        #region IUsbDevice Members

        /// <summary>
        /// Sends a usb device reset command.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="ResetDevice"/>, the <see cref="MonoUsbDevice"/> instance is disposed and
        /// no longer usable.  A new <see cref="MonoUsbDevice"/> instance must be obtained from the device list.
        /// </remarks>
        /// <returns>True on success.</returns>
        public bool ResetDevice()
        {
            int ret;
            if (!IsOpen) throw new UsbException(this, "Device is not opened.");
            ActiveEndpoints.Clear();

            if ((ret = MonoUsbApi.ResetDevice((MonoUsbDeviceHandle) mUsbHandle)) != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "ResetDevice Failed", this);
            }
            else
            {
                Close();
            }

            return ret == 0;
        }

        /// <summary>
        /// Returns the DriverMode this USB device is using.
        /// </summary>
        public override DriverModeType DriverMode
        {
            get
            {
                if (IsLinux)
                    return DriverModeType.MonoLibUsb;

                return DriverModeType.LibUsbWinBack;
            }
        }

        /// <summary>
        /// Closes the <see cref="UsbDevice"/> and disposes any <see cref="UsbDevice.ActiveEndpoints"/>.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Close()
        {
            ActiveEndpoints.Clear();
            if (IsOpen) mUsbHandle.Close();

            return true;
        }


        /// <summary>
        /// Transmits control data over a default control endpoint.
        /// </summary>
        /// <param name="setupPacket">An 8-byte setup packet which contains parameters for the control request. 
        /// See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information. </param>
        /// <param name="buffer">Data to be sent/received from the device.</param>
        /// <param name="bufferLength">Length of the buffer param.</param>
        /// <param name="lengthTransferred">Number of bytes sent or received (depends on the direction of the control transfer).</param>
        /// <returns>True on success.</returns>
        public override bool ControlTransfer(ref UsbSetupPacket setupPacket, IntPtr buffer, int bufferLength, out int lengthTransferred)
        {
            Debug.WriteLine(GetType().Name + ".ControlTransfer() Before", "Libusb-1.0");
            int ret = MonoUsbApi.ControlTransferAsync((MonoUsbDeviceHandle) mUsbHandle,
                                                      setupPacket.RequestType,
                                                      setupPacket.Request,
                                                      setupPacket.Value,
                                                      setupPacket.Index,
                                                      buffer,
                                                      (short) bufferLength,
                                                      UsbConstants.DEFAULT_TIMEOUT);

            Debug.WriteLine(GetType().Name + ".ControlTransfer() Error:" + ((MonoUsbError) ret).ToString(), "Libusb-1.0");
            if (ret < 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "ControlTransfer Failed", this);
                lengthTransferred = 0;
                return false;
            }
            lengthTransferred = ret;
            return true;
        }

        /// <summary>
        /// Gets a descriptor from the device. See <see cref="DescriptorType"/> for more information.
        /// </summary>
        /// <param name="descriptorType">The descriptor type ID to retrieve; this is usually one of the <see cref="DescriptorType"/> enumerations.</param>
        /// <param name="index">Descriptor index.</param>
        /// <param name="langId">Descriptor language id.</param>
        /// <param name="buffer">Memory to store the returned descriptor in.</param>
        /// <param name="bufferLength">Length of the buffer parameter in bytes.</param>
        /// <param name="transferLength">The number of bytes transferred to buffer upon success.</param>
        /// <returns>True on success.</returns>
        public override bool GetDescriptor(byte descriptorType, byte index, short langId, IntPtr buffer, int bufferLength, out int transferLength)
        {
            transferLength = 0;
            bool bSuccess = false;
            bool wasOpen = IsOpen;
            if (!wasOpen) Open();
            if (!IsOpen) return false;

            int ret = MonoUsbApi.GetDescriptor((MonoUsbDeviceHandle) mUsbHandle, descriptorType, index, buffer, (ushort) bufferLength);

            if (ret < 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "GetDescriptor Failed", this);
            }
            else
            {
                bSuccess = true;
                transferLength = ret;
            }

            if (!wasOpen && IsOpen) Close();

            return bSuccess;
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
            MonoUsbDeviceHandle handle = new MonoUsbDeviceHandle(mMonoUSBProfile.ProfileHandle);
            if (handle.IsInvalid)
            {
                UsbError.Error(ErrorCode.MonoApiError, (int) MonoUsbDeviceHandle.LastErrorCode, "MonoUsbDevice.Open Failed", this);
                mUsbHandle = null;
                return false;
            }
            mUsbHandle = handle;
            if (IsOpen) return true;

            mUsbHandle.Close();
            return false;
        }

        /// <summary>
        /// Gets the <see cref="MonoUsbProfile"/> for this usb device.
        /// </summary>
        public MonoUsbProfile Profile
        {
            get
            {
                return mMonoUSBProfile;
            }
        }
        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpointID">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <param name="endpointType">The type of endpoint to open.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
        public override UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize, EndpointType endpointType)
        {
            foreach (UsbEndpointBase activeEndpoint in mActiveEndpoints)
                if (activeEndpoint.EpNum == (byte)readEndpointID) 
                    return (UsbEndpointReader)activeEndpoint;

            UsbEndpointReader epNew = new MonoUsbEndpointReader(this, readBufferSize, readEndpointID, endpointType);
            return (UsbEndpointReader) ActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for writing
        /// </summary>
        /// <param name="writeEndpointID">Endpoint number for read operations.</param>
        /// <param name="endpointType">The type of endpoint to open.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
        public override UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID, EndpointType endpointType)
        {
            foreach (UsbEndpointBase activeEndpoint in ActiveEndpoints)
                if (activeEndpoint.EpNum == (byte)writeEndpointID)
                    return (UsbEndpointWriter)activeEndpoint;

            UsbEndpointWriter epNew = new MonoUsbEndpointWriter(this, writeEndpointID, endpointType);
            return (UsbEndpointWriter) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Sets the USB devices active configuration value. 
        /// </summary>
        /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
        /// <returns>True on success.</returns>
        /// <remarks>
        /// A USB device can have several different configurations, but only one active configuration.
        /// </remarks>
        public bool SetConfiguration(byte config)
        {
            int ret = MonoUsbApi.SetConfiguration((MonoUsbDeviceHandle) mUsbHandle, config);
            if (ret != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "SetConfiguration Failed", this);
                return false;
            }
            mCurrentConfigValue = config;
            return true;
        }

        /// <summary>
        /// Gets the USB devices active configuration value. 
        /// </summary>
        /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
        /// <returns>True on success.</returns>
        public override bool GetConfiguration(out byte config)
        {
            config = 0;
            int iconfig = 0;
            int ret = MonoUsbApi.GetConfiguration((MonoUsbDeviceHandle) mUsbHandle, ref iconfig);
            if (ret != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "GetConfiguration Failed", this);
                return false;
            }
            config = (byte) iconfig;
            mCurrentConfigValue = config;

            return true;
        }

        /// <summary>
        /// Gets the <see cref="UsbRegistry"/> class that opened the device, or null if the device was not opened by the <see cref="UsbRegistry"/> class.
        /// </summary>
        public override UsbRegistry UsbRegistryInfo
        {
            get { return null; }
        }

        ///<summary>
        /// Gets the available configurations for this <see cref="UsbDevice"/>
        ///</summary>
        /// <remarks>
        /// The first time this property is accessed it will query the <see cref="UsbDevice"/> for all configurations.  Subsequent request will return a cached copy of all configurations.
        /// </remarks>
        public override ReadOnlyCollection<UsbConfigInfo> Configs
        {
            get
            {
                if (ReferenceEquals(mConfigs, null))
                {
                    if (!IsOpen)
                    {
                        //Console.WriteLine("Device Not Opened!");
                        return null;
                    }
                    GetConfigs(this, out mConfigs);
                }

                return mConfigs.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the actual device descriptor the the current <see cref="UsbDevice"/>.
        /// </summary>
        public override UsbDeviceInfo Info
        {
            get
            {
                if (ReferenceEquals(mDeviceInfo, null))
                {
                    mDeviceInfo = new UsbDeviceInfo(this, mMonoUSBProfile.DeviceDescriptor);
                }
                return mDeviceInfo;
            }
        }

        /// <summary>
        /// Claims the specified interface of the device.
        /// </summary>
        /// <param name="interfaceID">The interface to claim.</param>
        /// <returns>True on success.</returns>
        public bool ClaimInterface(int interfaceID)
        {
            int ret = MonoUsbApi.ClaimInterface((MonoUsbDeviceHandle) mUsbHandle, interfaceID);
            if (ret != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "ClaimInterface Failed", this);
                return false;
            }
            mClaimedInteface = interfaceID;
            return true;
        }

        /// <summary>
        /// Releases an interface that was previously claimed with <see cref="ClaimInterface"/>.
        /// </summary>
        /// <param name="interfaceID">The interface to release.</param>
        /// <returns>True on success.</returns>
        public bool ReleaseInterface(int interfaceID)
        {
            int ret = MonoUsbApi.ReleaseInterface((MonoUsbDeviceHandle) mUsbHandle, interfaceID);
            if (ret != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "ReleaseInterface Failed", this);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets an alternate interface for the most recent claimed interface.
        /// </summary>
        /// <param name="alternateID">The alternate interface to select for the most recent claimed interface See <see cref="ClaimInterface"/>.</param>
        /// <returns>True on success.</returns>
        public bool SetAltInterface(int alternateID)
        {
            int ret = MonoUsbApi.SetInterfaceAltSetting((MonoUsbDeviceHandle) mUsbHandle, mClaimedInteface, alternateID);
            if (ret != 0)
            {
                UsbError.Error(ErrorCode.MonoApiError, ret, "SetAltInterface Failed", this);
                return false;
            }
            return true;
        }

        #endregion

        #region IDisposable implementation

        #endregion

        private static ErrorCode GetConfigs(MonoUsbDevice usbDevice, out List<UsbConfigInfo> configInfoListRtn)
        {
            configInfoListRtn = new List<UsbConfigInfo>();
            UsbError usbError = null;
            List<MonoUsbConfigDescriptor> configList = new List<MonoUsbConfigDescriptor>();
            int iConfigs = usbDevice.Info.Descriptor.ConfigurationCount;

            for (int iConfig = 0; iConfig < iConfigs; iConfig++)
            {
                MonoUsbConfigHandle nextConfigHandle;
                int ret = MonoUsbApi.GetConfigDescriptor(usbDevice.mMonoUSBProfile.ProfileHandle, (byte) iConfig, out nextConfigHandle);
                Debug.Print("GetConfigDescriptor:{0}", ret);
                if (ret != 0 || nextConfigHandle.IsInvalid)
                {
                    usbError = UsbError.Error(ErrorCode.MonoApiError,
                                              ret,
                                              String.Format("GetConfigDescriptor Failed at index:{0}", iConfig),
                                              usbDevice);
                    return usbError.ErrorCode;
                }
                try
                {
                    MonoUsbConfigDescriptor nextConfig = new MonoUsbConfigDescriptor();
                    Marshal.PtrToStructure(nextConfigHandle.DangerousGetHandle(), nextConfig);

                    UsbConfigInfo nextConfigInfo = new UsbConfigInfo(usbDevice, nextConfig);
                    configInfoListRtn.Add(nextConfigInfo);
                }
                catch (Exception ex)
                {
                    UsbError.Error(ErrorCode.InvalidConfig, Marshal.GetLastWin32Error(), ex.ToString(), usbDevice);
                }
                finally
                {
                    if (!nextConfigHandle.IsInvalid)
                        nextConfigHandle.Close();
                }
            }

            return ErrorCode.Success;
        }

        internal static int RefreshProfileList()
        {
            lock (OLockDeviceList)
            {
                MonoUsbApi.InitAndStart();
                if (mMonoUSBProfileList == null)
                {
                    mMonoUSBProfileList = new MonoUsbProfileList();
                }
                return (int) mMonoUSBProfileList.Refresh(MonoUsbEventHandler.SessionHandle);
            }
        }

        /// <summary>
        /// Initializes the <see cref="MonoUsbEventHandler.SessionHandle"/> with <see cref="MonoUsbEventHandler.Init()"/> and starts the static handle events thread with <see cref="MonoUsbEventHandler.Start"/>. 
        /// </summary>
        /// <remarks>
        /// This is done automatically when needed.
        /// <para>Usually there is no need to call this functions externally.</para> 
        /// </remarks>
        public static void Init() { MonoUsbApi.InitAndStart(); }
    }
}