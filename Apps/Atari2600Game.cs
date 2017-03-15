#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class Atari2600Game : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "atari 2600";
            }
        }

        public Atari2600Game(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

