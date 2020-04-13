using System.Collections.Generic;
using System.Xml.Serialization;
using TGDBHashTool.Models.Dat;

namespace TGDBHashTool.Models.Data
{
    [XmlRoot(ElementName = "game")]
    public class DataGame: DatGame
    {
        [XmlElement(ElementName = "tgdb")]
        public List<int> TgdbId = new List<int>();
    }
}
