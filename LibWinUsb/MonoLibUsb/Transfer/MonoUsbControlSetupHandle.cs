using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace MonoLibUsb.Transfer
{
    /// <summary>
    /// Allocates memory and fills an asynchronous control setup packet. 
    /// </summary>
    /// <remarks>
    /// <note type="tip">This type is used for asynchronous control transfers only.</note>
    /// </remarks>
    /// <seealso cref="MonoUsbControlSetup"/>
    public class MonoUsbControlSetupHandle:SafeContextHandle
    {
        private MonoUsbControlSetup mSetupPacket;
        /// <summary>
        /// Allocates memory and sets up a control setup packet. Copies control data into the control data buffer
        /// </summary>
        /// <remarks>
        /// <para>This constructor is used when <paramref name="requestType"/> has the <see cref="UsbCtrlFlags.Direction_In"/> flag and this request will contain control data (more than just the setup packet).</para>
        /// <para>Allocates <see cref="MonoUsbControlSetup.SETUP_PACKET_SIZE"/> + <paramref name="data"/>.Length for the setup packet. The setup packet is stored first then the control data.</para>
        /// <para>The <paramref name="data"/> array is copied into the setup packet starting at <see cref="MonoUsbControlSetup.SETUP_PACKET_SIZE"/>.</para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// This contructor is similar to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga5447311149ec2bd954b5f1a640a8e231">libusb_fill_control_setup()</a>.
        /// </note>
        /// </remarks>
        /// <param name="requestType">The request type field for the setup packet.</param>
        /// <param name="request">The request field for the setup packet.</param>
        /// <param name="value">The value field for the setup packet</param>
        /// <param name="index">The index field for the setup packet.</param>
        /// <param name="data">The control data buffer to copy into the setup packet.</param>
        /// <param name="length">Size of <paramref name="data"/> in bytes.  This value is also used for the wLength field of the setup packet.</param>
        public MonoUsbControlSetupHandle(byte requestType, byte request, short value, short index, object data, int length)
            : this(requestType, request, value, index, (short)(ushort)length)
                {
                    if (data != null)
                        mSetupPacket.SetData(data, 0, length);
                }

        /// <summary>
        /// Allocates memory and sets up a control setup packet.
        /// </summary>
        /// <remarks>
        /// <para>This constructor is used when:
        /// <list type="bullet">
        /// <item><paramref name="requestType"/> has the <see cref="UsbCtrlFlags.Direction_In"/> flag and this request will not contain extra data (just the setup packet).</item>
        /// <item><paramref name="requestType"/> does not have the <see cref="UsbCtrlFlags.Direction_In"/> flag.</item>
        /// </list>
        /// </para>
        /// <note title="Libusb-1.0 API Note:" type="cpp">
        /// This contructor is similar to
        /// <a href="http://libusb.sourceforge.net/api-1.0/group__asyncio.html#ga5447311149ec2bd954b5f1a640a8e231">libusb_fill_control_setup()</a>.
        /// </note>
        /// <para>Allocates <see cref="MonoUsbControlSetup.SETUP_PACKET_SIZE"/> + <paramref name="length"/> for the setup packet. The setup packet is stored first then the control data.</para>
        /// </remarks>
        /// <param name="requestType">The request type field for the setup packet.</param>
        /// <param name="request">The request field for the setup packet.</param>
        /// <param name="value">The value field for the setup packet</param>
        /// <param name="index">The index field for the setup packet.</param>
        /// <param name="length">The length to allocate for the data portion of the setup packet.</param>
        public MonoUsbControlSetupHandle(byte requestType, byte request, short value, short index, short length)
            :base(IntPtr.Zero,true)
        {
            ushort wlength = (ushort) length;
            int packetSize;
            if (wlength > 0)
                packetSize = MonoUsbControlSetup.SETUP_PACKET_SIZE + wlength + (IntPtr.Size - (wlength % IntPtr.Size));
            else
                packetSize = MonoUsbControlSetup.SETUP_PACKET_SIZE;
                
            IntPtr pConfigMem = Marshal.AllocHGlobal(packetSize);
            if (pConfigMem == IntPtr.Zero) throw new OutOfMemoryException(String.Format("Marshal.AllocHGlobal failed allocating {0} bytes", packetSize));
            SetHandle(pConfigMem);

            mSetupPacket = new MonoUsbControlSetup(pConfigMem);

            mSetupPacket.RequestType = requestType;
            mSetupPacket.Request = request;
            mSetupPacket.Value = value;
            mSetupPacket.Index = index;
            mSetupPacket.Length = (short) wlength;

        }

        /// <summary>
        /// Returns the <see cref="MonoUsbControlSetup"/> for this handle.
        /// </summary>
        public MonoUsbControlSetup ControlSetup
        {
            get
            {
                return mSetupPacket;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        { 
            if (!IsInvalid)
            {
                Marshal.FreeHGlobal(handle);
                SetHandleAsInvalid();
                Debug.Print(GetType().Name + " : ReleaseHandle");
            }
            return true;
        }
    }
}
