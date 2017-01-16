using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public static class IpsPatcher
    {
        public static void Patch(string patchFile, string inFile, string outFile)
        {
            var patch = File.ReadAllBytes(patchFile);
            var data = File.ReadAllBytes(inFile);
            if (Encoding.ASCII.GetString(patch, 0, 5) != "PATCH") throw new Exception("Invalid IPS file");
            int pos = 5;
            while (pos < data.Length)
            {
                UInt32 address = (UInt32)(patch[pos + 2] | patch[pos + 1] * 0x100 | patch[pos] * 0x10000);
                if (pos + 3 >= patch.Length) break;
                UInt16 length = (UInt16)(patch[pos + 4] | patch[pos + 3] * 0x100);
                if (pos + 5 >= patch.Length) break;
                pos += 5;
                if (length > 0)
                {
                    while (length > 0)
                    {
                        data[address] = patch[pos];
                        address++;
                        pos++;
                        length--;
                    }
                }
                else
                {
                    length = (UInt16)(patch[pos + 1] | patch[pos] * 0x100);
                    var b = patch[pos + 2];
                    while (length > 0)
                    {
                        data[address] = b;
                        address++;
                        length--;
                    }
                    pos += 3;
                }
            }
            File.WriteAllBytes(outFile, data);
        }
    }
}
