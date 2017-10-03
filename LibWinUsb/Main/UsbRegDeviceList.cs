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

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// Array of USB device available for communication via LibUsb or WinUsb.
    /// </summary>
    public class UsbRegDeviceList : IEnumerable<UsbRegistry>
    {
        private readonly List<UsbRegistry> mUsbRegistryList;

        ///<summary>
        /// Creates an empty <see cref="UsbRegDeviceList"/> instance.
        ///</summary>
        public UsbRegDeviceList() { mUsbRegistryList = new List<UsbRegistry>(); }

        private UsbRegDeviceList(IEnumerable<UsbRegistry> usbRegDeviceList) { mUsbRegistryList = new List<UsbRegistry>(usbRegDeviceList); }

        ///<summary>
        ///Gets the element at the specified index.
        ///</summary>
        ///
        ///<returns>
        ///The element at the specified index.
        ///</returns>
        ///
        ///<param name="index">The zero-based index of the element to get or set.</param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public UsbRegistry this[int index]
        {
            get { return mUsbRegistryList[index]; }
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///</summary>
        ///
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///</returns>
        ///
        public int Count
        {
            get { return mUsbRegistryList.Count; }
        }

        #region IEnumerable<UsbRegistry> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        ///</returns>
        IEnumerator<UsbRegistry> IEnumerable<UsbRegistry>.GetEnumerator() { return mUsbRegistryList.GetEnumerator(); }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        ///</returns>
        public IEnumerator GetEnumerator() { return ((IEnumerable<UsbRegistry>) this).GetEnumerator(); }

        #endregion

        /// <summary>
        /// Find the first UsbRegistry device that matches the FindUsbPredicate.  
        /// </summary>
        /// <param name="findUsbPredicate">The predicate function to use.</param>
        /// <returns>A valid usb registry class if the device was found or Null if the device was not found.</returns>
        public UsbRegistry Find(Predicate<UsbRegistry> findUsbPredicate) { return mUsbRegistryList.Find(findUsbPredicate); }

        /// <summary>
        /// Find all UsbRegistry devices that matches the FindUsbPredicate.  
        /// </summary>
        /// <param name="findUsbPredicate">The predicate function to use.</param>
        /// <returns>All usb registry classes that match.</returns>
        public UsbRegDeviceList FindAll(Predicate<UsbRegistry> findUsbPredicate) { return new UsbRegDeviceList(mUsbRegistryList.FindAll(findUsbPredicate)); }

        /// <summary>
        /// Find the last a UsbRegistry device that matches the FindUsbPredicate.  
        /// </summary>
        /// <param name="findUsbPredicate">The predicate function to use.</param>
        /// <returns>A valid usb registry class if the device was found or Null if the device was not found.</returns>
        public UsbRegistry FindLast(Predicate<UsbRegistry> findUsbPredicate) { return mUsbRegistryList.FindLast(findUsbPredicate); }

        /// <summary>
        /// Find the first UsbRegistry device using a <see cref="UsbDeviceFinder"/> instance.  
        /// </summary>
        /// <param name="usbDeviceFinder">The <see cref="UsbDeviceFinder"/> instance used to locate the usb registry devices.</param>
        /// <returns>A valid usb registry class if the device was found or Null if the device was not found.</returns>
        public UsbRegistry Find(UsbDeviceFinder usbDeviceFinder) { return mUsbRegistryList.Find((Predicate<UsbRegistry>) usbDeviceFinder.Check); }

        /// <summary>
        /// Find all UsbRegistry devices using a <see cref="UsbDeviceFinder"/> instance.  
        /// </summary>
        /// <param name="usbDeviceFinder">The <see cref="UsbDeviceFinder"/> instance used to locate the usb registry devices.</param>
        /// <returns>All usb registry classes that match.</returns>
        public UsbRegDeviceList FindAll(UsbDeviceFinder usbDeviceFinder) { return FindAll((Predicate<UsbRegistry>)usbDeviceFinder.Check); }

        /// <summary>
        /// Find the last UsbRegistry devices using a <see cref="UsbDeviceFinder"/> instance.  
        /// </summary>
        /// <param name="usbDeviceFinder">The <see cref="UsbDeviceFinder"/> instance used to locate the usb registry devices.</param>
        /// <returns>A valid usb registry class if the device was found or Null if the device was not found.</returns>
        public UsbRegistry FindLast(UsbDeviceFinder usbDeviceFinder) { return mUsbRegistryList.FindLast((Predicate<UsbRegistry>)usbDeviceFinder.Check); }

        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(UsbRegistry item) { return mUsbRegistryList.Contains(item); }

        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        ///<param name="offset">The zero-based index in Array at which copying begins.</param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">Offset is less than 0.</exception>
        ///<exception cref="T:System.ArgumentNullException">Array is null.</exception>
        ///<exception cref="T:System.ArgumentException">Array is multidimensional.-or-Offset is equal to or greater than the length of Array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from Offset to the end of the destination Array.-or-Type T cannot be cast automatically to the type of the destination Array.</exception>
        public void CopyTo(UsbRegistry[] array, int offset) { mUsbRegistryList.CopyTo(array, offset); }

        ///<summary>
        ///Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///</summary>
        ///
        ///<returns>
        ///The index of item if found in the list; otherwise, -1.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(UsbRegistry item) { return mUsbRegistryList.IndexOf(item); }

        internal bool Add(UsbRegistry item)
        {
            //for (int i = 0; i < Count; i++)
            //{
            //    if (mUsbRegistryList[i] == item)
            //        return false;
            //}
            mUsbRegistryList.Add(item);
            return true;
        }
    }
}