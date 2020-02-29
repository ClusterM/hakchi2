using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class StringInputForm : Form
    {
        public string Value { get => textBox.Text; set => textBox.Text = value; }
        public string Comments { get => labelComments.Text; set => labelComments.Text = value; }
        public StringInputForm() => InitializeComponent();

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
