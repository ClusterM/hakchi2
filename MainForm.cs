using com.clusterrr.clovershell;
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
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
        public const long DefaultMaxGamesSize = 300;
        public static IEnumerable<string> InternalMods;
        public static ClovershellConnection Clovershell;
        //readonly string UBootDump;
        public static string KernelDump;
        mooftpserv.Server ftpServer;

        

        public MainForm()
        {
            InitializeComponent();
            FormInitialize();
            Clovershell = new ClovershellConnection() { AutoReconnect = true, Enabled = true };
            Clovershell.OnConnected += Clovershell_OnConnected;

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

                KernelDump = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "dump"), "kernel.img");
                InternalMods = from m in Directory.GetFiles(Path.Combine(Program.BaseDirectoryInternal, "mods/hmods")) select Path.GetFileNameWithoutExtension(m);
                Manager.GameManager.GetInstance().GamesRemoved += MainForm_GamesRemoved; ;
                Manager.GameManager.GetInstance().NewGamesAdded += MainForm_NewGamesAdded;
                Manager.GameManager.GetInstance().LoadLibrary();
             //   LoadGames();
              
                LoadPresets();
                LoadLanguages();
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Text = string.Format("hakchi2 - v{0}.{1:D2}{2}", version.Major, version.Build, (version.Revision < 10) ?
                    ("rc" + version.Revision.ToString()) : (version.Revision > 20 ? ((char)('a' + (version.Revision - 20) / 10)).ToString() : ""))
#if DEBUG
 + " (debug version"
#if VERY_DEBUG
 + ", very verbose mode"
#endif
 + ")"
#endif
;
                // Some settnigs
                useExtendedFontToolStripMenuItem.Checked = ConfigIni.UseFont;
                epilepsyProtectionToolStripMenuItem.Checked = ConfigIni.AntiArmetLevel > 0;
                selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.ResetHack;
                enableAutofireToolStripMenuItem.Checked = ConfigIni.AutofireHack;
                useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked = ConfigIni.AutofireXYHack;
                nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
                famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
                upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.FcStart;
                compressGamesIfPossibleToolStripMenuItem.Checked = ConfigIni.Compress;

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

                max20toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 20;
                max25toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 25;
                max30toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 30;
                max35toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 35;
                max40toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 40;
                max45toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 45;
                max50toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 50;
                max60toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 60;
                max70toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 70;
                max80toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 80;
                max90toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 90;
                max100toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 100;

               

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
                ConfigIni.CustomFlashed = true; // Just in case of new installation
                // Requesting autoshutdown state
                var autoshutdown = Clovershell.ExecuteSimple("cat /var/lib/clover/profiles/0/shutdown.txt");
                // Disable automatic shutdown
                if (autoshutdown != "0")
                    Clovershell.ExecuteSimple("echo -n 0 > /var/lib/clover/profiles/0/shutdown.txt");
                // Setting actual time for file transfer operations
                Clovershell.ExecuteSimple(string.Format("date -s \"{0:yyyy-MM-dd HH:mm:ss}\"", DateTime.UtcNow));
                // Restoring automatic shutdown
                if (autoshutdown != "0")
                    Clovershell.ExecuteSimple(string.Format("echo -n {0} > /var/lib/clover/profiles/0/shutdown.txt", autoshutdown));
                var customFirmware = Clovershell.ExecuteSimple("[ -d /var/lib/hakchi/firmware/ ] && [ -f /var/lib/hakchi/firmware/*.hsqs ] && echo YES || echo NO");
                if (customFirmware == "NO")
                {
                    var region = Clovershell.ExecuteSimple("cat /etc/clover/REGION", 500, true);
                    Debug.WriteLine(string.Format("Detected region: {0}", region));
                    if (region == "JPN")
                        Invoke(new Action(delegate
                        {
                            famicomMiniToolStripMenuItem.PerformClick();
                        }));
                    if (region == "EUR_USA")
                        Invoke(new Action(delegate
                        {
                            nESMiniToolStripMenuItem.PerformClick();
                        }));
                }
                WorkerForm.GetMemoryStats();
                new Thread(RecalculateSelectedGamesThread).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

      

        private void MainForm_NewGamesAdded(List<NesMiniApplication> e)
        {
            foreach (var game in e.OrderBy(o=>o.Name))
            {
                checkedListBoxGames.Items.Add(game, game.Selected);
            }
            ShowStats();
            ShowSelected();
        }

        private void MainForm_GamesRemoved(List<NesMiniApplication> e)
        {
            foreach (var game in e)
            {
                checkedListBoxGames.Items.Remove(e);
            }
            ShowStats();
            ShowSelected();
        }

        public void ShowSelected()
        {
            NesMiniApplication selected = (NesMiniApplication)checkedListBoxGames.SelectedItem;
            gameDetail1.SetGame(selected);
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
                    delegate(object sender, EventArgs e)
                    {
                        var cols = ConfigIni.Presets[preset].Split('|');
                        ConfigIni.SelectedGames = cols[0];
                        ConfigIni.HiddenGames = cols[1];
                        var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        var hide = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        checkedListBoxGames.SetItemChecked(0, selected.Contains("default"));
                        for (int j = 1; j < checkedListBoxGames.Items.Count; j++)
                            checkedListBoxGames.SetItemChecked(j,
                                selected.Contains((checkedListBoxGames.Items[j] as NesMiniApplication).Code));
                        
                    }));
                deletePresetToolStripMenuItem.DropDownItems.Insert(i, new ToolStripMenuItem(preset, null,
                    delegate(object sender, EventArgs e)
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
                if (country.Length > 2) country = country.Substring(country.Length - 2).ToLower();
                item.Image = (Image)rm.GetObject(country);
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Click += delegate(object sender, EventArgs e)
                    {
                        ConfigIni.Language = langCodes[language];
                        SaveConfig();
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCodes[language]);
                        this.Controls.Clear();
                        this.InitializeComponent();
                        FormInitialize();
                        this.Invalidate(true);
                    };
                item.Checked = Thread.CurrentThread.CurrentUICulture.Name == langCodes[language];
                found |= item.Checked;
                if (langCodes[language] == "en-US")
                    english = item;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }
            if (!found)
                english.Checked = true;
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
                    Manager.GameManager.GetInstance().SaveChanges();
                    ConfigIni.Presets[name] = ConfigIni.SelectedGames + "|" + ConfigIni.HiddenGames;
                    LoadPresets();
                }
            }
        }

        private void checkedListBoxGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }



        


       

     
        private void SaveConfig()
        {
            Manager.GameManager.GetInstance().SaveChanges();
          
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
            
            //Process.GetCurrentProcess().Kill(); // Suicide! Just easy and dirty way to kill all threads.
        }


     
        void RecalculateSelectedGamesThread()
        {
            try
            {
                
                ShowStats();
            }
            catch
            {
                timerCalculateGames.Enabled = false;
                timerCalculateGames.Enabled = true;
            }
        }
        void ShowStats()
        {
            showStats(Manager.GameManager.GetInstance().GetStatistics());
        }
        void showStats(Manager.GameManager.CountResult stats)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Manager.GameManager.CountResult>(showStats), new object[] { stats });
                    return;
                }

                var maxGamesSize = DefaultMaxGamesSize * 1024 * 1024;
                if (WorkerForm.NandCTotal > 0)
                    maxGamesSize = (WorkerForm.NandCFree + WorkerForm.WritedGamesSize) - WorkerForm.ReservedMemory * 1024 * 1024;
                toolStripStatusLabelSelected.Text = stats.SelectedCount + " " + Resources.GamesSelected;
                toolStripStatusLabelSize.Text = string.Format("{0:F1}MB / {1:F1}MB", stats.SelectedSize / 1024.0 / 1024.0, maxGamesSize / 1024.0 / 1024.0);
                toolStripProgressBar.Maximum = (int)maxGamesSize;
                toolStripProgressBar.Value = Math.Min((int)stats.SelectedSize, toolStripProgressBar.Maximum);
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

        private void checkedListBoxGames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGames.IndexFromPoint(e.X, e.Y);
                selectAllToolStripMenuItem.Tag = unselectAllToolStripMenuItem.Tag = 0;
                deleteGameToolStripMenuItem.Tag = i;
                deleteGameToolStripMenuItem.Enabled = i > 0;
                contextMenuStrip.Show(sender as Control, e.X, e.Y);
            }
        }


        DialogResult RequireKernelDump()
        {
            if (File.Exists(KernelDump)) return DialogResult.OK; // OK - already dumped
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
            var kernelDump = RequireKernelDump(); // We need kernel dump first
            if (kernelDump == System.Windows.Forms.DialogResult.No)
                return DialogResult.No; // Abort if user has not dumped it
            if (MessageBox.Show((kernelDump == DialogResult.Yes ? (Resources.KernelDumped + "\r\n") : "") +
                    Resources.CustomWarning, Resources.CustomKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
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

            var stats = Manager.GameManager.GetInstance().GetStatistics();
            if (stats.SelectedCount == 0)
            {
                MessageBox.Show(Resources.SelectAtLeast, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var kernel = RequirePatchedKernel();
            if (kernel == DialogResult.No) return;
            if (kernel == DialogResult.Yes) // Message for new user
                MessageBox.Show(Resources.DoneYouCanUpload + "\r\n" + Resources.PressOkToContinue, Resources.Congratulations, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (UploadGames())
            {
                MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        bool DoKernelDump()
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.DumpingKernel;
            workerForm.Task = WorkerForm.Tasks.DumpKernel;
            //workerForm.UBootDump = UBootDump;
            workerForm.KernelDump = KernelDump;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool FlashCustomKernel()
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.FlasingCustom;
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.KernelDump = KernelDump;
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

        bool UploadGames()
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.UploadingGames;
            workerForm.Task = WorkerForm.Tasks.UploadGames;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_hakchi";
            workerForm.Config = ConfigIni.GetConfigDictionary();
            workerForm.Games = new NesMenuCollection();
            bool needOriginal = false;
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesMiniApplication)
                    workerForm.Games.Add(game as NesMiniApplication);
                else
                    needOriginal = true;
            }
         

            workerForm.FoldersMode = ConfigIni.FoldersMode;
            workerForm.MaxGamesPerFolder = ConfigIni.MaxGamesPerFolder;
            workerForm.MainForm = this;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        void AddGames(IEnumerable<string> files)
        {
            List<NesMiniApplication> addedApps;
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.LoadingGames;
            workerForm.Task = WorkerForm.Tasks.AddGames;
            workerForm.GamesToAdd = files;
            workerForm.Start();
            addedApps = workerForm.addedApplications;

            foreach (NesMiniApplication g in addedApps)
            {
                g.Selected = true;
            }
            Manager.GameManager.GetInstance().AddGames(addedApps);

            // Schedule recalculation
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        bool FlashOriginalKernel(bool boot = true)
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.FlasingOriginal;
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.KernelDump = KernelDump;
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
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.Uninstalling;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_uninstall";
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool InstallMods(string[] mods)
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.InstallingMods;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_hakchi";
            workerForm.hmodsInstall = new List<string>(mods);
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool UninstallMods(string[] mods)
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.UninstallingMods;
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_hakchi";
            workerForm.hmodsUninstall = new List<string>(mods);
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool DownloadAllCovers()
        {
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.DownloadAllCoversTitle;
            workerForm.Task = WorkerForm.Tasks.DownloadAllCovers;
            workerForm.Games = new NesMenuCollection();
            foreach (var game in Manager.GameManager.GetInstance().getAllGames())
            {
                if (game.GetType() != typeof(NesDefaultGame))
                    workerForm.Games.Add(game as NesMiniApplication);
            }
            return workerForm.Start() == DialogResult.OK;
        }

        private void dumpKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(KernelDump))
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

        private void flashCustomKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequireKernelDump() == DialogResult.No) return;
            if (MessageBox.Show(Resources.CustomKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (FlashCustomKernel()) MessageBox.Show(Resources.DoneYouCanUpload, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void flashOriginalKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(KernelDump))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(Resources.OriginalKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (FlashOriginalKernel()) MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(KernelDump))
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
                        FlashOriginalKernel();
                    MessageBox.Show(Resources.UninstallFactoryNote, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var form = new SelectButtonsForm(ConfigIni.ResetCombination);
            if (form.ShowDialog() == DialogResult.OK)
                ConfigIni.ResetCombination = form.SelectedButtons;
        }

        private void nESMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nESMiniToolStripMenuItem.Checked) return;
            ConfigIni.ConsoleType = 0;
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
            ConfigIni.HiddenGames = "";
            Manager.GameManager.GetInstance().ReloadDefault();
        }

        private void famicomMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (famicomMiniToolStripMenuItem.Checked) return;
            ConfigIni.ConsoleType = 1;
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
            ConfigIni.HiddenGames = "";
            Manager.GameManager.GetInstance().ReloadDefault();
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

        private void timerCalculateGames_Tick(object sender, EventArgs e)
        {
            new Thread(RecalculateSelectedGamesThread).Start(); // Calculate it in background
            timerCalculateGames.Enabled = false; // We don't need to count games repetedly
        }

        private void checkedListBoxGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            NesMiniApplication app = (NesMiniApplication)checkedListBoxGames.Items[e.Index];
            app.Selected = (e.NewValue == CheckState.Checked);
            Manager.GameManager.GetInstance().SaveChanges();
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }
        

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (ConfigIni.FirstRun && !File.Exists(KernelDump))
            {
                MessageBox.Show(this, Resources.FirstRun + "\r\n\r\n" + Resources.Donate, Resources.Hello, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ConfigIni.FirstRun = false;
                ConfigIni.Save();
            }
        }

        private void deleteGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteGame((int)(sender as ToolStripMenuItem).Tag);
        }

        private void deleteGame(int pos)
        {
            try
            {
                var game = checkedListBoxGames.Items[pos] as NesMiniApplication;
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
            // Schedule recalculation
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)(sender as ToolStripMenuItem).Tag == 0)
            {
                for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
                {
                    checkedListBoxGames.SetItemChecked(i, true);
                    var game = checkedListBoxGames.Items[i] as NesMiniApplication;
                    game.Selected = true;
                }
            }
            
        }

        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)(sender as ToolStripMenuItem).Tag == 0)
                for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
                {
                    checkedListBoxGames.SetItemChecked(i, false);
                    var game = checkedListBoxGames.Items[i] as NesMiniApplication;
                    game.Selected = false;
                }
          
        }

        private void checkedListBoxGames_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void checkedListBoxGames_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Need to determine type of files
            // Maybe it's cover art?
            if (files.Length == 1)
            {
                var ext = Path.GetExtension(files[0]).ToLower();
                if (ext == ".jpg" || ext == ".png")
                {
                    gameDetail1.SetImageForSelectedGame(files[0]);
                    
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

        private void downloadCoversForAllGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DownloadAllCovers())
                MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void checkedListBoxGames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && checkedListBoxGames.SelectedIndex > 0)
                deleteGame(checkedListBoxGames.SelectedIndex);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && e.Modifiers == Keys.Shift)
            {
                int counter = 0;
                foreach (var g in checkedListBoxGames.Items)
                {
                    if (g is NesMiniApplication)
                    {
                        var game = g as NesMiniApplication;
                        if (game is NesGame)
                        {
                            try
                            {
                                if ((game as NesGame).TryAutofill(new NesFile((game as NesGame).NesPath).CRC32))
                                    counter++;
                            }
                            catch { }
                        }
                    }
                }
                ShowSelected();
                MessageBox.Show(this, string.Format(Resources.AutofillResult, counter), Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripMenuMaxGamesPerFolder_Click(object sender, EventArgs e)
        {
            ConfigIni.MaxGamesPerFolder = byte.Parse((sender as ToolStripMenuItem).Text);
            max20toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 20;
            max25toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 25;
            max30toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 30;
            max35toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 35;
            max40toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 40;
            max45toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 45;
            max50toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 50;
            max60toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 60;
            max70toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 70;
            max80toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 80;
            max90toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 90;
            max100toolStripMenuItem.Checked = ConfigIni.MaxGamesPerFolder == 100;
        }

        private void compressGamesIfPossibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Compress = compressGamesIfPossibleToolStripMenuItem.Checked;
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
                    MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void timerConnectionCheck_Tick(object sender, EventArgs e)
        {
            if (Clovershell != null)
            {
                toolStripStatusConnectionIcon.Image = Clovershell.IsOnline ? Resources.green : Resources.red;
                toolStripStatusConnectionIcon.ToolTipText = Clovershell.IsOnline ? "Online" : "Offline";
            }
        }

        private void saveSettingsToNESMiniNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RequirePatchedKernel() == DialogResult.No) return;
            try
            {
                if (WaitingClovershellForm.WaitForDevice(this))
                {
                    WorkerForm.SyncConfig(ConfigIni.GetConfigDictionary(), true);
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
         
            foreach (var game in checkedListBoxGames.Items)
            {
                if (game is NesMiniApplication)
                    gameNames[(game as NesMiniApplication).Code] = (game as NesMiniApplication).Name;
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
                    var ftpThread = new Thread(delegate()
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
                            Invoke(new Action(delegate()
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
                    new Thread(delegate()
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
    }
}
