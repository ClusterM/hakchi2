using com.clusterrr.hakchi_gui.Properties;
using System;
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
                hakchi.Shell.Execute("sync; umount -ar; reboot -f");
            } catch { }

            return Conclusion.Success;
        }
        public static Conclusion Shutdown(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.PoweringOff);
            try
            {
                hakchi.Shell.Execute("sync; umount -ar; poweroff -f");
            } catch { }

            return Conclusion.Success;
        }
    }
}
