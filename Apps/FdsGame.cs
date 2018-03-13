#pragma warning disable 0108
using System;
using System.Drawing;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class FdsGame : NesApplication
    {
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet --fds-auto-disk-side-switch-on-keypress";

        public FdsGame(string path, AppMetadata metadata = null, bool ignoreEmptyConfig = false)
            : base(path, metadata, ignoreEmptyConfig)
        {
        }

        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref byte saveCount, ref uint crc32)
        {
            FindPatch(ref rawRomData, inputFileName, crc32);
            if (Encoding.ASCII.GetString(rawRomData, 0, 3) == "FDS") // header? cut it!
            {
                var fdsDataNoHeader = new byte[rawRomData.Length - 0x10];
                Array.Copy(rawRomData, 0x10, fdsDataNoHeader, 0, fdsDataNoHeader.Length);
                rawRomData = fdsDataNoHeader;
                crc32 = Shared.CRC32(rawRomData);
                // Try to find patch again, using new CRC
                FindPatch(ref rawRomData, inputFileName,  crc32);
            }
            if (ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.NES || ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.Famicom)
            {
                application = "/bin/clover-kachikachi-wr";
                args = DefaultArgs;
            }
            return true;
        }
    }
}

