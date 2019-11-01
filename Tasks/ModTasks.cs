using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    
    public class ModTasks
    {
        public class ModObject
        {
            public string[] InstalledHmods = new String[] { };
            public List<Hmod.Hmod> LoadedHmods = new List<Hmod.Hmod>();
            public List<string> HmodsToLoad = new List<string>();
        }

        public TaskFunc[] Tasks;
        public ModTasks(string[] hmodsInstall = null, string[] hmodsUninstall = null, bool transferOnly = false, string transferPath = "/tmp/hmods", string uninstallFile = "/var/lib/hakchi/transfer/uninstall")
        {
            List<TaskFunc> tasksList = new List<TaskFunc>();

            if(hmodsInstall != null && hmodsInstall.Length > 0)
            {
                foreach (string hmod in hmodsInstall)
                {
                    tasksList.Add(TransferHmod(transferPath, hmod));

                    if (!transferOnly)
                        tasksList.Add(InstallHmods(transferPath));
                }
            }
            if(hmodsUninstall != null && hmodsUninstall.Length > 0)
                foreach (string hmod in hmodsUninstall)
                    tasksList.Add(UninstallHmod(hmod));

            Tasks = tasksList.ToArray();
        }

        public static TaskFunc TransferHmod(string transferPath, string hmod)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.TransferringMods);
                var modName = hmod + ".hmod";
                foreach (var dir in Shared.hmodDirectories)
                {
                    string hmodHakchiPath = Shared.EscapeShellArgument($"{transferPath}/{modName}");
                    if (Directory.Exists(Path.Combine(dir, modName)))
                    {

                        hakchi.Shell.ExecuteSimple($"mkdir -p {hmodHakchiPath}");
                        using (var hmodTar = new TarStream(Path.Combine(dir, modName)))
                        {
                            if (hmodTar.Length > 0)
                            {
                                using (TrackableStream hmodStream = new TrackableStream(hmodTar))
                                {
                                    hmodStream.OnProgress += tasker.OnProgress;
                                    
                                    hakchi.Shell.Execute($"tar -xvC {hmodHakchiPath}", hmodStream, null, null, 0, true);

                                }
                            }
                        }
                        break;
                    }
                    if (File.Exists(Path.Combine(dir, modName)))
                    {
                        hakchi.Shell.ExecuteSimple($"mkdir -p {hmodHakchiPath}");
                        using (var hmodStream = new TrackableFileStream(Path.Combine(dir, modName), FileMode.Open))
                        {
                            hmodStream.OnProgress += tasker.OnProgress;

                            hakchi.Shell.Execute($"tar -xzvC {hmodHakchiPath}", hmodStream, null, null, 0, true);
                        }
                        break;
                    }
                }
                return Conclusion.Success;
            };
        }

        public TaskFunc InstallHmods(string transferPath = "/tmp/hmods")
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.InstallingMods);
                bool commandSucceeded = false;
                
                var splitStream = new SplitterStream(Program.debugStreams);
                commandSucceeded = hakchi.Shell.Execute($"hakchi packs_install {Shared.EscapeShellArgument(transferPath)}", null, splitStream, splitStream) == 0;

                return commandSucceeded ? Conclusion.Success : Conclusion.Error;
            };
        }

        public TaskFunc UninstallHmod(string mod)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.UninstallingMods);
                bool commandSucceeded = false;
                
                var splitStream = new SplitterStream(Program.debugStreams);
                commandSucceeded = hakchi.Shell.Execute($"hakchi pack_uninstall {Shared.EscapeShellArgument(mod)}", null, splitStream, splitStream) == 0;

                return commandSucceeded ? Conclusion.Success : Conclusion.Error;
            };
        }

        public static TaskFunc TransferBaseHmods(string transferPath = "/hakchi/transfer")
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.TransferringMods);
                var escapedTransferPath = Shared.EscapeShellArgument(transferPath);

                using (var baseHmods = File.OpenRead(Path.Combine(Program.BaseDirectoryInternal, "basehmods.tar")))
                using (var hmodStream = new TrackableStream(baseHmods))
                {
                    hmodStream.OnProgress += tasker.OnProgress;
                    hakchi.Shell.Execute($"mkdir -p {escapedTransferPath}", null, null, null, 0, true);
                    hakchi.Shell.Execute($"tar -xvC {escapedTransferPath} --exclude='./hakchi.hmod'", hmodStream, null, null, 0, true);
                }
                return Conclusion.Success;
            };
        }

        public static TaskFunc TransferHakchiHmod(string transferPath = "/hakchi/transfer")
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(Resources.TransferringMods);
                var escapedTransferPath = Shared.EscapeShellArgument(transferPath);
                var escapedHmodPath = Shared.EscapeShellArgument($"{transferPath}/hakchi.hmod");
                var hmodStream = new TrackableStream(hakchi.Hmod.GetHmodStream());
                hmodStream.OnProgress += tasker.OnProgress;
                hakchi.Shell.Execute($"mkdir -p {escapedTransferPath}", null, null, null, 0, true);
                hakchi.Shell.Execute($"cat > {escapedHmodPath}", hmodStream, null, null, 0, true);
                return Conclusion.Success;
            };
        }

        public static Conclusion GetHmods(Tasker tasker, Object modObject)
        {
            if (!(modObject is ModObject)) return Conclusion.Error;
            ModObject unboxedObject = (ModObject)modObject;
            
            unboxedObject.LoadedHmods = new List<Hmod.Hmod>();
            if (unboxedObject.HmodsToLoad == null) return Conclusion.Error;
            tasker.SetStatus(Properties.Resources.LoadingHmods);
            int progress = 0;

            foreach (string mod in unboxedObject.HmodsToLoad)
            {
                unboxedObject.LoadedHmods.Add(new Hmod.Hmod(mod, unboxedObject.InstalledHmods));
                progress++;
                tasker.SetProgress(progress, unboxedObject.HmodsToLoad.Count);
            }
            tasker.SetProgress(1, 1);
            return Conclusion.Success;
        }
    }
}
