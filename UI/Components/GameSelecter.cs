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
        public void RefreshGames()
        {
            string[] selectedGames = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if(!System.IO.Directory.Exists(NesMiniApplication.GamesDirectory))
            {
                Directory.CreateDirectory(NesMiniApplication.GamesDirectory);
            }
            var gameDirs = Directory.GetDirectories(NesMiniApplication.GamesDirectory);
            var games = new List<NesMiniApplication>();
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var game = NesMiniApplication.FromDirectory(gameDir);
                        games.Add(game);
                    }
                    catch (FileNotFoundException ex) // Remove bad directories if any
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        Directory.Delete(gameDir, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }
            var gamesSorted = games.OrderBy(o => o.Name);
            
            tvGameSelecter.Nodes.Clear();
            TreeNode tn = new TreeNode(Resources.Default30games);
            tn.Checked = selectedGames.Contains("default");
            tvGameSelecter.Nodes.Add(tn);
            Dictionary<string, TreeNode> consolesTreeNode = new Dictionary<string, TreeNode>();
            foreach (var game in gamesSorted)
            {
                if(!consolesTreeNode.ContainsKey(AppTypeCollection.GetAppByClass(game.GetType()).SystemName))
                {
                    AppTypeCollection.AppInfo inf = AppTypeCollection.GetAppByClass(game.GetType());
                    TreeNode systemNode = new TreeNode(inf.SystemName);
                    systemNode.Tag = inf;
                    consolesTreeNode[inf.SystemName]= systemNode;
                    tvGameSelecter.Nodes.Add(systemNode);

                }
                TreeNode gameNode = new TreeNode(game.Name);
                gameNode.Tag = game;
                gameNode.Checked = selectedGames.Contains(game.Code);
                consolesTreeNode[AppTypeCollection.GetAppByClass(game.GetType()).SystemName].Nodes.Add(gameNode);
       
            }
           // RecalculateSelectedGames();
           // ShowSelected();
        }
        /*  Debug.WriteLine("Loading games");
            var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Directory.CreateDirectory(NesMiniApplication.GamesDirectory);
            var gameDirs = Directory.GetDirectories(NesMiniApplication.GamesDirectory);
            var games = new List<NesMiniApplication>();
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var game = NesMiniApplication.FromDirectory(gameDir);
                        games.Add(game);
                    }
                    catch (FileNotFoundException ex) // Remove bad directories if any
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        Directory.Delete(gameDir, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }

            var gamesSorted = games.OrderBy(o => o.Name);
            checkedListBoxGames.Items.Clear();
            checkedListBoxGames.Items.Add(Resources.Default30games, selected.Contains("default"));
            foreach (var game in gamesSorted)
            {
                checkedListBoxGames.Items.Add(game, selected.Contains(game.Code));
            }
            RecalculateSelectedGames();
            ShowSelected();*/
    }
}
