using System.Collections.Generic;
using System.Xml.Serialization;
using TGDBHashTool.Models.Dat;

namespace TGDBHashTool.Models.Data
{
    [XmlRoot(ElementName = "group")]
    public class DataGroup
    {
        [XmlIgnore]
        public string Name 
        {
            get {
                var output = Header.Name?.Trim() ?? "";
                if (Header.Version != null && Header.Version.Trim().Length > 0)
                {
                    output += $" ({Header.Version.Trim()})";
                }
                return output;
            }
        }

        [XmlElement(ElementName = "header")]
        public DatHeader Header = new DatHeader();

        [XmlElement(ElementName = "game")]
        public List<DataGame> Games = new List<DataGame>();
    }
}
