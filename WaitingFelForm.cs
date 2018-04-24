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
                if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.DoYouWantCancel, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button2) == Tasks.MessageForm.Button.No)
                {
                    e.Cancel = true;
                }
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
                var fileName = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "driver"), "nesmini_driver.exe");
                process.StartInfo.FileName = fileName;
                process.Start();
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
        }
    }
}


