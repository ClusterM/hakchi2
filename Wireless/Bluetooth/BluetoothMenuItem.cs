using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Wireless.Bluetooth
{
    class BluetoothMenuItem: ToolStripMenuItem
    {
        private Control ParentControl;
        private BluetoothControl control = new BluetoothControl();
        private readonly IReadOnlyCollection<BluetoothControl.BluetoothDeviceData.DataType> interestedDataTypes = new BluetoothControl.BluetoothDeviceData.DataType[] {
            BluetoothControl.BluetoothDeviceData.DataType.Name,
            BluetoothControl.BluetoothDeviceData.DataType.Address,
            BluetoothControl.BluetoothDeviceData.DataType.Paired,
            BluetoothControl.BluetoothDeviceData.DataType.Pairable
        };

        private ToolStripMenuItem noDevicesDetectedItem;
        private Font headingFont;

        public struct MenuItemTag
        {
            public BluetoothControl.BluetoothDeviceData Device;
            public BluetoothControl Control;
        }

        public BluetoothMenuItem()
        {
            ParentControl = Parent;
            noDevicesDetectedItem = new ToolStripMenuItem()
            {
                Text = Properties.Resources.NoDevicesDetected,
                Enabled = false
            };
            headingFont = new Font(noDevicesDetectedItem.Font.FontFamily, noDevicesDetectedItem.Font.SizeInPoints, FontStyle.Bold);
            hakchi.OnConnected += Shell_Connected;
            hakchi.OnDisconnected += Shell_Disconnected;

            this.DropDownOpened += BluetoothMenuItem_DropDownOpened;
            this.DropDownClosed += BluetoothMenuItem_DropDownClosed;

            this.DropDownItems.Clear();
            this.DropDownItems.Add(noDevicesDetectedItem);

            control.OnData += Control_OnData;
        }

        private void BluetoothMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            ParentControl = Parent;
            if (!control.Connected)
            {
                control.StartListener(false);
            }
            control.ProbeDevices();
            control.SetScan(true);
            this.ParentControl?.Invoke(new Action(PopulateMenu));
        }

        private void BluetoothMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            control.SetScan(false);
        }

        private void Control_OnData(BluetoothControl.EventType eventType, BluetoothControl.BluetoothDeviceData data, BluetoothControl.BluetoothDeviceData.DataType dataType = BluetoothControl.BluetoothDeviceData.DataType.Undefined)
        {
            try
            {
                if (eventType == BluetoothControl.EventType.Delete || eventType == BluetoothControl.EventType.New)
                    this.ParentControl?.Invoke(new Action(PopulateMenu));

                if (eventType == BluetoothControl.EventType.Change && interestedDataTypes.Contains(dataType)
                )
                    this.ParentControl?.Invoke(new Action(PopulateMenu));
            }
            catch (Exception) { }
        }

        private void PopulateMenu()
        {
            var control = this.control;

            if (control == null)
            {
                this.DropDownItems.Clear();
                this.DropDownItems.Add(noDevicesDetectedItem);
                return;
            }

            List<ToolStripItem> menuItems = new List<ToolStripItem>();
            
            var controllers = control.Devices.Values.Where(e => e.Type == BluetoothControl.BluetoothDeviceType.Controller).OrderBy(e => e.Name);
            var devices = control.Devices.Values.Where(e => e.Type == BluetoothControl.BluetoothDeviceType.Device).OrderBy(e => e.Name);
            var pairedDevices = devices.Where(e => e.Paired == true).OrderBy(e => e.Name);
            var detectedDevices = devices.Where(e => e.Paired == false && e.Trusted == false).OrderBy(e => e.Name);
            var trustedDevices = devices.Where(e => e.Paired == false && e.Trusted == true).OrderBy(e => e.Name);

            if (devices.Count() == 0 && controllers.Count() == 0)
            {
                this.DropDownItems.Clear();
                this.DropDownItems.Add(noDevicesDetectedItem);
                return;
            }

            if (controllers.Count() > 0)
            {
                menuItems.Add(new ToolStripMenuItem()
                {
                    Text = Properties.Resources.BluetoothAdapters,
                    Enabled = false,
                    Font = headingFont
                });

                foreach (var controller in controllers)
                {
                    menuItems.Add(new ToolStripMenuItem()
                    {
                        Text = controller.Name == null ? controller.Address : $"{controller.Name} ({controller.Address})",
                        Enabled = false
                    });
                }

                menuItems.Add(new ToolStripSeparator());
            }

            if (pairedDevices.Count() > 0)
            {
                menuItems.Add(new ToolStripMenuItem()
                {
                    Text = Properties.Resources.PairedDevicesClickToUnpair,
                    Enabled = false,
                    Font = headingFont
                });

                foreach (var device in pairedDevices)
                {
                    menuItems.Add(new ToolStripMenuItem(device.Name == null ? device.Address : $"{device.Name} ({device.Address})", null, Unpair_Click)
                    {
                        Tag = new MenuItemTag()
                        {
                            Device = device,
                            Control = control
                        }
                    });
                }

                menuItems.Add(new ToolStripSeparator());
            }

            if (detectedDevices.Count() > 0)
            {
                menuItems.Add(new ToolStripMenuItem()
                {
                    Text = Properties.Resources.DetectedDevicesClickToPair,
                    Enabled = false,
                    Font = headingFont
                });

                foreach (var device in detectedDevices)
                {
                    menuItems.Add(new ToolStripMenuItem(device.Name == null ? device.Address : $"{device.Name} ({device.Address})", null, Pair_Click)
                    {
                        Tag = new MenuItemTag()
                        {
                            Device = device,
                            Control = control
                        }
                    });
                }

                menuItems.Add(new ToolStripSeparator());
            }

            if (trustedDevices.Count() > 0)
            {
                menuItems.Add(new ToolStripMenuItem()
                {
                    Text = Properties.Resources.TrustedNotPairedClickToRemove,
                    Enabled = false,
                    Font = headingFont
                });

                foreach (var device in trustedDevices)
                {
                    menuItems.Add(new ToolStripMenuItem(device.Name == null ? device.Address : $"{device.Name} ({device.Address})", null, Unpair_Click)
                    {
                        Tag = new MenuItemTag()
                        {
                            Device = device,
                            Control = control
                        }
                    });
                }

                menuItems.Add(new ToolStripSeparator());
            }

            if (devices.Count() > 0)
            {
                menuItems.Add(new ToolStripMenuItem("Remove all devices", null, RemoveAll_Click));
                menuItems.Add(new ToolStripSeparator());
            }

            menuItems.RemoveAt(menuItems.Count - 1);

            this.DropDownItems.Clear();
            this.DropDownItems.AddRange(menuItems.ToArray());
        }

        private void RemoveAll_Click(object sender, EventArgs e) => control.RemoveAllDevices();

        private void Pair_Click(object sender, EventArgs e)
        {
            var tag = (MenuItemTag)(((ToolStripMenuItem)sender).Tag);

            tag.Device.Trusting = true;
            tag.Device.Pairing = false;
            tag.Control.TrustDevice(tag.Device.Address);
        }

        private void Unpair_Click(object sender, EventArgs e)
        {
            var tag = (MenuItemTag)(((ToolStripMenuItem)sender).Tag);

            tag.Control.DisconnectDevice(tag.Device.Address);
            tag.Control.UntrustDevice(tag.Device.Address);
            tag.Control.RemoveDevice(tag.Device.Address);
        }

        public void Shell_Connected(ISystemShell shell)
        {
            Visible = !hakchi.MinimalMemboot && hakchi.CanInteract && shell.Execute("which bluetoothctl") == 0;
        }

        public void Shell_Disconnected()
        {
            Visible = false;
        }
    }
}
