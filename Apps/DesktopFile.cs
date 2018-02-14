using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    class DesktopFile : INesMenuElement
    {
        private string type = "Application";
        public string Type
        {
            get;
        }

        private string exec = string.Empty;
        private string bin = string.Empty;
        private string[] args = null;
        public string Exec
        {
            get
            {
                return exec;
            }
            set
            {
                exec = value;
                bin = string.Empty;
                if (!string.IsNullOrEmpty(exec))
                {
                    args = exec.Split(' ');
                    if (args.Length > 0)
                    {
                        bin = args[0];
                        args = args.Skip(1).ToArray();
                    }
                }
                touch();
            }
        }
        public string Bin
        {
            get { return bin; }
            set
            {
                if (!string.IsNullOrEmpty(bin)) exec.Replace(bin, value);
                bin = value;
                touch();
            }
        }
        public string[] Args
        {
            get { return args; }
        }

        private string savePath = string.Empty;
        public string SavePath
        {
            get { return savePath; }
            set
            {
                savePath = string.Empty;
                if (!string.IsNullOrEmpty(value))
                    savePath = Regex.Replace(value, "CLV-.-[A-Z]{5}", "").Replace("//", "/");
                touch();
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                touch();
            }
        }

        private string iconPath = string.Empty;
        private string iconFilename = string.Empty;
        public string IconPath
        {
            get { return iconPath; }
            set
            {
                iconPath = string.Empty;
                iconFilename = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    iconPath = Regex.Replace(value, "CLV-.-[A-Z]{5}\\/CLV-.-[A-Z]{5}\\.(?:gif|png|jpg)", "").Replace("//", "/");
                    iconFilename = Regex.Match(value, "CLV-.-[A-Z]{5}\\.(?:gif|png|jpg)").ToString();
                }
                touch();
            }
        }
        public string IconFilename
        {
            get { return iconFilename; }
            set
            {
                iconFilename = value;
                touch();
            }
        }

        private string code = string.Empty;
        public string Code
        {
            get { return code; }
            set { Code = value; touch(); }
        }

        private byte testId = 0;
        public byte TestId
        {
            get { return testId; }
            set { testId = value; touch(); }
        }

        private string status = string.Empty;
        public string Status
        {
            get { return status; }
            set { status = value; touch(); }
        }

        private byte players = 1;
        public byte Players
        {
            get { return players; }
            set { players = value; touch(); }
        }

        private bool simultaneous = false;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set { simultaneous = value; touch(); }
        }

        private DateTime releaseDate = DateTime.Parse("1900-01-01");
        public string ReleaseDate
        {
            get
            {
                return releaseDate.ToString("yyyy-MM-dd");
            }
            set
            {
                releaseDate = DateTime.Parse(value);
                touch();
            }
        }

        private byte saveCount = 0;
        public byte SaveCount
        {
            get { return saveCount; }
            set { saveCount = value; touch(); }
        }

        private string sortRawTitle = string.Empty;
        public string SortName
        {
            get { return sortRawTitle; }
            set
            {
                sortRawTitle = value;
                touch();
            }
        }

        private string sortRawPublisher = "UNKNOWN";
        public string Publisher
        {
            get { return sortRawPublisher; }
            set { sortRawPublisher = value; touch(); }
        }

        private string copyright = "hakchi2 CE ©2017";
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; touch(); }
        }

        private bool hasUnsavedChanges = false;
        private void touch()
        {
            hasUnsavedChanges = true;
        }

        private string currentFilePath = null;
        public bool LoadFile(string configPath)
        {
            if (!File.Exists(configPath)) throw new FileNotFoundException();
            currentFilePath = configPath;

            string[] configLines = File.ReadAllLines(currentFilePath);
            foreach (string line in configLines)
            {
                int pos = line.IndexOf('=');
                if (pos <= 0)
                    continue;
                var param = line.Substring(0, pos).Trim().ToLower();
                var value = line.Substring(pos + 1).Trim();
                if (param.Length <= 0 || value.Length <= 0)
                    continue;

                switch (param)
                {
                    case "exec":
                        Exec = value;
                        break;
                    case "path":
                        SavePath = value;
                        break;
                    case "name":
                        Name = value;
                        break;
                    case "icon":
                        IconPath = value;
                        break;
                    case "code":
                        Code = value;
                        break;
                    case "testid":
                        TestId = byte.Parse(value);
                        break;
                    case "status":
                        Status = value;
                        break;
                    case "players":
                        Players = byte.Parse(value);
                        break;
                    case "simultaneous":
                        Simultaneous = bool.Parse(value);
                        break;
                    case "releasedate":
                        ReleaseDate = value;
                        break;
                    case "savecount":
                        SaveCount = byte.Parse(value);
                        break;
                    case "sortrawtitle":
                        SortName = value;
                        break;
                    case "sortrawpublisher":
                        Publisher = value;
                        break;
                    case "copyright":
                        Copyright = value;
                        break;
                }
            }

            hasUnsavedChanges = false;
            return true;
        }

        public void SaveFile(string configPath = null, bool snesExtraFields = false)
        {
            if (configPath == null) configPath = currentFilePath;
            currentFilePath = configPath;

            if (hasUnsavedChanges)
            {
                File.WriteAllText(currentFilePath,
                    $"[Desktop Entry]\n" +
                    $"Type={this.type}\n" +
                    $"Exec={this.exec}\n" +
                    $"Path={this.savePath}{this.code}\n" +
                    $"Name={this.name ?? this.code}\n" +
                    $"Icon={this.iconPath}{this.code}/{this.iconFilename}\n\n" +
                    $"[X-CLOVER Game]\n" +
                    $"Code={this.code}\n" +
                    $"TestID={this.testId}\n" +
                    (snesExtraFields ? $"Status={this.status}\n" : "") +
                    $"ID=0\n" +
                    $"Players={this.players}\n" +
                    $"Simultaneous={(this.simultaneous ? 1 : 0)}\n" +
                    $"ReleaseDate={this.releaseDate.ToString("yyyy-MM-dd")}\n" +
                    $"SaveCount={this.saveCount}\n" +
                    $"SortRawTitle={this.sortRawTitle}\n" +
                    $"SortRawPublisher={this.sortRawPublisher.ToUpper()}\n" +
                    $"Copyright={this.copyright}\n" +
                    (snesExtraFields ? $"MyPlayDemoTime=45\n" : ""));
                hasUnsavedChanges = false;
            }
        }
    }
}
