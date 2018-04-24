using com.clusterrr.hakchi_gui.Properties;
using System;
using System.IO;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class ShellTasks
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
    }
}
