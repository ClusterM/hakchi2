using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class DesktopFile : INesMenuElement
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
                if (exec != value)
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
                    hasUnsavedChanges = true;
                }
            }
        }
        public string Bin
        {
            get { return bin; }
            set
            {
                if (bin != value)
                {
                    if (!string.IsNullOrEmpty(bin))
                        exec.Replace(bin, value);
                    bin = value;
                    hasUnsavedChanges = true;
                }
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
                if (savePath != value)
                {
                    savePath = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string iconPath = string.Empty;
        private string iconFilename = string.Empty;
        public string IconPath
        {
            get { return iconPath; }
            set
            {
                if (iconPath != value)
                {
                    iconPath = value;
                    hasUnsavedChanges = true;
                }
            }
        }
        public string IconFilename
        {
            get { return iconFilename; }
            set
            {
                if (iconFilename != value)
                {
                    iconFilename = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string code = string.Empty;
        public string Code
        {
            get { return code; }
            set
            {
                if (code != value)
                {
                    code = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private byte testId = 0;
        public byte TestId
        {
            get { return testId; }
            set
            {
                if (testId != value)
                {
                    testId = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string status = string.Empty;
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private byte players = 1;
        public byte Players
        {
            get { return players; }
            set
            {
                if (players != value)
                {
                    players = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private bool simultaneous = false;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set
            {
                if (simultaneous != value)
                {
                    simultaneous = value;
                    hasUnsavedChanges = true;
                }
            }
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
                if (releaseDate != DateTime.Parse(value))
                {
                    releaseDate = DateTime.Parse(value);
                    hasUnsavedChanges = true;
                }
            }
        }

        private byte saveCount = 0;
        public byte SaveCount
        {
            get { return saveCount; }
            set
            {
                if (saveCount != value)
                {
                    saveCount = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string sortRawTitle = string.Empty;
        public string SortName
        {
            get { return sortRawTitle; }
            set
            {
                if (sortRawTitle != value)
                {
                    sortRawTitle = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string sortRawPublisher = "UNKNOWN";
        public string Publisher
        {
            get { return sortRawPublisher; }
            set
            {
                if (sortRawPublisher != value)
                {
                    sortRawPublisher = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private string copyright = "hakchi2 CE ©2017";
        public string Copyright
        {
            get { return copyright; }
            set
            {
                if (copyright != value)
                {
                    copyright = value;
                    hasUnsavedChanges = true;
                }
            }
        }

        private bool hasUnsavedChanges = false;

        private string currentFilePath = null;
        public string CurrentFilePath
        {
            get;
        }

        public bool Load(string configPath)
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
                if (param.Length <= 0)
                    continue;

                switch (param)
                {
                    case "exec":
                        Exec = value;
                        break;
                    case "path":
                        if (string.IsNullOrEmpty(value))
                            SavePath = string.Empty;
                        else
                            SavePath = Regex.Replace(value, "CLV-.-[A-Z]{5}", "").Replace("//", "/");
                        break;
                    case "name":
                        Name = value;
                        break;
                    case "icon":
                        if (string.IsNullOrEmpty(value))
                        {
                            IconPath = string.Empty;
                            IconFilename = string.Empty;
                        }
                        else
                        {
                            Match m = Regex.Match(value, "CLV-.-[A-Z]{5}\\.(?:gif|png|jpg)");

                            IconPath = Regex.Replace(value, "CLV-.-[A-Z]{5}\\/CLV-.-[A-Z]{5}\\.(?:gif|png|jpg)", "").Replace("//", "/");
                            IconFilename = m.Success ? m.ToString() : string.Empty;
                        }
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

        public void CopyTo(string configPath, bool snesExtraFields = false, bool omitSavePathCode = false)
        {
            File.WriteAllText(configPath,
                $"[Desktop Entry]\n" +
                $"Type={this.type}\n" +
                $"Exec={this.exec}\n" +
                $"Path={this.savePath}{(omitSavePathCode ? "" : this.code)}\n" +
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
        }

        public void Save(string configPath = null, bool snesExtraFields = false, bool omitSavePathCode = false)
        {
            if (configPath == null) configPath = currentFilePath;
            currentFilePath = configPath;

            if (hasUnsavedChanges)
            {
                CopyTo(currentFilePath, snesExtraFields, omitSavePathCode);
                hasUnsavedChanges = false;
            }
        }
    }
}
