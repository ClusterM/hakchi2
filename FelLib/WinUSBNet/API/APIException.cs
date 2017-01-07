/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace MadWizard.WinUSBNet.API
{
    /// <summary>
    /// Exception used internally to catch Win32 API errors. This exception should
    /// not be thrown to the library's caller.
    /// </summary>
    internal class APIException : Exception
    {
        public APIException(string message) :
            base(message)
        {
            
        }
        public APIException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static APIException Win32(string message)
        {
            return APIException.Win32(message, Marshal.GetLastWin32Error());
        }
        
        public static APIException Win32(string message, int errorCode) 
        {
            return new APIException(message, new Win32Exception(errorCode));
            
        }

    }
}
