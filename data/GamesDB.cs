using DataTool.Models.SimpleHashes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui.data
{
    class GamesDB
   {
        public static string XmlFile
        {
            get
            {
                var path = Path.Combine(Program.BaseDirectoryExternal, "data", "romfiles.xml");

                if (File.Exists(path))
                {
                    return path;
                }

                return Path.Combine(Program.BaseDirectoryInternal, "data", "romfiles.xml");
            }
        }
        private static Regex Crc32Regex = new Regex("^[a-f0-9]{8}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Dictionary<UInt32, RomData> _HashLookup = null;
        public static IReadOnlyDictionary<UInt32, RomData> HashLookup
        {
            get
            {
                if (_HashLookup == null)
                {
                    _HashLookup = new Dictionary<UInt32, RomData>();
                    var data = XMLSerialization.DeserializeXMLFileToObject<RomFiles>(XmlFile);
                    foreach (var rom in data.Hashes)
                    {
                        UInt32 hash = 0;
                        if (rom.Crc32 != null && Crc32Regex.IsMatch(rom.Crc32, 0) && UInt32.TryParse(rom.Crc32, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hash) && !_HashLookup.ContainsKey(hash))
                        {
                            _HashLookup[hash] = rom;
                        }
                    }
                }

                return _HashLookup;
            }
        }
   }
}