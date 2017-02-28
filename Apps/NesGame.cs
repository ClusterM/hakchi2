#pragma warning disable 0108
using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class NesGame : NesMiniApplication
    {
        public delegate bool NeedPatchDelegate(Form parentForm, string nesFileName);
        public const char Prefix = 'H';
        public readonly string NesPath;
        public readonly string GameGeniePath;

        private string region = null;
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet";

        public override string GoogleSuffix
        {
            get
            {
                return "(nes | famicom)";
            }
        }

        public string Args
        {
            get
            {
                if (Command.Contains(".nes"))
                    return Command.Substring(Command.IndexOf(".nes") + 4).Trim();
                else
                    return "";
            }
            set
            {
                Command = string.Format("/bin/clover-kachikachi-wr /usr/share/games/nes/kachikachi/{0}/{0}.nes {1}", code, value);
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
        public string Region
        {
            get { return region; }
            private set { region = value; }
        }

        private static Dictionary<uint, CachedGameInfo> gameInfoCache = null;
        public const string GameGenieFileName = "gamegenie.txt";
        private static byte[] supportedMappers = new byte[] { 0, 1, 2, 3, 4, 5, 7, 9, 10, 86, 87, 184 };

        public NesGame(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
            NesPath = Path.Combine(GamePath, Code + ".nes");
            GameGeniePath = Path.Combine(path, GameGenieFileName);
            //if (!File.Exists(NesPath)) throw new FileNotFoundException("Invalid game directory: " + path);

            if (File.Exists(GameGeniePath))
                gameGenie = File.ReadAllText(GameGeniePath);
            Args = Args; // To update exec path if need
        }

        public static NesMiniApplication Import(string nesFileName, bool? ignoreMapper, ref bool? needPatch, NeedPatchDelegate needPatchCallback = null, Form parentForm = null, byte[] rawRomData = null)
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
                return NesMiniApplication.Import(nesFileName, rawRomData);
            }
            nesFile.CorrectRom();
            var crc32 = nesFile.CRC32;
            var code = GenerateCode(crc32, Prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var nesPath = Path.Combine(gamePath, code + ".nes");
            var patchesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "patches");
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

            //if (nesFile.Mapper == 71) nesFile.Mapper = 2; // games by Codemasters/Camerica - this is UNROM clone. One exception - Fire Hawk
            //if (nesFile.Mapper == 88) nesFile.Mapper = 4; // Compatible with MMC3... sometimes
            //if (nesFile.Mapper == 95) nesFile.Mapper = 4; // Compatible with MMC3
            //if (nesFile.Mapper == 206) nesFile.Mapper = 4; // Compatible with MMC3
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
            game.FindCover(nesFileName, (game.region == "Japan") ? Resources.blank_jp : Resources.blank_nes, crc32);
            game.Args = DefaultArgs;
            game.Save();
            return game;
        }

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
                Region = gameinfo.Region;
                return true;
            }
            return false;
        }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(gameGenie))
                File.WriteAllText(GameGeniePath, gameGenie);
            else
                File.Delete(GameGeniePath);
            base.Save();
        }

        public void ApplyGameGenie()
        {
            if (!string.IsNullOrEmpty(GameGenie))
            {
                var codes = GameGenie.Split(new char[] { ',', '\t', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                var nesFile = new NesFile(NesPath);
                foreach (var code in codes)
                {
                    nesFile.PRG = GameGeniePatcher.Patch(nesFile.PRG, code.Trim());
                }
                nesFile.Save(NesPath);
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
                var xmlDataBasePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data"), "nescarts.xml");
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

