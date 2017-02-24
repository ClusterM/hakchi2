#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class Sega32XGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "sega 32x";
            }
        }

        public Sega32XGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

