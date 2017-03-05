using System;
using System.Runtime.InteropServices;
using MonoLibUsb.Transfer.Internal;

namespace MonoLibUsb.Transfer
{
    /// <summary>
    /// Wraps an iso packet structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MonoUsbIsoPacket
    {
        private static readonly int OfsActualLength = Marshal.OffsetOf(typeof(libusb_iso_packet_descriptor), "actual_length").ToInt32();
        private static readonly int OfsLength = Marshal.OffsetOf(typeof(libusb_iso_packet_descriptor), "length").ToInt32();
        private static readonly int OfsStatus = Marshal.OffsetOf(typeof(libusb_iso_packet_descriptor), "status").ToInt32();

        private IntPtr mpMonoUsbIsoPacket = IntPtr.Zero;

        /// <summary>
        /// Creates a structure that wraps an iso packet.
        /// </summary>
        /// <param name="isoPacketPtr">The pointer to the iso packet to wrap.</param>
        public MonoUsbIsoPacket(IntPtr isoPacketPtr) { mpMonoUsbIsoPacket = isoPacketPtr; }

        /// <summary>
        /// Returns the location in memory of this iso packet.
        /// </summary>
        public IntPtr PtrIsoPacket
        {
            get { return mpMonoUsbIsoPacket; }
        }
        /// <summary>
        /// Amount of data that was actually transferred. 
        /// </summary>
        public int ActualLength
        {
            get { return Marshal.ReadInt32(mpMonoUsbIsoPacket, OfsActualLength); }
            set { Marshal.WriteInt32(mpMonoUsbIsoPacket, OfsActualLength, value); }
        }
        /// <summary>
        /// Length of data to request in this packet. 
        /// </summary>
        public int Length
        {
            get { return Marshal.ReadInt32(mpMonoUsbIsoPacket, OfsLength); }
            set { Marshal.WriteInt32(mpMonoUsbIsoPacket, OfsLength, value); }
        }
        /// <summary>
        /// Status code for this packet. 
        /// </summary>
        public MonoUsbTansferStatus Status
        {
            get { return (MonoUsbTansferStatus)Marshal.ReadInt32(mpMonoUsbIsoPacket, OfsStatus); }
            set { Marshal.WriteInt32(mpMonoUsbIsoPacket, OfsStatus, (int)value); }
        }

    }
}