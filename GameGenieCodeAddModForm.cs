using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.Famicom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeAddModForm : Form
    {
        NesGame FGame = null;

        public GameGenieCodeAddModForm()
        {
            InitializeComponent();
        }

        public string Code
        {
            get
            {
                return textBoxCode.Text;
            }
            set 
            {
                textBoxCode.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return textBoxDescription.Text;
            }
            set
            {
                textBoxDescription.Text = value;
            }
        }
        public NesGame Game 
        {
            get
            {
                return FGame;
            }
            set
            {
                FGame = value;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (textBoxCode.Text == "")
            {
                MessageBox.Show(this, "You must enter a code!", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (FGame != null)
            {
                string lGamesDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "games");
                NesFile lGame = new NesFile(Path.Combine(Path.Combine(lGamesDir, FGame.Code), FGame.Code + ".nes"));
                try
                {
                    lGame.PRG = GameGenie.Patch(lGame.PRG, textBoxCode.Text.Trim());
                }
                catch (GameGenieFormatException)
                {
                    MessageBox.Show(this, string.Format(Resources.GameGenieFormatError, textBoxCode.Text, FGame.Name), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (GameGenieNotFoundException)
                {
                    MessageBox.Show(this, string.Format(Resources.GameGenieNotFound, textBoxCode.Text, FGame.Name), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (textBoxDescription.Text == "")
            {
                MessageBox.Show(this, "You must enter a description!", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
