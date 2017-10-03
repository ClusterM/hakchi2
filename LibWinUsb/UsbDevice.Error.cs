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
using LibUsbDotNet.Internal;
using LibUsbDotNet.Main;
using MonoLibUsb;

namespace LibUsbDotNet
{
    partial class UsbDevice
    {
        internal static void FireUsbError(object sender, UsbError usbError)
        {
            EventHandler<UsbError> temp = UsbErrorEvent;
            if (!ReferenceEquals(null, temp))
                temp(sender, usbError);
        }
    }

    /// <summary> Describes a Usb error or setup API error.
    /// </summary> 
    public class UsbError : EventArgs
    {
        internal static int mLastErrorNumber;
        internal static string mLastErrorString = String.Empty;

        ///// <summary>
        ///// The the error is <see cref="Handled"/> field is set to true for errors resulting from endpoint read/write errors.  The operation will retry instead of exiting with an error code. 
        ///// </summary>
        //public bool Handled;

        internal string mDescription;

        internal ErrorCode mErrorCode;
        private object mSender;
        internal int mWin32ErrorNumber;
        internal string mWin32ErrorString = "None";

        internal UsbError(ErrorCode errorCode, int win32ErrorNumber, string win32ErrorString, string description, object sender)
        {
            mSender = sender;
            string senderText = String.Empty;
            if ((mSender is UsbEndpointBase)|| (mSender is UsbTransfer))
            {
                UsbEndpointBase ep;
                if (mSender is UsbTransfer)
                    ep = ((UsbTransfer)mSender).EndpointBase;
                else
                    ep = mSender as UsbEndpointBase;

                if (ep.mEpNum != 0)
                {

                    senderText = senderText+=string.Format(" Ep 0x{0:X2} ", ep.mEpNum);
                }
            }
            else if (mSender is Type)
            {
                Type t = mSender as Type;
                senderText = senderText += string.Format(" {0} ", t.Name);
            }
            mErrorCode = errorCode;
            mWin32ErrorNumber = win32ErrorNumber;
            mWin32ErrorString = win32ErrorString;
            mDescription = description + senderText;
        }

        /// <summary>
        /// The sender of the exception.
        /// </summary>
        public object Sender
        {
            get { return mSender; }
        }

        /// <summary>
        /// Gets the general errorcode.
        /// </summary>
        public ErrorCode ErrorCode
        {
            get { return mErrorCode; }
        }

        /// <summary>
        /// Gets the Windows specific error number.  Only valid when <see cref="ErrorCode"/> is set to <see cref="Main.ErrorCode"/>.<see cref="Main.ErrorCode.Win32Error"/>.
        /// </summary>
        public int Win32ErrorNumber
        {
            get { return mWin32ErrorNumber; }
        }

        /// <summary>
        /// Gets the general description for the error.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }

        /// <summary>
        /// Gets the Windows specific error string. Only valid when <see cref="ErrorCode"/> is set to <see cref="Main.ErrorCode"/>.<see cref="Main.ErrorCode.Win32Error"/>.
        /// </summary>
        public string Win32ErrorString
        {
            get { return mWin32ErrorString; }
        }


        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbError"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbError"/>.
        ///</returns>
        public override string ToString()
        {
            if (Win32ErrorNumber != 0)
            {
                return String.Format("{0}:{1}\r\n{2}:{3}", ErrorCode, Description, Win32ErrorNumber, mWin32ErrorString);
            }
            return String.Format("{0}:{1}", ErrorCode, Description);
        }

        internal static UsbError Error(ErrorCode errorCode, int ret, string description, object sender)
        {
            string win32Error = String.Empty;
            if (errorCode == ErrorCode.Win32Error && !UsbDevice.IsLinux && ret != 0)
            {
                win32Error = Kernel32.FormatSystemMessage(ret);
            }
            else if (errorCode == ErrorCode.MonoApiError && ret != 0)
            {
                win32Error = ((MonoUsbError) ret) + ":" + MonoUsbApi.StrError((MonoUsbError) ret);
            }
            UsbError err = new UsbError(errorCode, ret, win32Error, description, sender);
            lock (mLastErrorString)
            {
                mLastErrorNumber = (int) err.ErrorCode;
                mLastErrorString = err.ToString();
            }
            UsbDevice.FireUsbError(sender, err);

            return err;
        }
    }
}