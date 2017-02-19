#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class NesUGame : NesMiniApplication
    {
        public const char Prefix = 'U';
        public static Image DefaultCover { get { return Resources.blank_jp; } }
        public const string DefaultApp = "/bin/nes";

        public NesUGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

