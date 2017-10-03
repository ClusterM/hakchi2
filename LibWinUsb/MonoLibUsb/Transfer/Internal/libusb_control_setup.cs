using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace MonoLibUsb.Transfer.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal class libusb_control_setup
    {

        /** Request type. Bits 0:4 determine recipient, see
         * \ref libusb_request_recipient. Bits 5:6 determine type, see
         * \ref libusb_request_type. Bit 7 determines data transfer direction, see
         * \ref libusb_endpoint_direction.
         */
        public readonly byte bmRequestType;

        /** Request. If the type bits of bmRequestType are equal to
         * \ref libusb_request_type::LIBUSB_REQUEST_TYPE_STANDARD
         * "LIBUSB_REQUEST_TYPE_STANDARD" then this field refers to
         * \ref libusb_standard_request. For other cases, use of this field is
         * application-specific. */
        public readonly byte bRequest;

        /** Value. Varies according to request */
        public readonly short wValue;

        /** Index. Varies according to request, typically used to pass an index
         * or offset */
        public readonly short wIndex;

        /** Number of bytes to transfer */
        public readonly short wLength;

    }
}