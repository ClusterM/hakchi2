using System.IO;

namespace com.clusterrr.util
{
    public class TrackableStream : MemoryStream
    {
        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnProgress = delegate { };
        private long internalStreamProgress = 0;
        private Stream internalStream;

        //
        // Summary:
        //     Initializes a new instance of the TrackableStream class with an expandable
        //     capacity initialized to zero.
        public TrackableStream() : base() { }

        //
        // Summary:
        //     Initializes a new instance of the TrackableStream class with the contents of stream.
        //     capacity initialized to zero.
        public TrackableStream(Stream stream) : base()
        {
            internalStream = stream;
        }

        //
        // Summary:
        //     Initializes a new instance of the TrackableStream class with an expandable
        //     capacity initialized as specified.
        //
        // Parameters:
        //   capacity:
        //     The initial size of the internal array in bytes.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     capacity is negative.
        public TrackableStream(int capacity) : base(capacity) { }

        //
        // Summary:
        //     Initializes a new non-resizable instance of the TrackableStream class
        //     based on the specified byte array.
        //
        // Parameters:
        //   buffer:
        //     The array of unsigned bytes from which to create the current stream.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        public TrackableStream(byte[] buffer) : base(buffer) { }

        //
        // Summary:
        //     Initializes a new non-resizable instance of the TrackableStream class
        //     based on the specified byte array with the TrackableStream.CanWrite property
        //     set as specified.
        //
        // Parameters:
        //   buffer:
        //     The array of unsigned bytes from which to create this stream.
        //
        //   writable:
        //     The setting of the TrackableStream.CanWrite property, which determines
        //     whether the stream supports writing.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        public TrackableStream(byte[] buffer, bool writable) : base(buffer, writable) { }

        //
        // Summary:
        //     Initializes a new non-resizable instance of the TrackableStream class
        //     based on the specified region (index) of a byte array.
        //
        // Parameters:
        //   buffer:
        //     The array of unsigned bytes from which to create this stream.
        //
        //   index:
        //     The index into buffer at which the stream begins.
        //
        //   count:
        //     The length of the stream in bytes.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        public TrackableStream(byte[] buffer, int index, int count) : base(buffer, index, count) { }

        //
        // Summary:
        //     Initializes a new non-resizable instance of the TrackableStream class
        //     based on the specified region of a byte array, with the TrackableStream.CanWrite
        //     property set as specified.
        //
        // Parameters:
        //   buffer:
        //     The array of unsigned bytes from which to create this stream.
        //
        //   index:
        //     The index in buffer at which the stream begins.
        //
        //   count:
        //     The length of the stream in bytes.
        //
        //   writable:
        //     The setting of the TrackableStream.CanWrite property, which determines
        //     whether the stream supports writing.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count are negative.
        //
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        public TrackableStream(byte[] buffer, int index, int count, bool writable) : base(buffer, index, count, writable) { }

        //
        // Summary:
        //     Initializes a new instance of the TrackableStream class based on the specified
        //     region of a byte array, with the TrackableStream.CanWrite property set
        //     as specified, and the ability to call TrackableStream.GetBuffer set as
        //     specified.
        //
        // Parameters:
        //   buffer:
        //     The array of unsigned bytes from which to create this stream.
        //
        //   index:
        //     The index into buffer at which the stream begins.
        //
        //   count:
        //     The length of the stream in bytes.
        //
        //   writable:
        //     The setting of the TrackableStream.CanWrite property, which determines
        //     whether the stream supports writing.
        //
        //   publiclyVisible:
        //     true to enable TrackableStream.GetBuffer, which returns the unsigned byte
        //     array from which the stream was created; otherwise, false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        public TrackableStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) : base(buffer, index, count, writable, publiclyVisible) { }

        //
        // Summary:
        //     Writes a block of bytes to the current stream using data read from a buffer.
        //
        // Parameters:
        //   buffer:
        //     The buffer to write data from.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin copying bytes to the current
        //     stream.
        //
        //   count:
        //     The maximum number of bytes to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.NotSupportedException:
        //     The stream does not support writing. For additional information see System.IO.Stream.CanWrite.-or-
        //     The current position is closer than count bytes to the end of the stream, and
        //     the capacity cannot be modified.
        //
        //   T:System.ArgumentException:
        //     offset subtracted from the buffer length is less than count.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     offset or count are negative.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The current stream instance is closed.
        public override void Write(byte[] array, int offset, int count)
        {
            if (internalStream is Stream)
            {
                internalStream.Write(array, offset, count);
                OnProgress(internalStream.Position, internalStream.Length);
            }
            else
            {
                base.Write(array, offset, count);
                OnProgress(this.Position, this.Length);
            }
        }

        //
        // Summary:
        //     Writes a byte to the current stream at the current position.
        //
        // Parameters:
        //   value:
        //     The byte to write.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The stream does not support writing. For additional information see System.IO.Stream.CanWrite.-or-
        //     The current position is at the end of the stream, and the capacity cannot be
        //     modified.
        //
        //   T:System.ObjectDisposedException:
        //     The current stream is closed.
        public override void WriteByte(byte value)
        {
            if (internalStream is Stream)
            {
                internalStream.WriteByte(value);
                OnProgress(internalStream.Position, internalStream.Length);
            }
            else
            {
                base.WriteByte(value);
                OnProgress(this.Position, this.Length);
            }
        }

        //
        // Summary:
        //     Reads a block of bytes from the current stream and writes the data to a buffer.
        //
        // Parameters:
        //   buffer:
        //     When this method returns, contains the specified byte array with the values between
        //     offset and (offset + count - 1) replaced by the characters read from the current
        //     stream.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin storing data from the
        //     current stream.
        //
        //   count:
        //     The maximum number of bytes to read.
        //
        // Returns:
        //     The total number of bytes written into the buffer. This can be less than the
        //     number of bytes requested if that number of bytes are not currently available,
        //     or zero if the end of the stream is reached before any bytes are read.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     offset or count is negative.
        //
        //   T:System.ArgumentException:
        //     offset subtracted from the buffer length is less than count.
        //
        //   T:System.ObjectDisposedException:
        //     The current stream instance is closed.
        public override int Read(byte[] array, int offset, int count)
        {
            int r;
            if (internalStream is Stream)
            {
                r = internalStream.Read(array, offset, count);
                internalStreamProgress += r;
                OnProgress(internalStreamProgress, this.Length);
            }
            else
            {
                r = base.Read(array, offset, count);
                OnProgress(this.Position, this.Length);
            }

            return r;
        }

        //
        // Summary:
        //     Reads a byte from the current stream.
        //
        // Returns:
        //     The byte cast to a System.Int32, or -1 if the end of the stream has been reached.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The current stream instance is closed.
        public override int ReadByte()
        {
            int r;
            if (internalStream is Stream)
            {
                r = internalStream.ReadByte();
                internalStreamProgress += r;
                OnProgress(internalStreamProgress, this.Length);
            }
            else
            {
                r = base.ReadByte();
                OnProgress(this.Position, this.Length);
            }
            return r;
        }
    }
}
