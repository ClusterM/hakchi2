using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class TaskerForm : Form
    {
        public enum State { Undefined, Starting, Running, Waiting, Paused, Error, Finishing, Done }
        public enum Conclusion { Undefined, Error, Abort, Success }
        public delegate Conclusion TaskFunc(TaskerForm tasker, Object syncObject = null);

        public Form HostForm
        {
            get; private set;
        }

        public Object SyncObject
        {
            get; set;
        }

        public Conclusion TaskConclusion
        {
            get; set;
        }

        // helper combined setters

        public void SetProgress(int value, int maxValue, State state, string status)
        {
            SetProgress(value, maxValue);
            SetState(state);
            SetStatus(status);
        }

        public void SetStatus(State state, string status)
        {
            SetState(state);
            SetStatus(status);
        }

        private State taskState = State.Undefined;
        public State SetState(State state)
        {
            if (Disposing) return State.Undefined;
            if (state == taskState) return state;
            try
            {
                if (InvokeRequired)
                {
                    return (State)Invoke(new Func<State, State>(SetState), new object[] { state });
                }
                State previousState = taskState;
                taskState = state;
                if (showTaskbarProgress)
                {
                    switch (state)
                    {
                        case State.Starting:
                        case State.Finishing:
                        case State.Waiting:
                            if (previousState != State.Starting && previousState != State.Finishing && previousState != State.Waiting)
                            {
                                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.NoProgress); Thread.Sleep(20);
                                TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Indeterminate); // workaround to make it work
                            }
                            break;
                        case State.Running:
                            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Normal);
                            break;
                        case State.Undefined:
                        case State.Done:
                            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.NoProgress);
                            break;
                        case State.Paused:
                            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Paused);
                            break;
                        case State.Error:
                            TaskbarProgress.SetState(this, TaskbarProgress.TaskbarStates.Error);
                            break;
                    }
                }
                return previousState;
            }
            catch (InvalidOperationException) { Debug.WriteLine("InvalidOperationException"); }
            return State.Undefined;
        }

        public void SetProgress(int value, int maxValue)
        {
            if (value < 0 || maxValue < 0) return;
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { SetProgress(value, maxValue); }));
                    return;
                }
                // adjust values
                if (value > maxValue) value = maxValue;
                value = (doneTasks * maxValue) + value;
                maxValue = maxValue * (doneTasks + tasks.Count + 1);

                // scale it up to prevent discrepancies
                progressBarEx1.Maximum = 512;
                progressBarEx1.Value = (int)((double)value / maxValue * 512);

                // if long task, also show task bar progress
                if (showTaskbarProgress && (taskState == State.Running || taskState == State.Paused || taskState == State.Error))
                {
                    TaskbarProgress.SetValue(this, progressBarEx1.Value, progressBarEx1.Maximum);
                }
            }
            catch (InvalidOperationException) { }
        }

        public void SetTitle(string title)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(SetTitle), new object[] { title });
                    return;
                }
                this.Text = title;
            }
            catch (InvalidOperationException) { }
        }

        public void SetStatusImage(Image image)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Image>(SetStatusImage), new object[] { image });
                    return;
                }
                statusPictureBox.Image = Shared.ResizeImage(image, null, null, 32, 32, false, true, true, true);
            }
            catch (InvalidOperationException) { }
        }

        public void SetStatus(string status)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(SetStatus), new object[] { status });
                    return;
                }
                statusLabel.Text = status;
            }
            catch (InvalidOperationException) { }
        }

        public void ShowError(Exception ex, bool stop = false)
        {
            if (Disposing) return;
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<Exception, bool>(ShowError), new object[] { ex, stop });
                    return;
                }
                string formattedText = ex.GetType().ToString() + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                Debug.WriteLine(formattedText);

                State prevState = SetState(State.Error);
                ErrorForm.ShowDialog(null, ex.Message, formattedText);
                if (stop)
                {
                    TaskConclusion = Conclusion.Error;
                    SetState(State.Undefined);
                    thread.Abort();
                }
                else
                    SetState(prevState);
            }
            catch (InvalidOperationException) { }
        }

        public DialogResult ShowMessage(string message, string caption, MessageBoxButtons mbButtons = MessageBoxButtons.OK, MessageBoxIcon mbIcon = MessageBoxIcon.Information, MessageBoxDefaultButton mbDefaultButtons = MessageBoxDefaultButton.Button1)
        {
            if (Disposing) return DialogResult.None;
            try
            {
                if (InvokeRequired)
                {
                    return (DialogResult)Invoke(new Func<string, string, MessageBoxButtons, MessageBoxIcon, MessageBoxDefaultButton, DialogResult>(ShowMessage), new object[] { message, caption, mbButtons, mbIcon, mbDefaultButtons });
                }
                return MessageBox.Show(HostForm, message, caption, mbButtons, mbIcon, mbDefaultButtons);
            }
            catch (InvalidOperationException) { }
            return DialogResult.None;
        }

        public Conclusion Start(bool showTaskbarProgress = false, int endDelay = 0)
        {
            TaskConclusion = Conclusion.Undefined;
            this.showTaskbarProgress = showTaskbarProgress;
            this.endDelay = endDelay;
            this.doneTasks = 0;
            this.closing = false;
            
            // set up thread
            thread = new Thread(startThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            ShowDialog();

            // clean up
            thread = null;
            GC.Collect();
            return TaskConclusion;
        }

        public TaskerForm AddTask(TaskFunc task)
        {
            tasks.Enqueue(task);
            return this;
        }

        public TaskerForm AddTasks(params TaskFunc[] tasks)
        {
            foreach (TaskFunc task in tasks)
            {
                AddTask(task);
            }
            return this;
        }

        public TaskerForm CancelRemainingTasks()
        {
            tasks.Clear();
            return this;
        }

        public TaskerForm(Form f)
        {
            InitializeComponent();
            this.HostForm = f;
            this.SyncObject = new Object();
            this.tasks = new Queue<TaskFunc>();
            this.thread = null;
        }

        private Queue<TaskFunc> tasks;
        private Thread thread;
        private bool showTaskbarProgress;
        private int endDelay;
        private int doneTasks;
        private bool closing;

        private void startThread()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Instance.Language);
            SetProgress(0, 1);

            try
            {
                // iterate through tasks queue
                while (tasks.Any())
                {
                    // pop out next task
                    TaskFunc currentTask = tasks.Dequeue();

                    // compute readable name
                    string
                        currentTaskClass = currentTask.Method.DeclaringType.ToString();
                        currentTaskClass = currentTaskClass.Substring(currentTaskClass.LastIndexOf('.') + 1);
                    string currentTaskName = currentTaskClass + "." + currentTask.Method.Name;

                    // give basic title
                    Debug.WriteLine("Executing task: " + currentTaskName);
                    SetTitle(currentTaskName);

                    // run task and assign conclusion
                    Conclusion conclusion = currentTask(this, SyncObject);
                    if (conclusion == Conclusion.Error || conclusion == Conclusion.Abort)
                    {
                        TaskConclusion = conclusion;
                        break;
                    }
                    SetProgress(1, 1);
                    ++doneTasks;
                    Thread.Sleep(100); // artificial delay for testing TODO remove
                }

                // done
                Debug.WriteLine(TaskConclusion.ToString());
                if (TaskConclusion == Conclusion.Success)
                {
                    SetStatus(State.Done, "Done!");
                    if (endDelay > 0)
                        Thread.Sleep(endDelay);
                }
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("Thread aborted");
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
            }
            finally
            {
                if (TaskConclusion == Conclusion.Undefined) TaskConclusion = Conclusion.Success;
                SetState(State.Undefined);
            }

            this.closing = true;
            Invoke(new Action(Close));
        }

        private void Tasker_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Starting...";
        }

        private void Tasker_Shown(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void TaskerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread != null && !closing && e.CloseReason == CloseReason.UserClosing)
            {
                // make more checks to bypass question when appropriate
                if(ShowMessage(Resources.DoYouWantCancel, Resources.AreYouSure,MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    TaskConclusion = Conclusion.Abort;
                    thread.Abort();
                    return;
                }
                e.Cancel = true;
            }
        }

    }
}
