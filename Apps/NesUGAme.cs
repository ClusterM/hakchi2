#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class NesUGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(nes | famicom)";
            }
        }

        public NesUGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

