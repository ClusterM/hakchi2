using com.clusterrr.util;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class TaskerTaskbar : ITaskerView
    {
        public Tasker Tasker
        {
            get; set;
        }

        public ITaskerView SetState(Tasker.State state)
        {
            if (state == taskState) return this;
            try
            {
                if (Tasker.HostForm.InvokeRequired)
                {
                    return (ITaskerView)Tasker.HostForm.Invoke(new Func<Tasker.State, ITaskerView>(SetState), new object[] { state });
                }
                switch (state)
                {
                    case Tasker.State.Starting:
                    case Tasker.State.Finishing:
                    case Tasker.State.Waiting:
                        if (!(new Tasker.State[] { Tasker.State.Starting, Tasker.State.Finishing, Tasker.State.Waiting }).Contains(taskState))
                        {
                            TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.NoProgress);
                            Thread.Sleep(20); // workaround to make it work
                            TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.Indeterminate);
                        }
                        break;
                    case Tasker.State.Running:
                        TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.Normal);
                        break;
                    case Tasker.State.Undefined:
                    case Tasker.State.Done:
                        TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.NoProgress);
                        break;
                    case Tasker.State.Paused:
                        TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.Paused);
                        break;
                    case Tasker.State.Error:
                        TaskbarProgress.SetState(Tasker.HostForm, TaskbarProgress.TaskbarStates.Error);
                        break;
                }
                taskState = state;
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public ITaskerView SetProgress(long value, long maximum)
        {
            if (Tasker.HostForm.Disposing || value < 0 || maximum < 0) return this;
            try
            {
                if (Tasker.HostForm.InvokeRequired)
                {
                    return (ITaskerView)Tasker.HostForm.Invoke(new Func<long, long, ITaskerView>(SetProgress), new object[] { value, maximum });
                }
                if ((new Tasker.State[] { Tasker.State.Running, Tasker.State.Paused, Tasker.State.Error }).Contains(taskState))
                {
                    TaskbarProgress.SetValue(Tasker.HostForm, value, maximum);
                }
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public void OnProgress(long value, long maximum)
        {
            SetProgress(value, maximum);
        }

        public ITaskerView SetTitle(string title)
        {
            return this;
        }

        public ITaskerView SetStatusImage(Image image)
        {
            return this;
        }

        public ITaskerView SetStatus(string status)
        {
            return this;
        }

        public ITaskerView Show()
        {
            return this;
        }

        public ITaskerView Close()
        {
            return SetState(Tasker.State.Undefined);
        }

        private Tasker.State taskState
        {
            get; set;
        }

        public TaskerTaskbar(Tasker tasker = null)
        {
            this.taskState = Tasker.State.Undefined;
            this.Tasker = tasker;
        }
    }
}
