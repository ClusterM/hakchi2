using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectSnesButtonsForm : Form
    {
        [Flags]
        public enum SnesButtons
        {
            A = 0x01,
            B = 0x02,
            Select = 0x04,
            Start = 0x08,
            Up = 0x10,
            Down = 0x20,
            Left = 0x40,
            Right = 0x080,
            X = 0x100,
            Y = 0x200,
            L = 0x400,
            R = 0x800
        }

        public SnesButtons SelectedButtons;

        public SelectSnesButtonsForm(SnesButtons buttons)
        {
            InitializeComponent();
            checkBoxA.Checked = (buttons & SnesButtons.A) != 0;
            checkBoxB.Checked = (buttons & SnesButtons.B) != 0;
            checkBoxSelect.Checked = (buttons & SnesButtons.Select) != 0;
            checkBoxStart.Checked = (buttons & SnesButtons.Start) != 0;
            checkBoxUp.Checked = (buttons & SnesButtons.Up) != 0;
            checkBoxDown.Checked = (buttons & SnesButtons.Down) != 0;
            checkBoxLeft.Checked = (buttons & SnesButtons.Left) != 0;
            checkBoxRight.Checked = (buttons & SnesButtons.Right) != 0;
            checkBoxX.Checked = (buttons & SnesButtons.X) != 0;
            checkBoxY.Checked = (buttons & SnesButtons.Y) != 0;
            checkBoxL.Checked = (buttons & SnesButtons.L) != 0;
            checkBoxR.Checked = (buttons & SnesButtons.R) != 0;
            SelectedButtons = buttons;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            SelectedButtons =
                (checkBoxA.Checked ? SnesButtons.A : 0) |
                (checkBoxB.Checked ? SnesButtons.B : 0) |
                (checkBoxSelect.Checked ? SnesButtons.Select : 0) |
                (checkBoxStart.Checked ? SnesButtons.Start : 0) |
                (checkBoxUp.Checked ? SnesButtons.Up : 0) |
                (checkBoxDown.Checked ? SnesButtons.Down : 0) |
                (checkBoxLeft.Checked ? SnesButtons.Left : 0) |
                (checkBoxRight.Checked ? SnesButtons.Right : 0) |
                (checkBoxX.Checked ? SnesButtons.X : 0) |
                (checkBoxY.Checked ? SnesButtons.Y : 0) |
                (checkBoxL.Checked ? SnesButtons.L : 0) |
                (checkBoxR.Checked ? SnesButtons.R : 0);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            int buttonCount = 0;
            for (int i = 0; i < 12; i++)
                if (((int)SelectedButtons & (1 << i)) != 0)
                    buttonCount++;
            if (buttonCount < 2)
            {
                MessageBox.Show(this, Resources.SelectAtLeastTwo, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
