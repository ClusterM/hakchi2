#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class GbcGame : NesMiniApplication
    {
        public const char Prefix = 'C';
        public static Image DefaultCover { get { return Resources.blank_gbc; } }
        public const string DefaultApp = "/bin/gbc";

        public GbcGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

