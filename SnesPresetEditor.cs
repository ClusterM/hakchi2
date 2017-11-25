using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SnesPresetEditor : Form
    {
        SnesGame game;
        bool wasCompressed;
        SnesGame.SfromHeader2 header2;

        public SnesPresetEditor(SnesGame game)
        {
            InitializeComponent();
            try
            {
                wasCompressed = game.DecompressPossible().Count() > 0;
                if (wasCompressed)
                    game.Decompress();
                this.game = game;
                header2 = game.ReadSfromHeader2();
                textBoxPresetID.Text = header2.PresetID.ToString("X4"); // PresetID is already in 0xMSB,LSB order
                textBoxExtra.Text = string.Format("{0:X2}", header2.Chip & 0xFF);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                var presetID = Convert.ToInt32(textBoxPresetID.Text, 16);
                var chip = Convert.ToInt32(textBoxExtra.Text, 16);
                header2.PresetID = (ushort)presetID;
                header2.Chip = (uint)chip;
                game.WriteSfromHeader2(header2);
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SnesPresetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (wasCompressed)
                    game.Compress();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
