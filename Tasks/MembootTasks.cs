using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using FelLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public enum UbootType { Normal, SD }
    public class MembootTasks
    {
        // Constants
        public const int MembootWaitDelay = 120000;

        // Enums
        public enum MembootTaskType {
            InstallHakchi,
            ResetHakchi,
            UninstallHakchi,
            DumpNand,
            DumpSystemPartition,
            FlashSystemPartition,
            DumpUserPartition,
            FlashUserPartition,
            FormatUserPartition,
            ProcessMods,
            Memboot,
            MembootOriginal,
            MembootRecovery,
            FlashNormalUboot,
            FlashSDUboot,
            FactoryReset,
            DumpStockKernel,
        }
        
        public enum HakchiTasks { Install, Reset, Uninstall }
        public enum NandTasks { DumpNand, DumpSystemPartition, FlashSystemPartition, DumpUserPartition, FlashUserPartition, FormatUserPartition }

        // Private variables
        private Fel fel;
        private MemoryStream stockKernel;
        private bool ignoreBackupKernel = false;

        // Public variables
        public bool userRecovery = false;

        // Public Static variables
        public static readonly IReadOnlyDictionary<hakchi.ConsoleType, string[]> correctKernels = Shared.CorrectKernels();
        public static readonly IReadOnlyDictionary<hakchi.ConsoleType, string[]> correctKeys = Shared.CorrectKeys();

        public readonly TaskFunc[] Tasks;

        public MembootTasks(MembootTaskType type, string[] hmodsInstall = null, string[] hmodsUninstall = null, string dumpPath = null, bool forceRecoveryReload = false, bool ignoreBackupKernel = false, bool requireSD = false)
        {
            this.ignoreBackupKernel = ignoreBackupKernel;
            userRecovery = (hakchi.Shell.IsOnline && hakchi.MinimalMemboot && hakchi.UserMinimalMemboot);

            fel = new Fel();
            List<TaskFunc> taskList = new List<TaskFunc>();
            if (!hakchi.MinimalMemboot || forceRecoveryReload)
            {
                taskList.Add(WaitForFelOrMembootableShell(requireSD));
                taskList.Add(TaskIf(() => { return hakchi.Shell.IsOnline; }, Memboot, MembootFel));
                taskList.Add(WaitForShellCycle(-1));
                taskList.Add(ShellTasks.ShowSplashScreen);
            }
            switch (type)
            {
                case MembootTaskType.InstallHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        ModTasks.TransferHakchiHmod("/hakchi/transfer")
                    }); ;
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.ResetHakchi:
                    taskList.AddRange(new TaskFunc[]
                    {
                        HandleHakchi(HakchiTasks.Reset),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        ModTasks.TransferHakchiHmod("/hakchi/transfer")
                    });
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.UninstallHakchi:
                    taskList.Add(TaskIf(() => hakchi.IsMdPartitioning, SuccessTask, GetStockKernel));

                    if (ignoreBackupKernel)
                        taskList.Add(TaskIf(() => hakchi.IsMdPartitioning, SuccessTask, EraseBackupKernel));

                    taskList.AddRange(new TaskFunc[]{
                        TaskIf(() => hakchi.IsMdPartitioning, SuccessTask, FlashKernel),
                        HandleHakchi(HakchiTasks.Uninstall)
                    });

                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FactoryReset:
                    taskList.Add(TaskIf(() => hakchi.IsMdPartitioning, ErrorTask(Resources.NotSupportedOnThisPlatform), SuccessTask));
                    taskList.Add(GetStockKernel);

                    if (ignoreBackupKernel)
                        taskList.Add(EraseBackupKernel);

                    taskList.AddRange(new TaskFunc[] {
                        FlashKernel,
                        ShellTasks.UnmountBase,
                        (Tasker tasker, Object syncObject) =>  ShellTasks.FormatDevice($"/dev/{Sunxi.NandInfo.GetNandInfo().GetDataPartition().Device}")(tasker, syncObject)
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

                case MembootTaskType.DumpSystemPartition:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpSystemPartition)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.DumpUserPartition:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.DumpUserPartition)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashSystemPartition:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashSystemPartition)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashUserPartition:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ProcessNand(dumpPath, NandTasks.FlashUserPartition)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FormatUserPartition:
                    taskList.AddRange(new TaskFunc[]
                    {
                        ShellTasks.UnmountBase,
                        (Tasker tasker, Object syncObject) =>  ShellTasks.FormatDevice($"/dev/{Sunxi.NandInfo.GetNandInfo().GetDataPartition().Device}")(tasker, syncObject),
                        HandleHakchi(HakchiTasks.Install),
                        ModTasks.TransferBaseHmods("/hakchi/transfer"),
                        ModTasks.TransferHakchiHmod("/hakchi/transfer")
                    });
                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    break;

                case MembootTaskType.ProcessMods:
                    bool unmountAfter = userRecovery && hakchi.Shell.Execute("hakchi eval 'mountpoint -q \"$mountpoint/var/lib/\"'") == 0;
                    
                    taskList.Add(ShellTasks.MountBase);
                    taskList.AddRange(new ModTasks(hmodsInstall, hmodsUninstall).Tasks);

                    if (!userRecovery)
                        taskList.Add(BootHakchi);

                    if (unmountAfter)
                        taskList.Add(ShellTasks.UnmountBase);
                    break;

                case MembootTaskType.FlashNormalUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(UbootType.Normal)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.FlashSDUboot:
                    taskList.AddRange(new TaskFunc[]
                    {
                        FlashUboot(UbootType.SD)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;

                case MembootTaskType.Memboot:
                    taskList.Add(BootHakchi);
                    break;

                case MembootTaskType.MembootOriginal:
                    taskList.Add(TaskIf(() => hakchi.IsMdPartitioning, ErrorTask(Resources.NotSupportedOnThisPlatform), SuccessTask));
                    taskList.Add(BootBackup2);
                    break;

                case MembootTaskType.MembootRecovery:
                    break;

                case MembootTaskType.DumpStockKernel:
                    taskList.AddRange(new TaskFunc[]
                    {
                        TaskIf(() => hakchi.IsMdPartitioning, ErrorTask(Resources.NotSupportedOnThisPlatform), SuccessTask),
                        DumpStockKernel(dumpPath)
                    });
                    if (!userRecovery)
                        taskList.Add(ShellTasks.Reboot);

                    break;
            }
            Tasks = taskList.ToArray();
        }

        private Conclusion WaitForFelOrMembootableShell(Tasker tasker, Object syncObject = null, bool sdRequired = false)
        {
            var hostForm = tasker.GetSpecificViews<Form>().FirstOrDefault();
            if (hostForm == default(Form))
                hostForm = tasker.HostForm;
            if (hostForm.InvokeRequired)
            {
                return (Conclusion)hostForm.Invoke(new Func<Tasker, Object, bool, Conclusion>(WaitForFelOrMembootableShell), new object[] { tasker, syncObject, sdRequired });
            }

            tasker.SetStatus(Resources.WaitingForDevice);
            if (hakchi.Shell.IsOnline && hakchi.Shell.Execute("[ -f /proc/atags ]") == 0)
            {
                if (sdRequired)
                {
                    if (hakchi.Shell.Execute("hakchi mmcUsed") == 0)
                    {
                        return Conclusion.Success;
                    }
                }
                else
                {
                    return Conclusion.Success;
                }
            }

            if (!WaitingFelForm.WaitForDevice(hostForm))
                return Conclusion.Abort;

            fel.Fes1Bin = Resources.fes1;
            fel.UBootBin = hakchi.Hmod.GetUboot(UbootType.SD).ToArray();
            if (!fel.Open())
                throw new FelException("Can't open device");
            tasker.SetStatus(Resources.UploadingFes1);
            fel.InitDram(true);
            return Conclusion.Success;
        }

        public TaskFunc WaitForFelOrMembootableShell(bool requireSd)
        {
            return (Tasker tasker, Object syncObject) => WaitForFelOrMembootableShell(tasker, syncObject, requireSd);
        }

        public static Conclusion WaitForShellCycle(Tasker tasker, Object syncObject = null, int maxWaitTime = -1, string title = null, string message = null)
        {
            var hostForm = tasker.GetSpecificViews<Form>().FirstOrDefault();
            if (hostForm == default(Form))
                hostForm = tasker.HostForm;
            if (hostForm.InvokeRequired)
            {
                return (Conclusion)hostForm.Invoke(new Func<Tasker, object, int, string, string, Conclusion>(WaitForShellCycle), new object[] { tasker, syncObject, maxWaitTime, title, message });
            }

            tasker.SetStatus(Resources.WaitingForDevice);
            tasker.PushState(State.Waiting);
            var result = WaitingShellCycleForm.WaitForDevice(hostForm, maxWaitTime == -1 ? MembootWaitDelay : maxWaitTime);
            tasker.PopState();
            if (result == DialogResult.OK)
            {
                return Conclusion.Success;
            }
            else if (result == DialogResult.No)
            {
                MessageForm.Show(hostForm, title == null ? Resources.WaitingForDevice : title, message == null ? Resources.WaitingForDeviceTakingALongTime : message, Resources.sign_clock);
            }

            return Conclusion.Abort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxWaitTime">The amount of time to wait.
        /// 
        /// If this is -1, the default duration will be used
        /// If this is 0, it will wait indefinitely or until closed.</param>
        /// <returns></returns>
        public static TaskFunc WaitForShellCycle(int maxWaitTime = -1, string title = null, string message = null)
        {
            return (Tasker tasker, Object syncObject) => WaitForShellCycle(tasker, syncObject, maxWaitTime, title, message);
        }

        public Conclusion Memboot(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Membooting);
            if (!hakchi.Shell.IsOnline)
                return Conclusion.Abort;

            // get kernel image (only custom kernel with this method)
            byte[] kernel = hakchi.Hmod.GetMembootImage().ToArray();

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
                kernel = hakchi.Hmod.GetMembootImage().ToArray();
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
            byte[] kernel = hakchi.Hmod.GetMembootImage().ToArray();
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

        public static Conclusion EraseBackupKernel(Tasker tasker, Object syncObject = null)
        {
            return hakchi.Shell.Execute("sunxi-flash log_write 68 1 </dev/zero") == 0 ? Conclusion.Success : Conclusion.Error;
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

            if ((!ignoreBackupKernel) && hakchi.Shell.Execute("hakchi getBackup2", null, stockKernel) == 0)
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

        public Conclusion FlashKernel(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.UploadingKernel);
            if (stockKernel == null || stockKernel.Length == 0)
                return Conclusion.Error;

            stockKernel.Seek(0, SeekOrigin.Begin);
            hakchi.Shell.Execute("cat > /kernel.img", stockKernel, null, null, 0, true);
            
            if (hakchi.Shell.Execute("sntool check /kernel.img") != 0)
                throw new Exception(Resources.KernelCheckFailed);

            hakchi.Shell.Execute("sntool kernel /kernel.img", null, null, null, 0, true);

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
                    hakchi.Shell.Execute("hakchi mod_uninstall");
                    hakchi.Shell.Execute("hakchi umount_base", null, null, null, 0, true);
                }

                if (task == HakchiTasks.Install || task == HakchiTasks.Reset)
                {
                    hakchi.Shell.Execute("echo \"cf_install=y\" >> /hakchi/config");
                    hakchi.Shell.Execute("echo \"cf_update=y\" >> /hakchi/config");
                    hakchi.Shell.Execute("mkdir -p /hakchi/transfer/");
                    if (hakchi.IsMdPartitioning)
                    {
                        hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);

                        var squashfs = hakchi.Shell.ExecuteSimple("hakchi get squashfs").Trim();
                        var version = hakchi.Shell.ExecuteSimple($"cat \"{squashfs}/version\"", 0, true).Trim();
                        using (var versionMemoryStream = new MemoryStream())
                        using (var hasher = new MD5CryptoServiceProvider())
                        {
                            hakchi.Shell.Execute($"cd \"{squashfs}\" && (echo \"$(cat version)\"; find -type d | sort; find -type l | sort | while read link; do echo \"$link -> $(readlink \"$link\")\"; done; find -type f | sort | while read file; do md5sum \"$file\"; done)", null, versionMemoryStream, versionMemoryStream, 0, true);
                            versionMemoryStream.Seek(0, SeekOrigin.Begin);
                            var hash = BitConverter.ToString(hasher.ComputeHash(versionMemoryStream)).Replace("-", "").ToLower();
                            versionMemoryStream.Seek(0, SeekOrigin.Begin);

                            bool knownHash = false;
                            bool knownVersion = false;

                            foreach (var moon in Shared.MoonHashes)
                            {
                                if (moon.Substring(0, 32) == hash)
                                {
                                    knownHash = true;
                                }

                                if (moon.Substring(34) == version)
                                {
                                    knownVersion = true;
                                }

                                if (knownHash && knownVersion)
                                    break;
                            }

                            if (knownVersion)
                            {
                                if (knownHash)
                                {
                                    // Hash good
                                    Trace.WriteLine(version);
                                }
                                else
                                {
                                    // Hash doesn't match for version
                                    if (!Directory.Exists(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes")))
                                        Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes"));

                                    File.WriteAllBytes(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes", $"mismatched_{version}_{hash}"), versionMemoryStream.ToArray());

                                    Trace.WriteLine(Encoding.UTF8.GetString(versionMemoryStream.ToArray()));
                                    if (MessageForm.Show("System Modification Detected", $"The system files appear to have been modified:\n\nVersion: {version}\nHash: {hash}\n\nDo you want to continue?", Resources.sign_error, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) != MessageForm.Button.Yes)
                                    {
                                        return Conclusion.Abort;
                                    }
                                }
                            }
                            else
                            {
                                // Unknown version
                                if (!Directory.Exists(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes")))
                                    Directory.CreateDirectory(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes"));

                                File.WriteAllBytes(Path.Combine(Program.BaseDirectoryExternal, "moon_hashes", $"unknown_{version}_{hash}"), versionMemoryStream.ToArray());

                                Trace.WriteLine(Encoding.UTF8.GetString(versionMemoryStream.ToArray()));
                                if (MessageForm.Show("Unknown System Version Detected", $"The system files are an unknown version:\n\nVersion: {version}\nHash: {hash}\n\nDo you want to continue?", Resources.sign_error, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) != MessageForm.Button.Yes){
                                    return Conclusion.Abort;
                                }
                            }
                        }
                        
                        hakchi.Shell.Execute("hakchi umount_base", null, null, null, 0, true);
                    }
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
                NandTasks[] validTasks = { NandTasks.DumpNand, NandTasks.DumpSystemPartition, NandTasks.DumpUserPartition, NandTasks.FlashSystemPartition, NandTasks.FlashUserPartition };
                if (!validTasks.Contains(task))
                    throw new ArgumentOutOfRangeException("task");
                
                if (task == NandTasks.FlashSystemPartition && !(Files.CheckFileType.IsSquashFs(nandDump) || Files.CheckFileType.IsExtFs(nandDump)))
                    throw new Exception(Properties.Resources.InvalidHsqs);


                bool isTar = false;
                if (task == NandTasks.FlashUserPartition)
                {
                    isTar = Files.CheckFileType.IsTar(nandDump);
                    if (!(isTar || Files.CheckFileType.IsExtFs(nandDump)))
                        throw new Exception(Properties.Resources.InvalidUserDataBackup);
                }

                var nandInfo = Sunxi.NandInfo.GetNandInfo();

                long partitionSize = 300 * 1024 * 1024;
                var splitStream = new SplitterStream(Program.debugStreams);
                string rootfsDevice = $"/dev/{nandInfo.GetRootfsPartition().Device}";
                string osDecryptedDevice = rootfsDevice;
                string userDataDevice = $"/dev/{nandInfo.GetDataPartition().Device}";
                bool hasKeyfile = hakchi.Shell.Execute("[ -f /key-file ]") == 0;

                if (hasKeyfile)
                    osDecryptedDevice = "/dev/mapper/root-crypt";

                bool systemIsHsqs = hakchi.Shell.ExecuteSimple($"dd if={osDecryptedDevice} bs=1 count=4", 0) == "HSQS";

                switch (task)
                {
                    case NandTasks.DumpSystemPartition:
                        if (systemIsHsqs)
                        {
                            partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"echo $((($(hexdump -e '1/4 \"%u\"' -s $((0x28)) -n 4 {osDecryptedDevice})+0xfff)/0x1000))", throwOnNonZero: true).Trim()) * 4 * 1024;
                        }
                        else
                        {
                            partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"blockdev --getsize64 {osDecryptedDevice}", throwOnNonZero: true));
                        }
                        break;

                    case NandTasks.FlashSystemPartition:
                        hakchi.Shell.Execute("hakchi umount_base", null, splitStream, splitStream);
                        hakchi.Shell.Execute("umount /newroot");
                        if (hasKeyfile)
                        {
                            hakchi.Shell.Execute("cryptsetup close root-crypt");
                            hakchi.Shell.ExecuteSimple($"cryptsetup open {rootfsDevice} root-crypt --type plain --cipher aes-xts-plain --key-file /key-file", 2000, true);
                        }

                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"blockdev --getsize64 {osDecryptedDevice}", throwOnNonZero: true));
                        break;

                    case NandTasks.DumpUserPartition:
                        hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("df -B 1 | grep /newroot/var/lib | head -n 1 | awk -e '{print $3 }'", throwOnNonZero: true).Trim());
                        break;

                    case NandTasks.FlashUserPartition:
                        partitionSize = long.Parse(hakchi.Shell.ExecuteSimple($"blockdev --getsize64 {userDataDevice}", throwOnNonZero: true));
                        break;

                    case NandTasks.DumpNand:
                        partitionSize = 536870912;
                        break;
                }

                FileMode mode = FileMode.Create;

                if (task == NandTasks.FlashUserPartition || task == NandTasks.FlashSystemPartition)
                    mode = FileMode.Open;

                tasker.SetStatus(mode == FileMode.Open ? Resources.FlashingNand : Resources.DumpingNand);
                using (var file = new TrackableFileStream(nandDump, mode))
                {
                    if (mode == FileMode.Open && file.Length > partitionSize)
                        throw new Exception(Resources.ImageTooLarge);

                    if (mode == FileMode.Create && task != NandTasks.DumpUserPartition && task != NandTasks.DumpSystemPartition)
                        file.SetLength(partitionSize);

                    if (task == NandTasks.DumpUserPartition)
                    {
                        file.OnProgress += (long position, long length) =>
                        {
                            tasker.OnProgress(Math.Min(position, partitionSize), partitionSize);
                        };
                    }
                    else if (task == NandTasks.DumpSystemPartition && !systemIsHsqs)
                    {
                        file.OnProgress += (long position, long length) =>
                        {
                            tasker.OnProgress(Math.Min(position, partitionSize) + partitionSize, partitionSize * 2);
                        };
                    }
                    else
                    {
                        file.OnProgress += tasker.OnProgress;
                    }

                    switch (task)
                    {
                        case NandTasks.DumpSystemPartition:
                            if (systemIsHsqs)
                            {
                                Shared.ShellPipe($"dd if={osDecryptedDevice} bs=128K count={(partitionSize / 1024) / 4 }", null, file, throwOnNonZero: true);
                            }
                            else
                            {
                                Regex mksquashfsProgress = new Regex(@"(\d+)/(\d+)", RegexOptions.Compiled);
                                using (var mksquashfs = File.OpenRead(Path.Combine(Program.BaseDirectoryInternal, "tools", "mksquashfs")))
                                {
                                    hakchi.Shell.Execute("cat > /mksquashfs", mksquashfs, throwOnNonZero: true);
                                    hakchi.Shell.Execute("chmod +x /mksquashfs", throwOnNonZero: true);
                                }

                                hakchi.Shell.ExecuteSimple("hakchi mount_base");
                                hakchi.Shell.ExecuteSimple("mkdir -p /tmp/");
                                using (EventStream mkSquashfsProgress = new EventStream())
                                {
                                    splitStream.AddStreams(mkSquashfsProgress);
                                    mkSquashfsProgress.OnData += (byte[] buffer) =>
                                    {
                                        string data = Encoding.ASCII.GetString(buffer);
                                        MatchCollection matches = mksquashfsProgress.Matches(data);

                                        if (matches.Count > 0)
                                        {
                                            tasker.SetProgress(long.Parse(matches[matches.Count - 1].Groups[1].Value), long.Parse(matches[matches.Count - 1].Groups[2].Value) * 2);
                                        }
                                    };

                                    hakchi.Shell.Execute("/mksquashfs /newroot/var/squashfs /tmp/rootfs.hsqs", null, splitStream, splitStream, throwOnNonZero: true);

                                    splitStream.RemoveStream(mkSquashfsProgress);
                                }

                                partitionSize = long.Parse(hakchi.Shell.ExecuteSimple("ls -la /tmp/rootfs.hsqs | awk -e '{ print $5 }'"));
                                
                                hakchi.Shell.ExecuteSimple("hakchi umount_base");
                                Shared.ShellPipe("cat /tmp/rootfs.hsqs", stdout: file);
                                hakchi.Shell.ExecuteSimple("rm /tmp/rootfs.hsqs");
                            }
                            
                            break;

                        case NandTasks.FlashSystemPartition:
                            Shared.ShellPipe($"dd of={osDecryptedDevice} bs=128K", file, throwOnNonZero: true);
                            if(hasKeyfile)
                                hakchi.Shell.Execute("cryptsetup close root-crypt", throwOnNonZero: true);
                            break;

                        case NandTasks.DumpUserPartition:
                            Shared.ShellPipe($"tar -cvC /newroot/var/lib/ .", null, file, null, throwOnNonZero: true);
                            break;

                        case NandTasks.FlashUserPartition:
                            if (isTar)
                            {
                                hakchi.Shell.Execute("hakchi mount_base", null, null, null, 0, true);
                                hakchi.Shell.Execute("rm -rf /newroot/var/lib/*", null, null, null, 0, true);
                                hakchi.Shell.Execute("tar -xvC /newroot/var/lib/", file, throwOnNonZero: true);
                            }
                            else
                            {
                                Shared.ShellPipe($"dd of={userDataDevice} bs=128K", file, throwOnNonZero: true);
                            }
                            break;

                        case NandTasks.DumpNand:
                            hakchi.Shell.Execute("hakchi umount_base", null, splitStream, splitStream, 0, true);
                            Shared.ShellPipe("sntool sunxi_flash phy_read 0", null, file, throwOnNonZero: true);
                            break;
                    }
                    file.Close();
                }

                tasker.SetStatus(Resources.Done);
                tasker.SetProgress(1, 1);
                return Conclusion.Success;
            };
        }

        public static TaskFunc FlashUboot(UbootType type)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.FlashingUboot);
                
                MemoryStream flashLog = new MemoryStream();
                var splitStream = new SplitterStream(flashLog).AddStreams(Program.debugStreams);
                if (hakchi.Shell.Execute($"sntool sd {(type == UbootType.SD ? "enable" : "disable")}", null, splitStream, splitStream) != 0)
                {
                    flashLog.Seek(0, SeekOrigin.Begin);
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
