using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling
{
    public abstract class TaskableTool
    {
        public delegate void StringDelegate(String status);
        public delegate void IntDelegate(int pct);
        public event IntDelegate ReportProgress;
        public event StringDelegate ReportStatus;
        protected void Status(string theStatus)
        {
            if(ReportStatus != null)
            {
                ReportStatus(theStatus);
            }
        }
        protected void Progress(int theprogress)
        {
            if (ReportProgress != null)
            {
                ReportProgress(theprogress);
            }
        }
        public abstract void Execute();
    }
}
