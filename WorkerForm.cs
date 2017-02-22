using com.clusterrr.Famicom;
using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WorkerForm : Form
    {
        public enum Tasks { DumpKernel, FlashKernel, Memboot, DownloadAllCovers, AddGames };
        public Tasks Task;
        //public string UBootDump;
        public string KernelDump;
        public string Mod = null;
        public Dictionary<string, string> Config = null;
        public string[] HiddenGames;
        public NesMenuCollection Games;
        public List<string> hmodsInstall;
        public List<string> hmodsUninstall;
        public string[] GamesToAdd;
        public NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Auto;
        public int MaxGamesPerFolder = 35;
        public MainForm MainForm;
        Thread thread = null;
        Fel fel = null;

        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;

        readonly string baseDirectory;
        readonly string fes1Path;
        readonly string ubootPath;
        readonly string tempDirectory;
        readonly string kernelDirectory;
        readonly string initramfs_cpio;
        readonly string initramfs_cpioPatched;
        readonly string ramfsDirectory;
        readonly string hakchiDirectory;
        readonly string modsDirectory;
        readonly string[] hmodDirectories;
        readonly string toolsDirectory;
        readonly string kernelPatched;
        readonly string ramdiskPatched;
        readonly string configPath;
        readonly string hiddenPath;
        readonly string tempGamesDirectory;
        readonly string tempHmodsDirectory;
        readonly string cloverconDriverPath;
        readonly string argumentsFilePath;
        readonly string transferDirectory;
        readonly string originalGamesConfigDirectory;
        string[] correctKernels;
        const long maxRamfsSize = 40 * 1024 * 1024;
        string selectedFile = null;
        public NesMiniApplication[] addedApplications;

        public WorkerForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            baseDirectory = MainForm.BaseDirectory;
            fes1Path = Path.Combine(Path.Combine(baseDirectory, "data"), "fes1.bin");
            ubootPath = Path.Combine(Path.Combine(baseDirectory, "data"), "uboot.bin");
            tempDirectory = Path.Combine(baseDirectory, "temp");
            kernelDirectory = Path.Combine(tempDirectory, "kernel");
            initramfs_cpio = Path.Combine(kernelDirectory, "initramfs.cpio");
            initramfs_cpioPatched = Path.Combine(kernelDirectory, "initramfs_mod.cpio");
            ramfsDirectory = Path.Combine(kernelDirectory, "initramfs");
            hakchiDirectory = Path.Combine(ramfsDirectory, "hakchi");
            modsDirectory = Path.Combine(baseDirectory, "mods");
            hmodDirectories = new string[]{
                Path.Combine(baseDirectory, "user_mods"),
                Path.Combine(modsDirectory, "hmods")
            };
            toolsDirectory = Path.Combine(baseDirectory, "tools");
            kernelPatched = Path.Combine(kernelDirectory, "patched_kernel.img");
            ramdiskPatched = Path.Combine(kernelDirectory, "kernel.img-ramdisk_mod.gz");
            cloverconDriverPath = Path.Combine(hakchiDirectory, "clovercon.ko");
            argumentsFilePath = Path.Combine(hakchiDirectory, "extra_args");
            transferDirectory = Path.Combine(hakchiDirectory, "transfer");
            configPath = Path.Combine(transferDirectory, "transfer");
            tempGamesDirectory = Path.Combine(transferDirectory, "games");
            tempHmodsDirectory = Path.Combine(transferDirectory, "hmod");
            originalGamesConfigDirectory = Path.Combine(tempGamesDirectory, "original");
            hiddenPath = Path.Combine(originalGamesConfigDirectory, "hidden");
            correctKernels = new string[] {
                "5cfdca351484e7025648abc3b20032ff", "07bfb800beba6ef619c29990d14b5158", // NES Mini
                "ac8144c3ea4ab32e017648ee80bdc230" // Famicom Mini
            };
        }

        public DialogResult Start()
        {
            SetProgress(0, 1);
            thread = new Thread(StartThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return ShowDialog();
        }

        DialogResult WaitForDeviceFromThread()
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<DialogResult>(WaitForDeviceFromThread));
            }
            SetStatus(Resources.WaitingForDevice);
            if (fel != null)
                fel.Close();
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            if (WaitingForm.WaitForDevice(vid, pid))
            {
                fel = new Fel();
                if (!File.Exists(fes1Path)) throw new FileNotFoundException(fes1Path + " not found");
                if (!File.Exists(ubootPath)) throw new FileNotFoundException(ubootPath + " not found");
                fel.Fes1Bin = File.ReadAllBytes(fes1Path);
                fel.UBootBin = File.ReadAllBytes(ubootPath);
                fel.Open(vid, pid);
                SetStatus(Resources.UploadingFes1);
                fel.InitDram(true);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                return DialogResult.OK;
            }
            else return DialogResult.Abort;
        }

        private delegate DialogResult MessageBoxFromThreadDelegate(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak);
        DialogResult MessageBoxFromThread(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak)
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new MessageBoxFromThreadDelegate(MessageBoxFromThread),
                    new object[] { owner, text, caption, buttons, icon, defaultButton, tweak });
            }
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            if (tweak) MessageBoxManager.Register(); // Tweak button names
            var result = MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
            if (tweak) MessageBoxManager.Unregister();
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        DialogResult FolderManagerFromThread(NesMenuCollection collection)
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<NesMenuCollection, DialogResult>(FolderManagerFromThread), new object[] { collection });
            }
            var constructor = new TreeContructorForm(collection, MainForm);
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            var result = constructor.ShowDialog();
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        DialogResult SelectFileFromThread(string[] files)
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<string[], DialogResult>(SelectFileFromThread), new object[] { files });
            }
            var form = new SelectFileForm(files);
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            var result = form.ShowDialog();
            selectedFile = form.listBoxFiles.SelectedItem.ToString();
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        public void StartThread()
        {
            SetProgress(0, 1);
            try
            {
                switch (Task)
                {
                    case Tasks.DumpKernel:
                        DoKernelDump();
                        break;
                    case Tasks.FlashKernel:
                        FlashKernel();
                        break;
                    case Tasks.Memboot:
                        Memboot();
                        break;
                    case Tasks.DownloadAllCovers:
                        DownloadAllCovers();
                        break;
                    case Tasks.AddGames:
                        AddGames(GamesToAdd);
                        break;
                }
                Thread.Sleep(1000);
                DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                thread = null;
                if (fel != null)
                {
                    fel.Close();
                    fel = null;
                }
            }
            GC.Collect();
        }

        void SetStatus(string status)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(SetStatus), new object[] { status });
                    return;
                }
                labelStatus.Text = status;
            }
            catch { }
        }

        void SetProgress(int value, int max)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<int, int>(SetProgress), new object[] { value, max });
                    return;
                }
                if (value > max) value = max;
                progressBar.Maximum = max;
                progressBar.Value = value;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                TaskbarProgress.SetValue(this.Handle, value, max);
            }
            catch { }
        }

        void ShowError(Exception ex, bool dontStop = false)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Exception, bool>(ShowError), new object[] { ex, dontStop });
                    return;
                }
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Error);
                var message = ex.Message;
#if DEBUG
                message += ex.StackTrace;
#endif
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (ex is GameGenieFormatException || ex is GameGenieNotFoundException)
                    MessageBox.Show(this, message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is MadWizard.WinUSBNet.USBException)
                    MessageBox.Show(this, message + "\r\n" + Resources.PleaseTryAgainUSB, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                if (!dontStop)
                {
                    thread = null;
                    Close();
                }
            }
            catch { }
        }

        void ShowMessage(string text, string title)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string, string>(ShowMessage), new object[] { text, title });
                    return;
                }
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
                MessageBox.Show(this, text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            }
            catch { }
        }

        public void DoKernelDump()
        {
            int progress = 5;
            const int maxProgress = 80;
            if (WaitForDeviceFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            progress += 5;
            SetProgress(progress, maxProgress);

            SetStatus(Resources.DumpingKernel);
            var kernel = fel.ReadFlash(Fel.kernel_base_f, Fel.sector_size * 0x20,
                delegate(Fel.CurrentAction action, string command)
                {
                    switch (action)
                    {
                        case Fel.CurrentAction.RunningCommand:
                            SetStatus(Resources.ExecutingCommand + " " + command);
                            break;
                        case Fel.CurrentAction.ReadingMemory:
                            SetStatus(Resources.DumpingKernel);
                            break;
                    }
                    progress++;
                    SetProgress(progress, maxProgress);
                }
            );

            var size = CalKernelSize(kernel);
            if (size == 0 || size > Fel.kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);
            if (kernel.Length > size)
            {
                var sm_kernel = new byte[size];
                Array.Copy(kernel, 0, sm_kernel, 0, size);
                kernel = sm_kernel;
            }

            SetProgress(maxProgress, maxProgress);
            SetStatus(Resources.Done);

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(kernel)).Replace("-", "").ToLower();
            if (!correctKernels.Contains(hash))
            {
                if (MessageBoxFromThread(this, Resources.MD5Failed + " " + hash + "\r\n" + Resources.MD5Failed2 +
                    "\r\n" + Resources.DoYouWantToContinue, Resources.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, false)
                    == DialogResult.No)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(KernelDump));
            File.WriteAllBytes(KernelDump, kernel);
        }

        public void FlashKernel()
        {
            int progress = 0;
            int maxProgress = 120 + (string.IsNullOrEmpty(Mod) ? 0 : 5);
            if (WaitForDeviceFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            maxProgress += 5;
            SetProgress(progress, maxProgress);

            byte[] kernel;
            if (!string.IsNullOrEmpty(Mod))
            {
                kernel = CreatePatchedKernel();
                progress += 5;
                SetProgress(progress, maxProgress);
            }
            else
                kernel = File.ReadAllBytes(KernelDump);
            var size = CalKernelSize(kernel);
            if (size > kernel.Length || size > Fel.kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);

            size = (size + Fel.sector_size - 1) / Fel.sector_size;
            size = size * Fel.sector_size;
            if (kernel.Length != size)
            {
                var newK = new byte[size];
                Array.Copy(kernel, newK, kernel.Length);
                kernel = newK;
            }

            fel.WriteFlash(Fel.kernel_base_f, kernel,
                delegate(Fel.CurrentAction action, string command)
                {
                    switch (action)
                    {
                        case Fel.CurrentAction.RunningCommand:
                            SetStatus(Resources.ExecutingCommand + " " + command);
                            break;
                        case Fel.CurrentAction.WritingMemory:
                            SetStatus(Resources.UploadingKernel);
                            break;
                    }
                    progress++;
                    SetProgress(progress, maxProgress);
                }
            );
            var r = fel.ReadFlash((UInt32)Fel.kernel_base_f, (UInt32)kernel.Length,
                delegate(Fel.CurrentAction action, string command)
                {
                    switch (action)
                    {
                        case Fel.CurrentAction.RunningCommand:
                            SetStatus(Resources.ExecutingCommand + " " + command);
                            break;
                        case Fel.CurrentAction.ReadingMemory:
                            SetStatus(Resources.Verifying);
                            break;
                    }
                    progress++;
                    SetProgress(progress, maxProgress);
                }
            );
            if (!kernel.SequenceEqual(r))
                throw new Exception(Resources.VerifyFailed);

            if (string.IsNullOrEmpty(Mod))
            {
                var shutdownCommand = string.Format("shutdown", Fel.kernel_base_m);
                SetStatus(Resources.ExecutingCommand + " " + shutdownCommand);
                fel.RunUbootCmd(shutdownCommand, true);
            }
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        public void Memboot()
        {
            int progress = 0;
            int maxProgress = -1;
            var stats = new GamesTreeStats();

            if (Games != null)
            {
                SetStatus(Resources.BuildingFolders);
                if (FoldersMode == NesMenuCollection.SplitStyle.Custom)
                {
                    if (FolderManagerFromThread(Games) != System.Windows.Forms.DialogResult.OK)
                    {
                        DialogResult = DialogResult.Abort;
                        return;
                    }
                    Games.AddBack();
                }
                else Games.Split(FoldersMode, MaxGamesPerFolder);
            }
            progress += 5;
            SetProgress(progress, 1000);

            do
            {
                if (stats.GamesProceed > 0)
                {
                    ShowMessage(Resources.ParticallyBody, Resources.ParticallyTitle);
                }
                GC.Collect();

                // Connecting to NES Mini
                if (WaitForDeviceFromThread() != DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
                progress += 5;
                SetProgress(progress, maxProgress > 0 ? maxProgress : 1000);

                byte[] kernel;
                if (!string.IsNullOrEmpty(Mod))
                    kernel = CreatePatchedKernel(stats);
                else
                    kernel = File.ReadAllBytes(KernelDump);
                var size = CalKernelSize(kernel);
                if (size > kernel.Length || size > Fel.kernel_max_size)
                    throw new Exception(Resources.InvalidKernelSize + " " + size);
                size = (size + Fel.sector_size - 1) / Fel.sector_size;
                size = size * Fel.sector_size;
                if (kernel.Length != size)
                {
                    var newK = new byte[size];
                    Array.Copy(kernel, newK, kernel.Length);
                    kernel = newK;
                }
                progress += 5;
                if (maxProgress < 0)
                {
                    if (stats.GamesProceed > 0)
                        maxProgress = (int)(((double)kernel.Length / (double)67000 + 20) * (double)stats.TotalSize / (double)stats.Size + 
                            100 * ((int)Math.Ceiling((double)stats.TotalSize / (double)stats.Size) - 1));
                    else
                        maxProgress = (int)((double)kernel.Length / (double)67000 + 20);
                }
                SetProgress(progress, maxProgress);

                SetStatus(Resources.UploadingKernel);
                fel.WriteMemory(Fel.flash_mem_base, kernel,
                    delegate(Fel.CurrentAction action, string command)
                    {
                        switch (action)
                        {
                            case Fel.CurrentAction.WritingMemory:
                                SetStatus(Resources.UploadingKernel);
                                break;
                        }
                        progress++;
                        SetProgress(progress, maxProgress);
                    }
                );

                var bootCommand = string.Format("boota {0:x}", Fel.kernel_base_m);
                SetStatus(Resources.ExecutingCommand + " " + bootCommand);
                fel.RunUbootCmd(bootCommand, true);
            } while (stats.GamesProceed < stats.TotalGames);
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        private byte[] CreatePatchedKernel(GamesTreeStats stats = null)
        {
            if (stats == null) stats = new GamesTreeStats();
            bool first = stats.GamesProceed == 0;
            bool partial = stats.GamesProceed > 0;
            SetStatus(Resources.BuildingCustom);
            if (first)
            {
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);
                Directory.CreateDirectory(tempDirectory);
                Directory.CreateDirectory(kernelDirectory);
                Directory.CreateDirectory(ramfsDirectory);
                if (!ExecuteTool("unpackbootimg.exe", string.Format("-i \"{0}\" -o \"{1}\"", KernelDump, kernelDirectory)))
                    throw new Exception("Can't unpack kernel image");
                if (!ExecuteTool("lzop.exe", string.Format("-d \"{0}\" -o \"{1}\"",
                    Path.Combine(kernelDirectory, "kernel.img-ramdisk.gz"), initramfs_cpio)))
                    throw new Exception("Can't unpack ramdisk");
                ExecuteTool("cpio.exe", string.Format("-imd --no-preserve-owner --quiet -I \"{0}\"",
                   @"..\initramfs.cpio"), ramfsDirectory);
                if (!File.Exists(Path.Combine(ramfsDirectory, "init"))) // cpio.exe fails on Windows XP for some reason. But working!
                    throw new Exception("Can't unpack ramdisk 2");
                if (Directory.Exists(hakchiDirectory)) Directory.Delete(hakchiDirectory, true);
                NesMiniApplication.DirectoryCopy(Path.Combine(modsDirectory, Mod), ramfsDirectory, true);
                var ramfsFiles = Directory.GetFiles(ramfsDirectory, "*.*", SearchOption.AllDirectories);
                foreach (var file in ramfsFiles)
                {
                    var fInfo = new FileInfo(file);
                    if (fInfo.Length > 10 && fInfo.Length < 100 && ((fInfo.Attributes & FileAttributes.System) == 0) &&
                        (Encoding.ASCII.GetString(File.ReadAllBytes(file), 0, 10)) == "!<symlink>")
                        fInfo.Attributes |= FileAttributes.System;
                }
            }

            if (!first && Directory.Exists(transferDirectory))
            {
                Debug.WriteLine("Clearing transfer directory");
                Directory.Delete(transferDirectory, true);
            }

            // Games!
            if (Games != null)
            {
                Directory.CreateDirectory(tempGamesDirectory);
                if (first)
                {
                    File.WriteAllBytes(Path.Combine(tempGamesDirectory, "clear"), new byte[0]);
                    Directory.CreateDirectory(originalGamesConfigDirectory);
                    if (HiddenGames != null && HiddenGames.Length > 0)
                    {
                        StringBuilder h = new StringBuilder();
                        foreach (var game in HiddenGames)
                            h.Append(game + "\n");
                        File.WriteAllText(hiddenPath, h.ToString());
                    }
                }

                stats.Next();
                AddMenu(Games, stats);
                Debug.WriteLine(string.Format("Games copied: {0}/{1}, part size: {2}", stats.GamesProceed, stats.TotalGames, stats.Size));
            }

            bool last = stats.GamesProceed >= stats.TotalGames;

            if (last && hmodsInstall != null && hmodsInstall.Count > 0)
            {
                Directory.CreateDirectory(tempHmodsDirectory);
                foreach (var hmod in hmodsInstall)
                {
                    var modName = hmod + ".hmod";
                    foreach (var dir in hmodDirectories)
                    {
                        if (Directory.Exists(Path.Combine(dir, modName)))
                        {
                            NesMiniApplication.DirectoryCopy(Path.Combine(dir, modName), Path.Combine(tempHmodsDirectory, modName), true);
                            break;
                        }
                        if (File.Exists(Path.Combine(dir, modName)))
                        {
                            File.Copy(Path.Combine(dir, modName), Path.Combine(tempHmodsDirectory, modName));
                            break;
                        }
                    }
                }
            }
            if (last && hmodsUninstall != null && hmodsUninstall.Count > 0)
            {
                Directory.CreateDirectory(tempHmodsDirectory);
                var mods = new StringBuilder();
                foreach (var hmod in hmodsUninstall)
                    mods.AppendFormat("{0}.hmod\n", hmod);
                File.WriteAllText(Path.Combine(tempHmodsDirectory, "uninstall"), mods.ToString());
            }

            // Writing config
            if (Config != null && Config.Count > 0)
            {
                Directory.CreateDirectory(transferDirectory);
                var config = new StringBuilder();
                foreach (var key in Config.Keys)
                    config.AppendFormat("cfg_{0}='{1}'\n", key, Config[key].Replace(@"'", @"\'"));
                File.WriteAllText(configPath, config.ToString());
            }

            // Building image
            if (first && Games != null && Games.Count > 0) // There is no reason to compress cryptsetup when we do not uploading games
                ExecuteTool("upx.exe", "--best sbin\\cryptsetup", ramfsDirectory);
            byte[] ramdisk;
            if (!ExecuteTool("mkbootfs.exe", string.Format("\"{0}\"", ramfsDirectory), out ramdisk))
                throw new Exception("Can't repack ramdisk");
            File.WriteAllBytes(initramfs_cpioPatched, ramdisk);
            var argCmdline = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-cmdline")).Trim();
            var argBoard = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-board")).Trim();
            var argBase = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-base")).Trim();
            var argPagesize = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-pagesize")).Trim();
            var argKerneloff = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-kerneloff")).Trim();
            var argRamdiscoff = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-ramdiskoff")).Trim();
            var argTagsoff = File.ReadAllText(Path.Combine(kernelDirectory, "kernel.img-tagsoff")).Trim();
            if (!ExecuteTool("lzop.exe", string.Format("--best -f -o \"{0}\" \"{1}\"",
                ramdiskPatched, initramfs_cpioPatched)))
                throw new Exception("Can't repack ramdisk 2");
            if (!ExecuteTool("mkbootimg.exe", string.Format("--kernel \"{0}\" --ramdisk \"{1}\" --cmdline \"{2}\" --board \"{3}\" --base \"{4}\" --pagesize \"{5}\" --kernel_offset \"{6}\" --ramdisk_offset \"{7}\" --tags_offset \"{8}\" -o \"{9}\"",
                Path.Combine(kernelDirectory, "kernel.img-zImage"), ramdiskPatched, argCmdline, argBoard, argBase, argPagesize, argKerneloff, argRamdiscoff, argTagsoff, kernelPatched)))
                throw new Exception("Can't rebuild kernel");

            var result = File.ReadAllBytes(kernelPatched);
#if !DEBUG
            if (last)
                Directory.Delete(tempDirectory, true);
#endif
            if (result.Length > Fel.kernel_max_size) throw new Exception("Kernel is too big");
            GC.Collect();
            return result;
        }

        void DownloadAllCovers()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesMiniApplication game in Games)
            {
                SetStatus(Resources.GooglingFor + " " + game.Name);
                string[] urls = null;
                for (int tries = 0; tries < 5; tries++)
                {
                    if (urls == null)
                    {
                        try
                        {
                            urls = ImageGooglerForm.GetImageUrls(game);
                            break;
                        }
                        catch (Exception ex)
                        {
                            SetStatus(Resources.Error + ": " + ex.Message);
                            Thread.Sleep(1500);
                            continue;
                        }
                    }
                }
                if (urls != null && urls.Length == 0)
                    SetStatus(Resources.NotFound + " " + game.Name);
                for (int tries = 0; urls != null && tries < 5 && tries < urls.Length; tries++)
                {
                    try
                    {
                        var cover = ImageGooglerForm.DownloadImage(urls[tries]);
                        game.Image = cover;
                        break;
                    }
                    catch (Exception ex)
                    {
                        SetStatus(Resources.Error + ": " + ex.Message);
                        Thread.Sleep(1500);
                        continue;
                    }
                }
                SetProgress(++i, Games.Count);
                Thread.Sleep(500); // not so fast, Google don't like it
            }
        }

        private class GamesTreeStats
        {
            public List<NesMenuCollection> allMenus = new List<NesMenuCollection>();
            public int TotalGames = 0;
            public int GamesStart = 0;
            public int GamesProceed = 0;
            public long Size = 0;
            public long TotalSize = 0;
            public bool Stopped;

            public void Next()
            {
                allMenus.Clear();
                GamesStart = GamesProceed;
                TotalGames = 0;
                GamesProceed = 0;
                Size = 0;
                TotalSize = 0;
                Stopped = false;
            }
        }

        private void AddMenu(NesMenuCollection menuCollection, GamesTreeStats stats = null)
        {
            if (stats == null)
                stats = new GamesTreeStats();
            if (!stats.allMenus.Contains(menuCollection))
                stats.allMenus.Add(menuCollection);
            int menuIndex = stats.allMenus.IndexOf(menuCollection);
            string targetDirectory;
            if (menuIndex == 0)
                targetDirectory = tempGamesDirectory;
            else
                targetDirectory = Path.Combine(tempGamesDirectory, string.Format("{0:D3}", menuIndex));
            foreach (var element in menuCollection)
            {
                if (element is NesMiniApplication)
                {
                    stats.TotalGames++;
                    var game = element as NesMiniApplication;
                    var gameSize = game.Size();
                    if (gameSize >= maxRamfsSize) throw new Exception(string.Format(Resources.GameTooBig, game.Name));
                    stats.TotalSize += gameSize;
                    if (stats.Stopped || stats.Size + gameSize >= maxRamfsSize)
                    {
                        stats.Stopped = true;
                        continue;
                    }
                    stats.GamesProceed++;
                    if (stats.GamesStart >= stats.GamesProceed) continue;
                    Debug.Write(string.Format("Processing {0} ('{1}'), #{2}", game.Code, game.Name, stats.GamesProceed));
                    var gameCopy = game.CopyTo(targetDirectory);
                    stats.Size += gameSize;
                    Debug.WriteLine(string.Format(", total size: {0}", stats.Size));
                    try
                    {
                        if (gameCopy is NesGame && File.Exists((gameCopy as NesGame).GameGeniePath))
                        {
                            (gameCopy as NesGame).ApplyGameGenie();
                            File.Delete((gameCopy as NesGame).GameGeniePath);
                        }
                    }
                    catch (GameGenieFormatException ex)
                    {
                        ShowError(new GameGenieFormatException(string.Format(Resources.GameGenieFormatError, ex.Code, game)), dontStop: true);
                    }
                    catch (GameGenieNotFoundException ex)
                    {
                        ShowError(new GameGenieNotFoundException(string.Format(Resources.GameGenieNotFound, ex.Code, game.Name)), dontStop: true);
                    }
                }
                if (element is NesMenuFolder)
                {
                    var folder = element as NesMenuFolder;
                    if (!stats.allMenus.Contains(folder.ChildMenuCollection))
                    {
                        stats.allMenus.Add(folder.ChildMenuCollection);
                        AddMenu(folder.ChildMenuCollection, stats);
                    }
                    if (stats.GamesStart == 0)
                    {
                        folder.ChildIndex = stats.allMenus.IndexOf(folder.ChildMenuCollection);
                        var folderDir = Path.Combine(targetDirectory, folder.Code);
                        folder.Save(folderDir);
                    }
                }
                if (element is NesDefaultGame)
                {
                    if (stats.GamesStart == 0)
                    {
                        var game = element as NesDefaultGame;
                        var gfilePath = Path.Combine(originalGamesConfigDirectory, string.Format("gpath-{0}", game.Code));
                        File.WriteAllText(gfilePath, menuIndex == 0 ? "." : string.Format("{0:D3}", menuIndex));
                    }
                }
            }
        }

        private bool ExecuteTool(string tool, string args, string directory = null, bool external = false)
        {
            byte[] output;
            return ExecuteTool(tool, args, out output, directory, external);
        }

        private bool ExecuteTool(string tool, string args, out byte[] output, string directory = null, bool external = false)
        {
            var process = new Process();
            var appDirectory = baseDirectory;
            var fileName = !external ? Path.Combine(toolsDirectory, tool) : tool;
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = args;
            if (string.IsNullOrEmpty(directory))
                directory = appDirectory;
            process.StartInfo.WorkingDirectory = directory;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            Debug.WriteLine("Executing: " + fileName);
            Debug.WriteLine("Arguments: " + args);
            Debug.WriteLine("Directory: " + directory);
            process.Start();
            string outputStr = process.StandardOutput.ReadToEnd();
            string errorStr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            output = Encoding.GetEncoding(866).GetBytes(outputStr);
            Debug.WriteLineIf(outputStr.Length > 0 && outputStr.Length < 300, "Output:\r\n" + outputStr);
            Debug.WriteLineIf(errorStr.Length > 0, "Errors:\r\n" + errorStr);
            Debug.WriteLine("Exit code: " + process.ExitCode);
            return process.ExitCode == 0;
        }

        static UInt32 CalKernelSize(byte[] header)
        {
            if (Encoding.ASCII.GetString(header, 0, 8) != "ANDROID!") throw new Exception(Resources.InvalidKernelHeader);
            UInt32 kernel_size = (UInt32)(header[8] | (header[9] * 0x100) | (header[10] * 0x10000) | (header[11] * 0x1000000));
            UInt32 kernel_addr = (UInt32)(header[12] | (header[13] * 0x100) | (header[14] * 0x10000) | (header[15] * 0x1000000));
            UInt32 ramdisk_size = (UInt32)(header[16] | (header[17] * 0x100) | (header[18] * 0x10000) | (header[19] * 0x1000000));
            UInt32 ramdisk_addr = (UInt32)(header[20] | (header[21] * 0x100) | (header[22] * 0x10000) | (header[23] * 0x1000000));
            UInt32 second_size = (UInt32)(header[24] | (header[25] * 0x100) | (header[26] * 0x10000) | (header[27] * 0x1000000));
            UInt32 second_addr = (UInt32)(header[28] | (header[29] * 0x100) | (header[30] * 0x10000) | (header[31] * 0x1000000));
            UInt32 tags_addr = (UInt32)(header[32] | (header[33] * 0x100) | (header[34] * 0x10000) | (header[35] * 0x1000000));
            UInt32 page_size = (UInt32)(header[36] | (header[37] * 0x100) | (header[38] * 0x10000) | (header[39] * 0x1000000));
            UInt32 dt_size = (UInt32)(header[40] | (header[41] * 0x100) | (header[42] * 0x10000) | (header[43] * 0x1000000));
            UInt32 pages = 1;
            pages += (kernel_size + page_size - 1) / page_size;
            pages += (ramdisk_size + page_size - 1) / page_size;
            pages += (second_size + page_size - 1) / page_size;
            pages += (dt_size + page_size - 1) / page_size;
            return pages * page_size;
        }


        bool YesForAllPatches = false;
        public ICollection<NesMiniApplication> AddGames(string[] files, Form parentForm = null)
        {
            var apps = new List<NesMiniApplication>();
            addedApplications = null;
            //bool NoForAllUnsupportedMappers = false;
            bool YesForAllUnsupportedMappers = false;
            YesForAllPatches = false;
            int count = 0;
            SetStatus(Resources.AddingGames);
            foreach (var file in files)
            {
                NesMiniApplication app = null;
                try
                {
                    var fileName = file;
                    var ext = Path.GetExtension(file).ToLower();
                    bool? needPatch = YesForAllPatches ? (bool?)true : null;
                    byte[] rawData = null;
                    string tmp = null;
                    if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                    {
                        SevenZipExtractor.SetLibraryPath(Path.Combine(baseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        using (var szExtractor = new SevenZipExtractor(file))
                        {
                            var filesInArchive = new List<string>();
                            var nesFilesInArchive = new List<string>();
                            foreach (var f in szExtractor.ArchiveFileNames)
                            {
                                var e = Path.GetExtension(f).ToLower();
                                if (e == ".nes" || e == ".fds" || e == ".unf" || e == ".unif" || e == ".desktop")
                                    nesFilesInArchive.Add(f);
                                filesInArchive.Add(f);
                            }
                            if (nesFilesInArchive.Count == 1) // Only one NES file (or app)
                            {
                                fileName = nesFilesInArchive[0];
                            }
                            else if (nesFilesInArchive.Count > 1) // Many NES files, need to select
                            {
                                if (SelectFileFromThread(nesFilesInArchive.ToArray()) == DialogResult.OK)
                                    fileName = selectedFile;
                                else continue;
                            }
                            else if (filesInArchive.Count == 1) // No NES files but only one another file
                            {
                                fileName = filesInArchive[0];
                            }
                            else // Need to select
                            {
                                if (SelectFileFromThread(filesInArchive.ToArray()) == DialogResult.OK)
                                    fileName = selectedFile;
                                else continue;
                            }
                            var o = new MemoryStream();
                            if (Path.GetExtension(fileName).ToLower() == ".desktop" // App in archive, need the whole directory
                                || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName)+".jpg") // Or it has cover in archive
                                || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".png"))
                            {
                                tmp = Path.Combine(Path.GetTempPath(), fileName);
                                Directory.CreateDirectory(tmp);
                                szExtractor.ExtractArchive(tmp);
                                fileName = Path.Combine(tmp, fileName);
                            }
                            else
                            {
                                szExtractor.ExtractFile(fileName, o);
                                rawData = new byte[o.Length];
                                o.Seek(0, SeekOrigin.Begin);
                                o.Read(rawData, 0, (int)o.Length);
                            }
                        }
                    }
                    if (Path.GetExtension(fileName).ToLower() == ".nes")
                    {
                        try
                        {
                            app = NesGame.ImportNes(fileName, YesForAllUnsupportedMappers ? (bool?)true : null, ref needPatch, needPatchCallback, this, rawData);

                            // Trying to import Game Genie codes
                            var lGameGeniePath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".xml");
                            if (File.Exists(lGameGeniePath))
                            {
                                GameGenieDataBase lGameGenieDataBase = new GameGenieDataBase(app);
                                lGameGenieDataBase.ImportCodes(lGameGeniePath, true);
                                lGameGenieDataBase.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is UnsupportedMapperException || ex is UnsupportedFourScreenException)
                            {
                                var r = MessageBoxFromThread(this,
                                    (ex is UnsupportedMapperException)
                                       ? string.Format(Resources.MapperNotSupported, Path.GetFileName(fileName), (ex as UnsupportedMapperException).ROM.Mapper)
                                       : string.Format(Resources.FourScreenNotSupported, Path.GetFileName(fileName)),
                                    Resources.AreYouSure,
                                    files.Length <= 1 ? MessageBoxButtons.YesNo : MessageBoxButtons.AbortRetryIgnore,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, true);
                                if (r == DialogResult.Abort)
                                    YesForAllUnsupportedMappers = true;
                                if (r == DialogResult.Yes || r == DialogResult.Abort || r == DialogResult.Retry)
                                    app = NesGame.ImportNes(fileName, true, ref needPatch, needPatchCallback, this, rawData);
                                else
                                    continue;
                            }
                            else throw ex;
                        }
                    }
                    else
                    {
                        app = NesMiniApplication.Import(fileName, rawData);
                    }
                    if (!string.IsNullOrEmpty(tmp) && Directory.Exists(tmp)) Directory.Delete(tmp, true);
                    ConfigIni.SelectedGames += ";" + app.Code;
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException) return null;
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    ShowError(ex, true);
                }
                if (app != null)
                    apps.Add(app);
                SetProgress(++count, files.Length);
            }
            return apps; // Added games/apps
        }

        private bool needPatchCallback(Form parentForm, string nesFileName)
        {
            if (GamesToAdd == null || GamesToAdd.Length <= 1)
            {
                return MessageBoxFromThread(parentForm,
                    string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Resources.PatchAvailable,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1, false) == DialogResult.Yes;
            }
            else
            {
                var r = MessageBoxFromThread(parentForm,
                    string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Resources.PatchAvailable,
                    MessageBoxButtons.AbortRetryIgnore,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2, true);
                if (r == DialogResult.Abort)
                    YesForAllPatches = true;
                return r != DialogResult.Ignore;
            }
        }

        private void WorkerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((thread != null) && (e.CloseReason == CloseReason.UserClosing))
            {
                if (MessageBox.Show(this, Resources.DoYouWantCancel, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                if (thread != null) thread.Abort();
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                TaskbarProgress.SetValue(this.Handle, 0, 1);
            }
        }
    }
}
