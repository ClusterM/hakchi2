using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class TechInfo : Form
    {
        public TechInfo()
        {
            InitializeComponent();
        }

        private void TechInfo_Load(object sender, EventArgs e)
        {
            try
            {
                ListViewGroup
                    generalInfoGroup = new ListViewGroup("General information"),
                    localHakchiGroup = new ListViewGroup("Loadl hakchi.hmod"),
                    shellInfoGroup = new ListViewGroup("Shell information"),
                    hakchiGroup = new ListViewGroup("Running hakchi.hmod info"),
                    pathsGroup = new ListViewGroup("Paths"),
                    memoryStatsGroup = new ListViewGroup("Memory stats"),
                    consoleSettingsGroup = new ListViewGroup("Console settings"),
                    settingsGroup = new ListViewGroup("Settings");

                listView1.Groups.AddRange(new ListViewGroup[] {
                    generalInfoGroup,
                    localHakchiGroup,
                    shellInfoGroup,
                    hakchiGroup,
                    pathsGroup,
                    memoryStatsGroup,
                    consoleSettingsGroup,
                    settingsGroup
                });

                string devTools = "";
                if (ConfigIni.Instance.ForceClovershell) devTools += "Force Clovershell, ";
                if (ConfigIni.Instance.ForceSSHTransfers) devTools += "Force SSH Transfers, ";
                if (ConfigIni.Instance.UploadToTmp) devTools += "Upload to /tmp, ";
                if (ConfigIni.Instance.DisableClovershellListener) devTools += "Disable Clovershell listener, ";
                if (ConfigIni.Instance.DisableSSHListener) devTools += "Disable SSH listener, ";
                devTools = devTools.TrimEnd(new char[] { ',', ' ' });
                if (string.IsNullOrWhiteSpace(devTools)) devTools = "None";

                var gamesSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games"));
                var gamesNum = gamesSize > 0 ? Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games")).Count() : 0;
                var gamesSnesSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games_snes"));
                var gamesSnesNum = gamesSnesSize > 0 ? Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games_snes")).Count() : 0;
                var gamesCacheSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games_cache"));
                var gamesCacheNum = gamesCacheSize > 0 ? Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games_cache")).Count() : 0;

                listView1.Items.AddRange(new ListViewItem[] {
                    // general info
                    new ListViewItem(new string[] { "hakchi2 CE version:", Shared.AppDisplayVersion }, generalInfoGroup),
                    new ListViewItem(new string[] { "Portable mode:", Program.isPortable ? Resources.Yes : Resources.No }, generalInfoGroup),
                    new ListViewItem(new string[] { "Developer tools:", devTools }, generalInfoGroup),
                    new ListViewItem(new string[] { "Internal path:", Program.BaseDirectoryInternal }, generalInfoGroup),
                    new ListViewItem(new string[] { "External path:", Program.BaseDirectoryExternal }, generalInfoGroup),
                    new ListViewItem(new string[] { "/games:", gamesNum > 0 ? ($"{gamesNum} directories (" + Shared.SizeSuffix(gamesSize) + ")") : Resources.None }, generalInfoGroup),
                    new ListViewItem(new string[] { "/games_snes:", gamesSnesNum > 0 ? ($"{gamesSnesNum} directories (" + Shared.SizeSuffix(gamesSnesSize) + ")") : Resources.None}, generalInfoGroup),
                    new ListViewItem(new string[] { "/games_cache", gamesCacheNum > 0 ? ($"{gamesCacheNum} directories (" + Shared.SizeSuffix(gamesCacheSize) + ")") : Resources.None }, generalInfoGroup),

                    // local hakchi info
                    new ListViewItem(new string[] { "Boot version:", hakchi.RawLocalBootVersion }, localHakchiGroup),
                    new ListViewItem(new string[] { "Kernel version:", hakchi.RawLocalKernelVersion }, localHakchiGroup),
                    new ListViewItem(new string[] { "Script version:", hakchi.RawLocalScriptVersion }, localHakchiGroup),

                    // shell info
                    new ListViewItem(new string[] { "Connected:", hakchi.Connected ? Resources.Yes : Resources.No }, shellInfoGroup),

                    // settings
                    new ListViewItem(new string[] { "Separate games for multiboot:", ConfigIni.Instance.SeparateGameStorage ? Resources.Yes : Resources.No }, settingsGroup),
                    new ListViewItem(new string[] { "Use linked sync:", ConfigIni.Instance.SyncLinked ? Resources.Yes : Resources.No }, settingsGroup),
                });

                if (hakchi.Connected)
                {
                    // shell
                    string shell = Resources.Unknown;
                    if (hakchi.Shell is INetworkShell)
                        shell = "SSH";
                    else if (hakchi.Shell is clovershell.ClovershellConnection)
                        shell = "Clovershell";

                    listView1.Items.AddRange(new ListViewItem[] {
                        // shell info
                        new ListViewItem(new string[] { "Shell:", shell }, shellInfoGroup),
                        new ListViewItem(new string[] { "Can interact:", hakchi.CanInteract ? Resources.Yes : Resources.No }, shellInfoGroup),
                        new ListViewItem(new string[] { "Minimal memboot:", hakchi.MinimalMemboot ? Resources.Yes : Resources.No }, shellInfoGroup),
                        new ListViewItem(new string[] { "Console unique ID:", hakchi.UniqueID }, shellInfoGroup),

                        // hakchi info
                        new ListViewItem(new string[] { "Boot version:", hakchi.RawBootVersion }, hakchiGroup),
                        new ListViewItem(new string[] { "Kernel version:", hakchi.RawKernelVersion }, hakchiGroup),
                        new ListViewItem(new string[] { "Script version:", hakchi.RawScriptVersion }, hakchiGroup),
                    });
                    

                    if (hakchi.MinimalMemboot)
                    {
                        // no-op
                    }
                    else
                    {
                        if (hakchi.CanInteract)
                        {
                            listView1.Items.AddRange(new ListViewItem[]
                            {
                                // more shell info
                                new ListViewItem(new string[] { "Detected console type:", MainForm.GetConsoleTypeName(hakchi.DetectedConsoleType) }, shellInfoGroup),
                                new ListViewItem(new string[] { "Custom firmware:", hakchi.CustomFirmwareLoaded ? Resources.Yes : Resources.No }, shellInfoGroup),

                                // paths
                                new ListViewItem(new string[] { "Config:", hakchi.ConfigPath }, pathsGroup),
                                new ListViewItem(new string[] { "Remote sync:", hakchi.RemoteGameSyncPath }, pathsGroup),
                                new ListViewItem(new string[] { "System code:", hakchi.SystemCode ?? "-" }, pathsGroup),
                                new ListViewItem(new string[] { "Media:", hakchi.MediaPath }, pathsGroup),
                                new ListViewItem(new string[] { "Original games:", hakchi.OriginalGamesPath }, pathsGroup),
                                new ListViewItem(new string[] { "Games:", hakchi.GamesPath }, pathsGroup),
                                new ListViewItem(new string[] { "RootFS:", hakchi.RootFsPath }, pathsGroup),
                                new ListViewItem(new string[] { "Profile:", hakchi.GamesProfilePath }, pathsGroup),
                                new ListViewItem(new string[] { "SquashFS:", hakchi.SquashFsPath }, pathsGroup),

                                // memory stats
                                new ListViewItem(new string[] { "Storage total:", Shared.SizeSuffix(MemoryStats.StorageTotal) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "Storage used:", Shared.SizeSuffix(MemoryStats.StorageUsed) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "Storage free:", Shared.SizeSuffix(MemoryStats.StorageFree) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "External saves:", MemoryStats.ExternalSaves ? Resources.Yes : Resources.No }, memoryStatsGroup),
                                new ListViewItem(new string[] { "Saves:", Shared.SizeSuffix(MemoryStats.SaveStatesSize) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "All games:", Shared.SizeSuffix(MemoryStats.AllGamesSize) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "Non multiboot games:", Shared.SizeSuffix(MemoryStats.NonMultibootGamesSize) }, memoryStatsGroup),
                                new ListViewItem(new string[] { "Extra files:", Shared.SizeSuffix(MemoryStats.ExtraFilesSize) }, memoryStatsGroup),
                            });

                            // collections sizes
                            foreach (var pair in MemoryStats.Collections)
                            {
                                listView1.Items.Add(
                                    new ListViewItem(new string[] { MainForm.GetConsoleTypeName(pair.Key) + " games:", Shared.SizeSuffix(pair.Value) }, memoryStatsGroup));
                            }
                        }
                    }
                }

                logContentsRichTextBox.Text = Program.GetCurrentLogContent();
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var writer = new StreamWriter(File.OpenWrite(saveFileDialog1.FileName)))
                    {
                        listView1.Items.Cast<ListViewItem>().ToList().ForEach(item => { writer.WriteLine(item.SubItems[0].Text + " " + item.SubItems[1].Text); });
                        writer.Write(
                            Environment.NewLine +
                            "--- DEBUGLOG.TXT content ---" + Environment.NewLine +
                            logContentsRichTextBox.Text +
                            "--- End of DEBUGLOG.TXT content ---" + Environment.NewLine);
                    }
                    try
                    {
                        new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = saveFileDialog1.FileName
                            }
                        }.Start();
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    Tasks.ErrorForm.Show(this, ex);
                }
            }
        }

        private void TechInfo_Resize(object sender, EventArgs e)
        {
            listView1.Columns[1].Width = -2;
        }

        private void TechInfo_Shown(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
        }
    }
}
