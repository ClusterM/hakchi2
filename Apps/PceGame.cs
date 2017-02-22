#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class PceGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(pce | pc engine | turbografx 16)";
            }
        }

        public PceGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

