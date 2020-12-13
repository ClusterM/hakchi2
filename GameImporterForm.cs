using ArxOne.Ftp;
using com.clusterrr.clovershell;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Tasks;
using com.clusterrr.util;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui
{
    public partial class GameImporterForm : Form
    {
        public bool gameCopied = false;
        public IEnumerable<FoundGame> SelectedGames => listViewGames.SelectedItems.Cast<ListViewItem>().Where(item => item.Tag is FoundGame).Select(item => item.Tag as FoundGame);
        public static string NoSelection => Resources.NoItemsSelected;
        public static string SingleSelection => Resources._0ItemSelected1;
        public static string MultiSelection => Resources._0ItemsSelected1;
        public class FoundGame
        {
            public string RemotePath;
            public DesktopFile Desktop;
            public long Size;
        }

        public GameImporterForm()
        {
            InitializeComponent();
            labelStatus.Text = NoSelection;
        }

        public GameImporterForm(List<FoundGame> games): this()
        {
            foreach (var game in games.OrderBy(e => e.Desktop.Name))
            {
                var item = new ListViewItem();
                listViewGames.Items.Add(new ListViewItem(new ListViewItem.ListViewSubItem[] {
                            new  ListViewItem.ListViewSubItem() { Text = game.Desktop.Name },
                            new  ListViewItem.ListViewSubItem() { Text = game.RemotePath },
                            new  ListViewItem.ListViewSubItem() { Text = Shared.SizeSuffix(game.Size, 2) },
                        }, 0)
                {
                    Tag = game
                });
            }
        }

        public static TaskFunc FindGamesTask(Dictionary<String, FoundGame> foundGames)
        {
            return (Tasker tasker, Object taskObject) =>
            {
                tasker.SetStatus(Resources.Scanning);
                if (hakchi.Shell.IsOnline)
                {
                    var mountpoint = hakchi.Shell is ClovershellConnection ? "" : hakchi.Shell.ExecuteSimple("hakchi get mountpoint", throwOnNonZero: true);
                    var rootfs = hakchi.Shell is ClovershellConnection ? "/var/lib/hakchi/rootfs" : hakchi.Shell.ExecuteSimple("hakchi get rootfs", throwOnNonZero: true);
                    var searchPaths = new string[]
                    {
                        $"{rootfs}/usr/share/games",
                        $"{mountpoint}/var/lib/hakchi/games",
                        $"{mountpoint}/media/hakchi/games"
                    };
                    using (var desktopTarStream = new MemoryStream())
                    using (var sizeStream = new MemoryStream())
                    {
                        var paths = "";
                        foreach (var path in searchPaths)
                        {
                            paths = $"{paths} {Shared.EscapeShellArgument(path)}";
                        }

                        hakchi.Shell.Execute($"find {paths} -name \"CLV-*.desktop\" | sort | tar -cf - -T -", null, desktopTarStream);
                        desktopTarStream.Seek(0, SeekOrigin.Begin);

                        var sizeRegex = new Regex(@"^(\d+)\s*(/.*)$", RegexOptions.Multiline);
                        if (desktopTarStream.Length > 0)
                        {
                            hakchi.Shell.Execute($"du {paths}", null, sizeStream);
                            sizeStream.Seek(0, SeekOrigin.Begin);

                            using (var extractor = ArchiveFactory.Open(desktopTarStream))
                            using (var reader = extractor.ExtractAllEntries())
                            {

                                while (reader.MoveToNextEntry())
                                {
                                    var entry = reader.Entry;
                                    Trace.WriteLine(entry.Key);
                                    using (var entryStream = reader.OpenEntryStream())
                                    {
                                        var key = Path.GetDirectoryName($"/{entry.Key}").Replace('\\', '/');
                                        var desktop = new DesktopFile(entryStream);

                                        if (desktop.IconPath.EndsWith("/.storage"))
                                        {
                                            // This is a linked game
                                            key = $"{mountpoint}{desktop.IconPath}/{desktop.Code}";
                                        }

                                        desktop.Exec = desktop.Exec.Replace(desktop.IconPath, "/var/games");
                                        desktop.IconPath = "/var/games";
                                        desktop.ProfilePath = "/var/saves";

                                        if (!NesApplication.AllDefaultGames.ContainsKey(desktop.Code) && desktop.Bin != "/bin/chmenu")
                                        {
                                            foundGames.Add(key, new FoundGame()
                                            {
                                                RemotePath = key,
                                                Desktop = desktop,
                                                Size = 0
                                            });
                                        }
                                    }
                                }
                            }

                            using (var sr = new StreamReader(sizeStream))
                            {
                                var matches = sizeRegex.Matches(sr.ReadToEnd());
                                foreach (Match match in matches)
                                {
                                    var size = long.Parse(match.Groups[1].Value) * 1024;
                                    var path = match.Groups[2].Value;
                                    if (foundGames.ContainsKey(path))
                                    {
                                        foundGames[path].Size = size;
                                    }
                                }
                            }
                        }
                    }
                    return Conclusion.Success;
                }
                return Conclusion.Error;
            };
        }

        private void tableLayoutPanelButtons_Paint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(SystemColors.ControlLight);
            e.Graphics.DrawLine(pen, 0, 0, tableLayoutPanelButtons.Width, 0);
        }

        private void listViewGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var totalSize = SelectedGames
                .Select(game => game.Size)
                .Sum();

            if (listViewGames.SelectedItems.Count == 0)
            {
                labelStatus.Text = NoSelection;
            } 
            else
            {
                labelStatus.Text = string.Format(listViewGames.SelectedItems.Count == 1 ? SingleSelection : MultiSelection, listViewGames.SelectedItems.Count, Shared.SizeSuffix(totalSize));
            }

            buttonImport.Enabled = listViewGames.SelectedItems.Count > 0;
        }

        public static TaskFunc GameCopyTask(FoundGame game)
        {
            return (Tasker tasker, Object sync) =>
            {
                if (game.Desktop.Code.StartsWith("CLV-"))
                {
                    long dataTransferred = 0;
                    var destinationPath = Path.Combine(NesApplication.GamesDirectory, game.Desktop.Code);
                    tasker?.SetStatus($"{game.Desktop.Name}");

                    if (Directory.Exists(destinationPath))
                    {
                        Directory.Delete(destinationPath, true);
                    }

                    Directory.CreateDirectory(destinationPath);

                    foreach (var folder in hakchi.Shell.ExecuteSimple($"cd {Shared.EscapeShellArgument(game.RemotePath)}; find -type d").Split('\n'))
                    {
                        Directory.CreateDirectory(Path.Combine(destinationPath, folder));
                    }

                    FtpClient ftp = null;

                    if (hakchi.Shell is INetworkShell)
                    {
                        ftp = new FtpClient(new Uri($"ftp://{(hakchi.Shell as INetworkShell).IPAddress}"), new NetworkCredential("root", "root"));
                    }

                    foreach (Match match in new Regex(@"^(\d+)\s*\./(.*)$", RegexOptions.Multiline).Matches(hakchi.Shell.ExecuteSimple($"cd {Shared.EscapeShellArgument(game.RemotePath)}; find -type f -exec du {"{}"} \\;")))
                    {
                        var size = long.Parse(match.Groups[1].Value) * 1024;
                        var filename = match.Groups[2].Value;

                        using (var file = File.Create(Path.Combine(destinationPath, filename)))
                        using (var tracker = new TrackableStream(file))
                        {
                            tracker.OnProgress += (long transferred, long length) =>
                            {
                                var totalTransferred = Math.Min(dataTransferred + transferred, game.Size);
                                tasker?.SetProgress(totalTransferred, game.Size);
                                tasker?.SetStatus($"{game.Desktop.Name} ({Shared.SizeSuffix(totalTransferred, 2)} / {Shared.SizeSuffix(game.Size, 2)})");
                            };

                            if (hakchi.Shell is INetworkShell)
                            {
                                using (var ftpStream = ftp.Retr($"{game.RemotePath}/{filename}"))
                                {
                                    ftpStream.CopyTo(tracker);
                                }
                            }
                            else
                            {
                                hakchi.Shell.Execute($"cat {Shared.EscapeShellArgument($"{game.RemotePath}/{filename}")}", null, tracker, throwOnNonZero: true);
                            }

                            dataTransferred += size;
                        }
                    }

                    ftp?.Dispose();
                    ftp = null;

                    game.Desktop.Save(Path.Combine(destinationPath, $"{game.Desktop.Code}.desktop"));

                    return Conclusion.Success;
                }

                return Conclusion.Error;
            };
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count > 0)
            {
                gameCopied = true;
                using (var tasker = new Tasks.Tasker(this))
                {
                    tasker.AttachView(new TaskerTaskbar());
                    tasker.AttachView(new TaskerForm());
                    tasker.SetTitle(Resources.CopyingGames);
                    if (hakchi.Shell.IsOnline)
                    {
                        foreach (var game in SelectedGames)
                        {
                            tasker.AddTask(GameCopyTask(game));
                        }
                    }

                    tasker.Start();
                }
            }
        }
    }
}
