using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeForm : Form
    {
        private readonly NesMiniApplication FGame;
        private GameGenieDataBase FGameGenieDataBase;
        private List<string> OtherCodes;

        public GameGenieCodeForm(NesMiniApplication AGame)
        {
            InitializeComponent();
            FGame = AGame;
            FGameGenieDataBase = new GameGenieDataBase(FGame);
            this.Text += string.Format(": {0}", FGame.Name);
            LoadGameGenieCodes();
        }

        private void LoadGameGenieCodes()
        {
            checkedListBoxGameCode.Items.Clear();

            var lCodeSorted = FGameGenieDataBase.GameCodes.OrderBy(o => o.Description);
            var lSelectedCode = (FGame as NesMiniApplication).GameGenie.ToUpper().Split(new char[] { ',', '\t', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var code in lCodeSorted)
                checkedListBoxGameCode.Items.Add(code, lSelectedCode.Contains(code.Code.ToUpper().Trim()));

            OtherCodes = new List<string>();
            var knownCodes = from o in lCodeSorted select o.Code.ToUpper().Trim();
            foreach (var code in lSelectedCode)
            {
                if (!knownCodes.Contains(code.ToUpper().Trim()))
                    OtherCodes.Add(code);
            }
        }

        private void SaveSelectedCodes()
        {
            var selected = new List<string>();
            foreach (GameGenieCode code in checkedListBoxGameCode.CheckedItems)
                selected.Add(code.Code);
            selected.AddRange(OtherCodes);
            (FGame as NesMiniApplication).GameGenie = string.Join(",", selected.ToArray());
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SaveSelectedCodes();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void GameGenieForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (FGameGenieDataBase.Modified)
                FGameGenieDataBase.Save();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            GameGenieCodeAddModForm lForm = new GameGenieCodeAddModForm(FGame);

            if (lForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var newCode = lForm.Code.ToUpper().Trim();
                GameGenieCode lCode = null;

                foreach (GameGenieCode Code in checkedListBoxGameCode.Items)
                {
                    if (Code.Code.ToUpper().Trim() == newCode)
                    {
                        lCode = Code;
                        break;
                    }
                }

                if (lCode != null)
                {
                    if (MessageBox.Show(this, Resources.GGCodeExists, Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        lCode.Description = lForm.Description;
                        FGameGenieDataBase.ModifyCode(lCode);
                    }
                }
                else
                {
                    GameGenieCode lNewCode = new GameGenieCode(newCode, lForm.Description);
                    FGameGenieDataBase.AddCode(lNewCode);
                    LoadGameGenieCodes();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var i = (int)(sender as ToolStripMenuItem).Tag;
            GameGenieCode lCode = (GameGenieCode)checkedListBoxGameCode.Items[i];
            GameGenieCodeAddModForm lForm = new GameGenieCodeAddModForm(FGame);
            lForm.Code = lCode.Code;
            lForm.Description = lCode.Description;

            if (lForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lCode.Code = lForm.Code;
                lCode.Description = lForm.Description;
                FGameGenieDataBase.ModifyCode(lCode);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var i = (int)(sender as ToolStripMenuItem).Tag;
            GameGenieCode lCode = (GameGenieCode)checkedListBoxGameCode.Items[i];
            if (MessageBox.Show(this, string.Format(Resources.GGCodeDelete, lCode.Description),
                Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                FGameGenieDataBase.DeleteCode(lCode);
                checkedListBoxGameCode.Items.Remove(lCode);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (ofdXmlFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FGameGenieDataBase.ImportCodes(ofdXmlFile.FileName);
                LoadGameGenieCodes();
            }
        }

        private void checkedListBoxGameCode_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGameCode.IndexFromPoint(e.X, e.Y);
                removeToolStripMenuItem.Tag = editToolStripMenuItem.Tag = i;
                removeToolStripMenuItem.Enabled = editToolStripMenuItem.Enabled = i >= 0;
                contextMenuStrip.Show(sender as Control, e.X, e.Y);
            }
        }
    }
}
