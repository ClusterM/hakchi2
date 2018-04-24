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
        }

        public static void Show(Form hostForm, Exception ex, string title = null)
        {
            string text = formatException(ex);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
#endif
            Show(hostForm, (title ?? Resources.Error), ex.Message, text, Resources.sign_error);
        }

        public static void Show(Exception ex, string title = null)
        {
            string text = formatException(ex);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
#endif
            Show((title ?? Resources.Error), ex.Message, text, Resources.sign_error);
        }

        public static void Show(Form hostForm, string title, string message, string details, Image icon)
        {
            if (hostForm != null)
            {
                if (hostForm.Disposing) return;
                if (hostForm.InvokeRequired)
                {
                    hostForm.Invoke(new Action<Form, string, string, string, Image>(Show), new object[] { hostForm, title, message, details, icon });
                    return;
                }
            }
            ErrorForm form = new ErrorForm();
            form.pictureBox1.Image = icon;
            form.Text = title ?? Resources.Error;
            form.errorLabel.Text = message;
            form.richTextBox1.AppendText(details);
            if (hostForm != null)
            {
                form.ShowDialog(hostForm);
            }
            else
            {
                form.ShowDialog();
            }
        }

        public static void Show(string title, string message, string details, Image icon)
        {
            Show(null, title, message, details, icon);
        }

        private static string formatException(Exception ex)
        {
            return
            (
                ex.GetType().Name + Environment.NewLine +
                ex.Message + Environment.NewLine +
                ex.StackTrace + Environment.NewLine +
                "--- DEBUGLOG.TXT content ---" + Environment.NewLine +
                Program.GetCurrentLogContent() +
                "--- End of DEBUGLOG.TXT content ---" + Environment.NewLine
            );
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
