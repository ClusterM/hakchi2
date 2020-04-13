using System.Collections.Generic;
using System.Xml.Serialization;

namespace TGDBHashTool.Models.Dat
{
    [XmlRoot(ElementName = "game")]
    public class DatGame
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlElement(ElementName = "description")]
        public string Description;

        [XmlElement(ElementName = "rom")]
        public List<DatRom> Roms = new List<DatRom>();
    }
}
