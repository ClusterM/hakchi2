using System.IO;

namespace com.clusterrr.hakchi_gui
{
    class EventStream : Stream
    {
        public delegate void OnDataEventHandler(byte[] buffer);
        public event OnDataEventHandler OnData = delegate { };

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;
        public override long Position { get => 0; set { } }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => 0;
        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count)
        {
            OnData?.Invoke(buffer);
        }
    }
}
