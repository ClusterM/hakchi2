using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class MainForm : Form
    {
        readonly string BaseDir;
        readonly string GamesDir;
        //readonly string UBootDump;
        readonly string KernelDump;

        public MainForm()
        {
            InitializeComponent();
            BaseDir = Path.GetDirectoryName(Application.ExecutablePath);
            GamesDir = Path.Combine(BaseDir, "games");
            //UBootDump = Path.Combine(Path.Combine(BaseDir, "dump"), "uboot.bin");
            KernelDump = Path.Combine(Path.Combine(BaseDir, "dump"), "kernel.img");
            LoadGames();
        }

        public void LoadGames()
        {
            var selected = Settings.Default.SelectedGames.Split(';');
            Directory.CreateDirectory(GamesDir);
            var gameDirs = Directory.GetDirectories(GamesDir);
            var games = new List<NesGame>();
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    var game = new NesGame(gameDir);
                    games.Add(game);
                }
                catch (Exception ex)
                {
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
            if (selected == null || !(selected is NesGame))
            {
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
            else
            {
                var game = selected as NesGame;
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
                groupBoxOptions.Enabled = true;
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
                game.SetImage(Image.FromFile(openFileDialogImage.FileName));
                ShowSelected();
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            var selected = checkedListBoxGames.SelectedItem;
            if (selected == null || !(selected is NesGame)) return;
            var game = (selected as NesGame);
            var googler = new ImageGooglerForm(game.Name + " nes box art");
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.SetImage(googler.Result);
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
            Settings.Default.SelectedGames = string.Join(";", selected.ToArray());
            Settings.Default.Save();
        }

        private void SaveConfig()
        {
            SaveSelectedGames();
            foreach (var game in checkedListBoxGames.Items)
            {
                try
                {
                    if (game is NesGame)
                        (game as NesGame).Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }

        int RecalculateSelectedGames(int i = -1, bool v = false)
        {
            int c = 0;
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesGame)
                    c++;
                else
                    c += 30;
            }
            if (i == 0)
            {
                if (v) c += 30;
                else c -= 30;
            }
            else if (i > 0)
            {
                if (v) c += 1;
                else c -= 1;
            }
            toolStripStatusLabelSelected.Text = c + " " + Resources.GamesSelected;
            return c;
        }

        private void checkedListBoxGames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            RecalculateSelectedGames(e.Index, e.NewValue == CheckState.Checked);
        }

        private void buttonAddGames_Click(object sender, EventArgs e)
        {
            if (openFileDialogNes.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveConfig();
                NesGame nesGame = null;
                foreach (var file in openFileDialogNes.FileNames)
                {
                    try
                    {
                        try
                        {
                            nesGame = new NesGame(GamesDir, file);
                        }
                        catch (UnsupportedMapperException ex)
                        {
                            if (MessageBox.Show(this, string.Format(Resources.MapperNotSupported, Path.GetFileName(file), ex.ROM.Mapper), Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                == System.Windows.Forms.DialogResult.Yes)
                                nesGame = new NesGame(GamesDir, file, true);
                            else continue;
                        }
                        Settings.Default.SelectedGames += ";" + nesGame.Code;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                LoadGames();
                if (openFileDialogNes.FileNames.Length == 1)
                {
                    for (int i = 1; i < checkedListBoxGames.Items.Count; i++)
                        if ((checkedListBoxGames.Items[i] as NesGame).Code == nesGame.Code)
                        {
                            checkedListBoxGames.SelectedIndex = i;
                            break;
                        }
                }
            }
        }

        private void deleteGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var game = checkedListBoxGames.Items[(int)(sender as ToolStripMenuItem).Tag] as NesGame;
                if (MessageBox.Show(this, string.Format(Resources.DeleteQ, game.Name), Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveConfig();
                    Directory.Delete(game.GamePath, true);
                    LoadGames();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkedListBoxGames_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var i = checkedListBoxGames.IndexFromPoint(e.X, e.Y);
                deleteGameToolStripMenuItem.Tag = i;
                if (i > 0)
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
            /*
            if (gamesCount > 97)
            {
                if (MessageBox.Show(Resources.ManyGames, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                return;
            }
             */
            if (/*!File.Exists(UBootDump) ||*/ !File.Exists(KernelDump))
            {
                if (MessageBox.Show(Resources.NoKernelWarning, Resources.NoKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!DoKernelDump()) return;
                }
                else return;
            }
            if (!Settings.Default.CustomFlashed)
            {
                if (MessageBox.Show(Resources.CustomWarning, Resources.CustomKernel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
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
            workerForm.Task = WorkerForm.Tasks.DumpKernel;
            //workerForm.UBootDump = UBootDump;
            workerForm.KernelDump = KernelDump;
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool FlashCustomKernel()
        {
            var workerForm = new WorkerForm();
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_kernel";
            workerForm.Start();
            var result = workerForm.DialogResult == DialogResult.OK;
            if (result)
            {
                Settings.Default.CustomFlashed = true;
                Settings.Default.Save();
            }
            return result;
        }

        bool UploadGames()
        {
            var workerForm = new WorkerForm();
            workerForm.Task = WorkerForm.Tasks.Memboot;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = "mod_transfer";
            workerForm.OriginalGames = false;
            var games = new List<NesGame>();
            foreach (var game in checkedListBoxGames.CheckedItems)
            {
                if (game is NesGame)
                    games.Add(game as NesGame);
                else
                    workerForm.OriginalGames = true;
            }
            workerForm.Games = games.ToArray();
            workerForm.Start();
            return workerForm.DialogResult == DialogResult.OK;
        }

        bool FlashOriginalKernel()
        {
            var workerForm = new WorkerForm();
            workerForm.Task = WorkerForm.Tasks.FlashKernel;
            workerForm.KernelDump = KernelDump;
            workerForm.Mod = null;
            workerForm.Start();
            var result = workerForm.DialogResult == DialogResult.OK;
            if (result)
            {
                Settings.Default.CustomFlashed = false;
                Settings.Default.Save();
            }
            return result;
        }

        private void dumpKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.DumpKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                if (!File.Exists(KernelDump) || MessageBox.Show(Resources.ReplaceKernelQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    if (DoKernelDump()) MessageBox.Show(Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.ShowDialog();
        }
    }
}
