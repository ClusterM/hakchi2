using System;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;
using MonoLibUsb.Transfer.Internal;

namespace MonoLibUsb.Transfer
{
    /// <summary>
    /// Reads/writes a Libusb-1.0 control setup packet pointer.  Control setup packets should be allocated with <see cref="MonoUsbControlSetupHandle"/>.
    /// </summary>
    /// <remarks>
    /// <para>This class that reads and writes values directly from/to the setup packet <see cref="IntPtr"/> using <see cref="Marshal"/>.</para>
    /// <note type="tip">This type is used for asynchronous control transfers only.</note>
    /// </remarks>
    /// <seealso cref="MonoUsbControlSetupHandle"/>
    [StructLayout(LayoutKind.Sequential)]
    public class MonoUsbControlSetup
    {
        /// <summary>
        /// Size of a Libusb-1.0 setup packet.
        /// </summary>
        public static int SETUP_PACKET_SIZE = Marshal.SizeOf(typeof(libusb_control_setup));
        
        private static readonly int OfsRequestType = Marshal.OffsetOf(typeof(libusb_control_setup), "bmRequestType").ToInt32();
        private static readonly int OfsRequest = Marshal.OffsetOf(typeof(libusb_control_setup), "bRequest").ToInt32();
        private static readonly int OfsValue = Marshal.OffsetOf(typeof(libusb_control_setup), "wValue").ToInt32();
        private static readonly int OfsIndex = Marshal.OffsetOf(typeof(libusb_control_setup), "wIndex").ToInt32();
        private static readonly int OfsLength = Marshal.OffsetOf(typeof(libusb_control_setup), "wLength").ToInt32();
        private static readonly int OfsPtrData = SETUP_PACKET_SIZE;


        private IntPtr handle;

        /// <summary>
        /// Creates a <see cref="MonoUsbControlSetup"/> structure for a control setup packet pointer.
        /// </summary>
        /// <remarks>
        /// The <paramref name="pControlSetup"/> pointer must be a pointer in memory to a valid Libusb-1.0 <a href="http://libusb.sourceforge.net/api-1.0/structlibusb__control__setup.html">libusb__control__setup</a> that was allocated with <see cref="MonoUsbControlSetupHandle"/>.
        /// </remarks>
        /// <param name="pControlSetup">Pointer to the setup packet.  This will usually be <see cref="MonoUsbTransfer.PtrBuffer">MonoUsbTransfer.PtrBuffer</see></param>
        public MonoUsbControlSetup(IntPtr pControlSetup)
        {
            handle = pControlSetup;
        }

        /// <summary>
        /// The request type.
        /// </summary>
        public byte RequestType
        {
            get { return Marshal.ReadByte(handle, OfsRequestType); }
            set { Marshal.WriteByte(handle, OfsRequestType, value); }
        }

        /// <summary>
        /// The request.
        /// </summary>
        public byte Request
        {
            get { return Marshal.ReadByte(handle, OfsRequest); }
            set { Marshal.WriteByte(handle, OfsRequest, value); }
        }
        /// <summary>
        /// The wValue.
        /// </summary>
        /// <remarks>
        /// <note type="tip">The get/set accessors automatically manage the little-endian to host-endian/host-endian to little-endian conversions using the <see cref="Helper.HostEndianToLE16"/> method.</note>
        /// </remarks>
        public short Value
        {
            get { return Helper.HostEndianToLE16(Marshal.ReadInt16(handle, OfsValue)); }
            set { Marshal.WriteInt16(handle, OfsValue, Helper.HostEndianToLE16(value)); }

        }
        /// <summary>
        /// The wIndex.
        /// </summary>
        /// <remarks>
        /// <note type="tip">The get/set accessors automatically manage the little-endian to host-endian/host-endian to little-endian conversions using the <see cref="Helper.HostEndianToLE16"/> method.</note>
        /// </remarks>
        public short Index
        {
            get { return Helper.HostEndianToLE16(Marshal.ReadInt16(handle, OfsIndex)); }
            set { Marshal.WriteInt16(handle, OfsIndex, Helper.HostEndianToLE16(value)); }
        }
        /// <summary>
        /// Number of bytes to transfer. 
        /// </summary>
        /// <remarks>
        /// <note type="tip">The get/set accessors automatically manage the little-endian to host-endian/host-endian to little-endian conversions using the <see cref="Helper.HostEndianToLE16"/> method.</note>
        /// </remarks>
        public short Length
        {
            get { return Helper.HostEndianToLE16(Marshal.ReadInt16(handle, OfsLength)); }
            set { Marshal.WriteInt16(handle, OfsLength, Helper.HostEndianToLE16(value)); }
        }

        /// <summary>
        /// Gets a pointer to the control data buffer.
        /// </summary>
        /// <remarks>This is the <see cref="IntPtr"/> to the control data inside of the setup packet, not to the setup packet itself.</remarks>
        public IntPtr PtrData
        {
            get
            {
                return new IntPtr(handle.ToInt64() + OfsPtrData);
            }
        }
        /// <summary>
        /// Copies data into <see cref="PtrData"/>.
        /// </summary>
        /// <param name="data">
        /// <para>Data buffer to copy into <see cref="PtrData"/>for an output control transfer.</para>
        /// This value can be:
        /// <list type="bullet">
        /// <item>An <see cref="Array"/> of bytes or other <a href="http://msdn.microsoft.com/en-us/library/75dwhxf7.aspx">blittable</a> types.</item>
        /// <item>An already allocated, pinned <see cref="GCHandle"/>. In this case <see cref="GCHandle.AddrOfPinnedObject"/> is used for the buffer address.</item>
        /// <item>An <see cref="IntPtr"/>.</item>
        /// </list>
        /// </param>
        /// <param name="offset">The offset in <paramref name="data"/> to begin copying.</param>
        /// <param name="length">Number of to copy.</param>
        public void SetData(object data, int offset, int length)
        {
            PinnedHandle p = new PinnedHandle(data);
            Byte[] temp = new byte[length];
            Marshal.Copy(p.Handle, temp, offset, length);
            p.Dispose();
            Marshal.Copy(temp, 0, PtrData, length);
        }

        /// <summary>
        /// Gets control data as bytes.
        /// </summary>
        /// <param name="transferLength">The number of bytes to copy out of <see cref="PtrData"/>. This will usually come from <see cref="MonoUsbTransfer.ActualLength">MonoUsbTransfer.ActualLength</see>.</param>
        /// <returns>A new byte array of control data.</returns>
        public Byte[] GetData(int transferLength)
        {
            byte[] data = new byte[transferLength];
            Marshal.Copy(PtrData,data,0,data.Length);
            return data;
        }

    }
}