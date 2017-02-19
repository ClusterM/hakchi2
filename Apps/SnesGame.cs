#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class SnesGame : NesMiniApplication
    {
        public const char Prefix = 'U';
        public static Image DefaultCover { get { return Resources.blank_snes_us; } }
        internal const string DefaultApp = "/bin/snes";

        public SnesGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

