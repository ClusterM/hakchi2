using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class Tasker : IDisposable
    {
        public interface ITaskCollection
        {
            TaskFunc[] Tasks { get; }
        }

        public enum State { Undefined, Starting, Running, Waiting, Paused, Error, Finishing, Done }
        public enum Conclusion { Undefined, Error, Abort, Success }

        // task delegate

        public delegate Conclusion TaskFunc(Tasker tasker, Object syncObject = null);

        // task object

        public class Task
        {
            public string displayName;
            public int weight;
            public TaskFunc task;

            public Task(TaskFunc task, int weight = 1)
            {
                this.displayName = Regex.Match(task.Method.DeclaringType.Name, @"[^.]+$").Value + "." + task.Method.Name;
                this.weight = weight;
                this.task = task;
            }

            public Task()
            {
                this.displayName = string.Empty;
                this.weight = 1;
                this.task = null;
            }

            public static implicit operator TaskFunc(Task t) => t.task;
            public static implicit operator Task(TaskFunc t) => new Task(t);
        }

        // public properties

        public Form HostForm
        {
            get; private set;
        }

        public bool Ready
        {
            get; set;
        }

        public Object SyncObject
        {
            get; set;
        }

        public Task CurrentTask
        {
            get; private set;
        }

        public Conclusion TaskConclusion
        {
            get; set;
        }

        // helpers to manage states

        public State TaskState
        {
            get
            {
                return this.state.Any() ? this.state.Peek() : State.Undefined;
            }
            set
            {
                SetState(value);
            }
        }

        public Tasker PushState(State state)
        {
            this.state.Push(state);

            this.views.ForEach(view => view.SetState(state));
            return this;
        }

        public Tasker PopState()
        {
            State state = State.Undefined;
            if (this.state.Any())
            {
                state = this.state.Pop();
            }

            this.views.ForEach(view => view.SetState(state));
            return this;
        }

        public Tasker SetState(Tasker.State state)
        {
            if (this.state.Any())
            {
                if (this.state.Peek() == state) return this;
                this.state.Pop();
            }
            this.state.Push(state);

            this.views.ForEach(view => view.SetState(state));
            return this;
        }

        public Tasker SetProgress(long value, long maximum)
        {
            if (value == -1 || maximum == -1) return this;

            const int scale = 100;
            if (value > maximum) value = maximum;

            int currentWeight = CurrentTask == null ? 1 : CurrentTask.weight;
            int totalWeight = doneWeight + currentWeight + tasks.Sum(task => task.weight);
            double currentProgress = (double)value / maximum;
            value = (long)((doneWeight + (currentProgress * currentWeight)) * scale);
            maximum = totalWeight * scale;

            this.views.ForEach(view => view.SetProgress(value, maximum));
            return this;
        }

        public void OnProgress(long value, long maximum)
        {
            SetProgress(value, maximum);
        }

        public Tasker SetTitle(string title)
        {
            this.titleSet = true;
            this.views.ForEach(view => view.SetTitle(title));
            return this;
        }

        public Tasker SetStatusImage(Image image)
        {
            this.views.ForEach(view => view.SetStatusImage(image));
            return this;
        }

        public Tasker SetStatus(string status)
        {
            this.views.ForEach(view => view.SetStatus(status));
            return this;
        }

        public Tasker Show()
        {
            this.views.ForEach(view => view.Show());
            this.Ready = true;
            return this;
        }

        public Tasker Close()
        {
            this.views.ForEach(view => view.Close());
            return this;
        }

        // static helpers

        public static Conclusion QuickStart(Form hostForm, params Task[] tasks)
        {
            var tasker = new Tasker(hostForm);
            return tasker.AddTasks(tasks).Start();
        }

        public static Conclusion QuickStart(Form hostForm, params TaskFunc[] funcs)
        {
            var tasker = new Tasker(hostForm);
            return tasker.AddTasks(funcs).Start();
        }

        // message display helpers

        public void ShowError(Exception ex, string title = null, bool stop = false)
        {
            try
            {
                PushState(State.Error);
                ErrorForm.Show(HostForm, ex, title);
                PopState();
                if (stop)
                {
                    TaskConclusion = Conclusion.Error;
                    SetState(State.Undefined);
                    Abort();
                }
            }
            catch (InvalidOperationException) { }
        }

        public MessageForm.Button ShowMessage(string title, string message, Image icon = null, MessageForm.Button[] buttons = null, MessageForm.DefaultButton defaultButton = MessageForm.DefaultButton.Button1)
        {
            try
            {
                return MessageForm.Show(HostForm, title, message, icon, buttons, defaultButton);
            }
            catch (InvalidOperationException) { }
            return MessageForm.Button.Undefined;
        }

        // Task-related methods

        public Tasker AddTask(Task task)
        {
            tasks.Enqueue(task);
            return this;
        }

        public Tasker AddTask(TaskFunc func, int weight = 1)
        {
            tasks.Enqueue(new Task(func, weight));
            return this;
        }

        public Tasker AddTasks(params Task[] tasks)
        {
            foreach (Task task in tasks)
            {
                AddTask(task);
            }
            return this;
        }

        public Tasker AddTasks(params TaskFunc[] funcs)
        {
            foreach (TaskFunc func in funcs)
            {
                AddTask(func);
            }
            return this;
        }

        public Tasker AddTasks(IEnumerable<TaskFunc> funcs)
        {
            foreach (TaskFunc func in funcs)
            {
                AddTask(func);
            }
            return this;
        }

        public Tasker AddTasks(params ITaskCollection[] collection)
        {
            foreach (var task in collection)
                AddTasks(task.Tasks);

            return this;
        }

        public Tasker AddFinalTask(Task task)
        {
            if (this.finalTask != null)
            {
                throw new ArgumentException("Final task already defined");
            }
            this.finalTask = task;
            return this;
        }

        public Tasker AddFinalTask(TaskFunc func)
        {
            return AddFinalTask(new Task(func));
        }

        public Tasker CancelRemainingTasks()
        {
            this.tasks.Clear();
            return this;
        }

        public void Abort()
        {
            if (thread != null)
            {
                #warning Refactor this to get rid of Thread.Abort!
                thread.Abort();
            }
        }

        // Views-related methods

        public Tasker AttachView(ITaskerView view)
        {
            if (view.Tasker == null) view.Tasker = this;
            this.views.Add(view);
            return this;
        }

        public Tasker AttachViews(params ITaskerView[] views)
        {
            foreach (var view in views)
            {
                AttachView(view);
            }
            return this;
        }

        public IEnumerable<T> GetSpecificViews<T>()
        {
            return views.OfType<T>();
        }

        // run

        public Conclusion Start()
        {
            this.Ready = false;
            this.CurrentTask = null;
            this.TaskConclusion = Conclusion.Undefined;
            this.doneTasks = 0;
            this.doneWeight = 0;

            // set up thread
            thread = new Thread(startThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Show();
            return TaskConclusion;
        }

        // create

        public Tasker(Form hostForm, params ITaskerView[] views)
            : this(hostForm)
        {
            AttachViews(views);
        }

        public Tasker(Form hostForm)
        {
            // init public properties
            this.HostForm = hostForm;
            this.Ready = false;
            this.SyncObject = new Object();
            this.CurrentTask = null;
            this.TaskConclusion = Conclusion.Undefined;

            // init internals
            this.views = new List<ITaskerView>();
            this.state = new Stack<State>();
            this.tasks = new Queue<Task>();
            this.finalTask = null;
            this.thread = null;
            this.titleSet = false;
            this.doneTasks = 0;
            this.doneWeight = 0;
        }

        // inner workings

        private List<ITaskerView> views;
        private Stack<State> state;
        private Queue<Task> tasks;
        private Task finalTask;
        private Thread thread;
        private bool titleSet;
        private int doneTasks;
        private int doneWeight;

        private void startThread()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Instance.Language);
            while (!Ready) { Thread.Sleep(1); Trace.Write("."); }
            SetStatus(Resources.Starting);
            TaskState = State.Starting;
            SetProgress(0, 1);
            try
            {
                // iterate through tasks queue
                bool firstTask = true;
                while (tasks.Any())
                {
                    // pop out next task
                    CurrentTask = tasks.Dequeue();
                    Trace.WriteLine("Executing task: " + CurrentTask.displayName);

                    // set title if not already set
                    if (firstTask && !titleSet)
                    {
                        SetTitle(CurrentTask.displayName);
                    }
                    firstTask = false;

                    // run task and assign conclusion
                    Conclusion conclusion = CurrentTask.task(this, SyncObject);
                    if (conclusion == Conclusion.Error || conclusion == Conclusion.Abort)
                    {
                        TaskConclusion = conclusion;
                        break;
                    }
                    SetProgress(1, 1);

                    // increase counters
                    doneWeight += CurrentTask.weight;
                    ++doneTasks;
                }
            }
            catch (ThreadAbortException)
            {
                Trace.WriteLine("Thread aborted");
                if (TaskConclusion == Conclusion.Undefined)
                {
                    TaskConclusion = Conclusion.Abort;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    ShowError(ex.InnerException);
                }
                else
                {
                    ShowError(ex);
                }
                TaskConclusion = Conclusion.Error;
            }
            finally
            {
                CurrentTask = null;
                if (finalTask != null)
                {
                    Trace.WriteLine("Executing final task: " + finalTask.displayName);
                    finalTask.task(this, SyncObject);
                    finalTask = null;
                }
            }

            // done
            if (TaskConclusion == Conclusion.Undefined || TaskConclusion == Conclusion.Success)
            {
                TaskConclusion = Conclusion.Success;
                SetState(State.Done).SetStatus(Resources.Done);
            }

            Trace.WriteLine($"Tasker completed all tasks, conclusion: {TaskConclusion.ToString()}");
            Close();
            thread = null;
        }

        public void Dispose()
        {
            this.views.ForEach(view => { if (view is IDisposable) (view as IDisposable).Dispose(); });
            GC.Collect();
        }

        // generic tasks

        public static Conclusion Stub(Tasker tasker, Object syncObject)
        {
            return Conclusion.Success;
        }

        public static TaskFunc Wait(int milliseconds, string message)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                const int step = 100;

                tasker.SetStatus(message);
                int steps = milliseconds / step;
                for (int i = 0; i < steps; ++i)
                {
                    tasker.SetProgress(i, steps);
                    Thread.Sleep(step);
                }
                return Conclusion.Success;
            };
        }

        public static TaskFunc TaskTitle(string title) => (Tasker tasker, Object syncObject) => 
        {
            tasker.SetTitle(title);
            return Conclusion.Success;
        };
        public static TaskFunc TaskIf(TaskFunc condition, TaskFunc successTask, TaskFunc failureTask = null)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                TaskFunc task = condition(tasker, syncObject) == Conclusion.Success ? successTask : (failureTask ?? Stub);
                Trace.WriteLine($"Forwarding task: {Regex.Match(task.Method.DeclaringType.Name, @"[^.]+$").Value + "." + task.Method.Name}");
                return task(tasker, syncObject);
            };
        }

        public static TaskFunc TaskIf(Func<bool> condition, TaskFunc trueTask, TaskFunc falseTask = null)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                TaskFunc task = (condition()) ? trueTask : (falseTask ?? Stub);
                Trace.WriteLine($"Forwarding task: {Regex.Match(task.Method.DeclaringType.Name, @"[^.]+$").Value + "." + task.Method.Name}");
                return task(tasker, syncObject);
            };
        }


        public static TaskFunc ErrorTask(string message, string innerMessage = null)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                if (innerMessage != null)
                    throw new Exception(message, new Exception(innerMessage));

                throw new Exception(message);
            };
        }

        public static Conclusion SuccessTask(Tasker tasker, object syncObject) => Conclusion.Success;

    }

    public static class TaskerExtensions
    {
        public static Tasker SetProgress(this Tasker tasker, long value, long maximum, Tasker.State state, string status)
        {
            tasker.SetProgress(value, maximum);
            tasker.SetState(state);
            tasker.SetStatus(status);
            return tasker;
        }

        public static Tasker SetStatus(this Tasker tasker, Tasker.State state, string status)
        {
            tasker.SetState(state);
            tasker.SetStatus(status);
            return tasker;
        }
    }
}
