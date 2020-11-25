using com.clusterrr.hakchi_gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.util
{
    public class TarStream : Stream
    {
        const string LongLinkFlag = "././@LongLink";
        readonly string rootDirectory;

        private class Entry
        {
            public string FileName = null; // when in normal mode, files are stored in this var and are absolute
            public ApplicationFileInfo ExtInfo = null; // when in ApplicationFileInfo mode, data is stored here, except for long link flag
        }
        List<Entry> entries = new List<Entry>();

        long totalSize = 0;
        long position = 0;
        int currentEntry = 0;
        long currentEntryPosition = 0;
        long currentEntryLength = 0;
        Stream currentFile = null;
        bool currentFileOwned = false;
        byte[] currentHeader;

        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnReadProgress = delegate { };

        string currentFileName = "";
        public delegate void OnAdvancedProgressDelegate(long Position, long Length, string FileName);
        public event OnAdvancedProgressDelegate OnAdvancedReadProgress = delegate { };

        [StructLayout(LayoutKind.Sequential)]
        private struct TarHeader
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string FileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string FileMode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string OwnerID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string GroupID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string FileSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string LastModificationTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Checksum;
            [MarshalAs(UnmanagedType.U1)]
            public char FileType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string LinkedName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string UstarMagic;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            //public char[] UstarVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string OwnerUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string OwnerGroupName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string DeviceMajorNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string DeviceMinorNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 155)]
            public string FileNamePrefix;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            string padding;

            public void CalcChecksum()
            {
                Checksum = new byte[] { 32, 32, 32, 32, 32, 32, 32, 32 };
                var bytes = GetBytes();
                uint summ = 0;
                foreach (var b in bytes)
                    summ += b;
                Array.Copy(Encoding.ASCII.GetBytes(Convert.ToString(summ, 8).PadLeft(6, '0')), 0, Checksum, 0, 6);
                Checksum[6] = 0;
                Checksum[7] = 32;
            }

            public byte[] GetBytes()
            {
                int size = Marshal.SizeOf(this);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }
        }

        public TarStream(IEnumerable<ApplicationFileInfo> localGameSet, string rootDirectory = ".", string[] skipFiles = null)
        {
            this.rootDirectory = rootDirectory ?? string.Empty;
            LoadApplicationFileInfo(localGameSet, skipFiles);
            InsertLongFileLinks();
        }

        public TarStream(string directory, string rootDirectory = null, string[] skipFiles = null)
        {
            if (rootDirectory == null)
                rootDirectory = directory;
            if (!Directory.Exists(directory))
                throw new Exception($"Directory doesn't exist: \"{directory}\"");
            directory = Path.GetFullPath(directory);
            if (!Directory.Exists(rootDirectory))
                throw new Exception($"Root directory doesn't exist: \"{rootDirectory}\"");
            rootDirectory = Path.GetFullPath(rootDirectory); // Full path
            if (!directory.StartsWith(rootDirectory))
                throw new Exception("Invalid root directory");
            this.rootDirectory = rootDirectory;

            LoadDirectory(directory, skipFiles);
            InsertLongFileLinks();
        }

        private void InsertLongFileLinks()
        {
            for (int i = entries.Count - 1; i >= 0; i--) // Checking filenames
            {
                var name = (entries[i].FileName ?? entries[i].ExtInfo.FilePath).Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                if (name.Length > 99) // Need to create LongLink
                {
                    entries.Insert(i, new Entry() { FileName = LongLinkFlag });
                    int size = name.Length;
                    if (size % 512 != 0)
                        size += 512 - (size % 512);
                    totalSize += 512 + size;
                }
            }
            if (totalSize % 10240 != 0)
                totalSize += (10240 - (totalSize % 10240));
        }

        private void LoadApplicationFileInfo(IEnumerable<ApplicationFileInfo> localGameSet, string[] skipFiles = null)
        {
            // extract directories out of the filenames in game set (hashset will keep entries unique)
            HashSet<string> directories = new HashSet<string>();
            foreach (var afi in localGameSet)
            {
                if (Path.GetDirectoryName(afi.FilePath).Replace(@"\", "/").Substring(rootDirectory.Length + 1).Length > 0)
                    directories.Add(Path.GetDirectoryName(afi.FilePath).Replace(@"\", "/").TrimEnd('/') + '/');
            }

            // add directories as separate entries (to mimick normal version)
            foreach(var d in directories)
            {
                entries.Add(new Entry() { ExtInfo = new ApplicationFileInfo(d, 0, DateTime.UtcNow) });
                totalSize += 512;
            }

            // add files as ApplicationFileInfo entries
            foreach(var afi in localGameSet)
            {
                if (skipFiles != null && skipFiles.Contains(Path.GetFileName(afi.FilePath)))
                    continue;
                entries.Add(new Entry() { ExtInfo = afi });
                long size = afi.FileSize;
                if (size % 512 != 0)
                    size += 512 - (size % 512);
                totalSize += 512 + size;
            }
        }

        private void LoadDirectory(string directory, string[] skipFiles)
        {
            if (!Directory.Exists(directory)) return;
            var directories = Directory.GetDirectories(directory);
            foreach (var d in directories)
            {
                var dname = d;
                if (!dname.EndsWith(@"\"))
                    dname += @"\";
                entries.Add(new Entry() { FileName = dname });
                totalSize += 512;
                LoadDirectory(d, skipFiles);
            }
            var files = Directory.GetFiles(directory);
            foreach (var f in files)
            {
                if (skipFiles != null && skipFiles.Contains(Path.GetFileName(f)))
                    continue;
                entries.Add(new Entry() { FileName = f });
                long size = new FileInfo(f).Length;
                if (size % 512 != 0)
                    size += 512 - (size % 512);
                totalSize += 512 + size;
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return totalSize; }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value != 0)
                    throw new NotImplementedException();

                position = 0;
                currentEntry = 0;
                currentEntryPosition = 0;
                currentEntryLength = 0;
                if (currentFile != null)
                {
                    if (currentFileOwned)
                        currentFile.Dispose();
                    currentFile = null;
                    currentFileOwned = false;
                }
                currentHeader = null;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int origCount = count;

            while (count > 0)
            {
                if (currentEntryLength > 0 && currentEntryPosition >= currentEntryLength) // Next entry
                {
                    currentEntry++;
                    currentEntryPosition = 0;
                }
                if (currentEntry >= entries.Count) // End of archive, write zeros
                {
                    if (currentFile != null)
                    {
                        if (currentFileOwned) currentFile.Dispose();
                        currentFile = null;
                        currentFileOwned = false;
                    }
                    long l = Math.Min(count, totalSize - position);
                    var dummy = new byte[l];
                    Array.Copy(dummy, 0, buffer, offset, l);
                    count -= (int)l;
                    position += l;
                    currentEntryPosition += l;
                    offset += (int)l;
                    break;
                }

                if (currentEntryPosition == 0) // New entry
                {
                    currentEntryLength = 512;
                    var header = new TarHeader();

                    var fileName = entries[currentEntry].FileName ?? entries[currentEntry].ExtInfo.FilePath;
                    if (fileName != LongLinkFlag)
                    {
                        header.FileName = fileName.Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                    }

                    if (currentFile != null)
                    {
                        if (currentFileOwned) currentFile.Dispose();
                        currentFile = null;
                        currentFileOwned = false;
                    }

                    if (fileName == LongLinkFlag)
                    {
                        header.FileName = fileName;
                        header.FileMode = "0000000";
                        fileName = (entries[currentEntry + 1].FileName ?? entries[currentEntry + 1].ExtInfo.FilePath).Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                        var nameBuff = Encoding.UTF8.GetBytes(fileName);
                        currentFile = new MemoryStream(nameBuff.Length + 1);
                        currentFile.Write(nameBuff, 0, nameBuff.Length);
                        currentFile.WriteByte(0);
                        currentFile.Seek(0, SeekOrigin.Begin);
                        currentEntryLength += currentFile.Length;
                        if (currentFile.Length % 512 != 0)
                            currentEntryLength += 512 - (currentFile.Length % 512);
                        header.FileSize = Convert.ToString(currentFile.Length, 8).PadLeft(11, '0');
                        header.LastModificationTime = "0".PadLeft(11, '0');
                        header.FileType = 'L';
                    }
                    else if (!header.FileName.EndsWith("/")) // It's a file!
                    {
                        string localFilePath = entries[currentEntry].FileName ?? entries[currentEntry].ExtInfo.LocalFilePath;
                        DateTime lastWriteTimeUtc;
                        if (localFilePath != null) // Standard file
                        {
                            currentFile = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                            currentFileOwned = true;
                            lastWriteTimeUtc = new FileInfo(localFilePath).LastWriteTimeUtc;
                        }
                        else // file link
                        {
                            currentFile = entries[currentEntry].ExtInfo.FileStream ?? new MemoryStream();
                            try
                            {
                                currentFile.Position = 0;
                            }
                            catch { }
                            currentFileOwned = entries[currentEntry].ExtInfo.FileStream == null;
                            lastWriteTimeUtc = entries[currentEntry].ExtInfo.ModifiedTime;
                        }

                        header.FileMode = "0100644";
                        currentEntryLength += currentFile.Length;
                        if (currentFile.Length % 512 != 0)
                            currentEntryLength += 512 - (currentFile.Length % 512);
                        header.FileSize = Convert.ToString(currentFile.Length, 8).PadLeft(11, '0');
                        header.LastModificationTime = Convert.ToString(
                            (long)lastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                            , 8).PadLeft(11, '0');
                        header.FileType = '0';

                        this.currentFileName = header.FileName; // keep filename for advanced progress report
                    }
                    else if (header.FileName.EndsWith("/")) // It's a directory...
                    {
                        DateTime lastWriteTimeUtc = entries[currentEntry].FileName != null ? new DirectoryInfo(entries[currentEntry].FileName).LastWriteTimeUtc : DateTime.UtcNow;
                        header.FileMode = "0040755";
                        header.FileSize = "".PadLeft(11, '0');
                        header.LastModificationTime = Convert.ToString(
                            (long)lastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                            , 8).PadLeft(11, '0');
                        header.FileType = '5';
                    }
                    header.OwnerID = "0000000";
                    header.GroupID = "0000000";
                    header.UstarMagic = "ustar  ";
                    //header.UstarVersion = new char[] {'0', '0'};
                    header.CalcChecksum();
                    currentHeader = header.GetBytes();
                }

                if (currentEntryPosition < 512) // Header
                {
                    long l = Math.Min(count, 512 - currentEntryPosition);
                    Array.Copy(currentHeader, currentEntryPosition, buffer, offset, l);
                    count -= (int)l;
                    position += l;
                    currentEntryPosition += l;
                    offset += (int)l;
                }
                else // Data
                {
                    long l = Math.Min(count, currentEntryLength - currentEntryPosition);
                    currentFile.Read(buffer, offset, (int)l);
                    count -= (int)l;
                    position += l;
                    currentEntryPosition += l;
                    offset += (int)l;
                }
            }
            OnReadProgress(Position, Length);
            OnAdvancedReadProgress(Position, Length, this.currentFileName);
            return origCount - count;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void DebugWrite()
        {
            foreach (var e in entries)
            {
                Trace.WriteLine("Filename: " + e.FileName ?? "");
                if (e.ExtInfo != null)
                    ApplicationFileInfo.DebugListHashSet(new List<ApplicationFileInfo>() { e.ExtInfo });
            }
        }
    }
}
