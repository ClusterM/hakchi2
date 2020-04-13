using System.Xml.Serialization;

namespace TGDBHashTool.Models.Dat
{
    [XmlRoot(ElementName = "rom")]
    public class DatRom
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlAttribute(AttributeName = "size")]
        public long Size;

        [XmlAttribute(AttributeName = "crc")]
        public string Crc32;

        [XmlAttribute(AttributeName = "md5")]
        public string Md5;

        [XmlAttribute(AttributeName = "sha1")]
        public string Sha1;

        public bool ShouldSerializeSize() => Size > 0;
    }
}
