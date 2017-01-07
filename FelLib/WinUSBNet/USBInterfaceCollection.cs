/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// Collection of UsbInterface objects
    /// </summary>
    public class USBInterfaceCollection : IEnumerable<USBInterface>
    {
        private USBInterface[] _interfaces;

        internal USBInterfaceCollection(USBInterface[] interfaces)
        {
            _interfaces = interfaces;
        }

        private class USBInterfaceEnumerator : IEnumerator<USBInterface>
        {
            private int _index;
            private USBInterface[] _interfaces;

            public USBInterfaceEnumerator(USBInterface[] interfaces)
            {
                _interfaces = interfaces;
                _index = -1;
            }

            public void Dispose()
            {
                // Intentionally empty
            }
            private USBInterface GetCurrent()
            {
                try
                {
                    return _interfaces[_index];
                }

                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }

            public USBInterface Current
            {
                get
                {
                    return GetCurrent();
                }
            }


            object IEnumerator.Current
            {
                get
                {
                    return GetCurrent();
                }
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _interfaces.Length;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        /// <summary>
        /// Finds the first interface with that matches the device class 
        /// given by the <paramref name="interfaceClass"/> parameter.
        /// </summary>
        /// <param name="interfaceClass">The device class the interface should match</param>
        /// <returns>The first interface with the given interface class, or null
        /// if no such interface exists.</returns>
        public USBInterface Find(USBBaseClass interfaceClass)
        {
            for (int i = 0; i < _interfaces.Length; i++)
            {
                USBInterface iface = _interfaces[i];
                if (iface.BaseClass == interfaceClass)
                    return iface;
            }
            return null;
        }

        /// <summary>
        /// Finds all interfaces matching the device class given by the 
        /// <paramref name="interfaceClass"/> parameter.
        /// </summary>
        /// <param name="interfaceClass">The device class the interface should match</param>
        /// <returns>An array of USBInterface objects matching the device class, or an empty 
        /// array if no interface matches.</returns>
        public USBInterface[] FindAll(USBBaseClass interfaceClass)
        {
            List<USBInterface> matchingInterfaces = new List<USBInterface>();
            for (int i = 0; i < _interfaces.Length; i++)
            {
                USBInterface iface = _interfaces[i];
                if (iface.BaseClass == interfaceClass)
                    matchingInterfaces.Add(iface);
            }
            return matchingInterfaces.ToArray();
        }

        /// <summary>
        /// Returns a typed enumerator that iterates through a collection.
        /// </summary>
        /// <returns>The enumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<USBInterface> GetEnumerator()
        {
            return new USBInterfaceEnumerator(_interfaces);
        }

        /// <summary>
        /// Get interface by interface number
        /// </summary>
        /// <param name="interfaceNumber">Number of the interface to return. Note: this is the number from the interface descriptor, which
        /// is not necessarily the same as the interface index.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when the given interface number does not exist in the collection.</exception>
        /// <returns></returns>
        public USBInterface this[ int interfaceNumber ]
        {
            get
            {
                for (int i = 0; i < _interfaces.Length; i++)
                {
                    USBInterface iface = _interfaces[i];
                    if (iface.Number == interfaceNumber)
                        return iface;
                }
                throw new IndexOutOfRangeException(string.Format("No interface with number {0} exists.", interfaceNumber));
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new USBInterfaceEnumerator(_interfaces);
        }
    }
}
