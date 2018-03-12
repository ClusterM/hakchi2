using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Resources;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectConsoleDialog : Form
    {
        private MainForm parent;

        public SelectConsoleDialog(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            timer1.Enabled = true;
        }

        private void buttonNes_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (MainForm.DetectedBaseConsole == null)
                ConfigIni.Instance.ConsoleType = MainForm.ConsoleType.NES;
            Close();
        }

        private void buttonFamicom_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (MainForm.DetectedBaseConsole == null)
                ConfigIni.Instance.ConsoleType = MainForm.ConsoleType.Famicom;
            Close();
        }

        private void buttonSnes_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (MainForm.DetectedBaseConsole == null)
                ConfigIni.Instance.ConsoleType = MainForm.ConsoleType.SNES;
            Close();
        }

        private void buttonSuperFamicom_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (MainForm.DetectedBaseConsole == null)
                ConfigIni.Instance.ConsoleType = MainForm.ConsoleType.SuperFamicom;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MainForm.DetectedBaseConsole != null)
            {
                timer1.Enabled = false;
                labelSelectConsole.Text = string.Format(Resources.DetectedConsole, parent.GetConsoleTypeName(MainForm.DetectedBaseConsole));
                buttonFamicom.Enabled = MainForm.DetectedBaseConsole == MainForm.ConsoleType.Famicom;
                buttonNes.Enabled = MainForm.DetectedBaseConsole == MainForm.ConsoleType.NES;
                buttonSnes.Enabled = MainForm.DetectedBaseConsole == MainForm.ConsoleType.SNES;
                buttonSuperFamicom.Enabled = MainForm.DetectedBaseConsole == MainForm.ConsoleType.SuperFamicom;
            }

        }

        private void SelectConsoleDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!checkBox1.Checked)
            {
                parent.ResetOriginalGamesForAllSystems();
            }
        }
    }
}
