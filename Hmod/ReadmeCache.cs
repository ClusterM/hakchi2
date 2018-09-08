using System;
using System.Collections.Generic;

namespace com.clusterrr.hakchi_gui.Hmod
{
    public struct ReadmeCache
    {
        public string Checksum;
        public DateTime LastModified;
        public string[][] ReadmeData;
        public Dictionary<string, string> getReadmeDictionary()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (string[] item in ReadmeData)
            {
                data.Add(item[0], item[1]);
            }
            return data;
        }

        public ReadmeCache(Dictionary<string, string> ReadmeData, string Checksum, DateTime LastModified)
        {
            List<string[]> list = new List<string[]>();
            foreach (string key in ReadmeData.Keys)
            {
                list.Add(new string[] { key, ReadmeData[key] });
            }
            this.ReadmeData = list.ToArray();
            this.Checksum = Checksum;
            this.LastModified = LastModified;
        }
    }
}
