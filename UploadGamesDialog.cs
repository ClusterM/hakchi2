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
            if (ConfigIni.Instance.SeparateGameStorage)
            {
                radioEUR.Enabled = true;
                radioUSA.Enabled = true;
            }
            labelSelectedGamesCollection.Text += MainForm.GetConsoleTypeName(ConfigIni.Instance.ConsoleType);
            checkLinked.Checked = ConfigIni.Instance.SyncLinked;
            DialogResult = DialogResult.Abort;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ConfigIni.Instance.SeparateGameStorage)
            {
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
