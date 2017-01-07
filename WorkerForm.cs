using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
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
            correctKernels = new string[] { "5cfdca351484e7025648abc3b20032ff", "07bfb800beba6ef619c29990d14b5158" };
            gamesDirectory = Path.Combine(ramfsDirectory, "games");
        }

        public DialogResult Start()
        {
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
            try
            {
                fel.Open(vid, pid);
                switch (Task)
                {
                    case Tasks.DumpKernel:
                        SetText(Resources.DumpingKernel);
                        DoKernelDump();
                        break;
                    case Tasks.FlashKernel:
                        if (!string.IsNullOrEmpty(Mod))
                            SetText(Resources.FlasingCustom);
                        else
                            SetText(Resources.FlasingOriginal);
                        FlashKernel();
                        break;
                    case Tasks.Memboot:
                        SetText(Resources.UploadingGames);
                        Memboot();
                        break;
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                ShowError(ex.Message);
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

        void SetText(string text)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(SetText), new object[] { text });
                    return;
                }
                Text = text;
            }
            catch { }
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

        void ShowError(string text)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(ShowError), new object[] { text });
                    return;
                }
                MessageBox.Show(this, text + "\r\n" + Resources.PleaseTryAgain, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                thread = null;
                Close();
            }
            catch { }
        }

        void ExecCommand(string command, bool nopause = false)
        {
            SetStatus(Resources.ExecutingCommand + " " + command);
            fel.WriteMemory((uint)(uboot_base_m + cmdOffset), Encoding.ASCII.GetBytes(command + "\0"));
            fel.Exec((uint)uboot_base_m, nopause ? 0 : 10);
        }

        byte[] WaitRead(UInt32 address, UInt32 size)
        {
            SetStatus(Resources.WaitingForDevice);
            int errorCount = 0;
            while (true)
            {
                try
                {
                    if ((errorCount > 0) && (errorCount % 5) == 0)
                    {
                        fel.Close();
                        Thread.Sleep(1000);
                        fel.Open(vid, pid);
                    }
                    fel.ClearInputBuffer();
                    fel.VerifyDevice();
                    var data = fel.ReadMemory(address, size);
                    return data;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (errorCount >= 15) throw ex;
                    Thread.Sleep(1000);
                }
            }
        }

        public void DoKernelDump()
        {
            const int maxProgress = 100;
            SetProgress(1, maxProgress);
            SetStatus(Resources.UploadingFel1);
            fel.WriteMemory((uint)fes1_base_m, Resources.fes1);
            SetProgress(10, maxProgress);
            //Console.WriteLine("OK");
            //var r = fel.ReadMemory(0x2000, (UInt32)fel1.Length);
            SetStatus(Resources.ExecutingFel1);
            fel.Exec((uint)fes1_base_m, 3);
            SetProgress(20, maxProgress);
            SetStatus(Resources.UploadingUboot);
            fel.WriteMemory(uboot_base_m, Resources.uboot);
            SetProgress(30, maxProgress);
            /*
            var addr = 0x100000;
            UInt32 size = sector_size * 6;
            var command = string.Format("sunxi_flash phy_read {0:x} {1:x} {2:x};fastboot_test", flash_mem_base, addr / sector_size, (size + addr % sector_size + sector_size - 1) / sector_size);
            ExecCommand(command);
            SetProgress(50, maxDumpProgress);
            var header = WaitRead(flash_mem_base, 32);
            SetProgress(70, maxDumpProgress);
            size = (UInt32)(header[0x14] | (header[0x15] * 0x100) | (header[0x16] * 0x10000) | (header[0x17] * 0x1000000));
            if (size == 0 || size > kernel_max_size)
                throw new Exception("Invalid uboot size: " + size);
            SetStatus("Dumping uboot...");
            var uboot = fel.ReadMemory(flash_mem_base, size);
            Directory.CreateDirectory(Path.GetDirectoryName(UBootDump));
            File.WriteAllBytes(UBootDump, uboot);
            SetProgress(90, maxDumpProgress);
             */

            var addr = 0x600000;
            var size = sector_size * 0x20;
            var command = string.Format("sunxi_flash phy_read {0:x} {1:x} {2:x};fastboot_test", flash_mem_base, addr / sector_size, (size + addr % sector_size + sector_size - 1) / sector_size);
            ExecCommand(command);
            SetProgress(50, maxProgress);
            var header = WaitRead(flash_mem_base, 64);
            SetProgress(70, maxProgress);

            size = CalKernelSize(header);
            if (size == 0 || size > kernel_max_size)
                throw new Exception(Resources.InvalidKernelSize + " " + size);
            SetStatus(Resources.DumpingKernel);
            var kernel = fel.ReadMemory(flash_mem_base, size);

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
            SetProgress(maxProgress, maxProgress);
            SetStatus(Resources.Done);
            Thread.Sleep(1000);
            DialogResult = DialogResult.OK;
        }

        public void FlashKernel()
        {
            const int maxProgress = 180;
            SetProgress(0, maxProgress);

            byte[] kernel;
            if (!string.IsNullOrEmpty(Mod))
                kernel = CreatePatchedKernel(Mod);
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
            SetProgress(20, maxProgress);

            fel.Open(vid, pid);
            SetStatus(Resources.UploadingFel1);
            fel.WriteMemory((uint)fes1_base_m, Resources.fes1);
            SetProgress(30, maxProgress);
            //Console.WriteLine("OK");
            //var r = fel.ReadMemory(0x2000, (UInt32)fel1.Length);
            SetStatus(Resources.ExecutingFel1);
            fel.Exec((uint)fes1_base_m, 3);
            Thread.Sleep(3000);
            SetProgress(40, maxProgress);
            SetStatus(Resources.UploadingUboot);
            fel.WriteMemory(uboot_base_m, Resources.uboot);
            SetProgress(50, maxProgress);
            SetStatus(Resources.UploadingKernel);
            fel.WriteMemory(flash_mem_base, kernel);
            SetProgress(80, maxProgress);
            var addr = 0x600000;
            var command = string.Format("sunxi_flash phy_write {0:x} {1:x} {2:x};fastboot_test", flash_mem_base, addr / sector_size, size / sector_size);
            ExecCommand(command);
            SetProgress(100, maxProgress);
            WaitRead(flash_mem_base, 64);
            SetProgress(120, maxProgress);

            command = string.Format("sunxi_flash phy_read {0:x} {1:x} {2:x};fastboot_test", flash_mem_base, addr / sector_size, size / sector_size);
            ExecCommand(command);
            SetProgress(140, maxProgress);
            var header = WaitRead(flash_mem_base, 64);
            SetProgress(160, maxProgress);
            SetStatus(Resources.Verifying);
            var r = fel.ReadMemory((uint)flash_mem_base, size);
            for (int i = 0; i < size; i++)
                if (kernel[i] != r[i])
                {
                    throw new Exception(Resources.VerifyFailed);
                }
            if (string.IsNullOrEmpty(Mod))
                ExecCommand(string.Format("boota {0:x}", kernel_base_m), true);
            SetStatus(Resources.Done);
            SetProgress(180, maxProgress);
            Thread.Sleep(1000);
            DialogResult = DialogResult.OK;

        }

        public void Memboot()
        {
            const int maxProgress = 75;
            SetProgress(0, maxProgress);

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
            SetProgress(20, maxProgress);

            fel.Open(vid, pid);
            SetStatus(Resources.UploadingFel1);
            fel.WriteMemory((uint)fes1_base_m, Resources.fes1);
            SetProgress(30, maxProgress);
            //Console.WriteLine("OK");
            //var r = fel.ReadMemory(0x2000, (UInt32)fel1.Length);
            SetStatus(Resources.ExecutingFel1);
            fel.Exec((uint)fes1_base_m, 3);
            Thread.Sleep(3000);
            SetProgress(40, maxProgress);
            SetStatus(Resources.UploadingUboot);
            fel.WriteMemory(uboot_base_m, Resources.uboot);
            SetProgress(50, maxProgress);
            SetStatus(Resources.UploadingKernel);
            fel.WriteMemory(flash_mem_base, kernel);
            SetProgress(70, maxProgress);
            ExecCommand(string.Format("boota {0:x}", kernel_base_m), true);
            SetStatus(Resources.Done);
            SetProgress(75, maxProgress);
            Thread.Sleep(1000);
            DialogResult = DialogResult.OK;
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
            if (!ExecuteTool("cpio.exe", string.Format("-imd --no-preserve-owner --quiet -I \"{0}\"",
                @"..\initramfs.cpio"), ramfsDirectory))
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
