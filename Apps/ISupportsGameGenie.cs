namespace com.clusterrr.hakchi_gui
{
    interface ISupportsGameGenie
    {
        bool ApplyGameGenie(out byte[] gameFileData);
        void ApplyGameGenie();
    }
}
