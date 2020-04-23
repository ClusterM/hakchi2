using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataTool.Models.SimpleHashes
{
    [XmlRoot(ElementName = "romfiles")]
    public class RomFiles
    {
        [XmlElement(ElementName = "file")]
        public List<RomData> Hashes = new List<RomData>();
    }
}
