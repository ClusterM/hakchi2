using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WaitingShellCycleForm : Form
    {
        public WaitingShellCycleForm(int maxWaiting)
        {
            InitializeComponent();
            hakchi.OnDisconnected += Hakchi_OnDisconnected;
            hakchi.OnConnected += Hakchi_OnConnected;
            this.maxWaiting = maxWaiting;
            this.startTime = DateTime.Now;

            this.pictureBox1.Image = hakchi.Connected ? Resources.sign_sync : Resources.sign_sync_off;
            this.timer1.Enabled = true;
        }

        public static DialogResult WaitForDevice(IWin32Window owner, int maxWaiting = -1)
        {
            var form = new WaitingShellCycleForm(maxWaiting);
            var result = form.ShowDialog(owner);
            return result;
        }

        bool hasDisconnected = false;
        bool hasReconnected = false;
        int maxWaiting;
        DateTime startTime;

        private void Hakchi_OnConnected(ISystemShell caller)
        {
            this.Invoke(new Action(() =>
            {
                this.pictureBox1.Image = Resources.sign_check;
                this.hasReconnected = true;
                timer1.Enabled = false;

                progressBarEx1.Value = 100;
                timer1.Interval = 1000;
                timer1.Enabled = true;
            }));
        }

        private void Hakchi_OnDisconnected()
        {
            this.Invoke(new Action(() => {
                this.pictureBox1.Image = Resources.sign_sync_off;
                this.hasDisconnected = true;
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (hasReconnected)
            {
                DialogResult = DialogResult.OK;
                return;
            }

            double delay = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            if (delay > maxWaiting)
            {
                DialogResult = DialogResult.No;
                return;
            }

            progressBarEx1.Value = (int)(delay / maxWaiting * 100);
        }

        private void WaitingShellCycle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK || DialogResult == DialogResult.No)
            {
                return;
            }

            if (!hasReconnected)
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

        private void WaitingShellCycle_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            hakchi.OnConnected -= Hakchi_OnConnected;
            hakchi.OnDisconnected -= Hakchi_OnDisconnected;
        }
    }
}
