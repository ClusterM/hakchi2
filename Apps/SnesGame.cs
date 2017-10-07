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
    public class SnesGame : NesMiniApplication, ICloverAutofill
    {
        public enum SnesRomType { LoRom = 0x14, HiRom = 0x15 };

        const string DefaultArgs = "--volume 100 -rollback-snapshot-period 600";
        static List<byte> SfxTypes = new List<byte>() { 0x13, 0x14, 0x15, 0x1a };
        static List<byte> Dsp1Types = new List<byte>() { 0x03, 0x05 };
        static List<byte> SA1Types = new List<byte>() { 0x34, 0x35 };
        static Dictionary<string, ushort> knownPresets = new Dictionary<string, ushort>()
        {
            { "SUPER MARIOWORLD", 0x1011 },
            { "F-ZERO", 0x1018 },
            { "THE LEGEND OF ZELDA", 0x101D },
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
            { "Secret of MANA", 0x10B0 },
            { "FINAL FANTASY 3", 0x10DC },
            { "SUPER CASTLEVANIA 4", 0x1030 },
            { "CONTRA3 THE ALIEN WA", 0x1036 },
            { "STAR FOX", 0x1242 },
            { "YOSHI'S ISLAND", 0x123D },
            { "STARFOX2", 0x123C },
            { "ZELDANODENSETSU", 0x101F },
            { "SHVC FIREEMBLEM", 0x102B },
            { "SUPER DONKEY KONG", 0x1023 },
            { "Super Street Fighter", 0x1056 },
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
            { "KIRBY'S DREAM LAND 3", 0x10A2 },
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
        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;

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

        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref byte saveCount, ref uint crc32)
        {
            var ext = Path.GetExtension(inputFileName);
            if ((ext.ToLower() == ".smc") && ((rawRomData.Length % 1024) != 0))
            {
                var stripped = new byte[rawRomData.Length - 512];
                Array.Copy(rawRomData, 512, stripped, 0, stripped.Length);
                rawRomData = stripped;
                crc32 = CRC32(rawRomData);
            }
            FindPatch(ref rawRomData, inputFileName, crc32);
            if (inputFileName.Contains("(E)") || inputFileName.Contains("(J)"))
                cover = Resources.blank_snes_eu_jp;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                application = "/bin/clover-canoe-shvc-wr -rom";
                args = DefaultArgs;
                if (ext.ToLower() != ".sfrom") // Need to patch for canoe
                {
                    Debug.WriteLine($"Trying to convert {inputFileName}");
                    MakeSfrom(ref rawRomData, ref saveCount);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName) + ".sfrom";
                }
            }
            else
            {
                application = "/bin/snes";
            }

            return true;
        }

        private static void MakeSfrom(ref byte[] rawRomData, ref byte saveCount)
        {
            var romHeaderLoRom = SnesRomHeader.Read(rawRomData, 0x7FC0);
            var romHeaderHiRom = SnesRomHeader.Read(rawRomData, 0xFFC0);
            SnesRomHeader romHeader;
            bool loRom = true;
            bool hiRom = true;
            if (romHeaderLoRom.GameTitle.Length == 0)
                loRom = false;
            foreach (char c in romHeaderLoRom.GameTitle)
                if (c < 31 || c > 127) loRom = false;
            if (romHeaderHiRom.GameTitle.Length == 0)
                hiRom = false;
            foreach (char c in romHeaderHiRom.GameTitle)
                if (c < 31 || c > 127) hiRom = false;
            SnesRomType romType;
            if (loRom && !hiRom)
            {
                romType = SnesRomType.LoRom;
                romHeader = romHeaderLoRom;
            }
            else if (!loRom && hiRom)
            {
                romType = SnesRomType.HiRom;
                romHeader = romHeaderHiRom;
            }
            else if (((romHeaderLoRom.RomMakeup & 1) == 0) && ((romHeaderHiRom.RomMakeup & 1) == 0))
            {
                romType = SnesRomType.LoRom;
                romHeader = romHeaderLoRom;
            }
            else if (((romHeaderLoRom.RomMakeup & 1) == 1) && ((romHeaderHiRom.RomMakeup & 1) == 1))
            {
                romType = SnesRomType.HiRom;
                romHeader = romHeaderHiRom;
            }
            else
            {
                // WTF is it?
                romType = SnesRomType.HiRom;
                romHeader = romHeaderHiRom;
            }

            string gameTitle = romHeader.GameTitle.Trim();
            Debug.WriteLine($"Game title: {gameTitle}");
            ushort presetId = 0; // 0x1011;
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

            rawRomData = result;
        }

        public SfromHeader1 ReadSfromHeader1()
        {
            foreach (var f in Directory.GetFiles(GamePath, "*.sfrom"))
            {
                var sfrom = File.ReadAllBytes(f);
                var sfromHeader1 = SfromHeader1.Read(sfrom, 0);
                return sfromHeader1;
            }
            throw new Exception(".sfrom file not found");
        }

        public SfromHeader2 ReadSfromHeader2()
        {
            foreach (var f in Directory.GetFiles(GamePath, "*.sfrom"))
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
            foreach (var f in Directory.GetFiles(GamePath, "*.sfrom"))
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
            foreach (var f in Directory.GetFiles(GamePath, "*.sfrom"))
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
        private struct SnesRomHeader
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            public string GameTitle;
            [MarshalAs(UnmanagedType.U1)] // $xFD5
            public byte RomMakeup;
            [MarshalAs(UnmanagedType.U1)] // $xFD6
            public byte RomType;
            [MarshalAs(UnmanagedType.U1)] // $xFD7
            public byte RomSize;
            [MarshalAs(UnmanagedType.U1)] // $xFD8
            public byte SramSize;
            [MarshalAs(UnmanagedType.U2)] // $xFD9
            public ushort LicenseId;
            [MarshalAs(UnmanagedType.U1)] // $xFDB
            public byte Version;
            [MarshalAs(UnmanagedType.U2)] // $xFDC
            public ushort ChecksumComplement;
            [MarshalAs(UnmanagedType.U2)] // $xFDE
            public ushort Checksum;

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
                var xmlDataBasePath = Path.Combine(System.IO.Path.Combine(Program.BaseDirectoryInternal, "data"), "snescarts.xml");
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
                Name = gameinfo.Name;
                Players = gameinfo.Players;
                Simultaneous = gameinfo.Simultaneous;
                ReleaseDate = gameinfo.ReleaseDate;
                if (ReleaseDate.Length == 4) ReleaseDate += "-01";
                if (ReleaseDate.Length == 7) ReleaseDate += "-01";
                Publisher = gameinfo.Publisher.ToUpper();

                /*
                if (!string.IsNullOrEmpty(gameinfo.CoverUrl))
                {
                    if (NeedAutoDownloadCover != true)
                    {
                        if (NeedAutoDownloadCover != false)
                        {
                            var r = WorkerForm.MessageBoxFromThread(ParentForm,
                                string.Format(Resources.DownloadCoverQ, Name),
                                Resources.Cover,
                                MessageBoxButtons.AbortRetryIgnore,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2, true);
                            if (r == DialogResult.Abort)
                                NeedAutoDownloadCover = true;
                            if (r == DialogResult.Ignore)
                                return true;
                        }
                        else return true;
                    }

                    try
                    {
                        var cover = ImageGooglerForm.DownloadImage(gameinfo.CoverUrl);
                        Image = cover;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
                */
                return true;
            }
            return false;
        }
    }
}

