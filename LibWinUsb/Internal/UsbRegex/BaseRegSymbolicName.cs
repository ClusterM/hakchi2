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
using System.Text.RegularExpressions;

namespace LibUsbDotNet.Internal.UsbRegex
{
    internal class BaseRegSymbolicName : Regex
    {
        private const RegexOptions OPTIONS =
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled |
            RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

        private const string PATTERN =
            @"((&){0,1}Vid_(?<Vid>[0-9A-Fa-f]{1,4})(&){0,1}Pid_(?<Pid>[0-9A-Fa-f]{1,4})((&){0,1}Rev_(?<Rev>[0-9A-Fa-f]{1,4})){0,1})((\x23{0,1}\{(?<ClassGuid>([0-9A-Fa-f]+)-([0-9A-Fa-f]+)-([0-9A-Fa-f]+)-([0-9A-Fa-f]+)-([0-9A-Fa-f]+))})|(\x23(?<String>[\x20-\x22\x24-\x2b\x2d-\x7f]+?)(?=\x23|$)))*";

        public BaseRegSymbolicName() : base(PATTERN, OPTIONS) { }
    }
}