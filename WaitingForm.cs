using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WaitingForm : Form
    {
        readonly UInt16 vid, pid;

        public WaitingForm(UInt16 vid, UInt16 pid)
        {
            InitializeComponent();
            this.vid = vid;
            this.pid = pid;
            timer.Enabled = true;
        }

        public static bool WaitForDevice(UInt16 vid, UInt16 pid)
        {
            if (Fel.DeviceExists(vid, pid)) return true;
            var form = new WaitingForm(vid, pid);
            form.ShowDialog();
            return form.DialogResult == DialogResult.OK;
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

        /*
        private void buttonDriver_Click(object sender, EventArgs e)
        {
            try
            {
                // XP?
                if (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor <= 1)
                {
                    MessageBox.Show(this, Resources.XpZadig, "Windows XP/2000", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var process = new Process();
                    var fileName = "http://zadig.akeo.ie/";
                    process.StartInfo.FileName = fileName;
                    process.Start();
                }
                else
                {
                    var process = new Process();
                    var fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "driver"), "nesmini_driver.exe");
                    process.StartInfo.FileName = fileName;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         */

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
        }

        private void labelZadigLink_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://zadig.akeo.ie/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void labelDriverFolder_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "driver"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


