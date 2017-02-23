using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class StringInputForm : Form
    {
        public StringInputForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
