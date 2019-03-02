using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class MessageForm : Form
    {
        public enum Button { Undefined, OK, Cancel, Abort, Yes, No, YesToAll }
        public enum DefaultButton { Button1 = 0, Button2 = 1, Button3 = 2 }

        public MessageForm()
        {
            InitializeComponent();
            if(Shared.isWindows)
            {
                EnableMenuItem(GetSystemMenu(Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
            }
            else
            {
                this.ControlBox = false;
            }
        }

        public static Button Show(Form hostForm, string title, string message, Image icon = null, Button[] buttons = null, DefaultButton defaultButton = DefaultButton.Button1)
        {
            if (hostForm.Disposing) return Button.Undefined;
            if (hostForm.InvokeRequired)
            {
                return (Button)hostForm.Invoke(new Func<Form, string, string, Image, Button[], DefaultButton, Button>(Show), new object[] { hostForm, title, message, icon, buttons, defaultButton });
            }
            return Show(title, message, icon, buttons, defaultButton);
        }

        public static Button Show(string title, string message, Image icon = null, Button[] buttons = null, DefaultButton defaultButton = DefaultButton.Button1)
        {
            if (buttons != null && (buttons.Length < 1 || buttons.Length > 3))
                throw new ArgumentOutOfRangeException();

            MessageForm form = new MessageForm();
            form.Text = title ?? form.Text;
            form.messageLabel.MaximumSize = new Size(form.panel3.Size.Width, 0);
            form.messageLabel.Text = message;
            form.pictureBox1.Image = icon ?? Resources.sign_info;

            form.buttons = buttons ?? new Button[] { Button.OK };
            System.Windows.Forms.Button[] formButtons = new System.Windows.Forms.Button[] { form.button1, form.button2, form.button3 };
            for(int i = 0; i < form.buttons.Length; ++i)
            {
                formButtons[i].Text = Resources.ResourceManager.GetString(form.buttons[i].ToString());
            }
            switch (form.buttons.Length)
            {
                default:
                    throw new NotSupportedException();
                case 1:
                    formButtons[0].Left = formButtons[2].Left;
                    formButtons[1].Visible = false;
                    formButtons[2].Visible = false;
                    break;
                case 2:
                    formButtons[0].Left = formButtons[1].Left;
                    formButtons[1].Left = formButtons[2].Left;
                    formButtons[2].Visible = false;
                    break;
                case 3:
                    break;
            }
            formButtons[(int)defaultButton].Select();

            form.ShowDialog();
            return form.result;
        }

        private Button[] buttons;
        private Button result;

        private void button1_Click(object sender, EventArgs e)
        {
            result = buttons[0];
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            result = buttons[1];
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            result = buttons[2];
            Close();
        }

        private void MessageForm_Shown(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        // imports to disable X button instead of hiding it
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        private void MessageForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    foreach (var button in buttons)
                        if (button == Button.Abort)
                        {
                            result = Button.Abort;
                            Close();
                            return;
                        }
                    foreach (var button in buttons)
                        if (button == Button.Cancel)
                        {
                            result = Button.Cancel;
                            Close();
                            return;
                        }
                    foreach (var button in buttons)
                        if (button == Button.No)
                        {
                            result = Button.No;
                            Close();
                            return;
                        }
                    break;
            }
        }
    }
}
