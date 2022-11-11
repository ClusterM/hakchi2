using System.IO;
using System.Linq;

namespace com.clusterrr.hakchi_gui.Files
{
    class CheckFileType
    {
        public static bool IsTar(string filename)
        {
            using (var dataStream = File.OpenRead(filename))
            {
                return IsTar(dataStream);
            }
        }
        public static bool IsTar(Stream dataStream)
        {
            var fileMagic = new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72 };
            dataStream.Seek(257, SeekOrigin.Begin);
            var tempFileMagic = dataStream.ReadArray(5);
            bool output = tempFileMagic.SequenceEqual(fileMagic);
            return output;
        }

        public static bool IsSquashFs(string filename)
        {
            using (var dataStream = File.OpenRead(filename))
            {
                return IsSquashFs(dataStream);
            }
        }
        public static bool IsSquashFs(Stream dataStream)
        {
            var fileMagic = new byte[] { 0x68, 0x73, 0x71, 0x73 };
            bool output = dataStream.ReadArray(4).SequenceEqual(fileMagic);
            return output;
        }

        public static bool IsExtFs(string filename)
        {
            using (var dataStream = File.OpenRead(filename))
            {
                return IsExtFs(dataStream);
            }
        }
        public static bool IsExtFs(Stream dataStream)
        {
            dataStream.Seek(0x438, SeekOrigin.Begin);
            if (!dataStream.ReadArray(2).SequenceEqual(new byte[] { 0x53, 0xEF }))
                return false;

            dataStream.Seek(0x45c, SeekOrigin.Begin);
            int _45c = dataStream.ReadInt32();
            if ((_45c | 0x04) == 0x04) return true;

            if ((_45c & 0x04) == 0x04)
            {
                dataStream.Seek(0x460, SeekOrigin.Begin);
                int _460 = dataStream.ReadInt32();
                if (_460 < 0x40)
                {
                    dataStream.Seek(0x464, SeekOrigin.Begin);
                    int _464 = dataStream.ReadInt32();
                    if (_464 < 0x08 || _464 > 0x07) return true;
                }

                if (_460 > 0x3f) return true;
            }

            return false;
        }
    }
}
