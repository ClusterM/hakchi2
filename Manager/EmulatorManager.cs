using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace com.clusterrr.hakchi_gui.Manager
{
    public class EmulatorManager
    {
        private static string FoldersXmlPath = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir), "Emulators.txt");
        public class Emulator
        {
            public override string ToString()
            {
                return Name;
            }
            public bool NeedRomParameter { get; set; }
            public string SystemName { get; set; }
            public string Name { get; set; }
            public bool SupportZip { get; set; }
            public string DefaultImage { get; set; }
            public string Prefix { get; set; }
            public string Executable { get; set; }
            public List<string> AvailableArguments { get;set; }
            public List<string> Extensions { get; set; }

            public string getCommandLine(NesMiniApplication app)
            {
                if (NeedRomParameter)
                {
                    return (Executable + " " + app.NesClassicRomPath + " " + app.Arguments).Trim();
                }
                else
                {
                    return (Executable + " " + app.Arguments).Trim();
                }
            }
        }
        public static EmulatorManager getInstance()
        {
            if (instance == null)
            {
                instance = new EmulatorManager();
            }
            return instance;
        }
        private List<Emulator> emulators = new List<Emulator>();
        private static EmulatorManager instance;
        public List<Emulator> getEmulatorList()
        {
            List<Emulator> ret = new List<Emulator>();
            ret.AddRange(emulators);
            return ret;
        }
        private EmulatorManager()
        {
            LoadSettings();
            
            //  SaveSettings();
        }
        private void LoadSettings()
        {
            if (System.IO.File.Exists(FoldersXmlPath))
            {
                emulators = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Emulator>>(System.IO.File.ReadAllText(FoldersXmlPath));
            }
        }
        public void SaveSettings()
        {
            System.IO.File.WriteAllText(FoldersXmlPath, Newtonsoft.Json.JsonConvert.SerializeObject(emulators, Newtonsoft.Json.Formatting.Indented));
        }
        public Emulator GetEmulator(NesMiniApplication game)
        {
            Emulator ret = null;
            foreach(Emulator emu in emulators)
            {
                if(game.CommandWithoutArguments == emu.Executable)
                {
                    ret = emu;
                    
                    if(game.Arguments != "" && !ret.AvailableArguments.Contains(game.Arguments))
                    {
                        ret.AvailableArguments.Add(game.Arguments);
                        SaveSettings();
                    }
                    break;
                }
            }
            if(ret == null)
            {
                ret = getUnknowEmulator(game);
            }


            
            return ret;
        }
        private Emulator getUnknowEmulator(string fileName)
        {
            Emulator emu = new Emulator();
            emu.Extensions = new List<string>();
           
            emu.AvailableArguments = new List<string>();
            string exeName = fileName;
            if(System.IO.Path.GetExtension(fileName) !="")
            {
                emu.Extensions.Add(System.IO.Path.GetExtension(fileName));
                exeName = System.IO.Path.GetExtension(fileName).Substring(1);
            }
            emu.Executable = "/bin/" + exeName;

            if (emu.Extensions.Count > 0)
            {
                emu.SystemName = "Unknow / " + exeName;
                emu.NeedRomParameter = true;
            }
            else
            {
                emu.SystemName = "Application";
                emu.NeedRomParameter = false;
            }

            emu.Name = emu.SystemName;
            emu.DefaultImage = "blank_"+exeName+".png";
            emu.Prefix = "Z";
            emu.SupportZip = true;
            emulators.Add(emu);
            SaveSettings();
            return emu;
        }
        private Emulator getUnknowEmulator(NesMiniApplication game)
        {
            Emulator emu = new Emulator();
            emu.Extensions = new List<string>();
            emu.Extensions.Add(System.IO.Path.GetExtension(game.RomFile));
            emu.AvailableArguments = new List<string>();
            
            if((game.Command == null || game.Command == "" )&& emu.Extensions[0].Trim() != "")
            {
                emu.Executable = "/bin/" + emu.Extensions[0].Substring(1);
            }
            else
            {
                if (game.Executable.Trim() != "")
                {
                    emu.Executable = game.Executable;
                }
                else
                {
                    emu.Executable = game.Command;
                }
            }
            if(game.Arguments.Trim() !="")
            {
                emu.AvailableArguments.Add(game.Arguments);
            }
            if(game.NesClassicRomPath.Trim() =="")
            {
                emu.SystemName = "Application";
                emu.NeedRomParameter = false;
            }
            else
            {
                emu.SystemName = "Unknow";
                emu.NeedRomParameter = true;
            }
            emu.Name = emu.Executable;
            string exeName = "app";
            if(emu.Executable.StartsWith("/bin/"))
            {
                exeName = emu.Executable.Substring(5);
            }
            emu.DefaultImage = "blank_"+exeName+".png";
            emu.Prefix = "Z";
            emu.SupportZip = true;
            emulators.Add(emu);
            SaveSettings();
            return emu;

        }
        public bool isFileValidRom(string extension)
        {
            bool ret = false;
            foreach (Emulator sde in emulators)
            {
                foreach (string ext in sde.Extensions)
                {
                    if (ext.ToLower() == extension.ToLower())
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
        public List<string> GetSupportedExtensions()
        {
            List<string> ret = new List<string>();


            foreach (Emulator e in emulators)
            {
                ret.AddRange(e.Extensions);

            }

            return ret;
        }
        public List<Emulator> ListByFileType(string fileName)
        {
            List<Emulator> fileTypeAppliable = new List<Emulator>();
            foreach (Emulator e in emulators)
            {
                bool apply = false;
                foreach (string s in e.Extensions)
                {
                    if (s != "")
                    {
                        if (fileName.EndsWith(s) || (e.SupportZip && fileName.EndsWith(".7z")))
                        {
                            apply = true;
                            break;
                        }
                    }
                }
                if (apply)
                {
                    fileTypeAppliable.Add(e);
                }

            }
            if(fileTypeAppliable.Count==0)
            {
                fileTypeAppliable.Add(getUnknowEmulator(fileName));
            }
            return fileTypeAppliable;
        }
    }
}
