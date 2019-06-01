using com.clusterrr.hakchi_gui.Properties;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    static class ShellTasks
    {
        public static Conclusion Reboot(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.Rebooting);
            try
            {
                hakchi.Shell.ExecuteSimple("sync; umount -ar; reboot -f", 100);
            } catch { }

            return Conclusion.Success;
        }

        public static Conclusion Shutdown(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.PoweringOff);
            try
            {
                hakchi.Shell.ExecuteSimple("sync; umount -ar; poweroff -f", 100);
            }
            catch { }

            return Conclusion.Success;
        }

        public static Conclusion MountBase(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus("hakchi mount_base");
            return hakchi.Shell.Execute("hakchi mount_base") == 0 ? Conclusion.Success : Conclusion.Error;
        }

        public static Conclusion UnmountBase(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus("hakchi umount_base");
            return hakchi.Shell.Execute("hakchi umount_base") == 0 ? Conclusion.Success : Conclusion.Error;
        }

        public static TaskFunc ShellCommand(string command, Stream stdin = null, Stream stdout = null, Stream stderr = null, int timeout = 0, bool throwOnNonZero = false)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                tasker.SetStatus(command);
                hakchi.Shell.Execute(command, stdin, stdout, stderr, timeout, throwOnNonZero);
                return Conclusion.Success;
            };
        }

        public static Tasker.Conclusion ShowSplashScreen(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.PleaseWait);
            return hakchi.ShowSplashScreen() == 0 ? Tasker.Conclusion.Success : Tasker.Conclusion.Error;
        }

        public static Tasker.Conclusion SyncConfig(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.UploadingConfig);
            hakchi.SyncConfig(ConfigIni.GetConfigDictionary());
            return Tasker.Conclusion.Success;
        }

        public static TaskFunc FormatDevice(string device)
        {
            Regex mke2fsHeaderRegex = new Regex(@"(Allocating group tables|Writing inode tables|Creating journal \(\d+ blocks\)|Writing superblocks and filesystem accounting information)", RegexOptions.Compiled);
            Regex mke2fsProgressRegex = new Regex(@"(\d+/\d+|done)", RegexOptions.Compiled);

            return (Tasker tasker, Object sync) =>
            {
                using (EventStream formatProgress = new EventStream())
                using (var splitStream = new SplitterStream(Program.debugStreams))
                {
                    splitStream.AddStreams(formatProgress);
                    string currentHeading = null;
                    formatProgress.OnData += (byte[] buffer) =>
                    {
                        string data = Encoding.ASCII.GetString(buffer);
                        MatchCollection matches = mke2fsHeaderRegex.Matches(data);
                        if (matches.Count > 0)
                        {
                            currentHeading = matches[matches.Count - 1].Value;
                            tasker.SetStatus(currentHeading);
                        }

                        matches = mke2fsProgressRegex.Matches(data);

                        if (matches.Count > 0 && currentHeading != null && currentHeading != "Writing superblocks and filesystem accounting information")
                        {
                            tasker.SetStatus($"{currentHeading}: {matches[matches.Count - 1].Value}");
                            if (currentHeading == "Writing inode tables")
                            {
                                var inodes = matches[matches.Count - 1].Value.Split("/"[0]);
                                tasker.SetProgress(long.Parse(inodes[0]), long.Parse(inodes[1]));
                            }
                        }
                    };

                    hakchi.Shell.Execute($"yes | mke2fs -t ext4 -L data -b 4K -E stripe-width=32 -O ^huge_file,^metadata_csum {Shared.EscapeShellArgument(device)}", null, splitStream, splitStream, 0, true);

                    splitStream.RemoveStream(formatProgress);

                    return Conclusion.Success;
                }
            };
        }
    }
}
