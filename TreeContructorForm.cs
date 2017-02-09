using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class TreeContructorForm : Form
    {
        List<TreeNode> cuttedNodes = new List<TreeNode>();
        NesMenuCollection GamesCollection;
        private class NodeSorter : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 is ListViewItem)
                    o1 = (o1 as ListViewItem).Tag;
                if (o2 is ListViewItem)
                    o2 = (o2 as ListViewItem).Tag;

                if ((o1 as TreeNode).Tag is NesMenuCollection) return -1; // Root is always first
                if ((o2 as TreeNode).Tag is NesMenuCollection) return 1;
                INesMenuElement el1 = (o1 as TreeNode).Tag as INesMenuElement;
                INesMenuElement el2 = (o2 as TreeNode).Tag as INesMenuElement;
                var pos1 = 2;
                var pos2 = 2;
                if (el1 is NesMenuFolder) pos1 = (int)(el1 as NesMenuFolder).Position;
                if (el2 is NesMenuFolder) pos2 = (int)(el2 as NesMenuFolder).Position;
                if (pos1 != pos2) return pos1.CompareTo(pos2);
                return el1.Name.CompareTo(el2.Name);
            }
        }

        public TreeContructorForm(NesMenuCollection nesMenuCollection)
        {
            InitializeComponent();
            GamesCollection = nesMenuCollection;
            DrawTree();
            splitContainer.Panel2MinSize = 485;
            treeView.TreeViewNodeSorter = new NodeSorter();
            listViewContent.ListViewItemSorter = new NodeSorter();
        }

        void DrawTree(NesMenuCollection.SplitStyle splitStyle = NesMenuCollection.SplitStyle.NoSplit)
        {
            treeView.Nodes.Clear();
            var rootNode = new TreeNode(Resources.MainMenu);
            treeView.Nodes.Add(rootNode);
            GamesCollection.Unsplit();
            GamesCollection.Split(splitStyle, ConfigIni.MaxGamesPerFolder);
            rootNode.Tag = GamesCollection;
            AddNodes(rootNode.Nodes, GamesCollection);
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
                    if (usedFolders.Contains((nesElement as NesMenuFolder).ChildMenuCollection))
                    {
                        nesMenuCollection.Remove(nesElement); // We don't need any "back" folders
                        continue;
                    }
                    newNode.SelectedImageIndex = newNode.ImageIndex = 0;
                }
                else if (nesElement is NesGame)
                    newNode.SelectedImageIndex = newNode.ImageIndex = 2;
                else if (nesElement is NesDefaultGame)
                    newNode.SelectedImageIndex = newNode.ImageIndex = 4;
                newNode.Text = nesElement.Name;
                newNode.Tag = nesElement;
                treeNodeCollection.Add(newNode);
                if (nesElement is NesMenuFolder)
                {
                    AddNodes(newNode.Nodes, (nesElement as NesMenuFolder).ChildMenuCollection, usedFolders);
                }
            }
        }

        private void buttonNoFolders_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.NoSplit);
        }

        private void buttonNoFoldersOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.Original_NoSplit);
        }

        private void buttonFoldersEqually_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersEqual);
        }

        private void buttonFoldersEquallyOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.Original_FoldersEqual);
        }

        private void buttonFoldersLetters_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.FoldersAlphabetic_FoldersEqual);
        }

        private void buttonFoldersLettersOriginal_Click(object sender, EventArgs e)
        {
            DrawTree(NesMenuCollection.SplitStyle.Original_FoldersAlphabetic_FoldersEqual);
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
                pictureBoxArt.Image = (node.Tag is NesMenuFolder) ? (node.Tag as NesMenuFolder).Image : null;
                groupBoxArt.Enabled = (node.Tag is NesMenuFolder);
                groupBoxArt.Cursor = Cursors.Hand;
                listViewContent.Enabled = true;
                foreach (TreeNode n in node.Nodes)
                {
                    var element = (INesMenuElement)n.Tag;
                    var item = new ListViewItem();
                    item.Text = element.Name;
                    var transparency = cuttedNodes.Contains(n) ? 1 : 0;
                    if (element is NesMenuFolder)
                        item.ImageIndex = 0 + transparency;
                    else if (element is NesGame)
                        item.ImageIndex = 2 + transparency;
                    else if (element is NesDefaultGame)
                        item.ImageIndex = 4 + transparency;
                    item.Tag = n;
                    listViewContent.Items.Add(item);
                }
            }
            else
            {
                if (node != null && node.Tag is NesGame)
                {
                    var game = node.Tag as NesGame;
                    pictureBoxArt.Image = NesGame.LoadBitmap(game.IconPath);
                    groupBoxArt.Enabled = true;
                    listViewContent.Enabled = false;
                }
                else //if (e.Node.Tag is NesDefaultGame)
                {
                    pictureBoxArt.Image = null;
                    groupBoxArt.Enabled = false;
                }
                listViewContent.Enabled = false;
                groupBoxArt.Cursor = Cursors.Default;
            }
            ShowFolderStats();
        }

        void ShowFolderStats()
        {
            var node = treeView.SelectedNode;
            if (node != null && (node.Tag is NesMenuCollection || node.Tag is NesMenuFolder)) // Folder or root
            {
                labelElementCount.Text = string.Format("Folder \"{0}\" contains {1} elements.", node.Text, node.Nodes.Count);
                buttonNewFolder.Enabled = true;
            }
            else
            {
                labelElementCount.Text = "";
                buttonNewFolder.Enabled = false;
            }
            if (node != null && node.Tag is NesMenuFolder) // Folder 
            {
                labelPosition.Enabled = comboBoxPosition.Enabled = true;
                var folder = node.Tag as NesMenuFolder;
                var position = (int)folder.Position;
                if (position > 1) position--;
                comboBoxPosition.SelectedIndex = position;
            }
            else
            {
                labelPosition.Enabled = comboBoxPosition.Enabled = false;
                comboBoxPosition.SelectedIndex = -1;
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

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is NesMenuFolder))
                e.CancelEdit = true;
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if ((e.Node.Tag is NesMenuCollection) || (e.Node.Tag is NesDefaultGame)
                || string.IsNullOrEmpty(e.Label) || string.IsNullOrEmpty(e.Label.Trim()))
                e.CancelEdit = true;
            else
            {
                e.Node.Text = e.Label;
                (e.Node.Tag as INesMenuElement).Name = e.Label;
                var parent = e.Node.Parent;
                FixSort(e.Node);
            }
        }

        private void listViewContent_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (!((listViewContent.Items[e.Item].Tag as TreeNode).Tag is NesMenuFolder))
                e.CancelEdit = true;
        }

        private void listViewContent_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label) || string.IsNullOrEmpty(e.Label.Trim()))
                e.CancelEdit = true;
            else
            {
                var item = listViewContent.Items[e.Item];
                var node = item.Tag as TreeNode;
                node.Text = e.Label;
                (node.Tag as INesMenuElement).Name = e.Label;
                FixSort(item);
            }
        }

        private void FixSort(object o)
        {
            // This is most simple to resort node without resorting the whole tree/list
            new Thread(KostilKostilevich).Start(o);
        }
        private void KostilKostilevich(object o)
        {
            // This is stupid workaround for resort after renaming item, lol
            if (InvokeRequired)
            {
                Invoke(new Action<object>(KostilKostilevich), new object[] { o });
                return;
            }
            if (o is TreeNode)
            {
                var node = o as TreeNode;
                var parent = node.Parent;
                parent.Nodes.Remove(node);
                parent.Nodes.Add(node);
                treeView.SelectedNode = node;
                ShowFolderStats();
            }
            if (o is ListViewItem)
            {
                var item = o as ListViewItem;
                listViewContent.Items.Remove(item);
                listViewContent.Items.Add(item);
                var node = item.Tag as TreeNode;
                var parent = node.Parent;
                parent.Nodes.Remove(node);
                parent.Nodes.Add(node);
            }
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && !((e.Item as TreeNode).Tag is NesMenuCollection)) // We can't drag root
                DoDragDrop(new TreeNode[] { (TreeNode)e.Item }, DragDropEffects.Move);
        }

        private void listViewContent_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var nodes = new List<TreeNode>();
            foreach (ListViewItem i in listViewContent.SelectedItems)
                nodes.Add(i.Tag as TreeNode);
            DoDragDrop(nodes.ToArray(), DragDropEffects.Move);
        }

        private void TreeContructorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ConfigIni.Save(); // Need to save changed names
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode[]", false))
                e.Effect = DragDropEffects.Move;
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode[]", false))
            {
                TreeNode destinationNode;
                if (sender is TreeView)
                {
                    Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                    destinationNode = ((TreeView)sender).GetNodeAt(pt);
                }
                else
                {
                    Point pt = ((ListView)sender).PointToClient(new Point(e.X, e.Y));
                    var item = ((ListView)sender).GetItemAt(pt.X, pt.Y);
                    if (item == null)
                        destinationNode = treeView.SelectedNode;
                    else
                        destinationNode = (TreeNode)item.Tag;
                }
                var newNodes = (TreeNode[])e.Data.GetData("System.Windows.Forms.TreeNode[]");
                MoveToFolder(newNodes, destinationNode);
                if (sender is TreeView)
                    treeView.Select();
                else
                    listViewContent.Select();
            }
        }

        private void buttonNewFolder_Click(object sender, EventArgs e)
        {
            newFolder(null);
        }

        private void comboBoxPosition_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!(treeView.SelectedNode.Tag is NesMenuFolder)) return;
            int value = comboBoxPosition.SelectedIndex;
            if (value >= 2) value++;
            var node = treeView.SelectedNode;
            var folder = (node.Tag as NesMenuFolder);
            folder.Position = (NesMenuFolder.Priority)value;
            FixSort(node);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renameFolder(sender as ToolStripMenuItem);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Tag is TreeNode)
                deleteFolder((sender as ToolStripMenuItem).Tag as TreeNode);
            else
                deleteFolder(((sender as ToolStripMenuItem).Tag as ListViewItem).Tag as TreeNode);
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is TreeView)
            {
                var destinationNode = ((TreeView)sender).GetNodeAt(e.X, e.Y);
                if (destinationNode == null) return;
                treeView.SelectedNode = destinationNode;
                newFolderToolStripMenuItem.Tag = deleteToolStripMenuItem.Tag = renameToolStripMenuItem.Tag = destinationNode;
                renameToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = destinationNode.Tag is NesMenuFolder;
            }
            else
            {
                var item = ((ListView)sender).GetItemAt(e.X, e.Y);
                newFolderToolStripMenuItem.Tag = deleteToolStripMenuItem.Tag = renameToolStripMenuItem.Tag = item;
                renameToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = (item != null) && (item.Tag as TreeNode).Tag is NesMenuFolder;
                listViewContent.SelectedItems.Clear();
            }
            contextMenuStrip.Show(sender as Control, e.X, e.Y);
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Tag is TreeNode)
                newFolder((sender as ToolStripMenuItem).Tag as TreeNode);
            else
                newFolder(null);
        }
        bool MoveToFolder(IEnumerable<TreeNode> newNodes, TreeNode destinationNode, bool showDest = true)
        {
            if (destinationNode == null)
                destinationNode = treeView.Nodes[0]; // Root
            if (destinationNode.Tag is NesGame || destinationNode.Tag is NesDefaultGame)
                destinationNode = destinationNode.Parent;
            foreach (var newNode in newNodes)
            {
                if (!destinationNode.FullPath.StartsWith(newNode.FullPath) && (destinationNode != newNode.Parent))
                {
                    Debug.WriteLine(string.Format("Drag: {0}->{1}", newNode, destinationNode));
                    if (newNode.Parent.Tag is NesMenuFolder)
                        (newNode.Parent.Tag as NesMenuFolder).ChildMenuCollection.Remove(newNode.Tag as INesMenuElement);
                    else if (newNode.Parent.Tag is NesMenuCollection)
                        (newNode.Parent.Tag as NesMenuCollection).Remove(newNode.Tag as INesMenuElement);
                    newNode.Parent.Nodes.Remove(newNode);
                    destinationNode.Nodes.Add(newNode);
                    if (destinationNode.Tag is NesMenuFolder)
                        (destinationNode.Tag as NesMenuFolder).ChildMenuCollection.Add(newNode.Tag as INesMenuElement);
                    else if (destinationNode.Tag is NesMenuCollection)
                        (destinationNode.Tag as NesMenuCollection).Add(newNode.Tag as INesMenuElement);
                }
                else
                {
                    System.Media.SystemSounds.Hand.Play();
                    return false;
                }
            }
            if (showDest)
            {
                if (treeView.SelectedNode == destinationNode)
                    ShowSelected();
                else
                    treeView.SelectedNode = destinationNode;
                ShowFolderStats();
                foreach (ListViewItem item in listViewContent.Items)
                    item.Selected = newNodes.Contains(item.Tag as TreeNode);

            }
            return true;
        }


        void newFolder(TreeNode node = null)
        {
            var newnode = new TreeNode("New folder", 0, 0);
            newnode.Tag = new NesMenuFolder(newnode.Text);
            if (node != null)
            {
                node.Nodes.Add(newnode);
                treeView.SelectedNode = newnode;
                ShowSelected();
                newnode.BeginEdit();
            }
            else
            {
                node = treeView.SelectedNode;
                node.Nodes.Add(newnode);
                ShowFolderStats();
                var item = new ListViewItem(newnode.Text, 0);
                item.Tag = newnode;
                listViewContent.SelectedItems.Clear();
                listViewContent.Items.Add(item);
                item.BeginEdit();
            }
        }

        TreeNode getUnsortedFolder()
        {
            const string unsortedName = "Unsorted";
            var root = treeView.Nodes[0];
            foreach (TreeNode el in root.Nodes)
            {
                if (el.Text == unsortedName && el.Tag is NesMenuFolder)
                    return el;
            }
            var newNode = new TreeNode(unsortedName, 0, 0);
            var newFolder = new NesMenuFolder(newNode.Text);
            newFolder.Position = NesMenuFolder.Priority.Leftmost;
            newNode.Tag = newFolder;
            (root.Tag as NesMenuCollection).Add(newFolder);
            root.Nodes.Add(newNode);
            return newNode;
        }

        void deleteFolder(TreeNode node)
        {
            if (MessageBox.Show(this, string.Format(Resources.DeleteFolder, node.Text),
                Resources.AreYouSure, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            if (node.Nodes.Count > 0)
            {
                MessageBox.Show(this, Resources.FolderContent, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var unsortedFolder = getUnsortedFolder();
                if (node == unsortedFolder) // WTF? We can't delete it.
                {
                    System.Media.SystemSounds.Hand.Play();
                    return;
                }
                MoveToFolder(node.Nodes.Cast<TreeNode>().ToArray(), unsortedFolder, false);
                (node.Tag as NesMenuFolder).ChildMenuCollection.Clear();
            }
            foreach (var i in from i in listViewContent.Items.Cast<ListViewItem>().ToArray() where i.Tag == node select i)
                listViewContent.Items.Remove(i);
            var parent = node.Parent;
            if (parent.Tag is NesMenuFolder)
                (parent.Tag as NesMenuFolder).ChildMenuCollection.Remove(node.Tag as INesMenuElement);
            else if (parent.Tag is NesMenuCollection)
                (parent.Tag as NesMenuCollection).Remove(node.Tag as INesMenuElement);
            parent.Nodes.Remove(node);
            treeView.SelectedNode = parent;
        }

        void cutElements(IEnumerable<TreeNode> nodes)
        {
            foreach (var node in cuttedNodes)
                node.SelectedImageIndex = node.ImageIndex = node.ImageIndex / 2 * 2;
            cuttedNodes.Clear();

            foreach (var node in nodes)
            {
                cuttedNodes.Add(node);
                node.SelectedImageIndex = node.ImageIndex = node.ImageIndex / 2 * 2 + 1;
            }
            foreach (ListViewItem item in listViewContent.Items)
            {
                item.ImageIndex = item.ImageIndex / 2 * 2 +
                        ((cuttedNodes.Contains(item.Tag as TreeNode)) ? 1 : 0);
            }
        }

        void pasteElements(TreeNode node)
        {
            if ((cuttedNodes.Count > 0) && MoveToFolder(cuttedNodes, node))
            {
                cutElements(new TreeNode[0]);
            }
        }

        private void renameFolder(object folder)
        {
            if (folder is TreeNode)
                (folder as TreeNode).BeginEdit();
            else if (folder is ListViewItem)
                (folder as ListViewItem).BeginEdit();
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None)
            {
                if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag is NesMenuFolder))
                    deleteFolder(treeView.SelectedNode);
            }
            else if (e.KeyCode == Keys.F2 && e.Modifiers == Keys.None)
            {
                if ((treeView.SelectedNode != null) && (treeView.SelectedNode.Tag is NesMenuFolder))
                    renameFolder(treeView.SelectedNode);
            }
            else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
            {
                if ((treeView.SelectedNode != null) && !(treeView.SelectedNode.Tag is NesMenuCollection))
                    cutElements(new TreeNode[] { treeView.SelectedNode });
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (treeView.SelectedNode != null &&
                    (treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection))
                    pasteElements(treeView.SelectedNode);
            }
        }

        private void listViewContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listViewContent.SelectedItems.Count != 1) return;
                var item = listViewContent.SelectedItems[0];
                if ((item.Tag as TreeNode).Tag is NesMenuFolder)
                    deleteFolder(item.Tag as TreeNode);
            }
            else if (e.KeyCode == Keys.F2 && e.Modifiers == Keys.None)
            {
                if (treeView.SelectedNode != null && treeView.SelectedNode.Tag is NesMenuFolder)
                    renameFolder(treeView.SelectedNode);
                if (listViewContent.SelectedItems.Count != 1) return;
                var item = listViewContent.SelectedItems[0];
                renameFolder(item);
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                foreach (ListViewItem item in listViewContent.Items)
                    item.Selected = true;
            }
            else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
            {
                var elements = from i in listViewContent.SelectedItems.Cast<ListViewItem>().ToArray() select i.Tag as TreeNode;
                cutElements(elements);
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (treeView.SelectedNode != null &&
                    (treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection))
                    pasteElements(treeView.SelectedNode);
            }
        }

        private void listViewContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (listViewContent.SelectedItems.Count != 1) return;
                var item = listViewContent.SelectedItems[0];

                if (!((item.Tag as TreeNode).Tag is NesMenuFolder))
                    return;
                treeView.SelectedNode = item.Tag as TreeNode;
            }
            else if (e.KeyChar == (char)8)
            {
                if (treeView.SelectedNode != null && treeView.SelectedNode.Parent != null)
                {
                    treeView.SelectedNode = treeView.SelectedNode.Parent;
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void pictureBoxArt_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag is NesMenuFolder)
            {
                var folder = treeView.SelectedNode.Tag as NesMenuFolder;
                var form = new SelectIconForm(folder.ImageId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    folder.ImageId = form.listBox.SelectedItem.ToString();
                    pictureBoxArt.Image = folder.Image;
                }
            }
        }
    }
}
