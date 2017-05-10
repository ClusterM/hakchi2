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
            for(int x=0;x<checkedListBox1.Items.Count;x++)
            {
                if(checkedListBox1.Items[x] == app)
                {
                    checkedListBox1.SetItemChecked(x, app.Selected);
                    break;
                }
            }
        
        }
        private void deleteGame(int pos)
        {
            try
            {
                var game = checkedListBox1.Items[pos] as NesMiniApplication;
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
        public NesMiniApplication GetSelectedApp()
        {
            NesMiniApplication ret = null;
            if(checkedListBox1.SelectedItem != null)
            {
                ret = (NesMiniApplication)checkedListBox1.SelectedItem;
            }
            return ret;
        }
        private void GameSelecter_NewGamesAdded(List<NesMiniApplication> e)
        {
            foreach (var game in e.OrderBy(o => o.Name))
            {
                this.checkedListBox1.Items.Add(game, game.Selected);
            }
        }

        private void GameSelecter_GamesRemoved(List<NesMiniApplication> e)
        {
            foreach (var game in e)
            {
        
                checkedListBox1.Items.Remove(game);
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
            for(int x=0;x<checkedListBox1.Items.Count;x++)
            {
                if((checkedListBox1.Items[x] as NesMiniApplication).Name.ToLower().StartsWith(text.ToLower()))
                {
                    checkedListBox1.SelectedIndex = x;
                    break;
                }
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(SelectedAppChanged != null)
            {
                SelectedAppChanged(GetSelectedApp());
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
    }
}
