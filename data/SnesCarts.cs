using System;
using System.IO;
using System.Xml.Serialization;

namespace com.clusterrr.hakchi_gui.data
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Data")]
    public class SnesCarts
    {
        [XmlElementAttribute("Game")]
        public SnesCartData[] Game { get; set; }
        public static SnesCartData[] Deserialize()
        {
            var serializer = new XmlSerializer(typeof(SnesCarts));
            SnesCarts carts = null;

            using (var reader = new FileStream(Path.Combine(Program.BaseDirectoryInternal, "data", "snescarts.xml"), FileMode.Open))
            {
                carts = (SnesCarts)serializer.Deserialize(reader);
            }
            return carts.Game;
        }
    }

    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class SnesCartData
    {

        [XmlElement("api_id")]
        public int ApiId { get; set; }

        [XmlElementAttribute("cover")]
        public string Cover { get; set; }

        [XmlIgnore]
        public UInt32 Crc32 { get; set; }

        [XmlElementAttribute("crc")]
        public string Crc32Hex { 
            get => Crc32.ToString("x8"); 
            set => Crc32 = UInt32.Parse($"{value}", System.Globalization.NumberStyles.HexNumber); 
        }

        [XmlElementAttribute("date")]
        public string Date { get; set; }

        [XmlElementAttribute("name")]
        public string Name { get; set; }

        [XmlElementAttribute("players")]
        public byte Players { get; set; }

        [XmlElementAttribute("publisher")]
        public string Publisher { get; set; }

        [XmlElementAttribute("region")]
        public string Region { get; set; }

        [XmlIgnore]
        public bool Simultaneous { get; set; }

        [XmlElementAttribute("simultaneous", typeof(byte))]
        public byte SimultaneousByte { 
            get => (byte)(Simultaneous ? 1 : 0);
            set => Simultaneous = value == 1;
        }        
    }
}
