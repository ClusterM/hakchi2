using com.clusterrr.hakchi_gui.Apps;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class CustomBorderManager : Form
    {

        static DefaultBorder[] defaultBorders = new DefaultBorder[] {
            new DefaultBorder { Name = "01_ambient"},
            new DefaultBorder { Name = "02_wire"},
            new DefaultBorder { Name = "03_crystal"},
            new DefaultBorder { Name = "04_dot"},
            new DefaultBorder { Name = "05_mosaic"},
            new DefaultBorder { Name = "06_dot2"},
            new DefaultBorder { Name = "07_wood"},
            new DefaultBorder { Name = "08_space"},
            new DefaultBorder { Name = "09_speaker"},
            new DefaultBorder { Name = "10_curtain"},
            new DefaultBorder { Name = "11_midnight"}
        };

        public CustomBorderManager()
        {
            InitializeComponent();

            var selected = ConfigIni.SelectedBorders.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            LoadHidden();
            LoadBorders();

        }

        public void LoadBorders()
        {
            Debug.WriteLine("Loading borders");
            var selected = ConfigIni.SelectedBorders.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Directory.CreateDirectory(BorderElement.BackgroundsDirectory);
            var borderDirs = Directory.GetDirectories(BorderElement.BackgroundsDirectory);
            var borders = new List<BorderElement>();
            foreach (var borderDir in borderDirs)
            {
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var border = BorderElement.FromDirectory(borderDir);
                        borders.Add((BorderElement)border);
                    }
                    catch (FileNotFoundException ex) // Remove bad directories if any
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        Directory.Delete(borderDir, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }

            var bordersSorted = borders.OrderBy(o => o.Name);
            listViewBorders.Items.Clear();
            var listViewItem = new ListViewItem("Default borders");// Resources.Default30games);
            listViewItem.Tag = "default";
            listViewItem.Checked = selected.Contains("default");
            listViewBorders.Items.Add(listViewItem);
            foreach (var border in bordersSorted)
            {
                listViewItem = new ListViewItem(border.Name);
                listViewItem.Tag = border;
                listViewItem.Checked = selected.Contains(border.Name);
                listViewBorders.Items.Add(listViewItem);
            }
            /*
            RecalculateSelectedGames();*/
            ShowSelected();
        }

        private List<string> generateFileList(string base_name)
        {
            List<string> lfiles = new List<string>();
            lfiles.Add(base_name + "_4_3.png");
            lfiles.Add(base_name + "_4_3_options.txt");
            lfiles.Add(base_name + "_4_3_preview.png");
            lfiles.Add(base_name + "_pixel_perfect.png");
            lfiles.Add(base_name + "_pixel_perfect_options.txt");
            lfiles.Add(base_name + "_pixel_perfect_preview.png");
            lfiles.Add(base_name + "_thumbnail.png");
            return lfiles;
        }

        private void AddBorderZip(string[] files)
        {
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                var basename = Path.GetFileNameWithoutExtension(file);
                if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(file))
                    {
                        szExtractor.PreserveDirectoryStructure = false;
                        var filesInArchive = new List<string>();
                        var gameFilesInArchive = new List<string>();
                        List<string> lfiles = generateFileList(basename);
                        int lcount = 0;
                        foreach (var f in szExtractor.ArchiveFileNames)
                        {
                            if (lfiles.Contains(f))
                                lcount++;
                            else
                            {
                                foreach (var ff in lfiles)
                                {
                                    if (f.EndsWith(ff))
                                        lcount++;
                                }
                            }
                            filesInArchive.Add(f);
                        }
                        if (lcount == lfiles.Count)
                        {
                            string tmp = Path.Combine(BorderElement.BackgroundsDirectory, basename);
                            Directory.CreateDirectory(tmp);
                            szExtractor.ExtractArchive(tmp);

                            BorderElement border = new BorderElement(tmp);
                            bool found = false;
                            foreach (var i in listViewBorders.Items)
                            {
                                if (((ListViewItem)(i)).Text == border.Name)
                                    found = true;
                            }
                            if (!found)
                            {
                                ListViewItem listViewItem = new ListViewItem(border.Name);
                                listViewItem.Tag = border;
                                listViewItem.Checked = true;
                                listViewBorders.Items.Add(listViewItem);
                            }
                        }
                    }

                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (openFileDialogZipBorder.ShowDialog() == DialogResult.OK)
            {
                var files = openFileDialogZipBorder.FileNames;
                if (files == null) return;

                AddBorderZip(files);
            }
        }

        private void deleteBorder()
        {
            if (MessageBox.Show(this, "Delete border", Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                if (listViewBorders.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in listViewBorders.SelectedItems)
                    {
                        if (item.Tag is BorderElement)
                        {
                            try
                            {
                                Directory.Delete(((BorderElement)(item.Tag)).Path, true);
                                listViewBorders.Items.Remove(item);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("delete process failed: {0}", ex.Message);
                            }
                        }
                        //MessageBox.Show(this, Resources.Done, Resources.Wow, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            deleteBorder();
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Debug.Write("Copying " + target.FullName + "/" + fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        private void buttonSync_Click(object sender, EventArgs e)
        {
            const string src_backgroundPath = "/var/lib/hakchi/squashfs/usr/share/backgrounds";
            const string dest_backgroundPath = "/var/lib/hakchi/rootfs/usr/share/backgrounds";
            var clovershell = MainForm.Clovershell;
            string bgDirectory = BorderElement.BackgroundsDirectory;
            SaveSelectedBorders();
            ConfigIni.Save();
            string baseDirectoryInternal = Program.BaseDirectoryInternal;
            string tempDirectory;
#if DEBUG
            tempDirectory = Path.Combine(baseDirectoryInternal, "temp");
#else
            tempDirectory = Path.Combine(Path.GetTempPath(), "hakchi-temp");
#endif
            string tempBordersDirectory = Path.Combine(tempDirectory, "borders");
            Directory.CreateDirectory(tempDirectory);
            Directory.CreateDirectory(tempBordersDirectory);
            labelSyncing.Text = "Making sync";
            progressBarSync.Value = 0;
            labelSyncing.Visible = true;
            progressBarSync.Visible = true;
            labelSyncing.Refresh();
            progressBarSync.Refresh();

            if (clovershell.IsOnline)
            {
                this.Enabled = false;
                try
                {
                    /*labelSyncing.Text = "Deleting a file";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 10;
                    progressBarSync.Refresh();
                    string del_cmd = $"rm /etc/preinit.d/p8173_ownbgs";
                    clovershell.ExecuteSimple(del_cmd, 3000);
                    Thread.Sleep(1000);
                    labelSyncing.Text = "Reboot...";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 20;
                    progressBarSync.Refresh();
                    clovershell.ExecuteSimple("reboot", 3000);
                    Thread.Sleep(2000);
                    labelSyncing.Text = "Waiting reboot...";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 30;
                    progressBarSync.Refresh();
                    while (!(clovershell.IsOnline))
                    {
                        Debug.WriteLine("Wait reboot");
                        Thread.Sleep(2000);
                    }
                    */
                    labelSyncing.Text = "Creating directory...";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 10;
                    progressBarSync.Refresh();
                    string command = $"src=\"{src_backgroundPath}\" && " +
                                    $"dst=\"{dest_backgroundPath}\" && " +
                                    $"rm -rf \"$dst\" && " + 
                                    $"mkdir -p \"$dst\"";
                    clovershell.ExecuteSimple(command, 10000, true);

                    foreach (ListViewItem border in listViewBorders.CheckedItems)
                    {
                        if (border.Tag is BorderElement)
                        {
                            string repName = (border.Tag as BorderElement).Name;
                            DirectoryInfo diSource = new DirectoryInfo(bgDirectory + "/" + repName);
                            DirectoryInfo diTarget = new DirectoryInfo(tempBordersDirectory + "/" + repName);
                            CopyAll(diSource, diTarget);
                        }
                    }
                    string[] lines = { "overmount /usr/share/backgrounds/", "" };
                    System.IO.File.WriteAllLines(tempBordersDirectory + "/" + "p8173_ownbgs", lines);

                    labelSyncing.Text = "Uploading user borders...";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 20;
                    progressBarSync.Refresh();

                    using (var bordersTar = new TarStream(tempBordersDirectory))
                    {

                        if (bordersTar.Length > 0)
                        {
                            bordersTar.OnReadProgress += delegate (long pos, long len)
                            {
                                Debug.WriteLine("SetProgress(" + pos.ToString() + ", " + len.ToString());
                            };

                            clovershell.Execute(string.Format("tar -xvC {0}", dest_backgroundPath), bordersTar, null, null, 30000, true);
                        }
                    }

                    string move_cmd = $"mv {dest_backgroundPath}/p8173_ownbgs /etc/preinit.d/";
                    clovershell.ExecuteSimple(move_cmd, 3000, true);

                    labelSyncing.Text = "Managing default borders...";
                    labelSyncing.Refresh();
                    progressBarSync.Value = 80;
                    progressBarSync.Refresh();

                    if (listViewBorders.Items[0].Checked)
                    {
                        var selected = new List<string>();
                        foreach (DefaultBorder border in checkedListBoxDefaultBorders.CheckedItems)
                            selected.Add(border.Name);
                        foreach (string bordername in selected)
                        {
                            string cmd = $"src=\"{src_backgroundPath}/{bordername}\" && " +
                                            $"dst=\"{dest_backgroundPath}/{bordername}\" && " +
                                            $"ln -s \"$src\" \"$dst\"";
                            clovershell.ExecuteSimple(cmd, 10000, true);
                        }
                    }
                }
                finally
                {
                    try
                    {
                        labelSyncing.Text = "Reboot...";
                        labelSyncing.Refresh();
                        progressBarSync.Value = 100;
                        progressBarSync.Refresh();
                        Directory.Delete(tempDirectory, true);
                        if (clovershell.IsOnline)
                            clovershell.ExecuteSimple("reboot", 100);

                    }
                    catch { }
                    MessageBox.Show(this, "work is done", "Custom borders", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Enabled = true;
                    this.Close();
                }
            }
        }

        public void ShowSelected()
        {
            object selected = null;
            var selectedAll = listViewBorders.SelectedItems;
            if (selectedAll.Count == 1) selected = selectedAll[0].Tag;
            if (selected == null)
            {
                groupBoxDefaultBorders.Visible = false;
                groupBoxOptions.Visible = true;
                groupBoxOptions.Enabled = false;
                textBoxName.Text = "";
                pictureBoxArt.Image = null;
            }
            else if (!(selected is BorderElement))
            {
                groupBoxDefaultBorders.Visible = true;
                groupBoxOptions.Visible = false;
                groupBoxDefaultBorders.Enabled = listViewBorders.CheckedIndices.Contains(0);
            }
            else
            {
                var app = selected as BorderElement;
                groupBoxDefaultBorders.Visible = false;
                groupBoxOptions.Visible = true;
                textBoxName.Text = app.Name;
                if (File.Exists(app.IconPath))
                    pictureBoxArt.Image = BorderElement.LoadBitmap(app.IconPath);
                else
                    pictureBoxArt.Image = null;
                groupBoxOptions.Enabled = true;
            }
        }

        void LoadHidden()
        {
            checkedListBoxDefaultBorders.Items.Clear();
            DefaultBorder[] borders = defaultBorders;
            foreach (var border in borders.OrderBy(o => o.Name))
                checkedListBoxDefaultBorders.Items.Add(border, !ConfigIni.HiddenGamesBorders.Contains(border.Name));
        }

        private void listViewBorders_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelected();
        }

        private void SaveSelectedBorders()
        {
            var selected = new List<string>();
            foreach (ListViewItem border in listViewBorders.CheckedItems)
            {
                if (border.Tag is BorderElement)
                    selected.Add((border.Tag as BorderElement).Name);
                else
                    selected.Add("default");
            }
            ConfigIni.SelectedBorders = string.Join(";", selected.ToArray());
            selected.Clear();

            foreach (DefaultBorder border in checkedListBoxDefaultBorders.Items)
                selected.Add(border.Name);
            foreach (DefaultBorder border in checkedListBoxDefaultBorders.CheckedItems)
                selected.Remove(border.Name);
            ConfigIni.HiddenGamesBorders = string.Join(";", selected.ToArray());
        }

        private void CustomBorderManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSelectedBorders();
            ConfigIni.Save();
        }

        private void CustomBorderManager_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;
            AddBorderZip(files);
        }

        private void CustomBorderManager_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            var clovershell = MainForm.Clovershell;
            const string dest_backgroundPath = "/var/lib/hakchi/rootfs/usr/share/backgrounds";
            if (clovershell.IsOnline)
            {
                this.Enabled = false;
                try
                {
                    string del_cmd = $"rm /etc/preinit.d/p8173_ownbgs";
                    clovershell.ExecuteSimple(del_cmd, 3000);
                    Thread.Sleep(1000);
                    string command = $"dst=\"{dest_backgroundPath}\" && " +
                                    $"rm -rf \"$dst/\"";
                    clovershell.ExecuteSimple(command, 10000, true);
                }
                finally
                {
                    try
                    {
                        if (clovershell.IsOnline)
                            clovershell.ExecuteSimple("reboot", 100);
                    }
                    catch { }
                    MessageBox.Show(this, "work is done", "Custom borders", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Enabled = true;
                }
            }
        }

        private void listViewBorders_KeyDown(object sender, KeyEventArgs e)
        {
            if (listViewBorders.SelectedItems.Count == 0) return;
            if (e.KeyCode == Keys.Delete && ((listViewBorders.SelectedItems.Count > 1) || (listViewBorders.SelectedItems.Count == 1 && listViewBorders.SelectedItems[0].Tag is BorderElement)))
            {
                deleteBorder();
            }
        }
    }
}
