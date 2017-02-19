#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class GbGame : NesMiniApplication
    {
        public const char Prefix = 'B';
        public static Image DefaultCover { get { return Resources.blank_gb; } }
        public const string DefaultApp = "/bin/gb";

        public GbGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

