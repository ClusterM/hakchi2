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
    public partial class UploadGamesDialog : Form
    {
        public UploadGamesDialog()
        {
            InitializeComponent();
            if (ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.SNES && ConfigIni.Instance.SeparateGameStorage)
            {
                radioEUR.Enabled = true;
                radioUSA.Enabled = true;
                switch (ConfigIni.Instance.SyncRegion)
                {
                    case "EUR":
                        radioEUR.Checked = true;
                        break;

                    case "USA":
                        radioUSA.Checked = true;
                        break;
                }
            }
            labelSelectedGamesCollection.Text += MainForm.GetConsoleTypeName(ConfigIni.Instance.ConsoleType);
            checkLinked.Checked = ConfigIni.Instance.SyncLinked;
            DialogResult = DialogResult.Abort;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ConfigIni.Instance.SeparateGameStorage)
            {
                if (ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.SNES)
                {
                    if (radioEUR.Checked) ConfigIni.Instance.SyncRegion = "EUR";
                    if (radioUSA.Checked) ConfigIni.Instance.SyncRegion = "USA";
                    if (radioEUR.Checked == false && radioUSA.Checked == false)
                    {
                        Tasks.MessageForm.Show(Resources.ExportGames, Resources.SelectRegion, Resources.sign_error);
                        return;
                    }
                }
            }
            ConfigIni.Instance.SyncLinked = checkLinked.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
