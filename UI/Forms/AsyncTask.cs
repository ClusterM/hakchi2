using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace com.clusterrr.hakchi_gui.UI.Forms
{
    public partial class AsyncTask : Form
    {
        Tooling.TaskableTool taskToPerform;
        Thread thread;
        public AsyncTask(Tooling.TaskableTool tt)
        {
            InitializeComponent();
            taskToPerform = tt;
            this.Text = tt.Name;
            progressBar1.Value = 0;
            label1.Text = "";
            tt.Completed += Tt_Completed;
            tt.Progress += Tt_Progress;
            tt.Status += Tt_Status;
            tt.Error += Tt_Error;
            
            
        }
        
        private void Tt_Error(string message, bool fatal)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Tooling.TaskableTool.StringBoolDelegate(Tt_Error), new object[] { message,fatal });
            }
            else
            {
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if(fatal)
                {
                    this.Close();
                }
            }
        }

        private void Tt_Status(string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Tooling.TaskableTool.StringDelegate(Tt_Status), new object[] { status });
            }
            else
            {
                label1.Text = status;
            }
        }

        private void Tt_Progress(int pct)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Tooling.TaskableTool.IntDelegate(Tt_Progress),new object[] { pct });
            }
            else
            {
                progressBar1.Value = pct;
            }
        }

        private void Tt_Completed()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Tooling.TaskableTool.VoidDelegate(Tt_Completed));
            }
            else
            {
                this.Close();
            }
        }

        private void AsyncTask_Shown(object sender, EventArgs e)
        {
            thread = new Thread(taskToPerform.Execute);
            thread.Start();
        }
    }
}
