using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class GameTask
    {
        public List<NesApplication> Games
        {
            get; private set;
        }

        public Dictionary<NesApplication, string> GamesChanged
        {
            get; private set;
        }

        public GameTask()
        {
            Games = new List<NesApplication>();
            GamesChanged = new Dictionary<NesApplication, string>();
        }

        // --- scan files and create internal list
        public TaskerForm.Conclusion LoadGamesFromFiles(TaskerForm tasker, Object syncObject = null)
        {
            var sync = (LoadGamesSyncObject)syncObject;

            // list original game directories
            var originalGameDirs = new List<string>();
            foreach (var defaultGame in NesApplication.DefaultGames)
            {
                string gameDir = Path.Combine(NesApplication.OriginalGamesDirectory, defaultGame.Code);
                if (Directory.Exists(gameDir))
                    originalGameDirs.Add(gameDir);
            }

            // add custom games
            Directory.CreateDirectory(NesApplication.GamesDirectory);
            var gameDirs = Shared.ConcatArrays(Directory.GetDirectories(NesApplication.GamesDirectory), originalGameDirs.ToArray());

            var games = new List<ListViewItem>();

            foreach (var gameDir in gameDirs)
            {
                try
                {
                    try
                    {
                        var game = NesApplication.FromDirectory(gameDir);
                        games.Add(new ListViewItem(game.Name) { Tag = game });
                    }
                    catch // remove bad directories if any, no throw
                    {
                        Debug.WriteLine($"Game directory \"{gameDir}\" is invalid, deleting");
                        Directory.Delete(gameDir, true);
                    }
                }
                catch (Exception ex)
                {
                    tasker.ShowError(ex, true);
                    return TaskerForm.Conclusion.Error;
                }
            }
            sync.items = games.ToArray();

            return TaskerForm.Conclusion.Success;
        }

        // --- grab files/items from existing list
        public TaskerForm.Conclusion LoadGamesFromList(TaskerForm tasker, Object syncObject = null)
        {
            var sync = (LoadGamesSyncObject)syncObject;

            sync.items = new ListViewItem[ListViewGames.Items.Count];
            ListViewGames.Items.CopyTo(sync.items, 0);

            return TaskerForm.Conclusion.Success;
        }

        // --- as the title says, assigns groups to games
        public TaskerForm.Conclusion AssignGroupsToGames(TaskerForm tasker, Object syncObject = null)
        {
            tasker.SetTitle(Resources.ApplyChanges);
            tasker.SetProgress(0, 100, TaskerForm.State.Running, Resources.ApplyChanges);

            // assign groups to list view items
            foreach (var item in sync.items)
            {
                pair.Key.SetImageFile(pair.Value, ConfigIni.Instance.CompressCover);
                tasker.SetProgress(++i, max);
            }

            return TaskerForm.Conclusion.Success;
        }

        public TaskerForm.Conclusion RepairGames(TaskerForm tasker, Object SyncObject = null)
        {
            tasker.SetTitle(Resources.RepairGames);
            tasker.SetState(TaskerForm.State.Running);

            NesApplication.ParentForm = tasker;
            int i = 0, max = Games.Count;
            foreach (var game in Games)
            {
                tasker.SetStatus(string.Format(Resources.RepairingGame, game.Name));
                bool success = game.Repair();
                Debug.WriteLine($"Repairing game \"{game.Name}\" was " + (success ? "successful" : "not successful"));
                tasker.SetProgress(++i, max);
            }

            return TaskerForm.Conclusion.Success;
        }
    }
}
