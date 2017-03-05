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
namespace LibUsbDotNet.Internal.UsbRegex
{
    /// <summary>
    /// Regular expression class for parsing USB symbolic names and hardware ids.
    /// </summary>
    internal class RegSymbolicName : BaseRegSymbolicName
    {
        public static readonly NamedGroup[] NamedGroups = new NamedGroup[]
                                                              {
                                                                  new NamedGroup(1, "Vid"),
                                                                  new NamedGroup(2, "Pid"),
                                                                  new NamedGroup(3, "Rev"),
                                                                  new NamedGroup(4, "ClassGuid"),
                                                                  new NamedGroup(5, "String"),
                                                              };

        public new string[] GetGroupNames() { return new string[] {"Vid", "Pid", "Rev", "ClassGuid", "String"}; }

        public new int[] GetGroupNumbers() { return new int[] {1, 2, 3, 4, 5}; }

        public new string GroupNameFromNumber(int groupNumber)
        {
            switch (groupNumber)
            {
                case 1:
                    return "Vid";

                case 2:
                    return "Pid";

                case 3:
                    return "Rev";

                case 4:
                    return "ClassGuid";

                case 5:
                    return "String";
            }
            return "";
        }

        public new int GroupNumberFromName(string groupName)
        {
            switch (groupName.ToLower())
            {
                case "vid":
                    return 1;

                case "pid":
                    return 2;

                case "rev":
                    return 3;

                case "classguid":
                    return 4;

                case "string":
                    return 5;
            }
            return -1;
        }
    }

    internal enum NamedGroupType
    {
        Vid = 1,
        Pid = 2,
        Rev = 3,
        ClassGuid = 4,
        String = 5,
    }
}