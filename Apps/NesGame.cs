#pragma warning disable 0108
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class NesGame : NesMiniApplication, ICloverAutofill, ISupportsGameGenie
    {
        public const char Prefix = 'H';
        public string GameGeniePath { private set; get; }
        public static bool? IgnoreMapper;
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet";
        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;

        public const string GameGenieFileName = "gamegenie.txt";
        private static byte[] supportedMappers = new byte[] { 0, 1, 2, 3, 4, 5, 7, 9, 10, 86, 87, 184 };

        public override string GoogleSuffix
        {
            get
            {
                return "(nes | famicom)";
            }
        }

        private string gameGenie = "";
        public string GameGenie
        {
            get { return gameGenie; }
            set
            {
                if (gameGenie != value) hasUnsavedChanges = true;
                gameGenie = value;
            }
        }

        public NesGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
            GameGeniePath = System.IO.Path.Combine(path, GameGenieFileName);
            if (File.Exists(GameGeniePath))
                gameGenie = File.ReadAllText(GameGeniePath);
        }
        
        public static bool Patch(string inputFileName, ref byte[] rawRomData, ref char prefix, ref string application, ref string outputFileName, ref string args, ref Image cover, ref uint crc32)
        {
            // Try to patch before mapper check, maybe it will patch mapper
            FindPatch(ref rawRomData, inputFileName, crc32);

            NesFile nesFile;
            try
            {
                nesFile = new NesFile(rawRomData);
            }
            catch
            {
                application = "/bin/nes";
                return true;
            }
            nesFile.CorrectRom();
            crc32 = nesFile.CRC32;
            if (ConfigIni.ConsoleType != MainForm.ConsoleType.NES && ConfigIni.ConsoleType != MainForm.ConsoleType.Famicom)
                application = "/bin/nes";

            //if (nesFile.Mapper == 71) nesFile.Mapper = 2; // games by Codemasters/Camerica - this is UNROM clone. One exception - Fire Hawk
            //if (nesFile.Mapper == 88) nesFile.Mapper = 4; // Compatible with MMC3... sometimes
            //if (nesFile.Mapper == 95) nesFile.Mapper = 4; // Compatible with MMC3
            //if (nesFile.Mapper == 206) nesFile.Mapper = 4; // Compatible with MMC3
            if (!supportedMappers.Contains(nesFile.Mapper) && (IgnoreMapper != true))
            {
                if (IgnoreMapper != false)
                {
                    var r = WorkerForm.MessageBoxFromThread(ParentForm,
                        string.Format(Resources.MapperNotSupported, System.IO.Path.GetFileName(inputFileName), nesFile.Mapper),
                            Resources.AreYouSure,
                            MessageBoxButtons.AbortRetryIgnore,
                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, true);
                    if (r == DialogResult.Abort)
                        IgnoreMapper = true;
                    if (r == DialogResult.Ignore)
                        return false;
                }
                else return false;
            }
            if ((nesFile.Mirroring == NesFile.MirroringType.FourScreenVram) && (IgnoreMapper != true))
            {
                var r = WorkerForm.MessageBoxFromThread(ParentForm,
                    string.Format(Resources.FourScreenNotSupported, System.IO.Path.GetFileName(inputFileName)),
                        Resources.AreYouSure,
                        MessageBoxButtons.AbortRetryIgnore,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, true);
                if (r == DialogResult.Abort)
                    IgnoreMapper = true;
                if (r == DialogResult.No)
                    return false;
            }

            // TODO: Make trainer check. I think that the NES Mini doesn't support it.
            rawRomData = nesFile.GetRaw();
            if (inputFileName.Contains("(J)")) cover = Resources.blank_jp;
            args = DefaultArgs;
            return true;
        }

        /*
        public static NesMiniApplication Import(string nesFileName, string sourceFileName, bool? ignoreMapper, ref bool? needPatch, NeedPatchDelegate needPatchCallback = null, Form parentForm = null, byte[] rawRomData = null)
        {
            NesFile nesFile;
            try
            {
                if (rawRomData != null)
                    nesFile = new NesFile(rawRomData);
                else
                    nesFile = new NesFile(nesFileName);
            }
            catch
            {
                return NesMiniApplication.Import(nesFileName, sourceFileName, rawRomData);
            }
            nesFile.CorrectRom();
            var crc32 = nesFile.CRC32;
            var code = GenerateCode(crc32, Prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var nesPath = Path.Combine(gamePath, code + ".nes");
            var patchesDirectory = Path.Combine(Program.BaseDirectoryExternal, "patches");
            Directory.CreateDirectory(patchesDirectory);
            Directory.CreateDirectory(gamePath);
            var patches = Directory.GetFiles(patchesDirectory, string.Format("{0:X8}*.ips", crc32), SearchOption.AllDirectories);
            if (patches.Length > 0 && needPatch != false)
            {
                if (needPatch == true || ((needPatchCallback != null) && needPatchCallback(parentForm, Path.GetFileName(nesFileName))))
                {
                    needPatch = true;
                    var patch = patches[0];
                    if (rawRomData == null)
                        rawRomData = File.ReadAllBytes(nesFileName);
                    Debug.WriteLine(string.Format("Patching {0}", nesFileName));
                    IpsPatcher.Patch(patch, ref rawRomData);
                    nesFile = new NesFile(rawRomData);
                }
                else needPatch = false;
            }

            if (!supportedMappers.Contains(nesFile.Mapper) && (ignoreMapper != true))
            {
                Directory.Delete(gamePath, true);
                if (ignoreMapper != false)
                    throw new UnsupportedMapperException(nesFile);
                else
                {
                    Debug.WriteLine(string.Format("Game {0} has mapper #{1}, skipped", nesFileName, nesFile.Mapper));
                    return null;
                }
            }
            if ((nesFile.Mirroring == NesFile.MirroringType.FourScreenVram) && (ignoreMapper != true))
            {
                Directory.Delete(gamePath, true);
                if (ignoreMapper != false)
                    throw new UnsupportedFourScreenException(nesFile);
                else
                {
                    Debug.WriteLine(string.Format("Game {0} has four-screen mirroring, skipped", nesFileName, nesFile.Mapper));
                    return null;
                }
            }
            // TODO: Make trainer check. I think that the NES Mini doesn't support it.

            nesFile.Save(nesPath);
            var game = new NesGame(gamePath, true);

            game.Name = Path.GetFileNameWithoutExtension(nesFileName);
            if (game.Name.Contains("(J)")) game.region = "Japan";
            game.TryAutofill(crc32);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ");
            game.FindCover(nesFileName, sourceFileName, (game.region == "Japan") ? Resources.blank_jp : Resources.blank_nes, crc32);
            game.Args = DefaultArgs;
            game.Save();
            return game;
        }
        */

        public bool TryAutofill(uint crc32)
        {
            CachedGameInfo gameinfo;
            if (gameInfoCache != null && gameInfoCache.TryGetValue(crc32, out gameinfo))
            {
                Name = gameinfo.Name;
                Name = Name.Replace("_", " ").Replace("  ", " ").Trim();
                Players = gameinfo.Players;
                if (Players > 1) Simultaneous = true; // actually unknown...
                ReleaseDate = gameinfo.ReleaseDate;
                if (ReleaseDate.Length == 4) ReleaseDate += "-01";
                if (ReleaseDate.Length == 7) ReleaseDate += "-01";
                Publisher = gameinfo.Publisher.ToUpper();
                return true;
            }
            return false;
        }

        public override bool Save()
        {
            var old = hasUnsavedChanges;
            if (hasUnsavedChanges)
            {
                if (!string.IsNullOrEmpty(gameGenie))
                    File.WriteAllText(GameGeniePath, gameGenie);
                else
                    File.Delete(GameGeniePath);
            }
            return base.Save() || old;
        }

        public void ApplyGameGenie()
        {
            if (!string.IsNullOrEmpty(GameGenie))
            {
                var codes = GameGenie.Split(new char[] { ',', '\t', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                var nesFiles = Directory.GetFiles(this.Path, "*.nes", SearchOption.TopDirectoryOnly);
                foreach (var f in nesFiles)
                {
                    var nesFile = new NesFile(f);
                    foreach (var code in codes)
                    {
                        nesFile.PRG = GameGeniePatcher.Patch(nesFile.PRG, code.Trim());
                    }
                    nesFile.Save(f);
                }
            }
        }

        private struct CachedGameInfo
        {
            public string Name;
            public byte Players;
            public string ReleaseDate;
            public string Publisher;
            public string Region;
        }

        public static void LoadCache()
        {
            try
            {
                var xmlDataBasePath = System.IO.Path.Combine(System.IO.Path.Combine(Program.BaseDirectoryInternal, "data"), "nescarts.xml");
                Debug.WriteLine("Loading " + xmlDataBasePath);

                if (File.Exists(xmlDataBasePath))
                {
                    var xpath = new XPathDocument(xmlDataBasePath);
                    var navigator = xpath.CreateNavigator();
                    var iterator = navigator.Select("/database/game");
                    gameInfoCache = new Dictionary<uint, CachedGameInfo>();
                    while (iterator.MoveNext())
                    {
                        XPathNavigator game = iterator.Current;
                        var cartridges = game.Select("cartridge");
                        while (cartridges.MoveNext())
                        {
                            var cartridge = cartridges.Current;
                            try
                            {
                                var crc = Convert.ToUInt32(cartridge.GetAttribute("crc", ""), 16);
                                gameInfoCache[crc] = new CachedGameInfo
                                {
                                    Name = game.GetAttribute("name", ""),
                                    Players = (byte)((game.GetAttribute("players", "") != "1") ? 2 : 1),
                                    ReleaseDate = game.GetAttribute("date", ""),
                                    Publisher = game.GetAttribute("publisher", ""),
                                    Region = game.GetAttribute("region", "")
                                };
                            }
                            catch { }
                        };
                    }
                }
                Debug.WriteLine(string.Format("XML loading done, {0} roms total", gameInfoCache.Count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
        }

    }
}

