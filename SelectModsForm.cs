using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectModsForm : Form
    {
        private readonly string baseDirectory;
        private readonly string usermodsDirectory;
        private readonly string[] readmeFiles;

        public SelectModsForm(bool loadInstalledMods, bool allowDropMods)
        {
            InitializeComponent();
            baseDirectory = MainForm.BaseDirectory;
            usermodsDirectory = Path.Combine(baseDirectory, "user_mods");
            var modsList = new List<string>();
            if (loadInstalledMods && MainForm.Clovershell.IsOnline)
            {
                var modsstr = MainForm.Clovershell.ExecuteSimple("ls /var/lib/hakchi/hmod/uninstall-*", 1000, true);
                var installedMods = modsstr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var mod in installedMods)
                {
                    var modname = mod;
                    int pos;
                    while ((pos = modname.IndexOf("/")) >= 0)
                        modname = modname.Substring(pos + 1);
                    modname = modname.Substring("uninstall-".Length);
                    if (MainForm.InternalMods.Contains(modname))
                        continue;
                    modsList.Add(modname);
                }
            }
            else
            {
                if (Directory.Exists(usermodsDirectory))
                {
                    modsList.AddRange(from m
                                      in Directory.GetDirectories(usermodsDirectory, "*.hmod", SearchOption.TopDirectoryOnly)
                                      select Path.GetFileNameWithoutExtension(m));
                    modsList.AddRange(from m
                                      in Directory.GetFiles(usermodsDirectory, "*.hmod", SearchOption.TopDirectoryOnly)
                                      select Path.GetFileNameWithoutExtension(m));
                }
            }
            readmeFiles = new string[] { "readme.txt", "readme.md", "readme" };
            checkedListBoxMods.Items.Clear();
            checkedListBoxMods.Items.AddRange(modsList.ToArray());
            this.AllowDrop = allowDropMods;
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (checkedListBoxMods.CheckedItems.Count > 0)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkedListBoxMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxMods.SelectedItem != null)
                new Thread(loadReadMe).Start(checkedListBoxMods.SelectedItem.ToString());
            else
            {
                textBoxReadme.Text = "";
                textBoxReadme.Enabled = false;
            }
        }

        void loadReadMe(object obj)
        {
            try
            {
                var selected = obj as string;
                var text = "";
                var dir = Path.Combine(usermodsDirectory, selected + ".hmod");
                if (Directory.Exists(dir))
                {
                    foreach (var f in readmeFiles)
                    {
                        var fn = Path.Combine(dir, f);
                        if (File.Exists(fn))
                        {
                            text = File.ReadAllText(fn);
                            break;
                        }
                    }
                }
                else if (File.Exists(dir))
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(baseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(dir))
                    {
                        var tar = new MemoryStream();
                        szExtractor.ExtractFile(0, tar);
                        tar.Seek(0, SeekOrigin.Begin);
                        using (var szExtractorTar = new SevenZipExtractor(tar))
                        {
                            foreach (var f in szExtractorTar.ArchiveFileNames)
                            {
                                if (readmeFiles.Contains(f.ToLower()))
                                {
                                    var o = new MemoryStream();
                                    szExtractorTar.ExtractFile(f, o);
                                    var rawData = new byte[o.Length];
                                    o.Seek(0, SeekOrigin.Begin);
                                    o.Read(rawData, 0, (int)o.Length);
                                    text = Encoding.UTF8.GetString(rawData);
                                    if (!text.Contains("\r"))
                                        text = text.Replace("\n", "\r\n");
                                    break;
                                }
                            }
                        }
                    }
                }
                Invoke(new Action<string, string>(showReadMe), new object[] { selected, text });
            }
            catch
            {
            }
        }

        void showReadMe(string mod, string readme)
        {
            if (checkedListBoxMods.SelectedItem != null &&
                checkedListBoxMods.SelectedItem.ToString() == mod)
            {
                textBoxReadme.Text = readme;
                textBoxReadme.Enabled = readme.Length > 0;
            }
        }

        private void SelectModsForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void SelectModsForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var listAddedMods = new List<string>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".hmod")
                {
                    File.Copy(file, Path.Combine(usermodsDirectory, Path.GetFileName(file)), true);
                    listAddedMods.Add(Path.GetFileNameWithoutExtension(file));
                }
                else if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(baseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var szExtractor = new SevenZipExtractor(file))
                    {
                        foreach (var f in szExtractor.ArchiveFileNames)
                            if (Path.GetExtension(f).ToLower() == ".hmod")
                            {
                                using (var outFile = new FileStream(Path.Combine(usermodsDirectory, Path.GetFileName(f)), FileMode.Create))
                                {
                                    szExtractor.ExtractFile(f, outFile);
                                    listAddedMods.Add(Path.GetFileNameWithoutExtension(f));
                                }
                            }
                    }
                }
            }
            foreach (var mod in listAddedMods)
            {
                checkedListBoxMods.Items.Remove(mod);
                checkedListBoxMods.Items.Add(mod, true);
            }
        }
    }
}
