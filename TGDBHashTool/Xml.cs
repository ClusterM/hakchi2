using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TGDBHashTool
{
    public static class Xml
    {
        public static void Serialize<T>(Stream output, T data)
        {
            
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            XmlTextWriter myWriter = new XmlTextWriter(output, new UTF8Encoding(false))
            {
                Indentation = 1,
                IndentChar = '\t',
                Namespaces = false
            };

            mySerializer.Serialize(output, data);
            myWriter.Close();
        }
        public static T Deserialize<T>(Stream input)
        {
            var mySerializer = new XmlSerializer(typeof(T));
            var mySettings = new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Ignore
            };

            var myReader = XmlReader.Create(input, mySettings);
            return (T)mySerializer.Deserialize(myReader);
        }
    }
}
