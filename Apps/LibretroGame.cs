using com.clusterrr.hakchi_gui.Properties;

namespace com.clusterrr.hakchi_gui
{
    public class LibretroGame : NesMiniApplication
    {
        public LibretroGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}
