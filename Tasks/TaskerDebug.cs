using System.Diagnostics;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class TaskerDebug : ITaskerView
    {
        TaskerDebug()
        {
        }

        public ITaskerView SetState(Tasker.State state)
        {
            Debug.WriteLine($"Set state {state.ToString()}");
            return this;
        }

        public ITaskerView SetProgress(long value, long maximum)
        {
            Debug.WriteLine($"Set progress {value} of {maximum}");
            return this;
        }

        public ITaskerView SetTitle(string title)
        {
            Debug.WriteLine($"Set title \"{title}\"");
            return this;
        }

        public ITaskerView SetStatusImage(System.Drawing.Image image)
        {
            Debug.WriteLine($"Set status image");
            return this;
        }

        public ITaskerView SetStatus(string status)
        {
            Debug.WriteLine($"Set status \"{status}\"");
            return this;
        }

        public ITaskerView Show()
        {
            Debug.WriteLine("Showing");
            return this;
        }

        public ITaskerView Close()
        {
            Debug.WriteLine("Hiding");
            return this;
        }
    }
}
