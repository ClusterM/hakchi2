#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class SmsGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(sms | sega master system)";
            }
        }

        public SmsGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

