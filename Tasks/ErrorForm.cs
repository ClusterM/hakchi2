using com.clusterrr.hakchi_gui.Properties;
using System;
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

        public static void Show(Form hostForm, Exception ex, string title = null)
        {
            string formattedText = ex.GetType().Name + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(formattedText);
#endif
            Show(hostForm, title ?? Resources.Error, ex.Message, formattedText);
        }

        public static void Show(Exception ex, string title = null)
        {
            string formattedText = ex.GetType().Name + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(formattedText);
#endif
            Show(title ?? Resources.Error, ex.Message, formattedText);
        }

        public static void Show(Form hostForm, string title, string message, string details)
        {
            if (hostForm.Disposing) return;
            if (hostForm.InvokeRequired)
            {
                hostForm.Invoke(new Action<Form, string, string, string>(Show), new object[] { hostForm, title, message, details });
                return;
            }
            Show(title, message, details);
        }

        public static void Show(string title, string message, string details)
        {
            ErrorForm form = new ErrorForm();
            if (!string.IsNullOrEmpty(title))
            {
                form.Text = title;
            }
            form.errorLabel.Text = message;
            form.richTextBox1.Lines = details.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            form.ShowDialog();
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
