using com.clusterrr.hakchi_gui.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeAddModForm : Form
    {
        readonly NesApplication FGame = null;

        public GameGenieCodeAddModForm(NesApplication game)
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
                Tasks.MessageForm.Show(Resources.Error, Resources.GGCodeEmpty, Resources.sign_error);
                return;
            }

            if (FGame != null)
            {
                var tmpPath = Path.Combine(Path.GetTempPath(), FGame.Code);
                try
                {
                    FGame.CopyTo(tmpPath);
                    var lGame = NesApplication.FromDirectory(tmpPath);
                    (lGame as NesApplication).GameGenie = textBoxCode.Text;
                    lGame.Save();
                    (lGame as ISupportsGameGenie).ApplyGameGenie();
                }
                catch (GameGenieFormatException)
                {
                    Tasks.MessageForm.Show(Resources.Error, string.Format(Resources.GameGenieFormatError, textBoxCode.Text, FGame.Name), Resources.sign_error);
                    return;
                }
                catch (GameGenieNotFoundException)
                {
                    Tasks.MessageForm.Show(Resources.Error, string.Format(Resources.GameGenieNotFound, textBoxCode.Text, FGame.Name), Resources.sign_error);
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
                Tasks.MessageForm.Show(Resources.Error, Resources.GGDescriptionEmpty, Resources.sign_error);
                return;
            }
            textBoxCode.Text = textBoxCode.Text.ToUpper().Trim();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
