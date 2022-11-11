using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class TaskerForm : Form, ITaskerView
    {
        public Tasker Tasker
        {
            get; set;
        }

        public bool Blocking
        {
            get; private set;
        }

        public TaskerForm(bool blocking = true, Tasker tasker = null)
        {
            InitializeComponent();
            this.Tasker = tasker;
            this.Blocking = blocking;
        }

        public ITaskerView SetState(Tasker.State state)
        {
            if (Disposing || state == taskState) return this;
            try
            {
                if (InvokeRequired)
                {
                    return (ITaskerView)Invoke(new Func<Tasker.State, ITaskerView>(SetState), new object[] { state });
                }
                taskState = state;
            }
            catch (InvalidOperationException) { }
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
                progressBarEx1.Maximum = (int)maximum;
                progressBarEx1.Value = (int)value;
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
                statusPictureBox.Image = Shared.ResizeImage(image, null, null, 32, 32, false, true, true, true);
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
                throw new UnauthorizedAccessException("TaskerForm is already visible");
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
                throw new UnauthorizedAccessException("TaskerForm is already hidden");
            }
            this.isClosing = true;
            base.Close();
            return this;
        }

        private Tasker.State taskState = Tasker.State.Undefined;
        private bool isClosing = false;

        private void TaskerForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void TaskerForm_Shown(object sender, EventArgs e)
        {
            Tasker.Ready = true;
        }
    }
}
