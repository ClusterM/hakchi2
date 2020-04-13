using System.Collections.Generic;
using System.Xml.Serialization;

namespace TGDBHashTool.Models.Data
{
    [XmlRoot(ElementName = "collection")]
    public class DataCollection
    {
        [XmlElement(ElementName = "datafile")]
        public List<DataGroup> Groups = new List<DataGroup>();
    }
}
