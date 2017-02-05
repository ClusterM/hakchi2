using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.Famicom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.XPath;
using System.Xml;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameGenieCodeForm : Form
    {
        private NesGame FGame;
        private string FDBName;
        private GameGenieDataBase FGameGenieDataBase;

        public GameGenieCodeForm(NesGame AGame)
        {
            InitializeComponent();
            FGame = AGame;
            FDBName = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "GameGenieDB.xml");
            FGameGenieDataBase = new GameGenieDataBase(FDBName, FGame);
            this.Text = string.Format("Game Genie Code List : {0}", FGame.Name);
            LoadGameGenieCodes();
        }

        private void LoadGameGenieCodes()
        {
            checkedListBoxGameCode.Items.Clear();

            if (File.Exists(FDBName))
            {
                var lCodeSorted = FGameGenieDataBase.GameCodes.OrderBy(o => o.Description);
                var lSelectedCode = FGame.GameGenie.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var code in lCodeSorted)
                    checkedListBoxGameCode.Items.Add(code, lSelectedCode.Contains(code.Code));
            }
        }

        private void SaveSelectedCodes()
        {
            var selected = new List<string>();
            foreach (GameGenieCode code in checkedListBoxGameCode.CheckedItems)
                selected.Add(code.Code);
            FGame.GameGenie = string.Join(",", selected.ToArray());
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

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameGenieCodeAddModForm lForm = new GameGenieCodeAddModForm();
            lForm.Game = FGame;

            if (lForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameGenieCode lCode = null;

                foreach(GameGenieCode Code in checkedListBoxGameCode.Items)
                {
                    if (Code.Code == lForm.Code)
                    {
                        lCode = Code;
                        break;
                    }
                }

                if (lCode != null)
                {
                    if (MessageBox.Show(this, "This code already exist do you want to edit it?", Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        lCode.Description = lForm.Description;
                        FGameGenieDataBase.ModifyCode(lCode);
                    }
                }
                else
                {
                    GameGenieCode lNewCode = new GameGenieCode(lForm.Code, lForm.Description);
                    FGameGenieDataBase.AddCode(lNewCode);
                    LoadGameGenieCodes();
//                    checkedListBoxGameCode.Items.Add(lNewCode, false);
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkedListBoxGameCode.SelectedItem != null)
            {
                GameGenieCode lCode = (GameGenieCode)checkedListBoxGameCode.SelectedItem;
                GameGenieCodeAddModForm lForm = new GameGenieCodeAddModForm();
                lForm.Code = lCode.Code;
                lForm.Description = lCode.Description;
                lForm.Game = FGame;

                if (lForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lCode.Code = lForm.Code;
                    lCode.Description = lForm.Description;
                    FGameGenieDataBase.ModifyCode(lCode);
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((checkedListBoxGameCode.SelectedItem != null) &&
                (MessageBox.Show(this, "Do you want to delete this code?", Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
            {
                GameGenieCode lCode = (GameGenieCode)checkedListBoxGameCode.SelectedItem;
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
    }
}
