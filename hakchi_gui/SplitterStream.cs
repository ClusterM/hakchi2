using System.Collections.Generic;
using System.IO;

namespace com.clusterrr.hakchi_gui
{
    class SplitterStream : Stream
    {
        private List<Stream> internalStreams = new List<Stream>();
        public SplitterStream(params Stream[] streams)
        {
            internalStreams.AddRange(streams);
        }

        public SplitterStream(List<Stream> streams) : this(streams.ToArray()) { }

        public SplitterStream RemoveStream(Stream stream)
        {
            internalStreams.Remove(stream);
            return this;
        }
        public SplitterStream AddStreams(params Stream[] streams)
        {
            foreach (var stream in streams)
            {
                if (stream is null) continue;
                internalStreams.Remove(stream);
                internalStreams.Add(stream);
            }
            return this;
        }
        public SplitterStream AddStreams(List<Stream> streams)
        {
            AddStreams(streams.ToArray());
            return this;
        }
        public SplitterStream ClearStreams()
        {
            internalStreams.Clear();
            return this;
        }
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return 0; }
            set { }
        }

        public override void Flush()
        {
            foreach (var stream in internalStreams)
                try
                {
                    stream.Flush();
                }
                catch { }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var stream in internalStreams)
                try
                {
                    stream.Write(buffer, offset, count);
                }
                catch { }
        }
    }
}
