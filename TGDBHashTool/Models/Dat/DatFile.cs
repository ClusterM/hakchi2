using System.Collections.Generic;
using System.Xml.Serialization;

namespace TGDBHashTool.Models.Dat
{
    [XmlRoot(ElementName = "datafile")]
    public class DatFile
    {
        [XmlElement(ElementName = "header")]
        public DatHeader Header = new DatHeader();

        [XmlElement(ElementName = "game")]
        public List<DatGame> Games = new List<DatGame>();
    }
}
