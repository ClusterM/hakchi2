using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace com.clusterrr.hakchi_gui.Manager
{
    public class RomManager
    {
        public string RomFolder = System.IO.Path.Combine(Program.BaseDirectoryExternal, "roms");
        public  delegate void RomModificationHandler (Rom modifiedRom);
        public event RomModificationHandler RomAdded;
        public class Rom
        {
            public override string ToString()
            {
                return DetectedName;
            }

            public Rom(string filePath)
            {
                LocalPath = filePath;
                

            }
            public string Size
            {
                get
                {
                    return Math.Round(((new System.IO.FileInfo(LocalPath)).Length / 1024.0 / 1024.0), 2).ToString() + " mB";
                }
            }
            public string Extension
            {
                get
                {
                    return System.IO.Path.GetExtension(LocalPath);
                }
            }
            public string LocalPath { get; set; }
         
            public string DetectedName
            {
                get
                {
                    string ret = System.IO.Path.GetFileNameWithoutExtension(LocalPath);
                    ret = Regex.Replace(ret, @" ?\(.*?\)", string.Empty).Trim();
                    ret = Regex.Replace(ret, @" ?\[.*?\]", string.Empty).Trim();
                    ret = ret.Replace("_", " ").Replace("  ", " ").Trim();
                    return ret;
                }
            
            }
        }
        private RomManager()
        {
            LoadLibrary();
        }
        public List<Rom> GetLibrary()
        {
            List<Rom> ret = new List<Rom>();
            ret.AddRange(_RomLibrary);
            return ret;
        }
        private void LoadLibrary()
        {
            string[] subFiles = System.IO.Directory.GetFiles(RomFolder, "*.*", System.IO.SearchOption.AllDirectories);
            foreach(string s in subFiles)
            {
                AddRom(s);
            }
        }
        private bool IsRomInLibrary(string localPath)
        {
            bool ret = false;
            foreach (Rom r in _RomLibrary)
            {
                if (localPath.ToLower() == r.LocalPath.ToLower())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public Rom GetRom(string filePath)
        {
            Rom ret = null;
           
            foreach (Rom r in _RomLibrary)
            {
                if (filePath.ToLower() == r.LocalPath.ToLower())
                {
                    ret = r;
                    break;
                }
            }
         /*   if(ret == null)
            {
                ret = AddRom(filePath);
            }
            */
            return ret;
        }
        public Rom AddRom(string filePath)
        {
            Rom ret = null;

            if (System.IO.File.Exists(filePath))
            {
                string destinationPath = System.IO.Path.Combine(RomFolder, System.IO.Path.GetExtension(filePath).Replace(".", "") + "\\" + System.IO.Path.GetFileName(filePath));
                string ext = System.IO.Path.GetExtension(filePath);
              //  if (EmulatorManager.getInstance().isFileValidRom(ext))
                {
                    if (!IsRomInLibrary(destinationPath))
                    {
                        if (destinationPath != filePath)
                        {
                            string folder = System.IO.Path.GetDirectoryName(destinationPath);
                            if (!System.IO.Directory.Exists(folder))
                            {
                                System.IO.Directory.CreateDirectory(folder);
                            }
                            if (System.IO.File.Exists(destinationPath))
                            {
                                System.IO.File.Delete(destinationPath);
                            }
                            System.IO.File.Copy(filePath, destinationPath);
                        }
                        ret = new Rom(destinationPath);

                        _RomLibrary.Add(ret);
                        if(RomAdded != null)
                        {
                            RomAdded(ret);
                        }
                    }
                    else
                    {
                        ret = GetRom(destinationPath);
                    }
                }
            }
            return ret;
        }

        private List<Rom> _RomLibrary = new List<Rom>();
        private static RomManager instance;
        public static RomManager getInstance()
        {
            if (instance == null)
            {
                instance = new RomManager();
            }
            return instance;
        }
    }
}
