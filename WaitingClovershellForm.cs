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
    public partial class WaitingClovershellForm : Form
    {
        public WaitingClovershellForm()
        {
            InitializeComponent();
            timer.Enabled = true;
        }

        public static bool WaitForDevice()
        {
            if (DeviceExists()) return true;
            var form = new WaitingClovershellForm();
            form.ShowDialog();
            return form.DialogResult == DialogResult.OK;
        }

        static bool DeviceExists()
        {
            return MainForm.Clovershell.IsOnline;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (DeviceExists())
            {
                DialogResult = DialogResult.OK;
                timer.Enabled = false;
            }
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DeviceExists())
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


