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
using System.Collections;
using System.Collections.Generic;

namespace LibUsbDotNet.Main
{
    /// <summary> Endpoint list.
    /// </summary> 
    public class UsbEndpointList : IEnumerable<UsbEndpointBase>
    {
        private readonly List<UsbEndpointBase> mEpList = new List<UsbEndpointBase>();

        internal UsbEndpointList() { }

        /// <summary>
        /// Gets the <see cref="UsbEndpointBase"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <returns>The <see cref="UsbEndpointBase"/> item at the specified index.</returns>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="UsbEndpointList"/>.</exception>
        public UsbEndpointBase this[int index]
        {
            get { return mEpList[index]; }
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="UsbEndpointList"/>.
        ///</summary>
        ///
        ///<returns>
        ///The number of elements contained in the <see cref="UsbEndpointList"/>.
        ///</returns>
        ///
        public int Count
        {
            get { return mEpList.Count; }
        }

        #region IEnumerable<UsbEndpointBase> Members

        ///<summary>
        ///Returns <see cref="UsbEndpointBase"/> enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="UsbEndpointBase"/> enumerator that can be used to iterate through the collection.
        ///</returns>
        public IEnumerator<UsbEndpointBase> GetEnumerator() { return mEpList.GetEnumerator(); }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() { return mEpList.GetEnumerator(); }

        #endregion

        ///<summary>
        ///Removes all items from the <see cref="UsbEndpointList"/>.
        ///</summary>
        public void Clear()
        {
            while (mEpList.Count > 0)
                Remove(mEpList[0]);
        }

        ///<summary>
        ///Determines whether the <see cref="UsbEndpointList"/> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if item is found in the <see cref="UsbEndpointList"/>; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"/> to locate in the <see cref="UsbEndpointList"/>.</param>
        public bool Contains(UsbEndpointBase item) { return mEpList.Contains(item); }


        ///<summary>
        ///Determines the index of a specific <see cref="UsbEndpointBase"/> in the <see cref="UsbEndpointList"/>.
        ///</summary>
        ///
        ///<returns>
        ///The index of item if found in the list; otherwise, -1.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"/> to locate in the <see cref="UsbEndpointList"/>.</param>
        public int IndexOf(UsbEndpointBase item) { return mEpList.IndexOf(item); }

        ///<summary>
        ///Removes the specified <see cref="UsbEndpointBase"/> in the <see cref="UsbEndpointList"/>.
        ///</summary>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"/> to remove in the <see cref="UsbEndpointList"/>.</param>
        public void Remove(UsbEndpointBase item) { item.Dispose(); }

        ///<summary>
        ///Removes the <see cref="UsbEndpointList"/> item at the specified index.
        ///</summary>
        ///
        ///<param name="index">The zero-based index of the item to remove.</param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="UsbEndpointList"/>.</exception>
        public void RemoveAt(int index)
        {
            UsbEndpointBase item = mEpList[index];
            Remove(item);
        }


        internal UsbEndpointBase Add(UsbEndpointBase item)
        {
            foreach (UsbEndpointBase endpoint in mEpList)
            {
                if (endpoint.EpNum == item.EpNum)
                    return endpoint;
            }
            mEpList.Add(item);
            return item;
        }

        internal bool RemoveFromList(UsbEndpointBase item) { return mEpList.Remove(item); }
    }
}