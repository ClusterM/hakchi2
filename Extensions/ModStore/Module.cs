using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.module_library
{
    public enum ModuleType { unknown = 0, hmod, compressedFile }
    public class Module : ModStoreItem
    {
        public List<string> Categories = new List<string>();
        public ModuleType ModType;

        // Set Module Type (returns true if successful)
        public bool SetModType()
        {
            var extention = Path.Substring(Path.LastIndexOf('.') + 1).ToLower();
            if (extention.Equals("hmod"))
                ModType = ModuleType.hmod;
            else if (extention.Equals("zip") || extention.Equals("7z") || extention.Equals("rar"))
                ModType = ModuleType.compressedFile;
            else
                ModType = ModuleType.unknown;

            return ModType != ModuleType.unknown;
        }

        public override string Type
        {
            get { return "Module"; }
        }
    }
}
