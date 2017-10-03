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
using System.IO;
using System.Text.RegularExpressions;
using LibUsbDotNet.Descriptors;

namespace LibUsbDotNet.DeviceNotify.Linux
{
    public partial class LinuxDeviceNotifier
    {
        private static Regex _RegParseDeviceInterface;
        private static string DeviceIntefaceMatchExpression = "usbdev(?<BusNumber>[0-9]+)\\.(?<DeviceAddress>[0-9]+)$";
        private readonly string mDevDir;
        private FileSystemWatcher mUsbFS;

        private static Regex RegParseDeviceInterface
        {
            get
            {
                if (ReferenceEquals(_RegParseDeviceInterface, null))
                {
                    _RegParseDeviceInterface = new Regex(DeviceIntefaceMatchExpression,
                                                         RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant |
                                                         RegexOptions.Singleline);
                }
                return _RegParseDeviceInterface;
            }
        }


        private static bool IsDeviceEnterface(string name, out byte busNumber, out byte deviceAddress)
        {
            busNumber = 0;
            deviceAddress = 0;

            Match match = RegParseDeviceInterface.Match(name);
            if (match.Success)
            {
                try
                {
                    busNumber = byte.Parse(match.Groups["BusNumber"].Value);
                    deviceAddress = byte.Parse(match.Groups["DeviceAddress"].Value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private static bool ReadFileDescriptor(string fullPath, out byte[] deviceDescriptorBytes)
        {
            try
            {
                FileStream f = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                deviceDescriptorBytes = new byte[UsbDeviceDescriptor.Size];
                int iRead = f.Read(deviceDescriptorBytes, 0, UsbDeviceDescriptor.Size);
                f.Close();

                return iRead == UsbDeviceDescriptor.Size;
            }
            catch
            {
                deviceDescriptorBytes = null;
                return false;
            }
        }

        private void StartDevDirectoryMonitor()
        {
            if (!ReferenceEquals(mUsbFS, null)) return;

            BuildDevList();
            mUsbFS = new FileSystemWatcher(mDevDir);
            mUsbFS.IncludeSubdirectories = false;
            mUsbFS.Created += FileAdded;
            mUsbFS.Deleted += FileRemoved;
            mUsbFS.EnableRaisingEvents = true;
        }

        private void BuildDevList()
        {
            mLinuxDevItemList.Clear();
            string[] deviceInterfaceFiles = Directory.GetFiles(mDevDir, "usbdev*", SearchOption.TopDirectoryOnly);
            foreach (string deviceInterfaceFile in deviceInterfaceFiles)
            {
                byte busNumber;
                byte deviceAddress;
                string deviceFileName = Path.GetFileName(deviceInterfaceFile);
                if (IsDeviceEnterface(deviceFileName, out busNumber, out deviceAddress))
                {
                    byte[] descriptorBytes;
                    if (ReadFileDescriptor(deviceInterfaceFile, out descriptorBytes))
                    {
                        LinuxDevItem addedItem = new LinuxDevItem(deviceFileName, busNumber, deviceAddress, descriptorBytes);
                        mLinuxDevItemList.Add(addedItem);
                    }
                }
            }
            //Console.WriteLine("LinuxDeviceNotifier:BuildDevList Count:{0}",mLinuxDevItemList.Count);
        }

        private void FileRemoved(object sender, FileSystemEventArgs e)
        {
            byte busNumber;
            byte deviceAddress;
            if (IsDeviceEnterface(e.Name, out busNumber, out deviceAddress))
            {
                LinuxDevItem foundLinuxDevItem;
                if ((foundLinuxDevItem = mLinuxDevItemList.FindByName(e.Name)) == null) throw new Exception("FileRemoved:Invalid LinuxDevItem");

                //Console.WriteLine("Removed Vid:{0:X4} Pid:{1:X4}", foundLinuxDevItem.DeviceDescriptor.VendorID, foundLinuxDevItem.DeviceDescriptor.ProductID);
                //////////////////////
                // TODO:DEVICE REMOVAL
                //////////////////////

                mLinuxDevItemList.Remove(foundLinuxDevItem);
                EventHandler<DeviceNotifyEventArgs> deviceNotify = OnDeviceNotify;
                if (!ReferenceEquals(deviceNotify, null))
                {
                    deviceNotify(this, new LinuxDeviceNotifyEventArgs(foundLinuxDevItem, DeviceType.DeviceInterface, EventType.DeviceRemoveComplete));
                }
            }
        }

        private void FileAdded(object sender, FileSystemEventArgs e)
        {
            byte busNumber;
            byte deviceAddress;
            if (IsDeviceEnterface(e.Name, out busNumber, out deviceAddress))
            {
                byte[] descriptorBytes;
                if (ReadFileDescriptor(e.FullPath, out descriptorBytes))
                {
                    LinuxDevItem addedItem = new LinuxDevItem(e.Name, busNumber, deviceAddress, descriptorBytes);
                    if (mLinuxDevItemList.FindByName(e.Name) != null) throw new Exception("FileAdded:Invalid LinuxDevItem");
                    mLinuxDevItemList.Add(addedItem);

                    //Console.WriteLine("Added Vid:{0:X4} Pid:{1:X4}", addedItem.DeviceDescriptor.VendorID, addedItem.DeviceDescriptor.ProductID);
                    //////////////////////
                    // TODO:DEVICE ARRIVAL
                    //////////////////////

                    EventHandler<DeviceNotifyEventArgs> deviceNotify = OnDeviceNotify;
                    if (!ReferenceEquals(deviceNotify, null))
                    {
                        deviceNotify(this, new LinuxDeviceNotifyEventArgs(addedItem, DeviceType.DeviceInterface, EventType.DeviceArrival));
                    }
                }
            }
        }

        private void StopDevDirectoryMonitor()
        {
            if (!ReferenceEquals(mUsbFS, null))
            {
                mUsbFS.EnableRaisingEvents = false;
                mUsbFS.Created -= FileAdded;
                mUsbFS.Deleted -= FileRemoved;
                mUsbFS.Dispose();
                mUsbFS = null;
            }
        }
    }
}