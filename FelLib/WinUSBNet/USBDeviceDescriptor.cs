/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// USB device details
    /// </summary>
    public class USBDeviceDescriptor
    {
        /// <summary>
        /// Windows path name for the USB device
        /// </summary>
        public string PathName { get; private set; }

        /// <summary>
        /// USB vendor ID (VID) of the device
        /// </summary>
        public int VID { get; private set; }

        /// <summary>
        /// USB product ID (PID) of the device
        /// </summary>
        public int PID { get; private set; }

        /// <summary>
        /// Manufacturer name, or null if not available
        /// </summary>
        public string Manufacturer { get; private set; }

        /// <summary>
        /// Product name, or null if not available
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Device serial number, or null if not available
        /// </summary>
        public string SerialNumber { get; private set; }


        /// <summary>
        /// Friendly device name, or path name when no 
        /// further device information is available
        /// </summary>
        public string FullName 
        { 
            get 
            {
                if (Manufacturer != null && Product != null)
                    return Product + " - " + Manufacturer;
                else if (Product != null)
                    return Product;
                else if (SerialNumber != null)
                    return SerialNumber;
                else
                    return PathName;
            }
        }

        /// <summary>
        /// Device class code as defined in the interface descriptor
        /// This property can be used if the class type is not defined
        /// int the USBBaseClass enumeraiton
        /// </summary>
        public byte ClassValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Device subclass code
        /// </summary>
        public byte SubClass
        {
            get;
            private set;
        }

        /// <summary>
        /// Device protocol code
        /// </summary>
        public byte Protocol
        {
            get;
            private set;
        }

        /// <summary>
        /// Device class code. If the device class does
        /// not match any of the USBBaseClass enumeration values
        /// the value will be USBBaseClass.Unknown
        /// </summary>
        public USBBaseClass BaseClass
        {
            get;
            private set;
        }

        internal USBDeviceDescriptor(string path, API.USB_DEVICE_DESCRIPTOR deviceDesc, string manufacturer, string product, string serialNumber)
        {
            PathName = path;
            VID = deviceDesc.idVendor;
            PID = deviceDesc.idProduct;
            Manufacturer = manufacturer;
            Product = product;
            SerialNumber = serialNumber;


            ClassValue = deviceDesc.bDeviceClass;
            SubClass = deviceDesc.bDeviceSubClass;
            Protocol = deviceDesc.bDeviceProtocol;

            // If interface class is of a known type (USBBaseeClass enum), use this
            // for the InterfaceClass property.
            BaseClass = USBBaseClass.Unknown;
            if (Enum.IsDefined(typeof(USBBaseClass), (int)deviceDesc.bDeviceClass))
            {
                BaseClass = (USBBaseClass)(int)deviceDesc.bDeviceClass;
            }
           


        }
    }
}
