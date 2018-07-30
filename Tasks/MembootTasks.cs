using static com.clusterrr.hakchi_gui.Tasks.Tasker;
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

namespace com.clusterrr.hakchi_gui.Tasks
{
    class MembootTasks
    {
        // Constants
        public const int MembootWaitDelay = 120000;

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
            FactoryReset,
            DumpStockKernel
        }
        public enum HakchiTasks { Install, Reset, Uninstall }
        public enum NandTasks { DumpNand, DumpNandB, FlashNandB, DumpNandC, FlashNandC, FormatNandC }

        // Private variables
        private Fel fel;
        private MemoryStream stockKernel;

        // Public variables
        public bool userRecovery = false;

        // Public Static variables
        public static readonly Dictionary<hakchi.ConsoleType, string[]> correctKernels = Shared.CorrectKernels();
        public static readonly Dictionary<hakchi.ConsoleType, string[]> correctKeys = Shared.CorrectKeys();

        public readonly TaskFunc[] Tasks;

        public MembootTasks(MembootTaskType type, string[] hmodsInstall = null, string[] hmodsUninstall = null, string dumpPath = null, bool forceRecoveryReload = false)
        {
            userRecovery = (hakchi.Shell.IsOnline && hakchi.MinimalMemboot && hakchi.UserMinimalMemboot);

            fel = new Fel();
            List<TaskFunc> taskList = new List<TaskFunc>();
            if (!hakchi.MinimalMemboot || forceRecoveryReload)
            {
                taskList.Add(WaitForFelOrMembootableShell);
                taskList.Add(TaskIf(() => { return hakchi.Shell.IsOnline; }, Memboot, MembootFel));
                taskList.Add(WaitForShellCycle);
                taskList.Add(ShellTasks.ShowSplashScreen);
            }
            switch (type)
            {
                case MembootTaskType.InstallHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer")
                    });
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.ResetHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Reset),
                        ModTasks.TransferBaseHmods("/hakchi/transfer")
                    });
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.UninstallHakchi:
                    taskList.AddRange(new TaskFunc[] {
                        GetStockKernel,
                        FlashStockKernel,
                        HandleHakchi(HakchiTasks.Uninstall)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FactoryReset:
                    taskList.AddRange(new TaskFunc[] {
                        GetStockKernel,
                        FlashStockKernel,
                        ProcessNand(null, NandTasks.FormatNandC)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.DumpNand:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNand)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.DumpNandB:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNandB)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.DumpNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpNandC)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashNandB:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashNandB)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashNandC)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FormatNandC:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FormatNandC),
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer")
                    });
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.ProcessMods:
                    taskList.AddRange(new ModTasks(hmodsInstall, hmodsUninstall).Tasks);
                    break;

                case MembootTaskType.FlashNormalUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(Fel.UbootType.Normal)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashSDUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(Fel.UbootType.SD)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.Memboot:
                    taskList.Add(BootHakchi);
                    break;

                case MembootTaskType.MembootOriginal:
                    taskList.Add(BootBackup2);
                    break;

                case MembootTaskType.MembootRecovery:
                    break;

                case MembootTaskType.DumpStockKernel:
                    taskList.AddRange(new TaskFunc[]
                    {
                        DumpStockKernel(dumpPath)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;
            }
            Tasks = taskList.ToArray();
        }

        public Conclusion WaitForFelOrMembootableShell(Tasker tasker, Object syncObject = null)
        {
            var hostForm = tasker.GetSpecificViews<Form>().FirstOrDefault();
            if (hostForm == default(Form))
                hostForm = tasker.HostForm;
            if (hostForm.InvokeRequired)
            {
                return (Conclusion)hostForm.Invoke(new Func<Tasker, Object, Conclusion>(WaitForFelOrMembootableShell), new object[] { tasker, syncObject });
            }

            tasker.SetStatus(Resources.WaitingForDevice);
            if (hakchi.Shell.IsOnline && hakchi.Shell.Execute("[ -f /proc/atags ]") == 0)
                return Conclusion.Success;

            if (!WaitingFelForm.WaitForDevice(Shared.ClassicUSB.vid, Shared.ClassicUSB.pid, hostForm))
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

        public static Conclusion WaitForShellCycle(Tasker tasker, Object syncObject = null)
        {
            var hostForm = tasker.GetSpecificViews<Form>().FirstOrDefault();
            if (hostForm == default(Form))
                hostForm = tasker.HostForm;
            if (hostForm.InvokeRequired)
            {
                return (Conclusion)hostForm.Invoke(new Func<Tasker, object, Conclusion>(WaitForShellCycle), new object[] { tasker, syncObject });
            }

            tasker.SetStatus(Resources.WaitingForDevice);
            tasker.PushState(State.Waiting);
            var result = WaitingShellCycleForm.WaitForDevice(hostForm, MembootWaitDelay);
            tasker.PopState();
            if (result == DialogResult.OK)
            {
                return Conclusion.Success;
            }
            else if (result == DialogResult.No)
            {
                MessageForm.Show(hostForm, Resources.WaitingForDevice, Resources.WaitingForDeviceTakingALongTime, Resources.sign_clock);
            }

            return Conclusion.Abort;
        }

        public Conclusion Memboot(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Membooting);
            if (!hakchi.Shell.IsOnline)
                return Conclusion.Abort;

            // get kernel image (only custom kernel with this method)
            byte[] kernel = hakchi.GetMembootImage().ToArray();

            // override arguments
            string addedArgs = "";
            if (ConfigIni.Instance.ForceClovershell)
                addedArgs = " hakchi-clovershell";
            else if (ConfigIni.Instance.ForceNetwork)
                addedArgs = " hakchi-shell";

            // use detached-fallback script and up-to-date boot.img
            try
            {
                hakchi.Shell.ExecuteSimple("uistop");
                hakchi.Shell.ExecuteSimple("mkdir -p /tmp/kexec/", throwOnNonZero: true);
                hakchi.UploadFile(
                    Path.Combine(Program.BaseDirectoryInternal, "tools", "arm", "kexec.static"),
                    "/tmp/kexec/kexec");
                hakchi.UploadFile(
                    Path.Combine(Program.BaseDirectoryInternal, "tools", "arm", "detached-fallback"),
                    "/tmp/kexec/detached-fallback");

                TrackableStream kernelStream = new TrackableStream(kernel);
                kernelStream.OnProgress += tasker.OnProgress;
                hakchi.UploadFile(kernelStream, "/tmp/kexec/boot.img", false);

                try
                {
                    hakchi.Shell.ExecuteSimple("cd /tmp/kexec/; /bin/sh /tmp/kexec/detached-fallback recovery /tmp/kexec/boot.img" + addedArgs, 100);
                }
                catch { } // no-op
            }
            catch
            {
                try
                {
                    hakchi.Shell.ExecuteSimple("uistart");
                }
                catch { } // no-op
                throw;
            }

            return Conclusion.Success;
        }

        public Conclusion MembootKexec(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Membooting);
            if (!hakchi.Shell.IsOnline)
                return Conclusion.Abort;

            // load appropriate kernel
            byte[] kernel;
            if (stockKernel != null && stockKernel.Length > 0)
            {
                kernel = stockKernel.ToArray();
            }
            else
            {
                stockKernel = null;
                kernel = hakchi.GetMembootImage().ToArray();
            }

            // memboot using kexec (no way to force clovershell or shell)
            try
            {
                hakchi.Shell.ExecuteSimple("uistop");
                hakchi.Shell.ExecuteSimple("mkdir -p /tmp/kexec/", throwOnNonZero: true);
                hakchi.UploadFile(
                    Path.Combine(Program.BaseDirectoryInternal, "tools", "arm", "kexec.static"),
                    "/tmp/kexec/kexec");
                hakchi.UploadFile(
                    Path.Combine(Program.BaseDirectoryInternal, "tools", "arm", "unpackbootimg.static"),
                    "/tmp/kexec/unpackbootimg");

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
                catch { } // no-op
            }
            catch
            {
                try
                {
                    hakchi.Shell.ExecuteSimple("uistart");
                }
                catch { } // no-op
                throw;
            }

            return Conclusion.Success;
        }

        public Conclusion MembootFel(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Membooting);
            if (hakchi.Shell.IsOnline)
                return Conclusion.Abort;

            // check and adjust kernel size
            byte[] kernel = hakchi.GetMembootImage().ToArray();
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

            // clovershell override "hex-edit" boot.img
            if (ConfigIni.Instance.ForceClovershell || ConfigIni.Instance.ForceNetwork)
            {
                kernel.InPlaceStringEdit(64, 512, 0, (string str) => {
                    str.Replace("hakchi-shell", "").Replace("hakchi-clovershell", "");
                    if (ConfigIni.Instance.ForceClovershell)
                        str += " hakchi-clovershell";
                    else if (ConfigIni.Instance.ForceNetwork)
                        str += " hakchi-shell";
                    return str;
                });
            }

            // upload kernel through fel
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

            return Conclusion.Success;
        }

        public Conclusion BootBackup2(Tasker tasker, object syncObject)
        {
            tasker.SetStatus(Resources.Membooting);
            if (hakchi.Shell.Execute("hakchi haveBackup2") == 0)
            {
                try
                {
                    hakchi.Shell.Execute("hakchi bootBackup2");
                }
                catch
                {
                    // no-op
                }
            }
            else
            {
                tasker.AddTasks(
                    GetStockKernel,
                    MembootKexec);
            }
            return Conclusion.Success;
        }

        public static TaskFunc DumpStockKernel(string dumpPath)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                using (var stockKernelFile = File.Open(dumpPath, FileMode.Create))
                {
                    if (hakchi.Shell.Execute("hakchi getBackup2", null, stockKernelFile) == 0)
                    {
                        return Conclusion.Success;
                    }
                    else
                    {
                        stockKernelFile.Close();
                        File.Delete(dumpPath);
                        throw new Exception("Stock kernel not found on system.");
                    }
                }
            };
        }

        public Conclusion GetStockKernel(Tasker tasker, Object syncObject = null)
        {
            var hostForm = tasker.GetSpecificViews<Form>().FirstOrDefault();
            if (hostForm == default(Form))
                hostForm = tasker.HostForm;
            if (hostForm.InvokeRequired)
            {
                return (Conclusion)hostForm.Invoke(new Func<Tasker, Object, Conclusion>(GetStockKernel), new object[] { tasker, syncObject });
            }

            tasker.SetStatus(Resources.DumpingKernel);
            stockKernel = new MemoryStream();
            
            if (hakchi.Shell.Execute("hakchi getBackup2", null, stockKernel) == 0)
            {
                return Conclusion.Success;
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = $"{Resources.KernelDump}|*.img";
                    ofd.InitialDirectory = Shared.PathCombine(Program.BaseDirectoryExternal, "dump");
                    if (ofd.ShowDialog(hostForm) == DialogResult.OK)
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
                        MessageForm.Show(hostForm, Resources.Error, Resources.DumpOriginalKernelInvalid, Resources.sign_error);
                    }
                    else
                    {
                        return Conclusion.Abort;
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

            tasker.PushState(State.Waiting);
            try
            {
                hakchi.Shell.Execute("boot", null, splitStream, splitStream, 0, true);
            }
            catch { }
            tasker.PopState();

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
                
                if (task == NandTasks.FlashNandB && !Files.CheckFileType.IsSquashFs(nandDump))
                    throw new Exception(Properties.Resources.InvalidHsqs);


                bool isTar = false;
                if (task == NandTasks.FlashNandC)
                {
                    isTar = Files.CheckFileType.IsTar(nandDump);
                    if (!(isTar || Files.CheckFileType.IsExtFs(nandDump)))
                        throw new Exception(Properties.Resources.InvalidUserDataBackup);
                }

                long partitionSize = 300 * 1024 * 1024;
                var splitStream = new SplitterStream(Program.debugStreams);
                string osDecryptedDevice = "/dev/nandb";
                bool hasKeyfile = hakchi.Shell.Execute("[ -f /key-file ]") == 0;

                if (hasKeyfile)
                    osDecryptedDevice = "/dev/mapper/root-crypt";

                switch (task)
                {
                    case NandTasks.DumpNandB:
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"echo $((($(hexdump -e '1/4 \"%u\"' -s $((0x28)) -n 4 {osDecryptedDevice})+0xfff)/0x1000))", throwOnNonZero: true).Trim()) * 4 * 1024;
                        break;

                    case NandTasks.FlashNandB:
                        hakchi.Shell.Execute("hakchi umount_base", null, splitStream, splitStream);
                        hakchi.Shell.Execute("umount /newroot");
                        if (hasKeyfile)
                        {
                            hakchi.Shell.Execute("cryptsetup close root-crypt");
                            hakchi.Shell.ExecuteSimple("cryptsetup open /dev/nandb root-crypt --type plain --cipher aes-xts-plain --key-file /key-file", 2000, true);
                        }

                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"blockdev --getsize64 {osDecryptedDevice}", throwOnNonZero: true));
                        break;

                    case NandTasks.DumpNandC:
                        hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("df -B 1 | grep /newroot/var/lib | head -n 1 | awk -e '{print $3 }'", throwOnNonZero: true).Trim());
                        break;

                    case NandTasks.FlashNandC:
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("blockdev --getsize64 /dev/nandc", throwOnNonZero: true));
                        break;

                    case NandTasks.DumpNand:
                        partitionSize = 536870912;
                        break;

                    case NandTasks.FormatNandC:
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

                    if (mode == FileMode.Create && task != NandTasks.DumpNandC)
                        file.SetLength(partitionSize);

                    if (task == NandTasks.DumpNandC)
                    {
                        file.OnProgress += (long position, long length) =>
                        {
                            tasker.OnProgress(Math.Min(position, partitionSize), partitionSize);
                        };
                    }
                    else
                    {
                        file.OnProgress += tasker.OnProgress;
                    }

                    switch (task)
                    {
                        case NandTasks.DumpNandB:
                            Shared.ShellPipe($"dd if={osDecryptedDevice} bs=128K count={(partitionSize / 1024) / 4 }", null, file, throwOnNonZero: true);
                            break;

                        case NandTasks.FlashNandB:
                            Shared.ShellPipe($"dd of={osDecryptedDevice} bs=128K", file, throwOnNonZero: true);
                            if(hasKeyfile)
                                hakchi.Shell.Execute("cryptsetup close root-crypt", throwOnNonZero: true);
                            break;

                        case NandTasks.DumpNandC:
                            Shared.ShellPipe($"tar -cvC /newroot/var/lib/ .", null, file, null, throwOnNonZero: true);
                            break;

                        case NandTasks.FlashNandC:
                            if (isTar)
                            {
                                hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);
                                hakchi.Shell.Execute("rm -rf /newroot/var/lib/*", null, null, null, 0, true);
                                hakchi.Shell.Execute("tar -xvC /newroot/var/lib/", file, throwOnNonZero: true);
                            }
                            else
                            {
                                Shared.ShellPipe("dd of=/dev/nandc bs=128K", file, throwOnNonZero: true);
                            }
                            break;

                        case NandTasks.DumpNand:
                            hakchi.Shell.Execute("hakchi umount_base", null, splitStream, splitStream, 0, true);
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
                hakchi.Shell.Execute("truncate -s 640K /uboot.bin", null, null, null, 0, true);

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
