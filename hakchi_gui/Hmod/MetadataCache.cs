using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace com.clusterrr.hakchi_gui.Hmod
{
    public struct MetadataCache
    {
        public static int CurrentCacheVersion = 2;

        public int CacheVersion;
        public string Checksum;
        public DateTime LastModified;
        public string[][] ReadmeData;
        public string[][] LibretroInfo;
        public Dictionary<string, string> getReadmeDictionary()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (string[] item in ReadmeData)
            {
                data.Add(item[0], item[1]);
            }
            return data;
        }

        public MetadataCache(Dictionary<string, string> ReadmeData, Dictionary<string, string> LibretroInfo, string Checksum, DateTime LastModified)
        {
            this.CacheVersion = CurrentCacheVersion;
            this.Checksum = Checksum;
            this.LastModified = LastModified;

            // Take care of readme data
            {
                List<string[]> list = new List<string[]>();
                foreach (string key in ReadmeData.Keys)
                {
                    list.Add(new string[] { key, ReadmeData[key] });
                }
                this.ReadmeData = list.ToArray();
            }

            // Take care of libretro info
            {
                List<string[]> list = new List<string[]>();
                foreach (string key in LibretroInfo.Keys)
                {
                    list.Add(new string[] { key, LibretroInfo[key] });
                }
                this.LibretroInfo = list.ToArray();
            }
        }

        public static MetadataCache Deserialize(string xmlFile)
        {
            MetadataCache deserialized;

            using (StreamReader xmlStream = new StreamReader(xmlFile)) {
                XmlSerializer serializer = new XmlSerializer(typeof(MetadataCache));
                deserialized = (MetadataCache)serializer.Deserialize(xmlStream);
            }

            if (deserialized.CacheVersion != CurrentCacheVersion)
                throw new FormatException("Cache version does not match");

            return deserialized;

        }
    }
}
