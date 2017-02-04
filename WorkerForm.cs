using com.clusterrr.Famicom;
using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
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
        public enum Tasks { DumpKernel, FlashKernel, Memboot, DownloadAllCovers };
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
        Thread thread = null;
        Fel fel = null;

        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;

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
        readonly string gamesDirectory;
        readonly string cloverconDriverPath;
        readonly string argumentsFilePath;
        string[] correctKernels;
        const int maxRamfsSize = 40 * 1024 * 1024;
        DialogResult DeviceWaitResult = DialogResult.None;

        public WorkerForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            fes1Path = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "fes1.bin");
            ubootPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "uboot.bin");
            tempDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "temp");
            kernelDirectory = Path.Combine(tempDirectory, "kernel");
            initramfs_cpio = Path.Combine(kernelDirectory, "initramfs.cpio");
            initramfs_cpioPatched = Path.Combine(kernelDirectory, "initramfs_mod.cpio");
            ramfsDirectory = Path.Combine(kernelDirectory, "initramfs");
            hakchiDirectory = Path.Combine(ramfsDirectory, "hakchi");
            modsDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "mods");
            toolsDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "tools");
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
            gamesDirectory = Path.Combine(ramfsDirectory, "games");
        }

        public DialogResult Start()
        {
            SetProgress(0, 1);
            if (Task != Tasks.DownloadAllCovers)
            {
                if (!WaitingForm.WaitForDevice(vid, pid))
                {
                    DialogResult = DialogResult.Abort;
                    return DialogResult;
                }
            }
            thread = new Thread(StartThread);
            thread.Start();
            return ShowDialog();
        }

        void WaitForDeviceFromThread()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(WaitForDeviceFromThread));
                return;
            }
            if (fel != null)
                fel.Close();
            DeviceWaitResult = WaitingForm.WaitForDevice(vid, pid) ? DialogResult.OK : DialogResult.Abort;
        }

        public void StartThread()
        {
            fel = new Fel();
            SetProgress(0, 1);
            try
            {
                if (Task != Tasks.DownloadAllCovers)
                {
                    if (!File.Exists(fes1Path)) throw new FileNotFoundException(fes1Path + " not found");
                    if (!File.Exists(ubootPath)) throw new FileNotFoundException(ubootPath + " not found");
                    fel.Fes1Bin = File.ReadAllBytes(fes1Path);
                    fel.UBootBin = File.ReadAllBytes(ubootPath);
                    fel.Open(vid, pid);
                    SetStatus(Resources.UploadingFes1);
                    fel.InitDram(true);
                }
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
                Debug.WriteLine(ex.Message + ex.StackTrace);
                if (ex is GameGenieFormatException || ex is GameGenieNotFoundException)
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is MadWizard.WinUSBNet.USBException)
                    MessageBox.Show(this, ex.Message + "\r\n" + Resources.PleaseTryAgainUSB, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(this, text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
        }

        public void DoKernelDump()
        {
            int progress = 5;
            const int maxProgress = 80;
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
            int progress = 5;
            int maxProgress = 120 + (string.IsNullOrEmpty(Mod) ? 0 : 5);
            SetProgress(progress, maxProgress);

            byte[] kernel;
            int pos = 0, totalFiles;
            if (!string.IsNullOrEmpty(Mod))
            {
                kernel = CreatePatchedKernel(ref pos, out totalFiles);
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
            int pos = 0, totalFiles = 0;
            int progress = 5;
            SetProgress(progress, 300);
            int maxProgress = -1;

            do
            {
                if (pos > 0)
                {
                    ShowMessage(Resources.ParticallyBody, Resources.ParticallyTitle);
                    DeviceWaitResult = DialogResult.None;
                    WaitForDeviceFromThread();
                    while (DeviceWaitResult == DialogResult.None)
                        Thread.Sleep(500);
                    if (DeviceWaitResult != DialogResult.OK)
                    {
                        DialogResult = DialogResult.Abort;
                        return;
                    }
                    fel = new Fel();
                    fel.Fes1Bin = File.ReadAllBytes(fes1Path);
                    fel.UBootBin = File.ReadAllBytes(ubootPath);
                    fel.Open(vid, pid);
                    SetStatus(Resources.UploadingFes1);
                    fel.InitDram(true);
                }

                byte[] kernel;
                if (!string.IsNullOrEmpty(Mod))
                    kernel = CreatePatchedKernel(ref pos, out totalFiles);
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
                    if (pos > 0)
                        maxProgress = (kernel.Length / 67000 + 15) * totalFiles / pos + 75 * ((int)Math.Ceiling((float)totalFiles / (float)pos) - 1);
                    else
                        maxProgress = (kernel.Length / 67000 + 15);
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
            } while (pos < totalFiles);
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        private byte[] CreatePatchedKernel(ref int filesPos, out int totalFiles)
        {
            bool first = filesPos == 0;
            bool partial = filesPos > 0;
            SetStatus(Resources.BuildingCustom);
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

            if (Games != null)
                AddMenu(Games);
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
            if (!string.IsNullOrEmpty(ExtraCommandLineArguments))
            {
                File.WriteAllText(argumentsFilePath, ExtraCommandLineArguments);
            }

            if (Config != null && Config.ContainsKey("hakchi_remove_thumbnails") && Config["hakchi_remove_thumbnails"])
            {
                var thumbnails = Directory.GetFiles(gamesDirectory, "*_small.png", SearchOption.AllDirectories);
                foreach (var t in thumbnails)
                    File.WriteAllBytes(t, new byte[0]);
            }

            var romFiles = new List<string>();
            romFiles.AddRange(Directory.GetFiles(ramfsDirectory, "*.nes", SearchOption.AllDirectories));
            romFiles.AddRange(Directory.GetFiles(ramfsDirectory, "*.desktop", SearchOption.AllDirectories));
            romFiles.AddRange(Directory.GetFiles(ramfsDirectory, "*.png", SearchOption.AllDirectories));
            ramfsFiles = romFiles.OrderBy(o => o).ToArray();
            totalFiles = ramfsFiles.Length;
            for (int i = 0; i < filesPos; i++)
                File.Delete(ramfsFiles[i]);
            long size = 0;
            while (filesPos < totalFiles)
            {
                var fsize = new FileInfo(ramfsFiles[filesPos]).Length;
                if (size >= maxRamfsSize) break;
                size += fsize;
                filesPos++;
            }
            for (int i = filesPos; i < totalFiles; i++)
                File.Delete(ramfsFiles[i]);

            if (Config != null)
            {
                Config["hakchi_partial_first"] = first;
                Config["hakchi_partial_last"] = filesPos >= totalFiles;
                var config = new StringBuilder();

                foreach (var key in Config.Keys)
                    config.AppendFormat("{0}={1}\n", key, Config[key] ? 'y' : 'n');
                File.WriteAllText(configPath, config.ToString());
            }

            if (Games != null && Games.Count > 0) // There is no reason to compress cryptsetup when we do not uploading games
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

        private void AddMenu(NesMenuCollection menuCollection, List<NesMenuCollection> allMenus = null)
        {
            if (allMenus == null)
                allMenus = new List<NesMenuCollection>();
            if (!allMenus.Contains(menuCollection))
                allMenus.Add(menuCollection);
            int menuIndex = allMenus.IndexOf(menuCollection);
            string targetDirectory;
            if (menuIndex == 0)
                targetDirectory = gamesDirectory;
            else
                targetDirectory = Path.Combine(gamesDirectory, string.Format("sub{0:D3}", menuIndex));
            if (Directory.Exists(targetDirectory))
                return;
            foreach (var element in menuCollection)
            {
                if (element is NesGame)
                {
                    var game = element as NesGame;
                    var gameDir = Path.Combine(targetDirectory, game.Code);
                    Debug.WriteLine(string.Format("Processing {0}/{1}", game.Code, game.Name));
                    DirectoryCopy(game.GamePath, gameDir, true);
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
                    if (!allMenus.Contains(folder.Child))
                        allMenus.Add(folder.Child);
                    int childIndex = allMenus.IndexOf(folder.Child);
                    var folderDir = Path.Combine(targetDirectory, folder.Code);
                    folder.Save(folderDir, childIndex);
                    AddMenu(folder.Child, allMenus);
                }
                if (element is NesDefaultGame)
                {
                    var game = element as NesDefaultGame;
                    var gfilePath = Path.Combine(gamesDirectory, string.Format("gpath-{0}-{1}", game.Code, menuIndex));
                    Directory.CreateDirectory(Path.GetDirectoryName(gfilePath));
                    File.WriteAllText(gfilePath, menuIndex == 0 ? "." : string.Format("sub{0:D3}", menuIndex));
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
            var appDirectory = Path.GetDirectoryName(Application.ExecutablePath);
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

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
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
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void WorkerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((thread != null) && (e.CloseReason == CloseReason.UserClosing))
            {
                if (MessageBox.Show(this, Resources.DoYouWantCancel, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
                else if (thread != null) thread.Abort();
            }
        }
    }
}
