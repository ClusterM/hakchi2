using System.IO;
using System;

namespace SevenZip
{    
    /// <summary>
    /// The Stream extension class to emulate the archive part of a stream.
    /// </summary>
    internal class ArchiveEmulationStreamProxy : Stream, IDisposable
    {
        /// <summary>
        /// Gets the file offset.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// The source wrapped stream.
        /// </summary>
        public Stream Source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ArchiveEmulationStream class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="offset">The stream offset.</param>
        public ArchiveEmulationStreamProxy(Stream stream, int offset)
        {
            Source = stream;
            Offset = offset;
            Source.Position = offset;
        }

        public override bool CanRead
        {
            get { return Source.CanRead; }
        }

        public override bool CanSeek
        {
            get { return Source.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return Source.CanWrite; }
        }

        public override void Flush()
        {
            Source.Flush();
        }

        public override long Length
        {
            get { return Source.Length - Offset; }
        }

        public override long Position
        {
            get
            {
                return Source.Position - Offset;
            }
            set
            {
                Source.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Source.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Source.Seek(origin == SeekOrigin.Begin ? offset + Offset : offset,
                origin) - Offset;
        }

        public override void SetLength(long value)
        {
            Source.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Source.Write(buffer, offset, count);
        }

        public new void Dispose()
        {
            Source.Dispose();
        }

        public override void Close()
        {
            Source.Close();
        }
    }
}
