#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class GameGearGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "game gear";
            }
        }

        public GameGearGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

