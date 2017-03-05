using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace MonoLibUsb
{
    /// <summary>
    /// Class representing a Libusb-1.0 session session handle.
    /// Session handled are wrapped in a <see cref="System.Runtime.ConstrainedExecution.CriticalFinalizerObject"/>. 
    /// </summary>
    /// <remarks>
    /// <para>The concept of individual Libusb-1.0 sessions allows for your program to use two libraries 
    /// (or dynamically load two modules) which both independently use libusb. This will prevent interference between the 
    /// individual libusb users - for example <see cref="MonoUsbApi.SetDebug"/> will not affect the other 
    /// user of the library, and <see cref="SafeHandle.Close"/> will not destroy resources that the 
    /// other user is still using.</para>
    /// <para>Sessions are created when a new <see cref="MonoUsbSessionHandle"/> instance is created and destroyed through <see cref="SafeHandle.Close"/>.</para>
    /// <para>A <see cref="MonoUsbSessionHandle"/> instance must be created before calling any other <a href="http://libusb.sourceforge.net/api-1.0/index.html">Libusb-1.0 API</a> function.</para>
    /// <para>Session handles are equivalent to a <a href="http://libusb.sourceforge.net/api-1.0/group__lib.html#ga4ec088aa7b79c4a9599e39bf36a72833">libusb_context</a>.</para>
    /// </remarks>
    public class MonoUsbSessionHandle:SafeContextHandle
    {
        private static Object sessionLOCK = new object();
        private static MonoUsbError mLastReturnCode;
        private static String mLastReturnString=String.Empty;
        private static int mSessionCount;
        private static string DLL_NOT_FOUND_LINUX = "libusb-1.0 library not found.  This is often an indication that libusb-1.0 was installed to '/usr/local/lib' and mono.net is not looking for it there. To resolve this, add the path '/usr/local/lib' to '/etc/ld.so.conf' and run 'ldconfig' as root. (http://www.mono-project.com/DllNotFoundException)";
        private static string DLL_NOT_FOUND_WINDOWS = "libusb-1.0.dll not found. If this is a 64bit operating system, ensure that the 64bit version of libusb-1.0.dll exists in the '\\Windows\\System32' directory.";

        /// <summary>
        /// If the session handle is <see cref="SafeContextHandle.IsInvalid"/>, gets the <see cref="MonoUsbError"/> status code indicating the reason.
        /// </summary>
        public static MonoUsbError LastErrorCode
        {
            get
            {
                lock (sessionLOCK)
                {
                    return mLastReturnCode;
                }
            }
        }
        /// <summary>
        /// If the session handle is <see cref="SafeContextHandle.IsInvalid"/>, gets a descriptive string for the <see cref="LastErrorCode"/>.
        /// </summary>
        public static string LastErrorString
        {
            get
            {
                lock (sessionLOCK)
                {
                    return mLastReturnString;
                }
            }
        }

        /// <summary>
        /// Creates and initialize a <a href="http://libusb.sourceforge.net/api-1.0/index.html">Libusb-1.0</a> USB session handle.
        /// </summary>
        /// <remarks>
        /// <para>A <see cref="MonoUsbSessionHandle"/> instance must be created before calling any other <a href="http://libusb.sourceforge.net/api-1.0/index.html">Libusb-1.0 API</a> function.</para>
        /// </remarks>
        public MonoUsbSessionHandle() : base(IntPtr.Zero, true) 
        {
            lock (sessionLOCK)
            {
                IntPtr pNewSession = IntPtr.Zero;
                try
                {
                    mLastReturnCode = (MonoUsbError)MonoUsbApi.Init(ref pNewSession);
                }
                catch (DllNotFoundException dllNotFound)
                {
                    if (Helper.IsLinux)
                    {
                        throw new DllNotFoundException(DLL_NOT_FOUND_LINUX, dllNotFound);
                    }
                    else
                    {
                        throw new DllNotFoundException(DLL_NOT_FOUND_WINDOWS, dllNotFound);
                    }
                }
                if ((int)mLastReturnCode < 0)
                {
                    mLastReturnString = MonoUsbApi.StrError(mLastReturnCode);
                    SetHandleAsInvalid();
                }
                else
                {
                    SetHandle(pNewSession);
                    mSessionCount++;
                }
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
                lock (sessionLOCK)
                {
                    MonoUsbApi.Exit(handle);
                    SetHandleAsInvalid();
                    mSessionCount--;
                    Debug.Print(GetType().Name + " : ReleaseHandle #{0}", mSessionCount);

                }
            }
            return true;
        }
    }
}