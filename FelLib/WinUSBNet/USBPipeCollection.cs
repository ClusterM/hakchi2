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
    /// Collection of UsbPipe objects
    /// </summary>
    public class USBPipeCollection : IEnumerable<USBPipe>
    {
        private Dictionary<byte, USBPipe> _pipes;

        internal USBPipeCollection(USBPipe[] pipes)
        {
            _pipes = new Dictionary<byte, USBPipe>(pipes.Length);
            foreach (USBPipe pipe in pipes)
            {
                if (_pipes.ContainsKey(pipe.Address))
                    throw new USBException("Duplicate pipe address in endpoint.");
                _pipes[pipe.Address] = pipe;
            }
        }

        /// <summary>
        /// Returns the pipe from the collection with the given pipe address 
        /// </summary>
        /// <param name="pipeAddress">Address of the pipe to return</param>
        /// <returns>The pipe with the given pipe address</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if no pipe with the specified address
        /// is available in the collection.</exception>
        public USBPipe this [byte pipeAddress]
        {
            get
            {
                USBPipe pipe;
                if (!_pipes.TryGetValue(pipeAddress, out pipe))
                    throw new IndexOutOfRangeException();
                return pipe;
            }
        }

        private class UsbPipeEnumerator : IEnumerator<USBPipe>
        {
            private int _index;
            private USBPipe[] _pipes;

            public UsbPipeEnumerator(USBPipe[] pipes)
            {
                _pipes = pipes;
                _index = -1;
            }

            public void Dispose()
            {
                // Empty
            }
            private USBPipe GetCurrent()
            {
                try
                {
                    return _pipes[_index];
                }

                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }

            public USBPipe Current
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
                return _index < _pipes.Length;
            }

            public void Reset()
            {
                _index = -1;
            }

        }

        private USBPipe[] GetPipeList()
        {
            var values = _pipes.Values;
            USBPipe[] pipeList = new USBPipe[values.Count];
            values.CopyTo(pipeList, 0);
            return pipeList;
        }

        /// <summary>
        /// Returns a typed enumerator that iterates through a collection.
        /// </summary>
        /// <returns>The enumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<USBPipe> GetEnumerator()
        {
            return new UsbPipeEnumerator(GetPipeList());
        }
       
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new UsbPipeEnumerator(GetPipeList());
        }
    }
}
