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
        public Dictionary<string, bool> Config = null;
        public string[] HiddenGames;
        public NesMenuCollection Games;
        public SelectButtonsForm.NesButtons ResetCombination;
        public bool AutofireHack;
        public bool FcStart = true;
        public string ExtraCommandLineArguments = null;
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
        readonly string toolsDirectory;
        readonly string kernelPatched;
        readonly string ramdiskPatched;
        readonly string configPath;
        readonly string hiddenPath;
        readonly string tempGamesDirectory;
        readonly string cloverconDriverPath;
        readonly string argumentsFilePath;
        readonly string gamesDirectory;
        string[] correctKernels;
        const long maxRamfsSize = 40 * 1024 * 1024;
        DialogResult DeviceWaitResult = DialogResult.None;
        DialogResult MessageBoxResult = DialogResult.None;
        DialogResult FolderManagerResult = DialogResult.None;

        public WorkerForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            baseDirectory = MainForm.BaseDirectory;
            fes1Path = Path.Combine(Path.Combine(baseDirectory, "data"), "fes1.bin");
            ubootPath = Path.Combine(Path.Combine(baseDirectory, "data"), "uboot.bin");
            gamesDirectory = MainForm.GamesDirectory;
            tempDirectory = Path.Combine(baseDirectory, "temp");
            kernelDirectory = Path.Combine(tempDirectory, "kernel");
            initramfs_cpio = Path.Combine(kernelDirectory, "initramfs.cpio");
            initramfs_cpioPatched = Path.Combine(kernelDirectory, "initramfs_mod.cpio");
            ramfsDirectory = Path.Combine(kernelDirectory, "initramfs");
            hakchiDirectory = Path.Combine(ramfsDirectory, "hakchi");
            modsDirectory = Path.Combine(baseDirectory, "mods");
            toolsDirectory = Path.Combine(baseDirectory, "tools");
            kernelPatched = Path.Combine(kernelDirectory, "patched_kernel.img");
            ramdiskPatched = Path.Combine(kernelDirectory, "kernel.img-ramdisk_mod.gz");
            configPath = Path.Combine(hakchiDirectory, "config");
            hiddenPath = Path.Combine(hakchiDirectory, "hidden_games");
            cloverconDriverPath = Path.Combine(hakchiDirectory, "clovercon.ko");
            argumentsFilePath = Path.Combine(hakchiDirectory, "extra_args");
            correctKernels = new string[] {
                "5cfdca351484e7025648abc3b20032ff", "07bfb800beba6ef619c29990d14b5158", // NES Mini
                "ac8144c3ea4ab32e017648ee80bdc230" // Famicom Mini
            };
            tempGamesDirectory = Path.Combine(ramfsDirectory, "games");
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

        void WaitForDeviceFromThread()
        {
            DeviceWaitResult = DialogResult.None;
            if (InvokeRequired)
            {
                Invoke(new Action(WaitForDeviceFromThread));
                return;
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
                DeviceWaitResult = DialogResult.OK;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            }
            else DeviceWaitResult = DialogResult.Abort;
        }

        private delegate void MessageBoxFromThreadDelegate(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        void MessageBoxFromThread(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            MessageBoxResult = System.Windows.Forms.DialogResult.None;
            if (InvokeRequired)
            {
                Invoke(new MessageBoxFromThreadDelegate(MessageBoxFromThread),
                    new object[] { owner, text, caption, buttons, icon, defaultButton });
                return;
            }
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            MessageBoxResult = MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        void FolderManagerFromThread(NesMenuCollection collection)
        {
            FolderManagerResult = DialogResult.None;
            if (InvokeRequired)
            {
                Invoke(new Action<NesMenuCollection>(FolderManagerFromThread), new object[] { collection });
                return;
            }
            var constructor = new TreeContructorForm(collection, MainForm);
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            FolderManagerResult = constructor.ShowDialog();
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
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
                        AddGames(gamesDirectory, GamesToAdd);
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
            WaitForDeviceFromThread();
            while (DeviceWaitResult == DialogResult.None)
                Thread.Sleep(100);
            if (DeviceWaitResult != DialogResult.OK)
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
                if (MessageBox.Show(Resources.MD5Failed + " " + hash + "\r\n" + Resources.MD5Failed2 + "\r\n" + Resources.DoYouWantToContinue, Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
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
            WaitForDeviceFromThread();
            while (DeviceWaitResult == DialogResult.None)
                Thread.Sleep(100);
            if (DeviceWaitResult != DialogResult.OK)
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
                    FolderManagerFromThread(Games);
                    while (FolderManagerResult == DialogResult.None)
                        Thread.Sleep(100);
                    if (FolderManagerResult != System.Windows.Forms.DialogResult.OK)
                    {
                        DialogResult = DialogResult.Abort;
                        return;
                    }
                    Games.AddBack();
                }
                else Games.Split(FoldersMode, MaxGamesPerFolder);
            }
            progress += 5;
            SetProgress(progress, 300);

            do
            {
                if (stats.GamesProceed > 0)
                {
                    ShowMessage(Resources.ParticallyBody, Resources.ParticallyTitle);
                }

                // Connecting to NES Mini
                WaitForDeviceFromThread();
                while (DeviceWaitResult == DialogResult.None)
                    Thread.Sleep(500);
                if (DeviceWaitResult != DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
                progress += 5;
                SetProgress(progress, maxProgress);

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
                        maxProgress = (kernel.Length / 67000 + 20) * stats.GamesTotal / stats.GamesProceed + 75 * ((int)Math.Ceiling((float)stats.GamesTotal / (float)stats.GamesProceed) - 1);
                    else
                        maxProgress = (kernel.Length / 67000 + 20);
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
            } while (stats.GamesProceed < stats.GamesTotal);
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
                DirectoryCopy(Path.Combine(modsDirectory, Mod), ramfsDirectory, true);
                var ramfsFiles = Directory.GetFiles(ramfsDirectory, "*.*", SearchOption.AllDirectories);
                foreach (var file in ramfsFiles)
                {
                    var fInfo = new FileInfo(file);
                    if (fInfo.Length > 10 && fInfo.Length < 100 && ((fInfo.Attributes & FileAttributes.System) == 0) &&
                        (Encoding.ASCII.GetString(File.ReadAllBytes(file), 0, 10)) == "!<symlink>")
                        fInfo.Attributes |= FileAttributes.System;
                }

                if (HiddenGames != null && HiddenGames.Length > 0)
                {
                    StringBuilder h = new StringBuilder();
                    foreach (var game in HiddenGames)
                        h.Append(game + "\n");
                    File.WriteAllText(hiddenPath, h.ToString());
                }

                if (Config != null && Config.ContainsKey("hakchi_clovercon_hack")
                && Config["hakchi_clovercon_hack"] && File.Exists(cloverconDriverPath))
                {
                    byte[] drv = File.ReadAllBytes(cloverconDriverPath);
                    const string magicReset = "MAGIC_BUTTONS:";
                    for (int i = 0; i < drv.Length - magicReset.Length; i++)
                    {
                        if (Encoding.ASCII.GetString(drv, i, magicReset.Length) == magicReset)
                        {
                            int pos = i + magicReset.Length;
                            for (int b = 0; b < 8; b++)
                                drv[pos + b] = (byte)((((byte)ResetCombination & (1 << b)) != 0) ? '1' : '0');
                            break;
                        }
                    }
                    const string magicAutofire = "MAGIC_AUTOFIRE:";
                    for (int i = 0; i < drv.Length - magicAutofire.Length; i++)
                    {
                        if (Encoding.ASCII.GetString(drv, i, magicAutofire.Length) == magicAutofire)
                        {
                            int pos = i + magicAutofire.Length;
                            drv[pos] = (byte)(AutofireHack ? '1' : '0');
                            break;
                        }
                    }
                    const string magicFcStart = "MAGIC_FC_START:";
                    for (int i = 0; i < drv.Length - magicFcStart.Length; i++)
                    {
                        if (Encoding.ASCII.GetString(drv, i, magicFcStart.Length) == magicFcStart)
                        {
                            int pos = i + magicFcStart.Length;
                            drv[pos] = (byte)(FcStart ? '1' : '0');
                            break;
                        }
                    }
                    File.WriteAllBytes(cloverconDriverPath, drv);
                }
            } // if first transfer
            else // else clean games directory and extra files
            {
                if (Directory.Exists(tempGamesDirectory))
                {
                    Debug.WriteLine("Clearing games directory");
                    Directory.Delete(tempGamesDirectory, true);
                    Directory.CreateDirectory(tempGamesDirectory);
                }
                var dirs = Directory.GetDirectories(hakchiDirectory);
                foreach (var dir in dirs)
                    Directory.Delete(dir, true);
                var files = from f in Directory.GetFiles(hakchiDirectory) where (Path.GetFileName(f) != "init" && Path.GetFileName(f) != "config") select f;
                foreach (var file in files)
                    File.Delete(file);
            }

            // Games!
            if (Games != null)
            {
                stats.Next();
                AddMenu(Games, stats);
                Debug.WriteLine(string.Format("Games copied: {0}/{1}, part size: {2}", stats.GamesProceed, stats.GamesTotal, stats.Size));
            }

            bool last = stats.GamesProceed >= stats.GamesTotal;

            if (last)
            {
                if (!string.IsNullOrEmpty(ExtraCommandLineArguments))
                {
                    File.WriteAllText(argumentsFilePath, ExtraCommandLineArguments);
                }
            }

            // Remove thumbnails
            if (Config != null && Config.ContainsKey("hakchi_remove_thumbnails") && Config["hakchi_remove_thumbnails"])
            {
                var thumbnails = Directory.GetFiles(tempGamesDirectory, "*_small.png", SearchOption.AllDirectories);
                foreach (var t in thumbnails)
                    File.WriteAllBytes(t, new byte[0]);
            }

            // Writing config files
            if (Config != null)
            {
                Config["hakchi_partial_first"] = first;
                Config["hakchi_partial_last"] = last;
                var config = new StringBuilder();

                foreach (var key in Config.Keys)
                    config.AppendFormat("{0}={1}\n", key, Config[key] ? 'y' : 'n');
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
            return result;
        }

        void DownloadAllCovers()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesGame game in Games)
            {
                SetStatus(Resources.GooglingFor + " " + game.Name + ImageGooglerForm.Suffix);
                string[] urls = null;
                for (int tries = 0; tries < 5; tries++)
                {
                    if (urls == null)
                    {
                        try
                        {
                            urls = ImageGooglerForm.GetImageUrls(game.Name + ImageGooglerForm.Suffix);
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
                        game.SetImage(cover, ConfigIni.EightBitPngCompression);
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
            public int GamesTotal = 0;
            public int GamesStart = 0;
            public int GamesProceed = 0;
            public long Size = 0;

            public void Next()
            {
                allMenus.Clear();
                GamesStart = GamesProceed;
                GamesTotal = 0;
                GamesProceed = 0;
                Size = 0;
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
                targetDirectory = Path.Combine(tempGamesDirectory, string.Format("sub{0:D3}", menuIndex));
            foreach (var element in menuCollection)
            {
                if (element is NesGame)
                {
                    stats.GamesTotal++;
                    if (stats.Size >= maxRamfsSize) continue;
                    stats.GamesProceed++;
                    if (stats.GamesStart >= stats.GamesProceed) continue;
                    var game = element as NesGame;
                    var gameDir = Path.Combine(targetDirectory, game.Code);
                    Debug.Write(string.Format("Processing {0} ('{1}'), #{2}", game.Code, game.Name, stats.GamesProceed));
                    stats.Size += DirectoryCopy(game.GamePath, gameDir, true);
                    /*
                    if (stats.Size >= maxRamfsSize)
                    {
                        // Rollback. Just in case of huge last game
                        stats.GamesProceed--;
                        Directory.Delete(gameDir, true);
                        continue;
                    }
                     */
                    Debug.WriteLine(string.Format(", total size: {0}", stats.Size));
                    if (!string.IsNullOrEmpty(game.GameGenie))
                    {
                        var codes = game.GameGenie.Split(new char[] { ',', '\t', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        var newNesFilePath = Path.Combine(gameDir, game.Code + ".nes");
                        try
                        {
                            var nesFile = new NesFile(newNesFilePath);
                            foreach (var code in codes)
                            {
                                try
                                {
                                    nesFile.PRG = GameGenie.Patch(nesFile.PRG, code.Trim());
                                }
                                catch (GameGenieFormatException)
                                {
                                    ShowError(new GameGenieFormatException(string.Format(Resources.GameGenieFormatError, code, game)), dontStop: true);
                                }
                                catch (GameGenieNotFoundException)
                                {
                                    ShowError(new GameGenieNotFoundException(string.Format(Resources.GameGenieNotFound, code, game.Name)), dontStop: true);
                                }
                            }
                            nesFile.Save(newNesFilePath);
                            var ggFilePath = Path.Combine(gameDir, NesGame.GameGenieFileName);
                            if (File.Exists(ggFilePath)) File.Delete(ggFilePath);
                        }
                        catch // in case of FDS game... just ignore
                        {
                        }
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
                        int childIndex = stats.allMenus.IndexOf(folder.ChildMenuCollection);
                        var folderDir = Path.Combine(targetDirectory, folder.Code);
                        folder.Save(folderDir, childIndex);
                    }
                }
                if (element is NesDefaultGame)
                {
                    if (stats.GamesStart == 0)
                    {
                        var game = element as NesDefaultGame;
                        var gfilePath = Path.Combine(tempGamesDirectory, string.Format("gpath-{0}-{1}", game.Code, menuIndex));
                        Directory.CreateDirectory(Path.GetDirectoryName(gfilePath));
                        File.WriteAllText(gfilePath, menuIndex == 0 ? "." : string.Format("sub{0:D3}", menuIndex));
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

        private static long DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            long size = 0;
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                size += file.CopyTo(temppath, true).Length;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    size += DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
            return size;
        }

        bool YesForAllPatches = false;
        public NesGame AddGames(string gamesDirectory, string[] files, Form parentForm = null)
        {
            NesGame nesGame = null;
            bool NoForAllUnsupportedMappers = false;
            YesForAllPatches = false;
            if (parentForm == null) parentForm = this;
            int count = 0;
            SetStatus(Resources.AddingGames);
            foreach (var file in files)
            {
                try
                {
                    var nesFileName = file;
                    var ext = Path.GetExtension(file).ToLower();
                    bool? needPatch = YesForAllPatches ? (bool?)true : null;
                    byte[] rawData = null;
                    if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                    {
                        SevenZipExtractor.SetLibraryPath(Path.Combine(baseDirectory, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        var szExtractor = new SevenZipExtractor(file);
                        var filesInArchive = new List<string>();
                        foreach (var f in szExtractor.ArchiveFileNames)
                        {
                            var e = Path.GetExtension(f).ToLower();
                            if (e == ".nes" || e == ".fds")
                                filesInArchive.Add(f);
                        }
                        if (filesInArchive.Count == 1)
                        {
                            nesFileName = filesInArchive[0];
                        }
                        else
                        {
                            var fsForm = new SelectFileForm(filesInArchive.ToArray());
                            if (fsForm.ShowDialog() == DialogResult.OK)
                                nesFileName = (string)fsForm.listBoxFiles.SelectedItem;
                            else
                                continue;
                        }
                        var o = new MemoryStream();
                        szExtractor.ExtractFile(nesFileName, o);
                        rawData = new byte[szExtractor.ArchiveFileData[0].Size];
                        o.Seek(0, SeekOrigin.Begin);
                        o.Read(rawData, 0, rawData.Length);
                    }
                    try
                    {
                        nesGame = new NesGame(gamesDirectory, nesFileName, NoForAllUnsupportedMappers ? (bool?)false : null, ref needPatch, needPatchCallback, this, rawData);

                        // Trying to import Game Genie codes
                        var lGameGeniePath = Path.Combine(Path.GetDirectoryName(nesFileName), Path.GetFileNameWithoutExtension(nesFileName) + ".xml");
                        if (File.Exists(lGameGeniePath))
                        {
                            GameGenieDataBase lGameGenieDataBase = new GameGenieDataBase(nesGame);
                            lGameGenieDataBase.ImportCodes(lGameGeniePath, true);
                            lGameGenieDataBase.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is UnsupportedMapperException || ex is UnsupportedFourScreenException)
                        {
                            MessageBoxFromThread(this,
                                (ex is UnsupportedMapperException)
                                   ? string.Format(Resources.MapperNotSupported, Path.GetFileName(file), (ex as UnsupportedMapperException).ROM.Mapper)
                                   : string.Format(Resources.FourScreenNotSupported, Path.GetFileName(file)),
                                Resources.AreYouSure,
                                files.Length <= 1 ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                            while (MessageBoxResult == DialogResult.None) Thread.Sleep(100);
                            if (MessageBoxResult == DialogResult.Yes)
                                nesGame = new NesGame(gamesDirectory, nesFileName, true, ref needPatch, needPatchCallback, this, rawData);
                            else if (MessageBoxResult == System.Windows.Forms.DialogResult.Cancel)
                            {
                                NoForAllUnsupportedMappers = true;
                            }
                        }
                        else throw ex;
                    }
                    ConfigIni.SelectedGames += ";" + nesGame.Code;
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException) return null;
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBoxFromThread(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                SetProgress(++count, files.Length);
            }
            return nesGame; // Last added game if any
        }

        private bool needPatchCallback(Form parentForm, string nesFileName)
        {
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Paused);
            if (GamesToAdd == null || GamesToAdd.Length <= 1)
            {
                MessageBoxFromThread(parentForm,
                    string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Resources.PatchAvailable,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);
                while (MessageBoxResult == DialogResult.None) Thread.Sleep(100);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                return MessageBoxResult == DialogResult.Yes;
            }
            else
            {
                MessageBoxFromThread(parentForm,
                    string.Format(Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Resources.PatchAvailable,
                    MessageBoxButtons.AbortRetryIgnore,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                while (MessageBoxResult == DialogResult.None) Thread.Sleep(100);
                if (MessageBoxResult == DialogResult.Abort)
                    YesForAllPatches = true;
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
                return MessageBoxResult != DialogResult.Ignore;
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
