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
using System.Text;
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
           if (!Fel.DeviceExists(vid, pid))
           {
               var form = new WaitingForm(vid, pid);
               form.ShowDialog();
               return form.DialogResult == DialogResult.OK;
           }
           return true;
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
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
        }        
    }

}
