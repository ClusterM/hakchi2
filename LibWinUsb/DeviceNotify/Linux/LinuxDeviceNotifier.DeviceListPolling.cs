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
using System.Timers;
using LibUsbDotNet.LudnMonoLibUsb;
using MonoLibUsb.Profile;

namespace LibUsbDotNet.DeviceNotify.Linux
{
    public partial class LinuxDeviceNotifier
    {
        ///<summary>
        /// The interval (milliseconds) in which the device list is queried for changes when using the <see cref="LinuxDeviceNotifierMode.PollDeviceList"/> mode.
        ///</summary>
        public static int PollingInterval = 750;

        private Timer mDeviceListPollTimer;
        private object PollTimerLock = new object();

        private void StartDeviceListPolling()
        {
            lock (PollTimerLock)
            {
                if (mDeviceListPollTimer != null) return;

                MonoUsbDevice.RefreshProfileList();

                MonoUsbDevice.ProfileList.AddRemoveEvent += OnAddRemoveEvent;
                mDeviceListPollTimer = new Timer(PollingInterval);
                mDeviceListPollTimer.Elapsed += PollTimer_Elapsed;
                mDeviceListPollTimer.Start();
            }
        }

        private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (PollTimerLock)
            {
                mDeviceListPollTimer.Stop();
                MonoUsbDevice.RefreshProfileList();
                mDeviceListPollTimer.Start();
            }
        }

        private void StopDeviceListPolling()
        {
            lock (PollTimerLock)
            {
                if (mDeviceListPollTimer == null) return;
                mDeviceListPollTimer.Stop();
                mDeviceListPollTimer.Elapsed -= PollTimer_Elapsed;
                mDeviceListPollTimer.Dispose();
                MonoUsbDevice.ProfileList.AddRemoveEvent -= OnAddRemoveEvent;
                mDeviceListPollTimer = null;
            }
        }


        private void OnAddRemoveEvent(object sender, AddRemoveEventArgs e)
        {
            EventHandler<DeviceNotifyEventArgs> deviceNotify = OnDeviceNotify;
            if (!ReferenceEquals(deviceNotify, null))
            {
                string deviceFileName = String.Format("usbdev{0}.{1}", e.MonoUSBProfile.BusNumber, e.MonoUSBProfile.DeviceAddress);

                LinuxDevItem linuxDevItem = new LinuxDevItem(deviceFileName,
                                                             e.MonoUSBProfile.BusNumber,
                                                             e.MonoUSBProfile.DeviceAddress,
                                                             e.MonoUSBProfile.DeviceDescriptor);

                deviceNotify(this,
                             new LinuxDeviceNotifyEventArgs(linuxDevItem,
                                                            DeviceType.DeviceInterface,
                                                            e.EventType == AddRemoveType.Added
                                                                ? EventType.DeviceArrival
                                                                : EventType.DeviceRemoveComplete));
            }
        }
    }
}