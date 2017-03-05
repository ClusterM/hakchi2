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

namespace LibUsbDotNet.DeviceNotify.Linux
{
    ///<summary>
    /// Creates an instance of the <see cref="LinuxDeviceNotifier"/> class.
    /// See the <see cref="IDeviceNotifier"/> interface or <see cref="DeviceNotifier.OpenDeviceNotifier"/> method for more information
    ///</summary>
    ///<remarks>
    ///To make your code platform-independent use the <see cref="DeviceNotifier.OpenDeviceNotifier"/> method for creating instances.
    ///</remarks>
    public partial class LinuxDeviceNotifier : IDeviceNotifier
    {
        private readonly LinuxDevItemList mLinuxDevItemList = new LinuxDevItemList();
        private readonly LinuxDeviceNotifierMode mMode = LinuxDeviceNotifierMode.None;

        /// <summary>
        /// Creates a new instance of the LinuxDeviceNotifier using 'devDir' as the root device path. (IE. '/dev').
        /// </summary>
        /// <param name="devDir">The directory to monitor; usually '/dev'.</param>
        public LinuxDeviceNotifier(string devDir)
        {
            mDevDir = devDir;
            try
            {
                StartDevDirectoryMonitor();
                if (mLinuxDevItemList.Count == 0) throw new NotSupportedException("LinuxDeviceNotifier:Dev directory monitor not supported.");
                mMode = LinuxDeviceNotifierMode.MonitorDevDirectory;
                return;
            }
            catch 
            {
                StopDevDirectoryMonitor();
            }
            mMode = LinuxDeviceNotifierMode.PollDeviceList;
            StartDeviceListPolling();
        }

        /// <summary>
        /// Creates a new instance of the LinuxDeviceNotifier using '/dev' as the root device path.
        /// </summary>
        public LinuxDeviceNotifier()
            : this("/dev") { }

        ///<summary>
        /// Gets the mode being used to detect notification events.
        ///</summary>
        public LinuxDeviceNotifierMode Mode
        {
            get { return mMode; }
        }

        #region IDeviceNotifier Members

        ///<summary>
        /// Enables/Disables notification events.
        ///</summary>
        public bool Enabled
        {
            get
            {
                switch (mMode)
                {
                    case LinuxDeviceNotifierMode.PollDeviceList:
                        return mDeviceListPollTimer != null;
                    case LinuxDeviceNotifierMode.MonitorDevDirectory:
                        return mUsbFS != null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        /// <summary>
        /// Main Notify event for all device notifications.
        /// </summary>
        public event EventHandler<DeviceNotifyEventArgs> OnDeviceNotify;

        #endregion

        private void Stop()
        {
            switch (mMode)
            {
                case LinuxDeviceNotifierMode.PollDeviceList:
                    StopDeviceListPolling();
                    break;
                case LinuxDeviceNotifierMode.MonitorDevDirectory:
                    StopDevDirectoryMonitor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Start()
        {
            switch (mMode)
            {
                case LinuxDeviceNotifierMode.PollDeviceList:
                    StartDeviceListPolling();
                    break;
                case LinuxDeviceNotifierMode.MonitorDevDirectory:
                    StartDevDirectoryMonitor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}