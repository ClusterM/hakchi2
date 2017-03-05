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
using System.Threading;
using LibUsbDotNet.Main;

namespace MonoLibUsb
{
    /// <summary>
    /// Manages a static Libusb-1.0 <see cref="MonoUsbSessionHandle"/> and "handle_events" thread for simplified asynchronous IO.
    /// </summary>
    /// <remarks>
    /// <para>This class contains its own <see cref="MonoUsbSessionHandle"/> that is initialized with one of the overloaded <see cref="MonoUsbEventHandler.Init()">MonoUsbEventHandler.Init()</see> functions.</para>
    /// <para>This class contains a static thread that execute <see cref="MonoUsbApi.HandleEventsTimeout"/>. See the <see cref="Start"/> and <see cref="Stop"/> methods.</para>
    /// </remarks>
    public static class MonoUsbEventHandler
    {
        private static readonly ManualResetEvent mIsStoppedEvent = new ManualResetEvent(true);
        private static bool mRunning;
        private static MonoUsbSessionHandle mSessionHandle;
        internal static Thread mUsbEventThread;
        private static ThreadPriority mPriority = ThreadPriority.Normal;

        private static UnixNativeTimeval mWaitUnixNativeTimeval;

        /// <summary>
        /// Gets the session handle. 
        /// </summary>
        /// <remarks>
        /// Used for MonoLibUsb members that require the <see cref="MonoUsbSessionHandle"/> parameter.
        /// </remarks>
        public static MonoUsbSessionHandle SessionHandle
        {
            get { return mSessionHandle; }
        }

        /// <summary>
        /// False if the handle events thread is running.
        /// </summary>
        public static bool IsStopped
        {
            get { return mIsStoppedEvent.WaitOne(0, false); }
        }

        /// <summary>
        /// Thread proirity to use for the handle events thread.
        /// </summary>
        public static ThreadPriority Priority
        {
            get { return mPriority; }
            set {mPriority=value;}
        }

        /// <summary>
        /// Stops the handle events thread and closes the session handle.
        /// </summary>
        public static void Exit()
        {
            Stop(true);
            if (mSessionHandle == null) return;

            if (mSessionHandle.IsInvalid) return;
            mSessionHandle.Close();
            mSessionHandle = null;
        }

        private static void HandleEventFn(object oHandle)
        {
            MonoUsbSessionHandle sessionHandle = oHandle as MonoUsbSessionHandle;

            mIsStoppedEvent.Reset();

            while (mRunning)
                MonoUsbApi.HandleEventsTimeout(sessionHandle, ref mWaitUnixNativeTimeval);

            mIsStoppedEvent.Set();
        }


        /// <summary>
        /// Initializes the <see cref="SessionHandle"/> and sets a custom polling interval.
        /// </summary>
        /// <param name="tvSec">polling interval seconds</param>
        /// <param name="tvUsec">polling interval milliseconds</param>
        /// <seealso cref="Init()"/>
        /// <seealso cref="SessionHandle"/> 
        public static void Init(long tvSec, long tvUsec) { Init(new UnixNativeTimeval(tvSec, tvUsec)); }

        /// <summary>
        /// Initializes the <see cref="SessionHandle"/>.
        /// </summary>
        /// <remarks>
        /// <para>If the session has already been initialized, this method does nothing.</para>
        /// <para>The handle events thread is not started until the <see cref="Start"/> method is called.</para>
        /// <para>Uses the MonoLibUsb <see cref="UnixNativeTimeval.Default"/> polling interval for <see cref="MonoUsbApi.HandleEventsTimeout"/>.</para>
        /// </remarks>
        public static void Init() { Init(UnixNativeTimeval.Default); }

        private static void Init(UnixNativeTimeval unixNativeTimeval)
        {
            if (IsStopped && !mRunning && mSessionHandle==null)
            {
                mWaitUnixNativeTimeval = unixNativeTimeval;
                mSessionHandle=new MonoUsbSessionHandle();
                if (mSessionHandle.IsInvalid)
                {
                    mSessionHandle = null;
                    throw new UsbException(typeof (MonoUsbApi), String.Format("Init:libusb_init Failed:Invalid Session Handle"));
                }
            }
        }

        /// <summary>
        /// Starts the handle events thread.
        /// </summary>
        /// <remarks>
        /// <para>If the thread is already running, this method does nothing.</para>
        /// <para>
        /// Using a seperate thread which executes <see cref="MonoUsbApi.HandleEventsTimeout"/> can simplify asynchronous I/O
        /// and improve performance in multi-threaded applications which use multiple endpoints.
        /// </para>
        /// </remarks>
        /// <returns>
        /// True if the thread is started or is already running.
        /// </returns>
        public static bool Start()
        {
            if (IsStopped && !mRunning && mSessionHandle!=null)
            {
                mRunning = true;
                mUsbEventThread = new Thread(HandleEventFn);
                mUsbEventThread.Priority = mPriority;
                mUsbEventThread.Start(mSessionHandle);

            }
            return true;
        }

        /// <summary>
        /// Stops the handle events thread.
        /// </summary>
        /// <remarks>
        /// <para>Calling this method when the thread is not running will have no affect.</para>
        /// <note type="warning">
        /// If the thread is running, this method must be called before the application exits.
        /// Failure to do so will cause the application to hang.
        /// </note>
        /// </remarks>
        /// <param name="bWait">If true, wait for the thread to exit before returning.</param>
        public static void Stop(bool bWait)
        {
            if (!IsStopped && mRunning)
            {
                mRunning = false;

                if (bWait)
                {
                    bool bSuccess = mUsbEventThread.Join((int)((mWaitUnixNativeTimeval.tv_sec * 1000 + mWaitUnixNativeTimeval.tv_usec) * 1.2));
                    //bool bSuccess = mIsStoppedEvent.WaitOne((int)((mWaitUnixNativeTimeval.tv_sec * 1000 + mWaitUnixNativeTimeval.tv_usec) * 1.2), false);
                    if (!bSuccess)
                    {
                        mUsbEventThread.Abort();
                        throw new UsbException(typeof(MonoUsbEventHandler), "Critical timeout failure! MonoUsbApi.HandleEventsTimeout did not return within the allotted time.");
                        //LibUsbDotNet.UsbError.Error(ErrorCode.UnknownError, 0, "Critical timeout failure!", typeof(MonoUsbEventHandler));
                        //mIsStoppedEvent.Set();
                    }
                }
                mUsbEventThread = null;

            }
        }
    }
}