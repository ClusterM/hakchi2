using com.clusterrr.FelLib;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;
using SevenZip;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class MembootTasks
    {
        // Enums
        public enum MembootTaskType {
            InstallHakchi,
            ResetHakchi,
            UninstallHakchi,
            DumpNand,
            DumpNandB,
            FlashNandB,
            DumpNandC,
            FlashNandC,
            FormatNandC,
            ProcessMods,
            Memboot,
            MembootOriginal,
            MembootRecovery,
            FlashNormalUboot,
            FlashSDUboot,
            FactoryReset
        }
        public enum HakchiTasks { Install, Reset, Uninstall }
        public enum NandTasks { DumpNand, DumpNandB, FlashNandB, DumpNandC, FlashNandC, FormatNandC }

        // Private variables
        private Fel fel;
        private MemoryStream stockKernel;

        // Public Static variables
        public static readonly Dictionary<hakchi.ConsoleType, string[]> correctKernels = Shared.CorrectKernels();
        public static readonly Dictionary<hakchi.ConsoleType, string[]> correctKeys = Shared.CorrectKeys();

        public readonly TaskFunc[] Tasks;

        public MembootTasks(MembootTaskType type, string[] hmodsInstall = null, string[] hmodsUninstall = null, string dumpPath = null)
        {
            fel = new Fel();
            List<TaskFunc> taskList = new List<TaskFunc>();
            taskList.Add(WaitForFelOrMembootableShell);
            taskList.Add(Memboot);
            taskList.Add(Tasker.Wait(10000, Resources.PleaseWait));
            taskList.Add(WaitForShell);
            taskList.Add(ShellTasks.ShowSplashScreen);

            switch (type)
            {
                case MembootTaskType.InstallHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        BootHakchi
                    });
                    break;

                case MembootTaskType.ResetHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Reset),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        BootHakchi
                    });
                    break;

                case MembootTaskType.UninstallHakchi:
                    taskList.AddRange(new TaskFunc[] {
                        GetStockKernel,
                        FlashStockKernel,
                        HandleHakchi(HakchiTasks.Uninstall),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.FactoryReset:
                    taskList.AddRange(new TaskFunc[] {
                        GetStockKernel,
                        FlashStockKernel,
                        ProcessNand(null, NandTasks.FormatNandC),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.DumpNand:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNand),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.DumpNandB:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNandB),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.DumpNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNandC),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.FlashNandB:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashNandB),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.FlashNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashNandC),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.FormatNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FormatNandC),
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        BootHakchi
                    });
                    break;

                case MembootTaskType.ProcessMods:
                    taskList.AddRange(new ModTasks(hmodsInstall, hmodsUninstall).Tasks);
                    break;

                case MembootTaskType.FlashNormalUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(Fel.UbootType.Normal),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.FlashSDUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(Fel.UbootType.SD),
                        ShellTasks.Reboot
                    });
                    break;

                case MembootTaskType.Memboot:
                    taskList.Add(BootHakchi);
                    break;

                case MembootTaskType.MembootRecovery: break;

                case MembootTaskType.MembootOriginal:
                    taskList.AddRange(new TaskFunc[]
                    {
                        GetStockKernel,
                        Memboot
                    });
                    break;
            }
            Tasks = taskList.ToArray();
        }

        public Conclusion WaitForFelOrMembootableShell(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.InvokeRequired)
            {
                return (Conclusion)tasker.HostForm.Invoke(new Func<Tasker, Object, Conclusion>(WaitForFelOrMembootableShell), new object[] { tasker, syncObject });
            }

            tasker.SetStatus(Resources.WaitingForDevice);
            if (hakchi.Shell.IsOnline && hakchi.Shell.Execute("[ -f /proc/atags ]") == 0)
                return Conclusion.Success;

            if (!WaitingFelForm.WaitForDevice(Shared.ClassicUSB.vid, Shared.ClassicUSB.pid, tasker.HostForm))
                return Conclusion.Abort;

            fel.Fes1Bin = Resources.fes1;
            if (ConfigIni.Instance.MembootUboot == Fel.UbootType.SD)
            {
                fel.UBootBin = Resources.ubootSD;
            }
            else
            {
                fel.UBootBin = Resources.uboot;
            }
            if (!fel.Open(Shared.ClassicUSB.vid, Shared.ClassicUSB.pid))
                throw new FelException("Can't open device");
            tasker.SetStatus(Resources.UploadingFes1);
            fel.InitDram(true);
            return Conclusion.Success;
        }

        public Conclusion WaitForShell(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.InvokeRequired)
            {
                return (Conclusion)tasker.HostForm.Invoke(new Func<Tasker, object, Conclusion>(WaitForShell), new object[] { tasker, syncObject });
            }
            tasker.SetStatus(Resources.WaitingForDevice);
            var result = WaitingClovershellForm.WaitForDevice(tasker.HostForm, true);
            if (result)
                return Conclusion.Success;

            return Conclusion.Abort;
        }

        public Conclusion Memboot(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Membooting);

            byte[] kernel;
            if (stockKernel != null && stockKernel.Length > 0)
            {
                kernel = stockKernel.ToArray();
            }
            else
            {
                kernel = Shared.GetMembootImage().ToArray();
            }

            if (!hakchi.Shell.IsOnline)
            {
                var size = Shared.CalcKernelSize(kernel);
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
            }

            tasker.SetStatus(Resources.UploadingKernel);

            if (hakchi.Shell.IsOnline && hakchi.Shell.Execute("[ -f /proc/atags ]") == 0)
            {
                // do we care about stock kernel
                if (stockKernel == null)
                {
                    if (hakchi.MinimalMemboot) // already in minimal memboot?
                        return Conclusion.Success;
                    if (hakchi.Shell.ExecuteSimple("[ -e /bin/recovery ] && echo \"0\"") == "0") // recovery function?
                    {
                        try
                        {
                            hakchi.Shell.ExecuteSimple("/bin/recovery", 100);
                        }
                        catch
                        {
                            // no-op
                        }
                        return Conclusion.Success;
                    }
                }

                hakchi.Shell.ExecuteSimple("mkdir -p /tmp/kexec/", throwOnNonZero: true);
                hakchi.Shell.Execute(
                    command: "cat > /tmp/kexec/kexec && chmod +x /tmp/kexec/kexec",
                    stdin: File.OpenRead(Shared.PathCombine(Program.BaseDirectoryInternal, "tools", "arm", "kexec.static")),
                    throwOnNonZero: true
                );
                hakchi.Shell.Execute(
                    command: "cat > /tmp/kexec/unpackbootimg && chmod +x /tmp/kexec/unpackbootimg",
                    stdin: File.OpenRead(Shared.PathCombine(Program.BaseDirectoryInternal, "tools", "arm", "unpackbootimg.static")),
                    throwOnNonZero: true
                );

                hakchi.Shell.ExecuteSimple("uistop");

                TrackableStream kernelStream = new TrackableStream(kernel);

                kernelStream.OnProgress += tasker.OnProgress;

                hakchi.Shell.Execute(
                    command: "cat > /tmp/kexec/boot.img; cd /tmp/kexec/; ./unpackbootimg -i boot.img",
                    stdin: kernelStream,
                    throwOnNonZero: true
                );
                hakchi.Shell.ExecuteSimple("cd /tmp/kexec/ && ./kexec -l -t zImage boot.img-zImage \"--command-line=$(cat boot.img-cmdline)\" --ramdisk=boot.img-ramdisk.gz --atags", 0, true);
                hakchi.Shell.ExecuteSimple("cd /tmp/; umount -ar", 0);
                try
                {
                    hakchi.Shell.ExecuteSimple("/tmp/kexec/kexec -e", 100);
                }
                catch
                {
                    // no-op
                }
            }
            else
            {
                int progress = 0;
                int maxProgress = (int)((double)kernel.Length / (double)67000 + 50);
                fel.WriteMemory(Fel.transfer_base_m, kernel,
                    delegate (Fel.CurrentAction action, string command)
                    {
                        switch (action)
                        {
                            case Fel.CurrentAction.WritingMemory:
                                tasker.SetStatus(Resources.UploadingKernel);
                                break;
                        }
                        progress++;
                        tasker.SetProgress(progress, maxProgress);
                    }
                );

                var bootCommand = string.Format("boota {0:x}", Fel.transfer_base_m);
                tasker.SetStatus(Resources.ExecutingCommand + " " + bootCommand);
                fel.RunUbootCmd(bootCommand, true);
            }
            return Conclusion.Success;
        }

        public Conclusion GetStockKernel(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.InvokeRequired)
                return (Conclusion)tasker.HostForm.Invoke(new Func<Tasker, Object, Conclusion>(GetStockKernel), new object[] { tasker, syncObject });

            tasker.SetStatus(Resources.DumpingKernel);
            stockKernel = new MemoryStream();
            bool hasNandBackup = (hakchi.Shell.Execute("[ \"$(sntool sunxi_flash phy_read 68 1 | dd status=none bs=7 count=1)\" = \"ANDROID\" ]") == 0);

            if (hasNandBackup)
            {
                hakchi.Shell.Execute("sntool sunxi_flash read_boot2 68", null, stockKernel, null, 0, true);
                if(stockKernel.Length == 0)
                    throw new Exception("Error reading backup kernel.");

                return Conclusion.Success;
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Kernel Image (*.img)|*.img";
                    ofd.InitialDirectory = Shared.PathCombine(Program.BaseDirectoryExternal, "dump");
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.OpenRead(ofd.FileName).Length <= Fel.kernel_max_size)
                        {
                            byte[] kernelBytes = File.ReadAllBytes(ofd.FileName);

                            var md5 = System.Security.Cryptography.MD5.Create();
                            var hash = BitConverter.ToString(md5.ComputeHash(kernelBytes)).Replace("-", "").ToLower();
                            var matchedKernels = from k in correctKernels where k.Value.Contains(hash) select k.Key;
                            if (matchedKernels.Count() > 0)
                            {
                                stockKernel = new MemoryStream(kernelBytes);
                                return Conclusion.Success;
                            }
                        }
                        else
                        {
                            return Conclusion.Abort;
                        }
                    }
                }
            }
            return Conclusion.Error;
        }

        public Conclusion FlashStockKernel(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.UploadingKernel);
            if (stockKernel == null || stockKernel.Length == 0)
                return Conclusion.Error;

            stockKernel.Seek(0, SeekOrigin.Begin);
            hakchi.Shell.Execute("cat > /kernel.img && sntool kernel /kernel.img", stockKernel, null, null, 0, true);

            if (hakchi.Shell.Execute("hakchi flashBoot2 /kernel.img") != 0)
                throw new Exception(Resources.VerifyFailed);

            return Conclusion.Success;
        }

        public static Conclusion BootHakchi(Tasker tasker, Object syncObject = null)
        {
            // Continue the hakchi boot process
            tasker.SetStatus(Resources.BootingHakchi);
            MemoryStream hakchiLogStream = new MemoryStream();
            var splitStream = new SplitterStream(hakchiLogStream).AddStreams(Program.debugStreams);

            try
            {
                hakchi.Shell.Execute("boot", null, splitStream, splitStream, 0, true);
            }
            catch { }

            hakchiLogStream.Seek(0, SeekOrigin.Begin);
            string hakchiLog;
            using (StreamReader sr = new StreamReader(hakchiLogStream))
            {
                hakchiLog = sr.ReadToEnd();
            }
            foreach (string line in hakchiLog.Split(Convert.ToChar(0x0A)))
                if (line.StartsWith("flash md5 mismatch! "))
                    throw new Exception(line);

            return Conclusion.Success;
        }

        public TaskFunc HandleHakchi(HakchiTasks task)
        {
            var instance = this;
            return (Tasker tasker, Object sync) =>
            {
                switch (task)
                {
                    case HakchiTasks.Install:
                        tasker.SetStatus(Resources.InstallingHakchi);
                        break;
                    case HakchiTasks.Reset:
                        tasker.SetStatus(Resources.ResettingHakchi);
                        break;
                    case HakchiTasks.Uninstall:
                        tasker.SetStatus(Resources.Uninstalling);
                        break;
                }
                if (task == HakchiTasks.Reset || task == HakchiTasks.Uninstall)
                {
                    hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);
                    hakchi.Shell.Execute("rm -rf /newroot/var/lib/hakchi/");
                    hakchi.Shell.Execute("hakchi umount_base", null, null, null, 0, true);
                }

                if (task == HakchiTasks.Install || task == HakchiTasks.Reset)
                {
                    hakchi.Shell.Execute("echo \"cf_install=y\" >> /hakchi/config");
                    hakchi.Shell.Execute("echo \"cf_update=y\" >> /hakchi/config");
                    hakchi.Shell.Execute("mkdir -p /hakchi/transfer/");
                }

                return Conclusion.Success;
            };
        }
        
        public static TaskFunc RunCommand(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus($"Running command: {command}");
                hakchi.Shell.Execute(command, stdin, stdout, stderr, timeout, throwOnNonZero);
                return Conclusion.Success;
            };
        }

        public static TaskFunc ProcessNand(string nandDump, NandTasks task)
        {
            return (Tasker tasker, Object sync) =>
            {
                NandTasks[] validTasks = { NandTasks.DumpNand, NandTasks.DumpNandB, NandTasks.DumpNandC, NandTasks.FlashNandB, NandTasks.FlashNandC, NandTasks.FormatNandC };
                if (!validTasks.Contains(task))
                    throw new ArgumentOutOfRangeException("task");
                
                if (task == NandTasks.FlashNandB && !Shared.CheckHsqs(nandDump))
                    throw new Exception(Properties.Resources.InvalidHsqs);

                long partitionSize = 300 * 1024 * 1024;

                switch (task)
                {
                    case NandTasks.DumpNandB:
                    case NandTasks.FlashNandB:
                        hakchi.Shell.Execute("umount /newroot");
                        hakchi.Shell.Execute("cryptsetup close root-crypt");
                        hakchi.Shell.ExecuteSimple("cryptsetup open /dev/nandb root-crypt --type plain --cipher aes-xts-plain --key-file /key-file", 2000, true);

                        if (task == NandTasks.DumpNandB)
                            partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("echo $((($(hexdump -e '1/4 \"%u\"' -s $((0x28)) -n 4 /dev/mapper/root-crypt)+0xfff)/0x1000))", throwOnNonZero: true).Trim()) * 4 * 1024;

                        if (task == NandTasks.FlashNandB)
                            partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("blockdev --getsize64 /dev/mapper/root-crypt", throwOnNonZero: true));

                        break;

                    case NandTasks.DumpNandC:
                    case NandTasks.FlashNandC:
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("blockdev --getsize64 /dev/nandc", throwOnNonZero: true));
                        break;

                    case NandTasks.DumpNand:
                        partitionSize = 536870912;
                        break;
                    case NandTasks.FormatNandC:
                        var splitStream = new SplitterStream(Program.debugStreams);
                        hakchi.Shell.Execute("cat > /bin/mke2fs; chmod +x /bin/mke2fs", File.OpenRead(Shared.PathCombine(Program.BaseDirectoryInternal, "tools", "arm", "mke2fs.static")), null, null, 0, true);
                        hakchi.Shell.Execute("hakchi umount_base", null, splitStream, splitStream);
                        hakchi.Shell.Execute("yes | mke2fs -t ext4 -L data -b 4K -E stripe-width=32 -O ^huge_file,^metadata_csum /dev/nandc", null, splitStream, splitStream, 0, true);
                        hakchi.Shell.Execute("rm /bin/mke2fs");
                        return Conclusion.Success;
                }

                FileMode mode = FileMode.Create;

                if (task == NandTasks.FlashNandC || task == NandTasks.FlashNandB)
                    mode = FileMode.Open;

                tasker.SetStatus(mode == FileMode.Open ? Resources.FlashingNand : Resources.DumpingNand);
                using (var file = new TrackableFileStream(nandDump, mode))
                {
                    if (mode == FileMode.Open && file.Length > partitionSize)
                        throw new Exception(Resources.ImageTooLarge);

                    if (mode == FileMode.Create)
                        file.SetLength(partitionSize);

                    file.OnProgress += tasker.OnProgress;

                    switch (task)
                    {
                        case NandTasks.DumpNandB:
                            Shared.ShellPipe($"dd if=/dev/mapper/root-crypt bs=128K count={(partitionSize / 1024) / 4 }", null, file, throwOnNonZero: true);
                            break;

                        case NandTasks.FlashNandB:
                            Shared.ShellPipe("dd of=/dev/mapper/root-crypt bs=128K", file, throwOnNonZero: true);
                            hakchi.Shell.Execute("cryptsetup close root-crypt", throwOnNonZero: true);
                            break;

                        case NandTasks.DumpNandC:
                            Shared.ShellPipe($"dd if=/dev/nandc bs=128K count={(partitionSize / 1024) / 4 }", null, file, throwOnNonZero: true);
                            break;

                        case NandTasks.FlashNandC:
                            Shared.ShellPipe("dd of=/dev/nandc bs=128K", file, throwOnNonZero: true);
                            break;

                        case NandTasks.DumpNand:
                            Shared.ShellPipe("sntool sunxi_flash phy_read 0 1000", null, file, throwOnNonZero: true);
                            break;
                    }
                    file.Close();
                }

                tasker.SetStatus(Resources.Done);
                tasker.SetProgress(1, 1);
                return Conclusion.Success;
            };
        }

        public static TaskFunc FlashUboot(Fel.UbootType type)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.FlashingUboot);
                TrackableStream uboot;
                if(type == Fel.UbootType.Normal)
                {
                    uboot = new TrackableStream(Resources.uboot);
                }
                else
                {
                    uboot = new TrackableStream(Resources.ubootSD);
                }

                if (uboot.Length > 655360)
                {
                    throw new Exception(Resources.InvalidUbootSize + " " + uboot.Length);
                }

                uboot.OnProgress += tasker.OnProgress;

                hakchi.Shell.Execute("cat > /uboot.bin", uboot, null, null, 0, true);
                MemoryStream flashLog = new MemoryStream();
                var splitStream = new SplitterStream(flashLog).AddStreams(Program.debugStreams);
                if (hakchi.Shell.Execute("hakchi flashBoot2 /uboot.bin 8 5", null, splitStream) != 0)
                {
                    using (var sr = new StreamReader(flashLog))
                    {
                        throw new Exception(sr.ReadToEnd());
                    }
                }
                return Conclusion.Success;
            };
        }
    }
}
