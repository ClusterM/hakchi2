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
using System.Text.RegularExpressions;

namespace LibUsbDotNet.Internal.UsbRegex
{
    /// <summary>
    /// Regular expression class for quick parsing of usb hardware ids.
    /// </summary>
    internal class RegHardwareID : Regex
    {
        #region Enumerations

        public enum ENamedGroups
        {
            Vid = 1,
            Pid = 2,
            Rev = 3,
            MI= 4
        }

        #endregion

        private const RegexOptions OPTIONS =
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled |
            RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

        private const string PATTERN = "(Vid_(?<Vid>[0-9A-F]{1,4}))|(Pid_(?<Pid>[0-9A-F]{1,4}))|(Rev_(?<Rev>[0-9]{1,4}))|(MI_(?<MI>[0-9A-F]{1,2}))";

        public static readonly NamedGroup[] NAMED_GROUPS = new NamedGroup[]
                                                               {
                                                                   new NamedGroup(1, "Vid"), new NamedGroup(2, "Pid"), new NamedGroup(3, "Rev"),
                                                                   new NamedGroup(4, "MI")
                                                               };

        public RegHardwareID() : base(PATTERN, OPTIONS) { }

        private static RegHardwareID __globalInstance;
        public static RegHardwareID GlobalInstance
        {
            get 
            { 
                if (ReferenceEquals(__globalInstance,null))
                    __globalInstance=new RegHardwareID();
                return __globalInstance;
            }
        }

        public new string[] GetGroupNames() { return new string[] {"Vid", "Pid", "Rev", "MI"}; }

        public new int[] GetGroupNumbers() { return new int[] {1, 2, 3, 4}; }

        public new string GroupNameFromNumber(int GroupNumber)
        {
            switch (GroupNumber)
            {
                case 1:
                    return "Vid";
                case 2:
                    return "Pid";
                case 3:
                    return "Rev";
                case 4:
                    return "MI";
            }
            return "";
        }

        public new int GroupNumberFromName(string GroupName)
        {
            switch (GroupName)
            {
                case "Vid":
                    return 1;
                case "Pid":
                    return 2;
                case "Rev":
                    return 3;
                case "MI":
                    return 4;
            }
            return -1;
        }
    }
}