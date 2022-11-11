using System;
using System.IO;

namespace com.clusterrr.hakchi_gui.Files
{
    public static class Extensions
    {
        public static byte[] ReadArray(this Stream dataStream, int count)
        {
            byte[] buffer = new byte[count];
            dataStream.Read(buffer, 0, count);
            return buffer;
        }
        public static Int16 ReadInt16(this Stream dataStream, bool littleEndian = true)
        {
            var data = dataStream.ReadArray(2);
            if (BitConverter.IsLittleEndian != littleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt16(data, 0);
        }
        public static UInt16 ReadUInt16(this Stream dataStream, bool littleEndian = true)
        {
            var data = dataStream.ReadArray(2);
            if (BitConverter.IsLittleEndian != littleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }
        public static Int32 ReadInt32(this Stream dataStream, bool littleEndian = true)
        {
            var data = dataStream.ReadArray(4);
            if (BitConverter.IsLittleEndian != littleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt32(data, 0);
        }
        public static UInt32 ReadUInt32(this Stream dataStream, bool littleEndian = true)
        {
            var data = dataStream.ReadArray(4);
            if (BitConverter.IsLittleEndian != littleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt32(data, 0);
        }
    }
}
