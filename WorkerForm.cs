using com.clusterrr.Famicom;
using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WorkerForm : Form
    {
        public enum Tasks { DumpKernel, FlashKernel, DumpNand, FlashNand, DumpNandB, DumpNandC, FlashNandC, Memboot, UploadGames, DownloadCovers, AddGames, CompressGames, DecompressGames, DeleteGames };
        public Tasks Task;
        //public string UBootDump;
        public static string KernelDumpPath
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "dump"), "kernel.img");
                    case MainForm.ConsoleType.Famicom:
                        return Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "dump"), "kernel_famicom.img");
                    case MainForm.ConsoleType.SNES:
                        return Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "dump"), "kernel_snes.img");
                    case MainForm.ConsoleType.SuperFamicom:
                        return Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "dump"), "kernel_super_famicom.img");
                }
            }
        }
        public string NandDump;
        public string Mod = null;
        public Dictionary<string, string> Config = null;
        public NesMenuCollection Games;
        public IEnumerable<string> hmodsInstall;
        public IEnumerable<string> hmodsUninstall;
        public IEnumerable<string> GamesToAdd;
        public NesMenuCollection.SplitStyle FoldersMode = NesMenuCollection.SplitStyle.Auto;
        public int MaxGamesPerFolder = 35;
        private MainForm MainForm;
        Thread thread = null;
        Fel fel = null;

        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;

        readonly string baseDirectoryInternal;
        readonly string baseDirectoryExternal;
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
        readonly string tempHmodsDirectory;
        readonly string argumentsFilePath;
        readonly string transferDirectory;
        string tempGamesDirectory;
        //string originalGamesConfigDirectory;
        //string hiddenPath;
        Dictionary<MainForm.ConsoleType, string[]> correctKernels = new Dictionary<MainForm.ConsoleType, string[]>();
        Dictionary<MainForm.ConsoleType, string[]> correctKeys = new Dictionary<MainForm.ConsoleType, string[]>();
        const long maxCompressedsRamfsSize = 30 * 1024 * 1024;
        string selectedFile = null;
        public NesMiniApplication[] addedApplications;
        public static int NandCTotal, NandCUsed, NandCFree, WritedGamesSize, SaveStatesSize;
        public static bool ExternalSaves = false;
        public static long ReservedMemory
        {
            get
            {
                if (ExternalSaves)
                    return 5;
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return 10;
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return 30;
                }
            }
        }

        public WorkerForm(MainForm parentForm)
        {
            InitializeComponent();
            MainForm = parentForm;
            DialogResult = DialogResult.None;
            baseDirectoryInternal = Program.BaseDirectoryInternal;
            baseDirectoryExternal = Program.BaseDirectoryExternal;
            fes1Path = Path.Combine(Path.Combine(baseDirectoryInternal, "data"), "fes1.bin");
            ubootPath = Path.Combine(Path.Combine(baseDirectoryInternal, "data"), "uboot.bin");
#if DEBUG
            tempDirectory = Path.Combine(baseDirectoryInternal, "temp");
#else
            tempDirectory = Path.Combine(Path.GetTempPath(), "hakchi-temp");
#endif
            kernelDirectory = Path.Combine(tempDirectory, "kernel");
            initramfs_cpio = Path.Combine(kernelDirectory, "initramfs.cpio");
            initramfs_cpioPatched = Path.Combine(kernelDirectory, "initramfs_mod.cpio");
            ramfsDirectory = Path.Combine(kernelDirectory, "initramfs");
            hakchiDirectory = Path.Combine(ramfsDirectory, "hakchi");
            modsDirectory = Path.Combine(baseDirectoryInternal, "mods");
            hmodDirectories = new string[] {
                Path.Combine(baseDirectoryExternal, "user_mods"),
                Path.Combine(modsDirectory, "hmods")
            };
            toolsDirectory = Path.Combine(baseDirectoryInternal, "tools");
            kernelPatched = Path.Combine(kernelDirectory, "patched_kernel.img");
            ramdiskPatched = Path.Combine(kernelDirectory, "kernel.img-ramdisk_mod.gz");
            argumentsFilePath = Path.Combine(hakchiDirectory, "extra_args");
            transferDirectory = Path.Combine(hakchiDirectory, "transfer");
            tempHmodsDirectory = Path.Combine(transferDirectory, "hmod");

            correctKernels[MainForm.ConsoleType.NES] = new string[] {
                "5cfdca351484e7025648abc3b20032ff",
                "07bfb800beba6ef619c29990d14b5158",
            };
            correctKernels[MainForm.ConsoleType.Famicom] = new string[] {
                "ac8144c3ea4ab32e017648ee80bdc230",  // Famicom Mini
            };
            correctKernels[MainForm.ConsoleType.SNES] = new string[] {
                "d76c2a091ebe7b4614589fc6954653a5", // SNES Mini (EUR)
                "c2b57b550f35d64d1c6ce66f9b5180ce", // SNES Mini (EUR)
                "0f890bc78cbd9ede43b83b015ba4c022", // SNES Mini (EUR)
                "449b711238575763c6701f5958323d48", // SNES Mini (USA)
                "5296e64818bf2d1dbdc6b594f3eefd17", // SNES Mini (USA)
                "228967ab1035a347caa9c880419df487", // SNES Mini (USA)
            };
            correctKernels[MainForm.ConsoleType.SuperFamicom] = new string[]
            {
                "632e179db63d9bcd42281f776a030c14", // Super Famicom Mini (JAP)
                "c3378edfc1b96a5268a066d5fbe12d89", // Super Famicom Mini (JAP)
            };
            correctKeys[MainForm.ConsoleType.NES] =
                correctKeys[MainForm.ConsoleType.Famicom] =
                new string[] { "bb8f49e0ae5acc8d5f9b7fa40efbd3e7" };
            correctKeys[MainForm.ConsoleType.SNES] =
                correctKeys[MainForm.ConsoleType.SuperFamicom] =
                new string[] { "c5dbb6e29ea57046579cfd50b124c9e1" };
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

        DialogResult WaitForFelFromThread()
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<DialogResult>(WaitForFelFromThread));
            }
            SetStatus(Resources.WaitingForDevice);
            if (fel != null)
                fel.Close();
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
            var result = WaitingFelForm.WaitForDevice(vid, pid, this);
            if (result)
            {
                fel = new Fel();
                if (!File.Exists(fes1Path)) throw new FileNotFoundException(fes1Path + " not found");
                if (!File.Exists(ubootPath)) throw new FileNotFoundException(ubootPath + " not found");
                fel.Fes1Bin = File.ReadAllBytes(fes1Path);
                fel.UBootBin = File.ReadAllBytes(ubootPath);
                if (!fel.Open(vid, pid))
                    throw new FelException("Can't open device");
                SetStatus(Resources.UploadingFes1);
                fel.InitDram(true);
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
                return DialogResult.OK;
            }
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            return DialogResult.Abort;
        }
        DialogResult WaitForClovershellFromThread()
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<DialogResult>(WaitForClovershellFromThread));
            }
            SetStatus(Resources.WaitingForDevice);
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
            var result = WaitingClovershellForm.WaitForDevice(this);
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            if (result)
                return DialogResult.OK;
            else return DialogResult.Abort;
        }

        private delegate DialogResult MessageBoxFromThreadDelegate(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak);
        public static DialogResult MessageBoxFromThread(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak)
        {
            if ((owner as Form).InvokeRequired)
            {
                return (DialogResult)(owner as Form).Invoke(new MessageBoxFromThreadDelegate(MessageBoxFromThread),
                    new object[] { owner, text, caption, buttons, icon, defaultButton, tweak });
            }
            TaskbarProgress.SetState(owner as Form, TaskbarProgress.TaskbarStates.Paused);
            if (tweak) MessageBoxManager.Register(); // Tweak button names
            var result = MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
            if (tweak) MessageBoxManager.Unregister();
            TaskbarProgress.SetState(owner as Form, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        DialogResult FoldersManagerFromThread(NesMenuCollection collection)
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<NesMenuCollection, DialogResult>(FoldersManagerFromThread), new object[] { collection });
            }
            var constructor = new FoldersManagerForm(collection, MainForm);
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
            var result = constructor.ShowDialog();
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        DialogResult SelectFileFromThread(string[] files)
        {
            if (InvokeRequired)
            {
                return (DialogResult)Invoke(new Func<string[], DialogResult>(SelectFileFromThread), new object[] { files });
            }
            var form = new SelectFileForm(files);
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
            var result = form.ShowDialog();
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            if (form.listBoxFiles.SelectedItem != null)
                selectedFile = form.listBoxFiles.SelectedItem.ToString();
            else
                selectedFile = null;
            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            return result;
        }

        public void StartThread()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Language);
            SetProgress(0, 1);
            try
            {
                DialogResult = DialogResult.None;
                Debug.WriteLine("Executing task: " + Task.ToString());
                switch (Task)
                {
                    case Tasks.DumpKernel:
                        DoKernelDump();
                        break;
                    case Tasks.FlashKernel:
                        FlashKernel();
                        break;
                    case Tasks.DumpNand:
                        DoNandDump();
                        break;
                    case Tasks.FlashNand:
                        DoNandFlash();
                        break;
                    case Tasks.DumpNandB:
                    case Tasks.DumpNandC:
                    case Tasks.FlashNandC:
                        DoPartitionDump(Task);
                        break;
                    case Tasks.UploadGames:
                        UploadGames();
                        break;
                    case Tasks.Memboot:
                        Memboot();
                        break;
                    case Tasks.AddGames:
                        AddGames(GamesToAdd);
                        break;
                    case Tasks.DownloadCovers:
                        DownloadCovers();
                        break;
                    case Tasks.CompressGames:
                        CompressGames();
                        break;
                    case Tasks.DecompressGames:
                        DecompressGames();
                        break;
                    case Tasks.DeleteGames:
                        DeleteGames();
                        break;
                }
                if (DialogResult == DialogResult.None)
                    DialogResult = DialogResult.OK;
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    Debug.WriteLine(ex.InnerException.Message + ex.InnerException.StackTrace);
                    ShowError(ex.InnerException);
                }
                else
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    ShowError(ex);
                }
            }
            finally
            {
                thread = null;
                if (fel != null)
                {
                    fel.Close();
                    fel = null;
                }
                GC.Collect();
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
                    if (value == max)
                        Thread.Sleep(1000);
                    return;
                }
                if (value > max) value = max;
                progressBar.Maximum = max;
                progressBar.Value = value;
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
                TaskbarProgress.SetValue(this, value, max);
            }
            catch { }
        }

        void ShowError(Exception ex, bool dontStop = false, string prefix = null)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Exception, bool, string>(ShowError), new object[] { ex, dontStop, prefix });
                    return;
                }
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Error);
                var message = ex.Message;
#if DEBUG
                message += ex.StackTrace;
#endif
                Debug.WriteLine(ex.Message + ex.StackTrace);
                //if (ex is MadWizard.WinUSBNet.USBException) // TODO
                //    MessageBox.Show(this, message + "\r\n" + Resources.PleaseTryAgainUSB, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //else
                MessageBox.Show(this, (!string.IsNullOrEmpty(prefix) ? (prefix + ": ") : "") + message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
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
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
                MessageBox.Show(this, text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
            }
            catch { }
        }

        public bool DoKernelDump(string dumpPath = null, int maxProgress = 80, int progress = 0)
        {
            if (WaitForFelFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return false;
            }
            progress += 5;
            SetProgress(progress, maxProgress);

            SetStatus(Resources.DumpingKernel);
            var kernel = fel.ReadFlash(Fel.kernel_base_f, Fel.sector_size * 0x20,
                delegate (Fel.CurrentAction action, string command)
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

            var size = CalcKernelSize(kernel);
            if (size == 0 || size > Fel.kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);
            if (kernel.Length > size)
            {
                var sm_kernel = new byte[size];
                Array.Copy(kernel, 0, sm_kernel, 0, size);
                kernel = sm_kernel;
            }

            if (Task == Tasks.DumpKernel)
            {
                SetProgress(maxProgress, maxProgress);
                SetStatus(Resources.Done);
            }

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(kernel)).Replace("-", "").ToLower();
            var matchedKernels = from k in correctKernels where k.Value.Contains(hash) select k.Key;
            if (matchedKernels.Count() == 0)
            {
                // Unknown MD5? Hmm... Lets extract ramfs and check keyfile!
                string kernelDumpTemp = Path.Combine(tempDirectory, "kernel.img");
                Directory.CreateDirectory(tempDirectory);
                File.WriteAllBytes(kernelDumpTemp, kernel);
                UnpackRamfs(kernelDumpTemp);
                var key = File.ReadAllBytes(Path.Combine(ramfsDirectory, "key-file"));
                if (dumpPath == null)
                    Directory.Delete(tempDirectory, true);
                // I don't want to store keyfile inside my code, so I'll store MD5 of it
                var keymd5 = System.Security.Cryptography.MD5.Create();
                var keyhash = BitConverter.ToString(md5.ComputeHash(key)).Replace("-", "").ToLower();
                // Lets try to autodetect console using key hash
                var matchedKeys = from k in correctKeys where k.Value.Contains(keyhash) select k.Key;
                if (matchedKeys.Count() > 0)
                {
                    if (!matchedKeys.Contains(ConfigIni.ConsoleType))
                        throw new Exception(Resources.InvalidConsoleSelected + " " + matchedKernels.First());
                }
                else throw new Exception("Unknown key, unknown console");

                if (MessageBoxFromThread(this, Resources.MD5Failed + " " + hash + /*"\r\n" + Resources.MD5Failed2 +*/
                    "\r\n" + Resources.DoYouWantToContinue, Resources.Warning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, false)
                    == DialogResult.No)
                {
                    DialogResult = DialogResult.Abort;
                    return false;
                }
            }
            else
            {
                // Lets try to autodetect console using kernel hash
                if (!matchedKernels.Contains(ConfigIni.ConsoleType))
                    throw new Exception(Resources.InvalidConsoleSelected + " " + matchedKernels.First());
            }

            Directory.CreateDirectory(Path.GetDirectoryName(KernelDumpPath));
            if (!File.Exists(KernelDumpPath))
                File.WriteAllBytes(KernelDumpPath, kernel);
            if (!string.IsNullOrEmpty(dumpPath))
                File.WriteAllBytes(dumpPath, kernel);
            return true;
        }

        public void FlashKernel()
        {
            int progress = 0;
            int maxProgress = 115 + (string.IsNullOrEmpty(Mod) ? 0 : 110) +
                ((hmodsInstall != null && hmodsInstall.Count() > 0) ? 150 : 0);
            var tempKernelPath = Path.Combine(tempDirectory, "kernel.img");
            var hmods = hmodsInstall;
            hmodsInstall = null;
            if (WaitForFelFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            progress += 5;
            SetProgress(progress, maxProgress);

            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
            Directory.CreateDirectory(tempDirectory);

            byte[] kernel;
            if (!string.IsNullOrEmpty(Mod))
            {
                if (!DoKernelDump(tempKernelPath, maxProgress, progress))
                    return;
                progress += 80;
                kernel = CreatePatchedKernel(tempKernelPath);
                progress += 5;
                SetProgress(progress, maxProgress);
            }
            else
                kernel = File.ReadAllBytes(KernelDumpPath);
            var size = CalcKernelSize(kernel);
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
                delegate (Fel.CurrentAction action, string command)
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
                delegate (Fel.CurrentAction action, string command)
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

            hmodsInstall = hmods;
            if (hmodsInstall != null && hmodsInstall.Count() > 0)
            {
                Memboot(maxProgress, progress); // Lets install some mods                
            }
            else
            {
                var shutdownCommand = "shutdown";
                SetStatus(Resources.ExecutingCommand + " " + shutdownCommand);
                fel.RunUbootCmd(shutdownCommand, true);
#if !DEBUG
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);
#endif
                SetStatus(Resources.Done);
                SetProgress(maxProgress, maxProgress);
            }
        }

        public void DoNandDump()
        {
            int progress = 0;
            const int maxProgress = 8373;
            if (WaitForFelFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            progress += 5;
            SetProgress(progress, maxProgress);

            SetStatus(Resources.DumpingNand);
            var kernel = fel.ReadFlash(0, Fel.sector_size * 0x1000,
                delegate (Fel.CurrentAction action, string command)
                {
                    switch (action)
                    {
                        case Fel.CurrentAction.RunningCommand:
                            SetStatus(Resources.ExecutingCommand + " " + command);
                            break;
                        case Fel.CurrentAction.ReadingMemory:
                            SetStatus(Resources.DumpingNand);
                            break;
                    }
                    progress++;
                    SetProgress(progress, maxProgress);
                }
            );

            SetProgress(maxProgress, maxProgress);
            SetStatus(Resources.Done);

            Directory.CreateDirectory(Path.GetDirectoryName(NandDump));
            File.WriteAllBytes(NandDump, kernel);
        }

        public void DoNandFlash()
        {
            int progress = 0;
            const int maxProgress = 9605;
            if (WaitForFelFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            progress += 5;
            SetProgress(progress, maxProgress);

            var nand = File.ReadAllBytes(NandDump);
            if (nand.Length != 512 * 1024 * 1024)
                throw new Exception("Invalid NAND size");

            SetStatus("...");
            fel.WriteFlash(0, nand,
                delegate (Fel.CurrentAction action, string command)
                {
                    switch (action)
                    {
                        case Fel.CurrentAction.RunningCommand:
                            SetStatus(Resources.ExecutingCommand + " " + command);
                            break;
                        case Fel.CurrentAction.WritingMemory:
                            SetStatus("...");
                            break;
                    }
                    progress++;
                    SetProgress(progress, maxProgress);
                }
            );

            var shutdownCommand = "shutdown";
            SetStatus(Resources.ExecutingCommand + " " + shutdownCommand);
            fel.RunUbootCmd(shutdownCommand, true);
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        public void DoPartitionDump(Tasks task)
        {
            int progress = 0;
            int maxProgress = 500;
            var clovershell = MainForm.Clovershell;
            try
            {
                if (WaitForClovershellFromThread() != DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
                progress += 5;
                SetProgress(progress, maxProgress);

                ShowSplashScreen();

                var partitionSize = 0;
                switch (task)
                {
                    case Tasks.DumpNandB:
                        partitionSize = int.Parse(clovershell.ExecuteSimple("df /dev/mapper/root-crypt | tail -n 1 | awk '{ print $2 }'"));
                        break;
                    case Tasks.DumpNandC:
                    case Tasks.FlashNandC:
                        partitionSize = int.Parse(clovershell.ExecuteSimple("df /dev/nandc | tail -n 1 | awk '{ print $2 }'"));
                        break;
                }
                maxProgress = 5 + (int)Math.Ceiling(partitionSize / 1024.0 * 1.05);
                SetProgress(progress, maxProgress);

                if (task != Tasks.FlashNandC)
                {
                    SetStatus(Resources.DumpingNand);
                    using (var file = new TrackableFileStream(NandDump, FileMode.Create))
                    {
                        file.OnProgress += delegate (long Position, long Length)
                        {
                            progress = (int)(5 + Position / 1024 / 1024);
                            SetProgress(progress, maxProgress);
                        };
                        switch (task)
                        {
                            case Tasks.DumpNandB:
                                clovershell.Execute("dd if=/dev/mapper/root-crypt", null, file);
                                break;
                            case Tasks.DumpNandC:
                                clovershell.Execute("dd if=/dev/nandc", null, file);
                                break;
                        }
                        file.Close();
                    }
                }
                else
                {
                    SetStatus(Resources.FlashingNand);
                    using (var file = new TrackableFileStream(NandDump, FileMode.Open))
                    {
                        file.OnProgress += delegate (long Position, long Length)
                        {
                            progress = (int)(5 + Position / 1024 / 1024);
                            SetProgress(progress, maxProgress);
                        };
                        clovershell.Execute("dd of=/dev/nandc", file);
                        file.Close();
                    }
                }

                SetStatus(Resources.Done);
                SetProgress(maxProgress, maxProgress);
            }
            finally
            {
                try
                {
                    if (clovershell.IsOnline)
                        clovershell.ExecuteSimple("reboot", 100);
                }
                catch { }
            }
        }

        private void File_OnProgress(long Position, long Length)
        {
            throw new NotImplementedException();
        }

        public static void GetMemoryStats()
        {
            var clovershell = MainForm.Clovershell;
            var nandc = clovershell.ExecuteSimple("df /dev/nandc | tail -n 1 | awk '{ print $2 \" | \" $3 \" | \" $4 }'", 500, true).Split('|');
            ExternalSaves = clovershell.ExecuteSimple("mount | grep /var/lib/clover").Trim().Length > 0;
            WritedGamesSize = int.Parse(clovershell.ExecuteSimple("mkdir -p /var/lib/hakchi/rootfs/usr/share/games/ && du -s /var/lib/hakchi/rootfs/usr/share/games/ | awk '{ print $1 }'", 1000, true)) * 1024;
            SaveStatesSize = int.Parse(clovershell.ExecuteSimple("mkdir -p /var/lib/clover/profiles/0/ && du -s /var/lib/clover/profiles/0/ | awk '{ print $1 }'", 1000, true)) * 1024;
            NandCTotal = int.Parse(nandc[0]) * 1024;
            NandCUsed = int.Parse(nandc[1]) * 1024;
            NandCFree = int.Parse(nandc[2]) * 1024;
            Debug.WriteLine(string.Format("NANDC size: {0:F1}MB, used: {1:F1}MB, free: {2:F1}MB", NandCTotal / 1024.0 / 1024.0, NandCUsed / 1024.0 / 1024.0, NandCFree / 1024.0 / 1024.0));
            Debug.WriteLine(string.Format("Used by games: {0:F1}MB", WritedGamesSize / 1024.0 / 1024.0));
            Debug.WriteLine(string.Format("Used by save-states: {0:F1}MB", SaveStatesSize / 1024.0 / 1024.0));
            Debug.WriteLine(string.Format("Used by other files (mods, configs, etc.): {0:F1}MB", (NandCUsed - WritedGamesSize - SaveStatesSize) / 1024.0 / 1024.0));
            Debug.WriteLine(string.Format("Available for games: {0:F1}MB", (NandCFree + WritedGamesSize) / 1024.0 / 1024.0));
        }

        public static void ShowSplashScreen()
        {
            var clovershell = MainForm.Clovershell;
            var splashScreenPath = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "data"), "splash.gz");
            clovershell.ExecuteSimple("pkill -KILL clover-mcp");
            clovershell.ExecuteSimple("pkill -KILL ReedPlayer-Clover");
            clovershell.ExecuteSimple("pkill -KILL kachikachi");
            clovershell.ExecuteSimple("pkill -KILL canoe-shvc");
            if (File.Exists(splashScreenPath))
            {
                using (var splash = new FileStream(splashScreenPath, FileMode.Open))
                {
                    clovershell.Execute("gunzip -c - > /dev/fb0", splash, null, null, 3000);
                }
            }
        }

        public void UploadGames()
        {
            string gamesPath = NesMiniApplication.GamesCloverPath;
            const string rootFsPath = "/var/lib/hakchi/rootfs";
            const string installPath = "/var/lib/hakchi";
            const string squashFsPath = "/var/lib/hakchi/squashfs";
            int progress = 0;
            int maxProgress = 400;
            if (Games == null || Games.Count == 0)
                throw new Exception("there are no games");
            SetStatus(Resources.BuildingFolders);
            if (FoldersMode == NesMenuCollection.SplitStyle.Custom)
            {
                if (FoldersManagerFromThread(Games) != System.Windows.Forms.DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
                Games.AddBack();
            }
            else Games.Split(FoldersMode, MaxGamesPerFolder);
            progress += 5;
            SetProgress(progress, maxProgress);

            var clovershell = MainForm.Clovershell;
            try
            {
                if (WaitForClovershellFromThread() != DialogResult.OK)
                {
                    DialogResult = DialogResult.Abort;
                    return;
                }
                progress += 5;
                SetProgress(progress, maxProgress);

                ShowSplashScreen();
                UpdateRootfs();
                var squashFsMount = clovershell.ExecuteSimple($"mount | grep {squashFsPath}", 3000, false);
                if (string.IsNullOrEmpty(squashFsMount))
                    clovershell.ExecuteSimple($"mkdir -p {squashFsPath} && mount /dev/mapper/root-crypt {squashFsPath}", 3000, true);

                SetStatus(Resources.BuildingFolders);
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);
                Directory.CreateDirectory(tempDirectory);
                // Games!
                tempGamesDirectory = Path.Combine(tempDirectory, "games");
                Directory.CreateDirectory(tempDirectory);
                Directory.CreateDirectory(tempGamesDirectory);
                Dictionary<string, string> originalGames = new Dictionary<string, string>();
                var stats = new GamesTreeStats();
                AddMenu(Games, originalGames, stats);
                progress += 5;
                SetProgress(progress, maxProgress);

                GetMemoryStats();
                var maxGamesSize = (NandCFree + WritedGamesSize) - ReservedMemory * 1024 * 1024;
                if (stats.TotalSize > maxGamesSize)
                {
                    throw new Exception(string.Format(Resources.MemoryFull, stats.TotalSize / 1024 / 1024) + "\r\n\r\n" +
                        string.Format(Resources.MemoryStats.Replace("|", "\r\n"),
                        NandCTotal / 1024.0 / 1024.0,
                        (NandCFree + WritedGamesSize - ReservedMemory * 1024 * 1024) / 1024 / 1024,
                        SaveStatesSize / 1024.0 / 1024.0,
                        (NandCUsed - WritedGamesSize - SaveStatesSize) / 1024.0 / 1024.0));
                }

                int startProgress = progress;
                using (var gamesTar = new TarStream(tempGamesDirectory))
                {
                    maxProgress = (int)(gamesTar.Length / 1024 / 1024 + 20 + originalGames.Count() * 2);
                    SetProgress(progress, maxProgress);

                    clovershell.ExecuteSimple(string.Format("umount {0}", gamesPath));
                    clovershell.ExecuteSimple($"mkdir -p \"{rootFsPath}{gamesPath}\"", 3000, true);
                    if (ConfigIni.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom)
                    {
                        clovershell.ExecuteSimple($"[ -f \"{squashFsPath}{gamesPath}/title.fnt\" ] && [ ! -f \"{rootFsPath}{gamesPath}/title.fnt\" ] && cp -f \"{squashFsPath}{gamesPath}/title.fnt\" \"{rootFsPath}{gamesPath}\"/", 3000, false);
                        clovershell.ExecuteSimple($"[ -f \"{squashFsPath}{gamesPath}/copyright.fnt\" ] && [ ! -f \"{rootFsPath}{gamesPath}/copyright.fnt\" ] && cp -f \"{squashFsPath}{gamesPath}/copyright.fnt\" \"{rootFsPath}{gamesPath}\"/", 3000, false);
                    }
                    clovershell.ExecuteSimple(string.Format("rm -rf {0}{1}/CLV-* {0}{1}/??? {2}/menu", rootFsPath, gamesPath, installPath), 5000, true);

                    if (gamesTar.Length > 0)
                    {
                        gamesTar.OnReadProgress += delegate (long pos, long len)
                        {
                            progress = (int)(startProgress + pos / 1024 / 1024);
                            SetProgress(progress, maxProgress);
                        };

                        SetStatus(Resources.UploadingGames);
                        clovershell.Execute(string.Format("tar -xvC {0}{1}", rootFsPath, gamesPath), gamesTar, null, null, 30000, true);
                    }
                }

                SetStatus(Resources.UploadingOriginalGames);
                // Need to make sure that squashfs if mounted
                startProgress = progress;
                foreach (var originalCode in originalGames.Keys)
                {
                    string originalSyncCode = "";
                    switch (ConfigIni.ConsoleType)
                    {
                        case MainForm.ConsoleType.NES:
                        case MainForm.ConsoleType.Famicom:
                            originalSyncCode =
                                $"src=\"{squashFsPath}{gamesPath}/{originalCode}\" && " +
                                $"dst=\"{rootFsPath}{gamesPath}/{originalGames[originalCode]}/{originalCode}/\" && " +
                                $"mkdir -p \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}.png\" \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}_small.png\" \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}.nes\" \"$dst\" && " +
                                $"ln -s \"$src/autoplay/\" \"$dst/autoplay\" && " +
                                $"ln -s \"$src/pixelart/\" \"$dst/pixelart\" && " +
                                $"cp \"$src/{originalCode}.desktop\" \"$dst/{originalCode}.desktop\" && " +
                                $"sed -i -e 's/\\/usr\\/bin\\/clover-kachikachi/\\/bin\\/clover-kachikachi-wr/g' \"$dst/{originalCode}.desktop\"";
                            break;
                        case MainForm.ConsoleType.SNES:
                        case MainForm.ConsoleType.SuperFamicom:
                            originalSyncCode =
                                $"src=\"{squashFsPath}{gamesPath}/{originalCode}\" && " +
                                $"dst=\"{rootFsPath}{gamesPath}/{originalGames[originalCode]}/{originalCode}/\" && " +
                                $"mkdir -p \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}.png\" \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}_small.png\" \"$dst\" && " +
                                $"ln -s \"$src/{originalCode}.sfrom\" \"$dst\" && " +
                                $"ln -s \"$src/autoplay/\" \"$dst/autoplay\" && " +
                                $"cp \"$src/{originalCode}.desktop\" \"$dst/{originalCode}.desktop\" && " +
                                $"sed -i -e 's/\\/usr\\/bin\\/clover-canoe-shvc/\\/bin\\/clover-canoe-shvc-wr/g' \"$dst/{originalCode}.desktop\"";
                            break;
                    }
                    clovershell.ExecuteSimple(originalSyncCode, 30000, true);
                    progress += 2;
                    SetProgress(progress, maxProgress);
                };

                SetStatus(Resources.UploadingConfig);
                SyncConfig(Config);
#if !DEBUG
                if (Directory.Exists(tempDirectory))
                    Directory.Delete(tempDirectory, true);
#endif
                SetStatus(Resources.Done);
                SetProgress(maxProgress, maxProgress);
            }
            finally
            {
                try
                {
                    if (clovershell.IsOnline)
                        clovershell.ExecuteSimple("reboot", 100);
                }
                catch { }
            }
        }

        public void UpdateRootfs()
        {
            var modPath = Path.Combine(modsDirectory, Mod);
            var garbage = Directory.GetFiles(modPath, "p0000_config", SearchOption.AllDirectories);
            foreach (var file in garbage)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
            var rootFsPathes = Directory.GetDirectories(modsDirectory, "rootfs", SearchOption.AllDirectories);
            if (rootFsPathes.Length == 0) return;
            var rootFsPath = rootFsPathes[0];

            using (var updateTar = new TarStream(rootFsPath))
            {
                if (updateTar.Length > 0)
                {
                    var clovershell = MainForm.Clovershell;
                    clovershell.Execute("tar -xvC /", updateTar, null, null, 30000, true);
                    clovershell.ExecuteSimple("chmod +x /bin/*", 3000, true);
                    clovershell.ExecuteSimple("chmod +x /etc/init.d/*", 3000, true);
                }
            }
        }

        public static void SyncConfig(Dictionary<string, string> Config, bool reboot = false)
        {
            var clovershell = MainForm.Clovershell;
            const string configPath = "/etc/preinit.d/p0000_config";

            // Writing config
            var config = new MemoryStream();
            if (Config != null && Config.Count > 0)
            {
                foreach (var key in Config.Keys)
                {
                    var data = Encoding.UTF8.GetBytes(string.Format("cfg_{0}='{1}'\n", key, Config[key].Replace(@"'", @"\'")));
                    config.Write(data, 0, data.Length);
                }
            }
            clovershell.Execute($"cat >> {configPath}", config, null, null, 3000, true);
            config.Dispose();
            if (reboot)
            {
                try
                {
                    clovershell.ExecuteSimple("reboot", 100);
                }
                catch { }
            }
        }

        public static Image TakeScreenshot()
        {
            var clovershell = MainForm.Clovershell;
            var emulatorPID = findEmulatorProcess(clovershell);
            if (emulatorPID < 0)
            {
                Debug.WriteLine("PID of emulator can't be found. Capturing without SIGSTOP.");
            }

            var screenshot = new Bitmap(1280, 720, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var rawStream = new MemoryStream();
            pauseEmulatorProcess(clovershell, emulatorPID);
            clovershell.Execute("cat /dev/fb0", null, rawStream, null, 1000, true);
            resumeEmulatorProcess(clovershell, emulatorPID);
            var raw = rawStream.ToArray();
            BitmapData data = screenshot.LockBits(
                new Rectangle(0, 0, screenshot.Width, screenshot.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int rawOffset = 0;
            unsafe
            {
                for (int y = 0; y < screenshot.Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < screenshot.Width; ++x)
                    {
                        row[columnOffset] = raw[rawOffset];
                        row[columnOffset + 1] = raw[rawOffset + 1];
                        row[columnOffset + 2] = raw[rawOffset + 2];

                        columnOffset += 3;
                        rawOffset += 4;
                    }
                }
            }
            screenshot.UnlockBits(data);
            return screenshot;
        }

        // -- Emulator stop-and-go routines --
        // Stop emulator process to suppress tearing of the frame buffer.
        public static Int64 findEmulatorProcess(com.clusterrr.clovershell.ClovershellConnection sh)
        {
            var stdoutResult = new MemoryStream();
            sh.Execute("ps|grep canoe|grep -v grep", null, stdoutResult, null, 1000, false);
            var resultStr = Encoding.UTF8.GetString(stdoutResult.ToArray());
            resultStr = System.Text.RegularExpressions.Regex.Replace(resultStr, "^\\s+", ""); // Trim whitespaces

            var fields = System.Text.RegularExpressions.Regex.Split(resultStr, " +");
            if (fields.Length < 1)
            {
                return -1; // emulator not found
            }

            try
            {
                Int64 pid = Convert.ToInt64(fields[0], 10);
                return pid;
            }
            catch
            {
                Debug.WriteLine("Bad PID : " + resultStr);
                return -1;
            }
        }

        public static void pauseEmulatorProcess(com.clusterrr.clovershell.ClovershellConnection sh, Int64 pid)
        {
            if (pid < 0) { return; } // emulator not found
            sh.Execute($"kill -s SIGSTOP {pid}", null, null, null, 1000, false);
        }

        public static void resumeEmulatorProcess(com.clusterrr.clovershell.ClovershellConnection sh, Int64 pid)
        {
            if (pid < 0) { return; } // emulator not found
            sh.Execute($"kill -s SIGCONT {pid}", null, null, null, 1000, false);
        }


        public void Memboot(int maxProgress = -1, int progress = 0)
        {
            SetProgress(progress, maxProgress < 0 ? 1000 : maxProgress);

            // Connecting to NES Mini
            if (WaitForFelFromThread() != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            progress += 5;
            SetProgress(progress, maxProgress > 0 ? maxProgress : 1000);

            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
            Directory.CreateDirectory(tempDirectory);

            byte[] kernel;
            if (!string.IsNullOrEmpty(Mod))
                kernel = CreatePatchedKernel();
            else
                kernel = File.ReadAllBytes(KernelDumpPath);
            var size = CalcKernelSize(kernel);
            if (size > kernel.Length || size > Fel.transfer_max_size)
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
                maxProgress = (int)((double)kernel.Length / (double)67000 + 50);
            SetProgress(progress, maxProgress);

            SetStatus(Resources.UploadingKernel);
            fel.WriteMemory(Fel.transfer_base_m, kernel,
                delegate (Fel.CurrentAction action, string command)
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

            var bootCommand = string.Format("boota {0:x}", Fel.transfer_base_m);
            SetStatus(Resources.ExecutingCommand + " " + bootCommand);
            fel.RunUbootCmd(bootCommand, true);

            // Wait some time while booting
            int waitSeconds;
            if ((hmodsInstall != null && hmodsInstall.Count() > 0)
                || (hmodsUninstall != null && hmodsUninstall.Count() > 0))
                waitSeconds = 60;
            else
                waitSeconds = 5;
            for (int i = 0; i < waitSeconds * 2; i++)
            {
                Thread.Sleep(500);
                progress++;
                SetProgress(progress, maxProgress);
                if (MainForm.Clovershell.IsOnline)
                    break;
            }
#if !DEBUG
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
#endif
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        private void UnpackRamfs(string kernelPath = null)
        {
            Directory.CreateDirectory(tempDirectory);
            Directory.CreateDirectory(kernelDirectory);
            Directory.CreateDirectory(ramfsDirectory);
            string tempKernelDump = Path.Combine(tempDirectory, "kernel.img");
            if ((kernelPath ?? KernelDumpPath) != tempKernelDump)
                File.Copy(kernelPath ?? KernelDumpPath, tempKernelDump, true);
            if (!ExecuteTool("unpackbootimg.exe", string.Format("-i \"{0}\" -o \"{1}\"", tempKernelDump, kernelDirectory)))
                throw new Exception("Can't unpack kernel image");
            if (!ExecuteTool("lzop.exe", string.Format("-d \"{0}\" -o \"{1}\"",
                Path.Combine(kernelDirectory, "kernel.img-ramdisk.gz"), initramfs_cpio)))
                throw new Exception("Can't unpack ramdisk");
            ExecuteTool("cpio.exe", string.Format("-imd --no-preserve-owner --quiet -I \"{0}\"",
               @"..\initramfs.cpio"), ramfsDirectory);
            if (!File.Exists(Path.Combine(ramfsDirectory, "init"))) // cpio.exe fails on Windows XP for some reason. But working!
                throw new Exception("Can't unpack ramdisk 2");
        }

        private byte[] CreatePatchedKernel(string kernelPath = null)
        {
            SetStatus(Resources.BuildingCustom);
            if (!File.Exists(Path.Combine(ramfsDirectory, "init")))
                UnpackRamfs(kernelPath);
            if (Directory.Exists(hakchiDirectory))
                Directory.Delete(hakchiDirectory, true);
            NesMiniApplication.DirectoryCopy(Path.Combine(modsDirectory, Mod), ramfsDirectory, true);
            var ramfsFiles = Directory.GetFiles(ramfsDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var file in ramfsFiles)
            {
                var fInfo = new FileInfo(file);
                if (fInfo.Length > 10 && fInfo.Length < 100 && ((fInfo.Attributes & FileAttributes.System) == 0) &&
                    (Encoding.ASCII.GetString(File.ReadAllBytes(file), 0, 10)) == "!<symlink>")
                    fInfo.Attributes |= FileAttributes.System;
            }

            if (hmodsInstall != null && hmodsInstall.Count() > 0)
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
            if (hmodsUninstall != null && hmodsUninstall.Count() > 0)
            {
                Directory.CreateDirectory(tempHmodsDirectory);
                var mods = new StringBuilder();
                foreach (var hmod in hmodsUninstall)
                    mods.AppendFormat("{0}.hmod\n", hmod);
                File.WriteAllText(Path.Combine(tempHmodsDirectory, "uninstall"), mods.ToString());
            }

            // Building image
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
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
#endif
            return result;
        }

        private class GamesTreeStats
        {
            public List<NesMenuCollection> allMenus = new List<NesMenuCollection>();
            public int TotalGames = 0;
            public long TotalSize = 0;
            public long TransferSize = 0;
        }

        private void AddMenu(NesMenuCollection menuCollection, Dictionary<string, string> originalGames, GamesTreeStats stats = null)
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
                    Debug.WriteLine(string.Format("Processing {0} ('{1}'), size: {2}KB", game.Code, game.Name, gameSize / 1024));
                    var gameCopy = game.CopyTo(targetDirectory);
                    stats.TotalSize += gameSize;
                    stats.TransferSize += gameSize;
                    stats.TotalGames++;
                    try
                    {
                        if (gameCopy is ISupportsGameGenie && File.Exists(gameCopy.GameGeniePath))
                        {
                            bool compressed = false;
                            if (gameCopy.DecompressPossible().Count() > 0)
                            {
                                gameCopy.Decompress();
                                compressed = true;
                            }
                            (gameCopy as ISupportsGameGenie).ApplyGameGenie();
                            if (compressed)
                                gameCopy.Compress();
                            File.Delete((gameCopy as NesMiniApplication).GameGeniePath);
                        }
                    }
                    catch (GameGenieFormatException ex)
                    {
                        ShowError(new Exception(string.Format(Resources.GameGenieFormatError, ex.Code, game.Name)), dontStop: true);
                    }
                    catch (GameGenieNotFoundException ex)
                    {
                        ShowError(new Exception(string.Format(Resources.GameGenieNotFound, ex.Code, game.Name)), dontStop: true);
                    }
                }
                if (element is NesMenuFolder)
                {
                    var folder = element as NesMenuFolder;
                    if (!stats.allMenus.Contains(folder.ChildMenuCollection))
                    {
                        stats.allMenus.Add(folder.ChildMenuCollection);
                        AddMenu(folder.ChildMenuCollection, originalGames, stats);
                    }
                    folder.ChildIndex = stats.allMenus.IndexOf(folder.ChildMenuCollection);
                    var folderDir = Path.Combine(targetDirectory, folder.Code);
                    var folderSize = folder.Save(folderDir);
                    stats.TotalSize += folderSize;
                    stats.TransferSize += folderSize;

                }
                if (element is NesDefaultGame)
                {
                    var game = element as NesDefaultGame;
                    stats.TotalSize += game.Size;
                    originalGames[game.Code] = menuIndex == 0 ? "." : string.Format("{0:D3}", menuIndex);
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
            var appDirectory = baseDirectoryInternal;
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

        static UInt32 CalcKernelSize(byte[] header)
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
        public ICollection<NesMiniApplication> AddGames(IEnumerable<string> files, Form parentForm = null)
        {
            var apps = new List<NesMiniApplication>();
            addedApplications = null;
            NesMiniApplication.ParentForm = this;
            NesMiniApplication.NeedPatch = null;
            NesMiniApplication.Need3rdPartyEmulator = null;
            NesGame.IgnoreMapper = null;
            SnesGame.NeedAutoDownloadCover = null;
            int count = 0;
            SetStatus(Resources.AddingGames);
            foreach (var sourceFileName in files)
            {
                NesMiniApplication app = null;
                try
                {
                    var fileName = sourceFileName;
                    var ext = Path.GetExtension(sourceFileName).ToLower();
                    bool? needPatch = YesForAllPatches ? (bool?)true : null;
                    byte[] rawData = null;
                    string tmp = null;
                    if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                    {
                        SevenZipExtractor.SetLibraryPath(Path.Combine(baseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        using (var szExtractor = new SevenZipExtractor(sourceFileName))
                        {
                            var filesInArchive = new List<string>();
                            var gameFilesInArchive = new List<string>();
                            foreach (var f in szExtractor.ArchiveFileNames)
                            {
                                var e = Path.GetExtension(f).ToLower();
                                if (e == ".desktop" || AppTypeCollection.GetAppByExtension(e) != null)
                                    gameFilesInArchive.Add(f);
                                filesInArchive.Add(f);
                            }
                            if (gameFilesInArchive.Count == 1) // Only one known file (or app)
                            {
                                fileName = gameFilesInArchive[0];
                            }
                            else if (gameFilesInArchive.Count > 1) // Many known files, need to select
                            {
                                var r = SelectFileFromThread(gameFilesInArchive.ToArray());
                                if (r == DialogResult.OK)
                                    fileName = selectedFile;
                                else if (r == DialogResult.Ignore)
                                    fileName = sourceFileName;
                                else continue;
                            }
                            else if (filesInArchive.Count == 1) // No known files but only one another file
                            {
                                fileName = filesInArchive[0];
                            }
                            else // Need to select
                            {
                                var r = SelectFileFromThread(filesInArchive.ToArray());
                                if (r == DialogResult.OK)
                                    fileName = selectedFile;
                                else if (r == DialogResult.Ignore)
                                    fileName = sourceFileName;
                                else continue;
                            }
                            if (fileName != sourceFileName)
                            {
                                var o = new MemoryStream();
                                if (Path.GetExtension(fileName).ToLower() == ".desktop" // App in archive, need the whole directory
                                    || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".jpg") // Or it has cover in archive
                                    || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".png")
                                    || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".ips") // Or IPS file
                                    )
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
                    }
                    app = NesMiniApplication.Import(fileName, sourceFileName, rawData);

                    var lGameGeniePath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".xml");
                    if (File.Exists(lGameGeniePath))
                    {
                        GameGenieDataBase lGameGenieDataBase = new GameGenieDataBase(app);
                        lGameGenieDataBase.ImportCodes(lGameGeniePath, true);
                        lGameGenieDataBase.Save();
                    }

                    if (!string.IsNullOrEmpty(tmp) && Directory.Exists(tmp)) Directory.Delete(tmp, true);
                    if (app != null)
                        ConfigIni.SelectedGames += ";" + app.Code;
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException) return null;
                    if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        Debug.WriteLine(ex.InnerException.Message + ex.InnerException.StackTrace);
                        ShowError(ex.InnerException, true, Path.GetFileName(sourceFileName));
                    }
                    else
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        ShowError(ex, true, Path.GetFileName(sourceFileName));
                    }
                }
                if (app != null)
                    apps.Add(app);
                SetProgress(++count, files.Count());
            }
            addedApplications = apps.ToArray();
            return apps; // Added games/apps
        }

        void DownloadCovers()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesMiniApplication game in Games)
            {
                SetStatus(Resources.GooglingFor.Trim() + " " + game.Name + "...");
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

        void CompressGames()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesMiniApplication game in Games)
            {
                SetStatus(string.Format(Resources.Compressing, game.Name));
                game.Compress();
                SetProgress(++i, Games.Count);
            }
        }

        void DecompressGames()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesMiniApplication game in Games)
            {
                SetStatus(string.Format(Resources.Decompressing, game.Name));
                game.Decompress();
                SetProgress(++i, Games.Count);
            }
        }

        void DeleteGames()
        {
            if (Games == null) return;
            int i = 0;
            foreach (NesMiniApplication game in Games)
            {
                SetStatus(string.Format(Resources.Removing, game.Name));
                Directory.Delete(game.GamePath, true);
                SetProgress(++i, Games.Count);
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
                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.NoProgress);
                TaskbarProgress.SetValue(this, 0, 1);
            }
        }
    }
}
