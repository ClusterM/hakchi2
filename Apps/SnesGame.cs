#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class SnesGame : NesApplication, ICloverAutofill /*, ISupportsGameGenie*/
    {
        public enum SnesRomType { LoRom = 0x14, HiRom = 0x15 };

        const string DefaultCanoeArgs = "--volume 100 -rollback-snapshot-period 600";
        static List<byte> SfxTypes = new List<byte>() { 0x13, 0x14, 0x15, 0x1a };
        static List<byte> Dsp1Types = new List<byte>() { 0x03, 0x05 };
        static List<byte> SA1Types = new List<byte>() { 0x34, 0x35 };
        // Known presets
        static Dictionary<string, ushort> knownPresets = new Dictionary<string, ushort>()
        {
            { "SUPER MARIOWORLD", 0x1011 },
            { "F-ZERO", 0x1018 },
            //{ "THE LEGEND OF ZELDA", 0x101D }, // Removed to use hacks and translations
            { "SUPER MARIO KART", 0x10BD },
            { "Super Metroid", 0x1040 },
            { "EARTH BOUND", 0x1070 },
            { "Kirby's Dream Course", 0x1058 },
            { "DONKEY KONG COUNTRY", 0x1077 },
            { "KIRBY SUPER DELUXE", 0x109F },
            { "Super Punch-Out!!", 0x10A9 },
            { "MEGAMAN X", 0x1109 },
            { "SUPER GHOULS'N GHOST", 0x1003 },
            { "Street Fighter2 Turb", 0x1065 },
            { "SUPER MARIO RPG", 0x109E },
            //{ "Secret of MANA", 0x10B0 },// Removed to use hacks and translations
            { "FINAL FANTASY 3", 0x10DC },
            { "SUPER CASTLEVANIA 4", 0x1030 },
            { "CONTRA3 THE ALIEN WA", 0x1036 },
            { "STAR FOX", 0x1242 },
            //{ "YOSHI'S ISLAND", 0x123D }, // Removed to use hacks and translations
            { "STARFOX2", 0x123C },
            //{ "ZELDANODENSETSU", 0x101F }, // Removed to use hacks and translations
            { "SHVC FIREEMBLEM", 0x102B },
            { "SUPER DONKEY KONG", 0x1023 },
            //{ "Super Street Fighter", 0x1056 }, // Invalid
            { "ROCKMAN X", 0x110A },
            { "CHOHMAKAIMURA", 0x1004 },
            { "SeikenDensetsu 2", 0x10B2 },
            { "FINAL FANTASY 6", 0x10DD },
            { "CONTRA SPIRITS", 0x1037 },
            { "ganbare goemon", 0x1048 },
            { "SUPER FORMATION SOCC", 0x1240 },
            { "YOSSY'S ISLAND", 0x1243 },
            { "FINAL FIGHT", 0x100E },
            { "DIDDY'S KONG QUEST", 0x105D },
            //{ "KIRBY'S DREAM LAND 3", 0x10A2 }, // Reported as problematic, using ID from Mario RPG
            { "BREATH OF FIRE 2", 0x1068 },
            { "FINAL FIGHT 2", 0x10E1 },
            { "MEGAMAN X2", 0x1117 },
            { "FINAL FIGHT 3", 0x10E3 },
            { "GENGHIS KHAN 2", 0x10C4 },
            { "CASTLEVANIA DRACULA", 0x1131 },
            { "STREET FIGHTER ALPHA", 0x10DF },
            { "MEGAMAN 7", 0x113A },
            { "MEGAMAN X3", 0x113D },
            { "Breath of Fire", 0x1144 },
        };
        // Known LoRom games
        static List<string> gamesLoRom = new List<string>()
        {
        };
        // Known HiRom games
        static List<string> gamesHiRom = new List<string>()
        {
        };
        static List<string> problemGames = new List<string>()
        {
            { "ActRaiser-2 USA" }, // ActRaiser 2 (U) [!].smc
            { "ALIEN vs. PREDATOR" }, // Alien vs. Predator (U).smc
            { "ASTERIX" }, // Asterix (E) [!].smc
            { "BATMAN--REVENGE JOKER" }, // Batman - Revenge of the Joker (U).smc
            { "???????S???????????" }, // Bishoujo Senshi Sailor Moon S - Jougai Rantou! Shuyaku Soudatsusen (J).smc
            { "CHAMPIONSHIP POOL" }, // Championship Pool (U).smc
            { "ClayFighter 2" }, // Clay Fighter 2 - Judgment Clay (U) [!].smc
            { "CLOCK TOWER SFX" }, // Clock Tower (J).smc
            { "COOL WORLD" }, // Cool World (U) [!].smc
            { "CRYSTAL BEANS" }, // Crystal Beans From Dungeon Explorer (J).smc
            { "CYBER KNIGHT 2" }, // Cyber Knight II - Chikyuu Teikoku no Yabou (J).smc
            { "ASCII DARK LAW" }, // Dark Law - Meaning of Death (J).smc
            { "DIRT TRAX FX" }, // Dirt Trax FX (U) [!].smc
            { "DBZ HYPER DIMENSION" }, // Dragon Ball Z - Hyper Dimension (F).smc
            { "DRAGON BALL Z HD" }, // Dragon Ball Z - Hyper Dimension (J) [!].smc
            { "DRAGONBALL Z 2" }, // Dragon Ball Z - La Legende Saien (F).smc
            { "SFX DRAGONBALLZ2" }, // Dragon Ball Z - Super Butouden (F).smc
            { "SFX SUPERBUTOUDEN2" }, // Dragon Ball Z - Super Butouden 2 (J) (V1.0).smc
            { "DUNGEON MASTER" }, // Dungeon Master (U).smc
            { "EARTHWORM JIM 2" }, // Earthworm Jim 2 (U) [!].smc
            { "F1 WORLD CHAMP EDTION" }, // F1 World Championship Edition (E).smc
            { "FACEBALL 2000" }, // Faceball 2000 (U) [!].smc
            { "THE FIREMEN     PAL" }, // Firemen, The (E).smc
            { "HAMMERIN' HARRY (JPN)" }, // Ganbare Daiku no Gensan (J) [!].smc
            { "HARMELUNNOBAIOLINHIKI" }, // Hamelin no Violin Hiki (J).smc
            { "HOME ALONE" }, // Home Alone (U).smc
            { "HUMAN GRANDPRIX" }, // Human Grand Prix (J).smc
            { "HUMAN GRANDPRIX 3" }, // Human Grand Prix III - F1 Triple Battle (J).smc
            { "ILLUSION OF GAIA USA" }, // Illusion of Gaia (U) [!].smc
            { "ILLUSION OF TIME ENG" }, // Illusion of Time (E) [!].smc
            { "JumpinDerby" }, // Jumpin' Derby (J).smc
            { "KRUSTYS SUPERFUNHOUSE" }, // Krusty's Super Fun House (U) (V1.1).smc
            { "Mario's Time Machine" }, // Mario's Time Machine (U) [!].smc
            { "MARKOS MAGIC FOOTBALL" }, // Marko's Magic Football (E).smc
            { "POWER RANGERS FIGHT" }, // Mighty Morphin Power Rangers - The Fighting Edition (U).smc
            { "MOMOTETSU HAPPY" }, // Momotarou Dentetsu Happy (J) [!].smc
            { "NHL HOCKEY 1998" }, // NHL '98 (U).smc
            { "RENDERING RANGER R2" }, // Rendering Ranger R2 (J).smc
            { "ROBOTREK 1 USA" }, // Robotrek (U) [!].smc
            { "ROCK N' ROLL RACING" }, // Rock N' Roll Racing (U) [!].smc
            { "ROMANCING SAGA3" }, // Romancing SaGa 3 (J) (V1.1).smc
            { "SD??????GX" }, // SD Gundam GX (J) [!].smc
            { "SECRET OF EVERMORE" }, // Secret of Evermore (U) [!].smc
            //{ "Secret of MANA" }, // Secret of Mana (U) [!].smc
            { "SIM CITY 2000" }, // Sim City 2000 (U).smc
            { "SMASH TENNIS" }, // Smash Tennis (E) [!].smc
            { "Star Ocean" }, // Star Ocean (J) [!].smc
            { "STREET FIGHTER ALPHA2" }, // Street Fighter Alpha 2 (U) [!].smc
            { "SUPER BASES LOADED 2" }, // Super Bases Loaded 2 (U).smc
            { "PANIC BOMBER WORLD" }, // Super Bomberman - Panic Bomber W (J).smc
            { "TALES OF PHANTASIA" }, // Tales of Phantasia (J) [!].smc
            { "TERRANIGMA P" }, // Terranigma (E) [!].smc
            { "TOP GEAR 3000" }, // Top Gear 3000 (U) [!].smc
            { "UNIRACERS" }, // Uniracers (U) [!].smc
            { "WARIO'S WOODS" }, // Wario's Woods (U) [!].smc
            { "WORLD CLASS RUGBY" }, // World Class Rugby (E) [!].smc
            { "WORLD CUP STRIKER" }, // World Cup Striker (E) (M3) [!].smc
            { "WORLD MASTERS GOLF" }, // World Masters Golf (E).smc
            { "WWF SUPER WRESTLEMANI" }, // WWF Super WrestleMania (U) [!].smc
            { "WRESTLEMANIA" }, // WWF WrestleMania - The Arcade Game (U) [!].smc
            { "SENSIBLE SOCCER" }, // Sensible Soccer - International Edition (E).smc
        };

        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;

        public SnesGame(string path, AppMetadata metadata = null, bool ignoreEmptyConfig = false)
            : base(path, metadata, ignoreEmptyConfig)
        {
        }

        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref byte saveCount, ref uint crc32)
        {
            var ext = Path.GetExtension(inputFileName);
            if (inputFileName.Contains("(E)") || inputFileName.Contains("(J)"))
                cover = Resources.blank_snes_eu_jp;

            // already in sfrom?
            if (ext.ToLower() == ".sfrom")
            {
                Debug.WriteLine("ROM is already in SFROM format, no conversion needed");
                application = "/bin/clover-canoe-shvc-wr -rom";
                args = DefaultCanoeArgs;
                return true;
            }

            // header removal
            if ((ext.ToLower() == ".smc") && ((rawRomData.Length % 1024) != 0))
            {
                Debug.WriteLine("Removing SMC header");
                var stripped = new byte[rawRomData.Length - 512];
                Array.Copy(rawRomData, 512, stripped, 0, stripped.Length);
                rawRomData = stripped;
                crc32 = Shared.CRC32(rawRomData);
            }

            // check if we can use sfrom tool
            bool convertedSuccessfully = false;
            bool isSnesSystem = ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.Instance.ConsoleType == MainForm.ConsoleType.SuperFamicom;

            if (isSnesSystem && ConfigIni.Instance.UseSFROMTool && SfromToolWrapper.IsInstalled)
            {
                Debug.WriteLine($"Convert with SFROM Tool: {inputFileName}");
                if (SfromToolWrapper.ConvertROMtoSFROM(ref rawRomData))
                {
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName) + ".sfrom";
                    application = "/bin/clover-canoe-shvc-wr -rom";
                    args = DefaultCanoeArgs;
                    convertedSuccessfully = true;
                }
                else
                {
                    Debug.WriteLine("SFROM Tool conversion failed, attempting the built-in SFROM conversion");
                    convertedSuccessfully = false;
                }
            }

            if (!convertedSuccessfully)
            {
                // fallback method, with patching
                FindPatch(ref rawRomData, inputFileName, crc32);
                if (isSnesSystem)
                {
                    Debug.WriteLine($"Trying to convert {inputFileName}");
                    bool problemGame = false;
                    MakeSfrom(ref rawRomData, ref saveCount, out problemGame);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName) + ".sfrom";

                    // Using 3rd party emulator for this ROM
                    if (problemGame && Need3rdPartyEmulator != true)
                    {
                        if (Need3rdPartyEmulator != false)
                        {
                            var r = WorkerForm.MessageBoxFromThread(ParentForm,
                                string.Format(Resources.Need3rdPartyEmulator, Path.GetFileName(inputFileName)),
                                    Resources.AreYouSure,
                                    MessageBoxButtons.AbortRetryIgnore,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, true);
                            if (r == DialogResult.Abort)
                                Need3rdPartyEmulator = true;
                            if (r == DialogResult.Ignore)
                                problemGame = false;
                        }
                        else problemGame = false;
                    }
                    if (problemGame)
                    {
                        //application = "/bin/snes";
                        //args = "";
                    }
                    else
                    {
                        application = "/bin/clover-canoe-shvc-wr -rom";
                        args = DefaultCanoeArgs;
                    }
                }
                else
                {
                    //application = "/bin/snes";
                }
            }

            return true;
        }

        public static SnesRomHeader GetCorrectHeader(byte[] rawRomData, out SnesRomType romType, out string gameTitle)
        {
            var romHeaderLoRom = SnesRomHeader.Read(rawRomData, 0x7FC0);
            var romHeaderHiRom = SnesRomHeader.Read(rawRomData, 0xFFC0);
            var titleLo = romHeaderLoRom.GameTitle;
            var titleHi = romHeaderHiRom.GameTitle;

            // Boring LoRom/HiRom detection...
            if (((romHeaderLoRom.Checksum ^ 0xFFFF) == romHeaderLoRom.ChecksumComplement) &&
                ((romHeaderHiRom.Checksum ^ 0xFFFF) != romHeaderHiRom.ChecksumComplement || romHeaderHiRom.Checksum == 0 || romHeaderHiRom.ChecksumComplement == 0))
                romType = SnesRomType.LoRom;
            else if (((romHeaderLoRom.Checksum ^ 0xFFFF) != romHeaderLoRom.ChecksumComplement || romHeaderLoRom.Checksum == 0 || romHeaderLoRom.ChecksumComplement == 0) &&
                ((romHeaderHiRom.Checksum ^ 0xFFFF) == romHeaderHiRom.ChecksumComplement))
                romType = SnesRomType.HiRom;
            else if (titleLo.Length != 0 && titleHi.Length == 0)
                romType = SnesRomType.LoRom;
            else if (titleLo.Length == 0 && titleHi.Length != 0)
                romType = SnesRomType.HiRom;
            else if ((titleLo == titleHi) && ((romHeaderLoRom.RomMakeup & 1) == 0))
                romType = SnesRomType.LoRom;
            else if ((titleLo == titleHi) && ((romHeaderHiRom.RomMakeup & 1) == 1))
                romType = SnesRomType.HiRom;
            else if (gamesLoRom.Contains(titleLo))
                romType = SnesRomType.LoRom;
            else if (gamesHiRom.Contains(titleHi))
                romType = SnesRomType.HiRom;
            else
            {
                bool loRom = true;
                bool hiRom = true;
                foreach (char c in titleLo)
                    if (c < 31 || c > 127) loRom = false;
                foreach (char c in titleHi)
                    if (c < 31 || c > 127) hiRom = false;
                if (loRom && !hiRom)
                    romType = SnesRomType.LoRom;
                else if (!loRom && hiRom)
                    romType = SnesRomType.HiRom;
                else
                {
                    Debug.WriteLine("Can't detect ROM type");
                    throw new Exception("can't detect ROM type, seems like ROM is corrupted");
                }
            }

            SnesRomHeader romHeader;
            if (romType == SnesRomType.LoRom)
            {
                romHeader = romHeaderLoRom;
                gameTitle = titleLo;
            }
            else
            {
                romHeader = romHeaderHiRom;
                gameTitle = titleHi;
            }
            return romHeader;
        }

        private static void MakeSfrom(ref byte[] rawRomData, ref byte saveCount, out bool problemGame)
        {
            SnesRomType romType;
            string gameTitle;
            SnesRomHeader romHeader = GetCorrectHeader(rawRomData, out romType, out gameTitle);

            /*
            if (romType == SnesRomType.LoRom)
                rawRomData[0x7FD9] = 0x01; // Force NTSC
            else
                rawRomData[0xFFD9] = 0x01; // Force NTSC
            */

            Debug.WriteLine($"Game title: {gameTitle}");
            Debug.WriteLine($"ROM type: {romType}");
            ushort presetId = 0;
            ushort chip = 0;
            if (SfxTypes.Contains(romHeader.RomType)) // Super FX chip
            {
                Debug.WriteLine($"Super FX chip detected");
                chip = 0x0C;
            }
            if (!knownPresets.TryGetValue(gameTitle, out presetId)) // Known codes
            {
                if (Dsp1Types.Contains(romHeader.RomType))
                {
                    Debug.WriteLine($"DSP-1 chip detected");
                    presetId = 0x10BD; // ID from Mario Kard, DSP1
                }
                if (SA1Types.Contains(romHeader.RomType))
                {
                    Debug.WriteLine($"SA1 chip detected");
                    presetId = 0x109C; // ID from Super Mario RPG, SA1
                }
            }
            else
            {
                Debug.WriteLine($"We have preset for this game");
            }
            Debug.WriteLine(string.Format("PresetID: 0x{0:X2}{1:X2}, extra byte: {2:X2}", presetId & 0xFF, (presetId >> 8) & 0xFF, chip));

            var sfromHeader1 = new SfromHeader1((uint)rawRomData.Length);
            var sfromHeader2 = new SfromHeader2((uint)rawRomData.Length, presetId, romType, chip);
            var sfromHeader1Raw = sfromHeader1.GetBytes();
            var sfromHeader2Raw = sfromHeader2.GetBytes();
            var result = new byte[sfromHeader1Raw.Length + rawRomData.Length + sfromHeader2Raw.Length];
            Array.Copy(sfromHeader1Raw, 0, result, 0, sfromHeader1Raw.Length);
            Array.Copy(rawRomData, 0, result, sfromHeader1Raw.Length, rawRomData.Length);
            Array.Copy(sfromHeader2Raw, 0, result, sfromHeader1Raw.Length + rawRomData.Length, sfromHeader2Raw.Length);

            if (romHeader.SramSize > 0)
                saveCount = 3;
            else
                saveCount = 0;

            problemGame = problemGames.Contains(gameTitle);

            rawRomData = result;
        }

        public SfromHeader1 ReadSfromHeader1()
        {
            foreach (var f in Directory.GetFiles(basePath, "*.sfrom"))
            {
                var sfrom = File.ReadAllBytes(f);
                var sfromHeader1 = SfromHeader1.Read(sfrom, 0);
                return sfromHeader1;
            }
            throw new Exception(".sfrom file not found");
        }

        public SfromHeader2 ReadSfromHeader2()
        {
            foreach (var f in Directory.GetFiles(basePath, "*.sfrom"))
            {
                var sfrom = File.ReadAllBytes(f);
                var sfromHeader1 = SfromHeader1.Read(sfrom, 0);
                var sfromHeader2 = SfromHeader2.Read(sfrom, (int)sfromHeader1.Header2);
                return sfromHeader2;
            }
            throw new Exception(".sfrom file not found");
        }

        public void WriteSfromHeader1(SfromHeader1 sfromHeader1)
        {
            foreach (var f in Directory.GetFiles(basePath, "*.sfrom"))
            {
                var sfrom = File.ReadAllBytes(f);
                var data = sfromHeader1.GetBytes();
                Array.Copy(data, 0, sfrom, 0, data.Length);
                File.WriteAllBytes(f, sfrom);
                return;
            }
            throw new Exception(".sfrom file not found");
        }

        public void WriteSfromHeader2(SfromHeader2 sfromHeader2)
        {
            foreach (var f in Directory.GetFiles(basePath, "*.sfrom"))
            {
                var sfrom = File.ReadAllBytes(f);
                var sfromHeader1 = SfromHeader1.Read(sfrom, 0);
                var data = sfromHeader2.GetBytes();
                Array.Copy(data, 0, sfrom, (int)sfromHeader1.Header2, data.Length);
                File.WriteAllBytes(f, sfrom);
                return;
            }
            throw new Exception(".sfrom file not found");
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct SnesRomHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public byte[] GameTitleArr;
            [MarshalAs(UnmanagedType.U1)] // $xFD5
            public byte RomMakeup;
            [MarshalAs(UnmanagedType.U1)] // $xFD6
            public byte RomType;
            [MarshalAs(UnmanagedType.U1)] // $xFD7
            public byte RomSize;
            [MarshalAs(UnmanagedType.U1)] // $xFD8
            public byte SramSize;
            [MarshalAs(UnmanagedType.U1)] // $xFD9
            public byte Country;
            [MarshalAs(UnmanagedType.U1)] // $xFDA
            public byte License;
            [MarshalAs(UnmanagedType.U1)] // $xFDB
            public byte Version;
            [MarshalAs(UnmanagedType.U2)] // $xFDC
            public ushort ChecksumComplement;
            [MarshalAs(UnmanagedType.U2)] // $xFDE
            public ushort Checksum;

            public string GameTitle
            {
                get
                {
                    var data = new List<byte>(GameTitleArr);
                    if (data.Contains(0))
                        return "";
                    if (data.Contains(0xFF))
                        return "";
                    if (data[0] == 0x20)
                        return "";
                    while (data.Count > 0 && data[data.Count - 1] == 0x20)
                        data.RemoveAt(data.Count - 1);
                    return Encoding.ASCII.GetString(data.ToArray());
                }
            }

            public byte[] GetBytes()
            {
                int size = Marshal.SizeOf(this);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            public static SnesRomHeader Read(byte[] buffer, int pos)
            {
                var size = Marshal.SizeOf(typeof(SnesRomHeader));
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, pos, ptr, size);
                var r = (SnesRomHeader)Marshal.PtrToStructure(ptr, typeof(SnesRomHeader));
                Marshal.FreeHGlobal(ptr);
                return r;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct SfromHeader1
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint Uknown1_0x00000100;
            [MarshalAs(UnmanagedType.U4)]
            public uint FileSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint Uknown2_0x00000030;
            [MarshalAs(UnmanagedType.U4)]
            public uint RomEnd;
            [MarshalAs(UnmanagedType.U4)]
            public uint FooterStart;
            [MarshalAs(UnmanagedType.U4)]
            public uint Header2;
            [MarshalAs(UnmanagedType.U4)]
            public uint FileSize2;
            [MarshalAs(UnmanagedType.U4)]
            public uint Uknown3_0x00000000;
            [MarshalAs(UnmanagedType.U4)]
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] VCGameID;
            [MarshalAs(UnmanagedType.U4)]
            public uint Uknown4_0x00000000;

            public SfromHeader1(uint romSize)
            {
                Uknown1_0x00000100 = 0x00000100;
                FileSize = (uint)(48 + romSize + 38);
                Uknown2_0x00000030 = 0x00000030;
                RomEnd = (uint)(48 + romSize);
                FooterStart = FileSize;
                Header2 = RomEnd;
                FileSize2 = FileSize;
                Uknown3_0x00000000 = 0;
                Flags = FileSize - 11;
                VCGameID = new byte[8];
                var VCGameID_s = Encoding.ASCII.GetBytes("WUP-XXXX");
                Array.Copy(VCGameID_s, VCGameID, VCGameID_s.Length);
                Uknown4_0x00000000 = 0;
            }

            public byte[] GetBytes()
            {
                int size = Marshal.SizeOf(this);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            public static SfromHeader1 Read(byte[] buffer, int pos)
            {
                var size = Marshal.SizeOf(typeof(SfromHeader1));
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, pos, ptr, size);
                var r = (SfromHeader1)Marshal.PtrToStructure(ptr, typeof(SfromHeader1));
                Marshal.FreeHGlobal(ptr);
                return r;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SfromHeader2
        {
            [MarshalAs(UnmanagedType.U1)] // 0x00
            public byte FPS;
            [MarshalAs(UnmanagedType.U4)] // 0x01
            public uint RomSize;
            [MarshalAs(UnmanagedType.U4)] // 0x05
            public uint PcmSize;
            [MarshalAs(UnmanagedType.U4)] // 0x09
            public uint FooterSize;
            [MarshalAs(UnmanagedType.U2)] // 0x0D
            public ushort PresetID;
            [MarshalAs(UnmanagedType.U1)] // 0x0F
            public byte Mostly2;
            [MarshalAs(UnmanagedType.U1)] // 0x10
            public byte Volume;
            [MarshalAs(UnmanagedType.U1)] // 0x11
            public byte RomType;
            [MarshalAs(UnmanagedType.U4)] // 0x12
            public uint Chip;
            [MarshalAs(UnmanagedType.U4)] // 0x16
            public uint Unknown1_0x00000000;
            [MarshalAs(UnmanagedType.U4)] // 0x1A
            public uint Unknown2_0x00000100;
            [MarshalAs(UnmanagedType.U4)] // 0x1E
            public uint Unknown3_0x00000100;
            [MarshalAs(UnmanagedType.U4)]
            public uint Unknown4_0x00000000;

            public SfromHeader2(uint romSize, ushort presetId, SnesRomType romType, uint chip)
            {
                FPS = 60;
                RomSize = romSize;
                PcmSize = 0;
                FooterSize = 0;
                PresetID = presetId;
                Mostly2 = 2;
                Volume = 100;
                RomType = (byte)romType;
                Chip = chip;
                Unknown1_0x00000000 = 0x00000000;
                Unknown2_0x00000100 = 0x00000100;
                Unknown3_0x00000100 = 0x00000100;
                Unknown4_0x00000000 = 0x00000000;
            }

            public byte[] GetBytes()
            {
                int size = Marshal.SizeOf(this);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            public static SfromHeader2 Read(byte[] buffer, int pos)
            {
                var size = Marshal.SizeOf(typeof(SfromHeader2));
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, pos, ptr, size);
                var r = (SfromHeader2)Marshal.PtrToStructure(ptr, typeof(SfromHeader2));
                Marshal.FreeHGlobal(ptr);
                return r;
            }
        }

        private struct CachedGameInfo
        {
            public string Name;
            public byte Players;
            public bool Simultaneous;
            public string ReleaseDate;
            public string Publisher;
            public string Region;
            public string CoverUrl;
        }

        public static void LoadCache()
        {
            try
            {
                var xmlDataBasePath = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "data"), "snescarts.xml");
                Debug.WriteLine("Loading " + xmlDataBasePath);

                if (File.Exists(xmlDataBasePath))
                {
                    var xpath = new XPathDocument(xmlDataBasePath);
                    var navigator = xpath.CreateNavigator();
                    var iterator = navigator.Select("/Data");
                    gameInfoCache = new Dictionary<uint, CachedGameInfo>();
                    while (iterator.MoveNext())
                    {
                        XPathNavigator game = iterator.Current;
                        var cartridges = game.Select("Game");
                        while (cartridges.MoveNext())
                        {
                            var cartridge = cartridges.Current;
                            uint crc = 0;
                            var info = new CachedGameInfo();

                            try
                            {
                                var v = cartridge.Select("name");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.Name = v.Current.Value;
                                v = cartridge.Select("players");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.Players = byte.Parse(v.Current.Value);
                                v = cartridge.Select("simultaneous");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.Simultaneous = byte.Parse(v.Current.Value) != 0;
                                v = cartridge.Select("crc");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    crc = Convert.ToUInt32(v.Current.Value, 16);
                                v = cartridge.Select("date");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.ReleaseDate = v.Current.Value;
                                v = cartridge.Select("publisher");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.Publisher = v.Current.Value;
                                v = cartridge.Select("region");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.Region = v.Current.Value;
                                v = cartridge.Select("cover");
                                if (v.MoveNext() && !string.IsNullOrEmpty(v.Current.Value))
                                    info.CoverUrl = v.Current.Value;
                            }
                            catch
                            {
                                Debug.WriteLine($"Invalid XML record for game: {cartridge.OuterXml}");
                            }

                            gameInfoCache[crc] = info;
                        };
                    }
                }
                Debug.WriteLine(string.Format("SNES XML loading done, {0} roms total", gameInfoCache.Count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public bool TryAutofill(uint crc32)
        {
            CachedGameInfo gameinfo;
            if (gameInfoCache != null && gameInfoCache.TryGetValue(crc32, out gameinfo))
            {
                if (!string.IsNullOrEmpty(gameinfo.Name))
                    Name = gameinfo.Name;
                desktop.Players = gameinfo.Players;
                desktop.Simultaneous = gameinfo.Simultaneous;
                if (!string.IsNullOrEmpty(gameinfo.ReleaseDate))
                {
                    string releaseDate = gameinfo.ReleaseDate;
                    if (releaseDate.Length == 4) releaseDate += "-01";
                    if (releaseDate.Length == 7) releaseDate += "-01";
                    desktop.ReleaseDate = releaseDate;
                }
                if (!string.IsNullOrEmpty(gameinfo.Publisher))
                    desktop.Publisher = gameinfo.Publisher.ToUpper();

                return true;
            }
            return false;
        }

        public void ApplyGameGenie()
        {
            if (!string.IsNullOrEmpty(GameGenie))
            {
                var codes = GameGenie.Split(new char[] { ',', '\t', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                var nesFiles = Directory.GetFiles(this.basePath, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var f in nesFiles)
                {
                    byte[] data;
                    var ext = Path.GetExtension(f).ToLower();
                    int offset;
                    if (ext == ".sfrom")
                    {
                        data = File.ReadAllBytes(f);
                        offset = 48;
                    }  else if (ext == ".sfc" || ext == ".smc")
                    {
                        data = File.ReadAllBytes(f);
                        if ((data.Length % 1024) != 0)
                            offset = 512;
                        else
                            offset = 0;
                    }
                    else continue;

                    var rawData = new byte[data.Length - offset];
                    Array.Copy(data, offset, rawData, 0, rawData.Length);

                    foreach (var code in codes)
                        rawData = GameGeniePatcherSnes.Patch(rawData, code);

                    Array.Copy(rawData, 0, data, offset, rawData.Length);
                    File.WriteAllBytes(f, data);                        
                }
            }
        }
    }
}

