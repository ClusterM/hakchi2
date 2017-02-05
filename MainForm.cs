﻿using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        public static string BaseDirectory;
        public static string GamesDirectory;
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
            new NesDefaultGame { Code = "CLV-P-HABJJ",  Name = "ファイナルファンタジー®III" },
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
            try
            {
                InitializeComponent();
                ConfigIni.Load();
                BaseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                GamesDirectory = Path.Combine(BaseDirectory, "games");
                KernelDump = Path.Combine(Path.Combine(BaseDirectory, "dump"), "kernel.img");
                LoadGames();
                LoadHidden();
                LoadPresets();
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Text = string.Format("hakchi2 - v{0}.{1:D2}", version.Major, version.Build);

                // Some settnigs
                useExtendedFontToolStripMenuItem.Checked = ConfigIni.UseFont;
                ToolStripMenuItemArmetLevel0.Checked = ConfigIni.AntiArmetLevel == 0;
                ToolStripMenuItemArmetLevel1.Checked = ConfigIni.AntiArmetLevel == 1;
                ToolStripMenuItemArmetLevel2.Checked = ConfigIni.AntiArmetLevel == 2;
                selectButtonCombinationToolStripMenuItem.Enabled = resetUsingCombinationOfButtonsToolStripMenuItem.Checked = ConfigIni.ResetHack;
                enableAutofireToolStripMenuItem.Checked = ConfigIni.AutofireHack;
                removeThumbnailsAtTheBottomToolStripMenuItem.Checked = ConfigIni.RemoveThumbnails;
                betterPNGCompressionlowerQualityToolStripMenuItem.Checked = ConfigIni.EightBitPngCompression;
                nESMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 0;
                famicomMiniToolStripMenuItem.Checked = ConfigIni.ConsoleType == 1;
                upABStartOnSecondControllerToolStripMenuItem.Checked = ConfigIni.FcStart;
                disableMenuMusicToolStripMenuItem.Checked = ConfigIni.DisableMusic;
                foldersAZInsteadOfFullGameNameToolStripMenuItem.Checked = ConfigIni.FoldersAZ;
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

                // Loading games database in background
                new Thread(NesGame.LoadCache).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadGames()
        {
            Debug.WriteLine("Loading games");
            var selected = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Directory.CreateDirectory(GamesDirectory);
            var gameDirs = Directory.GetDirectories(GamesDirectory);
            var games = new List<NesGame>();
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var game = new NesGame(gameDir);
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
            else if (!(selected is NesGame))
            {
                groupBoxDefaultGames.Visible = true;
                groupBoxOptions.Visible = false;
                groupBoxDefaultGames.Enabled = checkedListBoxGames.CheckedIndices.Contains(0);
            }
            else
            {
                var game = selected as NesGame;
                groupBoxDefaultGames.Visible = false;
                groupBoxOptions.Visible = true;
                labelID.Text = "ID: " + game.Code;
                textBoxName.Text = game.Name;
                if (game.Simultaneous && game.Players == 2)
                    radioButtonTwoSim.Checked = true;
                else if (game.Players == 2)
                    radioButtonTwo.Checked = true;
                else
                    radioButtonOne.Checked = true;
                maskedTextBoxReleaseDate.Text = game.ReleaseDate;
                textBoxPublisher.Text = game.Publisher;
                textBoxArguments.Text = game.Args;
                if (File.Exists(game.IconPath))
                    pictureBoxArt.Image = LoadBitmap(game.IconPath);
                else
                    pictureBoxArt.Image = null;
                textBoxGameGenie.Enabled = game.Type == NesGame.GameType.Cartridge;
                textBoxGameGenie.Text = game.GameGenie;
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
                                selected.Contains((checkedListBoxGames.Items[j] as NesGame).Code));
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
                var selected = checkedListBoxGames.SelectedItem;
                if (selected == null || !(selected is NesGame)) return;
                var game = (selected as NesGame);
                game.SetImage(Image.FromFile(openFileDialogImage.FileName), ConfigIni.EightBitPngCompression);
                ShowSelected();
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            var googler = new ImageGooglerForm(game.Name + ImageGooglerForm.Suffix);
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.SetImage(googler.Result, ConfigIni.EightBitPngCompression);
                ShowSelected();
            }
        }

        public static Bitmap LoadBitmap(string path)
        {
            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.Name = textBoxName.Text;
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            game.Args = textBoxArguments.Text;
        }

        private void maskedTextBoxReleaseDate_TextChanged(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
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
                if (game is NesGame)
                    selected.Add((game as NesGame).Code);
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
            foreach (var game in checkedListBoxGames.Items)
            {
                try
                {
                    if (game is NesGame)
                        (game as NesGame).Save();
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

        int RecalculateSelectedGames()
        {
            int c = 0;
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesGame)
                    c++;
                else
                    c += checkedListBoxDefaultGames.CheckedItems.Count;
            }
            toolStripStatusLabelSelected.Text = c + " " + Resources.GamesSelected;
            return c;
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SaveConfig();
            var gamesCount = RecalculateSelectedGames();
            if (gamesCount == 0)
            {
                MessageBox.Show(Resources.SelectAtLeast, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            workerForm.Mod = "mod_kernel";
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
            workerForm.Mod = "mod_transfer";
            workerForm.Config = new Dictionary<string, bool>();
            workerForm.Config["hakchi_enabled"] = true;
            workerForm.Config["hakchi_remove_games"] = true;
            workerForm.Config["hakchi_original_games"] = false;
            workerForm.Config["hakchi_title_font"] = ConfigIni.UseFont;
            workerForm.Config["hakchi_clovercon_hack"] = ConfigIni.ResetHack || ConfigIni.AutofireHack || ConfigIni.FcStart;
            workerForm.Config["hakchi_remove_thumbnails"] = ConfigIni.RemoveThumbnails;
            workerForm.Config["hakchi_remove_music"] = ConfigIni.DisableMusic;
            workerForm.Games = new NesMenuCollection();
            var hiddenGames = new List<string>();
            bool needOriginal = false;
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesGame)
                    workerForm.Games.Add(game as NesGame);
                else
                    needOriginal = true;
            }
            if (needOriginal)
                for (int i = 0; i < checkedListBoxDefaultGames.Items.Count; i++)
                {
                    if (checkedListBoxDefaultGames.CheckedIndices.Contains(i))
                        workerForm.Games.Add((NesDefaultGame)checkedListBoxDefaultGames.Items[i]);
                    else
                        hiddenGames.Add(((NesDefaultGame)checkedListBoxDefaultGames.Items[i]).Code);
                }
            workerForm.Games.Split(ConfigIni.MaxGamesPerFolder);
            workerForm.Config["hakchi_original_games"] = needOriginal;
            if (ConfigIni.AntiArmetLevel == 1)
                workerForm.Config["hakchi_remove_armet_original"] = true;
            else if (ConfigIni.AntiArmetLevel == 2)
                workerForm.Config["hakchi_remove_armet_all"] = true;
            if (needOriginal)
                workerForm.HiddenGames = hiddenGames.ToArray();
            else
                workerForm.HiddenGames = null;
            workerForm.ResetCombination = ConfigIni.ResetHack ? ConfigIni.ResetCombination : (SelectButtonsForm.NesButtons)0xFF;
            workerForm.AutofireHack = ConfigIni.AutofireHack;
            workerForm.FcStart = ConfigIni.FcStart;
            workerForm.ExtraCommandLineArguments = ConfigIni.ExtraCommandLineArguments;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        void AddGames(string[] files)
        {
            SaveConfig();
            MessageBoxManager.Register(); // Tweak button names
            NesGame nesGame = null;
            var workerForm = new WorkerForm();
            workerForm.Text = Resources.LoadingGames;
            workerForm.Task = WorkerForm.Tasks.AddGames;
            if (files.Length <= 1)
                nesGame = workerForm.AddGames(GamesDirectory, files, this);
            else
            {
                workerForm.GamesToAdd = files;
                workerForm.Start();
            }

            LoadGames();
            if (files.Length == 1 && nesGame != null) // if added only one game select it
            {
                for (int i = 1; i < checkedListBoxGames.Items.Count; i++)
                    if ((checkedListBoxGames.Items[i] as NesGame).Code == nesGame.Code)
                    {
                        checkedListBoxGames.SelectedIndex = i;
                        break;
                    }
            }
            MessageBoxManager.Unregister();
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
            workerForm.Config = null;
            workerForm.Games = null;
            workerForm.HiddenGames = null;
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
                if (game is NesGame)
                    workerForm.Games.Add(game as NesGame);
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
            var name = (sender as ToolStripMenuItem).Name;
            ConfigIni.AntiArmetLevel = byte.Parse(name.Substring(name.Length - 1));
            ToolStripMenuItemArmetLevel0.Checked = ConfigIni.AntiArmetLevel == 0;
            ToolStripMenuItemArmetLevel1.Checked = ConfigIni.AntiArmetLevel == 1;
            ToolStripMenuItemArmetLevel2.Checked = ConfigIni.AntiArmetLevel == 2;
        }

        private void cloverconHackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectButtonCombinationToolStripMenuItem.Enabled =
                ConfigIni.ResetHack = resetUsingCombinationOfButtonsToolStripMenuItem.Checked;
        }

        private void removeThumbnailsAtTheBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.RemoveThumbnails = removeThumbnailsAtTheBottomToolStripMenuItem.Checked;
        }

        private void betterPNGCompressionlowerQualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.EightBitPngCompression = betterPNGCompressionlowerQualityToolStripMenuItem.Checked;
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
                MessageBox.Show(this, Resources.AutofireHelp1 + "\r\n" + Resources.AutofireHelp2, enableAutofireToolStripMenuItem.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void disableMenuMusicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.DisableMusic = disableMenuMusicToolStripMenuItem.Checked;
        }

        private void timerCalculateGames_Tick(object sender, EventArgs e)
        {
            RecalculateSelectedGames();
        }

        private void checkedListBoxGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0)
                groupBoxDefaultGames.Enabled = e.NewValue == CheckState.Checked;
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
                var game = checkedListBoxGames.Items[pos] as NesGame;
                if (MessageBox.Show(this, string.Format(Resources.DeleteQ, game.Name), Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
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
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file).ToLower();
                    if (ext == ".nes" || ext == ".fds")
                        e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void checkedListBoxGames_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddGames(files);
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
                    if (g is NesGame)
                    {
                        var game = g as NesGame;
                        if (game.Type == NesGame.GameType.Cartridge)
                        {
                            try
                            {
                                if (game.TryAutofill(new NesFile(game.NesPath).CRC32))
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

        private void buttonShowGameGenieDatabase_Click(object sender, EventArgs e)
        {
            if (!(checkedListBoxGames.SelectedItem is NesGame)) return;
            NesGame nesGame = (NesGame)checkedListBoxGames.SelectedItem;
            GameGenieCodeForm lFrm = new GameGenieCodeForm(nesGame);
            if (lFrm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxGameGenie.Text = nesGame.GameGenie;
        }

        private void foldersAZInsteadOfFullGameNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigIni.FoldersAZ = foldersAZInsteadOfFullGameNameToolStripMenuItem.Checked;
        }
    }
}
