namespace com.clusterrr.hakchi_gui.Tasks
{
    public interface ITaskerView
    {
        ITaskerView SetState(Tasker.State state);
        ITaskerView SetProgress(long value, long maximum);
        ITaskerView SetTitle(string title);
        ITaskerView SetStatusImage(System.Drawing.Image image);
        ITaskerView SetStatus(string status);
        ITaskerView Show();
        ITaskerView Close();
    }
}
