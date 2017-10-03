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
namespace LibUsbDotNet.Main
{
    /// <summary> 
    /// Availabled endpoint numbers/ids for reading.
    /// </summary> 
    public enum ReadEndpointID : byte
    {
        /// <summary>
        /// Endpoint 1
        /// </summary>
        Ep01 = 0x81,
        /// <summary>
        /// Endpoint 2
        /// </summary>
        Ep02 = 0x82,
        /// <summary>
        /// Endpoint 3
        /// </summary>
        Ep03 = 0x83,
        /// <summary>
        /// Endpoint 4
        /// </summary>
        Ep04 = 0x84,
        /// <summary>
        /// Endpoint 5
        /// </summary>
        Ep05 = 0x85,
        /// <summary>
        /// Endpoint 6
        /// </summary>
        Ep06 = 0x86,
        /// <summary>
        /// Endpoint 7
        /// </summary>
        Ep07 = 0x87,
        /// <summary>
        /// Endpoint 8
        /// </summary>
        Ep08 = 0x88,
        /// <summary>
        /// Endpoint 9
        /// </summary>
        Ep09 = 0x89,
        /// <summary>
        /// Endpoint 10
        /// </summary>
        Ep10 = 0x8A,
        /// <summary>
        /// Endpoint 11
        /// </summary>
        Ep11 = 0x8B,
        /// <summary>
        /// Endpoint 12
        /// </summary>
        Ep12 = 0x8C,
        /// <summary>
        /// Endpoint 13
        /// </summary>
        Ep13 = 0x8D,
        /// <summary>
        /// Endpoint 14
        /// </summary>
        Ep14 = 0x8E,
        /// <summary>
        /// Endpoint 15
        /// </summary>
        Ep15 = 0x8F,
    }
}