#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class PceGame : NesMiniApplication
    {
        public const char Prefix = 'E';
        public static Image DefaultCover { get { return Resources.blank_pce; } }
        internal const string DefaultApp = "/bin/pce";

        public PceGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

