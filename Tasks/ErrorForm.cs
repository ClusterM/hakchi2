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

        public static DialogResult ShowDialog(string message, string stackTrace)
        {
            ErrorForm form = new ErrorForm();
            form.errorLabel.Text = message;
            form.richTextBox1.Lines = stackTrace.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            form.ShowDialog();
            return DialogResult.OK;
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
