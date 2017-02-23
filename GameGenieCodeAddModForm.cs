using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeAddModForm : Form
    {
        readonly NesGame FGame = null;

        public GameGenieCodeAddModForm(NesGame game)
        {
            InitializeComponent();
            FGame = game;
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
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCode.Text.Trim()))
            {
                MessageBox.Show(this, Resources.GGCodeEmpty, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (FGame != null)
            {
                NesFile lGame = new NesFile(FGame.NesPath);
                try
                {
                    lGame.PRG = GameGeniePatcher.Patch(lGame.PRG, textBoxCode.Text);
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

            if (string.IsNullOrEmpty(textBoxDescription.Text.Trim()))
            {
                MessageBox.Show(this, Resources.GGDescriptionEmpty, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            textBoxCode.Text = textBoxCode.Text.ToUpper().Trim();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
