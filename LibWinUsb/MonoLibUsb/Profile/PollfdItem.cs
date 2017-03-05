using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// File descriptor for polling. <a href="http://libusb.sourceforge.net/api-1.0/structlibusb__pollfd.html#_details">More..</a>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class PollfdItem
    {
        internal PollfdItem(IntPtr pPollfd)
        {
            Marshal.PtrToStructure(pPollfd, this);
        }
        /// <summary>
        /// Numeric file descriptor.
        /// </summary>
        public readonly int fd;

        /// <summary>
        /// Event flags to poll for from poll.h. 
        /// </summary>
        public readonly short events;
    }
}
