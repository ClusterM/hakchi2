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

namespace MonoLibUsb.Descriptors
{
    ///<summary>A collection of alternate settings for a particular USB interface.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = MonoUsbApi.LIBUSB_PACK)]
    public class MonoUsbInterface
    {
        ///<summary> Array of interface descriptors. The length of this array is determined by the num_altsetting field.</summary>
        private IntPtr pAltSetting;

        ///<summary> The number of alternate settings that belong to this interface</summary>
        public readonly int num_altsetting;


        ///<summary> Array of interface descriptors. The length of this array is determined by the num_altsetting field.</summary>
        public List<MonoUsbAltInterfaceDescriptor> AltInterfaceList
        {
            get
            {
                List<MonoUsbAltInterfaceDescriptor> altInterfaceList = new List<MonoUsbAltInterfaceDescriptor>();
                int iAltInterface;
                for (iAltInterface = 0; iAltInterface < num_altsetting; iAltInterface++)
                {
                    IntPtr pNextInterface = new IntPtr(pAltSetting.ToInt64() + (Marshal.SizeOf(typeof (MonoUsbAltInterfaceDescriptor))*iAltInterface));
                    MonoUsbAltInterfaceDescriptor monoUSBAltInterfaceDescriptor = new MonoUsbAltInterfaceDescriptor();
                    Marshal.PtrToStructure(pNextInterface, monoUSBAltInterfaceDescriptor);

                    altInterfaceList.Add(monoUSBAltInterfaceDescriptor);
                }

                return altInterfaceList;
            }
        }
    }
}