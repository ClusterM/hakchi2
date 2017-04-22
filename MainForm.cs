using com.clusterrr.clovershell;
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        public const long DefaultMaxGamesSize = 300;
        public static string BaseDirectory;
        public static string[] InternalMods = new string[] { "clovercon", "fontfix", "clovershell" };
        public static ClovershellConnection Clovershell;
        //readonly string UBootDump;
        public static string KernelDump;
        mooftpserv.Server ftpServer;

        static NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros.", Size = 571031 },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3", Size = 1163285 },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2",Size = 1510337 },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong", Size = 556016 },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." , Size = 558176 },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike", Size = 573231 },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda", Size = 663910 },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure", Size = 1321661 },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid", Size = 662601 },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight", Size = 556131 },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link", Size = 1024158 },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream", Size = 1038128 },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber", Size = 553436 },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus", Size = 670710 },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros.", Size = 1018973 },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO", Size = 1089427 },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics", Size = 1299361 },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2", Size = 569868 },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™", Size = 440971 },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®", Size = 552556 },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE" , Size = 474232 },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN", Size = 325888 },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga", Size =  347079	},
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania", Size = 434240 },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS", Size = 370790 },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C", Size = 565974 },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest", Size = 569759 },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN", Size =573536 },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL", Size =568276 },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge", Size = 578900 }
        };
        NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ", Size = 596775 },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３", Size = 1411534 },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ", Size = 1501542 },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング" , Size = 568006 },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク" , Size = 597513 },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ" , Size = 798179 },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説", Size = 677971	},
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語" , Size = 1331436 },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド" , Size = 666895 },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト" , Size = 569750 },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険" , Size = 666452 },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー" , Size = 812372	 },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ" , Size = 1038275 },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ" , Size = 1083234	},
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎" , Size = 592425	},
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®", Size = 456166	},
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III" , Size = 830898 },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン" , Size = 341593 },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ", Size =  345552 },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" , Size = 428522 },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス", Size = 393055 },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅" , Size = 569537 },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー", Size = 336353 },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝" , Size = 578623 },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵" , Size = 387084 },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲", Size = 392595 },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge", Size = 579757 },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語" , Size = 588367 },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会", Size = 587083 },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎", Size = 376213 }
        };

        public MainForm()
        {
            ConfigIni.Load();
            try
            {
                if (!string.IsNullOrEmpty(ConfigIni.Language))
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Language);
            }
            catch { }
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
                shellToolStripMenuItem.Checked = true;
        }

        void FormInitialize()
        {
            try
            {
                BaseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                KernelDump = Path.Combine(Path.Combine(BaseDirectory, "dump"), "kernel.img");
                LoadGames();
                LoadHidden();
                LoadPresets();
                LoadLanguages();
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Text = string.Format("hakchi2 - v{0}.{1:D2}{2}", version.Major, version.Build, (version.Revision < 10) ?
                    ("rc" + version.Revision.ToString()) : (version.Revision > 10 ? ((char)('a' + version.Revision - 10)).ToString() : ""));

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
                WorkerForm.GetMemoryStats();
                new Thread(RecalculateSelectedGamesThread).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public void LoadGames()
        {
            Debug.WriteLine("Loading games");
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
            ShowSelected();
        }

        public void ShowSelected()
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null)
            {
                groupBoxDefaultGames.Visible = false;
                groupBoxOptions.Visible = true;
                groupBoxOptions.Enabled = false;
                labelID.Text = "ID: ";
                textBoxName.Text = "";
                radioButtonOne.Checked = true;
                radioButtonTwo.Checked = false;
                radioButtonTwoSim.Checked = false;
                maskedTextBoxReleaseDate.Text = "";
                textBoxPublisher.Text = "";
                textBoxArguments.Text = "";
                pictureBoxArt.Image = null;
            }
            else if (!(selected is NesMiniApplication))
            {
                groupBoxDefaultGames.Visible = true;
                groupBoxOptions.Visible = false;
                groupBoxDefaultGames.Enabled = checkedListBoxGames.CheckedIndices.Contains(0);
            }
            else
            {
                var app = selected as NesMiniApplication;
                groupBoxDefaultGames.Visible = false;
                groupBoxOptions.Visible = true;
                labelID.Text = "ID: " + app.Code;
                textBoxName.Text = app.Name;
                if (app.Simultaneous && app.Players == 2)
                    radioButtonTwoSim.Checked = true;
                else if (app.Players == 2)
                    radioButtonTwo.Checked = true;
                else
                    radioButtonOne.Checked = true;
                maskedTextBoxReleaseDate.Text = app.ReleaseDate;
                textBoxPublisher.Text = app.Publisher;
                if (app is NesGame)
                    textBoxArguments.Text = (app as NesGame).Args;
                else if (app is FdsGame)
                    textBoxArguments.Text = (app as FdsGame).Args;
                else
                    textBoxArguments.Text = app.Command;
                if (File.Exists(app.IconPath))
                    pictureBoxArt.Image = NesMiniApplication.LoadBitmap(app.IconPath);
                else
                    pictureBoxArt.Image = null;
                buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = app is NesGame;
                textBoxGameGenie.Text = (app is NesGame) ? (app as NesGame).GameGenie : "";
                groupBoxOptions.Enabled = true;
            }
        }

        void LoadHidden()
        {
            checkedListBoxDefaultGames.Items.Clear();
            var hidden = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var game in new List<NesDefaultGame>(ConfigIni.ConsoleType == 0 ? defaultNesGames : defaultFamicomGames).OrderBy(o => o.Name))
                checkedListBoxDefaultGames.Items.Add(game, !hidden.Contains(game.Code));
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
                        for (int j = 0; j < checkedListBoxDefaultGames.Items.Count; j++)
                            checkedListBoxDefaultGames.SetItemChecked(j,
                                !hide.Contains(((NesDefaultGame)checkedListBoxDefaultGames.Items[j]).Code));
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
            var languages = new List<string>(Directory.GetDirectories(Path.Combine(BaseDirectory, "languages")));
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
                item.Text = Path.GetFileName(language);
                item.Click += delegate(object sender, EventArgs e)
                    {
                        ConfigIni.Language = langCodes[language];
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCodes[language]);
                        this.Controls.Clear();
                        this.InitializeComponent();
                        FormInitialize();
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
                    SaveSelectedGames();
                    ConfigIni.Presets[name] = ConfigIni.SelectedGames + "|" + ConfigIni.HiddenGames;
                    LoadPresets();
                }
            }
        }

        private void checkedListBoxGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        void SetImageForSelectedGame(string fileName)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Image = NesMiniApplication.LoadBitmap(fileName);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            openFileDialogImage.Filter = Resources.Images + " (*.bmp;*.png;*.jpg;*.jpeg;*.gif)|*.bmp;*.png;*.jpg;*.jpeg;*.gif|" + Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetImageForSelectedGame(openFileDialogImage.FileName);
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            var googler = new ImageGooglerForm(game);
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.Image = googler.Result;
                ShowSelected();
                timerCalculateGames.Enabled = true;
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Name = textBoxName.Text;
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            if (game is NesGame)
                (game as NesGame).Args = textBoxArguments.Text;
            else if (game is FdsGame)
                (game as FdsGame).Args = textBoxArguments.Text;
            else
                game.Command = textBoxArguments.Text;
        }

        private void maskedTextBoxReleaseDate_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.ReleaseDate = maskedTextBoxReleaseDate.Text;
        }

        private void textBoxGameGenie_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.GameGenie = textBoxGameGenie.Text;
        }

        private void SaveSelectedGames()
        {
            var selected = new List<string>();
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesMiniApplication)
                    selected.Add((game as NesMiniApplication).Code);
                else
                    selected.Add("default");
            }
            ConfigIni.SelectedGames = string.Join(";", selected.ToArray());
            selected.Clear();
            foreach (NesDefaultGame game in checkedListBoxDefaultGames.Items)
                selected.Add(game.Code);
            foreach (NesDefaultGame game in checkedListBoxDefaultGames.CheckedItems)
                selected.Remove(game.Code);
            ConfigIni.HiddenGames = string.Join(";", selected.ToArray());
        }

        private void SaveConfig()
        {
            SaveSelectedGames();
            ConfigIni.Save();
            for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
            {
                var game = checkedListBoxGames.Items[i];
                try
                {
                    if (game is NesMiniApplication)
                    {
                        // Maybe type was changed? Need to reload games
                        if ((game as NesMiniApplication).Save())
                            checkedListBoxGames.Items[i] = NesMiniApplication.FromDirectory((game as NesMiniApplication).GamePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesMiniApplication)
                {
                    stats.Count++;
                    stats.Size += (game as NesMiniApplication).Size();
                }
                else
                {
                    stats.Count += checkedListBoxDefaultGames.CheckedItems.Count;
                    foreach (NesDefaultGame originalGame in checkedListBoxDefaultGames.CheckedItems)
                        stats.Size += originalGame.Size;
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
                    maxGamesSize = (WorkerForm.NandCFree + WorkerForm.WritedGamesSize) - WorkerForm.ReservedMemory * 1024 * 1024;
                toolStripStatusLabelSelected.Text = stats.Count + " " + Resources.GamesSelected;
                toolStripStatusLabelSize.Text = string.Format("{0:F1}MB / {1:F1}MB", stats.Size / 1024.0 / 1024.0, maxGamesSize / 1024.0 / 1024.0);
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

        private void checkedListBoxDefaultGames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGames.IndexFromPoint(e.X, e.Y);
                selectAllToolStripMenuItem.Tag = unselectAllToolStripMenuItem.Tag = 1;
                deleteGameToolStripMenuItem.Enabled = false;
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
            for (int i = 0; i < checkedListBoxDefaultGames.Items.Count; i++)
            {
                if (needOriginal && checkedListBoxDefaultGames.CheckedIndices.Contains(i))
                    workerForm.Games.Add((NesDefaultGame)checkedListBoxDefaultGames.Items[i]);
            }

            workerForm.FoldersMode = ConfigIni.FoldersMode;
            workerForm.MaxGamesPerFolder = ConfigIni.MaxGamesPerFolder;
            workerForm.MainForm = this;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        void AddGames(IEnumerable<string> files)
        {
            SaveConfig();
            ICollection<NesMiniApplication> addedApps;
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.LoadingGames;
            workerForm.Task = WorkerForm.Tasks.AddGames;
            workerForm.GamesToAdd = files;
            workerForm.Start();
            addedApps = workerForm.addedApplications;

            if (addedApps != null)
            {
                // Add games, only new ones
                var newApps = addedApps.Distinct(new NesMiniApplication.NesMiniAppEqualityComparer());
                var newCodes = from app in newApps select app.Code;
                var oldAppsReplaced = from app in checkedListBoxGames.Items.Cast<object>().ToArray()
                                      where (app is NesMiniApplication) && newCodes.Contains((app as NesMiniApplication).Code)
                                      select app;
                foreach (var replaced in oldAppsReplaced)
                    checkedListBoxGames.Items.Remove(replaced);
                checkedListBoxGames.Items.AddRange(newApps.ToArray());
                var first = checkedListBoxGames.Items[0];
                bool originalChecked = (checkedListBoxGames.CheckedItems.Contains(first));
                checkedListBoxGames.Items.Remove(first);
                checkedListBoxGames.Sorted = true;
                checkedListBoxGames.Sorted = false;
                checkedListBoxGames.Items.Insert(0, first);
                checkedListBoxGames.SetItemChecked(0, originalChecked);
            }
            else
            {
                // Reload all games (maybe process was terminated?)
                LoadGames();
            }
            if (addedApps != null) // if added only one game select it
            {
                bool first = true;
                foreach (var addedApp in addedApps)
                {
                    for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
                        if ((checkedListBoxGames.Items[i] is NesMiniApplication) &&
                            (checkedListBoxGames.Items[i] as NesMiniApplication).Code == addedApp.Code)
                        {
                            if (first)
                                checkedListBoxGames.SelectedIndex = i;
                            first = false;
                            checkedListBoxGames.SetItemChecked(i, true);
                            break;
                        }
                }
            }
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
            foreach (var game in checkedListBoxGames.Items)
            {
                if (game is NesMiniApplication)
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
            LoadHidden();
        }

        private void famicomMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (famicomMiniToolStripMenuItem.Checked) return;
            ConfigIni.ConsoleType = 1;
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
            ConfigIni.HiddenGames = "";
            LoadHidden();
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
            if (e.Index == 0)
                groupBoxDefaultGames.Enabled = e.NewValue == CheckState.Checked;
            // Schedule recalculation
            timerCalculateGames.Enabled = false;
            timerCalculateGames.Enabled = true;
        }

        private void checkedListBoxDefaultGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Schedule recalculation
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
                    Directory.Delete(game.GamePath, true);
                    checkedListBoxGames.Items.RemoveAt(pos);
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
                for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
                    checkedListBoxGames.SetItemChecked(i, true);
            else
                for (int i = 0; i < checkedListBoxDefaultGames.Items.Count; i++)
                    checkedListBoxDefaultGames.SetItemChecked(i, true);
        }

        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)(sender as ToolStripMenuItem).Tag == 0)
                for (int i = 0; i < checkedListBoxGames.Items.Count; i++)
                    checkedListBoxGames.SetItemChecked(i, false);
            else for (int i = 0; i < checkedListBoxDefaultGames.Items.Count; i++)
                    checkedListBoxDefaultGames.SetItemChecked(i, false);
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
                    SevenZipExtractor.SetLibraryPath(Path.Combine(BaseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
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

        private void buttonShowGameGenieDatabase_Click(object sender, EventArgs e)
        {
            if (!(checkedListBoxGames.SelectedItem is NesGame)) return;
            NesGame nesGame = checkedListBoxGames.SelectedItem as NesGame;
            GameGenieCodeForm lFrm = new GameGenieCodeForm(nesGame);
            if (lFrm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxGameGenie.Text = nesGame.GameGenie;
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
            foreach (var game in defaultNesGames)
                gameNames[game.Code] = game.Name;
            foreach (var game in defaultFamicomGames)
                gameNames[game.Code] = game.Name;
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
    }
}
