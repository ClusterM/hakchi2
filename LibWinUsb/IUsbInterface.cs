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
using System.Collections.ObjectModel;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

namespace LibUsbDotNet
{
    /// <summary>
    /// The <see cref="IUsbInterface"/> interface contains members needed communicate with an 
    /// interface of a usb device. 
    /// </summary>
    /// <remarks>
    /// All USB device classes implement these members.
    /// </remarks>
    public interface IUsbInterface
    {
        /// <summary>
        /// A list of endpoints that have beened opened by this <see cref="UsbDevice"/> class.
        /// </summary>
        UsbEndpointList ActiveEndpoints { get; }

        ///<summary>
        /// Gets the available configurations for this <see cref="UsbDevice"/>
        ///</summary>
        /// <remarks>
        /// The first time this property is accessed it will query the <see cref="UsbDevice"/> for all configurations.  Subsequent request will return a cached copy of all configurations.
        /// </remarks>
        ReadOnlyCollection<UsbConfigInfo> Configs { get; }

        /// <summary>
        /// Returns the DriverMode this USB device is using.
        /// </summary>
        UsbDevice.DriverModeType DriverMode { get; }

        /// <summary>
        /// Gets the actual device descriptor the the current <see cref="UsbDevice"/>.
        /// </summary>
        UsbDeviceInfo Info { get; }

        /// <summary>
        /// Gets a value indication if the device handle is valid.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the <see cref="UsbRegistry"/> class that opened the device, or null if the device was not opened by the <see cref="UsbRegistry"/> class.
        /// </summary>
        UsbRegistry UsbRegistryInfo { get; }

        /// <summary>
        /// Closes and frees device resources.  Once closed the device cannot be reopened.  A new <see cref="UsbDevice"/> class must be obtained using the <see cref="UsbGlobals"/> class.
        /// </summary>
        /// <returns>True on success.</returns>
        bool Close();

        /// <summary>
        /// Sends/Receives an IO control message to endpoint 0.
        /// </summary>
        /// <param name="setupPacket">Contains parameters for the control request. See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information. </param>
        /// <param name="buffer">Data to be sent/received from the device.</param>
        /// <param name="bufferLength">Length of the buffer param.</param>
        /// <param name="lengthTransferred">Number of bytes sent or received (depends on the direction of the control transfer).</param>
        /// <returns>True on success.</returns>
        bool ControlTransfer(ref UsbSetupPacket setupPacket, IntPtr buffer, int bufferLength, out int lengthTransferred);

        /// <summary>
        /// Transmits io control message to endpoint 0.
        /// </summary>
        /// <param name="setupPacket">Contains parameters for the control request. See section 9.3 USB Device Requests of the Universal Serial Bus Specification Revision 2.0 for more information. </param>
        /// <param name="buffer">Data to be sent/received from the device.  Th</param>
        /// <param name="bufferLength">Length of the buffer param.</param>
        /// <param name="lengthTransferred">Number of bytes sent or received (depends on the direction of the control transfer).</param>
        /// <returns>True on success.</returns>
        bool ControlTransfer(ref UsbSetupPacket setupPacket, object buffer, int bufferLength, out int lengthTransferred);

        /// <summary>
        /// Gets a specific descriptor from the device. See <see cref="DescriptorType"/> for more information.
        /// </summary>
        /// <param name="descriptorType">The descriptor type ID to retrieve; this is usually one of the <see cref="DescriptorType"/> enumerations.</param>
        /// <param name="index">Descriptor index.</param>
        /// <param name="langId">Descriptor language id.</param>
        /// <param name="buffer">Memory to store the returned descriptor in.</param>
        /// <param name="bufferLength">Length of the buffer parameter in bytes.</param>
        /// <param name="transferLength">The number of bytes transferred to buffer upon success.</param>
        /// <returns>True on success.</returns>
        bool GetDescriptor(byte descriptorType, byte index, short langId, IntPtr buffer, int bufferLength, out int transferLength);

        /// <summary>
        /// Gets a specific descriptor from the device. See <see cref="DescriptorType"/> for more information.
        /// </summary>
        /// <param name="descriptorType">The descriptor type ID to retrieve; this is usually one of the <see cref="DescriptorType"/> enumerations.</param>
        /// <param name="index">Descriptor index.</param>
        /// <param name="langId">Descriptor language id.</param>
        /// <param name="buffer">Memory to store the returned descriptor in.</param>
        /// <param name="bufferLength">Length of the buffer parameter in bytes.</param>
        /// <param name="transferLength">The number of bytes transferred to buffer upon success.</param>
        /// <returns>True on success.</returns>
        bool GetDescriptor(byte descriptorType, byte index, short langId, object buffer, int bufferLength, out int transferLength);

        /// <summary>
        /// Asking for the zero'th index is special - it returns a string
        /// descriptor that contains all the language IDs supported by the
        /// device. Typically there aren't many - often only one. The
        /// language IDs are 16 bit numbers, and they start at the third byte
        /// in the descriptor. See USB 2.0 specification, section 9.6.7, for
        /// more information on this. 
        /// </summary>
        /// <returns>A collection of LCIDs that the current <see cref="UsbDevice"/> supports.</returns>
        bool GetLangIDs(out short[] langIDs);

        /// <summary>
        /// Gets a string descriptor from the device.
        /// </summary>
        /// <param name="stringData">Buffer to store the returned string in upon success.</param>
        /// <param name="langId">The language ID to retrieve the string in. (0x409 for english).</param>
        /// <param name="stringIndex">The string index to retrieve.</param>
        /// <returns>True on success.</returns>
        bool GetString(out string stringData, short langId, byte stringIndex);

        ///<summary>
        /// Opens/re-opens this USB device instance for communication.
        ///</summary>
        ///<returns>True if the device is already opened or was opened successfully.  False if the device does not exists or is no longer valid.</returns>
        bool Open();

        /// <summary>
        /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
        /// </summary>
        /// <param name="readEndpointID">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
        UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize);

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpointID">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <param name="endpointType">The type of endpoint to open.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
        UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID, int readBufferSize, EndpointType endpointType);

        /// <summary>
        /// Opens a <see cref="EndpointType.Bulk"/> endpoint for reading
        /// </summary>
        /// <param name="readEndpointID">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading. If the specified endpoint is already been opened, the original <see cref="UsbEndpointReader"/> class is returned.</returns>
        UsbEndpointReader OpenEndpointReader(ReadEndpointID readEndpointID);

        /// <summary>
        /// Opens a <see cref="EndpointType.Bulk"/> endpoint for writing
        /// </summary>
        /// <param name="writeEndpointID">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
        UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID);

        /// <summary>
        /// Opens an endpoint for writing
        /// </summary>
        /// <param name="writeEndpointID">Endpoint number for read operations.</param>
        /// <param name="endpointType">The type of endpoint to open.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing. If the specified endpoint is already been opened, the original <see cref="UsbEndpointWriter"/> class is returned.</returns>
        UsbEndpointWriter OpenEndpointWriter(WriteEndpointID writeEndpointID, EndpointType endpointType);
    }
}