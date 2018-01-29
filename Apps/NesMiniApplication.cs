using com.clusterrr.hakchi_gui.Properties;
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

namespace com.clusterrr.hakchi_gui
{
    public class NesMiniApplication : INesMenuElement
    {
        internal const string DefaultApp = "/bin/path-to-your-app";
        const string DefaultReleaseDate = "1900-01-01";
        const string DefaultPublisher = "UNKNOWN";
        public const char DefaultPrefix = 'Z';
        public static Image DefaultCover = Resources.blank_app;
        public static Form ParentForm;
        public static bool? NeedPatch;
        public static bool? Need3rdPartyEmulator;
        public static bool? NeedAutoDownloadCover;

        public static NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros.", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2",Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros.", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga", Size =  25000 },
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN", Size =25000 },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL", Size =25000 },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge", Size = 25000 }
        };
        public static NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説", Size = 25000  },
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー" , Size = 25000    },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ" , Size = 25000   },
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎" , Size = 25000  },
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®", Size = 25000    },
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ", Size =  25000 },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語" , Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎", Size = 25000 }
        };
        public static NesDefaultGame[] defaultSnesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-SAAAE",  Name = "Super Mario World", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAABE",  Name = "F-ZERO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAEE",  Name = "The Legend of Zelda: A Link to the Past", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAFE",  Name = "Super Mario Kart", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAHE",  Name = "Super Metroid", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAJE",  Name = "EarthBound", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAKE",  Name = "Kirby's Dream Course", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAALE",  Name = "Donkey Kong Country", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAQE",  Name = "Kirby Super Star", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SAAXE",  Name = "Super Punch-Out!!", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABCE",  Name = "Mega Man X", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABDE",  Name = "Super Ghouls'n Ghosts", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABHE",  Name = "Street Fighter II Turbo: Hyper Fighting", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABQE",  Name = "Super Mario RPG: Legend of the Seven Stars", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABRE",  Name = "Secret of Mana", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SABTE",  Name = "Final Fantasy III", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SACBE",  Name = "Super Castlevania IV", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SACCE",  Name = "CONTRA III THE ALIEN WARS", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADGE",  Name = "Star Fox", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADJE",  Name = "Yoshi's Island", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-SADKE",  Name = "Star Fox 2", Size = 25000 }
        };
        public static NesDefaultGame[] defaultSuperFamicomGames = new NesDefaultGame[]
        {
            new NesDefaultGame { Code = "CLV-P-VAAAJ",  Name = "スーパーマリオワールド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAABJ",  Name = "F-ZERO", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAEJ",  Name = "ゼルダの伝説 神々のトライフォース", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAFJ",  Name = "スーパーマリオカート", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAGJ",  Name = "ファイアーエムブレム 紋章の謎", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAHJ",  Name = "スーパーメトロイド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAALJ",  Name = "スーパードンキーコング", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VAAQJ",  Name = "星のカービィ スーパーデラックス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABBJ",  Name = "スーパーストリートファイターⅡ ザ ニューチャレンジャーズ", Size = 24576 },
            new NesDefaultGame { Code = "CLV-P-VABCJ",  Name = "ロックマンX", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABDJ",  Name = "超魔界村", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABQJ",  Name = "スーパーマリオRPG", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABRJ",  Name = "聖剣伝説2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VABTJ",  Name = "ファイナルファンタジーVI", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VACCJ",  Name = "魂斗羅スピリッツ", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VACDJ",  Name = "がんばれゴエモン ゆき姫救出絵巻", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADFJ",  Name = "スーパーフォーメーションサッカー", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADGJ",  Name = "スターフォックス", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADJJ",  Name = "スーパーマリオ ヨッシーアイランド", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADKJ",  Name = "スターフォックス2", Size = 25000 },
            new NesDefaultGame { Code = "CLV-P-VADZJ",  Name = "パネルでポン", Size = 25000 },
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

        public static readonly NesDefaultGame[] AllDefaultGames = Program.ConcatArrays(
            defaultNesGames, defaultFamicomGames, defaultSnesGames, defaultSuperFamicomGames);

        public static readonly string OriginalGamesDirectory = Path.Combine(Program.BaseDirectoryExternal, "games_originals");
        public static string GamesDirectory
        {
            get
            {
                switch (ConfigIni.ConsoleType)
                {
                    default:
                    case MainForm.ConsoleType.NES:
                    case MainForm.ConsoleType.Famicom:
                        return System.IO.Path.Combine(Program.BaseDirectoryExternal, "games");
                    case MainForm.ConsoleType.SNES:
                    case MainForm.ConsoleType.SuperFamicom:
                        return System.IO.Path.Combine(Program.BaseDirectoryExternal, "games_snes");
                }
            }
        }
        public static string GamesCloverPath = "/var/games";

        protected string code;
        public string Code
        {
            get { return code; }
        }
        public virtual string GoogleSuffix
        {
            get { return "game"; }
        }


        public const string GameGenieFileName = "gamegenie.txt";
        public string GameGeniePath { private set; get; }
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

        public readonly string GamePath;
        public string GameFilePath
        {
            get
            {
                if (!Directory.Exists(GamePath)) return null;
                var exec = Regex.Replace(Command, "[/\\\"]", " ") + " ";
                var files = Directory.GetFiles(GamePath, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (exec.Contains(" " + Path.GetFileName(file) + " "))
                        return file;
                }
                return null;
            }
        }

        public readonly string ConfigPath;
        public readonly string IconPath;
        public readonly string SmallIconPath;
        protected string command;
        protected string cloverIconPath;
        protected bool hasUnsavedChanges = true;
        protected bool isDeleting = false;
        public bool Deleting
        {
            get { return isDeleting; }
            set
            {
                isDeleting = value;
                if (isDeleting)
                    hasUnsavedChanges = false;
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value) hasUnsavedChanges = true;
                name = value;
            }
        }
        public string Command
        {
            get { return command; }
            set
            {
                if (command != value) hasUnsavedChanges = true;
                command = value;
            }
        }
        private byte players;
        public byte Players
        {
            get { return players; }
            set
            {
                if (players != value) hasUnsavedChanges = true;
                players = value;
            }
        }
        private bool simultaneous;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set
            {
                if (simultaneous != value) hasUnsavedChanges = true;
                simultaneous = value;
            }
        }
        private string releaseDate;
        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                if (releaseDate != value) hasUnsavedChanges = true;
                releaseDate = value;
            }
        }
        private string publisher;
        public string Publisher
        {
            get { return publisher; }
            set
            {
                if (publisher != value) hasUnsavedChanges = true;
                publisher = value;
            }
        }
        private byte saveCount;
        public byte SaveCount
        {
            get { return saveCount; }
            set
            {
                if (saveCount != value) hasUnsavedChanges = true;
                saveCount = value;
            }
        }
        private uint testId;
        public uint TestId
        {
            get { return testId; }
            set
            {
                if (testId != value) hasUnsavedChanges = true;
                testId = value;
            }
        }
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value) hasUnsavedChanges = true;
                status = value;
            }
        }
        private string copyright;
        public string Copyright
        {
            get { return copyright; }
            set
            {
                if (copyright != value) hasUnsavedChanges = true;
                copyright = value;
            }
        }
        private string sortRawTitle;
        public string SortRawTitle
        {
            get { return sortRawTitle; }
            set
            {
                if (sortRawTitle != value) hasUnsavedChanges = true;
                sortRawTitle = value;
            }
        }
        public string SortName
        {
            get { return sortRawTitle; }
        }

        private bool isOriginalGame;
        public bool IsOriginalGame
        {
            get { return isOriginalGame; }
        }

        public static NesMiniApplication FromDirectory(string path, bool ignoreEmptyConfig = false)
        {
            (new DirectoryInfo(path)).Refresh();
            var files = Directory.GetFiles(path, "*.desktop", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new FileNotFoundException($"Invalid app folder: \"{path}\".");
            var config = File.ReadAllLines(files[0]);
            foreach (var line in config)
            {
                if (line.StartsWith("Exec="))
                {
                    string command = line.Substring(5);
                    var app = AppTypeCollection.GetAppByExec(command);
                    if (app != null)
                    {
                        var constructor = app.Class.GetConstructor(new Type[] { typeof(string), typeof(bool) });
                        return (NesMiniApplication)constructor.Invoke(new object[] { path, ignoreEmptyConfig });
                    }
                    break;
                }
            }
            return new NesMiniApplication(path, ignoreEmptyConfig);
        }

        public static NesMiniApplication Import(string inputFileName, string originalFileName = null, byte[] rawRomData = null)
        {
            var extension = System.IO.Path.GetExtension(inputFileName).ToLower();
            if (extension == ".desktop")
                return ImportApp(inputFileName);
            if (rawRomData == null) // Maybe it's already extracted data?
                rawRomData = File.ReadAllBytes(inputFileName); // If not, reading file
            if (originalFileName == null) // Original file name from archive
                originalFileName = System.IO.Path.GetFileName(inputFileName);
            char prefix = DefaultPrefix;
            string application = extension.Length > 2 ? ("/bin/" + extension.Substring(1)) : DefaultApp;
            string args = null;
            Image cover = DefaultCover;
            byte saveCount = 0;
            uint crc32 = CRC32(rawRomData);
            //string outputFileName = Regex.Replace(System.IO.Path.GetFileName(inputFileName), @"[^A-Za-z0-9()!\[\]\.\-]", "_").Trim();
            string outputFileName = Regex.Replace(System.IO.Path.GetFileName(inputFileName), @"[^A-Za-z0-9\.]+", "_").Trim();

            // Trying to determine file type
            var appinfo = AppTypeCollection.GetAppByExtension(extension);
            bool patched = false;
            if (appinfo != null)
            {
                if (appinfo.DefaultApps.Length > 0)
                    application = appinfo.DefaultApps[0];
                prefix = appinfo.Prefix;
                cover = appinfo.DefaultCover;
                var patch = appinfo.Class.GetMethod("Patch");
                if (patch != null)
                {
                    object[] values = new object[] { inputFileName, rawRomData, prefix, application, outputFileName, args, cover, saveCount, crc32 };
                    var result = (bool)patch.Invoke(null, values);
                    if (!result) return null;
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

            if (!patched)
                FindPatch(ref rawRomData, inputFileName, crc32);

            var code = GenerateCode(crc32, prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var romPath = Path.Combine(gamePath, outputFileName);
            if (Directory.Exists(gamePath))
            {
                var files = Directory.GetFiles(gamePath, "*.*", SearchOption.AllDirectories);
                foreach (var f in files)
                try
                {
                    File.Delete(f);
                }
                catch { }
            }
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(romPath, rawRomData);
            var game = new NesMiniApplication(gamePath, true);
            game.Name = System.IO.Path.GetFileNameWithoutExtension(inputFileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ").Trim();
            game.Command = $"{application} {GamesCloverPath}/{code}/{outputFileName}";
            if (!string.IsNullOrEmpty(args))
                game.Command += " " + args;
            game.FindCover(inputFileName, cover, crc32);
            game.SaveCount = saveCount;
            game.Save();

            var app = NesMiniApplication.FromDirectory(gamePath);
            if (app is ICloverAutofill)
                (app as ICloverAutofill).TryAutofill(crc32);

            if (ConfigIni.Compress)
            {
                app.Compress();
                app.Save();
            }

            return app;
        }

        private static NesMiniApplication ImportApp(string fileName)
        {
            if (!File.Exists(fileName)) // Archives are not allowed
                throw new FileNotFoundException("Invalid app folder");
            var code = System.IO.Path.GetFileNameWithoutExtension(fileName).ToUpper();
            var targetDir = System.IO.Path.Combine(GamesDirectory, code);
            DirectoryCopy(System.IO.Path.GetDirectoryName(fileName), targetDir, true);
            return FromDirectory(targetDir);
        }

        protected NesMiniApplication()
        {
            GamePath = null;
            ConfigPath = null;
            Players = 1;
            Simultaneous = false;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            Copyright = "hakchi2 ©2017 Alexey 'Cluster' Avdyukhin";
            Command = "";
            cloverIconPath = "";
            SaveCount = 0;
            Status = "";
            TestId = 0;
            isOriginalGame = false;
        }

        protected NesMiniApplication(string path, bool ignoreEmptyConfig = false)
        {
            GamePath = path;
            code = System.IO.Path.GetFileName(path);
            Name = Code;
            ConfigPath = System.IO.Path.Combine(path, Code + ".desktop");
            IconPath = System.IO.Path.Combine(path, Code + ".png");
            SmallIconPath = System.IO.Path.Combine(path, Code + "_small.png");
            Players = 1;
            Simultaneous = false;
            ReleaseDate = DefaultReleaseDate;
            Publisher = DefaultPublisher;
            Copyright = "hakchi2 ©2017 Alexey 'Cluster' Avdyukhin";
            Command = "";
            cloverIconPath = "";
            Status = "";
            TestId = 0;

            if (!File.Exists(ConfigPath))
            {
                if (ignoreEmptyConfig) return;
                throw new FileNotFoundException("Invalid application directory: " + path);
            }

            isOriginalGame = false;
            foreach(var game in DefaultGames)
            {
                if( game.Code == code )
                {
                    isOriginalGame = true;
                    break;
                }
            }

            var configLines = File.ReadAllLines(ConfigPath);
            foreach (var line in configLines)
            {
                int pos = line.IndexOf('=');
                if (pos <= 0) continue;
                var param = line.Substring(0, pos).Trim().ToLower();
                var value = line.Substring(pos + 1).Trim();
                switch (param)
                {
                    case "exec":
                        Command = value;
                        break;
                    case "name":
                        Name = value;
                        break;
                    case "players":
                        Players = byte.Parse(value);
                        break;
                    case "simultaneous":
                        Simultaneous = value != "0";
                        break;
                    case "releasedate":
                        ReleaseDate = value;
                        break;
                    case "sortrawtitle":
                        SortRawTitle = value;
                        break;
                    case "sortrawpublisher":
                        Publisher = value;
                        break;
                    case "copyright":
                        Copyright = value;
                        break;
                    case "savecount":
                        SaveCount = byte.Parse(value);
                        break;
                    case "status":
                        Status = value;
                        break;
                    case "testid":
                        TestId = uint.Parse(value);
                        break;
                    case "icon":
                        cloverIconPath = value;
                        break;
                }
            }

            GameGeniePath = Path.Combine(path, GameGenieFileName);
            if (File.Exists(GameGeniePath))
                gameGenie = File.ReadAllText(GameGeniePath);

            hasUnsavedChanges = false;
        }

        public virtual bool Save()
        {
            // safety when deleting
            if (isDeleting)
                return false;

            // check if a cover image might have been added to the original game directly in file system
            if (IsOriginalGame && ((cloverIconPath.StartsWith(GamesCloverPath) && !File.Exists(IconPath)) || (!cloverIconPath.StartsWith(GamesCloverPath) && File.Exists(IconPath))))
                hasUnsavedChanges = true;

            // only save if needed
            if (!hasUnsavedChanges)
                return false;
            Debug.WriteLine(string.Format("Saving application \"{0}\" as {1}", Name, Code));

            // setup name and sort name
            Name = Regex.Replace(Name, @"'(\d)", @"`$1"); // Apostrophe + any number in game name crashes whole system. What. The. Fuck?
            SortRawTitle = Name.ToLower();
            if (SortRawTitle.StartsWith("the "))
                SortRawTitle = SortRawTitle.Substring(4); // Sorting without "THE"

            // reference original icon path if no image exists for original game
            cloverIconPath = $"{GamesCloverPath}/{Code}/{Code}.png";
            if (IsOriginalGame && !File.Exists(IconPath))
                cloverIconPath = "/var/squashfs" + cloverIconPath;

            // these 2 lines are only present in snes/super famicom original games
            var statusLine = "";
            var lastLine = "";
            if (IsOriginalGame && (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom))
            {
                statusLine = $"Status={Status}\n";
                lastLine = "MyPlayDemoTime=45\n";
            }

            File.WriteAllText(ConfigPath, 
                $"[Desktop Entry]\n" +
                $"Type=Application\n" +
                $"Exec={command}\n" +
                $"Path=/var/lib/clover/profiles/0/{Code}\n" +
                $"Name={Name ?? Code}\n" +
                $"Icon={cloverIconPath}\n\n" +
                $"[X-CLOVER Game]\n" +
                $"Code={Code}\n" +
                $"TestID={TestId}\n" +
                statusLine +
                $"ID=0\n" +
                $"Players={Players}\n" +
                $"Simultaneous={(Simultaneous ? 1 : 0)}\n" +
                $"ReleaseDate={ReleaseDate ?? DefaultReleaseDate}\n" +
                $"SaveCount={SaveCount}\n" +
                $"SortRawTitle={SortRawTitle}\n" +
                $"SortRawPublisher={(Publisher ?? DefaultPublisher).ToUpper()}\n" +
                $"Copyright={Copyright}\n" +
                lastLine);

            // game genie stuff
            if (!string.IsNullOrEmpty(gameGenie))
                File.WriteAllText(GameGeniePath, gameGenie);
            else if (File.Exists(GameGeniePath))
                File.Delete(GameGeniePath);

            hasUnsavedChanges = false;
            return true;
        }

        public override string ToString()
        {
            return Name;
        }

        private void ProcessImage(Image inImage, string outPath, int targetWidth, int targetHeight, bool quantize)
        {
            int X, Y;
            if ((double)inImage.Width / (double)inImage.Height > (double)targetWidth / (double)targetHeight)
            {
                X = targetWidth;
                Y = (int)Math.Round((double)targetWidth * (double)inImage.Height / (double)inImage.Width);
                if (Y % 2 == 1) ++Y;
            }
            else
            {
                X = (int)Math.Round((double)targetHeight * (double)inImage.Width / (double)inImage.Height);
                if (X % 2 == 1) ++X;
                Y = targetHeight;
            }

            Bitmap outImage = new Bitmap(X, Y);
            using (Graphics gr = Graphics.FromImage(outImage))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                // Fix first line and column alpha shit
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    gr.DrawImage(inImage, new Rectangle(0, 0, outImage.Width, outImage.Height), 0, 0, inImage.Width, inImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
                gr.Flush();
                if (quantize)
                    Quantize(ref outImage);

                outImage.Save(outPath, ImageFormat.Png);
            }
            outImage.Dispose();
        }

        private void ProcessImageFile(string inPath, string outPath, int targetWidth, int targetHeight, bool quantize)
        {
            if (String.IsNullOrEmpty(inPath) || !File.Exists(inPath)) // failsafe
            {
                Debug.WriteLine($"ProcessImageFile: Image file \"{inPath}\" doesn't exist.");
                return;
            }

            // only file type candidate for direct copy is ".png"
            if (Path.GetExtension(inPath).ToLower() == ".png")
            {
                // if file is exactly the right aspect ratio, copy it
                Bitmap inImage = LoadBitmap(inPath);
                var pix = new PixelFormat[] { PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };
                if ((!quantize || pix.Contains(inImage.PixelFormat)) &&
                    ((inImage.Height == targetHeight && inImage.Width <= targetWidth) ||
                     (inImage.Width == targetWidth && inImage.Height <= targetHeight)))
                {
                    Debug.WriteLine($"ProcessImageFile: Image file \"{Path.GetFileName(inPath)}\" doesn't need resizing, kept intact!");
                    File.Copy(inPath, outPath, true);
                    return;
                }
            }

            // any other case, fully process image
            ProcessImage(LoadBitmap(inPath), outPath, targetWidth, targetHeight, quantize);
        }

        private void SetImage(Image img, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 204;
            int maxY = 204;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                maxX = 228;
                maxY = 204;
            }
            ProcessImage(img, IconPath, maxX, maxY, EightBitCompression);

            // thumbnail image ratio
            maxX = 40;
            maxY = 40;
            ProcessImage(img, SmallIconPath, maxX, maxY, EightBitCompression);
        }

        public void SetImageFile(string path, bool EightBitCompression = false)
        {
            // full-size image ratio
            int maxX = 204;
            int maxY = 204;
            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES || ConfigIni.ConsoleType == MainForm.ConsoleType.SuperFamicom)
            {
                maxX = 228;
                maxY = 204;
            }
            ProcessImageFile(path, IconPath, maxX, maxY, EightBitCompression);

            // check if a small image file might have accompanied the source image
            string thumbnailPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_small" + Path.GetExtension(path));
            if (File.Exists(thumbnailPath))
                path = thumbnailPath;

            // set thumbnail as well
            SetThumbnailFile(path, EightBitCompression);

            // save flag for original games
            if (IsOriginalGame)
                hasUnsavedChanges = true;
        }

        public void SetThumbnailFile(string path, bool EightBitCompression = false)
        {
            // thumbnail image ratio
            ProcessImageFile(path, SmallIconPath, 40, 40, EightBitCompression);
        }

        public Image Image
        {
            set
            {
                if (value == null)
                {
                    if (IsOriginalGame)
                    {
                        if (File.Exists(IconPath))
                        {
                            try
                            {
                                File.Delete(IconPath);
                                File.Delete(SmallIconPath);
                            }
                            catch { }
                            hasUnsavedChanges = true;
                        }
                    }
                    else
                    {
                        AppTypeCollection.AppInfo info = AppTypeCollection.GetAppByClass(GetType());
                        SetImage(info.DefaultCover);
                    }
                }
                else
                {
                    SetImage(value, ConfigIni.CompressCover);
                    if (IsOriginalGame)
                        hasUnsavedChanges = true;
                }
            }
            get
            {
                if (File.Exists(IconPath))
                    return LoadBitmap(IconPath);
                else
                {
                    if (IsOriginalGame)
                    {
                        string cachedIconPath = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "game_cache"), Path.Combine(Code, Code + ".png"));
                        if (File.Exists(cachedIconPath))
                            return LoadBitmap(cachedIconPath);
                        else
                            return DefaultCover;
                    }
                    return null;
                }
            }
        }

        public Image Thumbnail
        {
            get
            {
                if (File.Exists(SmallIconPath))
                    return LoadBitmap(SmallIconPath);
                else
                {
                    if (IsOriginalGame)
                    {
                        string cachedIconPath = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "game_cache"), Path.Combine(Code, Code + "_small.png"));
                        if (File.Exists(cachedIconPath))
                            return LoadBitmap(cachedIconPath);
                    }
                    return null;
                }
            }
        }

        private static void Quantize(ref Bitmap img)
        {
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
                ConvertTo32bppAndDisposeOriginal(ref img);

            try
            {
                var quantizer = new nQuant.WuQuantizer();
                Bitmap quantized = (Bitmap)quantizer.QuantizeImage(img);
                img.Dispose();
                img = quantized;
                Debug.WriteLine("an image has been compressed using nQuant.");
            }
            catch (nQuant.QuantizationException q)
            {
                Debug.WriteLine(q.Message);
            }
        }

        private static void ConvertTo32bppAndDisposeOriginal(ref Bitmap img)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            img.Dispose();
            img = bmp;
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
            var patchesDirectory = System.IO.Path.Combine(Program.BaseDirectoryExternal, "patches");
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
                patchesPath = Path.Combine(Path.GetDirectoryName(inputFileName), System.IO.Path.GetFileNameWithoutExtension(inputFileName) + ".ips");
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
                            string.Format(Resources.PatchQ, System.IO.Path.GetFileName(inputFileName)),
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
                IpsPatcher.Patch(patch, ref rawRomData);
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

        public NesMiniApplication CopyTo(string path, bool linkedGame = false, string mediaGamePath = null, string profilePath = null)
        {
            var targetDir = Path.Combine(path, code);

            if (linkedGame)
            {
                Directory.CreateDirectory(targetDir);
            }
            else
            {
                DirectoryCopy(GamePath, targetDir, true);
            }

            if (mediaGamePath != null || profilePath != null)
            {
                string desktopFile = File.ReadAllText($"{GamePath}\\{code}.desktop");
                if (mediaGamePath != null)
                {
                    // modified regex to only match when matching complete path (not within a longer path)
                    desktopFile = Regex.Replace(desktopFile, $"(([\\s=])(/usr/share/games/(nes/kachikachi/)?)|([\\s=])(/var/games/)){code}", "$2$5" + mediaGamePath);
                }
                if (profilePath != null)
                {
                    // match regular profile
                    desktopFile = Regex.Replace(desktopFile, @"^(Path=.*)$", "Path=" + profilePath, RegexOptions.Multiline);
                }
                File.WriteAllText(Path.Combine(targetDir, $"{code}.desktop"), desktopFile);
                return FromDirectory(targetDir);
            }

            return FromDirectory(targetDir);
        }

        internal static long DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            long size = 0;
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                size += file.CopyTo(temppath, true).Length;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    size += DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
            return size;
        }

        public long Size(string path = null)
        {
            if (path == null)
                path = GamePath;
            long size = 0;
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
                return 0;

            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                size += Size(subdir.FullName);
            }
            return size;
        }

        public static uint CRC32(byte[] data)
        {
            uint poly = 0xedb88320;
            uint[] table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                table[i] = temp;
            }
            uint crc = 0xffffffff;
            for (int i = 0; i < data.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ data[i]);
                crc = (uint)((crc >> 8) ^ table[index]);
            }
            return ~crc;
        }

        public static Bitmap LoadBitmap(string path)
        {
            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }
        }

        public static readonly string[] nonCompressibleExtensions = { ".7z", ".zip", ".hsqs", ".sh", ".pbp", ".chd" };
        public string[] CompressPossible()
        {
            if (!Directory.Exists(GamePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(Command, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(GamePath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (nonCompressibleExtensions.Contains(Path.GetExtension(file).ToLower()))
                    continue;
                if (exec.Contains(" " + Path.GetFileName(file) + " "))
                    result.Add(file);
            }
            return result.ToArray();
        }

        public string[] DecompressPossible()
        {
            if (!Directory.Exists(GamePath)) return new string[0];
            var result = new List<string>();
            var exec = Regex.Replace(Command, "[/\\\"]", " ") + " ";
            var files = Directory.GetFiles(GamePath, "*.7z", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (exec.Contains(" " + System.IO.Path.GetFileName(file) + " "))
                    result.Add(file);
            }
            return result.ToArray();
        }

        public void Compress()
        {
            SevenZipExtractor.SetLibraryPath(System.IO.Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in CompressPossible())
            {
                var archName = filename + ".7z";
                var compressor = new SevenZipCompressor();
                compressor.CompressionLevel = CompressionLevel.High;
                Debug.WriteLine("Compressing " + filename);
                compressor.CompressFiles(archName, filename);
                File.Delete(filename);
                Command = Command.Replace(System.IO.Path.GetFileName(filename), System.IO.Path.GetFileName(archName));
            }
        }

        public void Decompress()
        {
            SevenZipExtractor.SetLibraryPath(System.IO.Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            foreach (var filename in DecompressPossible())
            {
                using (var szExtractor = new SevenZipExtractor(filename))
                {
                    Debug.WriteLine("Decompressing " + filename);
                    szExtractor.ExtractArchive(GamePath);
                    foreach (var f in szExtractor.ArchiveFileNames)
                        Command = Command.Replace(System.IO.Path.GetFileName(filename), f);
                }
                File.Delete(filename);
            }
        }

        public class NesMiniAppEqualityComparer : IEqualityComparer<NesMiniApplication>
        {
            public bool Equals(NesMiniApplication x, NesMiniApplication y)
            {
                return x.Code == y.Code;
            }

            public int GetHashCode(NesMiniApplication obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}

