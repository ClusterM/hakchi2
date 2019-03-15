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
                textBoxPresetID.UnsignedValue = header2.PresetID;
                textBoxExtra.UnsignedValue = (byte)(header2.Chip);
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
                Close();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                header2.PresetID = textBoxPresetID.UnsignedValue;
                header2.Chip = textBoxExtra.UnsignedValue;
                game.WriteSfromHeader2(header2);
                Close();
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
                Close();
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
                Tasks.ErrorForm.Show(this, ex);
            }
        }
    }
}
