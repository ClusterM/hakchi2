using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class FdsGame : NesMiniApplication
    {
        protected const char prefixCode = 'D';

        public readonly string FdsPath;
        const string DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet --fds-auto-disk-side-switch-on-keypress";

        public string Args
        {
            get
            {
                if (Command.Contains(".fds"))
                    return Command.Substring(Command.IndexOf(".fds") + 4).Trim();
                else
                    return "";
            }
            set
            {
                Command = string.Format("/bin/clover-kachikachi-wr /usr/share/games/nes/kachikachi/{0}/{0}.fds {1}", code, value);
            }
        }  

        public FdsGame(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
            FdsPath = Path.Combine(GamePath, Code + ".fds");
            hasUnsavedChanges = false;
            Args = Args; // To update exec path if need
        }

        public static FdsGame Import(string fdsFileName, byte[] rawRomData = null)
        {
            if (rawRomData == null)
                rawRomData = File.ReadAllBytes(fdsFileName);
            var crc32 = CRC32(rawRomData);
            var code = GenerateCode(crc32, prefixCode);
            var gamePath = Path.Combine(GamesDirectory, code);
            var fdsPath = Path.Combine(gamePath, code + ".fds");
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(fdsPath, rawRomData);
            var game = new FdsGame(gamePath, true);

            game.Name = Path.GetFileNameWithoutExtension(fdsFileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ")/*.Replace(", The", "")*/.Trim();

            // Trying to find cover file
            Image cover = null;
            if (!string.IsNullOrEmpty(fdsFileName))
            {
                var imagePath = Path.Combine(Path.GetDirectoryName(fdsFileName), Path.GetFileNameWithoutExtension(fdsFileName) + ".png");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                imagePath = Path.Combine(Path.GetDirectoryName(fdsFileName), Path.GetFileNameWithoutExtension(fdsFileName) + ".jpg");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                var artDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "art");
                Directory.CreateDirectory(artDirectory);
                imagePath = Path.Combine(artDirectory, Path.GetFileNameWithoutExtension(fdsFileName) + ".png");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                imagePath = Path.Combine(artDirectory, Path.GetFileNameWithoutExtension(fdsFileName) + ".jpg");
                if (File.Exists(imagePath))
                    cover = LoadBitmap(imagePath);
                var covers = Directory.GetFiles(artDirectory, string.Format("{0:X8}*.*", crc32), SearchOption.AllDirectories);
                if (covers.Length > 0)
                    cover = LoadBitmap(covers[0]);
            }
            if (cover == null)
                cover = Resources.blank_fds;
            game.Image = cover;
            game.Args = DefaultArgs;
            game.Save();
            return game;
        }
    }
}

