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
namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary> Common interface describing a storage volume arrival or removal notification.
    /// </summary> 
    public interface IVolumeNotifyInfo
    {
        /// <summary>
        /// Under windows, gets the letter representation of the unitmask.
        /// Under linux, gets the full path of the device name.
        /// </summary>
        string Letter { get; }

        ///<summary>
        /// If true, change affects media in drive. If false, change affects physical device or drive.
        ///</summary>
        bool ChangeAffectsMediaInDrive { get; }

        /// <summary>
        /// If True, the indicated logical volume is a network volume
        /// </summary>
        bool IsNetworkVolume { get; }

        /// <summary>
        /// Raw DevBroadcastVolume flags.
        /// </summary>
        short Flags { get; }

        /// <summary>
        /// Gets the bit unit mask of the device. IE (bit 0 = A:, bit 1 = B:, etc..)
        /// </summary>
        int Unitmask { get; }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="VolumeNotifyInfo"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="VolumeNotifyInfo"/>.
        ///</returns>
        string ToString();
    }
}