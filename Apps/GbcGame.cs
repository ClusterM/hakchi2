#pragma warning disable 0108

namespace com.clusterrr.hakchi_gui
{
    public class GbcGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(gameboy | game boy)";
            }
        }
        public GbcGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

