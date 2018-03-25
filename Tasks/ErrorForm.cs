using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class ErrorForm : Form
    {
        public ErrorForm()
        {
            InitializeComponent();
            Icon = Resources.icon;
            pictureBox1.Image = Resources.sign_error;
        }

        public static DialogResult ShowDialog(string title, string message, string details)
        {
            ErrorForm form = new ErrorForm();
            if (!string.IsNullOrEmpty(title))
            {
                form.Text = title;
            }
            form.errorLabel.Text = message;
            form.richTextBox1.Lines = details.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            form.ShowDialog();
            return DialogResult.OK;
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Copy();
            richTextBox1.DeselectAll();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            richTextBox1.SelectAll();
        }
    }
}
