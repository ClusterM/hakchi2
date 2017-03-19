using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WaitingFelForm : Form
    {
        readonly UInt16 vid, pid;

        public WaitingFelForm(UInt16 vid, UInt16 pid)
        {
            InitializeComponent();
            buttonDriver.Left = label6.Left + label6.Width;
            this.vid = vid;
            this.pid = pid;
            timer.Enabled = true;
        }

        public static bool WaitForDevice(UInt16 vid, UInt16 pid, IWin32Window owner)
        {
            if (Fel.DeviceExists(vid, pid)) return true;
            var form = new WaitingFelForm(vid, pid);
            form.ShowDialog(owner);
            return form.DialogResult == DialogResult.OK;
        }

        static bool DeviceExists(UInt16 vid, UInt16 pid)
        {
            try
            {
                using (var fel = new Fel())
                {
                    fel.Open(vid, pid);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Fel.DeviceExists(vid, pid))
            {
                DialogResult = DialogResult.OK;
                timer.Enabled = false;
            }
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Fel.DeviceExists(vid, pid))
            {
                if (MessageBox.Show(this, Resources.DoYouWantCancel, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
                else
                {
                    DialogResult = DialogResult.Abort;
                }
            }
        }

        private void buttonDriver_Click(object sender, EventArgs e)
        {
            try
            {
                var process = new Process();
                var fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "driver"), "nesmini_driver.exe");
                process.StartInfo.FileName = fileName;
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
        }
    }
}


