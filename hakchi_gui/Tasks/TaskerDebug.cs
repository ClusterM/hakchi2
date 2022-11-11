using System.Diagnostics;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class TaskerDebug : ITaskerView
    {
        public Tasker Tasker
        {
            get; set;
        }

        public ITaskerView SetState(Tasker.State state)
        {
            Trace.WriteLine($"Set state {state.ToString()}");
            return this;
        }

        public ITaskerView SetProgress(long value, long maximum)
        {
            Trace.WriteLine($"Set progress {value} of {maximum}");
            return this;
        }

        public void OnProgress(long value, long maximum)
        {
            SetProgress(value, maximum);
        }

        public ITaskerView SetTitle(string title)
        {
            Trace.WriteLine($"Set title \"{title}\"");
            return this;
        }

        public ITaskerView SetStatusImage(System.Drawing.Image image)
        {
            Trace.WriteLine($"Set status image");
            return this;
        }

        public ITaskerView SetStatus(string status)
        {
            Trace.WriteLine($"Set status \"{status}\"");
            return this;
        }

        public ITaskerView Show()
        {
            Trace.WriteLine("Showing");
            return this;
        }

        public ITaskerView Close()
        {
            Trace.WriteLine("Hiding");
            return this;
        }

        public TaskerDebug()
        {
            Tasker = null;
        }

    }
}
