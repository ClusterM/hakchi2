using com.clusterrr.hakchi_gui.Properties;
using System.Collections.Generic;

namespace com.clusterrr.hakchi_gui
{
    public class LibretroGame : NesApplication
    {
        public LibretroGame(string path, AppMetadata metadata = null, bool ignoreEmptyConfig = false)
            : base(path, metadata, ignoreEmptyConfig)
        {
        }
    }
}
