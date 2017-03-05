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
    /// Event arguments that are passed when <see cref="UsbEndpointReader.DataReceivedEnabled"/> is changes state.
    /// </summary>
    public class DataReceivedEnabledChangedEventArgs : EventArgs
    {
        private readonly bool mEnabled;
        private readonly ErrorCode mErrorCode = ErrorCode.Success;

        internal DataReceivedEnabledChangedEventArgs(bool enabled, ErrorCode errorCode)
        {
            mEnabled = enabled;
            mErrorCode = errorCode;
        }

        internal DataReceivedEnabledChangedEventArgs(bool enabled)
            : this(enabled, ErrorCode.Success) { }

        /// <summary>
        /// The <see cref="Main.ErrorCode"/> that caused the <see cref="UsbEndpointReader.DataReceived"/> event to terminate.
        /// </summary>
        public ErrorCode ErrorCode
        {
            get { return mErrorCode; }
        }

        /// <summary>
        /// <c>True</c> if <see cref="UsbEndpointReader.DataReceivedEnabled"/> changes from <c>True</c> to <c>False</c>. 
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
        }
    }
}