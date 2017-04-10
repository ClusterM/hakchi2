using com.clusterrr.clovershell;
using com.clusterrr.hakchi_gui.Properties;
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
    public partial class SaveStateManager : Form
    {
        Dictionary<string, string> gameNames;
        ImagesForm imagesForm = null;
        Point lastPosition = Cursor.Position;

        public SaveStateManager(Dictionary<string, string> gameNames)
        {
            InitializeComponent();
            this.gameNames = gameNames;
            openFileDialog.Filter = saveFileDialog.Filter = Resources.SavesFlterName + "|*.clvs|" + Resources.AllFiles + "|*.*";
            try
            {
                new Thread(LoadSaveStatesList).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

        }

        void LoadSaveStatesList()
        {
            try
            {
                Invoke(new Action(delegate
                {
                    dataGridView.Visible = false;
                    exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled =
                        buttonExport.Enabled = buttonImport.Enabled = false;
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        Close();
                }));

                var clovershell = MainForm.Clovershell;
                WorkerForm.ShowSplashScreen();
                var listSavesScript =
                    @"#!/bin/sh
                     savespath=/var/lib/clover/profiles/0
                     find $savespath -mindepth 1 -maxdepth 1 -type d -name ""CLV-*"" | sed 's#.*/##' | while read code ; do
                       flags=F
                       [ -f $savespath/$code/save.sram ] && flags=${flags}S
                       [ -f $savespath/$code/1.state ] && flags=${flags}1
                       [ -f $savespath/$code/2.state ] && flags=${flags}2
                       [ -f $savespath/$code/3.state ] && flags=${flags}3
                       [ -f $savespath/$code/4.state ] && flags=${flags}4
                       if [ ""$flags"" != ""F"" ]; then
                         size=$(du $savespath/$code | awk '{ print $1 }')
                         name=$(find /var/lib -type f -name ""$code.desktop"" -exec cat {} + | sed -n 's/Name=\(.*\)/\1/p')
                         [ -z ""$name"" ] && name=UNKNOWN
                         echo $code $size $flags $name
                         unset flags
	                     unset name
                       else
                         rm -rf $savespath/$code
                       fi
                     done";
                var listSavesScriptStream = new MemoryStream(Encoding.UTF8.GetBytes(listSavesScript));
                listSavesScriptStream.Seek(0, SeekOrigin.Begin);
                var output = new MemoryStream();
                clovershell.Execute("sh", listSavesScriptStream, output, null, 10000, true);
                var lines = Encoding.UTF8.GetString(output.ToArray()).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Invoke(new Action(delegate
                {
                    dataGridView.Rows.Clear();
                    foreach (var line in lines)
                    {
                        var l = line;
                        var code = l.Substring(0, l.IndexOf(' '));
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var size = int.Parse(l.Substring(0, l.IndexOf(' ')));
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var flags = l.Substring(0, l.IndexOf(' ')).Replace("F", "");
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var name = l;
                        if (name == "UNKNOWN")
                        {
                            if (gameNames.ContainsKey(code))
                                name = gameNames[code];
                            else
                                name = Resources.UnknownGame;
                        }
                        var r = dataGridView.Rows.Add();
                        dataGridView.Rows[r].Cells["colCode"].Value = code;
                        dataGridView.Rows[r].Cells["colName"].Value = name;
                        dataGridView.Rows[r].Cells["colSize"].Value = size;
                        dataGridView.Rows[r].Cells["colFlags"].Value = flags;
                    }
                    dataGridView.Visible = true;
                    importToolStripMenuItem.Enabled = true;
                    buttonImport.Enabled = true;
                }));
            }
            catch (ThreadAbortException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                try
                {
                    Invoke(new Action(delegate
                    {
                        MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }));
                }
                catch { }
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            exportToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled =
                buttonExport.Enabled = buttonDelete.Enabled = dataGridView.SelectedRows.Count > 0;
            if (dataGridView.SelectedRows.Count > 0)
            {
                var size = 0;
                foreach (DataGridViewRow game in dataGridView.SelectedRows)
                    if (game.Cells["colSize"].Value != null)
                        size += (int)game.Cells["colSize"].Value;
                toolStripStatusLabelSize.Text = string.Format(Resources.SizeOfSaves, size);
            }
            else toolStripStatusLabelSize.Text = "";
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Resources.DeleteSavesQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != System.Windows.Forms.DialogResult.Yes)
                return;
            var savesToDelete = dataGridView.SelectedRows;
            new Thread(deleteRowsThread).Start(savesToDelete);
        }

        void deleteRowsThread(object o)
        {
            try
            {
                var savesToDelete = (DataGridViewSelectedRowCollection)o;
                Invoke(new Action(delegate
                {
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        return;
                }));
                foreach (DataGridViewRow game in savesToDelete)
                {
                    var clovershell = MainForm.Clovershell;
                    clovershell.ExecuteSimple("rm -rf /var/lib/clover/profiles/0/" + game.Cells["colCode"].Value, 3000, true);
                    Invoke(new Action(delegate
                    {
                        dataGridView.Rows.Remove(game);
                    }));
                }
            }
            catch (ThreadAbortException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                try
                {
                    Invoke(new Action(delegate
                    {
                        MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                catch { }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow game in dataGridView.SelectedRows)
                {
                    saveFileDialog.FileName = game.Cells["colName"].Value + ".clvs";
                    var name = game.Cells["colName"].Value != null ? game.Cells["colName"].Value.ToString() : "save";
                    saveFileDialog.Title = name;
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!WaitingClovershellForm.WaitForDevice(this))
                            return;
                        var clovershell = MainForm.Clovershell;
                        using (var save = new MemoryStream())
                        {
                            clovershell.Execute("cd /var/lib/clover/profiles/0 && tar -cz " + game.Cells["colCode"].Value, null, save, null, 10000, true);
                            var buffer = save.ToArray();
                            File.WriteAllBytes(saveFileDialog.FileName, buffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                try
                {
                    Invoke(new Action(delegate
                    {
                        MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                catch { }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = buttonImport.Text;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var files = openFileDialog.FileNames;
                new Thread(importSaves).Start(files);
            }
        }

        void importSaves(object o)
        {
            try
            {
                var files = (string[])o;
                Invoke(new Action(delegate
                {
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        return;
                }));
                foreach (var file in files)
                {
                    var clovershell = MainForm.Clovershell;
                    using (var f = new FileStream(file, FileMode.Open))
                    {
                        clovershell.Execute("cd /var/lib/clover/profiles/0 && tar -xvz", f, null, null, 10000, true);
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                try
                {
                    Invoke(new Action(delegate
                    {
                        MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                catch { }
            }
            LoadSaveStatesList();
        }

        private void SaveStateManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (imagesForm != null)
                imagesForm.Dispose();
            try
            {
                var clovershell = MainForm.Clovershell;
                if (clovershell.IsOnline)
                    clovershell.ExecuteSimple("reboot", 100);
            }
            catch { }
        }

        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            timerCellHover.Enabled = true;
        }

        private void dataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            timerCellHover.Enabled = false;
            if (imagesForm != null)
                imagesForm.Hide();
        }

        private void dataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lastPosition.Equals(Cursor.Position)) return;
            lastPosition = Cursor.Position;
            if (imagesForm != null)
                imagesForm.Hide();
            timerCellHover.Enabled = false;
            timerCellHover.Enabled = true;
        }

        private void timerCellHover_Tick(object sender, EventArgs e)
        {
            timerCellHover.Enabled = false;
            if (!MainForm.Clovershell.IsOnline) return;
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                var rect = dataGridView.RectangleToScreen(dataGridView.GetRowDisplayRectangle(i, true));
                if (Cursor.Position.Y >= rect.Top && Cursor.Position.Y <= rect.Bottom)
                {
                    var row = dataGridView.Rows[i];
                    if (row.Cells["colFlags"].Value.ToString().Equals("S")) return; // No images?
                    new Thread(loadImagesThread).Start(row.Cells["colCode"].Value);
                }
            }
        }

        // Mouse on cell? Show some screenshots to remember game!
        void loadImagesThread(object s)
        {
            var code = s.ToString();
            try
            {
                var clovershell = MainForm.Clovershell;
                var images = new List<Image>();
                using (var save = new MemoryStream())
                {
                    clovershell.Execute("cd /var/lib/clover/profiles/0 && tar -cz " + code, null, save, null, 10000, true);
                    save.Seek(0, SeekOrigin.Begin);
                    SevenZipExtractor.SetLibraryPath(Path.Combine(MainForm.BaseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(save))
                    {
                        var tar = new MemoryStream();
                        szExtractor.ExtractFile(0, tar);
                        tar.Seek(0, SeekOrigin.Begin);
                        using (var szExtractorTar = new SevenZipExtractor(tar))
                        {
                            foreach (var f in szExtractorTar.ArchiveFileNames)
                            {
                                if (Path.GetExtension(f).ToLower() == ".png")
                                {
                                    var o = new MemoryStream();
                                    szExtractorTar.ExtractFile(f, o);
                                    o.Seek(0, SeekOrigin.Begin);
                                    images.Add(Image.FromStream(o));
                                }
                            }
                        }
                    }

                }
                Debug.WriteLine("Loaded " + images.Count + " imags");
                Invoke(new Action(delegate
                {
                    // Maybe it's too late?
                    string name = null;
                    for (int i = 0; i < dataGridView.RowCount; i++)
                    {
                        var rect = dataGridView.RectangleToScreen(dataGridView.GetRowDisplayRectangle(i, true));
                        if (Cursor.Position.Y >= rect.Top && Cursor.Position.Y <= rect.Bottom)
                        {
                            var row = dataGridView.Rows[i];
                            if (row.Cells["colCode"].Value.ToString() != code)
                                return;
                            name = row.Cells["colName"].Value.ToString();
                        }
                    }
                    if (name == null) return; // No rows at all

                    if (imagesForm == null)
                        imagesForm = new ImagesForm();
                    imagesForm.Left = Cursor.Position.X + 5;
                    imagesForm.Top = Cursor.Position.Y + 5;
                    imagesForm.Text = name;
                    imagesForm.ShowImages(images);
                    imagesForm.Show();
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}
