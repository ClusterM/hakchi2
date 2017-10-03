// Copyright © 2006-2009 Travis Robinson. All rights reserved.
// 
// website: sourceforge.net/projects/libusbdotnet/
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
namespace LibUsbDotNet.Main
{
    /// <summary>Contains the locking strategy for a <see cref="UsbDevice"/> and it's associated endpoints.</summary>
    /// <remarks>Locking styles are use to change the way proccess/threads are allowed to communcate with a USB device and/or endpoints.
    /// See the <see cref="DeviceLockType"/>, <see cref="ControlEpLockType"/>, and <see cref="DataEpLockType"/> enumerations for a description of the various locking styles.
    /// </remarks> 
    public class UsbLockStyle
    {
        private ControlEpLockType mControlEpLock;
        private DataEpLockType mDataEpLock;
        private DeviceLockType mDeviceLockType;
        private int mEndpointControlTimeout;
        private int mEndpointLockTimeout;

        /// <summary>
        /// Create a device lock style class.
        /// </summary>
        /// <param name="deviceLockType">See <see cref="DeviceLockType"/>.</param>
        /// <param name="controlEpLockType">See <see cref="ControlEpLock"/>.</param>
        /// <param name="dataEpLockType">See <see cref="DataEpLock"/>.</param>
        public UsbLockStyle(DeviceLockType deviceLockType, ControlEpLockType controlEpLockType, DataEpLockType dataEpLockType)
            : this(deviceLockType, controlEpLockType, dataEpLockType, 1000, 1000)
        {
        }

        /// <summary>
        /// Create a device lock style class.
        /// </summary>
        /// <param name="deviceLockType">See <see cref="DeviceLockType"/>.</param>
        /// <param name="controlEpLockType">See <see cref="ControlEpLock"/>.</param>
        /// <param name="dataEpLockType">See <see cref="DataEpLock"/>.</param>
        /// <param name="endpoint0Timeout">Number of milliseconds to wait for an endpoint 0 lock before returning a timeout errorcode.</param>
        /// <param name="endpointLockTimeout">Number of milliseconds to wait for an endpoint lock before returning a timeout errorcode.</param>
        public UsbLockStyle(DeviceLockType deviceLockType,
                            ControlEpLockType controlEpLockType,
                            DataEpLockType dataEpLockType,
                            int endpoint0Timeout,
                            int endpointLockTimeout)
        {
            mDeviceLockType = deviceLockType;
            mControlEpLock = controlEpLockType;
            mDataEpLock = dataEpLockType;
            mEndpointControlTimeout = endpoint0Timeout;
            mEndpointLockTimeout = endpointLockTimeout;
        }

        /// <summary>
        /// Locking strategy for the device. See <see cref="DeviceLockType"/> for more information.
        /// </summary>
        public DeviceLockType DeviceLockType
        {
            get
            {
                return mDeviceLockType;
            }
            set
            {
                mDeviceLockType = value;
            }
        }

        /// <summary>
        /// Locking strategy for <see cref="UsbDevice"/> Endpoint0 operations. This property will generally always be <see cref="ControlEpLockType.None"/>, See <see cref="ControlEpLockType"/> for more information.
        /// </summary>
        public ControlEpLockType ControlEpLock
        {
            get
            {
                return mControlEpLock;
            }
            set
            {
                mControlEpLock = value;
            }
        }

        /// <summary>
        /// Locking strategy for the <see cref="UsbDevice"/> endpoint operations. See <see cref="DataEpLockType"/> for more information.
        /// </summary>
        public DataEpLockType DataEpLock
        {
            get
            {
                return mDataEpLock;
            }
            set
            {
                mDataEpLock = value;
            }
        }

        /// <summary>
        /// Timeout value used when attempting to aquire an <see cref="ControlEpLockType"/> when <see cref="ControlEpLock"/> is set to a value other than <see cref="ControlEpLockType.None"/>.
        /// </summary>
        public int EndpointControlTimeout
        {
            get
            {
                return mEndpointControlTimeout;
            }
            set
            {
                mEndpointControlTimeout = value;
            }
        }

        /// <summary>
        /// Maximum time(ms) to wait for an endpoint to become idle before returning a <see cref="ErrorCode.EndpointLockTimedOut"/> error code.
        /// </summary>
        /// <remarks>
        /// This property has no affect unless the <see cref="UsbDevice.LockStyle"/> includes the <see cref="DataEpLockType.Locked"/> enumeration.
        /// </remarks>
        public int EndpointLockTimeout
        {
            get
            {
                return mEndpointLockTimeout;
            }
            set
            {
                mEndpointLockTimeout = value;
            }
        }
    }
}