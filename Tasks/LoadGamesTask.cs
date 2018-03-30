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
    public class LoadGamesTask
    {
        enum ViewGroup { New, NoCoverArt, Original, Custom, All, Unknown }
        public List<NesApplication> Games
            { get; private set; }
        public Dictionary<NesApplication, string> GamesChanged
            { get; private set; }
        public ListView ListViewGames
            { get; set; }

        public LoadGamesTask(bool reload = false)
        {
            this.Games = new List<NesApplication>();
            this.GamesChanged = new Dictionary<NesApplication, string>();
            this.ListViewGames = null;
            this.reload = reload;
        }

        // --- needed for updatelistview
        private class LoadGamesSyncObject
        {
            public ListViewItem[] items;
            public ListViewGroup[] groups;
        }
        private bool reload;

        // --- groups
        private Dictionary<ViewGroup, ListViewGroup> normalGroups = null;
        private SortedDictionary<string, ListViewGroup> sortedGroups = null;
        private SortedDictionary<string, ListViewGroup> sortedCustomGroups = null;

        // --- update listview ui element
        public TaskerForm.Conclusion UpdateListView(TaskerForm tasker, Object syncObject = null)
        {
            if (ListViewGames.Disposing) return TaskerForm.Conclusion.Undefined;
            if (ListViewGames.InvokeRequired)
            {
                return (TaskerForm.Conclusion)ListViewGames.Invoke(new Func<TaskerForm, Object, TaskerForm.Conclusion>(UpdateListView), new object[] { tasker, syncObject });
            }

            var sync = (LoadGamesSyncObject)syncObject;
            try
            {
                ListViewGames.BeginUpdate();
                ListViewGames.Groups.Clear();
                ListViewGames.Items.Clear();
                ListViewGames.Groups.AddRange(sync.groups);
                ListViewGames.Items.AddRange(sync.items);
            }
            catch (InvalidOperationException) { }
            finally
            {
                ListViewGames.EndUpdate();
            }
            return TaskerForm.Conclusion.Success;
        }

        // --- create groups
        public TaskerForm.Conclusion CreateListViewGroups(TaskerForm tasker, Object syncObject = null)
        {
            this.normalGroups = new Dictionary<ViewGroup, ListViewGroup>();
            this.sortedGroups = new SortedDictionary<string, ListViewGroup>();
            this.sortedCustomGroups = new SortedDictionary<string, ListViewGroup>();

            // standard groups
            var h = HorizontalAlignment.Center;
            normalGroups.Add(ViewGroup.New, new ListViewGroup(Resources.ListCategoryNew, h));
            normalGroups.Add(ViewGroup.NoCoverArt, new ListViewGroup(Resources.ListCategoryNoCoverArt, h));
            normalGroups.Add(ViewGroup.Original, new ListViewGroup(Resources.ListCategoryOriginal, h));
            normalGroups.Add(ViewGroup.Custom, new ListViewGroup(Resources.ListCategoryCustom, h));
            normalGroups.Add(ViewGroup.All, new ListViewGroup(Resources.ListCategoryAll, h));
            normalGroups.Add(ViewGroup.Unknown, new ListViewGroup(Resources.ListCategoryUnknown, h));

            // order by system/core groups
            if (ConfigIni.Instance.GamesSorting == MainForm.GamesSorting.System)
            {
                foreach (var system in CoreCollection.Systems)
                    sortedGroups[system] = new ListViewGroup(system, h);
                foreach (var appInfo in AppTypeCollection.Apps)
                    if (!sortedGroups.ContainsKey(appInfo.Name))
                    {
                        sortedGroups[appInfo.Name] = new ListViewGroup(appInfo.Name, h);
                    }
            }
            else if (ConfigIni.Instance.GamesSorting == MainForm.GamesSorting.Core)
            {
                foreach (var core in CoreCollection.Cores)
                    sortedGroups[core.Bin] = new ListViewGroup(core.Name, h);
            }
            return TaskerForm.Conclusion.Success;
        }

        // --- assign groups to list view
        public TaskerForm.Conclusion AssignListViewGroups(TaskerForm tasker, Object syncObject = null)
        {
            var sync = (LoadGamesSyncObject)syncObject;

            var groups = new List<ListViewGroup>();
            if (ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.AtTop)
            {
                groups.Add(normalGroups[ViewGroup.New]);
                groups.Add(normalGroups[ViewGroup.NoCoverArt]);
                groups.Add(normalGroups[ViewGroup.Original]);
                if (ConfigIni.Instance.GamesSorting != MainForm.GamesSorting.Name)
                {
                    foreach (var group in sortedGroups) groups.Add(group.Value);
                    foreach (var group in sortedCustomGroups) groups.Add(group.Value);
                }
                else
                    groups.Add(normalGroups[ViewGroup.Custom]);
                groups.Add(normalGroups[ViewGroup.Unknown]);
            }
            else if (ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.AtBottom)
            {
                groups.Add(normalGroups[ViewGroup.New]);
                groups.Add(normalGroups[ViewGroup.NoCoverArt]);
                if (ConfigIni.Instance.GamesSorting != MainForm.GamesSorting.Name)
                {
                    foreach (var group in sortedGroups) groups.Add(group.Value);
                    foreach (var group in sortedCustomGroups) groups.Add(group.Value);
                }
                else
                    groups.Add(normalGroups[ViewGroup.Custom]);
                groups.Add(normalGroups[ViewGroup.Unknown]);
                groups.Add(normalGroups[ViewGroup.Original]);
            }
            else if (ConfigIni.Instance.GamesSorting != MainForm.GamesSorting.Name)
            {
                groups.Add(normalGroups[ViewGroup.New]);
                groups.Add(normalGroups[ViewGroup.NoCoverArt]);
                foreach (var group in sortedGroups) groups.Add(group.Value);
                foreach (var group in sortedCustomGroups) groups.Add(group.Value);
                groups.Add(normalGroups[ViewGroup.Unknown]);
            }
            else if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
            {
                groups.Add(normalGroups[ViewGroup.New]);
                groups.Add(normalGroups[ViewGroup.NoCoverArt]);
                groups.Add(normalGroups[ViewGroup.All]);
            }
            sync.groups = groups.ToArray();
            return TaskerForm.Conclusion.Success;
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

            var items = new List<ListViewItem>();
            int i = 0;
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    try
                    {
                        var game = NesApplication.FromDirectory(gameDir);
                        items.Add(new ListViewItem(game.Name) { Tag = game });
                    }
                    catch // remove bad directories if any, no throw
                    {
                        Debug.WriteLine($"Game directory \"{gameDir}\" is invalid, deleting");
                        Directory.Delete(gameDir, true);
                    }
                    tasker.SetProgress(i, gameDir.Length);
                }
                catch (Exception ex)
                {
                    tasker.ShowError(ex, true);
                    return TaskerForm.Conclusion.Error;
                }
            }
            sync.items = ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Hidden ?
                items.Where(item => !(item.Tag as NesApplication).IsOriginalGame).ToArray() :
                items.ToArray();

            return TaskerForm.Conclusion.Success;
        }

        // --- grab files/items from existing list
        public TaskerForm.Conclusion LoadGamesFromList(TaskerForm tasker, Object syncObject = null)
        {
            var sync = (LoadGamesSyncObject)syncObject;

            sync.items = new ListViewItem[ListViewGames.Items.Count];
            ListViewGames.Invoke(new Action(() => {
                int i = 0;
                foreach (ListViewItem item in ListViewGames.Items)
                {
                    sync.items[i++] = (ListViewItem)item.Clone();
                }
            }));

            return TaskerForm.Conclusion.Success;
        }

        // --- as the title says, assigns groups to games
        public TaskerForm.Conclusion AssignGroupsToGames(TaskerForm tasker, Object syncObject = null)
        {
            var sync = (LoadGamesSyncObject)syncObject;
            var selected = ConfigIni.Instance.SelectedGames;
            var original = ConfigIni.Instance.OriginalGames;

            // assign groups to list view items
            // int i = 0;
            foreach (var item in sync.items)
            {
                var game = item.Tag as NesApplication;

                item.Checked = selected.Contains(game.Code) || original.Contains(game.Code);
                ListViewGroup group = null;
                if (game.IsOriginalGame && ConfigIni.Instance.OriginalGamesPosition != MainForm.OriginalGamesPosition.Sorted)
                {
                    group = normalGroups[ViewGroup.Original];
                }
                if (group == null)
                {
                    AppTypeCollection.AppInfo appInfo = game.Metadata.AppInfo;
                    switch (ConfigIni.Instance.GamesSorting)
                    {
                        case MainForm.GamesSorting.Name:
                            group = game.IsOriginalGame ? normalGroups[ViewGroup.Original] : normalGroups[ViewGroup.Custom];
                            break;
                        case MainForm.GamesSorting.System:
                            if (!appInfo.Unknown)
                                group = sortedGroups[appInfo.Name];
                            else if (!string.IsNullOrEmpty(game.Metadata.System) && sortedGroups.ContainsKey(game.Metadata.System))
                                group = sortedGroups[game.Metadata.System];
                            break;
                        case MainForm.GamesSorting.Core:
                            if (string.IsNullOrEmpty(game.Metadata.Core))
                            {
                                int startPos = game.Desktop.Bin.LastIndexOf('/') + 1;
                                if (startPos > 0 && (startPos < game.Desktop.Bin.Length))
                                {
                                    string bin = game.Desktop.Bin.Substring(startPos).Trim();
                                    if (sortedGroups.ContainsKey(bin))
                                        group = sortedGroups[bin];
                                }
                            }
                            else if (sortedGroups.ContainsKey(game.Metadata.Core))
                                group = sortedGroups[game.Metadata.Core];
                            break;
                    }
                }
                if (group == null)
                {
                    if (game.Desktop.Bin.Trim().Length == 0)
                        group = normalGroups[ViewGroup.Unknown];
                    else
                    {
                        string bin = game.Desktop.Bin.Trim();
                        if (!sortedCustomGroups.ContainsKey(bin))
                            sortedCustomGroups.Add(bin, new ListViewGroup(bin, HorizontalAlignment.Center));
                        group = sortedCustomGroups[bin];
                    }
                }
                if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
                {
                    if (!game.Metadata.CustomCoverArt)
                        group = normalGroups[ViewGroup.NoCoverArt];
                    else if (ConfigIni.Instance.GamesSorting == MainForm.GamesSorting.Name)
                    {
                        if (ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Sorted || ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Hidden)
                            group = normalGroups[ViewGroup.All];
                    }
                }
                item.Group = group;
            }

            return TaskerForm.Conclusion.Success;
        }

        public TaskerForm.Conclusion LoadGames(TaskerForm tasker, Object syncObject = null)
        {
            tasker.SetProgress(-1, -1, TaskerForm.State.Running, Resources.LoadingGames);
            tasker.SetTitle(Resources.LoadingGames);

            tasker.SyncObject = new LoadGamesSyncObject();
            tasker.AddTasks(
                CreateListViewGroups,
                this.reload ?
                    (TaskerForm.TaskFunc)LoadGamesFromList : 
                    (TaskerForm.TaskFunc)LoadGamesFromFiles,
                AssignGroupsToGames,
                AssignListViewGroups,
                UpdateListView);

            return TaskerForm.Conclusion.Success;
        }

        public TaskerForm.Conclusion SetCoverArtForMultipleGames(TaskerForm tasker, Object SyncObject = null)
        {
            tasker.SetTitle(Resources.ApplyChanges);
            tasker.SetProgress(0, 100, TaskerForm.State.Running, Resources.ApplyChanges);

            int i = 0, max = GamesChanged.Count;
            foreach(var pair in GamesChanged)
            {
                pair.Key.SetImageFile(pair.Value, ConfigIni.Instance.CompressCover);
                tasker.SetProgress(++i, max);
            }

            return TaskerForm.Conclusion.Success;
        }
            
    }
}
