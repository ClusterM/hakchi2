using System.Collections.Generic;

namespace com.clusterrr.hakchi_gui
{
    public static class CoreCommands
    {
        private static Dictionary<string, string> translationTable = new Dictionary<string, string>()
        {
            { "bsnes_mercury_performance", "bsnes" },
            { "dosbox_svn", "dosbox-svn" },
            { "emux_chip8", "ch8" },
            { "fbalpha", "fba2016" },
            { "fbalpha2012", "fba2012" },
            { "fbalpha2012_cps1", "cps1" },
            { "fbalpha2012_cps2", "cps2" },
            { "fbalpha2012_cps3", "cps3" },
            { "fbalpha2012_neogeo", "neo" },
            { "genesis_plus_gx", "genesis-plus-gx" },
            { "mame2003_plus", "mame2003-plus" },
            { "mednafen_ngp", "ngp" },
            { "mednafen_pce_fast", "pce" },
            { "mednafen_pcfx", "pcfx" },
            { "mednafen_saturn", "mednafen-saturn" },
            { "mednafen_supergrafx", "sgfx" },
            { "mednafen_vb", "vb" },
            { "mednafen_wswan", "ws" },
            { "msu", "snes9x" },
            { "pcsx_rearmed_neon", "pcsx" },
            { "snes9x2002", "snes02" },
            { "snes9x2005", "snes05" },
            { "snes9x2010", "snes10" },
            { "snes9x_bright", "snes9x-bright" },
            { "vba_next", "vba-next" },
            { "vice_x64", "c64" },
        };

        public static string GetCommand(string core)
        {
            return translationTable.ContainsKey(core) ? translationTable[core] : core;
        }
    }
}
