using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WaitingShellForm : Form
    {
        public WaitingShellForm()
        {
            InitializeComponent();
            buttonDriver.Left = labelDriver.Left + labelDriver.Width;
            if (WaitingFelForm.DriverInstalled() )
            {
                labelDriver.Visible = false;
                buttonDriver.Visible = false;
            }
            timer.Enabled = true;
        }

        public static bool WaitForDevice(IWin32Window owner)
        {
            if (DeviceExists()) return true;
            var form = new WaitingShellForm();
            form.ShowDialog(owner);
            return form.DialogResult == DialogResult.OK;
        }

        bool hasConnected = false;

        static bool DeviceExists()
        {
            return hakchi.Shell.IsOnline;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!hasConnected)
            {
                if (DeviceExists())
                {
                    hasConnected = true;
                    timer.Enabled = false;
                    pictureBox1.Image = Resources.sign_light_bulb;
                    timer.Interval = 1000;
                    timer.Enabled = true;
                }
            }
            else
            {
                timer.Enabled = false;
                DialogResult = DialogResult.OK;
            }
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DeviceExists())
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
                buttonDriver.Enabled = WaitingFelForm.InstallDriver() != 0;
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


