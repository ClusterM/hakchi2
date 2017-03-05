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
using System.Runtime.InteropServices;

using LibUsbDotNet.Main;
using MonoLibUsb.Transfer.Internal;

namespace MonoLibUsb.Transfer
{
    /// <summary>
    /// Reads/writes a Libusb-1.0 transfer pointer.  Transfer should be allocated with <see cref="Alloc"/>.
    /// </summary>
    /// <remarks>
    /// The user populates this structure and then submits it in order to request a transfer. 
    /// After the transfer has completed, the library populates the transfer with the results 
    /// and passes it back to the user.
    /// <note title="Libusb-1.0 API Note:" type="cpp">
    /// The <see cref="MonoUsbTransfer"/> structure is roughly equivalent to
    /// the <a href="http://libusb.sourceforge.net/api-1.0/structlibusb__transfer.html">struct libusb_transfer</a>.
    /// </note>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MonoUsbTransfer 
    {
        private static readonly int OfsActualLength = Marshal.OffsetOf(typeof (libusb_transfer), "actual_length").ToInt32();
        private static readonly int OfsEndpoint = Marshal.OffsetOf(typeof (libusb_transfer), "endpoint").ToInt32();
        private static readonly int OfsFlags = Marshal.OffsetOf(typeof (libusb_transfer), "flags").ToInt32();
        private static readonly int OfsLength = Marshal.OffsetOf(typeof (libusb_transfer), "length").ToInt32();
        private static readonly int OfsPtrBuffer = Marshal.OffsetOf(typeof (libusb_transfer), "pBuffer").ToInt32();
        private static readonly int OfsPtrCallbackFn = Marshal.OffsetOf(typeof (libusb_transfer), "pCallbackFn").ToInt32();
        private static readonly int OfsPtrDeviceHandle = Marshal.OffsetOf(typeof (libusb_transfer), "deviceHandle").ToInt32();
        private static readonly int OfsPtrUserData = Marshal.OffsetOf(typeof (libusb_transfer), "pUserData").ToInt32();
        private static readonly int OfsStatus = Marshal.OffsetOf(typeof (libusb_transfer), "status").ToInt32();
        private static readonly int OfsTimeout = Marshal.OffsetOf(typeof (libusb_transfer), "timeout").ToInt32();
        private static readonly int OfsType = Marshal.OffsetOf(typeof (libusb_transfer), "type").ToInt32();
        private static readonly int OfsNumIsoPackets = Marshal.OffsetOf(typeof (libusb_transfer), "num_iso_packets").ToInt32();
        private static readonly int OfsIsoPackets = Marshal.OffsetOf(typeof (libusb_transfer), "iso_packets").ToInt32();

        private IntPtr handle;
        /// <summary>
        /// Allocate a libusb transfer with a specified number of isochronous packet descriptors 
        /// </summary>
        /// <remarks>
        /// <para>The transfer is pre-initialized for you. When the new transfer is no longer needed, it should be freed with <see cref="Free"/>.</para>
        /// <para>Transfers intended for non-isochronous endpoints (e.g. control, bulk, interrupt) should specify an iso_packets count of zero.</para>
        /// <para>For transfers intended for isochronous endpoints, specify an appropriate number of packet descriptors to be allocated as part of the transfer. The returned transfer is not specially initialized for isochronous I/O; you are still required to set the <see cref="MonoUsbTransfer.NumIsoPackets"/> and <see cref="MonoUsbTransfer.Type"/> fields accordingly.</para>
        /// <para>It is safe to allocate a transfer with some isochronous packets and then use it on a non-isochronous endpoint. If you do this, ensure that at time of submission, <see cref="MonoUsbTransfer.NumIsoPackets"/> is 0 and that type is set appropriately.</para>
        /// </remarks>
        /// <param name="numIsoPackets">number of isochronous packet descriptors to allocate.</param>
        public MonoUsbTransfer(int numIsoPackets)
        {
            handle = MonoUsbApi.AllocTransfer(numIsoPackets);
        }

        /// <summary>
        /// Creates a new wrapper for transfers allocated by <see cref="MonoUsbApi.AllocTransfer"/>,
        /// </summary>
        /// <param name="pTransfer">The pointer to the transfer that was previously allocated with<see cref="MonoUsbApi.AllocTransfer"/>. </param>
        internal MonoUsbTransfer(IntPtr pTransfer)
        {
            handle = pTransfer;
        }

        /// <summary>
        /// Gets the buffer data pointer.
        /// </summary>
        public IntPtr PtrBuffer
        {
            get { return Marshal.ReadIntPtr(handle, OfsPtrBuffer); }
            set { Marshal.WriteIntPtr(handle, OfsPtrBuffer, value); }
        }

        /// <summary>
        /// User context data to pass to the callback function.
        /// </summary>
        public IntPtr PtrUserData
        {
            get { return Marshal.ReadIntPtr(handle, OfsPtrUserData); }
            set { Marshal.WriteIntPtr(handle, OfsPtrUserData, value); }
        }

        /// <summary>
        /// Callback function pointer.
        /// </summary>
        /// <remarks>
        /// The callback function must be declared as a <see cref="MonoUsbTransferDelegate"/>.
        /// </remarks>
        public IntPtr PtrCallbackFn
        {
            get { return Marshal.ReadIntPtr(handle, OfsPtrCallbackFn); }
            set { Marshal.WriteIntPtr(handle, OfsPtrCallbackFn, value); }
        }

        /// <summary>
        /// Actual length of data that was transferred. 
        /// </summary>
        public int ActualLength
        {
            get { return Marshal.ReadInt32(handle, OfsActualLength); }
            set { Marshal.WriteInt32(handle, OfsActualLength, value); }
        }

        /// <summary>
        /// Length of the data buffer.
        /// </summary>
        public int Length
        {
            get { return Marshal.ReadInt32(handle, OfsLength); }
            set { Marshal.WriteInt32(handle, OfsLength, value); }
        }

        /// <summary>
        /// The status of the transfer.
        /// </summary>
        public MonoUsbTansferStatus Status
        {
            get { return (MonoUsbTansferStatus)Marshal.ReadInt32(handle, OfsStatus); }
            set { Marshal.WriteInt32(handle, OfsStatus, (int)value); }
        }

        /// <summary>
        /// Timeout for this transfer in millseconds.
        /// </summary>
        public int Timeout
        {
            get { return Marshal.ReadInt32(handle, OfsTimeout); }
            set { Marshal.WriteInt32(handle, OfsTimeout, value); }
        }

        /// <summary>
        /// Type of the endpoint. 
        /// </summary>
        public EndpointType Type
        {
            get { return (EndpointType)Marshal.ReadByte(handle, OfsType); }
            set { Marshal.WriteByte(handle, OfsType, (byte)value); }
        }

        /// <summary>
        /// Enpoint address.
        /// </summary>
        public byte Endpoint
        {
            get { return Marshal.ReadByte(handle, OfsEndpoint); }
            set { Marshal.WriteByte(handle, OfsEndpoint, value); }
        }

        /// <summary>
        /// A bitwise OR combination of <see cref="MonoUsbTransferFlags"/>.
        /// </summary>
        public MonoUsbTransferFlags Flags
        {
            get { return (MonoUsbTransferFlags)Marshal.ReadByte(handle, OfsFlags); }
            set { Marshal.WriteByte(handle, OfsFlags, (byte)value); }
        }

        /// <summary>
        /// Raw device handle pointer.
        /// </summary>
        public IntPtr PtrDeviceHandle
        {
            get { return Marshal.ReadIntPtr(handle, OfsPtrDeviceHandle); }
            set { Marshal.WriteIntPtr(handle, OfsPtrDeviceHandle, value); }
        }

        /// <summary>
        /// Number of isochronous packets. 
        /// </summary>
        public int NumIsoPackets
        {
            get { return Marshal.ReadInt32(handle, OfsNumIsoPackets); }
            set { Marshal.WriteInt32(handle, OfsNumIsoPackets, value); }
        }

        /// <summary>
        /// Frees this transfer.
        /// </summary>
        ///<remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="Free"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga6ab8b2cff4de9091298a06b2f4b86cd6">libusb_free_transfer()</a>.
        /// </note>
        /// <note type="warning">
        /// Calling <see cref="Free"/> on a transfer that has already been freed will result in a double free.
        /// </note> 
        /// </remarks>
        public void Free()
        {
            if (handle!=IntPtr.Zero)
            {
                MonoUsbApi.FreeTransfer(handle);
                handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets a unqiue name for this transfer.
        /// </summary>
        /// <returns>A unqiue name for this transfer.</returns>
        public String UniqueName()
        {
            String guidString = String.Format("_-EP[{0}]EP-_", handle);
            return guidString;
        }

        /// <summary>
        /// Gets a <see cref="MonoUsbIsoPacket"/> that represents the specified iso packet descriptor. 
        /// </summary>
        /// <param name="packetNumber">The iso packet descriptor to return.</param>
        /// <returns>The <see cref="MonoUsbIsoPacket"/> that represents <paramref name="packetNumber"/>.</returns>
        public MonoUsbIsoPacket IsoPacket(int packetNumber)
        {
            if (packetNumber > NumIsoPackets) throw new ArgumentOutOfRangeException("packetNumber");
            IntPtr pIsoPacket =
                new IntPtr(handle.ToInt64() + OfsIsoPackets + (packetNumber * Marshal.SizeOf(typeof(libusb_iso_packet_descriptor))));

            return new MonoUsbIsoPacket(pIsoPacket);
        }

        /// <summary>
        /// True if the transfer is allocated.
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                return (handle == IntPtr.Zero);
            }
        }
        /// <summary>
        /// Cancels this transfer.
        /// </summary>
        /// <remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="Cancel"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga685eb7731f9a0593f75beb99727bbe54">libusb_cancel_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <returns><see cref="MonoUsbError.Success"/> if the cancel succeeds, otherwise one of the other <see cref="MonoUsbError"/> codes.</returns>
        public MonoUsbError Cancel()
        {
            if (IsInvalid) return MonoUsbError.ErrorNoMem;

            return (MonoUsbError) MonoUsbApi.CancelTransfer(handle);
        }
        /// <summary>
        /// Helper function to populate the required <see cref="MonoUsbTransfer"/> properties for a bulk transfer.
        /// </summary>
        /// <remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="FillBulk"/> is similar to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#gad4ddb1a5c6c7fefc979a44d7300b95d7">libusb_fill_bulk_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="devHandle">handle of the device that will handle the transfer</param>
        /// <param name="endpoint">address of the endpoint where this transfer will be sent</param>
        /// <param name="buffer">data buffer</param>
        /// <param name="length">length of data buffer</param>
        /// <param name="callback">callback function to be invoked on transfer completion</param>
        /// <param name="userData">user data to pass to callback function</param>
        /// <param name="timeout">timeout for the transfer in milliseconds</param>
        public void FillBulk(MonoUsbDeviceHandle devHandle,
                         byte endpoint,
                         IntPtr buffer,
                         int length,
                         Delegate callback,
                         IntPtr userData,
                         int timeout)
        {
            PtrDeviceHandle = devHandle.DangerousGetHandle();
            Endpoint = endpoint;
            PtrBuffer = buffer;
            Length = length;
            PtrCallbackFn = Marshal.GetFunctionPointerForDelegate(callback);
            PtrUserData = userData;
            Timeout = timeout;
            Type = EndpointType.Bulk;
            Flags = MonoUsbTransferFlags.None;
            NumIsoPackets = 0;
            ActualLength = 0;


        }

        /// <summary>
        /// Helper function to populate the required <see cref="MonoUsbTransfer"/> properties for an interrupt transfer.
        /// </summary>
        /// <remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="FillInterrupt"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga90f53cea1124a7566df1aa1202b77510">libusb_fill_interrupt_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="devHandle">handle of the device that will handle the transfer</param>
        /// <param name="endpoint">address of the endpoint where this transfer will be sent</param>
        /// <param name="buffer">data buffer</param>
        /// <param name="length">length of data buffer</param>
        /// <param name="callback">callback function to be invoked on transfer completion</param>
        /// <param name="userData">user data to pass to callback function</param>
        /// <param name="timeout">timeout for the transfer in milliseconds</param>
        public void FillInterrupt(MonoUsbDeviceHandle devHandle,
                 byte endpoint,
                 IntPtr buffer,
                 int length,
                 Delegate callback,
                 IntPtr userData,
                 int timeout)
        {
            PtrDeviceHandle = devHandle.DangerousGetHandle();
            Endpoint = endpoint;
            PtrBuffer = buffer;
            Length = length;
            PtrCallbackFn = Marshal.GetFunctionPointerForDelegate(callback);
            PtrUserData = userData;
            Timeout = timeout;
            Type = EndpointType.Interrupt;
            Flags = MonoUsbTransferFlags.None;
        }

        /// <summary>
        /// Helper function to populate the required <see cref="MonoUsbTransfer"/> properties for an isochronous transfer.
        /// </summary>
        /// <remarks>
        /// <note type="tip">
        /// <para>Isochronous transfers are not supported on windows.</para>
        /// </note>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="FillIsochronous"/> is similar to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga30fdce8c461e851f0aa4c851014e1aa7">libusb_fill_iso_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="devHandle">handle of the device that will handle the transfer</param>
        /// <param name="endpoint">address of the endpoint where this transfer will be sent</param>
        /// <param name="buffer">data buffer</param>
        /// <param name="length">length of data buffer</param>
        /// <param name="numIsoPackets">the number of isochronous packets</param>
        /// <param name="callback">callback function to be invoked on transfer completion</param>
        /// <param name="userData">user data to pass to callback function</param>
        /// <param name="timeout">timeout for the transfer in milliseconds</param>
        public void FillIsochronous(MonoUsbDeviceHandle devHandle,
                 byte endpoint,
                 IntPtr buffer,
                 int length,int numIsoPackets,
                 Delegate callback,
                 IntPtr userData,
                 int timeout)
        {
            PtrDeviceHandle = devHandle.DangerousGetHandle();
            Endpoint = endpoint;
            PtrBuffer = buffer;
            Length = length;
            PtrCallbackFn = Marshal.GetFunctionPointerForDelegate(callback);
            PtrUserData = userData;
            Timeout = timeout;
            Type = EndpointType.Isochronous;
            Flags = MonoUsbTransferFlags.None;
            NumIsoPackets = numIsoPackets;
        }
        
        /// <summary>
        /// Convenience function to locate the position of an isochronous packet within the buffer of an isochronous transfer. 
        /// </summary>
        /// <remarks>
        /// <para>This is a thorough function which loops through all preceding packets, accumulating their lengths to find the position of the specified packet. Typically you will assign equal lengths to each packet in the transfer, and hence the above method is sub-optimal. You may wish to use <see cref="GetIsoPacketBufferSimple"/> instead.</para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="GetIsoPacketBuffer"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga7f6ea0eb35a216d19d984977e454a7b3">libusb_get_iso_packet_buffer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="packet">The packet to return the address of.</param>
        /// <returns>the base address of the packet buffer inside the transfer buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown if the packet requested is >= <see cref="NumIsoPackets"/>.</exception>
        public IntPtr GetIsoPacketBuffer(int packet)
        {
            if (packet >= NumIsoPackets) throw new ArgumentOutOfRangeException("packet", "GetIsoPacketBuffer: packet must be < NumIsoPackets");
            long offset = PtrBuffer.ToInt64();

            for (int i = 0; i < packet; i++)
                offset += IsoPacket(i).Length;
            
            return new IntPtr(offset);
        }

        /// <summary>
        /// Convenience function to locate the position of an isochronous packet within the buffer of an isochronous transfer, for transfers where each packet is of identical size.
        /// </summary>
        /// <remarks>
        /// <para>This function relies on the assumption that every packet within the transfer is of identical size to the first packet. Calculating the location of the packet buffer is then just a simple calculation: buffer + (packet_size * packet)</para>
        /// <para>Do not use this function on transfers other than those that have identical packet lengths for each packet.</para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="GetIsoPacketBufferSimple"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga3df9a28c4f5c8f1850181ddb5efd12fd">libusb_get_iso_packet_buffer_simple()</a>.
        /// </note>        
        /// </remarks>
        /// <param name="packet">The packet to return the address of.</param>
        /// <returns>the base address of the packet buffer inside the transfer buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown if the packet requested is >= <see cref="NumIsoPackets"/>.</exception>
        public IntPtr GetIsoPacketBufferSimple(int packet)
        {
            if (packet >= NumIsoPackets) throw new ArgumentOutOfRangeException("packet", "GetIsoPacketBufferSimple: packet must be < NumIsoPackets");

            return new IntPtr((PtrBuffer.ToInt64() + (IsoPacket(0).Length * packet)));

        }

        /// <summary>
        /// Convenience function to set the length of all packets in an isochronous transfer, based on the num_iso_packets field in the transfer structure. 
        /// </summary>
        /// <remarks>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="SetIsoPacketLengths"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#gacbdecd6f50093f0c1d0e72ee35ace274">libusb_set_iso_packet_lengths()</a>.
        /// </note>   
        /// </remarks>
        /// <param name="length">The length to set in each isochronous packet descriptor.</param>
        public void SetIsoPacketLengths(int length)
        {
            int packetCount = NumIsoPackets;
            for (int i = 0; i < packetCount; i++)
                IsoPacket(i).Length = length;

        }
        /// <summary>
        /// Submits this transfer.
        /// </summary>
        /// <remarks>
        /// This functions submits the USB transfer and return immediately.
        /// <note>
        /// <see cref="Submit"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#gabb0932601f2c7dad2fee4b27962848ce">libusb_submit_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <returns>
        /// <see cref="MonoUsbError.Success"/> if the submit succeeds, 
        /// otherwise one of the other <see cref="MonoUsbError"/> codes.
        /// </returns>
        public MonoUsbError Submit()
        {
            if (IsInvalid) return MonoUsbError.ErrorNoMem;
            return (MonoUsbError)MonoUsbApi.SubmitTransfer(handle);
        }

        /// <summary>
        /// Allocate a libusb transfer with a specified number of isochronous packet descriptors 
        /// </summary>
        /// <remarks>
        /// <para>The returned transfer is pre-initialized for you. When the new transfer is no longer needed, it should be freed with <see cref="Free"/>.</para>
        /// <para>Transfers intended for non-isochronous endpoints (e.g. control, bulk, interrupt) should specify an iso_packets count of zero.</para>
        /// <para>For transfers intended for isochronous endpoints, specify an appropriate number of packet descriptors to be allocated as part of the transfer. The returned transfer is not specially initialized for isochronous I/O; you are still required to set the <see cref="MonoUsbTransfer.NumIsoPackets"/> and <see cref="MonoUsbTransfer.Type"/> fields accordingly.</para>
        /// <para>It is safe to allocate a transfer with some isochronous packets and then use it on a non-isochronous endpoint. If you do this, ensure that at time of submission, <see cref="MonoUsbTransfer.NumIsoPackets"/> is 0 and that type is set appropriately.</para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="Alloc"/> is roughly equivalent to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga13cc69ea40c702181c430c950121c000">libusb_alloc_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="numIsoPackets">number of isochronous packet descriptors to allocate.</param>
        /// <returns>A newly allocated <see cref="MonoUsbTransfer"/>.</returns>
        /// <exception cref="OutOfMemoryException">If the transfer was not allocated.</exception>
        public static MonoUsbTransfer Alloc(int numIsoPackets)
        {
            IntPtr p = MonoUsbApi.AllocTransfer(numIsoPackets);
            if (p == IntPtr.Zero) throw new OutOfMemoryException("AllocTransfer");
            return new MonoUsbTransfer(p);
        }

        /// <summary>
        /// Helper function to populate the required <see cref="MonoUsbTransfer"/> properties for a control transfer.
        /// </summary>
        /// <remarks>
        /// <note type="tip">
        /// <para>Isochronous transfers are not supported on windows.</para>
        /// </note>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// <see cref="FillControl"/> is similar to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga3a8513ed87229fe2c9771ef0bf17206e">libusb_fill_control_transfer()</a>.
        /// </note>
        /// </remarks>
        /// <param name="devHandle">handle of the device that will handle the transfer</param>
        /// <param name="controlSetupHandle">the setup packet/control data to transfer.</param>
        /// <param name="callback">callback function to be invoked on transfer completion</param>
        /// <param name="userData">user data to pass to callback function</param>
        /// <param name="timeout">timeout for the transfer in milliseconds</param>
        public void FillControl(MonoUsbDeviceHandle devHandle, MonoUsbControlSetupHandle controlSetupHandle, Delegate callback, IntPtr userData, int timeout) 
        {
            PtrDeviceHandle = devHandle.DangerousGetHandle();
            Endpoint = 0;
            PtrCallbackFn = Marshal.GetFunctionPointerForDelegate(callback);
            PtrUserData = userData;
            Timeout = timeout;
            Type = EndpointType.Control;
            Flags = MonoUsbTransferFlags.None;

            IntPtr pSetupPacket = controlSetupHandle.DangerousGetHandle();
            PtrBuffer = pSetupPacket;
            MonoUsbControlSetup w = new MonoUsbControlSetup(pSetupPacket);
            Length = MonoUsbControlSetup.SETUP_PACKET_SIZE + w.Length;
        }
    }
}