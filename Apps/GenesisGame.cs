#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class GenesisGame : NesMiniApplication
    {
        public const char Prefix = 'G';
        public static Image DefaultCover { get { return Resources.blank_genesis; } }
        public const string DefaultApp = "/bin/md";

        public GenesisGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

