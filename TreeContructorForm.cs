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
        public TreeContructorForm(NesMenuCollection nesMenuCollection)
        {
            InitializeComponent();
            treeView.Nodes.Clear();
            AddNodes(treeView.Nodes, nesMenuCollection);
        }

        void AddNodes(TreeNodeCollection treeNodeCollection, NesMenuCollection nesMenuCollection, List<NesMenuCollection> usedFolders = null)
        {
            if (usedFolders == null)
                usedFolders = new List<NesMenuCollection>();
            if (usedFolders.Contains(nesMenuCollection))
                return;
            usedFolders.Add(nesMenuCollection);
            var sorted = nesMenuCollection.OrderBy(o => o.Name);
            foreach (var nesElement in sorted)
            {
                var newNode = new TreeNode();
                newNode.Tag = nesElement;
                if (nesElement is NesMenuFolder)
                {
                    newNode.Text = string.Format("{0} ({1} elements)", nesElement.Name.Substring(1), (nesElement as NesMenuFolder).Child.Count);
                    newNode.SelectedImageIndex = newNode.ImageIndex = 0;
                }
                else if (nesElement is NesGame)
                {
                    newNode.Text = nesElement.Name;
                    newNode.SelectedImageIndex = newNode.ImageIndex = 1;
                }
                else if (nesElement is NesDefaultGame)
                {
                    newNode.Text = nesElement.Name;
                    newNode.SelectedImageIndex = newNode.ImageIndex = 2;
                }
                treeNodeCollection.Add(newNode);
                if (nesElement is NesMenuFolder)
                {
                    AddNodes(newNode.Nodes, (nesElement as NesMenuFolder).Child, usedFolders);
                }
            }
        }
    }
}
