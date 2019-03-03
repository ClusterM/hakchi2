using com.clusterrr.hakchi_gui.Properties;
using AutoUpdaterDotNET;
using SharpCompress.Archives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using com.clusterrr.hakchi_gui.Tasks;
using com.clusterrr.hakchi_gui.ModHub.Repository;
using com.clusterrr.hakchi_gui.ModHub;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The URL for the update metadata XML file
        /// </summary>
#if DEBUG
        private static readonly string UPDATE_XML_URL = "https://teamshinkansen.github.io/xml/updates/update-debug.xml";
#else
        private static readonly string UPDATE_XML_URL = "https://teamshinkansen.github.io/xml/updates/update-release.xml";
#endif
        private static readonly string MOTD_URL = "https://teamshinkansen.github.io/motd.md";
        private static readonly string SFROM_TOOL_URL = "http://darkakuma.z-net.us/p/sfromtool.html";
        private static readonly string MotdFilename = Path.Combine(Program.BaseDirectoryExternal, "config", "motd.md");

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
                case hakchi.ConsoleType.ShonenJump: return Resources.consoleTypeShonenJump;
                case hakchi.ConsoleType.Unknown: return Resources.Unknown;
            }
            return string.Empty;
        }

        public static bool? DownloadCover;
        public const int MaxGamesPerFolder = 50;
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
        }

        private void FormInitialize()
        {
            try
            {
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
                extensions += Resources.AllFiles + "|*.*|" + Resources.ArchiveFiles + "|*.zip;*.7z;*.rar|";
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
                Tasks.ErrorForm.Show(this, ex, Resources.CriticalError);
            }
        }

        private void ShowMOTD(string message = null)
        {
            try
            {
                if (message == null)
                {
                    message = File.ReadAllText(Path.Combine(Program.BaseDirectoryExternal, "config", "motd.md"));
                }
                new Motd(message).ShowDialog(this);
            }
            catch
            {
                Trace.WriteLine("Could not show \"Message of the day\"");
            }
        }

        private void UpdateMOTD()
        {
            try
            {
                if (File.Exists(MotdFilename))
                {
                    if (new FileInfo(MotdFilename).LastWriteTime.AddHours(6) > DateTime.Now)
                    {
                        return;
                    }
                }

                var client = new WebClient();

                Trace.WriteLine("Downloading motd file, URL: " + MOTD_URL);
                string motd = client.DownloadString(MOTD_URL);
                if (!string.IsNullOrEmpty(motd))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(MotdFilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(MotdFilename));
                    File.WriteAllText(MotdFilename, motd);
                }
                Match m = Regex.Match(motd, "\\<\\!\\-\\-\\-\\s([^\\s]+)\\s\\-\\-\\>");
                if (m.Success)
                {
                    Trace.WriteLine("Motd timestamp: " + m.Groups[1].Value);
                    DateTime date = DateTime.Parse(m.Groups[1].Value);
                    if (date.CompareTo(ConfigIni.Instance.LastMOTD) > 0)
                    {
                        ConfigIni.Instance.LastMOTD = date;
                        Invoke(new Action<string>(ShowMOTD), new object[] { motd });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try // wrap upgrade check in an exception check, to avoid past mistakes
            {
                AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
                AutoUpdater.RemindLaterAt = 7;
                AutoUpdater.ShowRemindLaterButton = true;
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.Start(UPDATE_XML_URL);
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // centralized upgrade actions system
            new Upgrade(this).Run();
            populateRepos();

            // welcome message, only run for new new users
            if (ConfigIni.Instance.RunCount++ == 0)
            {
                Tasks.MessageForm.Show(Resources.Hello, Resources.FirstRun, Resources.Nintendo_NES_icon);
            }

            // nothing else will call this at the moment, so need to do it
            SyncConsoleSettings(true);
            SyncConsoleType(true);

            // setup system shell
            hakchi.OnConnected += Shell_OnConnected;
            hakchi.OnDisconnected += Shell_OnDisconnected;
            hakchi.Initialize();

            // enable timers
            timerConnectionCheck.Enabled = true;
            timerCalculateGames.Enabled = true;

            // defer but run message of the day
            new Thread(UpdateMOTD).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hakchi.MinimalMemboot)
            {
                var result = MessageForm.Show(this, Resources.Warning, Resources.RecoveryModeCloseWarning, Resources.sign_life_buoy, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No, MessageForm.Button.Cancel }, MessageForm.DefaultButton.Button1);
                if (result == MessageForm.Button.Yes)
                {
                    using (var tasker = new Tasker(this))
                    {
                        tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                        tasker.SetStatusImage(Resources.sign_sync);
                        tasker.SetTitle(Resources.Rebooting);
                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.Memboot).Tasks);
                        tasker.AddTask(Tasker.Wait(1000, Resources.WaitingForDevice));
                        tasker.Start();
                    }
                }
                else if (result == MessageForm.Button.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Trace.WriteLine("Closing main form");
            SaveConfig();
            hakchi.Shutdown();
        }

        private void SetWindowTitle()
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action(SetWindowTitle));
                return;
            }

            string title = $"hakchi CE v{Shared.AppDisplayVersion}";

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

        private void SetupShellMenuItems(ISystemShell caller)
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action<ISystemShell>(SetupShellMenuItems), new object[] { caller });
                return;
            }

            if (caller == null || caller is UnknownShell)
            {
                openFTPInExplorerToolStripMenuItem.Enabled = false;
                openTelnetToolStripMenuItem.Enabled = false;
            }
            else if (caller is INetworkShell)
            {
                openFTPInExplorerToolStripMenuItem.Enabled = true;
                openTelnetToolStripMenuItem.Enabled = true;
            }
            else // caller is ClovershellConnection
            {
                openFTPInExplorerToolStripMenuItem.Enabled = false;
                openTelnetToolStripMenuItem.Enabled = true;
            }
        }

        void Shell_OnConnected(ISystemShell caller)
        {
            try
            {
                SyncConsoleSettings();

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
                    SyncConsoleType();

                    if (hakchi.SystemEligibleForRootfsUpdate())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedScripts, Resources.SystemEligibleForRootfsUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            Invoke(new Action(timerUpdate.Start));
                            return;
                        }
                    }
                    UpdateLocalCache();
                }
                else
                {
                    if (hakchi.SystemRequiresReflash())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedKernel, Resources.SystemRequiresReflash, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            Invoke(new Action(timerUpdate.Start));
                            return;
                        }
                    }
                    else if (hakchi.SystemRequiresRootfsUpdate())
                    {
                        if (Tasks.MessageForm.Show(this, Resources.OutdatedScripts, Resources.SystemRequiresRootfsUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                        {
                            Invoke(new Action(timerUpdate.Start));
                            return;
                        }
                    }

                    // show warning message that any interaction is ill-advised
                    Tasks.MessageForm.Show(this, Resources.Warning, Resources.PleaseUpdate, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            if (InstallHakchi())
                Tasks.MessageForm.Show(this, Resources.UpdateComplete, Resources.DoneYouCanUpload, Resources.sign_check, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK }, Tasks.MessageForm.DefaultButton.Button1);
        }

        void Shell_OnDisconnected()
        {
            SyncConsoleSettings();
            SyncConsoleType();
        }

        private static bool lastConnected = false;
        private void SyncConsoleSettings(bool force = false)
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SyncConsoleSettings), new object[] { force });
                return;
            }

            if (lastConnected == hakchi.Connected && !force)
            {
                return;
            }
            lastConnected = hakchi.Connected;

            // setup ui items
            SetupShellMenuItems(hakchi.Shell);
            SetWindowTitle();
            gamesConsoleComboBox.Enabled = !hakchi.Connected || ConfigIni.Instance.SeparateGameStorage;

            // developer settings
            devForceSshToolStripMenuItem.Checked = ConfigIni.Instance.ForceSSHTransfers;
            uploadTotmpforTestingToolStripMenuItem.Checked = ConfigIni.Instance.UploadToTmp;
            forceNetworkMembootsToolStripMenuItem.Checked = ConfigIni.Instance.ForceNetwork;
            forceClovershellMembootsToolStripMenuItem.Checked = ConfigIni.Instance.ForceClovershell;
            developerToolsToolStripMenuItem.Visible =
                devForceSshToolStripMenuItem.Checked ||
                uploadTotmpforTestingToolStripMenuItem.Checked ||
                forceNetworkMembootsToolStripMenuItem.Checked ||
                forceClovershellMembootsToolStripMenuItem.Checked;

            // console settings
            epilepsyProtectionToolStripMenuItem.Checked = ConfigIni.Instance.AntiArmetLevel > 0;
            selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.Instance.ResetHack;
            enableAutofireToolStripMenuItem.Checked = ConfigIni.Instance.AutofireHack;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked = ConfigIni.Instance.AutofireXYHack;
            upABStartOnSecondControllerToolStripMenuItem.Enabled = true;
            upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.Instance.FcStart && upABStartOnSecondControllerToolStripMenuItem.Enabled;

            // enable/disable options based on console being connected and can interact or not
            epilepsyProtectionToolStripMenuItem.Enabled =
                cloverconHackToolStripMenuItem.Enabled =
                globalCommandLineArgumentsexpertsOnluToolStripMenuItem.Enabled =
                saveSettingsToNESMiniNowToolStripMenuItem.Enabled = (hakchi.Connected && hakchi.CanInteract);

            // just to be sure
            timerCalculateGames.Enabled = true;
        }

        private static hakchi.ConsoleType lastConsoleType = hakchi.ConsoleType.Unknown;
        private void SyncConsoleType(bool force = false)
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SyncConsoleType), new object[] { force });
                return;
            }

            if (lastConsoleType == ConfigIni.Instance.ConsoleType && !force)
            {
                return;
            }
            lastConsoleType = ConfigIni.Instance.ConsoleType;

            // games collection combo box
            for (int i = 0; i < gamesConsoleComboBox.Items.Count; ++i)
            {
                if (GetConsoleTypeName(ConfigIni.Instance.ConsoleType) == gamesConsoleComboBox.Items[i] as string)
                {
                    gamesConsoleComboBox.SelectedIndex = i;
                    break;
                }
            }

            // more settings
            convertSNESROMSToSFROMToolStripMenuItem.Checked = ConfigIni.Instance.ConvertToSFROM;
            separateGamesStorageToolStripMenuItem.Checked = ConfigIni.Instance.SeparateGameLocalStorage;
            compressGamesToolStripMenuItem.Checked = ConfigIni.Instance.Compress;
            compressBoxArtToolStripMenuItem.Checked = ConfigIni.Instance.CompressCover;
            centerBoxArtThumbnailToolStripMenuItem.Checked = ConfigIni.Instance.CenterThumbnail;
            separateGamesForMultibootToolStripMenuItem.Checked = ConfigIni.Instance.SeparateGameStorage;
            disableHakchi2PopupsToolStripMenuItem.Checked = ConfigIni.Instance.DisablePopups;
            useLinkedSyncToolStripMenuItem.Checked = ConfigIni.Instance.SyncLinked;
            alwaysCopyOriginalGamesToolStripMenuItem.Checked = ConfigIni.Instance.AlwaysCopyOriginalGames;

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

        private void UpdateLocalCache()
        {
            if (Disposing) return;
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateLocalCache));
                return;
            }

            using (var tasker = new Tasks.Tasker(this))
            {
                var task = new Tasks.GameCacheTask();
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                tasker.AddTask(task.UpdateLocal);
                if (tasker.Start() == Tasks.Tasker.Conclusion.Success)
                {
                    Trace.WriteLine("Done refreshing local original games cache.");
                    if (ConfigIni.Instance.AlwaysCopyOriginalGames && task.LoadedGames > 0)
                        LoadGames();
                }
            }
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

        private void LoadPresets()
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

        private void LoadLanguages()
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
                    Trace.WriteLine($"There is no flag for \"{country}\"");
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Click += delegate (object sender, EventArgs e)
                    {
                        ConfigIni.Instance.Language = langCodes[language];
                        SaveConfig();
                        lastConsoleType = hakchi.ConsoleType.Unknown;

                        var ci = new CultureInfo(langCodes[language]);
                        Thread.CurrentThread.CurrentCulture = ci;
                        Thread.CurrentThread.CurrentUICulture = ci;
                        this.Hide();
                        this.Controls.Clear();
                        this.InitializeComponent();
                        this.FormInitialize();
                        this.SyncConsoleSettings(true);
                        this.SyncConsoleType(true);
                        this.Show();
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
            {
                newItem.Checked = true;
            }
            folderImagesSetToolStripMenuItem.DropDownItems.Add(newItem);

            var folderImagesDirs = Directory.GetDirectories(Path.Combine(Program.BaseDirectoryExternal, "folder_images"), "*.*", SearchOption.TopDirectoryOnly);
            if (folderImagesDirs.Any())
            {
                folderImagesSetToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                foreach (var dir in folderImagesDirs)
                {
                    string name = Path.GetFileName(dir);
                    newItem = new ToolStripMenuItem(name, null, delegate (object sender, EventArgs e)
                    {
                        ConfigIni.Instance.FolderImagesSet = name;

                        folderImagesSetToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().ToList().ForEach(item => item.Checked = false);
                        (sender as ToolStripMenuItem).Checked = true;
                    });
                    if (ConfigIni.Instance.FolderImagesSet == name)
                    {
                        newItem.Checked = true;
                    }
                    folderImagesSetToolStripMenuItem.DropDownItems.Add(newItem);
                }
            }
        }

        private void SaveSelectedGames()
        {
            Trace.WriteLine("Saving selected games");
            List<string> selected = new List<string>();
            HashSet<string> original = new HashSet<string>();
            original.UnionWith(ConfigIni.Instance.OriginalGames);

            foreach (ListViewItem item in listViewGames.Items)
            {
                if (!(item.Tag is NesApplication))
                    continue;

                NesApplication game = item.Tag as NesApplication;
                if (item.Checked)
                {
                    if (game.IsOriginalGame) original.Add(game.Code); else selected.Add(game.Code);
                }
                else
                {
                    if (game.IsOriginalGame) original.Remove(game.Code);
                }
            }

            ConfigIni.Instance.SelectedGames.Clear();
            ConfigIni.Instance.SelectedGames.AddRange(selected);

            ConfigIni.Instance.OriginalGames.Clear();
            ConfigIni.Instance.OriginalGames.AddRange(original);
        }

        private void SaveGameItem(ListViewItem item)
        {
            try
            {
                NesApplication game = item.Tag as NesApplication;
                if (game != null && !game.IsDeleting)
                {
                    if (game.Save()) // maybe type was changed? need to reload games
                    {
                        item.Tag = NesApplication.FromDirectory(game.BasePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void SaveConfig()
        {
            SaveSelectedGames();
            ConfigIni.Save();
            listViewGames.Items.Cast<ListViewItem>().ToList().ForEach(item => SaveGameItem(item));
        }

        private void AddPreset(object sender, EventArgs e)
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

        private void SelectAll(bool selected = true)
        {
            listViewGames.SuspendLayout();
            listViewGames.ItemSelectionChanged -= listViewGames_ItemSelectionChanged;
            try
            {
                for (int i = 0; i < listViewGames.Items.Count; ++i)
                {
                    if (!selected && listViewGames.Items[i].Selected)
                        SaveGameItem(listViewGames.Items[i]);
                    listViewGames.Items[i].Selected = selected;
                }
            }
            finally
            {
                listViewGames.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(listViewGames_ItemSelectionChanged);
                listViewGames.ResumeLayout();
            }
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

        private int lastListViewGamesSelectionCount = 0;
        private void listViewGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected && e.Item != null)
                SaveGameItem(e.Item);

            int c = listViewGames.SelectedItems.Count, last = lastListViewGamesSelectionCount;
            lastListViewGamesSelectionCount = c;
            if (c == 1)
            {
                NesApplication game = e.Item.Tag as NesApplication;
                if (game == null || game.IsDeleting)
                {
                    c = 0;
                }
                else
                {
                    explorerToolStripMenuItem.Enabled =
                        downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                        scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                        deleteSelectedGamesBoxArtToolStripMenuItem.Enabled = 
                        archiveSelectedGamesToolStripMenuItem.Enabled = 
                        true;

                    deleteSelectedGamesToolStripMenuItem.Enabled =
                        repairGamesToolStripMenuItem.Enabled =
                        selectEmulationCoreToolStripMenuItem.Enabled = !game.IsOriginalGame;

                    compressSelectedGamesToolStripMenuItem.Enabled = game.CompressPossible().Count() > 0;
                    decompressSelectedGamesToolStripMenuItem.Enabled = game.DecompressPossible().Count() > 0;

                    sFROMToolToolStripMenuItem1.Enabled =
                        editROMHeaderToolStripMenuItem.Enabled =
                        resetROMHeaderToolStripMenuItem.Enabled =
                            !game.IsOriginalGame && SfromToolWrapper.IsInstalled && game is SnesGame && 
                            (game.GameFilePath ?? "").ToLower().Contains(".sfrom");
                }
            }

            if (c == 0)
            {
                if (last == 0)
                    return;

                explorerToolStripMenuItem.Enabled =
                    downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    scanForNewBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesBoxArtToolStripMenuItem.Enabled =
                    compressSelectedGamesToolStripMenuItem.Enabled =
                    decompressSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesToolStripMenuItem.Enabled =
                    sFROMToolToolStripMenuItem1.Enabled =
                    repairGamesToolStripMenuItem.Enabled =
                    selectEmulationCoreToolStripMenuItem.Enabled = 
                    archiveSelectedGamesToolStripMenuItem.Enabled =
                    false;
            }
            else if (c > 1)
            {
                if (last > 1)
                    return;

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

            timerShowSelected.Enabled = true;
        }

        private void timerShowSelected_Tick(object sender, EventArgs e)
        {
            timerShowSelected.Enabled = false;
            ShowSelected();
        }

        private void listViewGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        private void timerCalculateGames_Tick(object sender, EventArgs e)
        {
            new Thread(RecalculateSelectedGamesThread).Start();
            timerCalculateGames.Enabled = false;
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
                case Keys.F12:
                    if (e.Modifiers == Keys.Control)
                        developerToolsToolStripMenuItem.Visible = true;
                    break;
            }
        }

        private void listViewGames_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                        SelectAll();
                    break;
                case Keys.Escape:
                    if (e.Modifiers == 0)
                        SelectAll(false);
                    break;
                case Keys.N:
                    if (e.Modifiers == Keys.Control)
                        SelectAll(false);
                    break;
                case Keys.Delete:
                    if ((listViewGames.SelectedItems.Count > 1) ||
                        (listViewGames.SelectedItems.Count == 1 &&
                        listViewGames.SelectedItems[0].Tag is NesApplication &&
                        (!(listViewGames.SelectedItems[0].Tag as NesApplication).IsOriginalGame)))
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
                    usagePercentage = 1.0;
                if (usagePercentage < 0.0)
                    usagePercentage = 0.0;
                toolStripStatusLabelSelected.Text = stats.Count + " " + Resources.GamesSelected;
                toolStripProgressBar.Maximum = int.MaxValue;
                toolStripProgressBar.Value = Convert.ToInt32(usagePercentage * int.MaxValue);
                toolStripStatusLabelSize.ForeColor =
                    (toolStripProgressBar.Value < toolStripProgressBar.Maximum) ?
                    SystemColors.ControlText :
                    Color.Red;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void buttonAddGames_Click(object sender, EventArgs e)
        {
            if (openFileDialogNes.ShowDialog() == DialogResult.OK)
            {
                AddGames(openFileDialogNes.FileNames);
            }
        }


        private void asIsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogNes.ShowDialog() == DialogResult.OK)
            {
                AddGames(openFileDialogNes.FileNames, true);
            }
        }

        private void reloadGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                        SaveConfig();

                        ConfigIni.Instance.ConsoleType = c;
                        SyncConsoleType();
                        return;
                    }
                }
            }

        }

        bool RequirePatchedKernel()
        {
            if (hakchi.Connected) return true; // OK - Shell is online
            var returnVal = Tasks.MessageForm.Show(this, Resources.CustomKernel, Resources.CustomKernelInstalledQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No, Tasks.MessageForm.Button.Cancel }, Tasks.MessageForm.DefaultButton.Button1);
            if (returnVal == MessageForm.Button.Yes)
            {
                return WaitingShellForm.WaitForDevice(this);
            }
            else if (returnVal == MessageForm.Button.No)
            {
                if (Tasks.MessageForm.Show(this, Resources.CustomKernel, Resources.CustomWarning, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
                {
                    if (InstallHakchi())
                    {
                        return WaitingShellForm.WaitForDevice(this);
                    }
                }
            }
            return false;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (listViewGames.CheckedItems.Count == 0)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.SelectAtLeast, Resources.sign_info);
                return;
            }
            if (!RequirePatchedKernel())
            {
                return;
            }
            if (hakchi.MinimalMemboot)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                return;
            }
            if (!hakchi.CanInteract)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                return;
            }
            if (!hakchi.CanSync)
            {
                Tasks.MessageForm.Show(Resources.UploadGames, Resources.CannotProceedCannotSync, Resources.sign_ban);
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
            using (var tasker = new Tasks.Tasker(this, new Tasks.TaskerTaskbar(), new Tasks.TaskerTransferForm()))
            {
                var syncTask = new Tasks.SyncTask();
                syncTask.Games.AddRange(listViewGames.CheckedItems.Cast<ListViewItem>().
                    Where(item => item.Tag is NesApplication).
                    Select(item => item.Tag as NesApplication));
                tasker.AddTask(exportGames ? (Tasks.Tasker.TaskFunc)syncTask.ExportGames : syncTask.UploadGames, 0);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        bool DumpDialog(FileAccess type, string FileName, string FileExt, out string DumpFileName, string DialogFilter = null)
        {
            DumpFileName = null;
            string currentFilter;
            switch (type)
            {
                case FileAccess.Read:
                    openDumpFileDialog.FileName = FileName;
                    openDumpFileDialog.DefaultExt = FileExt;
                    currentFilter = openDumpFileDialog.Filter;

                    if (DialogFilter != null)
                        openDumpFileDialog.Filter = DialogFilter;

                    if (openDumpFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        DumpFileName = openDumpFileDialog.FileName;
                        openDumpFileDialog.Filter = currentFilter;
                        return true;
                    }

                    openDumpFileDialog.Filter = currentFilter;
                    return false;

                case FileAccess.Write:
                    saveDumpFileDialog.FileName = FileName;
                    saveDumpFileDialog.DefaultExt = FileExt;
                    currentFilter = saveDumpFileDialog.Filter;

                    if (DialogFilter != null)
                        saveDumpFileDialog.Filter = DialogFilter;

                    if (saveDumpFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        DumpFileName = saveDumpFileDialog.FileName;
                        saveDumpFileDialog.Filter = currentFilter;
                        return true;
                    }

                    saveDumpFileDialog.Filter = currentFilter;
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
                        if (!DumpDialog(FileAccess.Write, "nandb.hsqs", "hsqs", out dumpFilename, $"{Resources.SystemSoftwareBackup}|*.hsqs"))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpNandB, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.DumpNandC:
                        if (!DumpDialog(FileAccess.Write, "nandc.tar", "tar", out dumpFilename, $"{Resources.UserDataBackup}|*.tar"))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpNandC, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.FlashNandB:
                        if (!DumpDialog(FileAccess.Read, "nandb.hsqs", "hsqs", out dumpFilename, $"{Resources.SystemSoftwareBackup}|*.hsqs"))
                            return false;

                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.FlashNandB, dumpPath: dumpFilename).Tasks);
                        break;

                    case MembootTasks.NandTasks.FlashNandC:
                        if (!DumpDialog(FileAccess.Read, "nandc.tar", "tar", out dumpFilename, $"{Resources.UserDataBackup}|*.tar;*.hsqs"))
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
                tasker.SetStatusImage(reset ? Resources.sign_sync : Resources.sign_keyring);
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

        public void AddGames(IEnumerable<string> files, bool asIs = false, Form parentForm = null)
        {
            using (var tasker = new Tasks.Tasker(parentForm ?? this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                var task = new Tasks.AddGamesTask(listViewGames, files, asIs);
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

        bool Uninstall(bool ignoreBackupKernel = false)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_delete);
                tasker.SetTitle(Resources.UninstallingHakchi);
                tasker.AddTasks(new MembootTasks(
                    MembootTasks.MembootTaskType.UninstallHakchi,
                    ignoreBackupKernel: ignoreBackupKernel
                ).Tasks);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        public bool InstallMods(string[] mods)
        {
            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetTitle(Resources.InstallingMods);
                tasker.SetStatusImage(Resources.sign_brick);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery).Tasks);
                tasker.AddTask(ShellTasks.MountBase);
                tasker.AddTask(ShellTasks.ShowSplashScreen);
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
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery).Tasks);
                tasker.AddTask(ShellTasks.MountBase);
                tasker.AddTask(ShellTasks.ShowSplashScreen);
                tasker.AddTasks(new ModTasks(null, mods).Tasks);
                tasker.AddFinalTask(MembootTasks.BootHakchi);
                return tasker.Start() == Tasker.Conclusion.Success;
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ignoreBackupKernel = Control.ModifierKeys == Keys.Shift;

            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.UninstallQ1, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                if (Uninstall(ignoreBackupKernel))
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
            MembootCustomKernel();
        }

        private void membootRecoveryKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool forceRecoveryReload = false;
            if(hakchi.Shell.IsOnline && hakchi.MinimalMemboot)
            {
                forceRecoveryReload = (Control.ModifierKeys == Keys.Shift) || (Tasks.MessageForm.Show(this, Resources.AlreadyInRecovery, Resources.AlreadyInRecoveryQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes);
            }

            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                tasker.SetStatusImage(Resources.sign_life_buoy);
                tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.MembootRecovery, forceRecoveryReload: forceRecoveryReload).Tasks);
                tasker.AddTask(ShellTasks.ShellCommand("touch /user-recovery.flag"));
                if (tasker.Start() == Tasker.Conclusion.Success)
                    Tasks.MessageForm.Show(Resources.RecoveryKernel, Resources.RecoveryModeMessage, Resources.sign_life_buoy);
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
            bool ignoreBackupKernel = Control.ModifierKeys == Keys.Shift;

            if (Tasks.MessageForm.Show(this, Resources.Warning, Resources.FactoryResetQ, Resources.sign_warning, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) == MessageForm.Button.Yes)
            {
                using (var tasker = new Tasker(this))
                {
                    tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                    tasker.SetStatusImage(Resources.sign_delete);
                    tasker.SetTitle(((ToolStripMenuItem)sender).Text);
                    tasker.AddTasks(new MembootTasks(
                        MembootTasks.MembootTaskType.FactoryReset,
                        ignoreBackupKernel: ignoreBackupKernel
                    ).Tasks);
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
            using (var form = new Hmod.SelectForm(false, true, add))
            {
                form.Text = Resources.SelectModsInstall;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    List<string> hmods = new List<string>();
                    foreach (ListViewItem item in form.listViewHmods.CheckedItems)
                    {
                        hmods.Add(((Hmod.Hmod)item.Tag).RawName);
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
            using (var form = new Hmod.SelectForm(true, false))
            {
                form.Text = Resources.SelectModsUninstall;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    List<string> hmods = new List<string>();
                    foreach (ListViewItem item in form.listViewHmods.CheckedItems)
                    {
                        hmods.Add(((Hmod.Hmod)item.Tag).RawName);
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

        private void openWebsiteLink(Object sender, EventArgs e) => Process.Start((string)((ToolStripMenuItem)sender).Tag);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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
            SaveConfig();
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
        }

        public void ResetOriginalGamesForAllSystems(bool nonDestructiveSync)
        {
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());

                var task = new Tasks.GameTask();
                task.ResetAllOriginalGames = true;
                task.NonDestructiveSync = nonDestructiveSync;
                tasker.AddTask(task.SyncOriginalGames);

                var conclusion = tasker.Start();
                if (conclusion == Tasks.Tasker.Conclusion.Success)
                {
                    foreach (var c in Enum.GetValues(typeof(hakchi.ConsoleType)).Cast<hakchi.ConsoleType>())
                    {
                        if (c != hakchi.ConsoleType.Unknown)
                        {
                            ConfigIni.Instance.SelectedOriginalGamesForConsole(c).Clear();
                            ConfigIni.Instance.SelectedOriginalGamesForConsole(c).AddRange(NesApplication.DefaultGames[c]);
                        }
                    }
                }
            }
            LoadGames();
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

        private void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void dragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;

            if (files.Length == 1)
            {
                var ext = Path.GetExtension(files[0]).ToLower();
                if (ext == ".jpg" || ext == ".png")
                {
                    SetImageForSelectedGame(files[0]);
                    return;
                }
                if (Path.GetFileName(files[0]).ToLower().StartsWith("sfrom_tool") && (ext == ".rar" || ext == ".zip" || ext == ".rar"))
                {
                    using (var extractor = ArchiveFactory.Open(files[0]))
                    {
                        extractor.WriteToDirectory(Path.Combine(Program.BaseDirectoryExternal, "sfrom_tool"), new SharpCompress.Common.ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                    enableSFROMToolToolStripMenuItem.Checked = true;
                    enableSFROMToolToolStripMenuItem_Click(sender, e);

                    if (SfromToolWrapper.IsInstalled)
                    {
                        Tasks.MessageForm.Show(this, Resources.SfromTool, Resources.SfromToolInstalled, Resources.sign_handshake);
                    }
                    return;
                }
            }

            string patchesPath = Path.Combine(Program.BaseDirectoryExternal, "sfrom_tool", "patches");
            if (!Directory.Exists(patchesPath))
                patchesPath = null;

            var filesToAdd = new List<string>();
            var modsToInstall = new List<string>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".hrepo" && File.Exists(file))
                {
                    var hrepo = new HmodReadme(File.ReadAllText(file), true);
                    string name = null;
                    string link = null;

                    if (hrepo.frontMatter.TryGetValue("Name", out name) && (hrepo.frontMatter.TryGetValue("Link", out link)))
                    {
                        var repoList = ConfigIni.Instance.repos.ToList();
                        repoList.Add(new RepositoryInfo(name, link));
                        ConfigIni.Instance.repos = repoList.ToArray();
                        ConfigIni.Save();
                        populateRepos();
                    }
                }
                else if (ext == ".hmod")
                {
                    modsToInstall.Add(file);
                }
                else if (Directory.Exists(file))
                {
                    // nothing for now (too many potential problems)
                }
                else if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                {
                    using (var extractor = ArchiveFactory.Open(file))
                    {
                        bool isMod = false;
                        foreach (var f in extractor.Entries)
                            if (Path.GetExtension(f.Key).ToLower() == ".hmod")
                            {
                                modsToInstall.Add(file);
                                isMod = true;
                                break;
                            }

                        if (!isMod)
                        {
                            filesToAdd.Add(file);
                        }
                    }
                }
                else if (File.Exists(file))
                {
                    if (ext == ".cnp")
                    {
                        if (patchesPath != null)
                            File.Copy(file, Path.Combine(patchesPath, Path.GetFileName(file)));
                    }
                    else
                    {
                        filesToAdd.Add(file);
                    }
                }
            }

            if (modsToInstall.Count > 0)
            {
                installModules(modsToInstall.ToArray());
            }
            if (filesToAdd.Count > 0)
            {
                AddGames(filesToAdd);
            }
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

            if (hakchi.Connected)
            {
                if (!ConfigIni.Instance.SeparateGameStorage && hakchi.DetectedConsoleType != null)
                    ConfigIni.Instance.ConsoleType = hakchi.DetectedConsoleType.Value;
                SyncConsoleType();
                gamesConsoleComboBox.Enabled = !hakchi.Connected || ConfigIni.Instance.SeparateGameStorage;
            }
        }

        private void disableHakchi2PopupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.DisablePopups = disableHakchi2PopupsToolStripMenuItem.Checked;
        }

        private void useLinkedSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.SyncLinked = useLinkedSyncToolStripMenuItem.Checked;
        }

        private void alwaysCopyOriginalGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.AlwaysCopyOriginalGames = alwaysCopyOriginalGamesToolStripMenuItem.Checked;
            SaveSelectedGames();
            LoadGames();
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

        private int timerConnectionAnim = 0;
        private void timerConnectionCheck_Tick(object sender, EventArgs e)
        {
            if (hakchi.Connected)
            {
                toolStripStatusConnectionIcon.Image = Resources.green;
                toolStripStatusConnectionIcon.ToolTipText = "Online";
                if (timerConnectionAnim++ < 4)
                    toolStripStatusLabelShell.Text = "Online";
                else
                    toolStripStatusLabelShell.Text = (hakchi.Shell is INetworkShell) ? "SSH" : "Clovershell";
                if (timerConnectionAnim == 8)
                    timerConnectionAnim = 0;
            }
            else
            {
                toolStripStatusConnectionIcon.Image = Resources.red;
                toolStripStatusConnectionIcon.ToolTipText = "Offline";
                toolStripStatusLabelShell.Text = "Offline";
            }
        }

        private void saveSettingsToNESMiniNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingShellForm.WaitForDevice(this))
                {
                    hakchi.SyncConfig(ConfigIni.GetConfigDictionary(), true);
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void saveStateManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingShellForm.WaitForDevice(this))
                {
                    if (hakchi.MinimalMemboot)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                        return;
                    }
                    if (!hakchi.CanInteract)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                        return;
                    }

                    var gameNames = new Dictionary<string, string>();
                    foreach (var game in NesApplication.AllDefaultGames)
                    {
                        gameNames[game.Key] = game.Value.Name;
                    }
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
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
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
                return;
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
                Trace.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(this, ex);
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
            else if (hakchi.Shell is clovershell.ClovershellConnection)
            {
                (hakchi.Shell as clovershell.ClovershellConnection).ShellEnabled = true;
                ip = "127.0.0.1";
                port = "1023";
            }
            else
            {
                return;
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
                Trace.WriteLine(ex.Message + ex.StackTrace);
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void takeScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingShellForm.WaitForDevice(this))
                {
                    Program.FormContext.AddForm(new ScreenshotForm());
                }
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
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
                Tasks.ErrorForm.Show(this, ex);
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
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private bool groupTaskWithSelected(Tasks.GameTask task, Tasks.Tasker.TaskFunc taskFunc, bool allowOriginalGames = true)
        {
            SaveConfig();
            using (var tasker = new Tasks.Tasker(this))
            {
                tasker.AttachView(new Tasks.TaskerTaskbar());
                tasker.AttachView(new Tasks.TaskerForm());
                if (allowOriginalGames)
                {
                    task.Games.AddRange(listViewGames.SelectedItems.Cast<ListViewItem>()
                            .Where(item => item.Tag is NesApplication)
                            .Select(item => item.Tag as NesApplication));
                }
                else
                {
                    task.Games.AddRange(listViewGames.SelectedItems.Cast<ListViewItem>()
                            .Where(item => item.Tag is NesApplication && !(item.Tag as NesApplication).IsOriginalGame)
                            .Select(item => item.Tag as NesApplication));
                }
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

        private void archiveSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.ArchiveGames, false))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);

            listViewGames.SelectedItems.Cast<ListViewItem>().ToList().
                ForEach(i => i.Selected = false);
        }

        private void compressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.CompressGames, false))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);

            listViewGames.SelectedItems.Cast<ListViewItem>().ToList().
                ForEach(i => i.Selected = false);
        }

        private void decompressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var task = new Tasks.GameTask();
            if (groupTaskWithSelected(task, task.DecompressGames, false))
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);

            listViewGames.SelectedItems.Cast<ListViewItem>().ToList().
                ForEach(i => i.Selected = false);
        }

        private void deleteSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.DeleteSelectedGamesQ, Resources.sign_delete, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                var task = new Tasks.GameTask();
                if (groupTaskWithSelected(task, task.DeleteGames, false))
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
                if (groupTaskWithSelected(task, task.ResetROMHeaders, false))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void repairGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.RepairSelectedGamesQ, Resources.sign_question, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                var task = new Tasks.GameTask();
                if (groupTaskWithSelected(task, task.RepairGames, false))
                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void originalGamesPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.Checked) return;
            SaveConfig();

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
            SaveConfig();

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
            SaveConfig();
            LoadGames(false);
        }

        private void openFoldersManager()
        {
            SaveConfig();
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
                if (WaitingShellForm.WaitForDevice(this))
                {
                    if (hakchi.MinimalMemboot)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                        return;
                    }
                    if (!hakchi.CanInteract)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                        return;
                    }
                    using (OpenFileDialog ofdPng = new OpenFileDialog())
                    {
                        ofdPng.Filter = $"{Resources.Images}|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
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
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void disableBootImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingShellForm.WaitForDevice(this))
                {
                    if (hakchi.MinimalMemboot)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                        return;
                    }
                    if (!hakchi.CanInteract)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                        return;
                    }
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
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void resetDefaultBootImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hakchi.MinimalMemboot)
                {
                    Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                    return;
                }
                if (!hakchi.CanInteract)
                {
                    Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                    return;
                }
                if (WaitingShellForm.WaitForDevice(this))
                {
                    hakchi.Shell.ExecuteSimple("hakchi unset cfg_boot_logo; rm \"$(hakchi get rootfs)/etc/boot.png\"");
                    hakchi.Shell.ExecuteSimple("rm \"/media/hakchi/boot.png\"");

                    if (!ConfigIni.Instance.DisablePopups)
                        Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                }
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void selectEmulationCoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count == 0)
                return;

            SaveConfig();
            using (SelectCoreDialog selectCoreDialog = new SelectCoreDialog())
            {
                selectCoreDialog.Games.AddRange(listViewGames.SelectedItems.Cast<ListViewItem>().
                    Select(item => item.Tag).
                    Where(tag => tag != null && tag is NesApplication).
                    Select(tag => tag as NesApplication).
                    Where(game => !game.IsOriginalGame));
                if (selectCoreDialog.Games.Count == 0)
                    return;
                if (selectCoreDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (selectCoreDialog.Modified)
                    {
                        SaveConfig();
                        LoadGames(false);
                    }
                }
            }

        }

        private void addCustomAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (NewCustomGameForm customGameForm = new NewCustomGameForm())
            {
                if (customGameForm.ShowDialog(this) == DialogResult.OK)
                {
                    SelectAll(false);

                    var newGroup = listViewGames.Groups.OfType<ListViewGroup>().Where(group => group.Header == Resources.ListCategoryNew).First();
                    var item = new ListViewItem(customGameForm.NewApp.Name);
                    item.Group = newGroup;
                    item.Tag = customGameForm.NewApp;
                    item.Checked = true;
                    item.Selected = true;

                    listViewGames.Items.Add(item);
                    listViewGames.EnsureVisible(item.Index);
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
                bool justBoot = false;
                if (hakchi.MinimalMemboot)
                {
                    var result = Tasks.MessageForm.Show(this, Resources.Rebooting, Resources.FinishBootSequenceQ, Resources.sign_life_buoy, new Tasks.MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No, MessageForm.Button.Cancel }, MessageForm.DefaultButton.Button1);
                    if (result == MessageForm.Button.Yes)
                    {
                        justBoot = true;
                    }
                    else if (result == MessageForm.Button.Cancel)
                    {
                        return;
                    }
                }

                using (var tasker = new Tasker(this))
                {
                    tasker.AttachViews(new Tasks.TaskerTaskbar(), new Tasks.TaskerForm());
                    tasker.SetStatusImage(Resources.sign_sync);
                    tasker.SetTitle(Resources.Rebooting);
                    if (justBoot)
                        tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.Memboot).Tasks);
                    else
                        tasker.AddTask(Tasks.ShellTasks.Reboot);
                    tasker.Start();
                }
            }
        }

        private void messageOfTheDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMOTD();
        }

        private void forceClovershellMembootsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.ForceClovershell = forceClovershellMembootsToolStripMenuItem.Checked;
            if (ConfigIni.Instance.ForceClovershell)
            {
                forceNetworkMembootsToolStripMenuItem.Checked = ConfigIni.Instance.ForceNetwork = false;
            }
        }

        private void forceNetworkMembootsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.ForceNetwork = forceNetworkMembootsToolStripMenuItem.Checked;
            if (ConfigIni.Instance.ForceNetwork)
            {
                forceClovershellMembootsToolStripMenuItem.Checked = ConfigIni.Instance.ForceClovershell = false;
            }
        }

        private void technicalInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TechInfo().ShowDialog(this);
        }

        private void dumpOriginalKernellegacyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dumpFilename;
            if (!DumpDialog(FileAccess.Write, "kernel.img", "img", out dumpFilename, $"{Resources.KernelDump} (*.img)|*.img"))
                return;

            using (var tasker = new Tasker(this))
            {
                tasker.AttachViews(new TaskerTaskbar(), new TaskerForm());
                tasker.SetStatusImage(Resources.sign_cogs);
                tasker.SetTitle(Resources.DumpingKernel);
                tasker.AddTasks(new MembootTasks(MembootTasks.MembootTaskType.DumpStockKernel, dumpPath: dumpFilename).Tasks);
                if (tasker.Start() == Tasker.Conclusion.Success)
                    MessageForm.Show(this, Resources.DumpOriginalKernelCompleteTitle, Resources.DumpOriginalKernelCompleteMessage, Resources.sign_check);
            }
        }

        private void convertSNESROMSToSFROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.ConvertToSFROM = convertSNESROMSToSFROMToolStripMenuItem.Checked;
        }

        private void separateGamesStorageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Instance.SeparateGameLocalStorage = separateGamesStorageToolStripMenuItem.Checked;
            SaveConfig();
            LoadGames();
        }

        private void switchRunningFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaitingShellForm.WaitForDevice(this))
                {
                    if (hakchi.MinimalMemboot)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedMinimalMemboot, Resources.sign_life_buoy);
                        return;
                    }
                    if (!hakchi.CanInteract)
                    {
                        Tasks.MessageForm.Show(this, Resources.Warning, Resources.CannotProceedCannotInteract, Resources.sign_ban);
                        return;
                    }
                    if (WaitingShellForm.WaitForDevice(this))
                    {
                        new SelectFirmwareDialog().ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void addModInfoToReport(ref List<string> list, ref List<Hmod.Hmod> mods)
        {
            foreach (var mod in mods)
            {
                list.Add($"  {mod.RawName}");
                list.Add("  --------------------");
                foreach(var fmKey in mod.Readme.frontMatter.Keys)
                {
                    list.Add($"    {fmKey}: {mod.Readme.frontMatter[fmKey]}");
                }
                list.Add("");
            }
        }

        private void generateModulesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Filter = "Text File|*.txt",
                FileName = "hmod_audit.txt"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                var installedMods = Hmod.Hmod.GetMods(true, null, this).OrderBy(o => o.RawName).ToList();
                var availableMods = Hmod.Hmod.GetMods(false, null, this).OrderBy(o => o.RawName).ToList();
                var separatorLine = "--------------------";

                var outLines = new List<string>();

                if (hakchi.CanInteract)
                {
                    outLines.Add("Installed Mods:");
                    outLines.Add(separatorLine);
                    outLines.Add("");
                    addModInfoToReport(ref outLines, ref installedMods);
                }
                else
                {
                    outLines.Add("System Not Online");
                    outLines.Add("");
                }

                outLines.Add("Available Mods:");
                outLines.Add(separatorLine);
                outLines.Add("");
                addModInfoToReport(ref outLines, ref availableMods);

                File.WriteAllText(sfd.FileName, String.Join("\r\n", outLines.ToArray()));
            }
        }

        private List<ToolStripMenuItem> repoMenuItems = new List<ToolStripMenuItem>();
        public void populateRepos()
        {
            foreach (var menuItem in repoMenuItems)
            {
                modulesToolStripMenuItem.DropDownItems.Remove(menuItem);
                if (!menuItem.IsDisposed)
                {
                    menuItem.Dispose();
                }
            }
            repoMenuItems.Clear();

            if (ConfigIni.Instance.repos.Length == 0)
            {
                var menuItem = new ToolStripMenuItem(Resources.NoRepositoriesConfigured) { Enabled = false };
                repoMenuItems.Add(menuItem);

                var index = modulesToolStripMenuItem.DropDownItems.IndexOf(modRepoEndSeparator);
                modulesToolStripMenuItem.DropDownItems.Insert(index, menuItem);
            }
            else
            {
                foreach (RepositoryInfo repo in ConfigIni.Instance.repos)
                {
                    var menuItem = new ToolStripMenuItem(repo.Name) { Tag = repo.URL };
                    menuItem.Click += repoMenuItem_Click;
                    repoMenuItems.Add(menuItem);

                    var index = modulesToolStripMenuItem.DropDownItems.IndexOf(modRepoEndSeparator);
                    modulesToolStripMenuItem.DropDownItems.Insert(index, menuItem);
                }
            }
        }

        private void repoMenuItem_Click(object sender, EventArgs e)
        {
            if(sender is ToolStripMenuItem)
            {
                ToolStripMenuItem unboxed = (ToolStripMenuItem)sender;

                if (unboxed.Tag is string && unboxed.Tag != null)
                {
                    var url = (string)(unboxed.Tag);
                    if (url == "modstore://") {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        var repo = new ModHub.Repository.Repository(url);
                        repo.LoadTasker(this);
                        using (var hub = new ModHub.ModHubForm())
                        {
                            hub.Text = unboxed.Text;
                            hub.LoadData(repo);
                            hub.ShowDialog(this);
                        }
                    }
                }
            }
        }

        private void manageModRepositoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepoManagementForm().ShowDialog(this);
        }

        private void saveDmesgOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hakchi.Shell.IsOnline || WaitingShellForm.WaitForDevice(this))
            {
                using (var sfd = new SaveFileDialog() { Filter = "Text Files|*.txt" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var outputFile = File.Create(sfd.FileName))
                        using (var tasker = new Tasks.Tasker(this))
                        {
                            tasker.AttachView(new TaskerTaskbar());
                            tasker.AttachView(new TaskerForm());
                            tasker.AddTask(ShellTasks.ShellCommand("dmesg", null, outputFile, outputFile));
                            tasker.Start();
                            outputFile.Close();
                            Tasks.MessageForm.Show(Resources.Wow, Resources.Done, Resources.sign_check);
                        }
                    }
                }
            }
        }
    }
}
