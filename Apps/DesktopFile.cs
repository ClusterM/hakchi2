using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public class DesktopFile : INesMenuElement, ICloneable
    {
        private void setIf<T>(T newValue, ref T field)
        {
            if (!EqualityComparer<T>.Default.Equals(newValue, field))
            {
                field = newValue;
                hasUnsavedChanges = true;
            }
        }

        private string type = "Application";
        public string Type
        {
            get;
        }

        private string exec = string.Empty;
        private string bin = string.Empty;
        private string[] args = new string[0];
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
                        string tmp_exec = string.Copy(exec);
                        Match m = Regex.Match(tmp_exec, @"^[^\s]+(?:\s+-rom)*");
                        if (m.Success)
                        {
                            bin = m.Value;
                            tmp_exec = tmp_exec.Replace(bin, "");
                        }
                        args = tmp_exec.Length > 0 ? tmp_exec.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
                    }
                    else
                    {
                        args = new string[0];
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
                        exec = exec.Replace(bin, value);
                    bin = value;
                    hasUnsavedChanges = true;
                }
            }
        }
        public string[] Args
        {
            get { return args; }
        }

        private string profilePath = string.Empty;
        public string ProfilePath
        {
            get { return profilePath; }
            set { setIf(value, ref profilePath); }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { setIf(value, ref name); }
        }

        private string cePrefix = string.Empty;
        public string CePrefix
        {
            get { return cePrefix; }
            set { setIf(value, ref cePrefix); }
        }

        private string iconPath = string.Empty;
        private string iconFilename = string.Empty;
        public string IconPath
        {
            get { return iconPath; }
            set { setIf(value, ref iconPath); }
        }
        public string IconFilename
        {
            get { return iconFilename; }
            set { setIf(value, ref iconFilename); }
        }

        private string code = string.Empty;
        public string Code
        {
            get { return code; }
            set { setIf(value, ref code); }
        }

        private int testId = 0;
        public int TestId
        {
            get { return testId; }
            set { setIf(value, ref testId); }
        }

        private string status = string.Empty;
        public string Status
        {
            get { return status; }
            set { setIf(value, ref status); }
        }

        private byte players = 1;
        public byte Players
        {
            get { return players; }
            set { setIf(value, ref players); }
        }

        private bool simultaneous = false;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set { setIf(value, ref simultaneous); }
        }

        private string releaseDate = "1900-01-01";
        public string ReleaseDate
        {
            get { return releaseDate; }
            set { setIf(value, ref releaseDate); }
        }

        private byte saveCount = 0;
        public byte SaveCount
        {
            get { return saveCount; }
            set { setIf(value, ref saveCount); }
        }

        private string sortRawTitle = string.Empty;
        public string SortName
        {
            get { return sortRawTitle; }
            set { setIf(value, ref sortRawTitle); }
        }

        private string sortRawPublisher = "UNKNOWN";
        public string Publisher
        {
            get { return sortRawPublisher; }
            set { setIf(value, ref sortRawPublisher); }
        }

        private string copyright = "";
        public string Copyright
        {
            get { return copyright; }
            set { setIf(value, ref copyright); }
        }

        private string genre = string.Empty;
        public string Genre
        {
            get { return genre; }
            set { setIf(value, ref genre); }
        }

        private string index = string.Empty;
        public string Index
        {
            get { return index; }
            set { setIf(value, ref index); }
        }

        private string demoTime = string.Empty;
        public string DemoTime
        {
            get { return demoTime; }
            set { setIf(value, ref demoTime); }
        }

        private string country = string.Empty;
        public string Country
        {
            get { return country; }
            set { setIf(value, ref country); }
        }

        private string regionTag = string.Empty;
        public string RegionTag
        {
            get { return regionTag; }
            set { setIf(value, ref regionTag); }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { setIf(value, ref description); }
        }

        private string currentFilePath = null;
        public string CurrentFilePath
        {
            get;
        }

        private bool hasUnsavedChanges = false;

        public DesktopFile() : base() { }
        public DesktopFile(Stream configStream) : base() => Load(configStream);
        public bool Load(Stream configStream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(configStream))
                {
                    string line;
                    bool hitDescription = false;
                    var descriptionLines = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == "[Description]")
                        {
                            hitDescription = true;
                            if ((line = reader.ReadLine()) == "Text = ")
                                continue;
                        }
                            
                        if (hitDescription)
                        {
                            descriptionLines.Add(line);
                        }

                        else
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
                                case "ceprefix":
                                    CePrefix = value;
                                    break;
                                case "path":
                                    if (string.IsNullOrEmpty(value))
                                        ProfilePath = string.Empty;
                                    else
                                        try
                                        {
                                            ProfilePath = Path.GetDirectoryName(value).Replace("\\", "/").Replace("//", "/");
                                        }
                                        catch
                                        {
                                            System.Diagnostics.Trace.WriteLine($"Error parsing desktop file. Invalid profile path \"{value}\"");
                                            ProfilePath = string.Empty;
                                        }
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
                                        try
                                        {
                                            IconPath = Path.GetDirectoryName(Path.GetDirectoryName(value)).Replace("\\", "/").Replace("//", "/");
                                            IconFilename = Path.GetFileName(value);
                                        }
                                        catch
                                        {
                                            System.Diagnostics.Trace.WriteLine($"Error parsing desktop file. Invalid icon path \"{value}\"");
                                            IconPath = string.Empty;
                                            IconFilename = string.Empty;
                                        }
                                    }
                                    break;
                                case "code":
                                    Code = value;
                                    break;
                                case "testid":
                                    TestId = int.Parse(value);
                                    break;
                                case "status":
                                    Status = value;
                                    break;
                                case "players":
                                    Players = byte.Parse(value);
                                    break;
                                case "simultaneous":
                                    Simultaneous = (bool)(int.Parse(value) != 0);
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
                                case "sortrawgenre":
                                    Genre = value;
                                    break;
                                case "index":
                                    Index = value;
                                    break;
                                case "demo_time":
                                    DemoTime = value;
                                    break;
                                case "country":
                                    Country = value;
                                    break;
                                case "regiontag":
                                    RegionTag = value;
                                    break;
                            }
                        }
                    }
                    description = String.Join("\r\n", descriptionLines);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception: " + ex);
            }

            hasUnsavedChanges = false;
            return true;
        }

        public bool Load(string configPath)
        {
            if (!File.Exists(configPath)) throw new FileNotFoundException();
            currentFilePath = configPath;

            try
            {
                return Load(File.OpenRead(configPath));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception: " + ex);
            }
            return true;
        }

        public string GetConfig(bool snesExtraFields, bool omitProfilePathCode)
        {
            string content = (
                $"[Desktop Entry]\n" +
                $"Type={this.type}\n" +
                $"Exec={this.exec}\n" +
                $"Path={this.profilePath}{(omitProfilePathCode ? "" : "/" + this.code)}\n" +
                $"Name={this.name ?? this.code}\n" +
                $"CePrefix={this.cePrefix}\n" +
                $"Icon={this.iconPath}/{this.code}/{this.iconFilename}\n" +
                "\n" +
                $"[X-CLOVER Game]\n" +
                $"Code={this.code}\n" +
                $"TestID={this.testId}\n" +
                (snesExtraFields ? $"Status={this.status}\n" : "") +
                $"ID=0\n" +
                $"Players={this.players}\n" +
                $"Simultaneous={(this.simultaneous ? 1 : 0)}\n" +
                $"ReleaseDate={this.releaseDate}\n" +
                $"SaveCount={this.saveCount}\n" +
                $"SortRawTitle={this.sortRawTitle}\n" +
                $"SortRawPublisher={this.sortRawPublisher.ToUpper()}\n" +
                $"Copyright={this.copyright}\n" +
                (snesExtraFields ? $"MyPlayDemoTime=45\n" : "") +
                "\n" +
                "[m2engage]\n" +
                $"regionTag={this.regionTag}\n" +
                $"sortRawGenre={this.genre}\n" +
                $"index={this.index}\n" +
                $"demo_time={this.demoTime}\n" +
                $"country={this.country}\n" +
                "\n" +
                "[Description]\nText = \n" +
                this.description.Replace("\r\n", "\n").Trim());
            return content;
        }

        public Stream SaveTo(Stream stream, bool snesExtraFields = false, bool omitProfilePathCode = false)
        {
            if (!stream.CanWrite)
                throw new IOException("File stream is not writable");
            byte[] buffer = Encoding.UTF8.GetBytes(GetConfig(snesExtraFields, omitProfilePathCode));
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            return stream;
        }

        public void SaveTo(string configPath, bool snesExtraFields = false, bool omitProfilePathCode = false)
        {
            File.WriteAllText(configPath, GetConfig(snesExtraFields, omitProfilePathCode));
        }

        public void Save(string configPath = null, bool snesExtraFields = false, bool omitProfilePathCode = false)
        {
            if (configPath == null) configPath = currentFilePath;
            currentFilePath = configPath;

            if (hasUnsavedChanges)
            {
                if (currentFilePath == null)
                    throw new FileLoadException("No path give to save application " + name);

                System.Diagnostics.Trace.WriteLine(string.Format("Saving application \"{0}\" as {1}", name, code));
                SaveTo(currentFilePath, snesExtraFields, omitProfilePathCode);
                hasUnsavedChanges = false;
            }
        }

        public object Clone()
        {
            var newObject = new DesktopFile
            {
                Exec = string.Copy(exec),
                profilePath = string.Copy(profilePath),
                name = string.Copy(name),
                cePrefix = string.Copy(cePrefix),
                iconPath = string.Copy(iconPath),
                iconFilename = string.Copy(iconFilename),
                code = string.Copy(code),
                testId = testId,
                status = string.Copy(status),
                players = players,
                simultaneous = simultaneous,
                releaseDate = string.Copy(releaseDate),
                saveCount = saveCount,
                sortRawTitle = string.Copy(sortRawTitle),
                sortRawPublisher = string.Copy(sortRawPublisher),
                copyright = string.Copy(copyright),
                description = string.Copy(description),
                genre = string.Copy(genre),
                index = string.Copy(index),
                demoTime = string.Copy(demoTime),
                country = string.Copy(country),
                regionTag = string.Copy(regionTag)

            };
            return newObject;
        }
    }
}
