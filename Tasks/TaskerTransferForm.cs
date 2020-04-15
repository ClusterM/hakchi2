using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class TaskerTransferForm : Form, ITaskerView
    {
        private const int SpeedSampleFreq = 500;
        private const int SpeedSampleNum = 5;

        public Tasker Tasker
        {
            get; set;
        }

        public bool Blocking
        {
            get; private set;
        }

        public TaskerTransferForm(bool blocking = true, Tasker tasker = null)
        {
            InitializeComponent();
            this.Tasker = tasker;
            this.Blocking = blocking;
        }

        public ITaskerView SetState(Tasker.State state)
        {
            return this;
        }

        public ITaskerView SetProgress(long value, long maximum)
        {
            if (Disposing || value < 0 || maximum < 0) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (ITaskerView)Invoke(new Func<long, long, ITaskerView>(SetProgress), new object[] { value, maximum });
                }
                if (value > maximum) value = maximum;
                progressBarEx1.Maximum = 100;
                progressBarEx1.Value = (int)((value / (double)maximum) * 100);
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public void OnProgress(long value, long maximum)
        {
            SetProgress(value, maximum);
        }

        private DateTime startTime = default(DateTime);
        private DateTime lastTime = default(DateTime);
        private long lastData = 0;
        private Queue<double> speedSamples;
        public TaskerTransferForm SetAdvancedProgress(long value, long maximum, string fileName)
        {
            // call parent tasker SetProgress to update other views (and this one's progress bar)
            Tasker.SetProgress(value, maximum);

            if (Disposing || value < 0 || maximum < 0) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (TaskerTransferForm)Invoke(new Func<long, long, string, TaskerTransferForm>(SetAdvancedProgress), new object[] { value, maximum, fileName });
                }

                // update other elements

                if (value >= maximum)
                {
                    this.labelFileName.Text = "";
                    this.labelTimeLeft.Text = Resources.Done;
                    this.labelTransferRate.Text = "";
                    return this;
                }
                this.labelFileName.Text = System.IO.Path.GetFileName(fileName);

                DateTime now = DateTime.Now;
                if (lastTime == default(DateTime))
                {
                    startTime = lastTime = now;
                    speedSamples = new Queue<double>();
                    return this;
                }
                else
                {
                    TimeSpan elapsed = now.Subtract(lastTime);
                    if (elapsed.TotalMilliseconds >= SpeedSampleFreq)
                    {
                        speedSamples.Enqueue(((value - lastData) / elapsed.TotalSeconds));
                        if (speedSamples.Count > SpeedSampleNum)
                            speedSamples.Dequeue();
                        lastTime = now;
                        lastData = value;
                    }
                }

                if (speedSamples.Any())
                {
                    double sampledSpeed = speedSamples.Average();
                    double timeLeft = (maximum - value) / (value / now.Subtract(startTime).TotalSeconds);
                    this.labelTimeLeft.Text = new TimeSpan(0, 0, (int)timeLeft).ToReadableString() + " (" + Shared.SizeSuffix(value) + " of " + Shared.SizeSuffix(maximum) + " copied)";
                    this.labelTransferRate.Text = Shared.SizeSuffix((long)sampledSpeed) + "/Sec";
                }
                else
                {
                    this.labelTimeLeft.Text = "(" + Shared.SizeSuffix(value) + " of " + Shared.SizeSuffix(maximum) + " copied)";
                    this.labelTransferRate.Text = "Calculating...";
                }
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public ITaskerView SetTitle(string title)
        {
            if (Disposing) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (ITaskerView)Invoke(new Func<string, ITaskerView>(SetTitle), new object[] { title });
                }
                this.Text = title;
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public ITaskerView SetStatusImage(Image image)
        {
            if (Disposing) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (ITaskerView)Invoke(new Func<Image, ITaskerView>(SetStatusImage), new object[] { image });
                }
                statusPictureBox.Image = Shared.ResizeImage(image, null, null, 48, 48, false, true, true, true);
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public ITaskerView SetStatus(string status)
        {
            if (Disposing) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (ITaskerView)Invoke(new Func<string, ITaskerView>(SetStatus), new object[] { status });
                }
                statusLabel.Text = status;
            }
            catch (InvalidOperationException) { }
            return this;
        }

        public new ITaskerView Show()
        {
            if (Disposing) return this;
            if (InvokeRequired)
            {
                return (ITaskerView)Invoke(new Func<ITaskerView>(Show));
            }
            if (Visible)
            {
                throw new UnauthorizedAccessException("TaskerTransferForm is already visible");
            }
            if (Blocking)
            {
                base.ShowDialog();
            }
            else
            {
                base.Show();
            }
            return this;
        }

        public new ITaskerView Close()
        {
            if (Disposing) return this;
            if (InvokeRequired)
            {
                return (ITaskerView)Invoke(new Func<ITaskerView>(Close));
            }
            if (!Visible)
            {
                throw new UnauthorizedAccessException("TaskerTransferForm is already hidden");
            }
            this.isClosing = true;
            base.Close();
            return this;
        }

        private bool isClosing = false;

        private void TaskerTransferForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = Resources.Starting;
            labelFileName.Text = "";
            labelTimeLeft.Text = "";
            labelTransferRate.Text = "";
        }

        private void TaskerTransferForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosing && e.CloseReason == CloseReason.UserClosing)
            {
                // make more checks to bypass question when appropriate
                if (Tasker.ShowMessage(Resources.AreYouSure, Resources.DoYouWantCancel, Resources.sign_warning, new MessageForm.Button[] { MessageForm.Button.Yes, MessageForm.Button.No }, MessageForm.DefaultButton.Button2) == MessageForm.Button.Yes)
                {
                    Tasker.Abort();
                    return;
                }
                e.Cancel = true;
            }
        }

        private void TaskerTransferForm_Shown(object sender, EventArgs e)
        {
            Tasker.Ready = true;
        }
    }
}
