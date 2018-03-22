using System.IO;

namespace com.clusterrr.util
{
    public class TrackableStream : MemoryStream
    {
        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnProgress = delegate { };
        public Stream InnerStream
        {
            get; private set;
        }

        public TrackableStream(Stream stream)
        {
            InnerStream = stream;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            InnerStream.Write(array, offset, count);
            OnProgress(this.Position, this.Length);
        }
        public override void WriteByte(byte value)
        {
            InnerStream.WriteByte(value);
            OnProgress(this.Position, this.Length);
        }
        public override int Read(byte[] array, int offset, int count)
        {
            var r = InnerStream.Read(array, offset, count);
            OnProgress(this.Position, this.Length);
            return r;
        }
        public override int ReadByte()
        {
            var r = InnerStream.ReadByte();
            OnProgress(this.Position, this.Length);
            return r;
        }
    }
}
