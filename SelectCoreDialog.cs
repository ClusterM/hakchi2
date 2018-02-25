using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectCoreDialog : Form
    {
        public List<NesApplication> Games = new List<NesApplication>();

        private void fillGames()
        {
            listViewGames.Items.Clear();
            foreach(var game in Games)
            {
                var item = new ListViewItem(new string[] { game.Name, Path.GetExtension(game.GameFilePath), game.Metadata.System, game.Metadata.Core });
                item.Tag = game;
                listViewGames.Items.Add(item);
            }
        }

        private void fillSystems()
        {
            listBoxSystem.Items.Clear();
            listBoxSystem.Items.Add("Unassigned");
            foreach(var system in CoreCollection.Systems)
            {
                listBoxSystem.Items.Add(system);
            }
        }

        private void fillCores(string system)
        {
            listBoxCore.Items.Clear();
            if (!string.IsNullOrEmpty(system))
            {
                var cores = CoreCollection.GetCoresFromSystem(system);
                if (cores == null)
                {
                    var bins = CoreCollection.Cores;
                    foreach (var bin in bins)
                    {
                        listBoxCore.Items.Add(CoreCollection.GetCore(bin));
                    }
                }
                else
                    foreach (var core in cores)
                    {
                        listBoxCore.Items.Add(core);
                    }
            }
            listBoxCore.Enabled = !string.IsNullOrEmpty(system);
        }

        public SelectCoreDialog()
        {
            InitializeComponent();
            listViewGames.DoubleBuffered(true);
            fillSystems();
            DialogResult = DialogResult.Abort;
        }

        private void SelectCoreDialog_Shown(object sender, EventArgs e)
        {
            fillGames();
            ShowSelected();
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("Are you sure you want to discard changes?", "Discard changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void listBoxSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (string)listBoxSystem.SelectedItem;
            listBoxCore.ClearSelected();
            fillCores(item);
        }

        private void listBoxCore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxCore.SelectedItem == null)
            {
                commandTextBox.Text = string.Empty;
                commandTextBox.Enabled = false;
                buttonApply.Enabled = false;
            }
            else
            {
                commandTextBox.Text = (listBoxCore.SelectedItem as CoreCollection.CoreInfo).QualifiedBin;
                commandTextBox.Enabled = true;
                buttonApply.Enabled = true;
            }
        }

        private void ShowSelected()
        {
            var items = listViewGames.SelectedItems;
            if(items.Count == 0)
            {
                listBoxSystem.ClearSelected();
                commandTextBox.Text = string.Empty;
                listBoxSystem.Enabled = false;
                listBoxCore.Enabled = false;
                commandTextBox.Enabled = false;
                buttonApply.Enabled = false;
            }
            else
            {
                listBoxSystem.Enabled = true;

                if(items.Count == 1)
                {
                    var item = items[0];
                    var game = item.Tag as NesApplication;
                    if (!string.IsNullOrEmpty(game.Metadata.System))
                    {
                        listBoxSystem.ClearSelected();
                        for (int i = 0; i < listBoxSystem.Items.Count; ++i)
                        {
                            if (listBoxSystem.Items[i].ToString() == game.Metadata.System)
                            {
                                listBoxSystem.SetSelected(i, true);
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(game.Metadata.Core))
                    {
                        for(int i = 0; i < listBoxCore.Items.Count; ++i)
                        {
                            if (listBoxCore.Items[i].ToString() == game.Metadata.Core)
                            {
                                listBoxCore.SetSelected(i, true);
                                break;
                            }
                        }
                    }
                }
            }
        }


        private void listViewGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            string system = (string)listBoxSystem.SelectedItem;
            string core = ((CoreCollection.CoreInfo)listBoxCore.SelectedItem).Name;
            string bin = commandTextBox.Text;
            foreach(ListViewItem item in listViewGames.SelectedItems)
            {
                var game = item.Tag as NesApplication;
                game.Metadata.System = system;
                game.Metadata.Core = core;
                game.Desktop.Bin = bin;
                item.SubItems[2].Text = system;
                item.SubItems[3].Text = core;
            }
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectCoreDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to discard changes?", "Discard changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
