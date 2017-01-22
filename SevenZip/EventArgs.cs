/*  This file is part of SevenZipSharp.

    SevenZipSharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SevenZipSharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with SevenZipSharp.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;

namespace SevenZip
{
    /// <summary>
    /// The definition of the interface which supports the cancellation of a process.
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        bool Cancel { get; set; }
    }

    /// <summary>
    /// EventArgs for storing PercentDone property.
    /// </summary>
    public class PercentDoneEventArgs : EventArgs
    {
        private readonly byte _percentDone;

        /// <summary>
        /// Initializes a new instance of the PercentDoneEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public PercentDoneEventArgs(byte percentDone)
        {
            if (percentDone > 100 || percentDone < 0)
            {
                throw new ArgumentOutOfRangeException("percentDone",
                                                      "The percent of finished work must be between 0 and 100.");
            }
            _percentDone = percentDone;
        }

        /// <summary>
        /// Gets the percent of finished work.
        /// </summary>
        public byte PercentDone
        {
            get
            {
                return _percentDone;
            }
        }        

        /// <summary>
        /// Converts a [0, 1] rate to its percent equivalent.
        /// </summary>
        /// <param name="doneRate">The rate of the done work.</param>
        /// <returns>Percent integer equivalent.</returns>
        /// <exception cref="System.ArgumentException"/>
        internal static byte ProducePercentDone(float doneRate)
        {
#if !WINCE
            return (byte) Math.Round(Math.Min(100*doneRate, 100), MidpointRounding.AwayFromZero);
#else
            return (byte) Math.Round(Math.Min(100*doneRate, 100));
#endif
        }
    }

    /// <summary>
    /// The EventArgs class for accurate progress handling.
    /// </summary>
    public sealed class ProgressEventArgs : PercentDoneEventArgs
    {
        private readonly byte _delta;

        /// <summary>
        /// Initializes a new instance of the ProgressEventArgs class.
        /// </summary>
        /// <param name="percentDone">The percent of finished work.</param>
        /// <param name="percentDelta">The percent of work done after the previous event.</param>
        public ProgressEventArgs(byte percentDone, byte percentDelta)
            : base(percentDone)
        {
            _delta = percentDelta;
        }

        /// <summary>
        /// Gets the change in done work percentage.
        /// </summary>
        public byte PercentDelta
        {
            get
            {
                return _delta;
            }
        }
    }

#if UNMANAGED
    /// <summary>
    /// EventArgs used to report the file information which is going to be packed.
    /// </summary>
    public sealed class FileInfoEventArgs : PercentDoneEventArgs, ICancellable
    {
        private readonly ArchiveFileInfo _fileInfo;

        /// <summary>
        /// Initializes a new instance of the FileInfoEventArgs class.
        /// </summary>
        /// <param name="fileInfo">The current ArchiveFileInfo.</param>
        /// <param name="percentDone">The percent of finished work.</param>
        public FileInfoEventArgs(ArchiveFileInfo fileInfo, byte percentDone)
            : base(percentDone)
        {
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the corresponding FileInfo to the event.
        /// </summary>
        public ArchiveFileInfo FileInfo
        {
            get
            {
                return _fileInfo;
            }
        }
    }

    /// <summary>
    /// EventArgs used to report the size of unpacked archive data
    /// </summary>
    public sealed class OpenEventArgs : EventArgs
    {
        private readonly ulong _totalSize;

        /// <summary>
        /// Initializes a new instance of the OpenEventArgs class
        /// </summary>
        /// <param name="totalSize">Size of unpacked archive data</param>
        [CLSCompliant(false)]
        public OpenEventArgs(ulong totalSize)
        {
            _totalSize = totalSize;
        }

        /// <summary>
        /// Gets the size of unpacked archive data
        /// </summary>
        [CLSCompliant(false)]
        public ulong TotalSize
        {
            get
            {
                return _totalSize;
            }
        }
    }

    /// <summary>
    /// Stores an int number
    /// </summary>
    public sealed class IntEventArgs : EventArgs
    {
        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the IntEventArgs class
        /// </summary>
        /// <param name="value">Useful data carried by the IntEventArgs class</param>
        public IntEventArgs(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of the IntEventArgs class
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
        }
    }

    /// <summary>
    /// EventArgs class which stores the file name.
    /// </summary>
    public sealed class FileNameEventArgs : PercentDoneEventArgs, ICancellable
    {
        private readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the FileNameEventArgs class.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="percentDone">The percent of finished work</param>
        public FileNameEventArgs(string fileName, byte percentDone) :
            base(percentDone)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Gets or sets whether to stop the current archive operation.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
        }
    }

    /// <summary>
    /// EventArgs for FileExists event, stores the file name and asks whether to overwrite it in case it already exists.
    /// </summary>
    public sealed class FileOverwriteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the FileOverwriteEventArgs class
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public FileOverwriteEventArgs(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Gets or sets the value indicating whether to cancel the extraction.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the file name to extract to. Null means skip.
        /// </summary>
        public string FileName { get; set; }
    }

    /// <summary>
    /// The reason for calling <see cref="ExtractFileCallback"/>.
    /// </summary>
    public enum ExtractFileCallbackReason
    {
        /// <summary>
        /// <see cref="ExtractFileCallback"/> is called the first time for a file.
        /// </summary>
        Start,

        /// <summary>
        /// All data has been written to the target without any exceptions.
        /// </summary>
        Done,

        /// <summary>
        /// An exception occured during extraction of the file.
        /// </summary>
        Failure
    }

    /// <summary>
    /// The arguments passed to <see cref="ExtractFileCallback"/>.
    /// </summary>
    /// <remarks>
    /// For each file, <see cref="ExtractFileCallback"/> is first called with <see cref="Reason"/>
    /// set to <see cref="ExtractFileCallbackReason.Start"/>. If the callback chooses to extract the
    /// file data by setting <see cref="ExtractToFile"/> or <see cref="ExtractToStream"/>, the callback
    /// will be called a second time with <see cref="Reason"/> set to
    /// <see cref="ExtractFileCallbackReason.Done"/> or <see cref="ExtractFileCallbackReason.Failure"/>
    /// to allow for any cleanup task like closing the stream.
    /// </remarks>
    public class ExtractFileCallbackArgs : EventArgs
    {
        private readonly ArchiveFileInfo _archiveFileInfo;
        private Stream _extractToStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractFileCallbackArgs"/> class.
        /// </summary>
        /// <param name="archiveFileInfo">The information about file in the archive.</param>
        public ExtractFileCallbackArgs(ArchiveFileInfo archiveFileInfo)
        {
            Reason = ExtractFileCallbackReason.Start;
            _archiveFileInfo = archiveFileInfo;
        }

        /// <summary>
        /// Information about file in the archive.
        /// </summary>
        /// <value>Information about file in the archive.</value>
        public ArchiveFileInfo ArchiveFileInfo
        {
            get
            {
                return _archiveFileInfo;
            }
        }

        /// <summary>
        /// The reason for calling <see cref="ExtractFileCallback"/>.
        /// </summary>
        /// <remarks>
        /// If neither <see cref="ExtractToFile"/> nor <see cref="ExtractToStream"/> is set,
        ///  <see cref="ExtractFileCallback"/> will not be called after <see cref="ExtractFileCallbackReason.Start"/>.
        /// </remarks>
        /// <value>The reason.</value>
        public ExtractFileCallbackReason Reason { get; internal set; }

        /// <summary>
        /// The exception that occurred during extraction.
        /// </summary>
        /// <value>The _Exception.</value>
        /// <remarks>
        /// If the callback is called with <see cref="Reason"/> set to <see cref="ExtractFileCallbackReason.Failure"/>,
        /// this member contains the _Exception that occurred.
        /// The default behavior is to rethrow the _Exception after return of the callback.
        /// However the callback can set <see cref="Exception"/> to <c>null</c> to swallow the _Exception
        /// and continue extraction with the next file.
        /// </remarks>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the extraction.
        /// </summary>
        /// <value><c>true</c> to cancel the extraction; <c>false</c> to continue. The default is <c>false</c>.</value>
        public bool CancelExtraction { get; set; }

        /// <summary>
        /// Gets or sets whether and where to extract the file.
        /// </summary>
        /// <value>The path where to extract the file to.</value>
        /// <remarks>
        /// If <see cref="ExtractToStream"/> is set, this mmember will be ignored.
        /// </remarks>
        public string ExtractToFile { get; set; }

        /// <summary>
        /// Gets or sets whether and where to extract the file.
        /// </summary>
        /// <value>The the extracted data is written to.</value>
        /// <remarks>
        /// If both this member and <see cref="ExtractToFile"/> are <c>null</c> (the defualt), the file
        /// will not be extracted and the callback will be be executed a second time with the <see cref="Reason"/>
        /// set to <see cref="ExtractFileCallbackReason.Done"/> or <see cref="ExtractFileCallbackReason.Failure"/>.
        /// </remarks>
        public Stream ExtractToStream
        {
            get
            {
                return _extractToStream;
            }
            set
            {
                if (_extractToStream != null && !_extractToStream.CanWrite)
                {
                    throw new ExtractionFailedException("The specified stream is not writable!");
                }
                _extractToStream = value;
            }
        }

        /// <summary>
        /// Gets or sets any data that will be preserved between the <see cref="ExtractFileCallbackReason.Start"/> callback call
        /// and the <see cref="ExtractFileCallbackReason.Done"/> or <see cref="ExtractFileCallbackReason.Failure"/> calls.
        /// </summary>
        /// <value>The data.</value>
        public object ObjectData { get; set; }
    }

    /// <summary>
    /// Callback delegate for <see cref="SevenZipExtractor.ExtractFiles(SevenZip.ExtractFileCallback)"/>.
    /// </summary>
    public delegate void ExtractFileCallback(ExtractFileCallbackArgs extractFileCallbackArgs);
#endif
}