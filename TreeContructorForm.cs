using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace com.clusterrr.hakchi_gui
{
    public partial class TreeContructorForm : Form
    {
        public static string FoldersXmlPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigIni.ConfigDir), "folders.xml");
        List<TreeNode> cuttedNodes = new List<TreeNode>();
        List<INesMenuElement> deletedGames = new List<INesMenuElement>();
        NesMenuCollection gamesCollection = new NesMenuCollection();
        MainForm mainForm;
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

        public TreeContructorForm(NesMenuCollection nesMenuCollection, MainForm mainForm = null)
        {
            try
            {
                InitializeComponent();
                gamesCollection = nesMenuCollection;
                this.mainForm = mainForm;
                if (File.Exists(FoldersXmlPath))
                {
                    try
                    {
                        XmlToTree(File.ReadAllText(FoldersXmlPath));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        File.Delete(FoldersXmlPath);
                        throw ex;
                    }
                }
                else DrawTree();
                splitContainer.Panel2MinSize = 485;
                treeView.TreeViewNodeSorter = new NodeSorter();
                listViewContent.ListViewItemSorter = new NodeSorter();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
#if DEBUG
                message += ex.StackTrace;
#endif
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void DrawTree()
        {
            cuttedNodes.Clear();
            treeView.Nodes.Clear();
            var rootNode = new TreeNode(Resources.MainMenu);
            treeView.Nodes.Add(rootNode);
            rootNode.Tag = gamesCollection;
            AddNodes(rootNode.Nodes, gamesCollection);
            rootNode.Expand();
            treeView.SelectedNode = rootNode;
        }

        void DrawSplitTree(NesMenuCollection.SplitStyle splitStyle = NesMenuCollection.SplitStyle.NoSplit)
        {
            var node = treeView.SelectedNode;
            NesMenuCollection collection;
            if (node.Tag is NesMenuFolder)
                collection = (node.Tag as NesMenuFolder).ChildMenuCollection;
            else if (node.Tag is NesMenuCollection)
                collection = node.Tag as NesMenuCollection;
            else return;
            // Collide and resplit collection
            collection.Unsplit();
            collection.Split(splitStyle, ConfigIni.MaxGamesPerFolder);
            // Refill nodes with new collection
            node.Nodes.Clear();
            AddNodes(node.Nodes, collection);
            node.Expand();
            treeView.SelectedNode = node;
            ShowSelected();
        }

        static int getImageIndex(INesMenuElement nesElement)
        {
            if (nesElement is NesMenuFolder || nesElement is NesMenuCollection)
                return 0;
            if (nesElement is NesDefaultGame)
                return 2;
            if (nesElement is NesGame)
                return 6;
            if (nesElement is NesUGame)
                return 8;
            if (nesElement is FdsGame)
                return 10;
            if (nesElement is SnesGame)
                return 12;
            if (nesElement is N64Game)
                return 14;
            if (nesElement is SmsGame)
                return 16;
            if (nesElement is GenesisGame)
                return 18;
            if (nesElement is PceGame)
                return 20;
            if (nesElement is GbGame)
                return 22;
            if (nesElement is GbcGame)
                return 24;
            if (nesElement is GbaGame)
                return 26;            
            
            return 4;
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
                }
                newNode.SelectedImageIndex = newNode.ImageIndex = getImageIndex(nesElement as INesMenuElement);
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
            DrawSplitTree(NesMenuCollection.SplitStyle.NoSplit);
        }

        private void buttonNoFoldersOriginal_Click(object sender, EventArgs e)
        {
            DrawSplitTree(NesMenuCollection.SplitStyle.Original_NoSplit);
        }

        private void buttonFoldersEqually_Click(object sender, EventArgs e)
        {
            DrawSplitTree(NesMenuCollection.SplitStyle.FoldersEqual);
        }

        private void buttonFoldersEquallyOriginal_Click(object sender, EventArgs e)
        {
            DrawSplitTree(NesMenuCollection.SplitStyle.Original_FoldersEqual);
        }

        private void buttonFoldersLetters_Click(object sender, EventArgs e)
        {
            DrawSplitTree(NesMenuCollection.SplitStyle.FoldersAlphabetic_FoldersEqual);
        }

        private void buttonFoldersLettersOriginal_Click(object sender, EventArgs e)
        {
            DrawSplitTree(NesMenuCollection.SplitStyle.Original_FoldersAlphabetic_FoldersEqual);
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
                groupBoxSplitModes.Enabled = true;
                pictureBoxArt.Cursor = Cursors.Hand;
                listViewContent.Enabled = true;
                foreach (TreeNode n in node.Nodes)
                {
                    var element = (INesMenuElement)n.Tag;
                    var item = new ListViewItem();
                    item.Text = element.Name;
                    var transparency = cuttedNodes.Contains(n) ? 1 : 0;
                    item.ImageIndex = getImageIndex(element) + transparency;
                    item.Tag = n;
                    listViewContent.Items.Add(item);
                }
            }
            else
            {
                if (node != null && node.Tag is NesMiniApplication)
                {
                    var game = node.Tag as NesMiniApplication;
                    pictureBoxArt.Image = NesMiniApplication.LoadBitmap(game.IconPath);
                    groupBoxArt.Enabled = true;
                    listViewContent.Enabled = false;
                }
                else //if (e.Node.Tag is NesDefaultGame)
                {
                    pictureBoxArt.Image = null;
                    groupBoxArt.Enabled = false;
                }
                listViewContent.Enabled = false;
                groupBoxSplitModes.Enabled = false;
                pictureBoxArt.Cursor = Cursors.Default;
            }
            ShowFolderStats();
        }

        void ShowFolderStats()
        {
            var node = treeView.SelectedNode;
            if (node != null && (node.Tag is NesMenuCollection || node.Tag is NesMenuFolder)) // Folder or root
            {
                labelElementCount.Text = string.Format(Resources.FolderStatistics, node.Text, node.Nodes.Count);
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
            if (!(e.Node.Tag is NesMenuFolder) ||
                string.IsNullOrEmpty(e.Label) || string.IsNullOrEmpty(e.Label.Trim()))
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
                MoveToFolder(newNodes, destinationNode, false);
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
            renameFolder((sender as ToolStripMenuItem).Tag);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Tag is TreeNode)
                deleteElements(new TreeNode[] { treeView.SelectedNode });
            else
                if ((sender as ToolStripMenuItem).Tag is ListView)
                    deleteElements(from i in listViewContent.SelectedItems.Cast<ListViewItem>().ToArray() select i.Tag as TreeNode);
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Tag is TreeNode)
                cutElements(new TreeNode[] { treeView.SelectedNode });
            else
                if ((sender as ToolStripMenuItem).Tag is ListView)
                    cutElements(from i in listViewContent.SelectedItems.Cast<ListViewItem>().ToArray() select i.Tag as TreeNode);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteElements(treeView.SelectedNode);
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is TreeView)
            {
                var destinationNode = ((TreeView)sender).GetNodeAt(e.X, e.Y);
                if (destinationNode == null) destinationNode = treeView.Nodes[0]; // Root

                newFolderToolStripMenuItem.Tag = deleteToolStripMenuItem.Tag = renameToolStripMenuItem.Tag =
                    cutToolStripMenuItem.Tag = pasteToolStripMenuItem.Tag = destinationNode;
                newFolderToolStripMenuItem.Enabled = true;
                if ((destinationNode.Tag is NesMenuFolder || destinationNode.Tag is NesMenuCollection)) // Folder
                    treeView.SelectedNode = destinationNode;
                else
                    newFolderToolStripMenuItem.Tag = destinationNode.Parent;
                renameToolStripMenuItem.Enabled = destinationNode.Tag is NesMenuFolder; // Folder
                cutToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = !(destinationNode.Tag is NesMenuCollection); // Not root
                pasteToolStripMenuItem.Enabled = cuttedNodes.Count > 0;
            }
            else
            {
                var item = ((ListView)sender).GetItemAt(e.X, e.Y);
                if (listViewContent.SelectedItems.Count == 0 && item != null) item.Selected = true;
                renameToolStripMenuItem.Tag = item;
                newFolderToolStripMenuItem.Tag = deleteToolStripMenuItem.Tag = cutToolStripMenuItem.Tag = pasteToolStripMenuItem.Tag = listViewContent;
                newFolderToolStripMenuItem.Enabled = treeView.SelectedNode != null && (treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection); // Folder                
                renameToolStripMenuItem.Enabled = (item != null) && (item.Tag as TreeNode).Tag is NesMenuFolder;
                cutToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = listViewContent.SelectedItems.Count > 0;
                pasteToolStripMenuItem.Enabled = cuttedNodes.Count > 0;
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
            if (destinationNode.Tag is NesMiniApplication || destinationNode.Tag is NesDefaultGame)
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
                treeView.SelectedNode = destinationNode;

            if (treeView.SelectedNode == destinationNode)
                ShowSelected();
            else
                foreach (var i in (from n in listViewContent.Items.Cast<ListViewItem>().ToArray() where newNodes.Contains(n.Tag as TreeNode) select n))
                    listViewContent.Items.Remove(i);
            foreach (ListViewItem item in listViewContent.Items)
                item.Selected = newNodes.Contains(item.Tag as TreeNode) || item.Tag == destinationNode;
            ShowFolderStats();
            return true;
        }


        void newFolder(TreeNode node = null)
        {
            var newnode = new TreeNode(Resources.FolderNameNewFolder, 0, 0);
            newnode.Tag = new NesMenuFolder(newnode.Text);
            if (node != null)
            {
                node.Nodes.Add(newnode);
                treeView.SelectedNode = newnode;
                ShowSelected();
                newnode.BeginEdit();
            }
            else if (treeView.SelectedNode != null)
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

        TreeNode getFolder(string name)
        {
            var root = treeView.Nodes[0];
            foreach (TreeNode el in root.Nodes)
            {
                if (el.Text == name && el.Tag is NesMenuFolder)
                    return el;
            }
            var newNode = new TreeNode(name, 0, 0);
            var newFolder = new NesMenuFolder(newNode.Text);
            newFolder.Position = NesMenuFolder.Priority.Leftmost;
            newNode.Tag = newFolder;
            (root.Tag as NesMenuCollection).Add(newFolder);
            root.Nodes.Add(newNode);
            return newNode;
        }

        void deleteElements(IEnumerable<TreeNode> nodes)
        {
            if (nodes.Count() == 1)
            {
                if (MessageBox.Show(this, string.Format(Resources.DeleteElement, nodes.First().Text),
                    Resources.AreYouSure, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            else
            {
                if (MessageBox.Show(this, string.Format(Resources.DeleteElements, nodes.Count()),
                    Resources.AreYouSure, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            bool needWarn = false;
            TreeNode parent = null;
            foreach (var node in nodes)
            {
                parent = node.Parent;
                if (node.Tag is NesMenuFolder) // Folder
                {
                    if (node.Nodes.Count > 0)
                    {
                        var unsortedFolder = getFolder(Resources.FolderNameTrashBin);
                        if (node.FullPath.StartsWith(unsortedFolder.FullPath)) // It's already in trash bin
                        {
                            (node.Tag as NesMenuFolder).ChildMenuCollection.Unsplit();
                            deletedGames.AddRange((node.Tag as NesMenuFolder).ChildMenuCollection);
                            (node.Tag as NesMenuFolder).ChildMenuCollection.Clear();
                        }
                        else
                        {
                            MoveToFolder(node.Nodes.Cast<TreeNode>().ToArray(), unsortedFolder, false);
                            needWarn = true;
                        }
                    }
                    if (parent.Tag is NesMenuFolder)
                        (parent.Tag as NesMenuFolder).ChildMenuCollection.Remove(node.Tag as INesMenuElement);
                    else if (parent.Tag is NesMenuCollection)
                        (parent.Tag as NesMenuCollection).Remove(node.Tag as INesMenuElement);
                    parent.Nodes.Remove(node);
                }
                else
                { // Game
                    var unsortedFolder = getFolder(Resources.FolderNameTrashBin);
                    if (node.FullPath.StartsWith(unsortedFolder.FullPath)) // It's already in trash bin
                    {
                        deletedGames.Add(node.Tag as INesMenuElement);
                        if (parent.Tag is NesMenuFolder)
                            (parent.Tag as NesMenuFolder).ChildMenuCollection.Remove(node.Tag as INesMenuElement);
                        else if (parent.Tag is NesMenuCollection)
                            (parent.Tag as NesMenuCollection).Remove(node.Tag as INesMenuElement);
                        parent.Nodes.Remove(node);
                    }
                    else
                    {
                        MoveToFolder(new TreeNode[] { node }, unsortedFolder, false);
                        needWarn = true;
                    }
                }
                foreach (var i in from i in listViewContent.Items.Cast<ListViewItem>().ToArray() where i.Tag == node select i)
                    listViewContent.Items.Remove(i);
                cuttedNodes.Remove(node);
            }
            if (parent != null)
                treeView.SelectedNode = parent;
            if (needWarn)
                MessageBox.Show(this, Resources.FolderContent, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            buttonOk.Enabled = treeView.Nodes[0].Nodes.Count > 0;
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
                if ((treeView.SelectedNode != null) && !(treeView.SelectedNode.Tag is NesMenuCollection))
                    deleteElements(new TreeNode[] { treeView.SelectedNode });
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
                if (treeView.SelectedNode != null)
                {
                    if ((treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection))
                        pasteElements(treeView.SelectedNode);
                    else
                        pasteElements(treeView.SelectedNode.Parent);
                }
            }
            else if (e.KeyCode == Keys.N && e.Modifiers == Keys.Control)
            {
                if (treeView.SelectedNode != null)
                {
                    if (treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection)
                        newFolder(treeView.SelectedNode);
                    else
                        newFolder(treeView.SelectedNode.Parent);
                }
            }
        }

        private void listViewContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteElements(from i in listViewContent.SelectedItems.Cast<ListViewItem>().ToArray() select i.Tag as TreeNode);
            }
            else if (e.KeyCode == Keys.F2 && e.Modifiers == Keys.None)
            {
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
                cutElements(from i in listViewContent.SelectedItems.Cast<ListViewItem>().ToArray() select i.Tag as TreeNode);
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (treeView.SelectedNode != null)
                {
                    if ((treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection))
                        pasteElements(treeView.SelectedNode);
                    else
                        pasteElements(treeView.SelectedNode.Parent);
                }
            }
            else if (e.KeyCode == Keys.N && e.Modifiers == Keys.Control)
            {
                if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag is NesMenuFolder || treeView.SelectedNode.Tag is NesMenuCollection))
                    newFolder();
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
            SaveTree();
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
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

        private void TreeContructorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || DialogResult == System.Windows.Forms.DialogResult.OK) return;
            var a = MessageBox.Show(this, Resources.FoldersSaveQ, this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (a == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (a == System.Windows.Forms.DialogResult.Yes)
                SaveTree();
            DialogResult = DialogResult.Cancel;
        }

        void SaveTree()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FoldersXmlPath));
            File.WriteAllText(FoldersXmlPath, TreeToXml());
            if (mainForm != null)
            {
                for (int i = 0; i < mainForm.checkedListBoxGames.Items.Count; i++)
                {
                    if (deletedGames.Contains(mainForm.checkedListBoxGames.Items[i] as NesMiniApplication))
                        mainForm.checkedListBoxGames.SetItemChecked(i, false);
                }
                for (int i = 0; i < mainForm.checkedListBoxDefaultGames.Items.Count; i++)
                {
                    if (deletedGames.Contains(mainForm.checkedListBoxDefaultGames.Items[i] as NesDefaultGame))
                        mainForm.checkedListBoxDefaultGames.SetItemChecked(i, false);
                }
                ConfigIni.Save();
            }
        }

        private string TreeToXml()
        {
            var root = treeView.Nodes[0];
            var xml = new XmlDocument();
            var treeNode = xml.CreateElement("Tree");
            xml.AppendChild(treeNode);
            NodeToXml(xml, treeNode, root);
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = new XmlTextWriter(stringWriter))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.WriteStartDocument();
                xml.WriteTo(xmlTextWriter);
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        private void NodeToXml(XmlDocument xml, XmlElement element, TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Tag is NesMenuFolder)
                {
                    var subElement = xml.CreateElement("Folder");
                    var folder = child.Tag as NesMenuFolder;
                    subElement.SetAttribute("name", folder.Name);
                    subElement.SetAttribute("icon", folder.ImageId);
                    subElement.SetAttribute("position", ((byte)folder.Position).ToString());
                    element.AppendChild(subElement);
                    NodeToXml(xml, subElement, child);
                }
                else if (child.Tag is NesMiniApplication)
                {
                    var subElement = xml.CreateElement("Game");
                    var game = child.Tag as NesMiniApplication;
                    subElement.SetAttribute("code", game.Code);
                    subElement.SetAttribute("name", game.Name);
                    element.AppendChild(subElement);
                }
                else if (child.Tag is NesDefaultGame)
                {
                    var subElement = xml.CreateElement("OriginalGame");
                    var game = child.Tag as NesDefaultGame;
                    subElement.SetAttribute("code", game.Code);
                    subElement.SetAttribute("name", game.Name);
                    element.AppendChild(subElement);
                }
            }
        }
        void XmlToTree(string xmlString)
        {
            gamesCollection.Unsplit();
            var oldCollection = new NesMenuCollection();
            oldCollection.AddRange(gamesCollection);
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            gamesCollection.Clear();
            XmlToNode(xml, xml.SelectSingleNode("/Tree").ChildNodes, oldCollection, gamesCollection);
            // oldCollection has only unsorted (new) games
            if (oldCollection.Count > 0)
            {
                NesMenuFolder unsorted;
                var unsorteds = from f in gamesCollection where f is NesMenuFolder && f.Name == Resources.FolderNameUnsorted select f;
                if (unsorteds.Count() > 0)
                    unsorted = unsorteds.First() as NesMenuFolder;
                else
                {
                    unsorted = new NesMenuFolder(Resources.FolderNameUnsorted);
                    unsorted.Position = NesMenuFolder.Priority.Leftmost;
                    gamesCollection.Add(unsorted);
                }
                foreach (var game in oldCollection)
                    unsorted.ChildMenuCollection.Add(game);
                MessageBox.Show(this, Resources.NewGamesUnsorted, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            DrawTree();
        }

        void XmlToNode(XmlDocument xml, XmlNodeList elements, NesMenuCollection rootMenuCollection, NesMenuCollection nesMenuCollection = null)
        {
            if (nesMenuCollection == null)
                nesMenuCollection = rootMenuCollection;
            foreach (XmlNode element in elements)
            {
                switch (element.Name)
                {
                    case "Folder":
                        var folder = new NesMenuFolder(element.Attributes["name"].Value, element.Attributes["icon"].Value);
                        folder.Position = (NesMenuFolder.Priority)byte.Parse(element.Attributes["position"].Value);
                        nesMenuCollection.Add(folder);
                        XmlToNode(xml, element.ChildNodes, rootMenuCollection, folder.ChildMenuCollection);
                        break;
                    case "Game":
                    case "OriginalGame":
                        var code = element.Attributes["code"].Value;
                        var games = from n in rootMenuCollection where ((n is NesMiniApplication || n is NesDefaultGame) && (n.Code == code)) select n;
                        if (games.Count() > 0)
                        {
                            var game = games.First();
                            nesMenuCollection.Add(game);
                            rootMenuCollection.Remove(game);
                        }
                        break;
                }
            }
        }
    }
}
