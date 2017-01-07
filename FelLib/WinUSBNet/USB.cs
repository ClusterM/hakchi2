/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;


namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// USB base class code enumeration, as defined in the USB specification
    /// </summary>
    public enum USBBaseClass
    {
        /// <summary>Unknown non-zero class code. Used when the actual class code
        /// does not match any of the ones defined in this enumeration. </summary>
        Unknown = -1,

        /// <summary>Base class defined elsewhere (0x00)</summary>
        None = 0x00,

        /// <summary>Audio base class (0x01)</summary>
        Audio = 0x01,

        /// <summary>Communications and CDC control base class (0x02)</summary>
        CommCDC = 0x02,

        /// <summary>HID base class (0x03)</summary>
        HID = 0x03,

        /// <summary>Physical base class (0x05)</summary>
        Physical = 0x05,

        /// <summary>Image base class (0x06)</summary>
        Image = 0x06,

        /// <summary>Printer base class (0x07)</summary>
        Printer = 0x07,

        /// <summary>Mass storage base class (0x08)</summary>
        MassStorage = 0x08,

        /// <summary>Hub base class (0x09)</summary>
        Hub = 0x09,

        /// <summary>CDC data base class (0x0A)</summary>
        CDCData = 0x0A,

        /// <summary>Smart card base class (0x0B)</summary>
        SmartCard = 0x0B,

        /// <summary>Content security base class (0x0D)</summary>
        ContentSecurity = 0x0D,

        /// <summary>Video base class (0x0E)</summary>
        Video = 0x0E,

        /// <summary>Personal healthcare base class (0x0F)</summary>
        PersonalHealthcare = 0x0F,

        /// <summary>Diagnosticdevice base class (0xDC)</summary>
        DiagnosticDevice = 0xDC,

        /// <summary>Wireless controller base class (0xE0)</summary>
        WirelessController = 0xE0,

        /// <summary>Miscellaneous base class (0xEF)</summary>
        Miscellaneous = 0xEF,

        /// <summary>Application specific base class (0xFE)</summary>
        ApplicationSpecific = 0xFE,

        /// <summary>Vendor specific base class (0xFF)</summary>
        VendorSpecific = 0xFF,
    };

}