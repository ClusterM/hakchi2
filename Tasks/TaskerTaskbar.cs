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
        public ITaskerView SetState(Tasker.State state)
        {
            if (state == taskState) return this;
            try
            {
                if (tasker.HostForm.InvokeRequired)
                {
                    return (ITaskerView)tasker.HostForm.Invoke(new Func<Tasker.State, ITaskerView>(SetState), new object[] { state });
                }
                switch (state)
                {
                    case Tasker.State.Starting:
                    case Tasker.State.Finishing:
                    case Tasker.State.Waiting:
                        if (!(new Tasker.State[] { Tasker.State.Starting, Tasker.State.Finishing, Tasker.State.Waiting }).Contains(taskState))
                        {
                            TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.NoProgress);
                            Thread.Sleep(20); // workaround to make it work
                            TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.Indeterminate);
                        }
                        break;
                    case Tasker.State.Running:
                        TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.Normal);
                        break;
                    case Tasker.State.Undefined:
                    case Tasker.State.Done:
                        TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.NoProgress);
                        break;
                    case Tasker.State.Paused:
                        TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.Paused);
                        break;
                    case Tasker.State.Error:
                        TaskbarProgress.SetState(tasker.HostForm, TaskbarProgress.TaskbarStates.Error);
                        break;
                }
                taskState = state;
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public ITaskerView SetProgress(long value, long maximum)
        {
            if (tasker.HostForm.Disposing || value < 0 || maximum < 0) return this;
            try
            {
                if (tasker.HostForm.InvokeRequired)
                {
                    return (ITaskerView)tasker.HostForm.Invoke(new Func<long, long, ITaskerView>(SetProgress), new object[] { value, maximum });
                }
                if ((new Tasker.State[] { Tasker.State.Running, Tasker.State.Paused, Tasker.State.Error }).Contains(taskState))
                {
                    TaskbarProgress.SetValue(tasker.HostForm, value, maximum);
                }
            }
            catch (InvalidOperationException) { }
            return this;
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

        private Tasker tasker
        {
            get; set;
        }

        public TaskerTaskbar(Tasker tasker)
        {
            this.taskState = Tasker.State.Undefined;
            this.tasker = tasker;
        }
    }
}
