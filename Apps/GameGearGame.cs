#pragma warning disable 0108

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

