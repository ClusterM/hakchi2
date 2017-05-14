using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace com.clusterrr.hakchi_gui.Manager
{
    public class SystemType
    {
        private static string FoldersXmlPath = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir), "SystemsDectection.txt");
        public class SystemDetectionEntry
        {
            public string Prefix { get; set; }
            public string Image { get; set; }
            public bool SupportZip { get; set; }
            public string SystemName { get; set; }
            public string Executable { get; set; }
            public List<string> Extensions { get; set; }
        }
        public static SystemType getInstance()
        {
            if(instance == null)
            {
                instance = new SystemType();
            }
            return instance;
        }
        private List<SystemDetectionEntry> systemDetections = new List<SystemDetectionEntry>();
        private static SystemType instance;
        private SystemType()
        {
            LoadSettings();
         
          //  SaveSettings();
        }
        private void LoadSettings()
        {
            if(System.IO.File.Exists(FoldersXmlPath))
            {
                systemDetections = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SystemDetectionEntry>>(System.IO.File.ReadAllText(FoldersXmlPath));
            }
        }
        public void SaveSettings()
        {
            System.IO.File.WriteAllText(FoldersXmlPath, Newtonsoft.Json.JsonConvert.SerializeObject(systemDetections,Newtonsoft.Json.Formatting.Indented));
        }
        public SystemDetectionEntry GetSystemInfo(NesMiniApplication game)
        {
            SystemDetectionEntry ret = null;
            List<Manager.SystemType.SystemDetectionEntry> execappliable = Manager.SystemType.getInstance().ListByExec(game.Command);
            if (execappliable.Count > 1)
            {
                /*Continue detection*/
                List<Manager.SystemType.SystemDetectionEntry> fileTypeAppliable = Manager.SystemType.getInstance().ListByFileType(game.RomFile, execappliable);
                if (fileTypeAppliable.Count == 0)
                {
                    //Cant detect file type...
                    List<Manager.SystemType.SystemDetectionEntry> byPrefix = Manager.SystemType.getInstance().ListByPrefix(game.Prefix, execappliable);
                    if (byPrefix.Count > 0)
                    {
                        //Take first one
                        ret = byPrefix[0];
                    }
                    else
                    {
                        ret = execappliable[0];
                    }

                }
                else
                {
                    if (fileTypeAppliable.Count == 1)
                    {
                        ret = fileTypeAppliable[0];
                    }
                    else
                    {
                        //Still multiple
                        List<Manager.SystemType.SystemDetectionEntry> byPrefix = Manager.SystemType.getInstance().ListByPrefix(game.Prefix, fileTypeAppliable);
                        if (byPrefix.Count > 0)
                        {
                            //Take first one
                            ret = byPrefix[0];
                        }
                        else
                        {
                            ret = fileTypeAppliable[0];
                        }
                    }
                }
            }
            else
            {
                if (execappliable.Count == 1)
                {
                    ret = execappliable[0];
                }
            }
            if(ret == null)
            {
                ret = AddBlank(game.RomFile);
            }
            return ret;
        }
        public SystemDetectionEntry AddBlank(string fileName)
        {
            SystemDetectionEntry sde = new SystemDetectionEntry();
            sde.Extensions = new List<string>();
            sde.Extensions.Add(System.IO.Path.GetExtension(fileName));
            if (sde.Extensions[0].Length != 0)
            {
                sde.Executable = "/bin/" + sde.Extensions[0].Substring(1);
            }
            else

            {
                sde.Executable = fileName;
            }
            sde.Image = "blank_app.png";
            sde.Prefix = "Z";
            sde.SupportZip = true;
            sde.SystemName = "Unknow";
            systemDetections.Add(sde);
            SaveSettings();
            return sde;
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByExec(string command)
        {
            return ListByExec(command, systemDetections);
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByExec(string command, List<Manager.SystemType.SystemDetectionEntry> subset)
        {
            List<Manager.SystemType.SystemDetectionEntry> execappliable = new List<Manager.SystemType.SystemDetectionEntry>();
            foreach (Manager.SystemType.SystemDetectionEntry e in subset)
            {
                if ((e.Executable != "" &&command.StartsWith(e.Executable)) || e.Executable == "" && command == "")
                {
                    execappliable.Add(e);
                }
            }
            return execappliable;
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByFileType(string fileName)
        {
            return ListByFileType(fileName, systemDetections);
        }
        public List<string> GetSupportedExtensions()
        {
            List<string> ret = new List<string>();

         
            foreach (Manager.SystemType.SystemDetectionEntry e in systemDetections)
            {
                ret.AddRange(e.Extensions);

            }
    
            return ret;
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByFileType(string fileName, List<Manager.SystemType.SystemDetectionEntry> subset)
        {
            List<Manager.SystemType.SystemDetectionEntry> fileTypeAppliable = new List<Manager.SystemType.SystemDetectionEntry>();
            foreach (Manager.SystemType.SystemDetectionEntry e in subset)
            {
                bool apply = false;
                foreach (string s in e.Extensions)
                {
                    if (fileName.EndsWith(s) || (e.SupportZip && fileName.EndsWith(".7z")))
                    {
                        apply = true;
                        break;
                    }
                }
                if (apply)
                {
                    fileTypeAppliable.Add(e);
                }

            }
            return fileTypeAppliable;
        }
        public bool isFileValidRom(string extension)
        {
            bool ret = false;
            foreach(SystemDetectionEntry sde in systemDetections)
            {
                foreach(string ext in sde.Extensions)
                {
                    if(ext == extension)
                    {
                        ret = true;
                        break;
                    }
                }
                if (ret)
                {
                    break;
                }
            }
            return ret;
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByPrefix(string prefix)
        {
            return ListByPrefix(prefix, systemDetections);
        }
        public List<Manager.SystemType.SystemDetectionEntry> ListByPrefix(string prefix, List<Manager.SystemType.SystemDetectionEntry> subset)
        {
            List<Manager.SystemType.SystemDetectionEntry> fileTypeAppliable = new List<Manager.SystemType.SystemDetectionEntry>();
            foreach (Manager.SystemType.SystemDetectionEntry e in subset)
            {
                
                if (e.Prefix == prefix )
                {
                    fileTypeAppliable.Add(e);
                }
                

            }
            return fileTypeAppliable;
        }
    }
}
