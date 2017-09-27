using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.util
{
    public class TrackableFileStream : FileStream
    {
        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnProgress = delegate { };

        public TrackableFileStream(string path, FileMode mode) : base(path, mode) { }

        public override void Write(byte[] array, int offset, int count)
        {
            base.Write(array, offset, count);
            OnProgress(this.Position, this.Length);
        }
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
            OnProgress(this.Position, this.Length);
        }
        public override int Read(byte[] array, int offset, int count)
        {
            var r = base.Read(array, offset, count);
            OnProgress(this.Position, this.Length);
            return r;
        }
        public override int ReadByte()
        {
            var r = base.ReadByte();
            OnProgress(this.Position, this.Length);
            return r;
        }
    }
}
