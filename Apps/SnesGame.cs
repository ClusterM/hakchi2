#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public class SnesGame : NesMiniApplication
    {
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
            { "STAR FOX", 0x123B },
            { "YOSHI'S ISLAND", 0x123D },
            { "STARFOX2", 0x1245 },
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

        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref uint crc32)
        {
            FindPatch(ref rawRomData, inputFileName, crc32);
            if (inputFileName.Contains("(E)") || inputFileName.Contains("(J)"))
                cover = Resources.blank_snes_eu_jp;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                application = "/bin/clover-canoe-shvc-wr -rom";
                args = DefaultArgs;
                var ext = Path.GetExtension(inputFileName);
                if (ext.ToLower() != ".sfrom") // Need to patch for canoe
                {
                    if ((ext.ToLower() == ".smc") && ((rawRomData.Length % 1024) != 0))
                    {
                        var stripped = new byte[rawRomData.Length - 512];
                        Array.Copy(rawRomData, 512, stripped, 0, stripped.Length);
                        rawRomData = stripped;
                    }
                    MakeSfrom(ref rawRomData);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName) + ".sfrom";
                }
            }
            else
            {
                application = "/bin/snes";
            }

            return true;
        }

        private static void MakeSfrom(ref byte[] rawRomData)
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
            else if ((romHeaderLoRom.RomMakeup & 1) == 0)
            {
                romType = SnesRomType.LoRom;
                romHeader = romHeaderLoRom;
            }
            else
            {
                romType = SnesRomType.HiRom;
                romHeader = romHeaderHiRom;
            }

            string gameTitle = romHeader.GameTitle.Trim();
            ushort presetId = 0; // 0x1011;
            ushort chip = 0;
            if (SfxTypes.Contains(romHeader.RomType)) // Super FX chip
                chip = 0x0C;
            if (!knownPresets.TryGetValue(gameTitle, out presetId)) // Known codes
            {
                if (Dsp1Types.Contains(romHeader.RomType))
                    presetId = 0x10BD; // ID from Mario Kard, DSP1
                if (SA1Types.Contains(romHeader.RomType))
                    presetId = 0x109C; // ID from Super Mario RPG, SA1
            }

            var sfromHeader1 = new SfromHeader1((uint)rawRomData.Length);
            var sfromHeader2 = new SfromHeader2((uint)rawRomData.Length, presetId, romType, chip);
            var sfromHeader1Raw = sfromHeader1.GetBytes();
            var sfromHeader2Raw = sfromHeader2.GetBytes();
            var result = new byte[sfromHeader1Raw.Length + rawRomData.Length + sfromHeader2Raw.Length];
            Array.Copy(sfromHeader1Raw, 0, result, 0, sfromHeader1Raw.Length);
            Array.Copy(rawRomData, 0, result, sfromHeader1Raw.Length, rawRomData.Length);
            Array.Copy(sfromHeader2Raw, 0, result, sfromHeader1Raw.Length + rawRomData.Length, sfromHeader2Raw.Length);
            rawRomData = result;
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
        private struct SfromHeader1
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
        private struct SfromHeader2
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

        private enum SnesRomType { LoRom = 0x14, HiRom = 0x15 };
    }
}

