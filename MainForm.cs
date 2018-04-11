using com.clusterrr.hakchi_gui.Properties;
using AutoUpdaterDotNET;
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
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using com.clusterrr.hakchi_gui.Tasks;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The URL for the update metadata XML file
        /// </summary>
#if DEBUG
        private static string UPDATE_XML_URL = "https://teamshinkansen.github.io/xml/updates/update-debug.xml";
#else
        private static string UPDATE_XML_URL = "https://teamshinkansen.github.io/xml/updates/update-release.xml";
#endif
        private static string SFROM_TOOL_URL = "http://darkakuma.z-net.us/p/sfromtool.html";

        public enum OriginalGamesPosition { AtTop = 0, AtBottom = 1, Sorted = 2, Hidden = 3 }
        public enum GamesSorting { Name = 0, Core = 1, System = 2 }
        public static string GetConsoleTypeName()
        {
            return GetConsoleTypeName(hakchi.DetectedConsoleType);
        }
        public static string GetConsoleTypeName(hakchi.ConsoleType? c)
        {
            switch (c)
            {
                case hakchi.ConsoleType.NES: return Resources.consoleTypeNes;
                case hakchi.ConsoleType.Famicom: return Resources.consoleTypeFamicom;
                case hakchi.ConsoleType.SNES_EUR: return Resources.consoleTypeSnesEur;
                case hakchi.ConsoleType.SNES_USA: return Resources.consoleTypeSnesUsa;
                case hakchi.ConsoleType.SuperFamicom: return Resources.consoleTypeSuperFamicom;
                case hakchi.ConsoleType.Unknown: return Resources.Unknown;
            }
            return string.Empty;
        }

        public static bool? DownloadCover;
        public const int MaxGamesPerFolder = 50;
        public static mooftpserv.Server FtpServer;
        public static MainForm StaticRef;

        private class GamesSorter : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 is ListViewItem)
                    o1 = (o1 as ListViewItem).Tag;
                if (o2 is ListViewItem)
                    o2 = (o2 as ListViewItem).Tag;
                if (!(o1 is NesApplication))
                    return -1;
                if (!(o2 is NesApplication))
                    return 1;
                return ((o1 as NesApplication).SortName.CompareTo((o2 as NesApplication).SortName));
            }
        }

        public MainForm()
        {
            StaticRef = this;
            InitializeComponent();
            FormInitialize();

            // setup system shell
            hakchi.OnConnected += Shell_OnConnected;
            hakchi.OnDisconnected += Shell_OnDisconnected;
            hakchi.Initialize();

            // setup ftp server for legacy system shell
            FtpServer = new mooftpserv.Server();
            FtpServer.AuthHandler = new mooftpserv.NesMiniAuthHandler();
            FtpServer.FileSystemHandler = new mooftpserv.NesMiniFileSystemHandler(hakchi.Shell);
            FtpServer.LogHandler = new mooftpserv.DebugLogHandler();
            FtpServer.LocalPort = 1021;

            // setup shell menu items
            setupShellMenuItems(null);
        }

        void setWindowTitle()
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action(setWindowTitle));
                return;
            }

            string title = $"hakchi2 CE v{Shared.AppDisplayVersion}";

#if DEBUG
            title += " (Debug";
#if VERY_DEBUG
            title += ", very verbose mode";
#endif
            title += ")"
#endif
            ;

            if (hakchi.MinimalMemboot)
            {
                title += " - " + Resources.RecoveryMode;
            }
            else if (!hakchi.CanInteract)
            {
                if (hakchi.Connected)
                {
                    title += " - " + Resources.CannotInteract;
                }
            }
            else if (hakchi.DetectedConsoleType != null)
            {
                title += " - " + GetConsoleTypeName(hakchi.DetectedConsoleType);
                if (hakchi.CustomFirmwareLoaded) title += " (HSQS)";
            }

            this.Text = title;
        }

        void FormInitialize()
        {
            try
            {
                setWindowTitle();

                // prepare collections
                LoadLanguages();
                CoreCollection.Load();

                // init list view control
                listViewGames.ListViewItemSorter = new GamesSorter();
                listViewGames.DoubleBuffered(true);

                // fill games collections combo box
                foreach (hakchi.ConsoleType c in Enum.GetValues(typeof(hakchi.ConsoleType)))
                    if (c != hakchi.ConsoleType.Unknown)
                        gamesConsoleComboBox.Items.Add(GetConsoleTypeName(c));

                // Little tweak for easy translation
                var tbl = textBoxName.Left;
                textBoxName.Left = labelName.Left + labelName.Width;
                textBoxName.Width -= textBoxName.Left - tbl;
                maskedTextBoxReleaseDate.Left = label1.Left + label1.Width + 3;
                tbl = textBoxPublisher.Left;
                textBoxPublisher.Left = label2.Left + label2.Width;
                textBoxPublisher.Width -= textBoxPublisher.Left - tbl;

                // supported extensions in add games dialog
                string extensions = string.Empty;
                extensions += "All Files|*.*|Archive Files|*.zip;*.7z;*.rar|";
                foreach(var system in CoreCollection.Systems)
                {
                    extensions += system + "|*" + string.Join(";*", CoreCollection.GetExtensionsFromSystem(system).ToArray()) + "|";
                }
                openFileDialogNes.Filter = extensions.Trim('|');
                
                // Loading games database in background
                new Thread(NesGame.LoadCache).Start();
                new Thread(SnesGame.LoadCache).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, "Critical error: " + ex.Message, ex.StackTrace);
            }
        }

        void setupShellMenuItems(ISystemShell caller)
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action(() => { setupShellMenuItems(caller); }));
                return;
            }

            if (caller == null || caller is UnknownShell)
            {
                FTPToolStripMenuItem.Text = string.Format(Resources.FTPServerOn, "127.0.0.1:21");
                FTPToolStripMenuItem.Enabled = true;
                FTPToolStripMenuItem.Checked = ConfigIni.Instance.FtpServer;
                FTPToolStripMenuItem_Click(null, null);

                shellToolStripMenuItem.Text = string.Format(Resources.TelnetServerOn, "127.0.0.1:1023");
                shellToolStripMenuItem.Enabled = true;
                shellToolStripMenuItem.Checked = ConfigIni.Instance.TelnetServer;
                shellToolStripMenuItem_Click(null, null);
            }
            else if (caller is INetworkShell)
            {
                FTPToolStripMenuItem.Text = string.Format(Resources.FTPServerOn, (caller as ssh.SshClientWrapper).IPAddress + ":21");
                FTPToolStripMenuItem.Enabled = false;
                openFTPInExplorerToolStripMenuItem.Enabled = true;
                changeFTPServerState();

                shellToolStripMenuItem.Text = string.Format(Resources.TelnetServerOn, (caller as ssh.SshClientWrapper).IPAddress + ":" + caller.ShellPort);
                shellToolStripMenuItem.Enabled = false;
                openTelnetToolStripMenuItem.Enabled = true;
            }
            else
            {
                FTPToolStripMenuItem.Text = string.Format(Resources.FTPServerOn, "127.0.0.1:21");
                FTPToolStripMenuItem.Enabled = true;
                FTPToolStripMenuItem_Click(null, null);

                shellToolStripMenuItem.Text = string.Format(Resources.TelnetServerOn, "127.0.0.1:1023");
                shellToolStripMenuItem.Enabled = true;
                shellToolStripMenuItem_Click(null, null);
            }
        }

        void Shell_OnConnected(ISystemShell caller)
        {
            try
            {
                // setup ui items
                setupShellMenuItems(caller);
                setWindowTitle();

                // then skip on if in minimal memboot
                if (hakchi.MinimalMemboot)
                    return;

                // detections and updates
                if (hakchi.CanInteract)
                {
                    if (hakchi.DetectedConsoleType != null)
                    {
                        if (hakchi.DetectedConsoleType != hakchi.ConsoleType.Unknown)
                            ConfigIni.Instance.ConsoleType = (hakchi.ConsoleType)hakchi.DetectedConsoleType;
                        ConfigIni.Instance.LastConnectedConsoleType = (hakchi.ConsoleType)hakchi.DetectedConsoleType;
                    }
                    Invoke(new Action(SyncConsoleType));

                    if (hakchi.SystemEligibleForRootfsUpdate())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedScripts, Resources.SystemEligibleForRootfsUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            if (MembootCustomKernel())
                                Tasks.MessageForm.Show(this, Resources.UpdateComplete, Resources.DoneYouCanUpload, Resources.sign_check, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
                            return;
                        }
                    }
                    Invoke(new Action(UpdateLocalCache));
                }
                else
                {
                    if (hakchi.SystemRequiresReflash())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedKernel, Resources.SystemRequiresReflash, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            if (InstallHakchi())
                                Tasks.MessageForm.Show(this, Resources.UpdateComplete, Resources.DoneYouCanUpload, Resources.sign_check, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
                            return;
                        }
                    }
                    else if (hakchi.SystemRequiresRootfsUpdate())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedScripts, Resources.SystemRequiresRootfsUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            if (MembootCustomKernel())
                                Tasks.MessageForm.Show(this, Resources.UpdateComplete, Resources.DoneYouCanUpload, Resources.sign_check, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
                            return;
                        }
                    }

                    // show warning message that any interaction is ill-advised
                    Tasks.MessageForm.Show(this, Resources.Warning, Resources.PleaseUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        void Shell_OnDisconnected()
        {
            Invoke(new Action(SyncConsoleType));
            setupShellMenuItems(hakchi.Shell);
            setWindowTitle();
        }

        static hakchi.ConsoleType lastConsoleType = hakchi.ConsoleType.Unknown;
        static bool lastConnected = false;
        public void SyncConsoleType()
        {
            // skip the rest if unchanged
            if (ConfigIni.Instance.ConsoleType == lastConsoleType && lastConnected == hakchi.Connected)
            {
                return;
            }
            lastConsoleType = ConfigIni.Instance.ConsoleType;
            lastConnected = hakchi.Connected;

            MemoryStats.DebugDisplay();

            // select games collection
            for (int i = 0; i < gamesConsoleComboBox.Items.Count; ++i)
            {
                if (GetConsoleTypeName(ConfigIni.Instance.ConsoleType) == gamesConsoleComboBox.Items[i] as string)
                {
                    gamesConsoleComboBox.SelectedIndex = i;
                    break;
                }
            }

            // console settings
            enableUSBHostToolStripMenuItem.Checked = ConfigIni.Instance.UsbHost;
            useExtendedFontToolStripMenuItem.Checked = ConfigIni.Instance.UseFont;
            epilepsyProtectionToolStripMenuItem.Checked = ConfigIni.Instance.AntiArmetLevel > 0;
            selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.Instance.ResetHack;
            enableAutofireToolStripMenuItem.Checked = ConfigIni.Instance.AutofireHack;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked = ConfigIni.Instance.AutofireXYHack;
            upABStartOnSecondControllerToolStripMenuItem.Enabled = true;
            upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.Instance.FcStart && upABStartOnSecondControllerToolStripMenuItem.Enabled;

            // enable/disable options based on console being connected and can interact or not
            enableUSBHostToolStripMenuItem.Enabled =
                useExtendedFontToolStripMenuItem.Enabled =
                epilepsyProtectionToolStripMenuItem.Enabled =
                cloverconHackToolStripMenuItem.Enabled =
                globalCommandLineArgumentsexpertsOnluToolStripMenuItem.Enabled =
                saveSettingsToNESMiniNowToolStripMenuItem.Enabled = (hakchi.Connected && hakchi.CanInteract);

            // more settings
            compressGamesToolStripMenuItem.Checked = ConfigIni.Instance.Compress;
            compressBoxArtToolStripMenuItem.Checked = ConfigIni.Instance.CompressCover;
            centerBoxArtThumbnailToolStripMenuItem.Checked = ConfigIni.Instance.CenterThumbnail;
            separateGamesForMultibootToolStripMenuItem.Checked = ConfigIni.Instance.SeparateGameStorage;
            disableHakchi2PopupsToolStripMenuItem.Checked = ConfigIni.Instance.DisablePopups;
            useLinkedSyncToolStripMenuItem.Checked = ConfigIni.Instance.SyncLinked;

            // sfrom tool
            enableSFROMToolToolStripMenuItem.Checked = ConfigIni.Instance.UseSFROMTool;
            usePCMPatchWhenAvailableToolStripMenuItem.Checked = ConfigIni.Instance.UsePCMPatch;
            if (SfromToolWrapper.IsInstalled)
            {
                usePCMPatchWhenAvailableToolStripMenuItem.Enabled = enableSFROMToolToolStripMenuItem.Checked;
            }
            else
            {
                ConfigIni.Instance.UseSFROMTool = enableSFROMToolToolStripMenuItem.Checked = false;
                usePCMPatchWhenAvailableToolStripMenuItem.Enabled = false;
            }

            // initial view menu
            positionAtTheTopToolStripMenuItem.Checked = ConfigIni.Instance.OriginalGamesPosition == OriginalGamesPosition.AtTop;
            positionAtTheBottomToolStripMenuItem.Checked = ConfigIni.Instance.OriginalGamesPosition == OriginalGamesPosition.AtBottom;
            positionSortedToolStripMenuItem.Checked = ConfigIni.Instance.OriginalGamesPosition == OriginalGamesPosition.Sorted;
            positionHiddenToolStripMenuItem.Checked = ConfigIni.Instance.OriginalGamesPosition == OriginalGamesPosition.Hidden;
            nameToolStripMenuItem.Checked = ConfigIni.Instance.GamesSorting == GamesSorting.Name;
            coreToolStripMenuItem.Checked = ConfigIni.Instance.GamesSorting == GamesSorting.Core;
            systemToolStripMenuItem.Checked = ConfigIni.Instance.GamesSorting == GamesSorting.System;
            showGamesWithoutBoxArtToolStripMenuItem.Checked = ConfigIni.Instance.ShowGamesWithoutCoverArt;
            devForceSshToolStripMenuItem.Checked = ConfigIni.Instance.ForceSSHTransfers;
            uploadTotmpforTestingToolStripMenuItem.Checked = ConfigIni.Instance.UploadToTmp;

            // folders modes
            disablePagefoldersToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 0;
            automaticToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 2;
            automaticOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 3;
            pagesToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 4;
            pagesOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 5;
            foldersToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 6;
            foldersOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 7;
            foldersSplitByFirstLetterToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 8;
            foldersSplitByFirstLetterOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 9;
            customToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 99;

            // items per folder
            maximumGamesPerFolderToolStripMenuItem.DropDownItems.Clear();
            for (byte f = 20; f <= 100; f += ((f < 50) ? (byte)5 : (byte)10))
            {
                var item = new ToolStripMenuItem();
                item.Name = "folders" + f.ToString();
                item.Text = f.ToString();
                item.Tag = f;
                if (f >= MaxGamesPerFolder)
                    item.Text += $" ({Resources.NotRecommended})";
                item.Checked = ConfigIni.Instance.MaxGamesPerFolder == f;
                item.Click += delegate (object sender, EventArgs e)
                {
                    var old = maximumGamesPerFolderToolStripMenuItem.DropDownItems.Find("folders" + ConfigIni.Instance.MaxGamesPerFolder.ToString(), true);
                    if (old.Count() > 0)
                        (old.First() as ToolStripMenuItem).Checked = false;
                    ConfigIni.Instance.MaxGamesPerFolder = (byte)((sender as ToolStripMenuItem).Tag);
                    var n = maximumGamesPerFolderToolStripMenuItem.DropDownItems.Find("folders" + ConfigIni.Instance.MaxGamesPerFolder.ToString(), true);
                    if (n.Count() > 0)
                        (n.First() as ToolStripMenuItem).Checked = true;
                };
                maximumGamesPerFolderToolStripMenuItem.DropDownItems.Add(item);
            }

            // back folder position
            leftmostToolStripMenuItem.Checked = ConfigIni.Instance.BackFolderPosition == NesMenuFolder.Priority.LeftBack;
            rightmostToolStripMenuItem.Checked = ConfigIni.Instance.BackFolderPosition == NesMenuFolder.Priority.Back;

            // load lists
            LoadPresets();
            LoadFolderImageSets();
            LoadGames();
        }

        void UpdateLocalCache()
        {
            string cachePath = Path.Combine(Program.BaseDirectoryExternal, "games_cache");
            var games = new NesMenuCollection();
            foreach (NesDefaultGame game in NesApplication.DefaultGames)
            {
                if (!Directory.Exists(Path.Combine(cachePath, game.Code)))
                    games.Add(game);
            }

            if (games.Count > 0)
            {
                using (var tasker = new Tasks.Tasker(this))
                {
                    var task = new Tasks.GameCacheTask();
                    task.Games = games;
                    tasker.AttachView(new Tasks.TaskerTaskbar());
                    tasker.AttachView(new Tasks.TaskerForm());
                    tasker.AddTask(task.UpdateLocal);
                    if (tasker.Start() == Tasks.Tasker.Conclusion.Success)
                        Debug.WriteLine("Successfully updated local original games cache.");
                }
            }
            else
                Debug.WriteLine("Local original games cache in sync.");
        }

        public void LoadGames(bool reloadFromFiles = true)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                var task = new Tasks.LoadGamesTask(listViewGames, reloadFromFiles);
                tasker.AttachView(new Tasks.TaskerForm());
                tasker.AddTask(task.LoadGames, 0);
                Tasks.Tasker.Conclusion c = tasker.Start();
            }
            new Thread(RecalculateSelectedGamesThread).Start();
            ShowSelected();
        }

        private bool showingSelected = false;
        public void ShowSelected()
        {
            object selected = null;
            var selectedAll = listViewGames.SelectedItems;
            if (selectedAll.Count == 1) selected = selectedAll[0].Tag;

            showingSelected = true;
            if (selected == null)
            {
                groupBoxOptions.Visible = true;
                groupBoxOptions.Enabled = false;
                labelID.Text = "ID: ";
                labelSize.Text = Resources.Size;
                textBoxName.Text = "";
                textBoxSortName.Text = "";
                radioButtonOne.Checked = true;
                radioButtonTwo.Checked = false;
                radioButtonTwoSim.Checked = false;
                maskedTextBoxReleaseDate.Text = "";
                textBoxPublisher.Text = "";
                textBoxArguments.Text = "";
                numericUpDownSaveCount.Value = 0;
                pictureBoxArt.Image = Resources.noboxart;
                pictureBoxThumbnail.Image = null;
                pictureBoxThumbnail.Visible = false;
                buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = false;
                textBoxGameGenie.Text = "";
                checkBoxCompressed.Enabled = false;
                checkBoxCompressed.Checked = false;
            }
            else
            {
                var app = selected as NesApplication;
                groupBoxOptions.Visible = true;
                labelID.Text = "ID: " + app.Code;
                labelSize.Text = $"{Resources.Size} {Shared.SizeSuffix(app.Size())}";
                textBoxName.Text = app.Name;
                textBoxSortName.Text = app.SortName;
                if (app.Desktop.Simultaneous && app.Desktop.Players == 2)
                    radioButtonTwoSim.Checked = true;
                else if (app.Desktop.Players == 2)
                    radioButtonTwo.Checked = true;
                else
                    radioButtonOne.Checked = true;
                maskedTextBoxReleaseDate.Text = app.Desktop.ReleaseDate;
                textBoxPublisher.Text = app.Desktop.Publisher;
                textBoxArguments.Text = app.Desktop.Exec;
                numericUpDownSaveCount.Value = app.Desktop.SaveCount;
                pictureBoxArt.Image = app.Image ?? Resources.noboxart;
                pictureBoxThumbnail.Image = app.Thumbnail;
                pictureBoxThumbnail.Visible = true;
                if (app.IsOriginalGame || !(app is ISupportsGameGenie))
                {
                    buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = false;
                    textBoxGameGenie.Text = "";
                }
                else
                {
                    buttonShowGameGenieDatabase.Enabled = app is NesGame; //ISupportsGameGenie;
                    textBoxGameGenie.Enabled = true;
                    textBoxGameGenie.Text = app.GameGenie;
                }
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
            showingSelected = false;
        }

        void LoadPresets()
        {
            while (presetsToolStripMenuItem.DropDownItems.Count > 3)
                presetsToolStripMenuItem.DropDownItems.RemoveAt(0);
            deletePresetToolStripMenuItem.Enabled = false;
            deletePresetToolStripMenuItem.DropDownItems.Clear();
            int i = 0;
            foreach (var preset in ConfigIni.Instance.Presets.Keys.OrderBy(o => o))
            {
                presetsToolStripMenuItem.DropDownItems.Insert(i, new ToolStripMenuItem(preset, null,
                    delegate (object sender, EventArgs e)
                    {
                        var presetSelected = ConfigIni.Instance.Presets[preset];
                        for (int j = 1; j < listViewGames.Items.Count; j++)
                        {
                            var code = (listViewGames.Items[j].Tag as NesApplication).Code;
                            if (presetSelected.Contains(code))
                            {
                                listViewGames.Items[j].Checked = true;
                            }
                            else
                            {
                                listViewGames.Items[j].Checked = false;
                            }
                        }
                        SaveSelectedGames();
                    }));
                deletePresetToolStripMenuItem.DropDownItems.Insert(i, new ToolStripMenuItem(preset, null,
                    delegate (object sender, EventArgs e)
                    {
                        if (Tasks.MessageForm.Show(this, Resources.AreYouSure, string.Format(Resources.DeletePreset, preset), Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            ConfigIni.Instance.Presets.Remove(preset);
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
                        ConfigIni.Instance.Language = langCodes[language];
                        SaveConfig();
                        lastConsoleType = hakchi.ConsoleType.Unknown;

                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCodes[language]);
                        this.Hide();
                        this.Controls.Clear();
                        this.InitializeComponent();
                        this.FormInitialize();
                        this.setupShellMenuItems(hakchi.Shell);
                        this.SyncConsoleType();
                        this.Show();
                        //this.Invalidate(true);
                    };
                if (Thread.CurrentThread.CurrentUICulture.Name.ToUpper() == langCodes[language].ToUpper())
                {
                    item.Checked = true;
                    if (string.IsNullOrEmpty(ConfigIni.Instance.Language))
                        ConfigIni.Instance.Language = langCodes[language];
                }
                found |= item.Checked;
                if (langCodes[language] == "en-US")
                    english = item;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }
            if (!found)
                english.Checked = true;
        }

        private void LoadFolderImageSets()
        {
            folderImagesSetToolStripMenuItem.DropDownItems.Clear();
            var newItem = new ToolStripMenuItem(Resources.Default, null, delegate (object sender, EventArgs e)
            {
                ConfigIni.Instance.FolderImagesSet = string.Empty;

                folderImagesSetToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().ToList().ForEach(item => item.Checked = false);
                (sender as ToolStripMenuItem).Checked = true;
            });
            if (string.IsNullOrEmpty(ConfigIni.Instance.FolderImagesSet))
                newItem.Checked = true;
            folderImagesSetToolStripMenuItem.DropDownItems.Add(newItem);
            folderImagesSetToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            string imgPath = Path.Combine(Program.BaseDirectoryExternal, "folder_images");
            foreach (var dir in Directory.GetDirectories(imgPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(dir);
                newItem = new ToolStripMenuItem(name, null, delegate (object sender, EventArgs e)
                {
                    ConfigIni.Instance.FolderImagesSet = name;

                    folderImagesSetToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().ToList().ForEach(item => item.Checked = false);
                    (sender as ToolStripMenuItem).Checked = true;
                });
                if (ConfigIni.Instance.FolderImagesSet == name)
                    newItem.Checked = true;
                folderImagesSetToolStripMenuItem.DropDownItems.Add(newItem);
            }
        }

        private void SaveSelectedGames()
        {
            Debug.WriteLine("Saving selected games");
            var selected = ConfigIni.Instance.SelectedGames;
            var original = ConfigIni.Instance.OriginalGames;
            selected.Clear();
            if (ConfigIni.Instance.OriginalGamesPosition != OriginalGamesPosition.Hidden)
                original.Clear();

            foreach (ListViewItem item in listViewGames.CheckedItems)
            {
                if (item.Tag is NesApplication)
                {
                    NesApplication game = item.Tag as NesApplication;
                    (game.IsOriginalGame ? original : selected).Add(game.Code);
                }
            }
        }

        private void SaveConfig()
        {
            SaveSelectedGames();
            ConfigIni.Save();
            foreach (ListViewItem game in listViewGames.Items)
            {
                try
                {
                    if (game.Tag is NesApplication)
                    {
                        // Maybe type was changed? Need to reload games
                        if ((game.Tag as NesApplication).Save())
                            game.Tag = NesApplication.FromDirectory((game.Tag as NesApplication).BasePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Tasks.ErrorForm.Show(Resources.Error, ex.Message, ex.StackTrace);
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
                    ConfigIni.Instance.Presets[name] =
                        Shared.ConcatArrays(ConfigIni.Instance.SelectedGames.ToArray(), ConfigIni.Instance.OriginalGames.ToArray()).ToList();
                    LoadPresets();
                }
            }
        }

        private void listViewGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int c = listViewGames.SelectedItems.Count;
            ListViewItem item = c == 1 ? listViewGames.SelectedItems[0] : null;
            if (item != null && item.Tag is NesApplication && (item.Tag as NesApplication).IsDeleting) c = 0;

            if (c == 0)
            {
                explorerToolStripMenuItem.Enabled = 
                    downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesBoxArtToolStripMenuItem.Enabled =
                    compressSelectedGamesToolStripMenuItem.Enabled =
                    decompressSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesToolStripMenuItem.Enabled =
                    sFROMToolToolStripMenuItem1.Enabled =
                    repairGamesToolStripMenuItem.Enabled =
                    selectEmulationCoreToolStripMenuItem.Enabled = false;
            }
            else if (c == 1)
            {
                explorerToolStripMenuItem.Enabled =
                    downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesBoxArtToolStripMenuItem.Enabled = true;

                deleteSelectedGamesToolStripMenuItem.Enabled = 
                    repairGamesToolStripMenuItem.Enabled =
                    selectEmulationCoreToolStripMenuItem.Enabled = !(item.Tag as NesApplication).IsOriginalGame;
                compressSelectedGamesToolStripMenuItem.Enabled = (item.Tag as NesApplication).CompressPossible().Count() > 0;
                decompressSelectedGamesToolStripMenuItem.Enabled = (item.Tag as NesApplication).DecompressPossible().Count() > 0;

                sFROMToolToolStripMenuItem1.Enabled =
                    editROMHeaderToolStripMenuItem.Enabled =
                    resetROMHeaderToolStripMenuItem.Enabled =
                        SfromToolWrapper.IsInstalled &&
                        (item.Tag is SnesGame &&
                        !(item.Tag as SnesGame).IsOriginalGame &&
                        ((item.Tag as SnesGame).GameFilePath ?? "").ToLower().Contains(".sfrom"));
            }
            else
            {
                explorerToolStripMenuItem.Enabled =
                    editROMHeaderToolStripMenuItem.Enabled = false;

                downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesBoxArtToolStripMenuItem.Enabled =
                    compressSelectedGamesToolStripMenuItem.Enabled =
                    decompressSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesToolStripMenuItem.Enabled =
                    repairGamesToolStripMenuItem.Enabled =
                    selectEmulationCoreToolStripMenuItem.Enabled = true;

                sFROMToolToolStripMenuItem1.Enabled =
                    resetROMHeaderToolStripMenuItem.Enabled = SfromToolWrapper.IsInstalled;

            }

            if (!e.IsSelected)
                (e.Item.Tag as NesApplication).Save();

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
                        editROMHeaderToolStripMenuItem_Click(sender, e);
                    }
                    break;
            }
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
                    if ((listViewGames.SelectedItems.Count > 1) || (listViewGames.SelectedItems.Count == 1 && listViewGames.SelectedItems[0].Tag is NesApplication))
                        deleteSelectedGamesToolStripMenuItem_Click(null, null);
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

        private NesApplication GetSelectedGame()
        {
            if (listViewGames.SelectedItems.Count != 1) return null;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return null;
            return selected as NesApplication;
        }

        private void SetImageForSelectedGame(string imagePath)
        {
            var app = GetSelectedGame();
            if (app != null)
            {
                app.SetImageFile(imagePath, ConfigIni.Instance.CompressCover);
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

            openFileDialogImage.Filter = Resources.Images + "|*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff|" + Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                app.SetImageFile(openFileDialogImage.FileName, ConfigIni.Instance.CompressCover);
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void pictureBoxThumbnail_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app == null) return;

            openFileDialogImage.Filter = Resources.Images + "|*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff|" + Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                app.SetThumbnailFile(openFileDialogImage.FileName, ConfigIni.Instance.CompressCover);
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var app = GetSelectedGame();
            if (app == null) return;

            using (var googler = new ImageGooglerForm(app))
            {
                if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    app.Image = googler.Result;
                    ShowSelected();
                    timerCalculateGames.Enabled = true;
                }
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selectedItem = listViewGames.SelectedItems[0];
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            if (selectedItem.Text != textBoxName.Text)
            {
                var newSortName = textBoxName.Text.ToLower();
                if (newSortName.StartsWith("the "))
                    newSortName = newSortName.Substring(4); // Sorting without "THE"
                selectedItem.Text = game.Name = textBoxName.Text;
                textBoxSortName.Text = newSortName;
            }
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            listViewGames.Sort();
        }

        private void textBoxSortName_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selectedItem = listViewGames.SelectedItems[0];
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.Desktop.SortName = textBoxSortName.Text = textBoxSortName.Text.ToLower();
        }

        private void textBoxSortName_Leave(object sender, EventArgs e)
        {
            listViewGames.Sort();
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.Desktop.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Desktop.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.Desktop.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void numericUpDownSaveCount_ValueChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            if (numericUpDownSaveCount.Value < 0)
                numericUpDownSaveCount.Value = 0;
            if (numericUpDownSaveCount.Value > 3)
                numericUpDownSaveCount.Value = 3;
            game.Desktop.SaveCount = decimal.ToByte(numericUpDownSaveCount.Value);
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.Desktop.Exec = textBoxArguments.Text;
        }

        private void maskedTextBoxReleaseDate_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.Desktop.ReleaseDate = maskedTextBoxReleaseDate.Text;
        }

        private void textBoxGameGenie_TextChanged(object sender, EventArgs e)
        {
            if (showingSelected) return;
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected == null || !(selected is NesApplication)) return;
            var game = (selected as NesApplication);
            game.GameGenie = textBoxGameGenie.Text;
        }

        private void buttonShowGameGenieDatabase_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (!(selected is ISupportsGameGenie)) return;
            NesApplication nesGame = selected as NesApplication;
            GameGenieCodeForm lFrm = new GameGenieCodeForm(nesGame);
            if (lFrm.ShowDialog() == DialogResult.OK)
                textBoxGameGenie.Text = (nesGame as NesApplication).GameGenie;
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
                if (game is NesApplication)
                {
                    stats.Count++;
                    stats.Size += (game as NesApplication).Size();
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
                    Invoke(new Action<CountResult>(showStats), new Object[] { stats });
                    return;
                }
                var maxGamesSize = MemoryStats.DefaultMaxGamesSize * 1024 * 1024;
                if (MemoryStats.StorageTotal > 0)
                {
                    maxGamesSize = MemoryStats.AvailableForGames();
                    toolStripStatusLabelSize.Text = string.Format("{0} / {1}", Shared.SizeSuffix(stats.Size), Shared.SizeSuffix(maxGamesSize));
                }
                else
                {
                    toolStripStatusLabelSize.Text = string.Format("{0} / ???MB", Shared.SizeSuffix(stats.Size));
                }
                double usagePercentage = ((double)stats.Size / (double)maxGamesSize);
                if (usagePercentage > 1.0)
                {
                    usagePercentage = 1.0;
                }
                toolStripStatusLabelSelected.Text = stats.Count + " " + Resources.GamesSelected;
                toolStripProgressBar.Maximum = int.MaxValue;
                toolStripProgressBar.Value = Convert.ToInt32(usagePercentage * int.MaxValue);
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
            listViewGames.BeginUpdate();
            foreach (ListViewItem item in listViewGames.Items)
                item.Selected = false;
            listViewGames.EndUpdate();
            SaveConfig();
            LoadGames();
        }

        private void structureButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            Point ptLowerLeft = button.PointToScreen(new Point(1, button.Height));
            foldersContextMenuStrip.Show(ptLowerLeft);
        }

        private void gamesConsoleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = gamesConsoleComboBox.SelectedItem as string;
            foreach (hakchi.ConsoleType c in Enum.GetValues(typeof(hakchi.ConsoleType)))
            {
                if (GetConsoleTypeName(c) == selected)
                {
                    if (ConfigIni.Instance.ConsoleType != c)
                    {
                        SaveSelectedGames();

                        ConfigIni.Instance.ConsoleType = c;
                        SyncConsoleType();
                        return;
                    }
                }
            }

        }

        Tasks.MessageForm.Button RequirePatchedKernel()
        {
            if (hakchi.Shell.IsOnline) return Tasks.MessageForm.Button.OK; // OK - Shell is online
            if (Tasks.MessageForm.Show(this, Resources.CustomKernel, Resources.CustomWarning, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (InstallHakchi())
                    return Tasks.MessageForm.Button.Yes; // Succesfully flashed
                else
                    return Tasks.MessageForm.Button.No; // Not flashed for some other reason
            }
            else
            {
                return Tasks.MessageForm.Button.No;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (listViewGames.CheckedItems.Count == 0)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.SelectAtLeast, Resources.sign_info);
                return;
            }
            if (RequirePatchedKernel() == Tasks.MessageForm.Button.No)
            {
                return;
            }
            if (hakchi.MinimalMemboot)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.CannotProceedMinimalMemboot, Resources.sign_error);
                return;
            }
            if (!hakchi.CanInteract)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.CannotProceedCannotInteract, Resources.sign_error);
                return;
            }
            if (ConfigIni.Instance.SeparateGameStorage)
            {
                if (MemoryStats.NonMultibootGamesSize > 0)
                {
                    if (Tasks.MessageForm.Show(
                        Resources.UploadGames,
                        string.Format(Resources.SyncMultibootWarning, Shared.SizeSuffix(MemoryStats.NonMultibootGamesSize)),
                        Resources.sign_delete, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }) != Tasks.MessageForm.Button.Yes)
                        return;
                }
            }
            else
            {
                if (hakchi.DetectedConsoleType != ConfigIni.Instance.ConsoleType)
                {
                    Tasks.MessageForm.Show(
                        Resources.UploadGames,
                        string.Format(Resources.CannotSyncToNonMultiBoot, GetConsoleTypeName(ConfigIni.Instance.ConsoleType), GetConsoleTypeName()),
                        Resources.sign_ban);
                    return;
                }
                if (MemoryStats.AllGamesSize - MemoryStats.NonMultibootGamesSize > 0)
                {
                    if (Tasks.MessageForm.Show(
                        Resources.UploadGames,
                        string.Format(Resources.SyncNonMultibootWarning, Shared.SizeSuffix(MemoryStats.AllGamesSize - MemoryStats.NonMultibootGamesSize)),
                        Resources.sign_delete, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }) != Tasks.MessageForm.Button.Yes)
                    return;
                }
            }
            SaveConfig();
            if (UploadGames())
            {
                new Thread(RecalculateSelectedGamesThread).Start();
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.UploadGames, Resources.Done, Resources.sign_check);
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveConfig();
            var stats = RecalculateSelectedGames();
            if (stats.Count == 0)
            {
                Tasks.MessageForm.Show(Resources.ExportGames, Resources.SelectAtLeast, Resources.sign_info);
                return;
            }
            if (UploadGames(true))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.ExportGames, Resources.Done, Resources.sign_check);
        }

        bool UploadGames(bool exportGames = false)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                var syncTask = new Tasks.SyncTask();
                foreach (ListViewItem item in listViewGames.CheckedItems)
                {
                    if (item.Tag is NesApplication)
                        syncTask.Games.Add(item.Tag as NesApplication);
                }
                tasker.AddTask(exportGames ? (Tasks.Tasker.TaskFunc)syncTask.ExportGames : (Tasks.Tasker.TaskFunc)syncTask.UploadGames);
                Tasks.Tasker.Conclusion c = tasker.Start();

                return c == Tasks.Tasker.Conclusion.Success;
            }
        }

        bool DumpDialog(FileAccess type, string FileName, string FileExt, out string DumpFileName)
        {
            DumpFileName = null;
            switch (type)
            {
                case FileAccess.Read:
                    openDumpFileDialog.FileName = FileName;
                    openDumpFileDialog.DefaultExt = FileExt;
                    if (openDumpFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        DumpFileName = openDumpFileDialog.FileName;
                        return true;
                    }
                    return false;

                case FileAccess.Write:
                    saveDumpFileDialog.FileName = FileName;
                    saveDumpFileDialog.DefaultExt = FileExt;
                    if (saveDumpFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        DumpFileName = saveDumpFileDialog.FileName;
                        return true;
                    }
                    return false;

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        bool DoNand(MembootTasks.NandTasks task, string title)
        {
            using (Tasker tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_cogs);
                tasker.SetTitle(title);
                string dumpFilename = null;
                switch (task)
                {
                    case MembootTasks.NandTasks.DumpNand:
                        if (!DumpDialog(FileAccess.Write, "nand.bin", "bin", out dumpFilename))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpNand, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.DumpNandB:
                        if (!DumpDialog(FileAccess.Write, "nandb.hsqs", "hsqs", out dumpFilename))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpNandB, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.DumpNandC:
                        if (!DumpDialog(FileAccess.Write, "nandc.hsqs", "hsqs", out dumpFilename))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpNandC, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.FlashNandB:
                        if (!DumpDialog(FileAccess.Read, "nandb.hsqs", "hsqs", out dumpFilename))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FlashNandB, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.FlashNandC:
                        if (!DumpDialog(FileAccess.Read, "nandc.hsqs", "hsqs", out dumpFilename))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FlashNandC, dumpPath: dumpFilename).Tasks);
                        break;
                        
                    case MembootTasks.NandTasks.FormatNandC:
                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FormatNandC).Tasks);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("task");
                }
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        bool InstallHakchi(bool reset = false)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetTitle(reset ? Resources.ResettingHakchi : Resources.InstallingHakchi);
                tasker.SetStatusImage(Resources.sign_keyring);
                if (reset)
                {
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.ResetHakchi).Tasks);
                }
                else
                {
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.InstallHakchi).Tasks);
                }
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        bool MembootCustomKernel()
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_keyring);
                tasker.SetTitle(Resources.Membooting);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.Memboot).Tasks);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        void AddGames(IEnumerable<string> files)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                var task = new Tasks.AddGamesTask(listViewGames, files);
                tasker.AddTask(task.AddGames, 4);
                tasker.AddTask(task.UpdateListView);

                if (tasker.Start() == Tasks.Tasker.Conclusion.Success)
                {
                    // Schedule recalculation
                    timerCalculateGames.Enabled = true;
                }
                else
                {
                    LoadGames(); // Reload all games (maybe process was terminated?)
                }
            }
        }

        bool Uninstall()
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_trashcan);
                tasker.SetTitle(Resources.UninstallingHakchi);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.UninstallHakchi).Tasks);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        bool InstallMods(string[] mods)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetTitle(Resources.InstallingMods);
                tasker.SetStatusImage(Resources.sign_brick);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery).Tasks);
                tasker.AddTask(ShellTasks.MountBase);
                tasker.AddTask(hakchi.ShowSplashScreen);
                tasker.AddTasks(new ModTasks(mods).Tasks);
                tasker.AddFinalTask(MembootTasks.BootHakchi);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        bool UninstallMods(string[] mods)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetTitle(Resources.UninstallingMods);
                tasker.SetStatusImage(Resources.sign_brick);
                if (!hakchi.Shell.IsOnline)
                {
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery).Tasks);
                    tasker.AddTask(ShellTasks.MountBase);
                }
                tasker.AddTask(hakchi.ShowSplashScreen);
                tasker.AddTasks(new ModTasks(null, mods).Tasks);
                tasker.AddFinalTask(ShellTasks.Reboot);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.UninstallQ1, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (Uninstall())
                    Tasks.MessageForm.Show(Resources.Done, Resources.UninstallNote, Resources.sign_check);
            }
        }

        private void normalModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.FlashUbootNormalQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                using (var tasker = new Tasker(this))
                {
                    tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                    tasker.SetTitle(Resources.FlashingUboot);
                    tasker.SetStatusImage(Resources.sign_cogs);
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FlashNormalUboot).Tasks);
                    if (tasker.Start() == Tasker.Conclusion.Success)
                        if (!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.FlashingUboot, Resources.Done, Resources.sign_check);

                }
            }
        }

        private void sDModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.FlashUbootSDQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                using (var tasker = new Tasker(this))
                {
                    tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                    tasker.SetTitle(Resources.FlashingUboot);
                    tasker.SetStatusImage(Resources.sign_cogs);
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FlashSDUboot).Tasks);
                    if (tasker.Start() == Tasker.Conclusion.Success)
                        if (!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.FlashingUboot, Resources.Done, Resources.sign_check);
                }
            }
        }

        private void dumpTheWholeNANDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DoNand(MembootTasks.NandTasks.DumpNand, ((ToolStripMenuItem)sender).Text))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Done, Resources.NandDumped, Resources.sign_check);
        }

        private void toolFlashTheWholeNANDStripMenuItem_Click(object sender, EventArgs e)
        {
            // Maybe I'll fix it one day...
            if (MessageBox.Show("It will brick your console. Do you want to continue?", Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                throw new NotImplementedException();
            }
        }

        private void dumpNANDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DoNand(MembootTasks.NandTasks.DumpNandB, ((ToolStripMenuItem)sender).Text))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Done, Resources.NandDumped, Resources.sign_check);
        }


        private void flashNANDBPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DoNand(MembootTasks.NandTasks.FlashNandB, ((ToolStripMenuItem)sender).Text))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Done, Resources.NandFlashed, Resources.sign_check);
        }

        private void dumpNANDCPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DoNand(MembootTasks.NandTasks.DumpNandC, ((ToolStripMenuItem)sender).Text))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Done, Resources.NandDumped, Resources.sign_check);
        }

        private void flashNANDCPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.FlashNandCQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (DoNand(MembootTasks.NandTasks.FlashNandC, ((ToolStripMenuItem)sender).Text))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Done, Resources.NandFlashed, Resources.sign_check);
            }
        }

        private void formatNANDCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.FormatNandCQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (DoNand(MembootTasks.NandTasks.FormatNandC, ((ToolStripMenuItem)sender).Text))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Done, Resources.NandFormatted, Resources.sign_check);
            }
        }

        private void flashCustomKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.CustomKernelQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (InstallHakchi())
                    Tasks.MessageForm.Show(Resources.CustomKernel, Resources.DoneYouCanUpload, Resources.sign_check);
            }
        }

        private void membootOriginalKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_keyring);
                tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootOriginal).Tasks);
                tasker.Start();
            }
        }
        
        private void membootCustomKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_keyring);
                tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.Memboot).Tasks);
                tasker.Start();
            }
        }

        private void membootRecoveryKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_life_buoy);
                tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery).Tasks);
                if (tasker.Start() == Tasker.Conclusion.Success)
                    Tasks.MessageForm.Show(Resources.RecoveryKernel, Resources.RecoveryModeMessage, Resources.sign_info);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(this, Resources.AreYouSure, Resources.ResetQ, Resources.sign_warning, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) == MessageForm.Button.Yes)
            {
                if (InstallHakchi(true))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void factoryResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(this, Resources.Warning, Resources.FactoryResetQ, Resources.sign_warning, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) == MessageForm.Button.Yes)
            {
                using (var tasker = new Tasker(this))
                {
                    tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                    tasker.SetStatusImage(Resources.sign_trashcan);
                    tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                    tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FactoryReset).Tasks);
                    if (tasker.Start() == Tasker.Conclusion.Success)
                        if(!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.Done, Resources.FactoryResetNote, Resources.sign_check);
                }
            }
        }

        private void installModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            installModules();
        }

        private void installModules(string[] add = null)
        {
            using (var form = new SelectModsForm(false, true, add))
            {
                form.Text = Resources.SelectModsInstall;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    List<string> hmods = new List<string>();
                    foreach (ListViewItem item in form.listViewHmods.CheckedItems)
                    {
                        if (((Hmod)item.Tag).isInstalled) continue;
                        hmods.Add(((Hmod)item.Tag).RawName);
                    }
                    if (hmods.Count == 0) return;
                    if (InstallMods(hmods.ToArray()))
                    {
                        if (!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                    }
                }
            }
        }

        private void uninstallModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SelectModsForm(true, false))
            {
                form.Text = Resources.SelectModsUninstall;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    List<string> hmods = new List<string>();
                    foreach (ListViewItem item in form.listViewHmods.CheckedItems)
                    {
                        hmods.Add(((Hmod)item.Tag).RawName);
                    }
                    if (hmods.Count == 0) return;
                    if (UninstallMods(hmods.ToArray()))
                    {
                        if (!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutBox())
            {
                about.Text = aboutToolStripMenuItem.Text.Replace("&", "");
                about.ShowDialog();
            }
        }

        private void gitHubPageWithActualReleasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TeamShinkansen/hakchi2/releases");
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TeamShinkansen/hakchi2/wiki/FAQ");
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
            ConfigIni.Instance.UseFont = useExtendedFontToolStripMenuItem.Checked;
        }

        private void ToolStripMenuItemArmet_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.AntiArmetLevel = epilepsyProtectionToolStripMenuItem.Checked ? (byte)2 : (byte)0;
        }

        private void cloverconHackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectButtonCombinationToolStripMenuItem.Enabled =
                ConfigIni.Instance.ResetHack = resetUsingCombinationOfButtonsToolStripMenuItem.Checked;
        }

        private void upABStartOnSecondControllerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.FcStart = upABStartOnSecondControllerToolStripMenuItem.Checked;
        }

        private void selectButtonCombinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (ConfigIni.Instance.ConsoleType)
            {
                default:
                case hakchi.ConsoleType.NES:
                case hakchi.ConsoleType.Famicom:
                    using (var form = new SelectNesButtonsForm((SelectNesButtonsForm.NesButtons)ConfigIni.Instance.ResetCombination))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                            ConfigIni.Instance.ResetCombination = (uint)form.SelectedButtons;
                    }
                    break;
                case hakchi.ConsoleType.SNES_EUR:
                case hakchi.ConsoleType.SNES_USA:
                case hakchi.ConsoleType.SuperFamicom:
                    using (var form = new SelectSnesButtonsForm((SelectSnesButtonsForm.SnesButtons)ConfigIni.Instance.ResetCombination))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                            ConfigIni.Instance.ResetCombination = (uint)form.SelectedButtons;
                    }
                    break;
            }
        }

        public void ResetOriginalGamesForCurrentSystem(bool nonDestructiveSync = false)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());

                var task = new Tasks.GameTask();
                task.ResetAllOriginalGames = false;
                task.NonDestructiveSync = nonDestructiveSync;
                tasker.AddTask(task.SyncOriginalGames);

                var conclusion = tasker.Start();
                if (conclusion == Tasks.Tasker.Conclusion.Success && !ConfigIni.Instance.DisablePopups)
                {
                    Tasks.MessageForm.Show(Resources.Default30games, Resources.Done, Resources.sign_check);
                }
            }
            LoadGames();
            timerCalculateGames.Enabled = true;
        }

        public void ResetOriginalGamesForAllSystems()
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());

                var task = new Tasks.GameTask();
                task.ResetAllOriginalGames = true;
                task.NonDestructiveSync = true;
                tasker.AddTask(task.SyncOriginalGames);

                var conclusion = tasker.Start();
                if (conclusion == Tasks.Tasker.Conclusion.Success)
                {
                    if (!ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.NES).Any())
                        AddDefaultsToSelectedGames(NesApplication.defaultNesGames, ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.NES));
                    if (!ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.Famicom).Any())
                        AddDefaultsToSelectedGames(NesApplication.defaultFamicomGames, ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.Famicom));
                    if (!ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SNES_EUR).Any())
                        AddDefaultsToSelectedGames(NesApplication.defaultSnesGames, ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SNES_EUR));
                    if (!ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SNES_USA).Any())
                        AddDefaultsToSelectedGames(NesApplication.defaultSnesGames, ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SNES_USA));
                    if (!ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SuperFamicom).Any())
                        AddDefaultsToSelectedGames(NesApplication.defaultSuperFamicomGames, ConfigIni.Instance.SelectedOriginalGamesForConsole(hakchi.ConsoleType.SuperFamicom));
                }
            }
            LoadGames();
            timerCalculateGames.Enabled = true;
        }

        private void AddDefaultsToSelectedGames(NesDefaultGame[] games, ICollection<string> selectedGames)
        {
            foreach (NesDefaultGame game in games)
                if (!selectedGames.Contains(game.Code))
                    selectedGames.Add(game.Code);
        }

        private void resetOriginalGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.Default30games, string.Format(Resources.ResetOriginalGamesQ, GetConsoleTypeName(ConfigIni.Instance.ConsoleType)), Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                ResetOriginalGamesForCurrentSystem();
            }
        }

        private void enableAutofireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.AutofireHack = enableAutofireToolStripMenuItem.Checked;
            if (ConfigIni.Instance.AutofireHack)
                Tasks.MessageForm.Show(enableAutofireToolStripMenuItem.Text, Resources.AutofireHelp1);
        }

        private void useXYOnClassicControllerAsAutofireABToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.AutofireXYHack = useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked;
        }

        private void globalCommandLineArgumentsexpertsOnluToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var cmdLineType = (ConfigIni.ExtraCmdLineTypes)byte.Parse(menuItem.Tag.ToString());
            using (var form = new StringInputForm())
            {
                form.Text = Resources.ExtraArgsTitle + " (" + menuItem.Text + ")";
                form.labelComments.Text = Resources.ExtraArgsInfo;
                form.textBox.Text = ConfigIni.Instance.ExtraCommandLineArguments[cmdLineType];
                if (form.ShowDialog() == DialogResult.OK)
                    ConfigIni.Instance.ExtraCommandLineArguments[cmdLineType] = form.textBox.Text;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try // wrap upgrade check in an exception check, to avoid past mistakes
            {
                AutoUpdater.Start(UPDATE_XML_URL);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("There was an error running the auto-updater: " + ex.Message + "\r\n" + ex.StackTrace);
                Tasks.ErrorForm.Show(Resources.Error, "There was an error running the auto-updater: " + ex.Message, ex.StackTrace);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // centralized upgrade actions system
            new Upgrade(this).Run();

            // welcome message, only run for new new users
            if (ConfigIni.Instance.RunCount++ == 0)
            {
                Tasks.MessageForm.Show(Resources.Hello, Resources.FirstRun, Resources.Nintendo_NES_icon);
            }

            // nothing else will call this at the moment, so need to do it
            SyncConsoleType();

            // enable timers
            timerConnectionCheck.Enabled = true;
            timerCalculateGames.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Closing main form");
            SaveConfig();
            FtpServer.Stop();
            hakchi.Shutdown();
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

        private void enableSFROMToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SfromToolWrapper.IsInstalled)
            {
                ConfigIni.Instance.UseSFROMTool = enableSFROMToolToolStripMenuItem.Checked;
                usePCMPatchWhenAvailableToolStripMenuItem.Enabled = enableSFROMToolToolStripMenuItem.Checked;
            }
            else
            {
                ConfigIni.Instance.UseSFROMTool = enableSFROMToolToolStripMenuItem.Checked = false;
                usePCMPatchWhenAvailableToolStripMenuItem.Enabled = false;

                if (Tasks.MessageForm.Show(Resources.SfromTool, string.Format(Resources.DownloadSfromTool, Program.BaseDirectoryExternal), Resources.sign_globe, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                {
                    Process.Start(SFROM_TOOL_URL);
                }
            }

            sFROMToolToolStripMenuItem1.Enabled = ConfigIni.Instance.UseSFROMTool && SfromToolWrapper.IsInstalled;
        }

        private void usePCMPatchWhenAvailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.UsePCMPatch = usePCMPatchWhenAvailableToolStripMenuItem.Checked;
        }

        private void compressGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.Compress = compressGamesToolStripMenuItem.Checked;
        }

        private void compressBoxArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.CompressCover = compressBoxArtToolStripMenuItem.Checked;
        }

        private void centerBoxArtThumbnailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.CenterThumbnail = centerBoxArtThumbnailToolStripMenuItem.Checked;
        }

        private void separateGamesForMultibootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.SeparateGameStorage = separateGamesForMultibootToolStripMenuItem.Checked;
            MemoryStats.DebugDisplay();
            timerCalculateGames.Enabled = true;
        }

        private void disableHakchi2PopupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.DisablePopups = disableHakchi2PopupsToolStripMenuItem.Checked;
        }

        private void useLinkedSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.SyncLinked = useLinkedSyncToolStripMenuItem.Checked;
        }

        private void enableUSBHostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.UsbHost = enableUSBHostToolStripMenuItem.Checked;
        }

        private void devForceSshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.ForceSSHTransfers = devForceSshToolStripMenuItem.Checked;
        }

        private void uploadTotmpforTestingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.UploadToTmp = uploadTotmpforTestingToolStripMenuItem.Checked;
        }

        private void pagesModefoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == customToolStripMenuItem && customToolStripMenuItem.Checked)
            {
                openFoldersManager();
                return;
            }

            ConfigIni.Instance.FoldersMode = (NesMenuCollection.SplitStyle)byte.Parse((sender as ToolStripMenuItem).Tag.ToString());
            disablePagefoldersToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 0;
            automaticToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 2;
            automaticOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 3;
            pagesToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 4;
            pagesOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 5;
            foldersToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 6;
            foldersOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 7;
            foldersSplitByFirstLetterToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 8;
            foldersSplitByFirstLetterOriginalToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 9;
            customToolStripMenuItem.Checked = (byte)ConfigIni.Instance.FoldersMode == 99;
        }

        private void timerConnectionCheck_Tick(object sender, EventArgs e)
        {
            toolStripStatusConnectionIcon.Image = hakchi.Connected ? Resources.green : Resources.red;
            toolStripStatusConnectionIcon.ToolTipText = hakchi.Connected ? "Online" : "Offline";
        }

        private void saveSettingsToNESMiniNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == Tasks.MessageForm.Button.No) return;
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    hakchi.SyncConfig(ConfigIni.GetConfigDictionary(), true);
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
            }
        }

        private void saveStateManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == Tasks.MessageForm.Button.No) return;
            var gameNames = new Dictionary<string, string>();
            foreach (var game in NesApplication.AllDefaultGames)
                gameNames[game.Code] = game.Name;
            foreach (ListViewItem item in listViewGames.Items)
            {
                if (item.Tag is NesApplication)
                    gameNames[(item.Tag as NesApplication).Code] = (item.Tag as NesApplication).Name;
            }
            using (var form = new SaveStateManager(gameNames))
            {
                form.ShowDialog();
            }
        }

        private bool changeFTPServerState()
        {
            if (FTPToolStripMenuItem.Enabled && FTPToolStripMenuItem.Checked && hakchi.Shell.IsOnline)
            {
                try
                {
                    var ftpThread = new Thread(delegate ()
                    {
                        try
                        {
                            (FtpServer.FileSystemHandler as mooftpserv.NesMiniFileSystemHandler).UpdateShell(hakchi.Shell);
                            FtpServer.Run();
                        }
                        catch (ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            try
                            {
                                FtpServer.Stop();
                            }
                            catch { }
                            Debug.WriteLine(ex.Message + ex.StackTrace);
                            Invoke(new Action(delegate ()
                            {
                                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
                                ConfigIni.Instance.FtpServer = openFTPInExplorerToolStripMenuItem.Enabled = FTPToolStripMenuItem.Checked = false;
                            }));
                        }
                    });
                    ftpThread.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
                }
            }
            else
            {
                FtpServer.Stop();
            }
            return false;
        }

        private void FTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.FtpServer = FTPToolStripMenuItem.Checked;
            openFTPInExplorerToolStripMenuItem.Enabled = changeFTPServerState();
        }

        private void shellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigIni.Instance.TelnetServer = shellToolStripMenuItem.Checked;
                hakchi.Shell.ShellEnabled = shellToolStripMenuItem.Checked;
                openTelnetToolStripMenuItem.Enabled = shellToolStripMenuItem.Checked && hakchi.Shell.IsOnline;
            }
            catch (Exception ex)
            {
                ConfigIni.Instance.TelnetServer =
                    openTelnetToolStripMenuItem.Enabled =
                    shellToolStripMenuItem.Checked = false;
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
            }
        }

        private void openFTPInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ip, port;
            if (hakchi.Shell is INetworkShell)
            {
                ip = (hakchi.Shell as INetworkShell).IPAddress;
                port = "21";
            }
            else
            {
                ip = "127.0.0.1";
                port = FtpServer.LocalPort.ToString();
            }

            try
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = String.Format(ConfigIni.Instance.FtpCommand, "root", "clover", ip, port),
                        Arguments = String.Format(ConfigIni.Instance.FtpArguments, "root", "clover", ip, port)
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
            }
        }

        private void openTelnetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ip, port;
            if (hakchi.Shell is INetworkShell)
            {
                ip = (hakchi.Shell as INetworkShell).IPAddress;
                port = hakchi.Shell.ShellPort.ToString();
            }
            else
            {
                ip = "127.0.0.1";
                port = "1023";
            }

            try
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = String.Format(ConfigIni.Instance.TelnetCommand, ip, port),
                        Arguments = String.Format(ConfigIni.Instance.TelnetArguments, ip, port)
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.MessageForm.Show(Resources.Error, Resources.NoTelnet, Resources.sign_error);
            }
        }

        private void takeScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == Tasks.MessageForm.Button.No) return;
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    Program.FormContext.AddForm(new ScreenshotForm());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
            }
        }

        private void checkBoxCompressed_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var selected = GetSelectedGame();
                if (selected != null)
                {
                    checkBoxCompressed.Enabled = false;
                    if (checkBoxCompressed.Checked)
                        selected.Compress();
                    else
                        selected.Decompress();
                    selected.Save();
                    ShowSelected();
                    timerCalculateGames.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, ex.Message, ex.StackTrace);
            }
        }

        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sel = listViewGames.SelectedItems;
            if (sel.Count != 1) return;

            try
            {
                string path = (sel[0].Tag as NesApplication).BasePath;
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

        private bool groupTaskWithSelected(Tasks.GameTask task, Tasks.Tasker.TaskFunc taskFunc)
        {
            SaveSelectedGames();
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                task.Games.AddRange(listViewGames.SelectedItems.Cast<ListViewItem>()
                        .Where(item => item.Tag is NesApplication && !(item.Tag as NesApplication).IsOriginalGame)
                        .Select(item => item.Tag as NesApplication));
                if (task.Games.Count > 0)
                {
                    tasker.AddTask(taskFunc);
                    var conclusion = tasker.Start();
                    ShowSelected();
                    timerCalculateGames.Enabled = true;
                    return conclusion == Tasker.Conclusion.Success;
                }
            }
            return false;
        }

        private void scanForNewBoxArtForSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NesApplication.CachedCoverFiles = null;
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.ScanCovers))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
                LoadGames(false);
        }

        private void downloadBoxArtForSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.DownloadCovers))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
                LoadGames(false);
        }

        private void deleteSelectedGamesBoxArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.DeleteCovers))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            if (ConfigIni.Instance.ShowGamesWithoutCoverArt)
                LoadGames(false);
        }

        private void compressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.CompressGames))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
        }

        private void decompressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.DecompressGames))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
        }

        private void deleteSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.DeleteSelectedGamesQ, Resources.sign_delete, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                var task = new Tasks.GameTask();
                if (groupTaskWithSelected(task, task.DeleteGames))
                {
                    listViewGames.BeginUpdate();
                    foreach (ListViewItem item in listViewGames.SelectedItems)
                        if (item.Tag is NesApplication && !(item.Tag as NesApplication).IsOriginalGame)
                            listViewGames.Items.Remove(item);
                    listViewGames.EndUpdate();
                    ShowSelected();
                    RecalculateSelectedGames();
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
                else
                    LoadGames();
            }
        }

        private void editROMHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if (selected is SnesGame && !(selected as SnesGame).IsOriginalGame)
            {
                SnesGame game = selected as SnesGame;
                if (ConfigIni.Instance.UseSFROMTool && SfromToolWrapper.IsInstalled)
                {
                    bool wasCompressed = game.DecompressPossible().Length > 0;
                    if (wasCompressed)
                        game.Decompress();
                    SfromToolWrapper.EditSFROM(game.GameFilePath);
                    if (wasCompressed)
                        game.Compress();
                }
                else
                {
                    new SnesPresetEditor(game).ShowDialog();
                }
                ShowSelected();
            }
        }

        private void resetROMHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.ResetROMHeaderSelectedGamesQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                var task = new Tasks.GameTask();
                if (groupTaskWithSelected(task, task.ResetROMHeaders))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void repairGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.RepairSelectedGamesQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                var task = new Tasks.GameTask();
                if (groupTaskWithSelected(task, task.RepairGames))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void originalGamesPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.Checked) return;
            SaveSelectedGames();

            OriginalGamesPosition newPosition = (OriginalGamesPosition)byte.Parse(menuItem.Tag.ToString());
            bool reload = newPosition == OriginalGamesPosition.Hidden || ConfigIni.Instance.OriginalGamesPosition == OriginalGamesPosition.Hidden;

            ConfigIni.Instance.OriginalGamesPosition = newPosition;
            positionAtTheTopToolStripMenuItem.Checked = newPosition == OriginalGamesPosition.AtTop;
            positionAtTheBottomToolStripMenuItem.Checked = newPosition == OriginalGamesPosition.AtBottom;
            positionSortedToolStripMenuItem.Checked = newPosition == OriginalGamesPosition.Sorted;
            positionHiddenToolStripMenuItem.Checked = newPosition == OriginalGamesPosition.Hidden;
            LoadGames(reload);
        }

        private void sortByToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.Checked) return;
            SaveSelectedGames();

            GamesSorting newSorting = (GamesSorting)byte.Parse(menuItem.Tag.ToString());
            ConfigIni.Instance.GamesSorting = newSorting;
            nameToolStripMenuItem.Checked = newSorting == GamesSorting.Name;
            coreToolStripMenuItem.Checked = newSorting == GamesSorting.Core;
            systemToolStripMenuItem.Checked = newSorting == GamesSorting.System;
            LoadGames(false);
        }

        private void showGamesWithoutBoxArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.ShowGamesWithoutCoverArt = showGamesWithoutBoxArtToolStripMenuItem.Checked;
            SaveSelectedGames();
            LoadGames(false);
        }

        private void openFoldersManager()
        {
            SaveSelectedGames();
            using (var tasker = new Tasker(this))
            {
                var task = new Tasks.SyncTask();
                task.Games.AddRange(listViewGames.CheckedItems.OfType<ListViewItem>()
                    .Where(item => item.Tag is NesApplication)
                    .Select(item => item.Tag as NesApplication));
                task.ShowFoldersManager(tasker, task.Games);
            }
        }

        private void changeBootImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    using (OpenFileDialog ofdPng = new OpenFileDialog())
                    {
                        ofdPng.Filter = "Image files|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
                        if (ofdPng.ShowDialog(this) != DialogResult.OK) return;

                        string imageFile = ofdPng.FileName;
                        using (Image image = Image.FromFile(imageFile))
                        {
                            if (Path.GetExtension(imageFile) != ".png" || image.Height != 720 || image.Width != 1280)
                            {
                                var outImage = Shared.ResizeImage(image, PixelFormat.Format24bppRgb, null, 1280, 720, true, false, true, true);
                                imageFile = Shared.PathCombine(Path.GetTempPath(), "hakchi-temp", "tempBootImage.png");
                                try
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(imageFile));
                                    File.Delete(imageFile);
                                }
                                catch { }
                                outImage.Save(imageFile, ImageFormat.Png);
                            }
                        }

                        hakchi.Shell.Execute("hakchi unset cfg_boot_logo; cat > \"$(hakchi get rootfs)/etc/boot.png\"", File.OpenRead(imageFile));
                        bool usbHost = hakchi.Shell.ExecuteSimple("if [ -d /media/hakchi/ ]; then echo 1; else echo 0; fi;").Equals("1");
                        if (usbHost)
                        {
                            hakchi.Shell.Execute("cat > \"/media/hakchi/boot.png\"", File.OpenRead(imageFile));
                        }

                        if (!ConfigIni.Instance.DisablePopups)
                            Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, "Error changing boot image: " + ex.Message, ex.StackTrace);
            }
        }

        private void disableBootImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    var assembly = GetType().Assembly;

                    hakchi.Shell.Execute("hakchi unset cfg_boot_logo; cat > \"$(hakchi get rootfs)/etc/boot.png\"", File.OpenRead(Shared.PathCombine(Program.BaseDirectoryInternal, "data", "blankBoot.png")));
                    bool usbHost = hakchi.Shell.ExecuteSimple("if [ -d /media/hakchi/ ]; then echo 1; else echo 0; fi;").Equals("1");
                    if (usbHost)
                    {
                        hakchi.Shell.Execute("cat > \"/media/hakchi/boot.png\"", File.OpenRead(Shared.PathCombine(Program.BaseDirectoryInternal, "data", "blankBoot.png")));
                    }

                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, "Error disabling boot image: " + ex.Message, ex.StackTrace);
            }
        }

        private void resetDefaultBootImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    hakchi.Shell.ExecuteSimple("hakchi unset cfg_boot_logo; rm \"$(hakchi get rootfs)/etc/boot.png\"");
                    hakchi.Shell.ExecuteSimple("rm \"/media/hakchi/boot.png\"");

                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(null, "Error resetting boot image: " + ex.Message, ex.StackTrace);
            }
        }

        private void selectEmulationCoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count == 0)
                return;

            SaveSelectedGames();
            using (SelectCoreDialog selectCoreDialog = new SelectCoreDialog())
            {
                foreach (ListViewItem item in listViewGames.SelectedItems)
                {
                    if (!(item.Tag as NesApplication).IsOriginalGame)
                    {
                        selectCoreDialog.Games.Add(item.Tag as NesApplication);
                        item.Selected = false;
                    }
                }
                if (selectCoreDialog.Games.Count == 0)
                    return;

                if (selectCoreDialog.ShowDialog(this) == DialogResult.OK)
                    LoadGames();
            }

        }

        private void addCustomAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (NewCustomGameForm customGameForm = new NewCustomGameForm())
            {
                if (customGameForm.ShowDialog(this) == DialogResult.OK)
                {
                    var newGroup = listViewGames.Groups.OfType<ListViewGroup>().Where(group => group.Header == Resources.ListCategoryNew).First();
                    var item = new ListViewItem(customGameForm.NewApp.Name);
                    item.Group = newGroup;
                    item.Tag = customGameForm.NewApp;
                    item.Selected = true;
                    item.Checked = true;

                    foreach(ListViewItem i in listViewGames.Items)
                        i.Selected = false;
                    listViewGames.Items.Add(item);
                }
            }
        }

        private void prepareArtDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var selectSystemDialog = new SelectSystemDialog())
            {
                selectSystemDialog.ShowDialog();
            }
        }

        private void rebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hakchi.Shell.IsOnline)
            {
                try
                {
                    hakchi.Shell.ExecuteSimple("sync; umount -ar; reboot -f", 100);
                }
                catch { }
            }
        }

        private void leftmostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leftmostToolStripMenuItem.Checked = true;
            rightmostToolStripMenuItem.Checked = false;
            ConfigIni.Instance.BackFolderPosition = NesMenuFolder.Priority.LeftBack;
        }

        private void rightmostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leftmostToolStripMenuItem.Checked = false;
            rightmostToolStripMenuItem.Checked = true;
            ConfigIni.Instance.BackFolderPosition = NesMenuFolder.Priority.Back;
        }

        private void syncStructureForAllGamesCollectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(this, Resources.SaveSettings, Resources.SaveStructureQ, Resources.sign_sync, new MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }) == Tasks.MessageForm.Button.Yes)
            {
                ConfigIni.Instance.SyncGamesCollectionsStructureSettings();
            }
        }
    }
}
