using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class TreeContructorForm : Form
    {
        NesMenuCollection originalCollection;

        public TreeContructorForm(NesMenuCollection nesMenuCollection)
        {
            InitializeComponent();
            originalCollection = nesMenuCollection;
            DrawTree();
            splitContainer.Panel2MinSize = 485;
        }

        void DrawTree(NesMenuCollection.SplitStyle splitStyle = NesMenuCollection.SplitStyle.NoSplit, bool originalToRoot = false)
        {
            treeView.Nodes.Clear();
            var rootNode = new TreeNode("NES Mini");
            treeView.Nodes.Add(rootNode);
            var newCollection = new NesMenuCollection();
            newCollection.AddRange(originalCollection);
            newCollection.Split(splitStyle, originalToRoot, ConfigIni.MaxGamesPerFolder);
            AddNodes(rootNode.Nodes, newCollection);
            rootNode.Expand();
            treeView.SelectedNode = rootNode;
        }

        void AddNodes(TreeNodeCollection treeNodeCollection, NesMenuCollection nesMenuCollection, List<NesMenuCollection> usedFolders = null)
        {
            if (usedFolders == null)
                usedFolders = new List<NesMenuCollection>();
            if (usedFolders.Contains(nesMenuCollection))
                return;
            usedFolders.Add(nesMenuCollection);
            var sorted = nesMenuCollection.OrderBy(o => o.Name).OrderBy(o => (o is NesMenuFolder) ? (byte)(o as NesMenuFolder).Position : 2);
            foreach (var nesElement in sorted)
            {
                var newNode = new TreeNode();
                if (nesElement is NesMenuFolder)
                {
                    if (usedFolders.Contains((nesElement as NesMenuFolder).Child))
                    {
                        nesMenuCollection.Remove(nesElement); // We don't need any "back" buttons
                        continue;
                    }
                    newNode.SelectedImageIndex = newNode.ImageIndex = 0;
                }
                else if (nesElement is NesGame)
                    newNode.SelectedImageIndex = newNode.ImageIndex = 1;
                else if (nesElement is NesDefaultGame)
                    newNode.SelectedImageIndex = newNode.ImageIndex = 2;
                newNode.Text = nesElement.Name;
                newNode.Tag = nesElement;
                treeNodeCollection.Add(newNode);
                if (nesElement is NesMenuFolder)
                {
                    AddNodes(newNode.Nodes, (nesElement as NesMenuFolder).Child, usedFolders);
                }
            }
        }

        private void buttonNoFolders_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.NoSplit, false);
        }

        private void buttonNoFoldersOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.NoSplit, true);
        }

        private void buttonFoldersEqually_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersEqual, false);
        }

        private void buttonFoldersEquallyOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersEqual, true);
        }

        private void buttonFoldersLetters_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersAlphabetic_FoldersEqual, false);
        }

        private void buttonFoldersLettersOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersAlphabetic_FoldersEqual, true);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowSelected();
        }

        private void ShowSelected()
        {
            var node = treeView.SelectedNode;
            listViewContent.Clear();
            if (node != null && (node.Nodes.Count > 0 || node.Tag is NesMenuFolder)) // Folder or root
            {
                pictureBoxArt.Image = (node.Tag == null) ? null : (node.Tag as NesMenuFolder).Image;
                pictureBoxArt.Enabled = node.Tag != null;
                listViewContent.Enabled = true;
                foreach (TreeNode n in node.Nodes)
                {
                    var element = (INesMenuElement)n.Tag;
                    var item = new ListViewItem();
                    item.Text = element.Name;
                    if (element is NesMenuFolder)
                        item.ImageIndex = 0;
                    else if (element is NesGame)
                        item.ImageIndex = 1;
                    else if (element is NesDefaultGame)
                        item.ImageIndex = 2;
                    item.Tag = n;
                    listViewContent.Items.Add(item);
                }
                labelElementCount.Text = string.Format("This folder contains {0} elements", node.Nodes.Count);
            }
            else
            {
                if (node != null && node.Tag is NesGame)
                {
                    var game = node.Tag as NesGame;
                    pictureBoxArt.Image = NesGame.LoadBitmap(game.IconPath);
                    pictureBoxArt.Enabled = true;
                    listViewContent.Enabled = false;
                }
                else //if (e.Node.Tag is NesDefaultGame)
                {
                    pictureBoxArt.Image = null;
                    pictureBoxArt.Enabled = false;
                }
                listViewContent.Enabled = false;
                labelElementCount.Text = "";
            }
        }

        private void listViewContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && listViewContent.SelectedItems.Count == 1)
            {
                var node = (listViewContent.SelectedItems[0].Tag as TreeNode);
                treeView.SelectedNode = node;
                //node.Expand();
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if ((e.Node.Tag == null) || string.IsNullOrEmpty(e.Label) || string.IsNullOrEmpty(e.Label.Trim()))
                e.CancelEdit = true;
            else
                (e.Node.Tag as INesMenuElement).Name = e.Label;
        }

        private void TreeContructorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConfigIni.Save(); // Need to save changed names
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag == null)
                e.CancelEdit = true;
        }

        private void listViewContent_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label) || string.IsNullOrEmpty(e.Label.Trim()))
                e.CancelEdit = true;
            else
            {
                (listViewContent.Items[(e.Item)].Tag as TreeNode).Text = e.Label;
                ((listViewContent.Items[(e.Item)].Tag as TreeNode).Tag as INesMenuElement).Name = e.Label;
            }
        }

    }
}
