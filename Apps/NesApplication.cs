using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace com.clusterrr.hakchi_gui
{
    public class NesApplication : NesMenuElementBase
    {
        public const uint MaxCompress = 16 * 1024 * 1024; // 16 megabytes

        public static Form ParentForm;
        public static bool? NeedPatch;
        public static bool? Need3rdPartyEmulator;
        public static bool? NeedAutoDownloadCover;

        public static NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros." },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3" },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2" },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong" },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike" },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda" },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure" },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid" },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight" },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link" },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream" },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber" },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus" },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros." },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO" },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics" },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2" },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™" },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®" },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE"  },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN" },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga" },
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania" },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS" },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C" },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest" },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN" },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL" },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge" }
        };
        public static NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ" },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３" },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ" },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング"  },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク"  },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ"  },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説"  },
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語"  },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド"  },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト"  },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険"  },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー"     },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ"  },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ"    },
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎"   },
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®"    },
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III"  },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン"  },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ" },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス" },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅"  },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー" },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝"  },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵"  },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲" },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge" },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語"  },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会" },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎" }
        };
        public static NesDefaultGame[] defaultSnesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-SAAAE",  Name = "Super Mario World" },
            new NesDefaultGame { Code = "CLV-P-SAABE",  Name = "F-ZERO" },
            new NesDefaultGame { Code = "CLV-P-SAAEE",  Name = "The Legend of Zelda: A Link to the Past" },
            new NesDefaultGame { Code = "CLV-P-SAAFE",  Name = "Super Mario Kart" },
            new NesDefaultGame { Code = "CLV-P-SAAHE",  Name = "Super Metroid" },
            new NesDefaultGame { Code = "CLV-P-SAAJE",  Name = "EarthBound" },
            new NesDefaultGame { Code = "CLV-P-SAAKE",  Name = "Kirby's Dream Course" },
            new NesDefaultGame { Code = "CLV-P-SAALE",  Name = "Donkey Kong Country" },
            new NesDefaultGame { Code = "CLV-P-SAAQE",  Name = "Kirby Super Star" },
            new NesDefaultGame { Code = "CLV-P-SAAXE",  Name = "Super Punch-Out!!" },
            new NesDefaultGame { Code = "CLV-P-SABCE",  Name = "Mega Man X" },
            new NesDefaultGame { Code = "CLV-P-SABDE",  Name = "Super Ghouls'n Ghosts" },
            new NesDefaultGame { Code = "CLV-P-SABHE",  Name = "Street Fighter II Turbo: Hyper Fighting" },
            new NesDefaultGame { Code = "CLV-P-SABQE",  Name = "Super Mario RPG: Legend of the Seven Stars" },
            new NesDefaultGame { Code = "CLV-P-SABRE",  Name = "Secret of Mana" },
            new NesDefaultGame { Code = "CLV-P-SABTE",  Name = "Final Fantasy III" },
            new NesDefaultGame { Code = "CLV-P-SACBE",  Name = "Super Castlevania IV" },
            new NesDefaultGame { Code = "CLV-P-SACCE",  Name = "CONTRA III THE ALIEN WARS" },
            new NesDefaultGame { Code = "CLV-P-SADGE",  Name = "Star Fox" },
            new NesDefaultGame { Code = "CLV-P-SADJE",  Name = "Yoshi's Island" },
            new NesDefaultGame { Code = "CLV-P-SADKE",  Name = "Star Fox 2" }
        };
        public static NesDefaultGame[] defaultSuperFamicomGames = new NesDefaultGame[]
        {
            new NesDefaultGame { Code = "CLV-P-VAAAJ",  Name = "スーパーマリオワールド" },
            new NesDefaultGame { Code = "CLV-P-VAABJ",  Name = "F-ZERO" },
            new NesDefaultGame { Code = "CLV-P-VAAEJ",  Name = "ゼルダの伝説 神々のトライフォース" },
            new NesDefaultGame { Code = "CLV-P-VAAFJ",  Name = "スーパーマリオカート" },
            new NesDefaultGame { Code = "CLV-P-VAAGJ",  Name = "ファイアーエムブレム 紋章の謎" },
            new NesDefaultGame { Code = "CLV-P-VAAHJ",  Name = "スーパーメトロイド" },
            new NesDefaultGame { Code = "CLV-P-VAALJ",  Name = "スーパードンキーコング" },
            new NesDefaultGame { Code = "CLV-P-VAAQJ",  Name = "星のカービィ スーパーデラックス" },
            new NesDefaultGame { Code = "CLV-P-VABBJ",  Name = "スーパーストリートファイターⅡ ザ ニューチャレンジャーズ" },
            new NesDefaultGame { Code = "CLV-P-VABCJ",  Name = "ロックマンX" },
            new NesDefaultGame { Code = "CLV-P-VABDJ",  Name = "超魔界村" },
            new NesDefaultGame { Code = "CLV-P-VABQJ",  Name = "スーパーマリオRPG" },
            new NesDefaultGame { Code = "CLV-P-VABRJ",  Name = "聖剣伝説2" },
            new NesDefaultGame { Code = "CLV-P-VABTJ",  Name = "ファイナルファンタジーVI" },
            new NesDefaultGame { Code = "CLV-P-VACCJ",  Name = "魂斗羅スピリッツ" },
            new NesDefaultGame { Code = "CLV-P-VACDJ",  Name = "がんばれゴエモン ゆき姫救出絵巻" },
            new NesDefaultGame { Code = "CLV-P-VADFJ",  Name = "スーパーフォーメーションサッカー" },
            new NesDefaultGame { Code = "CLV-P-VADGJ",  Name = "スターフォックス" },
            new NesDefaultGame { Code = "CLV-P-VADJJ",  Name = "スーパーマリオ ヨッシーアイランド" },
            new NesDefaultGame { Code = "CLV-P-VADKJ",  Name = "スターフォックス2" },
            new NesDefaultGame { Code = "CLV-P-VADZJ",  Name = "パネルでポン" },
        };

        public static NesDefaultGame[] DefaultGames
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                        return defaultNesGames;
                    case MainForm.ConsoleType.Famicom:
                        return defaultFamicomGames;
                    case MainForm.ConsoleType.SNES:
                        return defaultSnesGames;
                    case MainForm.ConsoleType.SuperFamicom:
                        return defaultSuperFamicomGames;
                }
            }
        }
        public static NesDefaultGame[] AllDefaultGames
        {
            get { return Shared.ConcatArrays(defaultNesGames, defaultFamicomGames, defaultSnesGames, defaultSuperFamicomGames); }
        }

        public static readonly string OriginalGamesDirectory = Path.Combine(Program.BaseDirectoryExternal, "games_originals");
        public static readonly string OriginalGamesCacheDirectory = Path.Combine(Program.BaseDirectoryExternal, "games_cache");
        public static readonly string GamesDirectory = Path.Combine(Program.BaseDirectoryExternal, "games");
        public static string MediaHakchiPath = "/media";
        public static string GamesHakchiPath = "/var/games";
        public static string GamesHakchiProfilePath = "/var/saves";
        public static string GamesSquashPath
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return "/usr/share/games/nes/kachikachi";
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return "/usr/share/games";
                }
            }
        }

        protected AppTypeCollection.AppInfo appInfo = AppTypeCollection.UnknownApplicationType;
        public AppTypeCollection.AppInfo AppInfo
        {
            get { return appInfo; }
        }

        public class AppMetadata
        {
            public AppTypeCollection.AppInfo AppInfo = null;
            public CoreCollection.CoreInfo Core = null;
            public string System = string.Empty;
            public string OriginalFilename = string.Empty;
            public uint OriginalCrc32 = 0;
        }
        public AppMetadata Metadata = new AppMetadata();

        public const string GameGenieFileName = "gamegenie.txt";
        public string GameGeniePath { private set; get; }
        private string gameGenie = "";
        public string GameGenie
        {
            get { return gameGenie; }
            set
            {
                //if (gameGenie != value) hasUnsavedChanges = true;
                gameGenie = value;
            }
        }

        // derived from NesMenuElementBase:
        //
        // DesktopFile desktop
        // string basePath
        // string iconPath
        // string smallIconPath

        public string GameFilePath
        {
            get
            {
                foreach (var arg in desktop.Args)
                {
                    Match m = Regex.Match(arg, @"(^\/.*)\/(?:" + desktop.Code + @"\/)([^.]*)(.*$)"); // actual regex: /(^\/.*)\/(?:...-.-.....\/)([^.]*)(.*$)/
                    if (m.Success)
                    {
                        string gameFile = Path.Combine(basePath, m.Groups[2].ToString() + m.Groups[3].ToString());
                        if (File.Exists(gameFile))
                            return gameFile;
                    }
                }
                return null;
            }
        }

        private bool isOriginalGame;
        public bool IsOriginalGame
        {
            get { return isOriginalGame; }
        }

        private bool isDeleting;
        public bool IsDeleting
        {
            get { return isDeleting; }
            set { isDeleting = value; }
        }

        public static NesApplication FromDirectory(string path, bool ignoreEmptyConfig = false)
        {
            (new DirectoryInfo(path)).Refresh();
            string[] files = Directory.GetFiles(path, "*.desktop", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new FileNotFoundException($"Invalid application folder: \"{path}\".");

            // look for the new metadata
            if (File.Exists(Path.Combine(path, "metadata.json")))
            {
                // TODO read metadata and use it to create more precise app
            }

            // read .desktop file and guess app type
            string[] config = File.ReadAllLines(TarStream.refFileGet(files[0]));
            foreach (var line in config)
            {
                if (line.StartsWith("Exec="))
                {
                    string exec = line.Substring(5);
                    var app = AppTypeCollection.GetAppByExec(exec);
                    if (!app.Unknown)
                    {
                        var constructor = app.Class.GetConstructor(new Type[] { typeof(string), typeof(bool) });
                        return (NesApplication)constructor.Invoke(new object[] { path, ignoreEmptyConfig });
                    }
                    break;
                }
            }
            return new NesApplication(path, ignoreEmptyConfig);
        }

        public static NesApplication Import(string inputFileName, string originalFileName = null, byte[] rawRomData = null)
        {
            var ext = Path.GetExtension(inputFileName).ToLower();
            if (ext == ".desktop") // already hakchi2-ed game
                return ImportApp(inputFileName);

            if (rawRomData == null && (new FileInfo(inputFileName)).Length <= MaxCompress) // read file if not already and not too big
                rawRomData = File.ReadAllBytes(inputFileName);
            if (originalFileName == null) // Original file name from archive
                originalFileName = Path.GetFileName(inputFileName);

            // start building some file type info
            AppTypeCollection.AppInfo appInfo = AppTypeCollection.GetAppByExtension(ext);
            char prefix = appInfo.Prefix;
            string application = ext.Length > 2 ? ("/bin/" + ext.Substring(1)) : "";
            string args = string.Empty;
            byte saveCount = 0;
            Image cover = appInfo.DefaultCover;
            uint crc32 = rawRomData != null ? Shared.CRC32(rawRomData) : 0;
            string outputFileName = Regex.Replace(Path.GetFileName(inputFileName), @"[^A-Za-z0-9\.]+", "_").Trim();

            bool patched = false;
            if (!appInfo.Unknown)
            {
                application = appInfo.DefaultApps[0];
                var patch = appInfo.Class.GetMethod("Patch");
                if (patch != null)
                {
                    object[] values = new object[] { inputFileName, rawRomData, prefix, application, outputFileName, args, cover, saveCount, crc32 };
                    var result = (bool)patch.Invoke(null, values);
                    if (!result)
                        return null;

                    rawRomData = (byte[])values[1];
                    prefix = (char)values[2];
                    application = (string)values[3];
                    outputFileName = (string)values[4];
                    args = (string)values[5];
                    cover = (Image)values[6];
                    saveCount = (byte)values[7];
                    crc32 = (uint)values[8];
                    patched = true;
                }
            }

            // find ips patches if applicable
            if (!patched)
                FindPatch(ref rawRomData, inputFileName, crc32);

            // TODO : Make method that creates new app instead of using this discombobulated method

            // create directory and rom file
            var code = GenerateCode(crc32, prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var romPath = Path.Combine(gamePath, outputFileName);
            if (Directory.Exists(gamePath))
            {
                Shared.DirectoryDeleteInside(gamePath);
            }
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(romPath, rawRomData);

            var game = new NesApplication(gamePath, true);
            var name = Path.GetFileNameWithoutExtension(inputFileName);
            name = Regex.Replace(name, @" ?\(.*?\)", string.Empty).Trim();
            name = Regex.Replace(name, @" ?\[.*?\]", string.Empty).Trim();
            game.desktop.Name = name.Replace("_", " ").Replace("  ", " ").Trim();
            game.desktop.Exec = $"{application} {GamesHakchiPath}/{code}/{outputFileName} {args}";
            game.desktop.ProfilePath = GamesHakchiProfilePath;
            game.desktop.IconPath = GamesHakchiPath;
            game.desktop.Code = code;
            game.desktop.SaveCount = saveCount;
            game.Save();

            game = NesApplication.FromDirectory(gamePath);
            if (game is ICloverAutofill)
                (game as ICloverAutofill).TryAutofill(crc32);
            Debug.WriteLine("Icon: " + game.iconPath);
            Debug.WriteLine("Small Icon: " + game.smallIconPath);
            game.FindCover(inputFileName, cover, crc32);

            if (ConfigIni.Compress)
            {
                game.Compress();
                game.Save();
            }
            return game;
        }

        private static NesApplication ImportApp(string filename)
        {
            if (!File.Exists(filename) || Path.GetExtension(filename).ToLower() != ".desktop")
                throw new FileNotFoundException($"Invalid application file \"{filename}\"");
            var code = Path.GetFileNameWithoutExtension(filename).ToUpper();
            var targetDir = Path.Combine(GamesDirectory, code);
            Shared.DirectoryCopy(Path.GetDirectoryName(filename), targetDir, true, false, true, false);
            return FromDirectory(targetDir);
        }

        protected NesApplication() : base()
        {
            isOriginalGame = false;
            isDeleting = false;
        }

        protected NesApplication(string path, bool ignoreEmptyConfig = false)
            : base(path, ignoreEmptyConfig)
        {
            isOriginalGame = false;
            isDeleting = false;
            foreach (var og in DefaultGames)
            {
                if( og.Code == desktop.Code )
                {
                    isOriginalGame = true;
                    break;
                }
            }

            GameGeniePath = Path.Combine(path, GameGenieFileName);
            if (File.Exists(GameGeniePath))
                gameGenie = File.ReadAllText(GameGeniePath);
        }

        public override bool Save()
        {
            // safety when deleting
            if (IsDeleting)
                return false;

            // fix potentially problematic name and sort name
            desktop.Name = Regex.Replace(desktop.Name, @"'(\d)", @"`$1"); // Apostrophe + any number in game name crashes whole system. What. The. Fuck?
            desktop.SortName = Regex.Replace(desktop.SortName, @"'(\d)", @"`$1");
            if (string.IsNullOrEmpty(desktop.SortName))
            {
                string s = desktop.Name.ToLower();
                if (s.StartsWith("the "))
                    s = s.Substring(4); // Sorting without "THE"
                desktop.SortName = s;
            }

            // save .desktop file
            bool snesExtraFields = IsOriginalGame && (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom);
            desktop.Save($"{basePath}/{desktop.Code}.desktop", snesExtraFields);

            // game genie stuff
            if (!string.IsNullOrEmpty(gameGenie))
                File.WriteAllText(GameGeniePath, gameGenie);
            else if (File.Exists(GameGeniePath))
                File.Delete(GameGeniePath);

            return true;
        }

        public override Image Image
        {
            set
            {
                if (value == null)
                {
                    if (IsOriginalGame)
                    {
                        base.Image = null;
                        Save();
                    }
                    else
                    {
                        SetImage(AppInfo.DefaultCover, false);
                    }
                }
                else
                {
                    base.Image = value;
                    if (IsOriginalGame) Save();
                }
            }
            get
            {
                Image i = base.Image;
                if (IsOriginalGame && i == null)
                {
                    string cachedIconPath = Shared.PathCombine(OriginalGamesCacheDirectory, Code, Code + ".png");
                    return File.Exists(cachedIconPath) ? Shared.LoadBitmapCopy(cachedIconPath) : AppInfo.DefaultCover;
                }
                return i;
            }
        }

        public override Image Thumbnail
        {
            get
            {
                Image i = base.Thumbnail;
                if (IsOriginalGame && i == null)
                {
                    string cachedIconPath = Shared.PathCombine(OriginalGamesCacheDirectory, Code, Code + "_small.png");
                    return File.Exists(cachedIconPath) ? Image.FromFile(cachedIconPath) : AppInfo.DefaultCover;
                }
                return i;
            }
        }

        public bool FindCover(string inputFileName, Image defaultCover, uint crc32 = 0, string alternateTitle = null)
        {
            var artDirectory = Path.Combine(Program.BaseDirectoryExternal, "art");
            var imageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
            var name = Path.GetFileNameWithoutExtension(inputFileName);
            string[] covers;

            Directory.CreateDirectory(artDirectory);

            // first test for crc32 match (most precise)
            if (crc32 != 0)
            {
                covers = Directory.GetFiles(artDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                if (covers.Length > 0 && imageExtensions.Contains(Path.GetExtension(covers[0])))
                {
                    SetImageFile(covers[0], ConfigIni.CompressCover);
                    return true;
                }
            }

            // test presence of image file alongside the source file, if inputFileName is fully qualified
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(Path.GetDirectoryName(inputFileName)))
            {
                foreach (var ext in imageExtensions)
                {
                    var imagePath = Path.Combine(Path.GetDirectoryName(inputFileName), name + ext);
                    if (File.Exists(imagePath))
                    {
                        SetImageFile(imagePath, ConfigIni.CompressCover);
                        return true;
                    }
                }
            }

            // first fuzzy search on inputFileName
            covers = Directory.GetFiles(artDirectory, "*.*", SearchOption.AllDirectories);
            if (!string.IsNullOrEmpty(name))
            {
                string imageFile = FuzzyCoverSearch(name, covers);
                if (!string.IsNullOrEmpty(imageFile))
                {
                    SetImageFile(imageFile, ConfigIni.CompressCover);
                    return true;
                }
            }

            // second fuzzy search on alternateTitle
            if (!string.IsNullOrEmpty(alternateTitle))
            {
                string imageFile = FuzzyCoverSearch(alternateTitle, covers);
                if (!string.IsNullOrEmpty(imageFile))
                {
                    SetImageFile(imageFile, ConfigIni.CompressCover);
                    return true;
                }
            }

            // failed to find a cover, using default cover if provided
            if (defaultCover != null)
                SetImage(defaultCover);
            return false;
        }

        private static string FuzzyCoverSearch(string title, string[] covers)
        {
            var imageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
            var rgx = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
            string sanitizedTitle = rgx.Replace(title, string.Empty).ToLower();

            if (!string.IsNullOrEmpty(sanitizedTitle))
            {
                string matchFile = string.Empty;
                string sanitized = string.Empty;
                int matchDistance = 0;
                int distance = 0;

                foreach (var file in covers)
                {
                    if (imageExtensions.Contains(Path.GetExtension(file)))
                    {
                        sanitized = rgx.Replace(Path.GetFileNameWithoutExtension(file), string.Empty).ToLower();
                        if (MatchDistance(sanitizedTitle, sanitized, out distance))
                        {
                            if (distance > matchDistance)
                            {
                                matchDistance = distance;
                                matchFile = file;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(matchFile))
                {
                    Debug.WriteLine($"FuzzyCoverSearch matched \"{title}\" with \"{matchFile}\".");
                    return matchFile;
                }
            }
            return null;
        }

        private static bool MatchDistance(string a, string b, out int distance)
        {
            int maxLength = Math.Min(a.Length, b.Length);
            distance = 0;
            for (int c = 0; c < maxLength; ++c)
                if (a[c] == b[c])
                    ++distance;
                else
                    break;
            return (distance == maxLength);
        }

        protected static bool FindPatch(ref byte[] rawRomData, string inputFileName, uint crc32 = 0)
        {
            string patch = null;
            var patchesDirectory = Path.Combine(Program.BaseDirectoryExternal, "patches");
            Directory.CreateDirectory(patchesDirectory);
            if (!string.IsNullOrEmpty(inputFileName))
            {
                if (crc32 != 0)
                {
                    var patches = Directory.GetFiles(patchesDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                    if (patches.Length > 0)
                        patch = patches[0];
                }
                var patchesPath = Path.Combine(patchesDirectory, Path.GetFileNameWithoutExtension(inputFileName) + ".ips");
                if (File.Exists(patchesPath))
                    patch = patchesPath;
                patchesPath = Path.Combine(Path.GetDirectoryName(inputFileName), Path.GetFileNameWithoutExtension(inputFileName) + ".ips");
                if (File.Exists(patchesPath))
                    patch = patchesPath;
            }

            if (!string.IsNullOrEmpty(patch))
            {
                if (NeedPatch != true)
                {
                    if (NeedPatch != false)
                    {
                        var r = WorkerForm.MessageBoxFromThread(ParentForm,
                            string.Format(Resources.PatchQ, Path.GetFileName(inputFileName)),
                            Resources.PatchAvailable,
                            MessageBoxButtons.AbortRetryIgnore,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2, true);
                        if (r == DialogResult.Abort)
                            NeedPatch = true;
                        if (r == DialogResult.Ignore)
                            return false;
                    }
                    else return false;
                }
                IpsPatcher.Patch(patch, ref rawRomData); // TODO
                return true;
            }
            return false;
        }

        protected static string GenerateCode(uint crc32, char prefixCode)
        {
            return string.Format("CLV-{5}-{0}{1}{2}{3}{4}",
                (char)('A' + (crc32 % 26)),
                (char)('A' + (crc32 >> 5) % 26),
                (char)('A' + ((crc32 >> 10) % 26)),
                (char)('A' + ((crc32 >> 15) % 26)),
                (char)('A' + ((crc32 >> 20) % 26)),
                prefixCode);
        }

        public enum CopyMode { Standard, Sync, Export, LinkedExport }
        public NesApplication CopyTo(string path, CopyMode copyMode = CopyMode.Standard, bool pseudoLinks = false)
        {
            var targetDir = Path.Combine(path, desktop.Code);
            if (copyMode == CopyMode.Standard)
            {
                Shared.DirectoryCopy(basePath, targetDir, true, false, true, false);
                return FromDirectory(targetDir);
            }

            string relativeGamesPath = MediaHakchiPath + GamesDirectory.Substring(2).Replace("\\", "/");
            string relativeOriginalGamesPath = MediaHakchiPath + OriginalGamesCacheDirectory.Substring(2).Replace("\\", "/");

            // handle all 3 different sync scenarios (with original or custom games)
            string mediaGamePath = null, profilePath = null, iconPath = null;
            if (IsOriginalGame)
            {
                switch (copyMode)
                {
                    case CopyMode.Sync:
                        mediaGamePath = Shared.SquashFsPath + GamesSquashPath;
                        iconPath = File.Exists(this.iconPath) ? GamesHakchiPath : Shared.SquashFsPath + GamesSquashPath;
                        break;
                    case CopyMode.Export:
                        mediaGamePath = Directory.Exists(Path.Combine(OriginalGamesCacheDirectory, desktop.Code)) ? GamesHakchiPath : Shared.SquashFsPath + GamesSquashPath;
                        iconPath = File.Exists(this.iconPath) ? GamesHakchiPath : Shared.SquashFsPath + GamesSquashPath;
                        break;
                    case CopyMode.LinkedExport:
                        mediaGamePath = Directory.Exists(Path.Combine(OriginalGamesCacheDirectory, desktop.Code)) ? relativeOriginalGamesPath : Shared.SquashFsPath + GamesSquashPath;
                        iconPath = File.Exists(this.iconPath) ? relativeGamesPath : mediaGamePath;
                        break;
                }
            }
            else
            {
                mediaGamePath = iconPath = (copyMode == CopyMode.LinkedExport ? relativeGamesPath : GamesHakchiPath);
            }
            profilePath = GamesHakchiProfilePath;

            // debug stuff
            Debug.WriteLine($"Copying game \"{desktop.Name}\" into \"{path}\"");
            Debug.WriteLine($"mediaGamePath: {mediaGamePath}");
            Debug.WriteLine($"iconPath: {iconPath}");

            // copy to new target
            switch (copyMode)
            {
                case CopyMode.Sync:
                    Shared.DirectoryCopy(basePath, targetDir, true, false, true,
                        pseudoLinks && !(this is ISupportsGameGenie && File.Exists(this.GameGeniePath)),
                        new string[] {desktop.Code + ".desktop"});
                    break;

                case CopyMode.Export:
                    Shared.DirectoryCopy(basePath, targetDir, true, false, true, false);
                    if (Directory.Exists(Path.Combine(OriginalGamesCacheDirectory, desktop.Code)))
                    {
                        Shared.DirectoryCopy(Path.Combine(OriginalGamesCacheDirectory, desktop.Code), targetDir, true, true, false, false);
                    }
                    break;

                case CopyMode.LinkedExport:
                    Directory.CreateDirectory(targetDir);
                    if (Directory.Exists(Shared.PathCombine(OriginalGamesCacheDirectory, desktop.Code, "autoplay")))
                    {
                        Shared.DirectoryCopy(Shared.PathCombine(OriginalGamesCacheDirectory, desktop.Code, "autoplay"),
                            Path.Combine(targetDir, "autoplay"), true, false, true, false);
                    }
                    if (Directory.Exists(Shared.PathCombine(OriginalGamesCacheDirectory, desktop.Code, "pixelart")))
                    {
                        Shared.DirectoryCopy(Shared.PathCombine(OriginalGamesCacheDirectory, desktop.Code, "pixelart"),
                            Path.Combine(targetDir, "pixelart"), true, false, true, false);
                    }
                    break;
            }

            // copy adjusted desktop file to target
            DesktopFile newDesktop = (DesktopFile)desktop.Clone();
            foreach (var arg in newDesktop.Args)
            {
                Match m = Regex.Match(arg, @"(^\/.*)\/(?:" + newDesktop.Code + @"\/)([^.]*)(.*$)");
                if (m.Success)
                {
                    newDesktop.Exec = newDesktop.Exec.Replace(m.Groups[1].ToString(), mediaGamePath);
                    break;
                }
            }
            newDesktop.IconPath = iconPath;
            newDesktop.ProfilePath = profilePath;
            newDesktop.SaveTo(Path.Combine(targetDir, desktop.Code + ".desktop"));

            // return new app
            return FromDirectory(targetDir);
        }

        public static readonly string[] nonCompressibleExtensions = { ".7z", ".zip", ".hsqs", ".sh", ".pbp", ".chd" };
        public string[] CompressPossible()
        {
            if (!Directory.Exists(basePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(desktop.Exec, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(basePath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (nonCompressibleExtensions.Contains(Path.GetExtension(file).ToLower()))
                    continue;
                if (exec.Contains(" " + Path.GetFileName(file) + " ")) { 
                    if ((new FileInfo(file)).Length <= MaxCompress)
                        result.Add(file);
                }
            }
            return result.ToArray();
        }

        public string[] DecompressPossible()
        {
            if (!Directory.Exists(basePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(desktop.Exec, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(basePath, "*.7z", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (exec.Contains(" " + Path.GetFileName(file) + " "))
                    result.Add(file);
            }
            return result.ToArray();
        }

        public void Compress()
        {
            SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in CompressPossible())
            {
                var archName = filename + ".7z";
                var compressor = new SevenZipCompressor();
                compressor.CompressionLevel = CompressionLevel.High;
                Debug.WriteLine("Compressing " + filename);
                compressor.CompressFiles(archName, filename);
                File.Delete(filename);
                desktop.Exec = desktop.Exec.Replace(Path.GetFileName(filename), Path.GetFileName(archName));
            }
        }

        public void Decompress()
        {
            SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in DecompressPossible())
            {
                using (var szExtractor = new SevenZipExtractor(filename))
                {
                    Debug.WriteLine("Decompressing " + filename);
                    szExtractor.ExtractArchive(basePath);
                    foreach (var f in szExtractor.ArchiveFileNames)
                        desktop.Exec = desktop.Exec.Replace(Path.GetFileName(filename), f);
                }
                File.Delete(filename);
            }
        }

        public class NesAppEqualityComparer : IEqualityComparer<NesApplication>
        {
            public bool Equals(NesApplication x, NesApplication y)
            {
                return x.Code == y.Code;
            }

            public int GetHashCode(NesApplication obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}
