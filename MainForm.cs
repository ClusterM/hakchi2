using com.clusterrr.clovershell;
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
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
        public static IEnumerable<string> InternalMods;
        public static ClovershellConnection Clovershell;
        mooftpserv.Server ftpServer;
        public static bool? DownloadCover;

        static NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros.", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2",Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros.", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga", Size =  25000 },
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN", Size =25000 },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL", Size =25000 },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge", Size = 25000 }
        };
        static NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説", Size = 25000  },
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー" , Size = 25000    },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ" , Size = 25000   },
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎" , Size = 25000  },
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®", Size = 25000    },
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ", Size =  25000 },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎", Size = 25000 }
        };
        static NesDefaultGame[] defaultSnesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-SAAAE",  Name = "Super Mario World", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAABE",  Name = "F-ZERO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAEE",  Name = "The Legend of Zelda: A Link to the Past", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAFE",  Name = "Super Mario Kart", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAHE",  Name = "Super Metroid", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAJE",  Name = "EarthBound", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAKE",  Name = "Kirby's Dream Course", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAALE",  Name = "Donkey Kong Country", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAQE",  Name = "Kirby Super Star", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAXE",  Name = "Super Punch-Out!!", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABCE",  Name = "Mega Man X", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABDE",  Name = "Super Ghouls'n Ghosts", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABHE",  Name = "Street Fighter II Turbo: Hyper Fighting", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABQE",  Name = "Super Mario RPG: Legend of the Seven Stars", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABRE",  Name = "Secret of Mana", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABTE",  Name = "Final Fantasy III", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SACBE",  Name = "Super Castlevania IV", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SACCE",  Name = "CONTRA III THE ALIEN WARS", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADGE",  Name = "Star Fox", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADJE",  Name = "Yoshi's Island", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADKE",  Name = "Star Fox 2", Size = 25000 }
        };
        static NesDefaultGame[] defaultSuperFamicomGames = new NesDefaultGame[]
        {
            new NesDefaultGame { Code = "CLV-P-VAAAJ",  Name = "スーパーマリオワールド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAABJ",  Name = "F-ZERO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAEJ",  Name = "ゼルダの伝説 神々のトライフォース", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAFJ",  Name = "スーパーマリオカート", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAGJ",  Name = "ファイアーエムブレム 紋章の謎", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAHJ",  Name = "スーパーメトロイド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAALJ",  Name = "スーパードンキーコング", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAQJ",  Name = "星のカービィ スーパーデラックス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABBJ",  Name = "スーパーストリートファイターⅡ ザ ニューチャレンジャーズ", Size = 24576 },
            new NesDefaultGame { Code = "CLV-P-VABCJ",  Name = "ロックマンX", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABDJ",  Name = "超魔界村", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABQJ",  Name = "スーパーマリオRPG", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABRJ",  Name = "聖剣伝説2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABTJ",  Name = "ファイナルファンタジーVI", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VACCJ",  Name = "魂斗羅スピリッツ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VACDJ",  Name = "がんばれゴエモン ゆき姫救出絵巻", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADFJ",  Name = "スーパーフォーメーションサッカー", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADGJ",  Name = "スターフォックス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADJJ",  Name = "スーパーマリオ ヨッシーアイランド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADKJ",  Name = "スターフォックス2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADZJ",  Name = "パネルでポン", Size = 25000 },
        };


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
                SyncConsoleType();
                InternalMods = from m in Directory.GetFiles(Path.Combine(Program.BaseDirectoryInternal, "mods/hmods")) select Path.GetFileNameWithoutExtension(m);
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

                listViewGames.ListViewItemSorter = new GamesSorter();

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
                    switch (board)
                    {
                        default:
                        case "dp-nes":
                        case "dp-hvc":
                            switch (region)
                            {
                                case "EUR_USA":
                                    ConfigIni.ConsoleType = ConsoleType.NES;
                                    break;
                                case "JPN":
                                    ConfigIni.ConsoleType = ConsoleType.Famicom;
                                    break;
                            }
                            break;
                        case "dp-shvc":
                            switch (region)
                            {
                                case "USA":
                                case "EUR":
                                    ConfigIni.ConsoleType = ConsoleType.SNES;
                                    break;
                                case "JPN":
                                    ConfigIni.ConsoleType = ConsoleType.SuperFamicom;
                                    break;
                            }
                            break;
                    }
                    Invoke(new Action(SyncConsoleType));
                }

                ConfigIni.CustomFlashed = true; // Just in case of new installation

                WorkerForm.GetMemoryStats();
                new Thread(RecalculateSelectedGamesThread).Start();

                /*
                // It's good idea to sync time... or not?
                // Requesting autoshutdown state
                var autoshutdown = Clovershell.ExecuteSimple("cat /var/lib/clover/profiles/0/shutdown.txt");
                // Disable automatic shutdown
                if (autoshutdown != "0")
                {
                    Clovershell.ExecuteSimple("echo -n 0 > /var/lib/clover/profiles/0/shutdown.txt");
                    Thread.Sleep(1500);
                }
                // Setting actual time for file transfer operations
                Clovershell.ExecuteSimple(string.Format("date -s \"{0:yyyy-MM-dd HH:mm:ss}\"", DateTime.UtcNow));
                // Restoring automatic shutdown
                if (autoshutdown != "0")
                    Clovershell.ExecuteSimple(string.Format("echo -n {0} > /var/lib/clover/profiles/0/shutdown.txt", autoshutdown));
                */
                // It was bad idea
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
            listViewGames.Items.Clear();
            var listViewItem = new ListViewItem(Resources.Default30games);
            listViewItem.Tag = "default";
            listViewItem.Checked = selected.Contains("default");
            listViewGames.Items.Add(listViewItem);
            foreach (var game in gamesSorted)
            {
                listViewItem = new ListViewItem(game.Name);
                listViewItem.Tag = game;
                listViewItem.Checked = selected.Contains(game.Code);
                listViewGames.Items.Add(listViewItem);
            }
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
                groupBoxDefaultGames.Visible = false;
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
                buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = false;
                textBoxGameGenie.Text = "";
                checkBoxCompressed.Enabled = false;
                checkBoxCompressed.Checked = false;
            }
            else if (!(selected is NesMiniApplication))
            {
                groupBoxDefaultGames.Visible = true;
                groupBoxOptions.Visible = false;
                groupBoxDefaultGames.Enabled = listViewGames.CheckedIndices.Contains(0);
            }
            else
            {
                var app = selected as NesMiniApplication;
                groupBoxDefaultGames.Visible = false;
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
                if (File.Exists(app.IconPath))
                    pictureBoxArt.Image = NesMiniApplication.LoadBitmap(app.IconPath);
                else
                    pictureBoxArt.Image = null;
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

        void LoadHidden()
        {
            checkedListBoxDefaultGames.Items.Clear();
            NesDefaultGame[] games = null;
            switch (ConfigIni.ConsoleType)
            {
                case ConsoleType.NES:
                    games = defaultNesGames;
                    break;
                case ConsoleType.Famicom:
                    games = defaultFamicomGames;
                    break;
                case ConsoleType.SNES:
                    games = defaultSnesGames;
                    break;
                case ConsoleType.SuperFamicom:
                    games = defaultSuperFamicomGames;
                    break;
            }
            foreach (var game in games.OrderBy(o => o.Name))
                checkedListBoxDefaultGames.Items.Add(game, !ConfigIni.HiddenGames.Contains(game.Code));
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
                        ConfigIni.HiddenGames = cols[1];
                        var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        var hide = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 1; j < listViewGames.Items.Count; j++)
                            listViewGames.Items[j].Checked = selected.Contains((listViewGames.Items[j].Tag as NesMiniApplication).Code);
                        for (int j = 0; j < checkedListBoxDefaultGames.Items.Count; j++)
                            checkedListBoxDefaultGames.SetItemChecked(j,
                                !hide.Contains(((NesDefaultGame)checkedListBoxDefaultGames.Items[j]).Code));
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

        private void listViewGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        void SetImageForSelectedGame(string fileName)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
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
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
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

        private void SaveSelectedGames()
        {
            var selected = new List<string>();
            foreach (ListViewItem game in listViewGames.CheckedItems)
            {
                if (game.Tag is NesMiniApplication)
                    selected.Add((game.Tag as NesMiniApplication).Code);
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
                {
                    maxGamesSize = (WorkerForm.NandCFree + WorkerForm.WritedGamesSize) - WorkerForm.ReservedMemory * 1024 * 1024;
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
            {
                MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        bool UploadGames()
        {
            var workerForm = new WorkerForm(this);
            workerForm.Text = Resources.UploadingGames;
            workerForm.Task = WorkerForm.Tasks.UploadGames;
            workerForm.Mod = "mod_hakchi";
            workerForm.Config = ConfigIni.GetConfigDictionary();
            workerForm.Games = new NesMenuCollection();
            bool needOriginal = false;
            foreach (ListViewItem game in listViewGames.CheckedItems)
            {
                if (game.Tag is NesMiniApplication)
                    workerForm.Games.Add(game.Tag as NesMiniApplication);
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
            if (DoNandCFlash())
                MessageBox.Show(Resources.NandDumped, Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void flashNANDCPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.FlashNandCQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                if (RequirePatchedKernel() == DialogResult.No) return;
                if (DoNandCFlash())
                    MessageBox.Show("NAND-C flashed", Resources.Done, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=clusterrr%40clusterrr%2ecom&lc=RU&item_name=Cluster&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
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

        static ConsoleType lastConsoleType = ConsoleType.Unknown;
        public void SyncConsoleType()
        {
            if (lastConsoleType == ConfigIni.ConsoleType) return;
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.NES;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.Famicom;
            sNESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.SNES;
            superFamicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == ConsoleType.SuperFamicom;
            epilepsyProtectionToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.NES || ConfigIni.ConsoleType == ConsoleType.Famicom;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.NES || ConfigIni.ConsoleType == ConsoleType.Famicom;
            upABStartOnSecondControllerToolStripMenuItem.Enabled = ConfigIni.ConsoleType == ConsoleType.Famicom;

            // Some settnigs
            useExtendedFontToolStripMenuItem.Checked = ConfigIni.UseFont;
            epilepsyProtectionToolStripMenuItem.Checked = ConfigIni.AntiArmetLevel > 0 && epilepsyProtectionToolStripMenuItem.Enabled;
            selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.ResetHack;
            enableAutofireToolStripMenuItem.Checked = ConfigIni.AutofireHack;
            useXYOnClassicControllerAsAutofireABToolStripMenuItem.Checked = ConfigIni.AutofireXYHack && useXYOnClassicControllerAsAutofireABToolStripMenuItem.Enabled;
            upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.FcStart && upABStartOnSecondControllerToolStripMenuItem.Enabled;
            compressGamesToolStripMenuItem.Checked = ConfigIni.Compress;

            // Reset known free space
            WorkerForm.NandCTotal = WorkerForm.NandCFree = WorkerForm.NandCUsed = 0;
            if (Clovershell != null && Clovershell.IsOnline)
                new Thread(Clovershell_OnConnected).Start();

            LoadHidden();
            LoadGames();
            lastConsoleType = ConfigIni.ConsoleType;
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

        private void listViewGames_ItemCheck(object sender, ItemCheckEventArgs e)
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
            ConfigIni.RunCount++;
            if (ConfigIni.RunCount == 1)
            {
                new SelectConsoleDialog().ShowDialog();
                SyncConsoleType();
                MessageBox.Show(this, Resources.FirstRun, Resources.Hello, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void compressGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.Compress = compressGamesToolStripMenuItem.Checked;
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
            foreach (var game in defaultSnesGames)
                gameNames[game.Code] = game.Name;
            foreach (var game in defaultSuperFamicomGames)
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
                return ((o1 as NesMiniApplication).Name.CompareTo((o2 as NesMiniApplication).Name));
            }
        }

        bool GroupTaskWithSelected(WorkerForm.Tasks task)
        {
            var workerForm = new WorkerForm(this);
            switch (task)
            {
                case WorkerForm.Tasks.DownloadCovers:
                    workerForm.Text = Resources.DownloadAllCoversTitle;
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

        private void downloadBoxArtForSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.DownloadCovers))
                MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void compressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.CompressGames))
                MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void decompressSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GroupTaskWithSelected(WorkerForm.Tasks.DecompressGames))
                MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
            timerCalculateGames.Enabled = true;
        }

        private void DeleteSelectedGames()
        {
            if (MessageBox.Show(this, Resources.DeleteSelectedGamesQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                if (GroupTaskWithSelected(WorkerForm.Tasks.DeleteGames))
                {
                    foreach (ListViewItem item in listViewGames.SelectedItems)
                        if (item.Tag is NesMiniApplication)
                            listViewGames.Items.Remove(item);
                    //MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else LoadGames();
                timerCalculateGames.Enabled = true;
            }
        }

        private void deleteSelectedGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedGames();
        }

        private void listViewGames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                downloadBoxArtForSelectedGamesToolStripMenuItem.Enabled =
                    compressSelectedGamesToolStripMenuItem.Enabled =
                    decompressSelectedGamesToolStripMenuItem.Enabled =
                    deleteSelectedGamesToolStripMenuItem.Enabled =
                    (listViewGames.SelectedItems.Count > 1) || (listViewGames.SelectedItems.Count == 1 && listViewGames.SelectedItems[0].Tag is NesMiniApplication);
                contextMenuStrip.Show(sender as Control, e.X, e.Y);
            }
        }

        private void listViewGames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && ((listViewGames.SelectedItems.Count > 1) || (listViewGames.SelectedItems.Count == 1 && listViewGames.SelectedItems[0].Tag is NesMiniApplication)))
                DeleteSelectedGames();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;
            var selected = listViewGames.SelectedItems[0].Tag;
            if ((e.KeyCode == Keys.E) && (e.Modifiers == (Keys.Alt | Keys.Control)) && (selected is SnesGame))
            {
                new SnesPresetEditor(selected as SnesGame).ShowDialog();
                ShowSelected();
            }
        }
    }
}
