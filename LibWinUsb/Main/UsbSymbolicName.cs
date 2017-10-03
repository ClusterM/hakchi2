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
using System.Globalization;
using System.Text.RegularExpressions;
using LibUsbDotNet.Internal.UsbRegex;

namespace LibUsbDotNet.Main
{
    /// <summary> USB device symbolic names are persistent accrossed boots and uniquely identify each device.
    /// </summary> 
    /// <remarks> As well as uniquely identify connected devices, the UsbSymbolicName class parses the symbolic name key into usable fields.
    /// </remarks> 
    public class UsbSymbolicName
    {
        private static RegHardwareID _regHardwareId;
        private static RegSymbolicName _regSymbolicName;
        private readonly string mSymbolicName;

        private Guid mClassGuid = Guid.Empty;
        private bool mIsParsed;
        private int mProductID;
        private int mRevisionCode;
        private string mSerialNumber = String.Empty;
        private int mVendorID;

        internal UsbSymbolicName(string symbolicName) { mSymbolicName = symbolicName; }

        private static RegSymbolicName RegSymbolicName
        {
            get
            {
                if (ReferenceEquals(_regSymbolicName, null))
                {
                    _regSymbolicName = new RegSymbolicName();
                }

                return _regSymbolicName;
            }
        }

        private static RegHardwareID RegHardwareId
        {
            get
            {
                if (ReferenceEquals(_regHardwareId, null))
                {
                    _regHardwareId = new RegHardwareID();
                }

                return _regHardwareId;
            }
        }

        /// <summary>
        /// The full symbolic name of the device.
        /// </summary>
        public string FullName
        {
            get
            {
                if (mSymbolicName != null) return mSymbolicName.TrimStart(new char[] {'\\', '?'});
                return String.Empty;
            }
        }

        /// <summary>
        /// VendorId parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public int Vid
        {
            get
            {
                Parse();
                return mVendorID;
            }
        }

        /// <summary>
        /// ProductId parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public int Pid
        {
            get
            {
                Parse();
                return mProductID;
            }
        }

        /// <summary>
        /// SerialNumber parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public string SerialNumber
        {
            get
            {
                Parse();
                return mSerialNumber;
            }
        }

        /// <summary>
        /// Device class parsed out of the <see cref="UsbSymbolicName.FullName"/>
        /// </summary>
        public Guid ClassGuid
        {
            get
            {
                Parse();
                return mClassGuid;
            }
        }

        /// <summary>
        /// Usb device revision number.
        /// </summary>
        public int Rev
        {
            get
            {
                Parse();
                return mRevisionCode;
            }
        }


        /// <summary>
        /// Parses registry strings containing USB information.  This function can Parse symbolic names as well as hardware ids, compatible ids, etc.
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns>A <see cref="UsbSymbolicName"/> class with all the available information from the <paramref name="identifiers"/> string.</returns>
        /// <remarks>
        /// <code>
        ///             List&lt;UsbRegistryDeviceInfo&gt; regDeviceList = UsbGlobals.RegFindDevices();
        ///    foreach (UsbRegistryDeviceInfo regDevice in mDevList)
        ///    {
        ///        string[] hardwareIds = (string[])regDevice.Properties[DevicePropertyType.HardwareID];
        ///       UsbSymbolicName usbHardwareID = UsbSymbolicName.Parse(hardwareIds[0]);
        ///        Debug.Print(string.Format("Vid:0x{0:X4} Pid:0x{1:X4}", usbHardwareID.Vid, usbHardwareID.Pid));
        ///    }
        /// </code>
        /// </remarks>
        public static UsbSymbolicName Parse(string identifiers) { return new UsbSymbolicName(identifiers); }

        ///<summary>
        ///Returns a <see cref="T:System.String"/> that represents the current <see cref="UsbSymbolicName"/>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="System.String"/> that represents the current <see cref="UsbSymbolicName"/>.
        ///</returns>
        public override string ToString()
        {
            object[] o = new object[] {FullName, Vid.ToString("X4"), Pid.ToString("X4"), SerialNumber, ClassGuid};
            return string.Format("FullName:{0}\r\nVid:0x{1}\r\nPid:0x{2}\r\nSerialNumber:{3}\r\nClassGuid:{4}\r\n", o);
        }


        private void Parse()
        {
            if (!mIsParsed)
            {
                mIsParsed = true;
                if (mSymbolicName != null)
                {
                    MatchCollection matches = RegSymbolicName.Matches(mSymbolicName);
                    foreach (Match match in matches)
                    {
                        Group gVid = match.Groups[(int) NamedGroupType.Vid];
                        Group gPid = match.Groups[(int) NamedGroupType.Pid];
                        Group gRev = match.Groups[(int) NamedGroupType.Rev];
                        Group gString = match.Groups[(int) NamedGroupType.String];
                        Group gClass = match.Groups[(int) NamedGroupType.ClassGuid];

                        if (gVid.Success && mVendorID == 0)
                        {
                            int.TryParse(gVid.Captures[0].Value, NumberStyles.HexNumber, null, out mVendorID);
                        }
                        if (gPid.Success && mProductID == 0)
                        {
                            int.TryParse(gPid.Captures[0].Value, NumberStyles.HexNumber, null, out mProductID);
                        }
                        if (gRev.Success && mRevisionCode == 0)
                        {
                            int.TryParse(gRev.Captures[0].Value, out mRevisionCode);
                        }
                        if ((gString.Success) && mSerialNumber == String.Empty)
                        {
                            mSerialNumber = gString.Captures[0].Value;
                        }
                        if ((gClass.Success) && mClassGuid == Guid.Empty)
                        {
                            try
                            {
                                mClassGuid = new Guid(gClass.Captures[0].Value);
                            }
                            catch (Exception)
                            {
                                mClassGuid = Guid.Empty;
                            }
                        }
                    }
                }
            }
        }
    }
}