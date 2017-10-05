#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public class FdsGame : NesMiniApplication
    {
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet --fds-auto-disk-side-switch-on-keypress";
        const char Prefix = 'D';

        public override string GoogleSuffix
        {
            get
            {
                return "(fds | nes | famicom)";
            }
        }

        public FdsGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
        }

        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref uint crc32)
        {
            FindPatch(ref rawRomData, inputFileName, crc32);
            if (Encoding.ASCII.GetString(rawRomData, 0, 3) == "FDS") // header? cut it!
            {
                var fdsDataNoHeader = new byte[rawRomData.Length - 0x10];
                Array.Copy(rawRomData, 0x10, fdsDataNoHeader, 0, fdsDataNoHeader.Length);
                rawRomData = fdsDataNoHeader;
                crc32 = CRC32(rawRomData);
                // Try to find patch again, using new CRC
                FindPatch(ref rawRomData, inputFileName,  crc32);
            }
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.ConsoleType == MainForm.ConsoleType.Famicom)
                application = "/bin/clover-kachikachi-wr";
            else
                application = "/bin/nes";
            args = DefaultArgs;
            return true;
        }
    }
}

