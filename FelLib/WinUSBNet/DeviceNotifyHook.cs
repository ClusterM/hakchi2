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
    internal class DeviceNotifyHook : NativeWindow, IDisposable
    {
        // http://msdn.microsoft.com/en-us/library/system.windows.forms.nativewindow.aspx

        // TODO: disposed exception when disposed

        private Control _parent;
        private USBNotifier _notifier;
        private Guid _guid;
        private IntPtr _notifyHandle;

        public DeviceNotifyHook(USBNotifier notifier, Control parent, Guid guid)
        {
            _parent = parent;
            _guid = guid;
            _parent.HandleCreated += new EventHandler(this.OnHandleCreated);
            _parent.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
            _notifier = notifier;
        }

        ~DeviceNotifyHook()
        {
            Dispose(false);
        }

        // Listen for the control's window creation and then hook into it.
        internal void OnHandleCreated(object sender, EventArgs e)
        {
            try
            {
                // Window is now created, assign handle to NativeWindow.
                IntPtr handle = ((Control)sender).Handle;
                AssignHandle(handle);

                if (_notifyHandle != IntPtr.Zero)
                {
                    API.DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
                    _notifyHandle = IntPtr.Zero;
                }
                API.DeviceManagement.RegisterForDeviceNotifications(handle, _guid, ref _notifyHandle);
            }
            catch (API.APIException ex)
            {
                throw new USBException("Failed to register new window handle for device notification.", ex);
            }
        }

        internal void OnHandleDestroyed(object sender, EventArgs e)
        {
            try
            {
                // Window was destroyed, release hook.
                ReleaseHandle();
                if (_notifyHandle != IntPtr.Zero)
                {
                    API.DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
                    _notifyHandle = IntPtr.Zero;
                }
            }
            catch (API.APIException ex)
            {
                throw new USBException("Failed to unregister destroyed window handle for device notification.", ex);
            }
        }


        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages

            switch (m.Msg)
            {
                case API.DeviceManagement.WM_DEVICECHANGE:
                    _notifier.HandleDeviceChange(m);
                    break;
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // clean managed resources
                
                // do not clean the notifier here. the notifier owns and will dispose this object.
            }
            if (_notifyHandle != IntPtr.Zero)
            {
                API.DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
				_notifyHandle = IntPtr.Zero;
            }
        }

    }
}
