using com.clusterrr.clovershell;
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        public enum OriginalGamesPosition { AtTop = 0, AtBottom = 1, Sorted = 2 }
        public enum ConsoleType { NES = 0, Famicom = 1, SNES = 2, SuperFamicom = 3, Unknown = 255 }
        public long DefaultMaxGamesSize
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case ConsoleType.NES:
                    case ConsoleType.Famicom:
                        return 300;
                    case ConsoleType.SNES:
                    case ConsoleType.SuperFamicom:
                        return 200;
                }
            }
        }
        public static ConsoleType? DetectedConnectedConsole = null;
        public static string GetConsoleTypeName()
        {
            return GetConsoleTypeName(DetectedConnectedConsole);
        }
        public static string GetConsoleTypeName(ConsoleType? c)
        {
            switch (c)
            {
                case ConsoleType.NES: return "NES";
                case ConsoleType.Famicom: return "Famicom";
                case ConsoleType.SNES: return "SNES";
                case ConsoleType.SuperFamicom: return "Super Famicom";
            }
            return string.Empty;
        }

        public static IEnumerable<string> InternalMods;
        public static bool? DownloadCover;
        public const int MaxGamesPerFolder = 50;

        public static ClovershellConnection Clovershell;
        mooftpserv.Server ftpServer;

        public MainForm()
        {
            InitializeComponent();
            FormInitialize();
            Clovershell = new ClovershellConnection() { AutoReconnect = true, Enabled = true };
            Clovershell.OnConnected += Clovershell_OnConnected;
            Clovershell.OnDisconnected += Clovershell_OnDisconnected;

            ftpServer = new mooftpserv.Server();
            ftpServer.AuthHandler = new mooftpserv.NesMiniAuthHandler();
            ftpServer.FileSystemHandler = new mooftpserv.NesMiniFileSystemHandler(Clovershell);
            ftpServer.LogHandler = new mooftpserv.DebugLogHandler();
            ftpServer.LocalPort = 1021;

            if (ConfigIni.FtpServer)
                FTPToolStripMenuItem_Click(null, null);
            if (ConfigIni.TelnetServer)
                Clovershell.ShellEnabled = shellToolStripMenuItem.Checked = true;
        }

        void FormInitialize()
        {
            try
            {
                SyncConsoleType();
                InternalMods = from m in Directory.GetFiles(Path.Combine(Program.BaseDirectoryInternal, "mods/hmods")) select Path.GetFileNameWithoutExtension(m);
                LoadPresets();
                LoadLanguages();
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Text = string.Format("hakchi2 v2.21f (princess_daphie build: {0})", DateTime.Now.ToString("yyyyMMdd.HH"))
#if DEBUG
 + " (debug"
#if VERY_DEBUG
 + ", very verbose mode"
#endif
 + ")"
#endif
;

                //listViewGames.ListViewItemSorter = new GamesSorter();
                listViewGames.DoubleBuffered(true);

                // initial view menu
                positionAtTheTopToolStripMenuItem.Checked = ConfigIni.OriginalGamesPosition == OriginalGamesPosition.AtTop;
                positionAtTheBottomToolStripMenuItem.Checked = ConfigIni.OriginalGamesPosition == OriginalGamesPosition.AtBottom;
                positionSortedToolStripMenuItem.Checked = ConfigIni.OriginalGamesPosition == OriginalGamesPosition.Sorted;
                groupByAppTypeToolStripMenuItem.Checked = ConfigIni.GroupGamesByAppType;
                originalGamesToolStripMenuItem.Enabled = !ConfigIni.GroupGamesByAppType;

                // initial context menu state
                explorerToolStripMenuItem.Enabled =
                    scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesBoxArtToolStripMenuItem.Enabled =
                    compressSelectedGamesToolStripMenuItem.Enabled =
                    decompressSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesToolStripMenuItem.Enabled = false;

                // Little tweak for easy translation
                var tbl = textBoxName.Left;
                textBoxName.Left = labelName.Left + labelName.Width;
                textBoxName.Width -= textBoxName.Left - tbl;
                maskedTextBoxReleaseDate.Left = label1.Left + label1.Width + 3;
                tbl = textBoxPublisher.Left;
                textBoxPublisher.Left = label2.Left + label2.Width;
                textBoxPublisher.Width -= textBoxPublisher.Left - tbl;

                // Tweeks for message boxes
                MessageBoxManager.Yes = MessageBoxManager.Retry = Resources.Yes;
                MessageBoxManager.No = MessageBoxManager.Ignore = Resources.No;
                MessageBoxManager.Cancel = Resources.NoForAll;
                MessageBoxManager.Abort = Resources.YesForAll;

                var extensions = new List<string>() { "*.new", "*.unf", "*.unif", ".*fds", "*.desktop", "*.zip", "*.7z", "*.rar" };
                foreach (var app in AppTypeCollection.ApplicationTypes)
                    foreach (var ext in app.Extensions)
                        if (!extensions.Contains("*" + ext))
                            extensions.Add("*" + ext);
                openFileDialogNes.Filter = Resources.GamesAndApps + "|" + string.Join(";", extensions.ToArray()) + "|" + Resources.AllFiles + "|*.*";

                // Loading games database in background
                new Thread(NesGame.LoadCache).Start();
                new Thread(SnesGame.LoadCache).Start();
                // Recalculate games in background
                new Thread(RecalculateSelectedGamesThread).Start();

                openFTPInExplorerToolStripMenuItem.Enabled = FTPToolStripMenuItem.Checked = ConfigIni.FtpServer;
                openTelnetToolStripMenuItem.Enabled = shellToolStripMenuItem.Checked = ConfigIni.TelnetServer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Clovershell_OnConnected()
        {
            try
            {
                // Trying to autodetect console type
                var customFirmware = Clovershell.ExecuteSimple("[ -d /var/lib/hakchi/firmware/ ] && [ -f /var/lib/hakchi/firmware/*.hsqs ] && echo YES || echo NO");
                if (customFirmware == "NO")
                {
                    var board = Clovershell.ExecuteSimple("cat /etc/clover/boardtype", 500, true);
                    var region = Clovershell.ExecuteSimple("cat /etc/clover/REGION", 500, true);
                    Debug.WriteLine(string.Format("Detected board: {0}", board));
                    Debug.WriteLine(string.Format("Detected region: {0}", region));

                    var c = ConfigIni.ConsoleType;
                    switch (board)
                    {
                        default:
                        case "dp-nes":
                        case "dp-hvc":
                            switch (region)
                            {
                                case "EUR_USA":
                                    c = ConsoleType.NES;
                                    break;
                                case "JPN":
                                    c = ConsoleType.Famicom;
                                    break;
                            }
                            break;
                        case "dp-shvc":
                            switch (region)
                            {
                                case "USA":
                                case "EUR":
                                    c = ConsoleType.SNES;
                                    break;
                                case "JPN":
                                    c = ConsoleType.SuperFamicom;
                                    break;
                            }
                            break;
                    }
                    ConfigIni.ConsoleType = c;
                    DetectedConnectedConsole = c;

                    Invoke(new Action(SyncConsoleType));
                    Invoke(new Action(UpdateLocalCache));
                }

                ConfigIni.CustomFlashed = true; // Just in case of new installation
                WorkerForm.GetMemoryStats();
                new Thread(RecalculateSelectedGamesThread).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        void Clovershell_OnDisconnected()
        {
            DetectedConnectedConsole = null;
            Invoke(new Action(SyncConsoleType));
        }

        static ConsoleType lastConsoleType = ConsoleType.Unknown;
        public void SyncConsoleType()
        {
            nESMiniToolStripMenuItem.Enabled =
                famicomMiniToolStripMenuItem.Enabled =
                sNESMiniToolStripMenuItem.Enabled =
                superFamicomMiniToolStripMenuItem.Enabled = (DetectedConnectedConsole == null);

            if (ConfigIni.ConsoleType == lastConsoleType || ConfigIni.ConsoleType == ConsoleType.Unknown)
                return;

            // Console type and some settings
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.NES;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.Famicom;
            sNESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.SNES;
            superFamicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.SuperFamicom;
            epilepsyProtectionToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.NES || ConfigIni.ConsoleType == ConsoleType.Famicom;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.NES || ConfigIni.ConsoleType == ConsoleType.Famicom;
            upABStartOnSecondControllerToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.Famicom;

            // More settings
            useExtendedFontToolStripMenuItem.Checked = ConfigIni.UseFont;
            epilepsyProtectionToolStripMenuItem.Checked = ConfigIni.AntiArmetLevel > 0 && epilepsyProtectionToolStripMenuItem.Enabled;
            selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.ResetHack;
            enableAutofireToolStripMenuItem.Checked = ConfigIni.AutofireHack;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked = ConfigIni.AutofireXYHack && useXYOnClassicControllerAsAutofireABToolStripMenuItem.Enabled;
            upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.FcStart && upABStartOnSecondControllerToolStripMenuItem.Enabled;
            compressGamesToolStripMenuItem.Checked = ConfigIni.Compress;
            compressBoxArtToolStripMenuItem.Checked = ConfigIni.CompressCover;
            disableHakchi2PopupsToolStripMenuItem.Checked = ConfigIni.DisablePopups;

            // Folders mods
            disablePagefoldersToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 0;
            automaticToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 2;
            automaticOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 3;
            pagesToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 4;
            pagesOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 5;
            foldersToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 6;
            foldersOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 7;
            foldersSplitByFirstLetterToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 8;
            foldersSplitByFirstLetterOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 9;
            customToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 99;

            // Items per folder
            maximumGamesPerFolderToolStripMenuItem.DropDownItems.Clear();
            for (byte f = 20; f <= 100; f += ((f < 50) ? (byte)5 : (byte)10))
            {
                var item = new ToolStripMenuItem();
                item.Name = "folders" + f.ToString();
                item.Text = f.ToString();
                item.Tag = f;
                if (f >= MaxGamesPerFolder)
                    item.Text += $" ({Resources.NotRecommended})";
                item.Checked = ConfigIni.MaxGamesPerFolder == f;
                item.Click += delegate (object sender, EventArgs e)
                {
                    var old = maximumGamesPerFolderToolStripMenuItem.DropDownItems.Find("folders" + ConfigIni.MaxGamesPerFolder.ToString(), true);
                    if (old.Count() > 0)
                        (old.First() as ToolStripMenuItem).Checked = false;
                    ConfigIni.MaxGamesPerFolder = (byte)((sender as ToolStripMenuItem).Tag);
                    var n = maximumGamesPerFolderToolStripMenuItem.DropDownItems.Find("folders" + ConfigIni.MaxGamesPerFolder.ToString(), true);
                    if (n.Count() > 0)
                        (n.First() as ToolStripMenuItem).Checked = true;
                };
                maximumGamesPerFolderToolStripMenuItem.DropDownItems.Add(item);
            }

            LoadGames();
            lastConsoleType = ConfigIni.ConsoleType;
        }

        void UpdateLocalCache()
        {
            string cachePath = Path.Combine(Program.BaseDirectoryExternal, "image_cache");
            var games = new NesMenuCollection();
            foreach (NesDefaultGame game in NesMiniApplication.DefaultGames)
            {
                if (!File.Exists(Path.Combine(cachePath, game.Code + ".png")) || !File.Exists(Path.Combine(cachePath, game.Code + "_small.png")))
                    games.Add(game);
            }

            if (games.Count > 0)
            {
                var workerForm = new WorkerForm(this);
                workerForm.Text = Resources.UpdatingLocalCache;
                workerForm.Task = WorkerForm.Tasks.UpdateLocalCache;
                workerForm.Games = games;

                if (workerForm.Start() == DialogResult.OK)
                    Debug.WriteLine("successfully updated local original games cache.");
            }
            else
                Debug.WriteLine("local original games cache in sync.");
        }

        ListViewGroup[] lgvGroups = null;
        Dictionary<Type, ListViewGroup> lgvAppGroups = null;
        public void LoadGames()
        {
            Debug.WriteLine("Loading games");
            var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.WriteLine($"{selected.Length} selected.");

            // add original games
            var originalGames = new List<NesMiniApplication>();
            foreach(var defaultGame in NesMiniApplication.DefaultGames)
            {
                string gameDir = Path.Combine(NesMiniApplication.OriginalGamesDirectory, defaultGame.Code);
                if (Directory.Exists(gameDir))
                {
                    try
                    {
                        // Removing empty directories without errors
                        try
                        {
                            var game = NesMiniApplication.FromDirectory(gameDir);
                            originalGames.Add(game);
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
            }

            // add custom games
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

            // create groups
            if (lgvGroups == null)
            {
                lgvGroups = new ListViewGroup[4];
                lgvGroups[0] = new ListViewGroup("New Games", HorizontalAlignment.Center);
                lgvGroups[1] = new ListViewGroup("Original Games", HorizontalAlignment.Center);
                lgvGroups[2] = new ListViewGroup("Custom Games", HorizontalAlignment.Center);
                lgvGroups[3] = new ListViewGroup("All Games", HorizontalAlignment.Center);
            }

            if (lgvAppGroups == null)
            {
                var sortedApps = new SortedDictionary<string, Type>();
                foreach (var appinfo in AppTypeCollection.ApplicationTypes)
                    sortedApps[appinfo.Name] = appinfo.Class;

                lgvAppGroups = new Dictionary<Type, ListViewGroup>();
                foreach (var pair in sortedApps)
                    lgvAppGroups[pair.Value] = new ListViewGroup(pair.Key, HorizontalAlignment.Center);
            }

            listViewGames.BeginUpdate();
            listViewGames.Groups.Clear();
            listViewGames.Items.Clear();

            // apply games sorting
            var gamesSorted = new List<NesMiniApplication>();
            if (ConfigIni.GroupGamesByAppType)
            {
                games.AddRange(originalGames);
                gamesSorted.AddRange(games.OrderBy(o => o.SortName));
                foreach (var group in lgvAppGroups)
                    listViewGames.Groups.Add(group.Value);
            }
            else if (ConfigIni.OriginalGamesPosition == OriginalGamesPosition.AtTop)
            {
                gamesSorted.AddRange(originalGames.OrderBy(o => o.SortName));
                gamesSorted.AddRange(games.OrderBy(o => o.SortName));
                listViewGames.Groups.Add(lgvGroups[0]);
                listViewGames.Groups.Add(lgvGroups[1]);
                listViewGames.Groups.Add(lgvGroups[2]);
            }
            else if (ConfigIni.OriginalGamesPosition == OriginalGamesPosition.AtBottom)
            {
                gamesSorted.AddRange(games.OrderBy(o => o.SortName));
                gamesSorted.AddRange(originalGames.OrderBy(o => o.SortName));
                listViewGames.Groups.Add(lgvGroups[0]);
                listViewGames.Groups.Add(lgvGroups[2]);
                listViewGames.Groups.Add(lgvGroups[1]);
            }
            else
            {
                games.AddRange(originalGames);
                gamesSorted.AddRange(games.OrderBy(o => o.SortName));
                //listViewGames.Groups.Add(lgvGroups[0]);
                //listViewGames.Groups.Add(lgvGroups[3]);
            }

            // add games to ListView control
            foreach (var game in gamesSorted)
            {
                var listViewItem = new ListViewItem(game.Name);
                if (ConfigIni.GroupGamesByAppType)
                {
                    listViewItem.Group = lgvAppGroups[AppTypeCollection.GetAppByExec(game.Command).Class];
                }
                else
                {
                    if (ConfigIni.OriginalGamesPosition == OriginalGamesPosition.Sorted)
                        listViewItem.Group = lgvGroups[3];
                    else
                        listViewItem.Group = game.IsOriginalGame ? lgvGroups[1] : lgvGroups[2];
                }
                listViewItem.Tag = game;
                listViewItem.Checked = selected.Contains(game.Code);

                listViewGames.Items.Add(listViewItem);
            }
            listViewGames.EndUpdate();

            RecalculateSelectedGames();
            ShowSelected();
        }

        public void ShowSelected()
        {
            object selected = null;
            var selectedAll = listViewGames.SelectedItems;
            if (selectedAll.Count == 1) selected = selectedAll[0].Tag;
            if (selected == null)
            {
                groupBoxOptions.Visible = true;
                groupBoxOptions.Enabled = false;
                labelID.Text = "ID: ";
                labelSize.Text = Resources.Size;
                textBoxName.Text = "";
                radioButtonOne.Checked = true;
                radioButtonTwo.Checked = false;
                radioButtonTwoSim.Checked = false;
                maskedTextBoxReleaseDate.Text = "";
                textBoxPublisher.Text = "";
                textBoxArguments.Text = "";
                pictureBoxArt.Image = null;
                pictureBoxThumbnail.Image = null;
                buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = false;
                textBoxGameGenie.Text = "";
                checkBoxCompressed.Enabled = false;
                checkBoxCompressed.Checked = false;
            }
            else
            {
                var app = selected as NesMiniApplication;
                groupBoxOptions.Visible = true;
                labelID.Text = "ID: " + app.Code;
                labelSize.Text = Resources.Size + " " + (app.Size() / 1024) + "KB";
                textBoxName.Text = app.Name;
                if (app.Simultaneous && app.Players == 2)
                    radioButtonTwoSim.Checked = true;
                else if (app.Players == 2)
                    radioButtonTwo.Checked = true;
                else
                    radioButtonOne.Checked = true;
                maskedTextBoxReleaseDate.Text = app.ReleaseDate;
                textBoxPublisher.Text = app.Publisher;
                textBoxArguments.Text = app.Command;
                pictureBoxArt.Image = app.Image;
                pictureBoxThumbnail.Image = app.Thumbnail;
                buttonShowGameGenieDatabase.Enabled = app is NesGame; //ISupportsGameGenie;
                textBoxGameGenie.Enabled = app is ISupportsGameGenie;
                textBoxGameGenie.Text = (app is ISupportsGameGenie) ? (app as NesMiniApplication).GameGenie : "";
                groupBoxOptions.Enabled = true;
                if (app.CompressPossible().Count() > 0)
                {
                    checkBoxCompressed.Enabled = true;
                    checkBoxCompressed.Checked = false;
                }
                else if (app.DecompressPossible().Count() > 0)
                {
                    checkBoxCompressed.Enabled = true;
                    checkBoxCompressed.Checked = true;
                }
                else
                {
                    checkBoxCompressed.Enabled = false;
                    checkBoxCompressed.Checked = false;
                }
            }
        }

        void LoadPresets()
        {
            while (presetsToolStripMenuItem.DropDownItems.Count > 3)
                presetsToolStripMenuItem.DropDownItems.RemoveAt(0);
            deletePresetToolStripMenuItem.Enabled = false;
            deletePresetToolStripMenuItem.DropDownItems.Clear();
            int i = 0;
            foreach (var preset in ConfigIni.Presets.Keys.OrderBy(o => o))
            {
                presetsToolStripMenuItem.DropDownItems.Insert(i, new ToolStripMenuItem(preset, null,
                    delegate (object sender, EventArgs e)
                    {
                        var cols = ConfigIni.Presets[preset].Split('|');
                        ConfigIni.SelectedGames = cols[0];
                        //ConfigIni.HiddenGames = cols[1];
                        var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //var hide = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 1; j < listViewGames.Items.Count; j++)
                            listViewGames.Items[j].Checked = selected.Contains((listViewGames.Items[j].Tag as NesMiniApplication).Code);
                    }));
                deletePresetToolStripMenuItem.DropDownItems.Insert(i, new ToolStripMenuItem(preset, null,
                    delegate (object sender, EventArgs e)
                    {
                        if (MessageBox.Show(this, string.Format(Resources.DeletePreset, preset), Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            == DialogResult.Yes)
                        {
                            ConfigIni.Presets.Remove(preset);
                            LoadPresets();
                        }
                    }));
                deletePresetToolStripMenuItem.Enabled = true;
                i++;
            }
        }

        void LoadLanguages()
        {
            var languages = new List<string>(Directory.GetDirectories(Path.Combine(Program.BaseDirectoryInternal, "languages")));
            ResourceManager rm = Resources.ResourceManager;
            languages.Add("en-US"); // default language
            var langCodes = new Dictionary<string, string>();
            foreach (var language in languages)
            {
                var code = Path.GetFileName(language);
                langCodes[new CultureInfo(code).DisplayName] = code;
            }
            ToolStripMenuItem english = null;
            bool found = false;
            foreach (var language in langCodes.Keys.OrderBy<string, string>(o => o))
            {
                var item = new ToolStripMenuItem();
                var displayName = Regex.Replace(language, @" \(.+\)", "");
                if (langCodes.Keys.Count(o => Regex.Replace(o, @" \(.+\)", "") == displayName) == 1)
                    item.Text = displayName;
                else
                    item.Text = language;
                var country = langCodes[language];
                if (langCodes[language] == "zh-CHS" || langCodes[language] == "zh-CHT") // chinese is awkward
                    country = "cn";
                else
                    if (country.Length > 2) country = country.Substring(country.Length - 2).ToLower();
                // Trying to load flag
                item.Image = (Image)rm.GetObject(country);
                if (item.Image == null)
                    Debug.WriteLine($"There is no flag for \"{country}\"");
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Click += delegate (object sender, EventArgs e)
                    {
                        ConfigIni.Language = langCodes[language];
                        SaveConfig();
                        lastConsoleType = ConsoleType.Unknown;
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCodes[language]);
                        this.Controls.Clear();
                        this.InitializeComponent();
                        FormInitialize();
                        this.Invalidate(true);
                    };
                if (Thread.CurrentThread.CurrentUICulture.Name.ToUpper() == langCodes[language].ToUpper())
                {
                    item.Checked = true;
                    if (string.IsNullOrEmpty(ConfigIni.Language))
                        ConfigIni.Language = langCodes[language];
                }
                found |= item.Checked;
                if (langCodes[language] == "en-US")
                    english = item;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }
            if (!found)
                english.Checked = true;
        }

        private void SaveSelectedGames()
        {
            var selected = new List<string>();
            foreach (ListViewItem game in listViewGames.CheckedItems)
            {
                if (game.Tag is NesMiniApplication)
                    selected.Add((game.Tag as NesMiniApplication).Code);
            }
            ConfigIni.SelectedGames = string.Join(";", selected.ToArray());
        }

        private void SaveConfig()
        {
            SaveSelectedGames();
            ConfigIni.Save();
            foreach (ListViewItem game in listViewGames.Items)
            {
                try
                {
                    if (game.Tag is NesMiniApplication)
                    {
                        // Maybe type was changed? Need to reload games
                        if ((game.Tag as NesMiniApplication).Save())
                            game.Tag = NesMiniApplication.FromDirectory((game.Tag as NesMiniApplication).GamePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void AddPreset(object sender, EventArgs e)
        {
            var form = new StringInputForm();
            form.Text = Resources.NewPreset;
            form.labelComments.Text = Resources.InputPreset;
            if (form.ShowDialog() == DialogResult.OK)
            {
                var name = form.textBox.Text.Replace("=", " ");
                if (!string.IsNullOrEmpty(name))
                {
                    SaveSelectedGames();
                    ConfigIni.Presets[name] = ConfigIni.SelectedGames; // + "|" + ConfigIni.HiddenGames;
                    LoadPresets();
                }
            }
        }

        private void listViewGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            explorerToolStripMenuItem.Enabled = (listViewGames.SelectedItems.Count == 1);
            downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                deleteSelectedGamesBoxArtToolStripMenuItem.Enabled =
                compressSelectedGamesToolStripMenuItem.Enabled =
                decompressSelectedGamesToolStripMenuItem.Enabled =
                deleteSelectedGamesToolStripMenuItem.Enabled =
                (listViewGames.SelectedItems.Count >= 1);

            if(!e.IsSelected)
                (e.Item.Tag as NesMiniApplication).Save();

            timerShowSelected.Enabled = true;
        }

        private void timerShowSelected_Tick(object sender, EventArgs e)
        {
            timerShowSelected.Enabled = false;
            ShowSelected();
        }

        private void listViewGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Schedule recalculation
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        private void timerCalculateGames_Tick(object sender, EventArgs e)
        {
            new Thread(RecalculateSelectedGamesThread).Start(); // Calculate it in background
            timerCalculateGames.Enabled = false; // We don't need to count games repetedly
        }

        private void listViewGames_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        listViewGames.BeginUpdate();
                        foreach (ListViewItem item in listViewGames.Items)
                            item.Selected = true;
                        listViewGames.EndUpdate();
                    }
                    break;
                case Keys.Delete:
                    if ((listViewGames.SelectedItems.Count > 1) || (listViewGames.SelectedItems.Count == 1 && listViewGames.SelectedItems[0].Tag is NesMiniApplication))
                        DeleteSelectedGames();
                    break;
                case Keys.Space:
                    if (listViewGames.FocusedItem == null)
                    {
                        bool all = true;
                        foreach (ListViewItem item in listViewGames.SelectedItems)
                            if (!item.Checked)
                            {
                                all = false;
                                break;
                            }
                        foreach (ListViewItem item in listViewGames.SelectedItems)
                        {
                            item.Checked = !all;
                        }

                    }
                    break;
            }
        }

        private void listViewGames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip.Show(sender as Control, e.X + 5, e.Y);
        }

        private NesMiniApplication GetSelectedGame()
        {
            if (listViewGames.SelectedItems.Count != 1) return null;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return null;
            return selected as NesMiniApplication;
        }

        private void SetImageForSelectedGame(string imagePath)
        {
            var app = GetSelectedGame();
            if (app != null)
            {
                app.SetImageFile(imagePath, ConfigIni.CompressCover);
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void buttonDefaultCover_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app != null) { 
                app.Image = null;
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app == null) return;

            openFileDialogImage.Filter = Resources.Images + " (*.bmp;*.png;*.jpg;*.jpeg;*.gif)|*.bmp;*.png;*.jpg;*.jpeg;*.gif|" + Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                app.SetImageFile(openFileDialogImage.FileName, ConfigIni.CompressCover);
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void pictureBoxThumbnail_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app == null) return;

            openFileDialogImage.Filter = Resources.Images + " (*.bmp;*.png;*.jpg;*.jpeg;*.gif)|*.bmp;*.png;*.jpg;*.jpeg;*.gif|" + Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                app.SetThumbnailFile(openFileDialogImage.FileName, ConfigIni.CompressCover);
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app == null) return;

            var googler = new ImageGooglerForm(app);
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                app.Image = googler.Result;
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selectedItem = listViewGames.SelectedItems[0];
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            selectedItem.Text = game.Name = textBoxName.Text;
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Command = textBoxArguments.Text;
        }

        private void maskedTextBoxReleaseDate_TextChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.ReleaseDate = maskedTextBoxReleaseDate.Text;
        }

        private void textBoxGameGenie_TextChanged(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.GameGenie = textBoxGameGenie.Text;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Closing main form");
            SaveConfig();
            ftpServer.Stop();
            Clovershell.Dispose();
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill(); // Suicide! Just easy and dirty way to kill all threads.
        }

        struct CountResult
        {
            public int Count;
            public long Size;
        }
        void RecalculateSelectedGamesThread()
        {
            try
            {
                var stats = RecalculateSelectedGames();
                showStats(stats);
            }
            catch
            {
                timerCalculateGames.Enabled = false;
                timerCalculateGames.Enabled = true;
            }
        }
        CountResult RecalculateSelectedGames()
        {
            CountResult stats;
            stats.Count = 0;
            stats.Size = 0;
            if (!this.IsHandleCreated)
                return new CountResult(); ;
            var checkedGames = (IEnumerable<object>)Invoke(new Func<IEnumerable<object>>(delegate
            {
                var r = new List<object>();
                foreach (ListViewItem o in listViewGames.CheckedItems)
                    r.Add(o.Tag);
                return r;
            }));
            foreach (var game in checkedGames)
            {
                if (game is NesMiniApplication)
                {
                    stats.Count++;
                    stats.Size += (game as NesMiniApplication).Size();
                }
            }
            return stats;
        }
        void showStats(CountResult stats)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<CountResult>(showStats), new object[] { stats });
                    return;
                }
                var maxGamesSize = DefaultMaxGamesSize * 1024 * 1024;
                if (WorkerForm.NandCTotal > 0)
                {
                    maxGamesSize = (WorkerForm.NandCFree + WorkerForm.WrittenGamesSize) - WorkerForm.ReservedMemory * 1024 * 1024;
                    toolStripStatusLabelSize.Text = string.Format("{0:F1}MB / {1:F1}MB", stats.Size / 1024.0 / 1024.0, maxGamesSize / 1024.0 / 1024.0);
                }
                else
                {
                    toolStripStatusLabelSize.Text = string.Format("{0:F1}MB / ???MB", stats.Size / 1024.0 / 1024.0);
                }
                toolStripStatusLabelSelected.Text = stats.Count + " " + Resources.GamesSelected;
                toolStripProgressBar.Maximum = (int)maxGamesSize;
                toolStripProgressBar.Value = Math.Min((int)stats.Size, toolStripProgressBar.Maximum);
                toolStripStatusLabelSize.ForeColor =
                    (toolStripProgressBar.Value < toolStripProgressBar.Maximum) ?
                    SystemColors.ControlText :
                    Color.Red;
            }
            catch { }
        }

        private void buttonAddGames_Click(object sender, EventArgs e)
        {
            if (openFileDialogNes.ShowDialog() == DialogResult.OK)
            {
                AddGames(openFileDialogNes.FileNames);
            }
        }

        private void reloadGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfig();
            listViewGames.BeginUpdate();
            foreach (ListViewItem item in listViewGames.Items)
                item.Selected = false;
            listViewGames.EndUpdate();
            LoadGames();

        }

        DialogResult RequireKernelDump()
        {
            if (File.Exists(WorkerForm.KernelDumpPath)) return DialogResult.OK; // OK - already dumped
                                                                                // Asking user to dump kernel
            if (MessageBox.Show(Resources.NoKernelWarning, Resources.NoKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (DoKernelDump())
                    return DialogResult.Yes; // Succesfully dumped
                else
                    return DialogResult.No; // Not dumped for some other reason
            }
            else return DialogResult.No; // Kernel dump cancelled by user
        }

        DialogResult RequirePatchedKernel()
        {
            if (ConfigIni.CustomFlashed) return DialogResult.OK; // OK - already flashed
            if (MessageBox.Show(Resources.CustomWarning, Resources.CustomKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
            {
                if (FlashCustomKernel())
                    return DialogResult.Yes; // Succesfully flashed
                else
                    return DialogResult.No; // Not flashed for some other reason
            }
            else return DialogResult.No;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SaveConfig();

            var stats = RecalculateSelectedGames();
            if (stats.Count == 0)
            {
                MessageBox.Show(Resources.SelectAtLeast, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var kernel = RequirePatchedKernel();
            if (kernel == DialogResult.No) return;
            if (kernel == DialogResult.Yes) // Message for new user
                MessageBox.Show(Resources.DoneYouCanUpload + "\r\n" + Resources.PressOkToContinue, Resources.Congratulations, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (UploadGames())
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveConfig();
            var stats = RecalculateSelectedGames();
            if (stats.Count == 0)
            {
                MessageBox.Show(Resources.SelectAtLeast, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (UploadGames(true))
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool DoKernelDump()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.DumpingKernel;
            workerForm.Task = WorkerForm.Tasks.DumpKernel;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DoNandDump()
        {
            saveDumpFileDialog.FileName = "nand.bin";
            saveDumpFileDialog.DefaultExt = "bin";
            if (saveDumpFileDialog.ShowDialog() != DialogResult.OK)
                return false;
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.DumpingNand;
            workerForm.Task = WorkerForm.Tasks.DumpNand;
            workerForm.NandDump = saveDumpFileDialog.FileName;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DoNandFlash()
        {
            openDumpFileDialog.FileName = "nand.bin";
            openDumpFileDialog.DefaultExt = "bin";
            if (openDumpFileDialog.ShowDialog() != DialogResult.OK)
                return false;
            var workerForm = new WorkerForm(this);
            workerForm.Text = "Bricking your console";
            workerForm.Task = WorkerForm.Tasks.FlashNand;
            workerForm.NandDump = openDumpFileDialog.FileName;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DoNandBDump()
        {
            saveDumpFileDialog.FileName = "nandb.hsqs";
            saveDumpFileDialog.DefaultExt = "hsqs";
            if (saveDumpFileDialog.ShowDialog() != DialogResult.OK)
                return false;
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.DumpingNand;
            workerForm.Task = WorkerForm.Tasks.DumpNandB;
            workerForm.NandDump = saveDumpFileDialog.FileName;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DoNandCDump()
        {
            saveDumpFileDialog.FileName = "nandc.hsqs";
            saveDumpFileDialog.DefaultExt = "hsqs";
            if (saveDumpFileDialog.ShowDialog() != DialogResult.OK)
                return false;
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.DumpingNand;
            workerForm.Task = WorkerForm.Tasks.DumpNandC;
            workerForm.NandDump = saveDumpFileDialog.FileName;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DoNandCFlash()
        {
            openDumpFileDialog.FileName = "nandc.hsqs";
            openDumpFileDialog.DefaultExt = "hsqs";
            if (openDumpFileDialog.ShowDialog() != DialogResult.OK)
                return false;
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.FlashingNand;
            workerForm.Task = WorkerForm.Tasks.FlashNandC;
            workerForm.NandDump = openDumpFileDialog.FileName;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool FlashCustomKernel()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.FlasingCustom;
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.Mod = "mod_hakchi";
            workerForm.hmodsInstall = new List<string>(InternalMods);
            workerForm.Config = null;
            workerForm.Games = null;
            workerForm.Start();
            var result = workerForm.DialogResult == DialogResult.OK;
            if (result)
            {
                ConfigIni.CustomFlashed = true;
                ConfigIni.Save();
            }
            return result;
        }

        bool MembootOriginalKernel()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.Membooting;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.Mod = null;
            workerForm.Config = null;
            workerForm.Games = null;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool MembootCustomKernel()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.Membooting;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.Mod = "mod_hakchi";
            workerForm.Config = null;
            workerForm.Games = null;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool UploadGames(bool exportGames = false)
        {
            if (exportGames && exportFolderDialog.ShowDialog() != DialogResult.OK)
                return false;

            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.UploadingGames;
            workerForm.Task = WorkerForm.Tasks.UploadGames;
            workerForm.Mod = "mod_hakchi";
            workerForm.Config = ConfigIni.GetConfigDictionary();
            workerForm.Games = new NesMenuCollection();
            workerForm.exportGames = exportGames;
            
            if (exportGames)
                workerForm.exportDirectory = exportFolderDialog.SelectedPath;

            foreach (ListViewItem game in listViewGames.CheckedItems)
            {
                if (game.Tag is NesMiniApplication)
                    workerForm.Games.Add(game.Tag as NesMiniApplication);
            }

            workerForm.FoldersMode = ConfigIni.FoldersMode;
            workerForm.MaxGamesPerFolder = ConfigIni.MaxGamesPerFolder;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        void AddGames(IEnumerable<string> files)
        {
            SaveConfig();
            ICollection<NesMiniApplication> addedApps;
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.LoadingGames;
            workerForm.Task = WorkerForm.Tasks.AddGames;
            workerForm.GamesToAdd = files;
            workerForm.Start();
            addedApps = workerForm.addedApplications;

            if (addedApps != null)
            {
                foreach (ListViewItem item in listViewGames.Items)
                    item.Selected = false;
                // Add games, only new ones
                var newApps = addedApps.Distinct(new NesMiniApplication.NesMiniAppEqualityComparer());
                var newCodes = from app in newApps select app.Code;
                var oldAppsReplaced = from app in listViewGames.Items.Cast<ListViewItem>().ToArray()
                                      where (app.Tag is NesMiniApplication) && newCodes.Contains((app.Tag as NesMiniApplication).Code)
                                      select app;
                foreach (var replaced in oldAppsReplaced)
                    listViewGames.Items.Remove(replaced);
                foreach (var newApp in newApps)
                {
                    var item = new ListViewItem(newApp.Name);
                    item.Group = lgvGroups[0];
                    item.Tag = newApp;
                    item.Selected = true;
                    item.Checked = true;
                    listViewGames.Items.Add(item);
                }
            }
            else
            {
                // Reload all games (maybe process was terminated?)
                LoadGames();
            }
            // Schedule recalculation
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        bool FlashOriginalKernel(bool boot = true)
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.FlasingOriginal;
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.Mod = null;
            workerForm.Start();
            var result = workerForm.DialogResult == DialogResult.OK;
            if (result)
            {
                ConfigIni.CustomFlashed = false;
                ConfigIni.Save();
            }
            return result;
        }

        bool Uninstall()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.Uninstalling;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.Mod = "mod_uninstall";
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool InstallMods(string[] mods)
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.InstallingMods;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.Mod = "mod_hakchi";
            workerForm.hmodsInstall = new List<string>(mods);
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool UninstallMods(string[] mods)
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.UninstallingMods;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.Mod = "mod_hakchi";
            workerForm.hmodsUninstall = new List<string>(mods);
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        private void dumpKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(WorkerForm.KernelDumpPath))
            {
                MessageBox.Show(Resources.ReplaceKernelQ, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(Resources.DumpKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (DoKernelDump()) MessageBox.Show(Resources.KernelDumped, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dumpTheWholeNANDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DoNandDump()) MessageBox.Show(Resources.NandDumped, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolFlashTheWholeNANDStripMenuItem_Click(object sender, EventArgs e)
        {
            // Maybe I'll fix it one day...
            if (MessageBox.Show("It will brick your console. Do you want to continue? :)", Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                DoNandFlash();
            }
        }

        private void dumpNANDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            if (DoNandBDump())
                MessageBox.Show(Resources.NandDumped, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dumpNANDCPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            if (DoNandCDump())
                MessageBox.Show(Resources.NandDumped, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void flashNANDCPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.FlashNandCQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                if (RequirePatchedKernel() == DialogResult.No) return;
                if (DoNandCFlash())
                    MessageBox.Show(Resources.NandFlashed, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void flashCustomKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.CustomKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                if (FlashCustomKernel()) MessageBox.Show(Resources.DoneYouCanUpload, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void membootOriginalKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(WorkerForm.KernelDumpPath))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MembootOriginalKernel();
        }


        private void membootPatchedKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequireKernelDump() == DialogResult.No) return;
            MembootCustomKernel();
        }

        private void flashOriginalKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(WorkerForm.KernelDumpPath))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(Resources.OriginalKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (FlashOriginalKernel())
                    MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(WorkerForm.KernelDumpPath))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(Resources.UninstallQ1, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (Uninstall())
                {
                    if (ConfigIni.CustomFlashed && MessageBox.Show(Resources.UninstallQ2, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (FlashOriginalKernel())
                            MessageBox.Show(Resources.UninstallFactoryNote, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.Text = aboutToolStripMenuItem.Text.Replace("&", "");
            about.ShowDialog();
        }

        private void gitHubPageWithActualReleasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ClusterM/hakchi2/releases");
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ClusterM/hakchi2/wiki/FAQ");
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.me/clusterm");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void useExtendedFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.UseFont = useExtendedFontToolStripMenuItem.Checked;
        }

        private void ToolStripMenuItemArmet_Click(object sender, EventArgs e)
        {
            ConfigIni.AntiArmetLevel = epilepsyProtectionToolStripMenuItem.Checked ? (byte)2 : (byte)0;
        }

        private void cloverconHackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectButtonCombinationToolStripMenuItem.Enabled =
                ConfigIni.ResetHack = resetUsingCombinationOfButtonsToolStripMenuItem.Checked;
        }

        private void upABStartOnSecondControllerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.FcStart = upABStartOnSecondControllerToolStripMenuItem.Checked;
        }

        private void selectButtonCombinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ConfigIni.ConsoleType)
            {
                default:
                case ConsoleType.NES:
                case ConsoleType.Famicom:
                    {
                        var form = new SelectNesButtonsForm((SelectNesButtonsForm.NesButtons)ConfigIni.ResetCombination);
                        if (form.ShowDialog() == DialogResult.OK)
                            ConfigIni.ResetCombination = (uint)form.SelectedButtons;
                    }
                    break;
                case ConsoleType.SNES:
                case ConsoleType.SuperFamicom:
                    {
                        var form = new SelectSnesButtonsForm((SelectSnesButtonsForm.SnesButtons)ConfigIni.ResetCombination);
                        if (form.ShowDialog() == DialogResult.OK)
                            ConfigIni.ResetCombination = (uint)form.SelectedButtons;
                    }
                    break;
            }
        }

        private void nESMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nESMiniToolStripMenuItem.Checked) return;
            SaveConfig();
            ConfigIni.ConsoleType = ConsoleType.NES;
            SyncConsoleType();
        }

        private void famicomMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (famicomMiniToolStripMenuItem.Checked) return;
            SaveConfig();
            ConfigIni.ConsoleType = ConsoleType.Famicom;
            SyncConsoleType();
        }

        private void sNESMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sNESMiniToolStripMenuItem.Checked) return;
            SaveConfig();
            ConfigIni.ConsoleType = ConsoleType.SNES;
            SyncConsoleType();
        }

        private void superFamicomMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (superFamicomMiniToolStripMenuItem.Checked) return;
            SaveConfig();
            ConfigIni.ConsoleType = ConsoleType.SuperFamicom;
            SyncConsoleType();
        }

        public void ResetOriginalGames(bool nonDestructiveSync = false)
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.ResettingOriginalGames;
            workerForm.Task = WorkerForm.Tasks.SyncOriginalGames;
            workerForm.nonDestructiveSync = nonDestructiveSync;

            if (workerForm.Start() == DialogResult.OK)
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadGames();
        }

        private void resetOriginalGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.ResetOriginalGamesQ, Resources.Default30games, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                SaveSelectedGames();
                ResetOriginalGames();
            }
        }

        private void enableAutofireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.AutofireHack = enableAutofireToolStripMenuItem.Checked;
            if (ConfigIni.AutofireHack)
                MessageBox.Show(this, Resources.AutofireHelp1, enableAutofireToolStripMenuItem.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void useXYOnClassicControllerAsAutofireABToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.AutofireXYHack = useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked;
        }

        private void globalCommandLineArgumentsexpertsOnluToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new StringInputForm();
            form.Text = Resources.ExtraArgsTitle;
            form.labelComments.Text = Resources.ExtraArgsInfo;
            form.textBox.Text = ConfigIni.ExtraCommandLineArguments;
            if (form.ShowDialog() == DialogResult.OK)
                ConfigIni.ExtraCommandLineArguments = form.textBox.Text;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ConfigIni.RunCount++;
            if (ConfigIni.RunCount == 1)
            {
                ShowSelected();
                if (ConfigIni.ConsoleType == ConsoleType.Unknown && DetectedConnectedConsole == null)
                {
                    new SelectConsoleDialog(this).ShowDialog();
                    SyncConsoleType();
                    MessageBox.Show(this, Resources.FirstRun, Resources.Hello, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            if (ConfigIni.RunCount == 10 && !string.IsNullOrEmpty(Resources.Donate.Trim()))
            {
                MessageBox.Show(this, Resources.Donate, Resources.Hello, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void dragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;

            // Need to determine type of files
            // Maybe it's cover art?
            if (files.Length == 1)
            {
                var ext = Path.GetExtension(files[0]).ToLower();
                if (ext == ".jpg" || ext == ".png")
                {
                    SetImageForSelectedGame(files[0]);
                    return;
                }
            }

            // Maybe it's some mods?
            bool mods = false;
            foreach (var file in files)
                if (Path.GetExtension(file).ToLower() == ".hmod")
                    mods = true;
            // Maybe it's some mods in single archive?
            if (files.Length == 1)
            {
                var ext = Path.GetExtension(files[0]).ToLower();
                if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(files[0]))
                    {
                        foreach (var f in szExtractor.ArchiveFileNames)
                            if (Path.GetExtension(f).ToLower() == ".hmod")
                                mods = true;
                    }
                }
            }
            if (mods)
            {
                installModules(files);
                return;
            }

            // All other cases - games or apps
            var allFilesToAdd = new List<string>();
            foreach (var file in files)
                if (Directory.Exists(file))
                    allFilesToAdd.AddRange(Directory.GetFiles(file, "*.*", SearchOption.AllDirectories));
                else if (File.Exists(file))
                    allFilesToAdd.Add(file);
            if (allFilesToAdd.Count > 0)
                AddGames(allFilesToAdd);
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var searchForm = new SearchForm(this);
            searchForm.Left = this.Left + 200;
            searchForm.Top = this.Top + 300;
            searchForm.Show();
        }

        private void compressGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Compress = compressGamesToolStripMenuItem.Checked;
        }

        private void compressBoxArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.CompressCover = compressBoxArtToolStripMenuItem.Checked;
        }

        private void disableHakchi2PopupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.DisablePopups = disableHakchi2PopupsToolStripMenuItem.Checked;
        }

        private void buttonShowGameGenieDatabase_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (!(selected is ISupportsGameGenie)) return;
            NesMiniApplication nesGame = selected as NesMiniApplication;
            GameGenieCodeForm lFrm = new GameGenieCodeForm(nesGame);
            if (lFrm.ShowDialog() == DialogResult.OK)
                textBoxGameGenie.Text = (nesGame as NesMiniApplication).GameGenie;
        }

        private void pagesModefoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse((sender as ToolStripMenuItem).Tag.ToString());
            disablePagefoldersToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 0;
            automaticToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 2;
            automaticOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 3;
            pagesToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 4;
            pagesOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 5;
            foldersToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 6;
            foldersOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 7;
            foldersSplitByFirstLetterToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 8;
            foldersSplitByFirstLetterOriginalToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 9;
            customToolStripMenuItem.Checked = (byte)ConfigIni.FoldersMode == 99;
        }

        private void installModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            installModules();
        }

        private void installModules(string[] add = null)
        {
            if (RequireKernelDump() == DialogResult.No) return;
            var form = new SelectModsForm(false, true, add);
            form.Text = Resources.SelectModsInstall;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (InstallMods(((from m
                                   in form.checkedListBoxMods.CheckedItems.OfType<object>().ToArray()
                                  select m.ToString())).ToArray()))
                {
                    if (!ConfigIni.DisablePopups)
                        MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void uninstallModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequireKernelDump() == DialogResult.No) return;
            var form = new SelectModsForm(true, false);
            form.Text = Resources.SelectModsUninstall;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (UninstallMods(((from m
                                   in form.checkedListBoxMods.CheckedItems.OfType<object>().ToArray()
                                    select m.ToString())).ToArray()))
                {
                    if (!ConfigIni.DisablePopups)
                        MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void timerConnectionCheck_Tick(object sender, EventArgs e)
        {
            toolStripStatusConnectionIcon.Image = Clovershell.IsOnline ? Resources.green : Resources.red;
            toolStripStatusConnectionIcon.ToolTipText = Clovershell.IsOnline ? "Online" : "Offline";
        }

        private void saveSettingsToNESMiniNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    WorkerForm.SyncConfig(ConfigIni.GetConfigDictionary(), true);
                    if (!ConfigIni.DisablePopups)
                        MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveStateManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            var gameNames = new Dictionary<string, string>();
            foreach (var game in NesMiniApplication.AllDefaultGames)
                gameNames[game.Code] = game.Name;
            foreach (ListViewItem item in listViewGames.Items)
            {
                if (item.Tag is NesMiniApplication)
                    gameNames[(item.Tag as NesMiniApplication).Code] = (item.Tag as NesMiniApplication).Name;
            }
            var form = new SaveStateManager(gameNames);
            form.ShowDialog();
        }

        private void FTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FTPToolStripMenuItem.Checked)
            {
                try
                {
                    var ftpThread = new Thread(delegate ()
                    {
                        try
                        {
                            ftpServer.Run();
                        }
                        catch (ThreadAbortException)
                        {
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                ftpServer.Stop();
                            }
                            catch { }
                            Debug.WriteLine(ex.Message + ex.StackTrace);
                            Invoke(new Action(delegate ()
                                {
                                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    FTPToolStripMenuItem.Checked = false;
                                }));
                        }
                    });
                    ftpThread.Start();
                    ConfigIni.FtpServer = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FTPToolStripMenuItem.Checked = false;
                    ConfigIni.FtpServer = false;
                }
            }
            else
            {
                ftpServer.Stop();
                ConfigIni.FtpServer = false;
            }
            openFTPInExplorerToolStripMenuItem.Enabled = FTPToolStripMenuItem.Checked;
        }

        private void shellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigIni.TelnetServer = openTelnetToolStripMenuItem.Enabled = Clovershell.ShellEnabled = shellToolStripMenuItem.Checked;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigIni.TelnetServer = openTelnetToolStripMenuItem.Enabled = shellToolStripMenuItem.Checked = false;
            }
        }

        private void openFTPInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "ftp://root:clover@127.0.0.1:1021/",
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openTelnetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "telnet://127.0.0.1:1023",
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, Resources.NoTelnet, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void takeScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    var screenshot = WorkerForm.TakeScreenshot();
                    var screenshotPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
                    screenshot.Save(screenshotPath, ImageFormat.Png);
                    var showProcess = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = screenshotPath
                        }
                    };
                    showProcess.Start();
                    new Thread(delegate ()
                    {
                        try
                        {
                            showProcess.WaitForExit();
                        }
                        catch { }
                        try
                        {
                            File.Delete(screenshotPath);
                        }
                        catch { }
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxCompressed_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (listViewGames.SelectedItems.Count != 1) return;
                var selected = listViewGames.SelectedItems[0].Tag;
                checkBoxCompressed.Enabled = false;
                if (checkBoxCompressed.Checked)
                    (selected as NesMiniApplication).Compress();
                else
                    (selected as NesMiniApplication).Decompress();
                (selected as NesMiniApplication).Save();
                timerCalculateGames.Enabled = true;
                ShowSelected();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class GamesSorter : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 is ListViewItem)
                    o1 = (o1 as ListViewItem).Tag;
                if (o2 is ListViewItem)
                    o2 = (o2 as ListViewItem).Tag;
                if (!(o1 is NesMiniApplication))
                    return -1;
                if (!(o2 is NesMiniApplication))
                    return 1;
                return ((o1 as NesMiniApplication).SortName.CompareTo((o2 as NesMiniApplication).SortName));
            }
        }

        bool GroupTaskWithSelected(WorkerForm.Tasks task)
        {
            var workerForm = new WorkerForm(this);
            switch (task)
            {
                case WorkerForm.Tasks.ScanCovers:
                    workerForm.Text = Resources.ScanningCovers;
                    break;
                case WorkerForm.Tasks.DownloadCovers:
                    workerForm.Text = Resources.DownloadAllCoversTitle;
                    break;
                case WorkerForm.Tasks.DeleteCovers:
                    workerForm.Text = Resources.RemovingCovers;
                    break;
                case WorkerForm.Tasks.CompressGames:
                    workerForm.Text = Resources.CompressingGames;
                    break;
                case WorkerForm.Tasks.DecompressGames:
                    workerForm.Text = Resources.DecompressingGames;
                    break;
                case WorkerForm.Tasks.DeleteGames:
                    workerForm.Text = Resources.RemovingGames;
                    break;
            }
            workerForm.Task = task;
            workerForm.Games = new NesMenuCollection();
            foreach (ListViewItem game in listViewGames.SelectedItems)
            {
                if (game.Tag is NesMiniApplication)
                    workerForm.Games.Add(game.Tag as NesMiniApplication);
            }
            return workerForm.Start() == DialogResult.OK;
        }


        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sel = listViewGames.SelectedItems;
            if (sel.Count != 1) return;

            try
            {
                string path = (sel[0].Tag as NesMiniApplication).GamePath;
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = path
                    }
                }.Start();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void scanForNewBoxArtForSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.ScanCovers))
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void downloadBoxArtForSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.DownloadCovers))
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void deleteSelectedGamesBoxArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.DeleteCovers))
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void compressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.CompressGames))
                if (!ConfigIni.DisablePopups)
                    MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void decompressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.DecompressGames))
                if(!ConfigIni.DisablePopups)
                    MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void DeleteSelectedGames()
        {
            if (MessageBox.Show(this, Resources.DeleteSelectedGamesQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveSelectedGames();
                if (GroupTaskWithSelected(WorkerForm.Tasks.DeleteGames))
                {
                    foreach (ListViewItem item in listViewGames.SelectedItems)
                        if (item.Tag is NesMiniApplication)
                            listViewGames.Items.Remove(item);
                    if (!ConfigIni.DisablePopups)
                        MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    LoadGames();

                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void deleteSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedGames();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    explorerToolStripMenuItem_Click(sender, e);
                    break;
                case Keys.E:
                    if (e.Modifiers == (Keys.Alt | Keys.Control))
                    {
                        if (listViewGames.SelectedItems.Count != 1) return;
                        var selected = listViewGames.SelectedItems[0].Tag;
                        if (selected is SnesGame && !(selected as SnesGame).IsOriginalGame)
                        {
                            new SnesPresetEditor(selected as SnesGame).ShowDialog();
                            ShowSelected();
                        }
                    }
                    break;
            }
        }

        private void positionAtTheTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (positionAtTheTopToolStripMenuItem.Checked) return;
            ConfigIni.OriginalGamesPosition = OriginalGamesPosition.AtTop;
            positionAtTheTopToolStripMenuItem.Checked = true;
            positionAtTheBottomToolStripMenuItem.Checked = false;
            positionSortedToolStripMenuItem.Checked = false;
            SaveSelectedGames();
            LoadGames();
        }

        private void positionAtTheBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (positionAtTheBottomToolStripMenuItem.Checked) return;
            ConfigIni.OriginalGamesPosition = OriginalGamesPosition.AtBottom;
            positionAtTheTopToolStripMenuItem.Checked = false;
            positionAtTheBottomToolStripMenuItem.Checked = true;
            positionSortedToolStripMenuItem.Checked = false;
            SaveSelectedGames();
            LoadGames();
        }

        private void positionSortedInListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (positionSortedToolStripMenuItem.Checked) return;
            ConfigIni.OriginalGamesPosition = OriginalGamesPosition.Sorted;
            positionAtTheTopToolStripMenuItem.Checked = false;
            positionAtTheBottomToolStripMenuItem.Checked = false;
            positionSortedToolStripMenuItem.Checked = true;
            SaveSelectedGames();
            LoadGames();
        }

        private void groupByAppTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.GroupGamesByAppType = groupByAppTypeToolStripMenuItem.Checked;
            originalGamesToolStripMenuItem.Enabled = !ConfigIni.GroupGamesByAppType;
            SaveSelectedGames();
            LoadGames();
        }

        private void foldersManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedGames();
            var workerForm = new WorkerForm(this);
            workerForm.Games = new NesMenuCollection();

            foreach (ListViewItem game in listViewGames.CheckedItems)
            {
                if (game.Tag is NesMiniApplication)
                    workerForm.Games.Add(game.Tag as NesMiniApplication);
            }

            workerForm.FoldersMode = ConfigIni.FoldersMode;
            workerForm.MaxGamesPerFolder = ConfigIni.MaxGamesPerFolder;
            workerForm.FoldersManagerFromThread(workerForm.Games);
        }
    }
}
