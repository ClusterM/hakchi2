/*
 * MultiStream.cs
 * 
 * Copyright (c) 2015,2016, maxton. All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; If not, see
 * <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace GameArchives.Common
{
    /// <summary>
    /// A single stream which is actually made up of a number of streams in sequence.
    /// </summary>
    class MultiStream : Stream
    {
        private IList<Stream> parts;
        private IList<long> partSizes;
        private long position;

        internal MultiStream(IList<Stream> streams)
        {
            var length = 0L;
            partSizes = new List<long>(streams.Count);
            foreach (var stream in streams)
            {
                if (!stream.CanSeek)
                {
                    throw new Exception("All component streams must be seekable.");
                }
                if (!stream.CanRead)
                {
                    throw new Exception("All component streams must be readable.");
                }
                partSizes.Add(stream.Length);
                length += stream.Length;
            }
            Length = length;
            parts = streams;
        }

        /// <summary>
        /// Denotes whether the stream can be read from.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// Denotes whether the user can seek this stream.
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// Denotes whether the user can write to this stream.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// The total length of this file.
        /// </summary>
        public override long Length { get; }

        /// <summary>
        /// The current position the stream points to within the file.
        /// </summary>
        public override long Position
        {
            get { return position; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Attempted to seek to before the beginning of the file.");
                }
                if (value > Length)
                {
                    throw new System.IO.EndOfStreamException("Attempted to seek past the end of the file.");
                }
                position = value;
            }
        }

        /// <summary>
        /// Not implemented; read-only stream.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads `count` bytes into `buffer` at offset `offset`.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
            {
                throw new IndexOutOfRangeException("Attempt to fill buffer past its end");
            }
            if (this.Position == this.Length || this.Position + count > this.Length)
            {
                count = (int)(this.Length - this.Position);
                //throw new System.IO.EndOfStreamException("Cannot read past end of file.");
            }

            int totalBytesRead = 0;
            while (count > 0)
            {
                Stream current;
                long current_position = offsetToStream(position, out current);
                current.Position = current_position;
                int bytesRead = current.Read(buffer, offset, count);
                offset += bytesRead;
                count -= bytesRead;
                position += bytesRead;
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// Get the correct stream and offset.
        /// Returns the offset into that stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private long offsetToStream(long offset, out Stream s)
        {
            for (var i = 0; i < parts.Count; i++)
            {
                if (partSizes[i] > offset)
                {
                    s = parts[i];
                    return offset;
                }
                offset -= partSizes[i];
            }
            throw new ArgumentOutOfRangeException("Desired offset extends past final part file.");
        }

        /// <summary>
        /// Seek the stream to given position within the file relative to given origin.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    Position = offset;
                    break;
                case System.IO.SeekOrigin.Current:
                    Position = Position + offset;
                    break;
                case System.IO.SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
            return Position;
        }

        public override void Close()
        {
            base.Close();
            foreach (var s in parts)
            {
                s.Close();
            }
        }

        /// <summary>
        /// Not implemented; read-only stream.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not implemented; read-only stream.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}