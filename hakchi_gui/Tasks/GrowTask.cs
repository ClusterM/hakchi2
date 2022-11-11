using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.Sunxi;
using System.Diagnostics;
using System.Linq;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class GrowTask: ITaskCollection
    {
        private bool shouldGrow = false;
        private bool rebootToRecovery = false;
        private long estimatedSize = 0;
        private bool noReboot = false;
        public TaskFunc[] Tasks
        {
            get => new TaskFunc[]
            {
                PromptUser,
                Grow,
                Reboot,
                WaitForReboot
            };
        }

        public GrowTask(bool rebootToRecovery = false, bool shouldGrow = false)
        {
            this.rebootToRecovery = rebootToRecovery;
            this.shouldGrow = shouldGrow;
        }

        public Conclusion PromptUser(Tasker tasker, object syncObject)
        {
            var info = NandInfo.GetNandInfo();
            if (info != null)
            {
                var partUdisk = info.Partitions.Where(e => e.Label == "UDISK");

                if (partUdisk.Count() == 0)
                {
                    shouldGrow = false;
                    return Conclusion.Success;
                }

                var newInfo = NandInfo.GetNandInfo("sunxi-part grow --dry-run");

                estimatedSize = newInfo.GetDataPartition().Size - info.GetDataPartition().Size;

                if (shouldGrow)
                    return Conclusion.Success;

                var title = Resources.DoYouWantToExpandTheDataPartition;
                string message = Resources.PartitionExpandQ;
                
                if (hakchi.IsMd())
                    message = Resources.PartitionExpandQMd;

                message = string.Format(message, Shared.SizeSuffix(estimatedSize, 2));
                this.shouldGrow = (MessageForm.Show(tasker.HostForm, title, message, Resources.sign_question, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button1) == MessageForm.Button.Yes);
            }
            else
            {
                shouldGrow = false;
            }

            return Conclusion.Success;
        }

        public Conclusion Grow(Tasker tasker, object syncObject)
        {
            if (!shouldGrow)
                return Conclusion.Success;

            Trace.WriteLine($"Expanding data partition by {Shared.SizeSuffix(estimatedSize, 2)}.");

            using (var split = new SplitterStream(Program.debugStreams))
                return ShellTasks.ShellCommand("sunxi-part grow", null, split, split)(tasker, syncObject);
        }

        public Conclusion Reboot(Tasker tasker, object syncObject)
        {
            if (!shouldGrow)
                return Conclusion.Success;

            if (hakchi.Shell.Execute("sntool ismd") == 0)
            {
                noReboot = true;
                return Conclusion.Success;
            }

            if (rebootToRecovery)
                return MembootTasks.Memboot(tasker, syncObject);

            return ShellTasks.Reboot(tasker, syncObject);
        }

        public Conclusion WaitForReboot(Tasker tasker, object syncObject)
        {
            if (noReboot || !shouldGrow)
                return Conclusion.Success;

            return MembootTasks.WaitForShellCycle(-1)(tasker, syncObject);
        }
    }
}
