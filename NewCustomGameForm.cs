using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class NewCustomGameForm : Form
    {
        public NesApplication NewApp = null;

        public NewCustomGameForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (textBox1.Text.Length > 0)
                {
                    uint crc32 = Shared.CRC32(Encoding.UTF8.GetBytes(textBox1.Text.ToCharArray()));
                    maskedTextBox1.Text = NesApplication.GenerateCode(crc32, AppTypeCollection.GetAvailablePrefix(textBox1.Text));
                }
                else
                {
                    maskedTextBox1.Text = string.Empty;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.MaskCompleted && textBox1.Text.Length > 0)
            {
                var code = maskedTextBox1.Text;
                var path = Path.Combine(NesApplication.GamesDirectory, code);
                if (!Directory.Exists(path))
                {
                    NewApp = NesApplication.CreateEmptyApp(path, textBox1.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    Tasks.MessageForm.Show(Resources.NewCustomGame, Resources.CustomGameCodeAlreadyExists, Resources.sign_warning);
                }
            }
            else
            {
                Tasks.MessageForm.Show(Resources.NewCustomGame, Resources.CustomGameNeedValidData, Resources.sign_warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
