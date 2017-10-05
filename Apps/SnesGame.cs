#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class SnesGame : NesMiniApplication
    {
        public override string GoogleSuffix
        {
            get
            {
                return "(snes | super nintendo)";
            }
        }

        public SnesGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }

        public static NesMiniApplication Import(string fileName, string sourceFile = null, byte[] rawRomData = null)
        {
            if (ConfigIni.ConsoleType != MainForm.ConsoleType.SNES && ConfigIni.ConsoleType != MainForm.ConsoleType.SuperFamicom)
                return null;

            return null;
        }
    }
}

