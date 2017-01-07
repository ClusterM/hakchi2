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
    /// Represents a single USB interface from a USB device
    /// </summary>
    public class USBInterface
    {
        /// <summary>
        /// Collection of pipes associated with this interface
        /// </summary>
        public USBPipeCollection Pipes
        {
            get;
            private set;
        }

        /// <summary>
        /// Interface number from the interface descriptor
        /// </summary>
        public int Number
        {
            get;
            private set;
        }
      
        /// <summary>
        /// USB device associated with this interface
        /// </summary>
        public USBDevice Device
        {
            get;
            private set;
        }

        /// <summary>
        /// First IN direction pipe on this interface
        /// </summary>
        public USBPipe InPipe
        {
            get;
            private set;
        }

        /// <summary>
        /// First OUT direction pipe on this interface
        /// </summary>
        public USBPipe OutPipe
        {
            get;
            private set;
        }

        /// <summary>
        /// Interface class code. If the interface class does
        /// not match any of the USBBaseClass enumeration values
        /// the value will be USBBaseClass.Unknown
        /// </summary>
        public USBBaseClass BaseClass
        {
            get;
            private set;
        }

        /// <summary>
        /// Interface class code as defined in the interface descriptor
        /// This property can be used if the class type is not defined
        /// int the USBBaseClass enumeraiton
        /// </summary>
        public byte ClassValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Interface subclass code
        /// </summary>
        public byte SubClass
        {
            get;
            private set;
        }

        /// <summary>
        /// Interface protocol code
        /// </summary>
        public byte Protocol
        {
            get;
            private set;
        }
       
        /// Zero based interface index in WinUSB.
        /// Note that this is not necessarily the same as the interface *number*
        /// from the interface descriptor. There might be interfaces within the
        /// USB device that do not use WinUSB, these are not counted for index.
        internal int InterfaceIndex
        {
            get;
            private set;
        }

        internal USBInterface(USBDevice device, int interfaceIndex, API.USB_INTERFACE_DESCRIPTOR rawDesc, USBPipeCollection pipes)
        {
            // Set raw class identifiers
            ClassValue = rawDesc.bInterfaceClass;
            SubClass = rawDesc.bInterfaceSubClass;
            Protocol = rawDesc.bInterfaceProtocol;

            Number = rawDesc.bInterfaceNumber;
            InterfaceIndex = interfaceIndex;

            // If interface class is of a known type (USBBaseClass enum), use this
            // for the InterfaceClass property.
            BaseClass = USBBaseClass.Unknown;
            if (Enum.IsDefined(typeof(USBBaseClass), (int)rawDesc.bInterfaceClass))
            {
                BaseClass = (USBBaseClass)(int)rawDesc.bInterfaceClass;
            }
           

            Device = device;
            Pipes = pipes;

            // Handle pipes
            foreach (USBPipe pipe in pipes)
            {
                // Attach pipe to this interface
                pipe.AttachInterface(this);

                // If first in or out pipe, set InPipe and OutPipe
                if (pipe.IsIn && InPipe == null)
                    InPipe = pipe;
                if (pipe.IsOut && OutPipe == null)
                    OutPipe = pipe;

            }
        
        }
    }
}
