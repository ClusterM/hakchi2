using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling
{
    public abstract class TaskableTool
    {
  
        public TaskableTool(string theName)
        {
            Name = theName;
        }
        public string Name { get; set; }
        public delegate void StringDelegate(String status);
        public delegate void IntDelegate(int pct);
        public delegate void VoidDelegate();
        public delegate void StringBoolDelegate(string message, bool fatal);
        public event IntDelegate Progress;
        public event StringDelegate Status;
        public event VoidDelegate Completed;
        public event StringBoolDelegate Error;

        protected void ReportError(string message, bool fatal)
        {
            if(Error!=null)
            {
                Error(message, fatal);
            }
        }
        protected void ReportCompleted()
        {
            if(Completed != null)
            {
                Completed();
            }
        }

        protected void ReportStatus(string theStatus)
        {
            if(Status != null)
            {
                Status(theStatus);
            }
        }
        protected void ReportProgress(int theprogress)
        {
            if (Progress != null)
            {
                Progress(theprogress);
            }
        }
        public abstract void Execute();
    }
}
