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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// Finds and identifies usb devices. Used for easily locating  
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// Instances of this class can optionally be passed directly into 
    /// <see cref="UsbDevice.OpenUsbDevice(LibUsbDotNet.Main.UsbDeviceFinder)"/> 
    /// to quickly find and open a specific usb device in one step.
    /// </item>
    /// <item>
    /// Pass instances of this class into the 
    /// <see cref="UsbRegDeviceList.Find(UsbDeviceFinder)"/>, 
    /// <see cref="UsbRegDeviceList.FindAll(UsbDeviceFinder)"/>,  
    /// or <see cref="UsbRegDeviceList.FindLast(UsbDeviceFinder)"/> 
    /// functions of a  <see cref="UsbRegDeviceList"/> 
    /// instance to find connected usb devices without opening devices or interrogating the bus.
    /// After locating the required <see cref="UsbRegistry"/> instance, call the 
    /// <see cref="UsbRegistry.Open"/> method to start using the <see cref="UsbDevice"/> instance.
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code source="..\Examples\Show.Info\ShowInfo.cs" lang="cs"/>
    /// </example>
    public class UsbDeviceFinder : ISerializable
    {
        ///<summary> The "exclude from search" value for <see cref="Pid"/>. </summary>
        public const int NO_PID = int.MaxValue;

        ///<summary> The "exclude from search" value for <see cref="Revision"/>. </summary>
        public const int NO_REV = int.MaxValue;

        ///<summary> The "exclude from search" value for <see cref="SerialNumber"/>. </summary>
        public const string NO_SERIAL = null;

        ///<summary> The "exclude from search" value for <see cref="Vid"/>. </summary>
        public const int NO_VID = int.MaxValue;

        ///<summary>  The "exclude from search" value for <see cref="DeviceInterfaceGuid"/>. </summary>
        public static readonly Guid NO_GUID = Guid.Empty;

        private Guid mDeviceInterfaceGuid = Guid.Empty;
        private int mPid = int.MaxValue;
        private int mRevision = int.MaxValue;
        private string mSerialNumber;
        private int mVid = int.MaxValue;

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating and identifying usb devices.
        /// </summary>
        /// <param name="vid">The vendor id of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
        /// <param name="pid">The product id of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
        /// <param name="revision">The revision number of the usb device to find, or <see cref="int.MaxValue"/> to ignore.</param>
        /// <param name="serialNumber">The serial number of the usb device to find, or null to ignore.</param>
        /// <param name="deviceInterfaceGuid">The unique guid of the usb device to find, or <see cref="Guid.Empty"/> to ignore.</param>
        public UsbDeviceFinder(int vid, int pid, int revision, string serialNumber, Guid deviceInterfaceGuid)
        {
            mVid = vid;
            mPid = pid;
            mRevision = revision;
            mSerialNumber = serialNumber;
            mDeviceInterfaceGuid = deviceInterfaceGuid;
        }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices by VendorID, ProductID, and Serial number.
        /// </summary>
        /// <param name="vid">The vendor id of the usb device to find.</param>
        /// <param name="pid">The product id of the usb device to find.</param>
        /// <param name="serialNumber">The serial number of the usb device to find.</param>
        public UsbDeviceFinder(int vid, int pid, string serialNumber)
            : this(vid, pid, int.MaxValue, serialNumber, Guid.Empty) { }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices by VendorID, ProuctID, and Revision code.
        /// </summary>
        /// <param name="vid">The vendor id of the usb device to find.</param>
        /// <param name="pid">The product id of the usb device to find.</param>
        /// <param name="revision">The revision number of the usb device to find.</param>
        public UsbDeviceFinder(int vid, int pid, int revision)
            : this(vid, pid, revision, null, Guid.Empty) { }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices vendor and product ID.
        /// </summary>
        /// <param name="vid">The vendor id of the usb device to find.</param>
        /// <param name="pid">The product id of the usb device to find.</param>
        public UsbDeviceFinder(int vid, int pid)
            : this(vid, pid, int.MaxValue, null, Guid.Empty) { }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices.
        /// </summary>
        /// <param name="vid">The vendor id of the usb device to find.</param>
        public UsbDeviceFinder(int vid)
            : this(vid, int.MaxValue, int.MaxValue, null, Guid.Empty) { }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices by a serial number.
        /// </summary>
        /// <param name="serialNumber">The serial number of the usb device to find.</param>
        public UsbDeviceFinder(string serialNumber)
            : this(int.MaxValue, int.MaxValue, int.MaxValue, serialNumber, Guid.Empty) { }

        /// <summary>
        /// Creates a UsbDeviceFinder class for locating usb devices by a unique <see cref="Guid"/> string.
        /// </summary>
        /// <param name="deviceInterfaceGuid">The unique <see cref="Guid"/> to find.</param>
        public UsbDeviceFinder(Guid deviceInterfaceGuid)
            : this(int.MaxValue, int.MaxValue, int.MaxValue, null, deviceInterfaceGuid) { }

        /// <summary>
        /// Use a serialization stream to fill the <see cref="UsbDeviceFinder"/> class. 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UsbDeviceFinder(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            mVid = (int) info.GetValue("Vid", typeof (int));
            mPid = (int) info.GetValue("Pid", typeof (int));
            mRevision = (int) info.GetValue("Revision", typeof (int));
            mSerialNumber = (string) info.GetValue("SerialNumber", typeof (string));
            mDeviceInterfaceGuid = (Guid) info.GetValue("DeviceInterfaceGuid", typeof (Guid));
        }

        /// <summary>
        /// 
        /// </summary>
        protected UsbDeviceFinder() { }

        /// <summary>
        /// The device interface guid string to find, or <see cref="String.Empty"/> to ignore.
        /// </summary>
        public Guid DeviceInterfaceGuid
        {
            get { return mDeviceInterfaceGuid; }
            set { mDeviceInterfaceGuid = value; }
        }

        /// <summary>
        /// The serial number of the device to find.
        /// </summary>
        /// <remarks>
        /// Set to null to ignore.
        /// </remarks>
        public string SerialNumber
        {
            get { return mSerialNumber; }
            set { mSerialNumber = value; }
        }

        /// <summary>
        /// The revision number of the device to find.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="int.MaxValue"/> to ignore.
        /// </remarks>
        public int Revision
        {
            get { return mRevision; }
            set { mRevision = value; }
        }

        /// <summary>
        /// The product id of the device to find.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="int.MaxValue"/> to ignore.
        /// </remarks>
        public int Pid
        {
            get { return mPid; }
            set { mPid = value; }
        }

        /// <summary>
        /// The vendor id of the device to find.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="int.MaxValue"/> to ignore.
        /// </remarks>
        public int Vid
        {
            get { return mVid; }
            set { mVid = value; }
        }

        #region ISerializable Members

        /// <summary>
        /// Store this class as a binary serializtion object.
        /// </summary>
        /// <param name="info">The serialization instance to populate.</param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Vid", mVid);
            info.AddValue("Pid", mPid);
            info.AddValue("Revision", mRevision);
            info.AddValue("SerialNumber", mSerialNumber);
            info.AddValue("DeviceInterfaceGuid", mDeviceInterfaceGuid);
        }

        #endregion

        /// <summary>
        /// Load usb device finder properties from a binary stream.
        /// </summary>
        /// <param name="deviceFinderStream">The binary stream containing a
        /// <see cref="UsbDeviceFinder"/> </param> instance.
        /// <returns>A pre-loaded <see cref="UsbDeviceFinder"/> instance.</returns>
        public static UsbDeviceFinder Load(Stream deviceFinderStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(deviceFinderStream) as UsbDeviceFinder;
        }

        /// <summary>
        /// Saves a <see cref="UsbDeviceFinder"/> instance to a stream.
        /// </summary>
        /// <param name="usbDeviceFinder"></param>
        /// <param name="outStream"></param>
        public static void Save(UsbDeviceFinder usbDeviceFinder, Stream outStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(outStream, usbDeviceFinder);
        }

        /// <summary>
        /// Dynamic predicate find function. Pass this function into any method that has a <see cref="Predicate{UsbRegistry}"/> parameter.
        /// </summary>
        /// <remarks>
        /// Override this member when inheriting the <see cref="UsbDeviceFinder"/> class to change/alter the matching behavior.
        /// </remarks>
        /// <param name="usbRegistry">The UsbRegistry device to check.</param>
        /// <returns>True if the <see cref="UsbRegistry"/> instance matches the <see cref="UsbDeviceFinder"/> properties.</returns>
        public virtual bool Check(UsbRegistry usbRegistry)
        {
            if (mVid != int.MaxValue)
                if (usbRegistry.Vid != mVid) return false;
            if (mPid != int.MaxValue)
                if (usbRegistry.Pid != mPid) return false;
            if (mRevision != int.MaxValue)
                if (usbRegistry.Rev != mRevision) return false;

            if (!String.IsNullOrEmpty(mSerialNumber))
            {
                if (String.IsNullOrEmpty(usbRegistry.SymbolicName)) return false;

                UsbSymbolicName usbSymbolicName = UsbSymbolicName.Parse(usbRegistry.SymbolicName);
                if (mSerialNumber != usbSymbolicName.SerialNumber) return false;
            }
            if (mDeviceInterfaceGuid != Guid.Empty)
            {
                List<Guid> deviceGuids = new List<Guid>(usbRegistry.DeviceInterfaceGuids);
                if (!deviceGuids.Contains(mDeviceInterfaceGuid)) return false;
            }
            return true;
        }
        /// <summary>
        /// Dynamic predicate find function. Pass this function into any method that has a <see cref="Predicate{UsbDevice}"/> parameter.
        /// </summary>
        /// <remarks>
        /// Override this member when inheriting the <see cref="UsbDeviceFinder"/> class to change/alter the matching behavior.
        /// </remarks>
        /// <param name="usbDevice">The UsbDevice to check.</param>
        /// <returns>True if the <see cref="UsbDevice"/> instance matches the <see cref="UsbDeviceFinder"/> properties.</returns>
        public virtual bool Check(UsbDevice usbDevice)
        {
            if (mVid != int.MaxValue)
                if (((ushort)usbDevice.Info.Descriptor.VendorID) != mVid) return false;
            if (mPid != int.MaxValue)
                if (((ushort)usbDevice.Info.Descriptor.ProductID) != mPid) return false;
            if (mRevision != int.MaxValue)
                if (((ushort)usbDevice.Info.Descriptor.BcdDevice) != mRevision) return false;

            if (!String.IsNullOrEmpty(mSerialNumber))
                if (mSerialNumber!=usbDevice.Info.SerialString) return false;

            return true;
        }
    }
}