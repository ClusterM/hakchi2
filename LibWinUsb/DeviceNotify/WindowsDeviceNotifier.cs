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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LibUsbDotNet.DeviceNotify.Internal;

namespace LibUsbDotNet.DeviceNotify
{
    /// <summary>
    /// Notifies an application of a change to the hardware Configuration of a device or 
    /// the computer. See <see cref="IDeviceNotifier"/> or <see cref="DeviceNotifier.OpenDeviceNotifier"/> interface for more information
    /// </summary>
    /// <remarks>
    /// This is the windows implementation of the device notifier.
    /// </remarks>
    public class WindowsDeviceNotifier : IDeviceNotifier
    {
        private readonly DevBroadcastDeviceinterface mDevInterface = new DevBroadcastDeviceinterface(new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"));

        private SafeNotifyHandle mDevInterfaceHandle;
        private bool mEnabled = true;
        private DevNotifyNativeWindow mNotifyWindow;

        ///<summary>
        /// Creates an instance of the <see cref="WindowsDeviceNotifier"/> class.
        /// See the <see cref="IDeviceNotifier"/> interface or <see cref="DeviceNotifier.OpenDeviceNotifier"/> method for more information
        ///</summary>
        ///<remarks>
        ///To make your code platform-independent use the <see cref="DeviceNotifier.OpenDeviceNotifier"/> method for creating instances.
        ///</remarks>
        public WindowsDeviceNotifier() { mNotifyWindow = new DevNotifyNativeWindow(OnHandleChange, OnDeviceChange); }

        #region IDeviceNotifier Members

        ///<summary>
        /// Enables/Disables notification events.
        ///</summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }


        /// <summary>
        /// Main Notify event for all device notifications.
        /// </summary>
        public event EventHandler<DeviceNotifyEventArgs> OnDeviceNotify;

        #endregion

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterDeviceNotificationA", CharSet = CharSet.Ansi)]
        private static extern SafeNotifyHandle RegisterDeviceNotification(IntPtr hRecipient,
                                                                          [MarshalAs(UnmanagedType.AsAny), In] object notificationFilter,
                                                                          int flags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);

        ///<summary>
        ///Releases the resources associated with this window. 
        ///</summary>
        ///
        ~WindowsDeviceNotifier()
        {
            if (mNotifyWindow != null) mNotifyWindow.DestroyHandle();
            mNotifyWindow = null;

            if (mDevInterfaceHandle != null) mDevInterfaceHandle.Dispose();
            mDevInterfaceHandle = null;
        }

        internal bool RegisterDeviceInterface(IntPtr windowHandle)
        {
            if (mDevInterfaceHandle != null)
            {
                mDevInterfaceHandle.Dispose();
                mDevInterfaceHandle = null;
            }
            if (windowHandle != IntPtr.Zero)
            {
                mDevInterfaceHandle = RegisterDeviceNotification(windowHandle, mDevInterface, 0);
                if (mDevInterfaceHandle != null && !mDevInterfaceHandle.IsInvalid)
                    return true;
                return false;
            }
            return false;
        }


        private void OnDeviceChange(ref Message m)
        {
            if (!mEnabled) return;
            if (m.LParam.ToInt32() != 0)
            {
                EventHandler<DeviceNotifyEventArgs> temp = OnDeviceNotify;
                if (!ReferenceEquals(temp, null))
                {
                    DeviceNotifyEventArgs args;
                    DevBroadcastHdr hdr = new DevBroadcastHdr();
                    Marshal.PtrToStructure(m.LParam, hdr);
                    switch (hdr.DeviceType)
                    {
                        case DeviceType.Port:
                        case DeviceType.Volume:
                        case DeviceType.DeviceInterface:
                            args = new WindowsDeviceNotifyEventArgs(hdr, m.LParam, (EventType) m.WParam.ToInt32());
                            break;
                        default:
                            args = null;
                            break;
                    }

                    if (!ReferenceEquals(args, null)) temp(this, args);
                }
            }
        }

        private void OnHandleChange(IntPtr newWindowHandle)
        {
            bool bSuccess = RegisterDeviceInterface(newWindowHandle);
            Debug.Print("RegisterDeviceInterface:" + bSuccess);
        }
    }
}