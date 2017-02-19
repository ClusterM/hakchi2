#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class GbaGame : NesMiniApplication
    {
        public const char Prefix = 'A';
        public static Image DefaultCover { get { return Resources.blank_gba; } }
        public const string DefaultApp = "/bin/gba";

        public GbaGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}
