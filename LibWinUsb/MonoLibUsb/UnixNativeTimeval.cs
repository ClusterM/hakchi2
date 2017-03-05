using System;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace MonoLibUsb
{
    /// <summary>
    /// Unix mono.net timeval structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UnixNativeTimeval
    {
        private IntPtr mTvSecInternal;
        private IntPtr mTvUSecInternal;

        /// <summary>
        /// Default <see cref="UnixNativeTimeval"/> used by the <see cref="MonoUsbEventHandler"/> on windows platforms.
        /// </summary>
        public static UnixNativeTimeval WindowsDefault
        {
            get { return new UnixNativeTimeval(2, 0); }
        }

        /// <summary>
        /// Default <see cref="UnixNativeTimeval"/> used by the <see cref="MonoUsbEventHandler"/> on unix-like platforms.
        /// </summary>
        public static UnixNativeTimeval LinuxDefault
        {
            get { return new UnixNativeTimeval(2, 0); }
        }

        /// <summary>
        /// Default <see cref="UnixNativeTimeval"/>.
        /// </summary>
        public static UnixNativeTimeval Default
        {
            get { return Helper.IsLinux ? LinuxDefault : WindowsDefault; }
        }

        /// <summary>
        /// Timeval seconds property.
        /// </summary>
        public long tv_sec
        {
            get { return mTvSecInternal.ToInt64(); }
            set { mTvSecInternal = new IntPtr(value); }
        }

        /// <summary>
        /// Timeval milliseconds property.
        /// </summary>
        public long tv_usec
        {
            get { return mTvUSecInternal.ToInt64(); }
            set { mTvUSecInternal = new IntPtr(value); }
        }

        /// <summary>
        /// Timeval constructor.
        /// </summary>
        /// <param name="tvSec">seconds</param>
        /// <param name="tvUsec">milliseconds</param>
        public UnixNativeTimeval(long tvSec, long tvUsec)
        {
            mTvSecInternal = new IntPtr(tvSec);
            mTvUSecInternal = new IntPtr(tvUsec);
        }
    }
}