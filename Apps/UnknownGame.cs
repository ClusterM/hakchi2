using com.clusterrr.hakchi_gui.Properties;

namespace com.clusterrr.hakchi_gui
{
    public class UnknownGame : NesApplication
    {
        public UnknownGame(string path, AppMetadata metadata = null, bool ignoreEmptyConfig = false)
            : base(path, metadata, ignoreEmptyConfig)
        {
        }
    }
}
