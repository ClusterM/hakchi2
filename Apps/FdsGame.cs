﻿#pragma warning disable 0108
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public class FdsGame : NesMiniApplication
    {
        public const char Prefix = 'D';
        public static Image DefaultCover { get { return Resources.blank_fds; } }
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
            Args = Args; // To update exec path if need
        }

        public static FdsGame ImportFds(string fdsFileName, byte[] rawRomData = null)
        {
            if (rawRomData == null)
                rawRomData = File.ReadAllBytes(fdsFileName);
            if (Encoding.ASCII.GetString(rawRomData, 0, 3) == "FDS") // header? cut it!
            {
                var fdsDataNoHeader = new byte[rawRomData.Length - 0x10];
                Array.Copy(rawRomData, 0x10, fdsDataNoHeader, 0, fdsDataNoHeader.Length);
                rawRomData = fdsDataNoHeader;
            }
            var crc32 = CRC32(rawRomData);
            var code = GenerateCode(crc32, Prefix);
            var gamePath = Path.Combine(GamesDirectory, code);
            var fdsPath = Path.Combine(gamePath, code + ".fds");
            Directory.CreateDirectory(gamePath);
            File.WriteAllBytes(fdsPath, rawRomData);
            var game = new FdsGame(gamePath, true);

            game.Name = Path.GetFileNameWithoutExtension(fdsFileName);
            game.Name = Regex.Replace(game.Name, @" ?\(.*?\)", string.Empty).Trim();
            game.Name = Regex.Replace(game.Name, @" ?\[.*?\]", string.Empty).Trim();
            game.Name = game.Name.Replace("_", " ").Replace("  ", " ").Trim();
            game.FindCover(fdsFileName, Resources.blank_fds, crc32);
            game.Args = DefaultArgs;
            game.Save();
            return game;
        }
    }
}

