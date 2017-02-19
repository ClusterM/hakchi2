#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class SmsGame : NesMiniApplication
    {
        public const char Prefix = 'M';
        public static Image DefaultCover { get { return Resources.blank_sms; } }
        internal const string DefaultApp = "/bin/sms";

        public SmsGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

