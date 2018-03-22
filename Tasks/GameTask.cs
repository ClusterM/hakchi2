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
        enum ViewGroup { New, NoCoverArt, Original, Custom, All, Unknown }
        public List<NesApplication> Games
        {
            get; private set;
        }
        public ListView ListViewGames
        {
            get; set;
        }

        public GameTask()
        {
            Games = new List<NesApplication>();
            ListViewGames = null;
        }

        public TaskerForm.Conclusion LoadGames(TaskerForm tasker, Object syncObject = null)
        {
            tasker.SetTitle("Loading games");
            tasker.SetProgress(0, 220, TaskerForm.State.Starting, "Loading games...");
            var selected = ConfigIni.Instance.SelectedGames;

            // groups
            var normalGroups = new Dictionary<ViewGroup, ListViewGroup>();
            var sortedGroups = new SortedDictionary<string, ListViewGroup>();
            var sortedCustomGroups = new SortedDictionary<string, ListViewGroup>();

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

            tasker.SetState(TaskerForm.State.Running);
            Games = new List<NesApplication>();
            int i = 0;
            foreach (var gameDir in gameDirs)
            {
                tasker.SetProgress((int)((double)i++ / gameDirs.Length * 100), 220);
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var game = NesApplication.FromDirectory(gameDir);
                        Games.Add(game);
                    }
                    catch // Remove bad directories if any
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
            else if(ConfigIni.Instance.GamesSorting == MainForm.GamesSorting.Core)
            {
                foreach (var core in CoreCollection.Cores)
                    sortedGroups[core.Bin] = new ListViewGroup(core.Name, h);
            }

            // convert games to listviewitems
            var items = new List<ListViewItem>();
            i = 0;
            foreach (var game in Games)
            {
                tasker.SetProgress((int)((double)i++ / Games.Count * 100) + 100, 220);

                // escape tests
                if (ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Hidden && game.IsOriginalGame)
                    continue;

                var listViewItem = new ListViewItem(game.Name);
                listViewItem.Tag = game;
                listViewItem.Checked = selected.Contains(game.Code) || ConfigIni.Instance.HiddenGames.Contains(game.Code);

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
                            sortedCustomGroups.Add(bin, new ListViewGroup(bin, h));
                        group = sortedCustomGroups[bin];
                    }
                }
                if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
                {
                    if (!game.Metadata.CustomCoverArt)
                        group = normalGroups[ViewGroup.NoCoverArt];
                    else if (ConfigIni.Instance.GamesSorting == MainForm.GamesSorting.Name)
                    {
                        if(ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Sorted || ConfigIni.Instance.OriginalGamesPosition == MainForm.OriginalGamesPosition.Hidden)
                            group = normalGroups[ViewGroup.All];
                    }
                }
                listViewItem.Group = group;
                items.Add(listViewItem);
            }

            // add groups in the right order
            tasker.SetProgress(200, 220, TaskerForm.State.Finishing, "Adding groups");
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
                groups.Add(normalGroups[ViewGroup.NoCoverArt]);
                groups.Add(normalGroups[ViewGroup.All]);
            }

            // add games to list
            tasker.SetProgress(210, 220, TaskerForm.State.Finishing, "Adding games and groups to the list");
            UpdateListView(ListViewGames, items.ToArray(), groups.ToArray());
            return TaskerForm.Conclusion.Success;
        }

        public void UpdateListView(ListView listView, ListViewItem[] items, ListViewGroup[] groups)
        {
            if (listView.Disposing) return;
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action<ListView,ListViewItem[],ListViewGroup[]>(UpdateListView), new object[] { listView, items, groups });
                return;
            }
            try
            {
                listView.BeginUpdate();
                listView.Groups.Clear();
                listView.Items.Clear();
                listView.Groups.AddRange(groups);
                listView.Items.AddRange(items);
            }
            catch (InvalidOperationException) { }
            finally
            {
                listView.EndUpdate();
            }
        }
    }
}
