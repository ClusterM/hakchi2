using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ConsoleSelectDialog : Form
    {
        public ConsoleSelectDialog()
        {
            InitializeComponent();
        }

        private void buttonNes_Click(object sender, EventArgs e)
        {
            ConfigIni.ConsoleType = MainForm.ConsoleType.NES;
            Close();
        }

        private void buttonFamicom_Click(object sender, EventArgs e)
        {
            ConfigIni.ConsoleType = MainForm.ConsoleType.Famicom;
            Close();
        }

        private void buttonSnes_Click(object sender, EventArgs e)
        {
            ConfigIni.ConsoleType = MainForm.ConsoleType.SNES;
            Close();
        }

        private void buttonSuperFamicom_Click(object sender, EventArgs e)
        {
            ConfigIni.ConsoleType = MainForm.ConsoleType.SuperFamicom;
            Close();
        }
    }
}
