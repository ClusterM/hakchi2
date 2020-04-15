namespace com.clusterrr.hakchi_gui
{
    public interface INetworkShell
    {
        string IPAddress
        {
            get;
        }
        int Ping();
    }
}
