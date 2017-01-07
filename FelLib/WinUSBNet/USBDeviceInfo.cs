/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// Gives information about a device. This information is retrieved using the setup API, not the
    /// actual device descriptor. Device description and manufacturer will be the strings specified
    /// in the .inf file. After a device is opened the actual device descriptor can be read as well.
    /// </summary>
    public class USBDeviceInfo
    {
        private API.DeviceDetails _details;

        /// <summary>
        /// Vendor ID (VID) of the USB device
        /// </summary>
        public int VID
        {
            get
            {
                return _details.VID;
            }
        }

        /// <summary>
        /// Product ID (VID) of the USB device
        /// </summary>
        public int PID
        {
            get
            {
                return _details.PID;
            }
        }

        /// <summary>
        /// Manufacturer of the device, as specified in the INF file (not the device descriptor)
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return _details.Manufacturer;
            }
        }

        /// <summary>
        /// Description of the device, as specified in the INF file (not the device descriptor)
        /// </summary>
        public string DeviceDescription
        {
            get
            {
                return _details.DeviceDescription;
            }
        }

        /// <summary>
        /// Device pathname
        /// </summary>
        public string DevicePath
        {
            get
            {
                return _details.DevicePath;
            }
        }

        internal USBDeviceInfo(API.DeviceDetails details)
        {
            _details = details;
        }

    }
}
