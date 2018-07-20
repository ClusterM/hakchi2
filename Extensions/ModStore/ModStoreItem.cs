using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace com.clusterrr.hakchi_gui.module_library
{
    [XmlInclude(typeof(Module))]
    [XmlInclude(typeof(RACoreModule))]
    [XmlInclude(typeof(ModStoreGame))]
    public abstract class ModStoreItem
    {
        public string Id; // Need to develop some sort of website id system
        public string Name;
        public string Author;
        public string Path;
        public string Description;
        public string Version;
        public abstract string Type { get; }

        public InstalledModItem CreateInstalledItem()
        {
            return new InstalledModItem { Id = Id, Name = Name, Version = Version };
        }
    }

    public class InstalledModItem
    {
        public string Id;
        public string Name; //Temporary - incase of ID issues
        public string Version;
        public List<string> InstalledFiles = new List<string>();
    }
}
