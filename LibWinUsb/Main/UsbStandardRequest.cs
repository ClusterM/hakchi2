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

namespace LibUsbDotNet.Main
{
    /// <summary>
    /// Standard Device Requests.
    /// </summary>
    [Flags]
    public enum UsbStandardRequest : byte
    {
        /// <summary>
        /// Clear or disable a specific feature.
        /// </summary>
        ClearFeature = 0x01,
        /// <summary>
        /// Returns the current device Configuration value.
        /// </summary>
        GetConfiguration = 0x08,
        /// <summary>
        /// Returns the specified descriptor if the descriptor exists.
        /// </summary>
        GetDescriptor = 0x06,
        /// <summary>
        /// Returns the selected alternate setting for the specified interface.
        /// </summary>
        GetInterface = 0x0A,
        /// <summary>
        /// Returns status for the specified recipient.
        /// </summary>
        GetStatus = 0x00,
        /// <summary>
        /// Sets the device address for all future device accesses.
        /// </summary>
        SetAddress = 0x05,
        /// <summary>
        /// Sets the device Configuration.
        /// </summary>
        SetConfiguration = 0x09,
        /// <summary>
        /// Optional and may be used to update existing descriptors or new descriptors may be added.
        /// </summary>
        SetDescriptor = 0x07,
        /// <summary>
        /// used to set or enable a specific feature.
        /// </summary>
        SetFeature = 0x03,
        /// <summary>
        /// Allows the host to select an alternate setting for the specified interface.
        /// </summary>
        SetInterface = 0x0B,
        /// <summary>
        /// Used to set and then report an endpoint’s synchronization frame.
        /// </summary>
        SynchFrame = 0x0C,
    }
}