using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeAddModForm : Form
    {
        readonly NesMiniApplication FGame = null;

        public GameGenieCodeAddModForm(NesMiniApplication game)
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
                var tmpPath = Path.Combine(Path.GetTempPath(), FGame.Code);
                try
                {
                    FGame.CopyTo(tmpPath);
                    var lGame = NesMiniApplication.FromDirectory(tmpPath);
                    (lGame as NesMiniApplication).GameGenie = textBoxCode.Text;
                    lGame.Save();
                    (lGame as ISupportsGameGenie).ApplyGameGenie();
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
                finally
                {
                    if (Directory.Exists(tmpPath))
                        Directory.Delete(tmpPath, true);
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
