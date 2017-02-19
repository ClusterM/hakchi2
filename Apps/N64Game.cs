#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class N64Game : NesMiniApplication
    {
        public const char Prefix = '6';
        public static Image DefaultCover { get { return Resources.blank_n64; } }
        public const string DefaultApp = "/bin/n64";

        public N64Game(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

