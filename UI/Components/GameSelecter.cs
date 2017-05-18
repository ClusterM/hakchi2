using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using com.clusterrr.hakchi_gui.Properties;
namespace com.clusterrr.hakchi_gui.UI.Components
{
    public partial class GameSelecter : UserControl
    {

        public GameSelecter()
        {
            InitializeComponent();
            
        }
        public void Init()
        {
            Manager.GameManager.GetInstance().GamesRemoved += GameSelecter_GamesRemoved;
            Manager.GameManager.GetInstance().NewGamesAdded += GameSelecter_NewGamesAdded;
            Manager.GameManager.GetInstance().SelectedChanged += GameSelecter_SelectedChanged;
            Manager.EventBus.getInstance().SearchRequest += GameSelecter_SearchRequest;
        }

        private void GameSelecter_SearchRequest(string text)
        {
            Search(text);
        }

        public event NesMiniApplication.ValueChangedHandler SelectedAppChanged;
        private void GameSelecter_SelectedChanged(NesMiniApplication app)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new NesMiniApplication.ValueChangedHandler(GameSelecter_SelectedChanged), new object[] { app });
            }
            else

            {
                /*Checked listbox*/
                for (int x = 0; x < checkedListBox1.Items.Count; x++)
                {
                    if (checkedListBox1.Items[x] == app)
                    {
                        checkedListBox1.SetItemChecked(x, app.Selected);
                        break;
                    }
                }
                /*Treeview*/
                TreeNode tn = FindNode(app, treeView1.Nodes);
                if (tn != null)
                {
                    if (tn.Checked != app.Selected)
                    {
                        tn.Checked = app.Selected;
                        if (tn.Parent != null)
                        {
                            ValidateFolderCheck(tn.Parent);
                        }
                    }
                }

            }
        
        }
        private void ValidateAllFolderCheck()
        {
            foreach(TreeNode tn in treeView1.Nodes)
            {
                ValidateFolderCheck(tn);
            }
        }
        private void ValidateFolderCheck(TreeNode tn)
        {
            /*check parent status*/
            bool needCheck = true;
            
                foreach (TreeNode ch in tn.Nodes)
                {
                    if (ch.Checked == false)
                    {
                        needCheck = false;
                        break;
                    }
                }
            
            if (tn.Checked != needCheck && !InMassCheck)
            {
                if (needCheck == false)
                {
                    InFolderUncheck = true;
                }
                tn.Checked = needCheck;
                if (needCheck == false)
                {
                    InFolderUncheck = false;
                }

            }
        }
        private bool InFolderUncheck = false;
        private TreeNode FindNodeStartWith(string text)
        {
            TreeNode ret = null;
            foreach (TreeNode tn in treeView1.Nodes)
            {
                if (tn.Text.ToLower().StartsWith(text))
                {
                    ret = tn;
                    break;
                }
                else
                {
                    TreeNode tn2 = FindNodeStartWith(text, tn);
                    if (tn2 != null)
                    {
                        ret = tn2;
                        break;
                    }

                }
            }
            return ret;
        }
        private TreeNode FindNodeStartWith(string text, TreeNode n)
        {
            TreeNode ret = null;
            foreach (TreeNode tn in n.Nodes)
            {
                if (tn.Text.ToLower().StartsWith(text))
                {
                    ret = tn;
                    break;

                }
                else
                {
                    TreeNode tn2 = FindNodeStartWith(text, tn);
                    if (tn2 != null)
                    {
                        ret = tn2;
                        break;
                    }
                }
            }
            return ret;
        }
        private TreeNode FindNode(NesMiniApplication app,TreeNodeCollection nodes)
        {
            TreeNode ret = null;
            foreach(TreeNode tn in nodes)
            {
                if(tn.Tag== app)
                {
                    ret = tn;
                    break;
                }
                else
                {
                    TreeNode tn2 = FindNode(app, tn.Nodes);
                    if(tn2 != null)
                    {
                        ret = tn2;
                        break;
                    }
                    
                }
            }
            return ret;
        }
  
        private void deleteGame(NesMiniApplication game)
        {
            try
            {
                
                if (MessageBox.Show(this, string.Format(Resources.DeleteGame, game.Name), Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    Manager.GameManager.GetInstance().DeleteGames(new List<NesMiniApplication>() { game });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private NesMiniApplication GetSelectedListBox()
        {
            NesMiniApplication ret = null;
            if (checkedListBox1.SelectedItem != null)
            {
                ret = (NesMiniApplication)checkedListBox1.SelectedItem;
            }
            return ret;
        }
        private NesMiniApplication GetSelectedTreeView()
        {
            NesMiniApplication ret = null;

            if(treeView1.SelectedNode!=null)
            {
                if(treeView1.SelectedNode.Tag is NesMiniApplication)
                {
                    ret = treeView1.SelectedNode.Tag as NesMiniApplication;
                }
            }

            return ret;
        }
        public string getSelectedTab()
        {
            return tabControl1.SelectedTab.Text;
        }
        public NesMiniApplication GetSelectedApp()
        {
            if(getSelectedTab() == "A->Z")
            {
                return GetSelectedListBox();
            }
            else
            {
                return GetSelectedTreeView();
            }
        }
        private TreeNode getSystemTreeNode(string System)
        {
            TreeNode ret = null;
            foreach(TreeNode tn in treeView1.Nodes)
            {
                if(tn.Text == System)
                {
                    ret = tn;
                    break;
                }
            }
            if(ret == null)
            {
                ret = new TreeNode(System);
               
                treeView1.Nodes.Add(ret);
                treeView1.Sort();
            }
            return ret;
        }
        private void GameSelecter_NewGamesAdded(List<NesMiniApplication> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Manager.GameManager.GameListEventHandler(GameSelecter_NewGamesAdded), new object[] { e });
            }
            else

            {
                /*Add to A->Z*/
                foreach (var game in e.OrderBy(o => o.Name))
                {
                    this.checkedListBox1.Items.Add(game, game.Selected);
                }
                /*Add to treeview*/
                foreach (var game in e.OrderBy(o => o.Name))
                {
                    string systemName = game.GetEmulator().SystemName;
                    TreeNode tn = getSystemTreeNode(systemName);
                    TreeNode gameNode = new TreeNode(game.Name);
                    gameNode.Tag = game;
                    gameNode.Checked = game.Selected;
                    tn.Nodes.Add(gameNode);
                }
                treeView1.Sort();
                ValidateAllFolderCheck();

            }

        }

        private void GameSelecter_GamesRemoved(List<NesMiniApplication> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Manager.GameManager.GameListEventHandler(GameSelecter_GamesRemoved), new object[] { e });
            }
            else

            {
                /*remove from checked listbox*/
                foreach (var game in e)
                {

                    checkedListBox1.Items.Remove(game);
                }
                /*remove from treeview*/
                foreach (var game in e)
                {
                    TreeNode tn = FindNode(game, treeView1.Nodes);
                    if (tn != null)
                    {
                        tn.Remove();
                    }
                }
                ValidateAllFolderCheck();
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            NesMiniApplication app = (NesMiniApplication)this.checkedListBox1.Items[e.Index];
            app.Selected = (e.NewValue == CheckState.Checked);
       
        }

        private void checkedListBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && checkedListBox1.SelectedIndex > 0)
            {
                Manager.GameManager.GetInstance().DeleteGames(new List<NesMiniApplication>() { GetSelectedApp() });

            }
        }
        public void Search(string text)
        {
            /*Checked list box*/
            for(int x=0;x<checkedListBox1.Items.Count;x++)
            {
                if((checkedListBox1.Items[x] as NesMiniApplication).Name.ToLower().StartsWith(text.ToLower()))
                {
                    checkedListBox1.SelectedIndex = x;
                    break;
                }
            }
            /*treeview*/
            TreeNode tn = FindNodeStartWith(text);
            if(tn!=null)
            {
                treeView1.SelectedNode = tn;
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(checkedListBox1_SelectedIndexChanged), new object[] { sender, e });
            }
            else
            {
                if (getSelectedTab() == "A->Z")
                {
                    if (SelectedAppChanged != null)
                    {
                        SelectedAppChanged(GetSelectedApp());
                    }
                }
            }
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < checkedListBox1.Items.Count; x++)
            {
                var app = checkedListBox1.Items[x] as NesMiniApplication;
                if(!app.Selected)
                {
                    app.Selected = true;
                }
            }
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < checkedListBox1.Items.Count; x++)
            {
                var app = checkedListBox1.Items[x] as NesMiniApplication;
                if (app.Selected)
                {
                    app.Selected = false;
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(GetSelectedApp() == null || GetSelectedApp().GetType() == typeof(NesDefaultGame))
            {
                deleteSelectedToolStripMenuItem.Enabled = false;
            }
            else
            {
                deleteSelectedToolStripMenuItem.Enabled = true;
            }
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager.GameManager.GetInstance().DeleteGames(new List<NesMiniApplication>() { GetSelectedApp() });
        }
        private bool InMassCheck = false;
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            
            if (e.Node.Tag== null)
            {
                if(e.Node.Nodes!=null)
                {
                    if (!InFolderUncheck)
                    {
                        NesMiniApplication last = null;
                        Debug.WriteLine(e.Node.Text + " - " + e.Node.Checked.ToString());
                        Manager.GameManager.GetInstance().SelectedChangeBatch = true;
                        treeView1.BeginUpdate();
                        if(e.Node.Checked)
                        {
                            InMassCheck = true;
                        }
                        foreach (TreeNode tn in e.Node.Nodes)
                        {
                            if(tn.Tag != null  && tn.Tag is NesMiniApplication)
                            {
                                last = tn.Tag as NesMiniApplication;
                            }
                            if (tn.Checked != e.Node.Checked)
                            {
                                tn.Checked = e.Node.Checked;
                            }
                        }
                        if (e.Node.Checked)
                        {
                            InMassCheck = false;
                        }
                        treeView1.EndUpdate();
                        
                        Manager.GameManager.GetInstance().SelectedChangeBatch = false;
                        if (!Manager.GameManager.LoadingLibrary)
                        {
                            Manager.EventBus.getInstance().SizeRecalculationRequest();
                        }
                   
                       
                    }
                }
            }
            else
            {
                NesMiniApplication app = e.Node.Tag as NesMiniApplication;
                app.Selected = e.Node.Checked;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView1.Refresh();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("Doubleclick");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(getSelectedTab() != "A->Z")
            {
                if(treeView1.SelectedNode.Tag!=null)
                {
                    if (SelectedAppChanged != null)
                    {
                        SelectedAppChanged(GetSelectedApp());
                    }
                }
            }
        }
    }
}
