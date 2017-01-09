using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using MadWizard.WinUSBNet;
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
    public partial class WorkerForm : Form
    {
        public enum Tasks { DumpKernel, FlashKernel, Memboot };
        public Tasks Task;
        //public string UBootDump;
        public string KernelDump;
        public string Mod = null;
        public bool OriginalGames;
        public NesGame[] Games;
        Thread thread = null;
        Fel fel = null;

        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;

        const UInt32 cmdOffset = 0x604FF;
        const UInt32 cmdLen = 50;
        const UInt32 fes1_base_m = 0x2000;
        const UInt32 flash_mem_base = 0x43800000;
        const UInt32 flash_mem_size = 0x20;
        const UInt32 uboot_base_m = 0x47000000u;
        const UInt32 sector_size = 0x20000;
        const UInt32 uboot_base_f = 0x100000;
        const UInt32 kernel_base_f = (sector_size * 0x30);
        const UInt32 kernel_base_m = flash_mem_base;
        const UInt32 kernel_max_size = (uboot_base_m - flash_mem_base);
        const UInt32 kernel_max_flash_size = (sector_size * 0x20);

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
        readonly string gamesDirectory;

        string[] correctKernels;
        bool? waitDeviceResult = null;

        public WorkerForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
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
            correctKernels = new string[] { 
                "5cfdca351484e7025648abc3b20032ff", "07bfb800beba6ef619c29990d14b5158", // NES Mini
                "ac8144c3ea4ab32e017648ee80bdc230" // Famicom Mini
            };
            gamesDirectory = Path.Combine(ramfsDirectory, "games");
        }

        public DialogResult Start()
        {
            switch (Task)
            {
                case Tasks.DumpKernel:
                    Text = Resources.DumpingKernel;
                    break;
                case Tasks.FlashKernel:
                    if (!string.IsNullOrEmpty(Mod))
                        Text =  Resources.FlasingCustom;
                    else
                        Text =  Resources.FlasingOriginal;
                    break;
                case Tasks.Memboot:
                    Text = Resources.UploadingGames;
                    break;
            }
            SetProgress(0, 1);
            if (!WaitingForm.WaitForDevice(vid, pid))
            {
                DialogResult = DialogResult.Abort;
                return DialogResult;
            }
            thread = new Thread(StartThread);
            thread.Start();
            return ShowDialog();
        }

        public void StartThread()
        {
            fel = new Fel();
            fel.Fes1Bin = Resources.fes1;
            fel.UBootBin = Resources.uboot;
            SetProgress(0, 100);
            try
            {
                fel.Open(vid, pid);
                SetStatus(Resources.UploadingFes1);
                fel.InitDram(true);
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

        void ShowError(Exception ex)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Exception>(ShowError), new object[] { ex });
                    return;
                }
                if (ex is MadWizard.WinUSBNet.USBException)
                    MessageBox.Show(this, ex.Message + "\r\n" + Resources.PleaseTryAgain + "\r\n" + Resources.PleaseTryAgainUSB, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, ex.Message + "\r\n" + Resources.PleaseTryAgain, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                thread = null;
                Close();
            }
            catch { }
        }

        public void DoKernelDump()
        {
            int progress = 5;
            const int maxProgress = 80;
            SetProgress(progress, maxProgress);
            SetStatus(Resources.DumpingKernel);

            var kernel = fel.ReadFlash(kernel_base_f, sector_size * 0x20,
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
            if (size == 0 || size > kernel_max_size)
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
                    DialogResult = System.Windows.Forms.DialogResult.Abort;
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
            if (!string.IsNullOrEmpty(Mod))
            {
                kernel = CreatePatchedKernel(Mod);
                progress += 5;
                SetProgress(progress, maxProgress);
            }
            else
                kernel = File.ReadAllBytes(KernelDump);
            var size = CalKernelSize(kernel);
            if (size > kernel.Length || size > kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);

            size = (size + sector_size - 1) / sector_size;
            size = size * sector_size;
            if (kernel.Length != size)
            {
                var newK = new byte[size];
                Array.Copy(kernel, newK, kernel.Length);
                kernel = newK;
            }

            bool flashCommandExecuted = false;
            try
            {
                fel.WriteFlash(kernel_base_f, kernel,
                    delegate(Fel.CurrentAction action, string command)
                    {
                        switch (action)
                        {
                            case Fel.CurrentAction.RunningCommand:
                                SetStatus(Resources.ExecutingCommand + " " + command);
                                flashCommandExecuted = true;
                                break;
                            case Fel.CurrentAction.WritingMemory:
                                SetStatus(Resources.UploadingKernel);
                                break;
                        }
                        progress++;
                        SetProgress(progress, maxProgress);
                    }
                );
            }
            catch (USBException ex)
            {
                fel.Close();
                if (flashCommandExecuted)
                {
                    SetStatus(Resources.WaitingForDevice);
                    waitDeviceResult = null;
                    WaitForDeviceInvoke(vid, pid);
                    while (waitDeviceResult == null)
                        Thread.Sleep(100);
                    if (!(waitDeviceResult ?? false))
                    {
                        DialogResult = DialogResult.Abort;
                        return;
                    }
                    Thread.Sleep(500);
                    fel = new Fel();
                    fel.Fes1Bin = Resources.fes1;
                    fel.UBootBin = Resources.uboot;
                    fel.Open(vid, pid);
                    SetStatus(Resources.UploadingFes1);
                    fel.InitDram(true);
                }
                else throw ex;
            }
            var r = fel.ReadFlash((UInt32)kernel_base_f, (UInt32)kernel.Length,
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
            for (int i = 0; i < kernel.Length; i++)
                if (kernel[i] != r[i])
                {
                    throw new Exception(Resources.VerifyFailed);
                }
            var bootCommand = string.Format("boota {0:x}", kernel_base_m);
            SetStatus(Resources.ExecutingCommand + " " + bootCommand);
            fel.RunUbootCmd(bootCommand, true);
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        public void Memboot()
        {
            int progress = 5;
            int maxProgress = 300;
            SetProgress(progress, maxProgress);

            byte[] kernel;
            if (!string.IsNullOrEmpty(Mod))
                kernel = CreatePatchedKernel(Mod, true, OriginalGames, Games);
            else
                kernel = File.ReadAllBytes(KernelDump);
            var size = CalKernelSize(kernel);
            if (size > kernel.Length || size > kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);
            size = (size + sector_size - 1) / sector_size;
            size = size * sector_size;
            if (kernel.Length != size)
            {
                var newK = new byte[size];
                Array.Copy(kernel, newK, kernel.Length);
                kernel = newK;
            }

            progress += 5;
            maxProgress = kernel.Length / 67000 + 15;
            SetProgress(progress, maxProgress);

            SetStatus(Resources.UploadingKernel);
            fel.WriteMemory(flash_mem_base, kernel,
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

            var bootCommand = string.Format("boota {0:x}", kernel_base_m);
            SetStatus(Resources.ExecutingCommand + " " + bootCommand);
            fel.RunUbootCmd(bootCommand, true);
            SetStatus(Resources.Done);
            SetProgress(maxProgress, maxProgress);
        }

        private byte[] CreatePatchedKernel(string mod, bool createConfig = false, bool originalGames = false, NesGame[] games = null)
        {
            SetStatus(Resources.BuildingCustom);
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
            Directory.CreateDirectory(tempDirectory);
            Directory.CreateDirectory(kernelDirectory);
            Directory.CreateDirectory(ramfsDirectory);
            //SetStatus("Unpacking kernel image...");
            if (!ExecuteTool("unpackbootimg.exe", string.Format("-i \"{0}\" -o \"{1}\"", KernelDump, kernelDirectory)))
                throw new Exception("Can't unpack kernel image");
            //SetStatus("Unpacking ramdisk...");
            if (!ExecuteTool("lzop.exe", string.Format("-d \"{0}\" -o \"{1}\"",
                Path.Combine(kernelDirectory, "kernel.img-ramdisk.gz"), initramfs_cpio)))
                throw new Exception("Can't unpack ramdisk");
            //if (!ExecuteTool("cpio.exe", string.Format("-imd --no-preserve-owner --quiet -I \"{0}\"",
            //    @"..\initramfs.cpio"), ramfsDirectory))
            ExecuteTool("cpio.exe", string.Format("-imd --no-preserve-owner --quiet -I \"{0}\"",
               @"..\initramfs.cpio"), ramfsDirectory);
            if (!File.Exists(Path.Combine(ramfsDirectory, "init"))) // cpio.exe fails on Windows XP for some reason. But working!
                throw new Exception("Can't unpack ramdisk 2");
            //SetStatus("Patching...");
            if (Directory.Exists(hakchiDirectory)) Directory.Delete(hakchiDirectory, true);
            if (!ExecuteTool("xcopy", string.Format("\"{0}\" /h /y /c /r /s /q",
                Path.Combine(modsDirectory, mod)), ramfsDirectory, true))
                throw new Exception("Can't copy mod directory");
            if (createConfig)
            {
                var config = string.Format("hakchi_enabled=y\nhakchi_remove_games=y\nhakchi_original_games={0}\n", originalGames ? "y" : "n");
                File.WriteAllText(configPath, config);
            }
            if (games != null)
            {
                Directory.CreateDirectory(gamesDirectory);
                foreach (var game in games)
                {
                    var gameDir = Path.Combine(gamesDirectory, game.Code);
                    Directory.CreateDirectory(gameDir);
                    if (!ExecuteTool("xcopy", string.Format("\"{0}\" /h /y /c /r /e /q", game.GamePath),
                        gameDir, true))
                        throw new Exception("Can't copy " + game);
                }
            }
            ExecuteTool("upx.exe", "--best sbin\\cryptsetup", ramfsDirectory);
            if (!ExecuteTool("mkbootfs.bat", string.Format("\"{0}\" \"{1}\"", ramfsDirectory, initramfs_cpioPatched), toolsDirectory))
                throw new Exception("Can't repack ramdisk");
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
            Directory.Delete(tempDirectory, true);
            if (result.Length > kernel_max_size) throw new Exception("Kernel is too big");
            return result;
        }

        private bool ExecuteTool(string tool, string args, string directory = null, bool external = false)
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
            //process.StartInfo.RedirectStandardOutput = true;            
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        private void WaitForDeviceInvoke(UInt16 vid, UInt16 pid)
        {
            waitDeviceResult = null;
            if (InvokeRequired)
            {
                Invoke(new Action<UInt16, UInt16>(WaitForDeviceInvoke), new object[] { vid, pid});
                return;
            }
            waitDeviceResult = WaitingForm.WaitForDevice(vid, pid);
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

        private void WorkerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((thread != null) && (e.CloseReason == CloseReason.UserClosing))
            {
                if (MessageBox.Show(this, Resources.DoYouWantCancel, Resources.AreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
                if (thread != null) thread.Abort();
            }
        }
    }
}
