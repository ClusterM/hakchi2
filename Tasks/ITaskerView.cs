namespace com.clusterrr.hakchi_gui.Tasks
{
    public interface ITaskerView
    {
        Tasker Tasker { get; set; }
        ITaskerView SetState(Tasker.State state);
        ITaskerView SetProgress(long value, long maximum);
        void OnProgress(long value, long maximum); // prototype matching progress callback handlers
        ITaskerView SetTitle(string title);
        ITaskerView SetStatusImage(System.Drawing.Image image);
        ITaskerView SetStatus(string status);
        ITaskerView Show();
        ITaskerView Close();
    }
}
