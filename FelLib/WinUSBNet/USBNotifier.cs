/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *  
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// Delegate for event handler methods handing USB events
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Details of the event</param>
    public delegate void USBEventHandler(object sender, USBEvent e);
    
    /// <summary>
    /// Event type enumeration for WinUSB events
    /// </summary>
    public enum USBEventType
    {
        /// <summary>
        /// A device has been connected to the system
        /// </summary>
        DeviceArrival,

        /// <summary>
        /// A device has been disconnected from the system
        /// </summary>
        DeviceRemoval,
    }

    /// <summary>
    /// Contains the details of a USB event
    /// </summary>
    public class USBEvent : EventArgs
    {
        /// <summary>
        /// WinUSB interface GUID of the device as specified in the WinUSBNotifier
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Device pathname that identifies the device
        /// </summary>
        public string DevicePath;

        /// <summary>
        /// Type of event that occurred
        /// </summary>
        public USBEventType Type;

        internal USBEvent(USBEventType type, Guid guid, string devicePath)
        {
            this.Guid = guid;
            this.DevicePath = devicePath;
            this.Type= type;
        }
    }

    /// <summary>
    /// Helper class to receive notifications on USB device changes such as 
    /// connecting or removing a device.
    /// </summary>
    public class USBNotifier : IDisposable
    {
        private DeviceNotifyHook _hook;
        private Guid _guid;

        /// <summary>
        /// Event triggered when a new USB device that matches the USBNotifier's GUID is connected
        /// </summary>
        public event USBEventHandler Arrival;

        /// <summary>
        /// Event triggered when a new USB device that matches the USBNotifier's GUID  is disconnected
        /// </summary>
        public event USBEventHandler Removal;

        /// <summary>
        /// The interface GUID of devices this USBNotifier will watch
        /// </summary>
        public Guid Guid
        {
            get
            {
                return _guid;
            }
        }

        /// <summary>
        /// Constructs a new USBNotifier that will watch for events on 
        /// devices matching the given interface GUID. A Windows Forms control 
        /// is needed since the notifier relies on window messages.
        /// </summary>
        /// <param name="control">A control that will be used internally for device notification messages. 
        /// You can use a Form object for example.</param>
        /// <param name="guidString">The interface GUID string of the devices to watch.</param>
        public USBNotifier(Control control, string guidString) :
            this(control, new Guid(guidString))
        {
            // Handled in other constructor
        }


        /// <summary>
        /// Constructs a new USBNotifier that will watch for events on 
        /// devices matching the given interface GUID. A Windows Forms control 
        /// is needed since the notifier relies on window messages.
        /// </summary>
        /// <param name="control">A control that will be used internally for device notification messages. 
        /// You can use a Form object for example.</param>
        /// <param name="guid">The interface GUID of the devices to watch.</param>
        public USBNotifier(Control control, Guid guid)
        {
            _guid = guid;
            _hook = new DeviceNotifyHook(this, control, _guid);
        }

        /// <summary>
        /// Triggers the arrival event
        /// </summary>
        /// <param name="devicePath">Device pathname of the device that has been connected</param>
        protected void OnArrival(string devicePath)
        {
            if (Arrival != null)
                Arrival(this, new USBEvent(USBEventType.DeviceArrival, _guid, devicePath));
        }
        /// <summary>
        /// Trigggers the removal event
        /// </summary>
        /// <param name="devicePath">Device pathname of the device that has been connected</param>
        protected void OnRemoval(string devicePath)
        {
            if (Removal != null)
                Removal(this, new USBEvent(USBEventType.DeviceRemoval, _guid, devicePath));
        }

        internal void HandleDeviceChange(Message m)
        {
            if (m.Msg != API.DeviceManagement.WM_DEVICECHANGE)
                throw new USBException("Invalid device change message."); // should not happen

            if ((int)m.WParam == API.DeviceManagement.DBT_DEVICEARRIVAL)
            {

                string devName = API.DeviceManagement.GetNotifyMessageDeviceName(m, _guid);
                if (devName != null)
                    OnArrival(devName);
            }

            if ((int)m.WParam == API.DeviceManagement.DBT_DEVICEREMOVECOMPLETE)
            {
                string devName = API.DeviceManagement.GetNotifyMessageDeviceName(m, _guid);
                if (devName != null)
                    OnRemoval(devName);
            }

        }

        /// <summary>
        /// Disposes the USBNotifier object and frees all resources. 
        /// Call this method when the object is no longer needed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Disposes the object's resources.
        /// </summary>
        /// <param name="disposing">True when dispose is called manually, false when called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hook.Dispose();
            }
        }

    }
}
