using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.util
{
    class TrackableMemoryStream : MemoryStream
    {
        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnProgress = delegate { };
        public event OnProgressDelegate OnReadProgress = delegate { };
        public event OnProgressDelegate OnWriteProgress = delegate { };

        public TrackableMemoryStream() : base() { }
        public TrackableMemoryStream(byte[] buffer) : base(buffer) { }

        public override void Write(byte[] array, int offset, int count)
        {
            base.Write(array, offset, count);
            OnProgress(this.Position, this.Length);
            OnWriteProgress(this.Position, this.Length);
        }
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            OnProgress(this.Position, this.Length);
            OnWriteProgress(this.Position, this.Length);
        }
        public override int Read(byte[] array, int offset, int count)
        {
            var r = base.Read(array, offset, count);
            OnProgress(this.Position, this.Length);
            OnReadProgress(this.Position, this.Length);
            return r;
        }
        public override int ReadByte()
        {
            var r = base.ReadByte();
            OnProgress(this.Position, this.Length);
            OnReadProgress(this.Position, this.Length);
            return r;
        }
    }
}
