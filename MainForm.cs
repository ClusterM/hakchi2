using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        const long maxTotalSize = 300;

        public static string BaseDirectory;
        //readonly string UBootDump;
        readonly string KernelDump;

        NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros." },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3" },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2" },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong" },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike" },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda" },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure" },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid" },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight" },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link" },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream" },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber" },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus" },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros." },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO" },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics" },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2" },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™" },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®" },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE" },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN" },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga" },
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania" },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS" },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C" },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest" },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN" },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL" },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge" }
        };
        NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ" },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３" },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ" },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング" },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク" },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ" },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説" },
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語" },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド" },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト" },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険" },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー" },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ" },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ" },
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎" },
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®" },
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III" },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン" },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ" },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス" },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅" },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー" },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝" },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵" },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲" },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge" },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語" },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会" },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎" }
        };
       
        public MainForm()
        {
           // try
            {
                InitializeComponent();
                ConfigIni.Load();
                BaseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                KernelDump = Path.Combine(Path.Combine(BaseDirectory, "dump"), "kernel.img");
                LoadGames();
                LoadHidden();
                LoadPresets();
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
                openFileDialogNes.Filter = "Games and apps|" + string.Join(";", extensions.ToArray()) + "|All files|*.*";

                // Loading games database in background
                new Thread(NesGame.LoadCache).Start();
                // Recalculate games in background
                new Thread(RecalculateSelectedGamesThread).Start();
            }
          /*  catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }
        private void FillSystemInformation()
        {
            var selected = GetSelected();
            if(selected == null)
            {
                lblCompression.Text = "";
                lblNbGames.Text = "";
                lblSize.Text = "";
                lblSystemName.Text = "";
            }
            else
            {
                if (selected is NesMiniApplication)
                {
                    lblSystemName.Text = (selected as NesMiniApplication).Console;
                    foreach(TabPage tp in romListers.TabPages)
                    {
                        if(tp.Text == lblSystemName.Text)
                        {
                            lblNbGames.Text = ((CheckedListBox)tp.Controls[0]).CheckedItems.Count.ToString() + " / " + ((CheckedListBox)tp.Controls[0]).Items.Count.ToString() + " games selected";

                            long totalSize = 0;
                            decimal selectedSize = 0;
                            int nbCompressed = 0;
                            for(int x =0;x< ((CheckedListBox)tp.Controls[0]).Items.Count;x++)
                            {
                                object obj = ((CheckedListBox)tp.Controls[0]).Items[x];
                                if (obj is NesMiniApplication)
                                {
                                    NesMiniApplication nma = (obj as NesMiniApplication);
                                    
                                    long currentSize = nma.Size();

                                    totalSize += currentSize;
                                    if(((CheckedListBox)tp.Controls[0]).CheckedIndices.Contains(x))
                                    {
                                        selectedSize += currentSize;
                                        if (nma.isCompressed)
                                        {
                                            nbCompressed++;
                                        }
                                    }
                          
                                }
                                    
                            }
                            lblSize.Text = ((int)(selectedSize/1024/1024)).ToString() + "mB/" + ((int)(totalSize / 1024 / 1024)).ToString() + "mB";
                            lblCompression.Text = nbCompressed.ToString() + "/" + ((CheckedListBox)tp.Controls[0]).CheckedIndices.Count.ToString() + " roms compressed";
                        }
                    }
                }
                else
                {
                    lblSystemName.Text = "Unknow";
                    
                }
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
             //   try
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
              /*  catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }*/
            }

            var gamesSorted = games.OrderBy(o => o.Name);

            string selectedTab = romListers.SelectedTab.Text;
            romListers.TabPages.Clear();
           
          
            TabPage tp = new TabPage("Original");
            CheckedListBox chkLst = new CheckedListBox();
            chkLst.ContextMenuStrip = contextMenuStrip;
            chkLst.Dock = DockStyle.Fill;
            chkLst.SelectedIndexChanged += checkedListBoxGames_SelectedIndexChanged;
            tp.Controls.Add(chkLst);
            romListers.TabPages.Add(tp);

            chkLst.Items.Add(Resources.Default30games, selected.Contains("default"));

            

            
            foreach (var game in gamesSorted)
            {
                AddGameToView(game);

            }


            RecalculateSelectedGames();
            ShowSelected();
        }

     

        private void AddGameToView(NesMiniApplication app)
        {



            string tabName = app.Console;
            bool foundTabPage = false;
            foreach (TabPage tp in romListers.TabPages)
            {
                if (tp.Text == tabName)
                {
                    ((CheckedListBox)tp.Controls[0]).Items.Add(app, true);
                    foundTabPage = true;
                }
            }
            if(!foundTabPage)
            {
                TabPage tp = new TabPage(app.Console);
                CheckedListBox chkLst = new CheckedListBox();
                chkLst.Dock = DockStyle.Fill;
                chkLst.ContextMenuStrip = contextMenuStrip;
                chkLst.SelectedIndexChanged += checkedListBoxGames_SelectedIndexChanged;
                tp.Controls.Add(chkLst);
                chkLst.Items.Add(app, true);
                romListers.TabPages.Add(tp);
            }
           
        }
        private List<NesMiniApplication> GetAllGames()
        {
            List<NesMiniApplication> ret = new List<NesMiniApplication>();
            foreach(TabPage tp in romListers.TabPages)
            {
                foreach(object obj in ((CheckedListBox)tp.Controls[0]).Items)
                {
                   if(obj is NesMiniApplication)
                    {
                        ret.Add(obj as NesMiniApplication);
                    }
                }
            }

            return ret;
        }
        private List<NesMiniApplication> GetSelectedGames()
        {
            List<NesMiniApplication> ret = new List<NesMiniApplication>();
            foreach (TabPage tp in romListers.TabPages)
            {
                foreach (object obj in ((CheckedListBox)tp.Controls[0]).CheckedItems)
                {
                    if (obj is NesMiniApplication)
                    {
                        ret.Add(obj as NesMiniApplication);
                    }
                }
            }

            return ret;
        }
        private object GetSelected()
        {
            return((CheckedListBox)romListers.SelectedTab.Controls[0]).SelectedItem;
        }
       
        public void ShowSelected()
        {
            var selected = GetSelected();
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
                groupBoxDefaultGames.Enabled = true;
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
            FillSystemInformation();
        }

        void LoadHidden()
        {
            checkedListBoxDefaultGames.Items.Clear();
            var hidden = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var game in new List<NesDefaultGame>(ConfigIni.ConsoleType == 0 ? defaultNesGames : defaultFamicomGames).OrderBy(o => o.Name))
                checkedListBoxDefaultGames.Items.Add(game, !hidden.Contains(game.Code));
        }
        public void SetItemCheckedState(string code, bool checkedValue)
        {
            string prefix = code.Substring(code.IndexOf("-") + 1);
            prefix = code.Substring(0, code.IndexOf("-"));
            AppTypeCollection.AppInfo ai = AppTypeCollection.GetAppByPrefix(prefix[0]);
            string tabName = ai.ToString();
            foreach(TabPage tp in romListers.TabPages)
            {
                if(tp.Text == tabName)
                {
                    for(int x =0;x < ((CheckedListBox)tp.Controls[0]).Items.Count;x++)
                    {
                        if(((CheckedListBox)tp.Controls[0]).Items[x] is NesMiniApplication)
                        {
                            NesMiniApplication a = ((CheckedListBox)tp.Controls[0]).Items[x] as NesMiniApplication;
                            if (a.Code == code)
                            {
                                ((CheckedListBox)tp.Controls[0]).SetItemChecked(x, checkedValue);
                            }
                        }
                    }
                    
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
                    delegate(object sender, EventArgs e)
                    {
                        var cols = ConfigIni.Presets[preset].Split('|');
                        ConfigIni.SelectedGames = cols[0];
                        ConfigIni.HiddenGames = cols[1];
                        var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        var hide = ConfigIni.HiddenGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(NesMiniApplication ma in GetAllGames())
                        {
                            SetItemCheckedState(ma.Code, selected.Contains(ma.Code));
                        }
                     
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

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            if (openFileDialogImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selected = GetSelected();
                if (selected == null || !(selected is NesMiniApplication)) return;
                var game = (selected as NesMiniApplication);
                game.Image = NesMiniApplication.LoadBitmap(openFileDialogImage.FileName);
                ShowSelected();
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            var googler = new ImageGooglerForm(game);
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.Image = googler.Result;
                ShowSelected();
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Name = textBoxName.Text;
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            var selected = GetSelected();
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
            var selected = GetSelected();
            if (selected == null || !(selected is NesMiniApplication)) return;
            var game = (selected as NesMiniApplication);
            game.ReleaseDate = maskedTextBoxReleaseDate.Text;
        }

        private void textBoxGameGenie_TextChanged(object sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.GameGenie = textBoxGameGenie.Text;
        }

        private void SaveSelectedGames()
        {
            var selected = new List<string>();
            foreach (var game in GetSelectedGames())
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
            foreach (var game in GetAllGames())
            {
                try
                {
                    if (game is NesMiniApplication)
                        (game as NesMiniApplication).Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Closing main form");
            SaveConfig();
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
            foreach (var game in GetSelectedGames())
            {
                if (game is NesMiniApplication)
                {
                    stats.Count++;
                    stats.Size += (game as NesMiniApplication).Size();
                }
                else
                    stats.Count += checkedListBoxDefaultGames.CheckedItems.Count;
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
                toolStripStatusLabelSelected.Text = stats.Count + " " + Resources.GamesSelected;
                toolStripStatusLabelSize.Text = string.Format("{0}MB / {1}MB", stats.Size / 1024 / 1024, maxTotalSize);
                toolStripProgressBar.Maximum = (int)(maxTotalSize * 1024 * 1024);
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
         /*   if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGames.IndexFromPoint(e.X, e.Y);
                selectAllToolStripMenuItem.Tag = unselectAllToolStripMenuItem.Tag = 0;
                deleteGameToolStripMenuItem.Tag = i;
                deleteGameToolStripMenuItem.Enabled = i > 0;
                contextMenuStrip.Show(sender as Control, e.X, e.Y);
            }*/
        }

        private void checkedListBoxDefaultGames_MouseDown(object sender, MouseEventArgs e)
        {
          /*  if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGames.IndexFromPoint(e.X, e.Y);
                selectAllToolStripMenuItem.Tag = unselectAllToolStripMenuItem.Tag = 1;
                deleteGameToolStripMenuItem.Enabled = false;
                contextMenuStrip.Show(sender as Control, e.X, e.Y);
            }*/
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
            if (stats.Size > maxTotalSize * 1024 * 1024)
            {
                if (MessageBox.Show(string.Format(Resources.MemoryFull, stats.Size / 1024 / 1024) + " " + Resources.DoYouWantToContinue,
                    Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    return;
            }

            bool dumpedKernelNow = false;
            if (!File.Exists(KernelDump))
            {
                if (MessageBox.Show(Resources.NoKernelWarning, Resources.NoKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!DoKernelDump()) return;
                    dumpedKernelNow = true;
                }
                else return;
            }
            if (!ConfigIni.CustomFlashed)
            {
                if (MessageBox.Show((dumpedKernelNow ? (Resources.KernelDumped + "\r\n") : "") +
                    Resources.CustomWarning, Resources.CustomKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!FlashCustomKernel()) return;
                    MessageBox.Show(Resources.DoneYouCanUpload + "\r\n" + Resources.PressOkToContinue, Resources.Congratulations, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else return;
            }
            if (UploadGames())
            {
                MessageBox.Show(Resources.DoneUploaded, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            workerForm.Config = null;
            workerForm.Games = null;
            workerForm.HiddenGames = null;
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
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_hakchi";
            workerForm.Config = new Dictionary<string, string>();
            workerForm.hmodsInstall = new List<string>();
            workerForm.Games = new NesMenuCollection();
            var hiddenGames = new List<string>();
            if (ConfigIni.ResetHack || ConfigIni.AutofireHack || ConfigIni.AutofireXYHack || ConfigIni.FcStart)
            {
                workerForm.hmodsInstall.Add("clovercon");
                workerForm.Config["clovercon_enabled"] = "y";
            }
            else workerForm.Config["clovercon_enabled"] = "n";
            workerForm.Config["clovercon_home_combination"] = string.Format("0x{0:X2}", (byte)ConfigIni.ResetCombination);
            workerForm.Config["clovercon_autofire"] = ConfigIni.AutofireHack ? "1" : "0";
            workerForm.Config["clovercon_autofire_xy"] = ConfigIni.AutofireXYHack ? "1" : "0";
            workerForm.Config["clovercon_fc_start"] = ConfigIni.FcStart ? "1" : "0";
            if (ConfigIni.UseFont)
            {
                workerForm.hmodsInstall.Add("fontfix");
                workerForm.Config["fontfix_enabled"] = "y";
            }
            else workerForm.Config["fontfix_enabled"] = "n";
            bool needOriginal = false;
            foreach (var game in GetSelectedGames())
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
                else
                    hiddenGames.Add(((NesDefaultGame)checkedListBoxDefaultGames.Items[i]).Code);
            }
            workerForm.Config["disable_armet"] = (ConfigIni.AntiArmetLevel > 0) ? "y" : "n";
            workerForm.Config["nes_extra_args"] = ConfigIni.ExtraCommandLineArguments;
            workerForm.HiddenGames = hiddenGames.ToArray();
            workerForm.FoldersMode = ConfigIni.FoldersMode;
            workerForm.MaxGamesPerFolder = ConfigIni.MaxGamesPerFolder;
            workerForm.MainForm = this;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        void AddGames(string[] files)
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
                List<string> oldApp = new List<string>();
                foreach(NesMiniApplication app in GetAllGames())
                {
                    oldApp.Add(app.Code);
                }
                foreach(NesMiniApplication app in addedApps)
                {
                    if(!oldApp.Contains(app.Code))
                    {
                       
                        AddGameToView(app);
                    }
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
            foreach (var game in GetAllGames())
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
            if (!File.Exists(KernelDump))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
            ConfigIni.ConsoleType = 0;
            nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
            famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
            ConfigIni.HiddenGames = "";
            LoadHidden();
        }

        private void famicomMiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            //deleteGame((int)(sender as ToolStripMenuItem).Tag);
        }

        private void deleteGame(int pos)
        {
         /*   try
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
            timerCalculateGames.Enabled = true;*/
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int x =0;x < ((CheckedListBox)romListers.SelectedTab.Controls[0]).Items.Count;x++)
            {
                ((CheckedListBox)romListers.SelectedTab.Controls[0]).SetItemChecked(x, true);
            }
        
        }

        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < ((CheckedListBox)romListers.SelectedTab.Controls[0]).Items.Count; x++)
            {
                ((CheckedListBox)romListers.SelectedTab.Controls[0]).SetItemChecked(x, false);
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
            AddGames(files);
        }


        private void downloadCoversForAllGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DownloadAllCovers())
                MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowSelected();
        }

        private void checkedListBoxGames_KeyDown(object sender, KeyEventArgs e)
        {
           /* if (e.KeyCode == Keys.Delete && checkedListBoxGames.SelectedIndex > 0)
                deleteGame(checkedListBoxGames.SelectedIndex);*/
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
           /* if (e.KeyCode == Keys.F5 && e.Modifiers == Keys.Shift)
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
            }*/
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
            if (!(GetSelected() is NesGame)) return;
            NesGame nesGame = GetSelected() as NesGame;
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
            if (!File.Exists(KernelDump))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var form = new SelectModsForm();
            form.Text = Resources.SelectModsInstall;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (InstallMods(((from m
                                   in form.checkedListBoxMods.CheckedItems.OfType<object>().ToArray()
                                  select m.ToString())).ToArray()))
                {
                    MessageBox.Show(Resources.DoneUploaded, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void uninstallModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(KernelDump))
            {
                MessageBox.Show(Resources.NoKernelYouNeed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var form = new SelectModsForm();
            form.Text = Resources.SelectModsUninstall;
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (UninstallMods(((from m
                                   in form.checkedListBoxMods.CheckedItems.OfType<object>().ToArray()
                                    select m.ToString())).ToArray()))
                {
                    MessageBox.Show(Resources.DoneUploaded, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            var selected = GetSelected();
            int nbCompressed = 0;
            if (selected is NesMiniApplication)
            {
             
                foreach (TabPage tp in romListers.TabPages)
                {
                    if (tp.Text == (selected as NesMiniApplication).Console)
                    {
                        foreach (object o in ((CheckedListBox)tp.Controls[0]).CheckedItems)
                        {
                            if (o is NesMiniApplication)
                            {
                                NesMiniApplication nma = (o as NesMiniApplication);
                                if(!nma.isCompressed)
                                {

                                    nma.Compress();
                                    nbCompressed++;
                                }
                            }
                        }
                      
                    }
                }
            }
               
            


            FillSystemInformation();
        }
    }
}
