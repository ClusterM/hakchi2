using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI.Components
{
    public partial class EmulatorSelector : UserControl
    {
        NesMiniApplication currentApp;
        public EmulatorSelector()
        {
            InitializeComponent();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.TextChanged += ComboBox2_TextChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(currentApp!=null)
            {
                currentApp.Arguments = comboBox2.SelectedItem.ToString();
            }
            ReloadFullCommandLine();
        }

        private void ComboBox2_TextChanged(object sender, EventArgs e)
        {
            if (currentApp != null)
            {
                currentApp.Arguments = comboBox2.Text.ToString();
            }
            ReloadFullCommandLine();
        }

        private void ReloadFullCommandLine()
        {
            if(currentApp == null)
            {
                textBox1.Text = "";
            }
            else
            {
                textBox1.Text = currentApp.Command;
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            if (comboBox1.SelectedItem != null)
            {
                if (currentApp != null)
                {
                    currentApp.Command = ((Manager.EmulatorManager.Emulator)comboBox1.SelectedItem).getCommandLine(currentApp) ;
            }
                comboBox2.Items.AddRange(((Manager.EmulatorManager.Emulator)comboBox1.SelectedItem).AvailableArguments.ToArray());
            }
            if (currentApp != null)
            {
                comboBox2.SelectedItem = currentApp.Arguments;
            }
            ReloadFullCommandLine();
        }

    
        public void SetGame(NesMiniApplication app)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            comboBox1.Items.AddRange(Manager.EmulatorManager.getInstance().getEmulatorList().ToArray());


            currentApp = app;
            if (currentApp != null)
            {
                comboBox1.SelectedItem = currentApp.GetEmulator();
            }
            ReloadFullCommandLine();

        }
    }
}
