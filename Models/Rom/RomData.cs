using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTool.Models.SimpleHashes
{
    [XmlRoot(ElementName = "file")]
    public class RomData
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlAttribute(AttributeName = "crc32")]
        public string Crc32;

        [XmlElement(ElementName = "tgdb")]
        public List<int> TgdbId = new List<int>();
    }
}
