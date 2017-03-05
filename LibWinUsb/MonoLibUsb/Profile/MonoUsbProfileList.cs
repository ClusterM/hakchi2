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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// Manages the device list.  This class is thread safe.
    /// </summary>
    public class MonoUsbProfileList : IEnumerable<MonoUsbProfile>
    {
        private object LockProfileList = new object();

        private static bool FindDiscoveredFn(MonoUsbProfile check) { return check.mDiscovered; }
        private static bool FindUnDiscoveredFn(MonoUsbProfile check) { return !check.mDiscovered; }

        private List<MonoUsbProfile> mList=new List<MonoUsbProfile>();
        private void FireAddRemove(MonoUsbProfile monoUSBProfile, AddRemoveType addRemoveType)
        {
            EventHandler<AddRemoveEventArgs> temp = AddRemoveEvent;
            if (!ReferenceEquals(temp, null))
            {
                temp(this, new AddRemoveEventArgs(monoUSBProfile, addRemoveType));
            }
        }

        private void SetDiscovered(bool discovered)
        {
            foreach (MonoUsbProfile deviceProfile in this)
            {
                deviceProfile.mDiscovered = discovered;
            }
        }

        private void syncWith(MonoUsbProfileList newList)
        {
            SetDiscovered(false);
            newList.SetDiscovered(true);

            int iNewProfiles = newList.mList.Count;
            for (int iNewProfile = 0; iNewProfile < iNewProfiles; iNewProfile++)
            {
                MonoUsbProfile newProfile = newList.mList[iNewProfile];
                int iFoundOldIndex;
                if ((iFoundOldIndex = mList.IndexOf(newProfile)) == -1)
                {
                    //Console.WriteLine("DeviceDiscovery: Added: {0}", newProfile.ProfileHandle.DangerousGetHandle());
                    newProfile.mDiscovered = true;
                    mList.Add(newProfile);
                    FireAddRemove(newProfile, AddRemoveType.Added);
                }
                else
                {
                    //Console.WriteLine("DeviceDiscovery: Unchanged: Orig:{0} New:{1}", mList[iFoundOldIndex].ProfileHandle.DangerousGetHandle(), newProfile.ProfileHandle.DangerousGetHandle());
                    mList[iFoundOldIndex].mDiscovered = true;
                    newProfile.mDiscovered = false;
                   
                }
            }

            newList.mList.RemoveAll(FindDiscoveredFn);
            newList.Close();

            foreach (MonoUsbProfile deviceProfile in mList)
            {
                if (!deviceProfile.mDiscovered)
                {
                    // Close Unplugged device profiles.
                    //Console.WriteLine("DeviceDiscovery: Removed: {0}", deviceProfile.ProfileHandle.DangerousGetHandle());
                    FireAddRemove(deviceProfile, AddRemoveType.Removed);
                    deviceProfile.Close();
                }
            }

            // Remove Unplugged device profiles.
            mList.RemoveAll(FindUnDiscoveredFn);
        }


        /// <summary>
        /// Refreshes the <see cref="MonoUsbProfile"/> list.
        /// </summary>
        /// <remarks>
        /// <para>This is your entry point into finding a USB device to operate.</para>
        /// <para>This return value of this function indicates the number of devices in the resultant list.</para>
        /// <para>The <see cref="MonoUsbProfileList"/> has a crude form of built-in device notification that works on all platforms. By adding an event handler to the <see cref="AddRemoveEvent"/> changes in the device profile list are reported when <see cref="Refresh"/> is called.</para>
        /// </remarks>
        /// <param name="sessionHandle">A valid <see cref="MonoUsbSessionHandle"/>.</param>
        /// <returns>The number of devices in the outputted list, or <see cref="MonoUsbError.ErrorNoMem"/> on memory allocation failure.</returns>
        /// <example>
        /// <code source="..\MonoLibUsb\MonoUsb.ShowInfo\ShowInfo.cs" lang="cs"/>
        /// </example>
        public int Refresh(MonoUsbSessionHandle sessionHandle)
        {
            lock (LockProfileList)
            {
                MonoUsbProfileList newList = new MonoUsbProfileList();
                MonoUsbProfileListHandle monoUSBProfileListHandle;

                int ret = MonoUsbApi.GetDeviceList(sessionHandle, out monoUSBProfileListHandle);
                if (ret < 0 || monoUSBProfileListHandle.IsInvalid)
                {
#if LIBUSBDOTNET
                    UsbError.Error(ErrorCode.MonoApiError, ret, "Refresh:GetDeviceList Failed", this);
#else
                    System.Diagnostics.Debug.Print("libusb_get_device_list failed:{0} {1}",
                                                   (MonoUsbError) ret,
                                                   MonoUsbApi.StrError((MonoUsbError) ret));
#endif
                    return ret;
                }
                int stopCount = ret;
                foreach (MonoUsbProfileHandle deviceProfileHandle in monoUSBProfileListHandle)
                {
                    newList.mList.Add(new MonoUsbProfile(deviceProfileHandle));
                    stopCount--;
                    if (stopCount <= 0) break;
                }
                syncWith(newList);
                monoUSBProfileListHandle.Close();

                return ret;
            }
        }

        /// <summary>
        /// Frees all unreferenced profiles contained in the list.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="MonoUsbProfileHandle"/>s that are in-use are never closed until all reference(s) have gone 
        /// out-of-scope or specifically been closed with the <see cref="SafeHandle.Close"/> method.
        /// </para>
        /// </remarks>
        public void Close()
        {
            lock (LockProfileList)
            {
                foreach (MonoUsbProfile profile in mList)
                    profile.Close();

                mList.Clear();
            }
        }

        /// <summary>
        /// Usb device arrival/removal notification handler. 
        /// This event only reports when the <see cref="Refresh"/> method is called.
        /// </summary>
        /// <remarks>
        /// <see cref="AddRemoveEvent"/> could be used for a crude form for receiving usb 
        /// device arrival/removal notification.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Startup code
        /// MonoUsbProfileList profileList = new MonoUsbProfileList();
        /// profileList.AddRemoveEvent += OnDeviceAddRemove;
        /// 
        /// // Device AddRemove event template
        /// private void OnDeviceAddRemove(object sender, AddRemoveEventArgs addRemoveArgs)
        /// {
        /// // This method will only report when Refresh() is called.
        /// }
        /// 
        /// // Refresh profile list.
        /// // Any devices added or removed since the last call to Refresh() will be returned
        /// // in the OnDeviceAddRemove method.
        /// profileList.Refresh();
        /// </code>
        /// </example>
        public event EventHandler<AddRemoveEventArgs> AddRemoveEvent;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<MonoUsbProfile> GetEnumerator() 
        { 
            lock(LockProfileList)
                return mList.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>
        /// Returns the number of <see cref="MonoUsbProfile"/> instances in the list.
        /// </summary>
        public int Count
        {
            get
            {
                lock (LockProfileList)
                    return mList.Count;
            }
        }
        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="MonoUsbProfile"/> instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="MonoUsbProfileList"/> uses an internal list that is locked when changes must be made.
        /// The <see cref="GetList"/> method returns a copy of this list that can be searched and modified as needed by the user.
        /// </para>
        /// <para>
        /// The returned generic <see cref="List{T}"/> contains many more functions for finding devices.  
        /// It may be desirable to use these members, such as <see cref="List{T}.FindAll"/> or <see cref="List{T}.ForEach"/> to find a <see cref="MonoUsbProfile"/> instead of iterating through the <see cref="MonoUsbProfileList"/> one-by-one.
        /// </para>
        /// </remarks>
        /// <returns>A <see cref="List{T}"/> of <see cref="MonoUsbProfile"/> instances.</returns>
        public List<MonoUsbProfile> GetList()
        {
            lock (LockProfileList)
                return new List<MonoUsbProfile>(mList);
        }

        /// <summary>
        /// Gets the <see cref="MonoUsbProfile"/> at the specfied index.
        /// </summary>
        /// <param name="index">The index of the <see cref="MonoUsbProfile"/> to retrieve.</param>
        /// <returns>The <see cref="MonoUsbProfile"/> instance at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public MonoUsbProfile this[int index]
        {
            get
            {
                lock (LockProfileList)
                    return mList[index];
            }
        }
    }
}