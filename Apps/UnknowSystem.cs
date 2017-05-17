#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;

namespace com.clusterrr.hakchi_gui
{
    public class UnknowSystem : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(" + GetEmulator().SystemName + ")";
            }
        }
        
        public UnknowSystem(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}
