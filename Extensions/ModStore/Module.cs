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

        public override string Type
        {
            get { return "Module"; }
        }
    }
}
