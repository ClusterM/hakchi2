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

namespace MonoLibUsb.Transfer
{
    /// <summary>
    /// Transfer status codes. 
    /// </summary>
    public enum MonoUsbTansferStatus
    {
        /// <summary>
        /// Transfer completed without error. Note that this does not indicate that the entire amount of requested data was transferred.
        /// </summary>
        TransferCompleted,

        /// <summary>
        /// Transfer failed 
        /// </summary>
        TransferError,

        /// <summary>
        /// Transfer timed out 
        /// </summary>
        TransferTimedOut,

        /// <summary>
        /// Transfer was cancelled 
        /// </summary>
        TransferCancelled,

        /// <summary>
        /// For bulk/interrupt endpoints: halt condition detected (endpoint stalled). For control endpoints: control request not supported. 
        /// </summary>
        TransferStall,

        /// <summary>
        /// Device was disconnected 
        /// </summary>
        TransferNoDevice,

        /// <summary>
        /// Device sent more data than requested 
        /// </summary>
        TransferOverflow
    } ;
}