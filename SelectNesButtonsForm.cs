using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectNesButtonsForm : Form
    {
        [Flags]
        public enum NesButtons
        {
            A = 0x01,
            B = 0x02,
            Select = 0x04,
            Start = 0x08,
            Up = 0x10,
            Down = 0x20,
            Left = 0x40,
            Right = 0x080
        }

        public NesButtons SelectedButtons;

        public SelectNesButtonsForm(NesButtons buttons)
        {
            InitializeComponent();
            checkBoxA.Checked = (buttons & NesButtons.A) != 0;
            checkBoxB.Checked = (buttons & NesButtons.B) != 0;
            checkBoxSelect.Checked = (buttons & NesButtons.Select) != 0;
            checkBoxStart.Checked = (buttons & NesButtons.Start) != 0;
            checkBoxUp.Checked = (buttons & NesButtons.Up) != 0;
            checkBoxDown.Checked = (buttons & NesButtons.Down) != 0;
            checkBoxLeft.Checked = (buttons & NesButtons.Left) != 0;
            checkBoxRight.Checked = (buttons & NesButtons.Right) != 0;
            SelectedButtons = buttons;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            SelectedButtons =
                (checkBoxA.Checked ? NesButtons.A : 0) |
                (checkBoxB.Checked ? NesButtons.B : 0) |
                (checkBoxSelect.Checked ? NesButtons.Select : 0) |
                (checkBoxStart.Checked ? NesButtons.Start : 0) |
                (checkBoxUp.Checked ? NesButtons.Up : 0) |
                (checkBoxDown.Checked ? NesButtons.Down : 0) |
                (checkBoxLeft.Checked ? NesButtons.Left : 0) |
                (checkBoxRight.Checked ? NesButtons.Right : 0);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            int buttonCount = 0;
            for (int i = 0; i < 8; i++)
                if (((byte)SelectedButtons & (1 << i)) != 0)
                    buttonCount++;
            if (buttonCount < 2)
            {
                Tasks.MessageForm.Show(Resources.Error, Resources.SelectAtLeastTwo, Resources.sign_error);
                //MessageBox.Show(this, Resources.SelectAtLeastTwo, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
