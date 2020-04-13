using System.Xml.Serialization;

namespace TGDBHashTool.Models.Dat
{
    [XmlRoot(ElementName = "header")]
    public class DatHeader
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "description")]
        public string Description;

        [XmlElement(ElementName = "version")]
        public string Version;

        [XmlElement(ElementName = "author")]
        public string Author;

        [XmlElement(ElementName = "homepage")]
        public string Homepage;

        [XmlElement(ElementName = "url")]
        public string Url;
    }
}
