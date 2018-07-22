using System.Collections.Generic;

namespace com.clusterrr.hakchi_gui
{
    public static class CoreCommands
    {
        private static Dictionary<string, string> translationTable = new Dictionary<string, string>()
        {
            { "vice_x64", "c64" },
            { "emux_chip8", "ch8" },
            { "fbalpha2012", "fba2012" },
            { "fbalpha2012_cps1", "cps1" },
            { "fbalpha2012_cps2", "cps2" },
            { "fbalpha2012_cps3", "cps3" },
            { "fbalpha2012_neogeo", "neo" },
            { "fb_alpha", "fba2016" },
            { "vba_next", "vba-next" },
            { "mednafen_ngp", "ngp" },
            { "mednafen_supergrafx", "sgfx" },
            { "mednafen_pce_fast", "pce" },
            { "mednafen_pcfx", "pcfx" },
            { "pcsx_rearmed_neon", "pcsx" },
            { "genesis_plus_gx", "genesis-plus-gx" },
            { "snes9x2002", "snes02" },
            { "snes9x2005", "snes05" },
            { "snes9x2010", "snes10" },
            { "bsnes_mercury_performance", "bsnes" },
            { "mednafen_vb", "vb" },
            { "mednafen_wswan", "ws" },
            { "msu", "snes9x" },
        };

        public static string GetCommand(string core)
        {
            return translationTable.ContainsKey(core) ? translationTable[core] : core;
        }
    }
}
