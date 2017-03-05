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
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.WinUsb
{
    /// <summary> WinUsb Pipe information.
    /// </summary> 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PipeInformation
    {
        /// <summary>
        /// Size of the structure in bytes.
        /// </summary>
        public static readonly int Size = Marshal.SizeOf(typeof (PipeInformation));

        /// <summary>
        /// Specifies the pipe type.
        /// </summary>
        public EndpointType PipeType;

        /// <summary>
        /// The pipe identifier (ID). 
        /// </summary>
        public byte PipeId;

        /// <summary>
        /// The maximum size, in bytes, of the packets that are transmitted on the pipe.
        /// </summary>
        public short MaximumPacketSize;

        /// <summary>
        /// The pipe interval.
        /// </summary>
        public byte Interval;
    }
}