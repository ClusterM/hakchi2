using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            labelLoading.Text = Resources.PleaseWait;
            this.gameNames = gameNames;
            openFileDialog.Filter = saveFileDialog.Filter = Resources.SavesFlterName + " (*.clvs)|*.clvs|" + Resources.AllFiles + "|*.*";
            try
            {
                new Thread(LoadSaveStatesList).Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        void LoadSaveStatesList()
        {
            try
            {
                while (!this.IsHandleCreated) Thread.Sleep(100);
                if (!(bool)Invoke(new Func<bool>(delegate
                {
                    listViewSaves.Visible = false;
                    exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled =
                        buttonExport.Enabled = buttonImport.Enabled = false;
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        return false;
                    return true;
                })))
                {
                    Close();
                    return;
                }

                var clovershell = MainForm.Clovershell;
                WorkerForm.ShowSplashScreen();
                var listSavesScript =
                     "#!/bin/sh\n" +
                     "savespath=/var/lib/clover/profiles/0\n" +
                     "find $savespath -mindepth 1 -maxdepth 1 -type d -name \"CLV-*\" | sed 's#.*/##' | while read code ; do\n" +
                     "  flags=F\n" +
                     "  [ -f $savespath/$code/save.sram ] && flags=${flags}-S\n" +
                     "  [ -f $savespath/$code/cartridge.sram ] && [ $(wc -c <$savespath/$code/cartridge.sram) -gt 20 ] && flags=${flags}-S\n" +
                     "  [ -f $savespath/$code/1.state ] && flags=${flags}-1\n" +
                     "  [ -d $savespath/$code/suspendpoint1  ] && flags=${flags}-1\n" +
                     "  [ -f $savespath/$code/2.state ] && flags=${flags}-2\n" +
                     "  [ -d $savespath/$code/suspendpoint2 ] && flags=${flags}-2\n" +
                     "  [ -f $savespath/$code/3.state ] && flags=${flags}-3\n" +
                     "  [ -d $savespath/$code/suspendpoint3 ] && flags=${flags}-3\n" +
                     "  [ -f $savespath/$code/4.state ] && flags=${flags}-4\n" +
                     "  [ -d $savespath/$code/suspendpoint4 ] && flags=${flags}-4\n" +
                     "  if [ \"$flags\" != \"F\" ]; then\n" +
                     "    size=$(du -d 0 $savespath/$code | awk '{ print $1 }')\n" +
                     "    name=$(find /var/lib -type f -name \"$code.desktop\" -exec cat {} + | sed -n 's/Name=\\(.*\\)/\\1/p' | head -n 1)\n" +
                     "    [ -z \"$name\" ] && name=UNKNOWN\n" +
                     "    echo $code $size $flags $name\n" +
                     "    unset flags\n" +
                     "    unset name\n" +
                     "  else\n" +
                     "    rm -rf $savespath/$code\n" +
                     "  fi\n" +
                     "done";
                var listSavesScriptStream = new MemoryStream(Encoding.UTF8.GetBytes(listSavesScript));
                listSavesScriptStream.Seek(0, SeekOrigin.Begin);
                var output = new MemoryStream();
                clovershell.Execute("sh", listSavesScriptStream, output, null, 10000, true);
                var lines = Encoding.UTF8.GetString(output.ToArray()).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Invoke(new Action(delegate
                {
                    listViewSaves.Items.Clear();
                    foreach (var line in lines)
                    {
                        var l = line;
                        var code = l.Substring(0, l.IndexOf(' '));
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var size = l.Substring(0, l.IndexOf(' ')) + "KB";
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var flags = l.Substring(0, l.IndexOf(' ')).Replace("F-", "").Replace("-", " ").Trim();
                        l = l.Substring(l.IndexOf(' ') + 1);
                        var name = l;
                        if (name == "UNKNOWN")
                        {
                            if (gameNames.ContainsKey(code))
                                name = gameNames[code];
                            else
                                name = Resources.UnknownGame;
                        }
                        listViewSaves.Items.Add(new ListViewItem(new ListViewItem.ListViewSubItem[] {
                            new  ListViewItem.ListViewSubItem() { Name = "colName", Text = name},
                            new  ListViewItem.ListViewSubItem() { Name = "colCode", Text = code},
                            new  ListViewItem.ListViewSubItem() { Name = "colSize", Text = size},
                            new  ListViewItem.ListViewSubItem() { Name = "colFlags", Text = flags}                           
                        }, 0));
                        listViewSaves.ListViewItemSorter = new SavesSorter(0, false);
                        listViewSaves.Sort();
                    }
                    listViewSaves.Visible = true;
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Resources.DeleteSavesQ, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != System.Windows.Forms.DialogResult.Yes)
                return;
            var savesToDelete = new List<ListViewItem>();
            foreach (ListViewItem item in listViewSaves.SelectedItems)
                savesToDelete.Add(item);
            new Thread(deleteRowsThread).Start(savesToDelete);
        }

        void deleteRowsThread(object o)
        {
            try
            {
                var savesToDelete = (IEnumerable<ListViewItem>)o;
                if (!(bool)Invoke(new Func<bool>(delegate
                {
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        return false;
                    return true;
                }))) return;
                foreach (ListViewItem game in savesToDelete)
                {
                    var clovershell = MainForm.Clovershell;
                    clovershell.ExecuteSimple("rm -rf /var/lib/clover/profiles/0/" + game.SubItems["colCode"].Text, 3000, true);
                    Invoke(new Action(delegate
                    {
                        listViewSaves.Items.Remove(game);
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
                foreach (ListViewItem game in listViewSaves.SelectedItems)
                {
                    saveFileDialog.FileName = game.SubItems["colName"].Text + ".clvs";
                    var name = game.SubItems["colName"].Text != null ? game.SubItems["colName"].Text : "save";
                    saveFileDialog.Title = name;
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!WaitingClovershellForm.WaitForDevice(this))
                            return;
                        var clovershell = MainForm.Clovershell;
                        using (var save = new MemoryStream())
                        {
                            clovershell.Execute("cd /var/lib/clover/profiles/0 && tar -cz " + game.SubItems["colCode"].Text, null, save, null, 10000, true);
                            var buffer = save.ToArray();
                            File.WriteAllBytes(saveFileDialog.FileName, buffer);
                        }
                    }
                    else break;
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
                listViewSaves.Visible = false;
                buttonDelete.Enabled = buttonExport.Enabled = buttonImport.Enabled =
                    deleteToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = false;
                new Thread(importSaves).Start(files);
            }
        }

        void importSaves(object o)
        {
            try
            {
                var files = (string[])o;
                if (!(bool)Invoke(new Func<bool>(delegate
                {
                    if (!WaitingClovershellForm.WaitForDevice(this))
                        return false;
                    return true;
                }))) return;
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
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
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
                if (images.Count == 0) return;
                if (menuOpened) return; // Right click...
                Invoke(new Action(delegate
                {
                    // Maybe it's too late?
                    var p = listViewSaves.PointToClient(Cursor.Position);
                    var item = listViewSaves.GetItemAt(p.X, p.Y);
                    if (item == null) return; // No rows at all
                    if (item.SubItems["colCode"].Text != code) return; // Other item

                    if (imagesForm == null)
                        imagesForm = new ImagesForm();
                    imagesForm.Left = Cursor.Position.X + 5;
                    imagesForm.Top = Cursor.Position.Y + 5;
                    imagesForm.Text = item.SubItems["colName"].Text;
                    imagesForm.ShowImages(images);
                    imagesForm.Show();
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private void listViewSaves_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            exportToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled =
                buttonExport.Enabled = buttonDelete.Enabled = listViewSaves.SelectedItems.Count > 0;
            if (listViewSaves.SelectedItems.Count > 0)
            {
                var size = 0;
                foreach (ListViewItem game in listViewSaves.SelectedItems)
                    if (game.SubItems["colSize"].Text != null)
                        size += int.Parse(game.SubItems["colSize"].Text.Replace("KB", ""));
                toolStripStatusLabelSize.Text = Resources.SizeOfSaves + " " + size + "KB";
            }
            else toolStripStatusLabelSize.Text = "";
        }

        private void listViewSaves_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            new Thread(loadImagesThread).Start(e.Item.SubItems["colCode"].Text);
        }

        private void listViewSaves_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastPosition.Equals(Cursor.Position)) return;
            lastPosition = Cursor.Position;
            if (imagesForm != null)
                imagesForm.Hide();
        }

        bool menuOpened = false;
        private void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            menuOpened = true;
        }

        private void contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            menuOpened = false;
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (imagesForm != null)
                imagesForm.Hide();
        }

        private void listViewSaves_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && buttonDelete.Enabled)
            {
                buttonDelete_Click(null, null);
            }
        }

        private void listViewSaves_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewSaves.ListViewItemSorter = new SavesSorter(e.Column,
                ((listViewSaves.ListViewItemSorter as SavesSorter).Column == e.Column &&
                !(listViewSaves.ListViewItemSorter as SavesSorter).Reverse));
            listViewSaves.Sort();
        }

        private class SavesSorter : IComparer
        {
            public readonly int Column;
            public readonly bool Reverse;

            public SavesSorter(int column, bool reverse)
            {
                Column = column;
                Reverse = reverse;
            }

            public int Compare(object o1, object o2)
            {
                var l1 = o1 as ListViewItem;
                var l2 = o2 as ListViewItem;
                int r = Reverse ? -1 : 1;
                try
                {
                    switch (Column)
                    {
                        case 0:
                        default:
                            return l1.SubItems["colName"].Text.CompareTo(l2.SubItems["colName"].Text) * r;
                        case 1:
                            return l1.SubItems["colCode"].Text.CompareTo(l2.SubItems["colCode"].Text) * r;
                        case 2:
                            return (int.Parse(l1.SubItems["colSize"].Text.Replace("KB", "")) - int.Parse(l2.SubItems["colSize"].Text.Replace("KB", ""))) * r;
                        case 3:
                            return (l1.SubItems["colFlags"].Text.Length - l2.SubItems["colFlags"].Text.Length) * r;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
