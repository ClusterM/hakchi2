using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.util
{
    public class TarStream : Stream
    {
        List<string> entries = new List<string>();
        readonly string rootDirectory;
        const string LongLinkFlag = "././@LongLink";
        long totalSize = 0;
        long position = 0;
        int currentEntry = 0;
        long currentEntryPosition = 0;
        long currentEntryLength = 0;
        Stream currentFile = null;
        byte[] currentHeader;

        public delegate void OnProgressDelegate(long Position, long Length);
        public event OnProgressDelegate OnReadProgress = delegate { };

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
                var bytes = getBytes();
                uint summ = 0;
                foreach (var b in bytes)
                    summ += b;
                Array.Copy(Encoding.ASCII.GetBytes(Convert.ToString(summ, 8).PadLeft(6, '0')), 0, Checksum, 0, 6);
                Checksum[6] = 0;
                Checksum[7] = 32;
            }

            public byte[] getBytes()
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

        public TarStream(string directory, string rootDirectory = null)
        {
            if (rootDirectory == null) rootDirectory = directory;
            if (!Directory.Exists(directory))
                throw new Exception("Directory not exists");
            directory = Path.GetFullPath(directory);
            if (!Directory.Exists(rootDirectory))
                throw new Exception("Root directory not exists");
            rootDirectory = Path.GetFullPath(rootDirectory); // Full path
            if (!directory.StartsWith(rootDirectory))
                throw new Exception("Invarid root directory");

            LoadDirectory(directory);
            for (int i = entries.Count - 1; i >= 0; i--) // Checking filenames
            {
                var name = entries[i].Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                if (name.Length > 99) // Need to create LongLink
                {
                    entries.Insert(i, LongLinkFlag);
                    int size = name.Length;
                    if (size % 512 != 0)
                        size += 512 - (size % 512);
                    totalSize += 512 + size;
                }
            }
            if (totalSize % 10240 != 0)
                totalSize += (10240 - (totalSize % 10240));
            this.rootDirectory = rootDirectory;
        }

        private void LoadDirectory(string directory)
        {
            if (!Directory.Exists(directory)) return;
            var directories = Directory.GetDirectories(directory);
            foreach (var d in directories)
            {
                var dname = d;
                if (!dname.EndsWith(@"\"))
                    dname += @"\";
                entries.Add(dname);
                totalSize += 512;
                LoadDirectory(d);
            }
            var files = Directory.GetFiles(directory);
            foreach (var f in files)
            {
                entries.Add(f);
                var size = new FileInfo(f).Length;
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
                throw new NotImplementedException();
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

                if (currentEntry >= entries.Count) // end of archive, write zeros
                {
                    if (currentFile != null)
                    {
                        currentFile.Dispose();
                        currentFile = null;
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
                    if (entries[currentEntry] != LongLinkFlag)
                        header.FileName = entries[currentEntry].Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                    if (currentFile != null)
                    {
                        currentFile.Dispose();
                        currentFile = null;
                    }
                    if (entries[currentEntry] == LongLinkFlag)
                    {
                        header.FileName = entries[currentEntry];
                        header.FileMode = "0000000";
                        var name = entries[currentEntry+1].Substring(rootDirectory.Length + 1).Replace(@"\", "/");
                        var nameBuff = Encoding.UTF8.GetBytes(name);
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
                        currentFile = new FileStream(entries[currentEntry], FileMode.Open);
                        header.FileMode = "0100644";
                        currentEntryLength += currentFile.Length;
                        if (currentFile.Length % 512 != 0)
                            currentEntryLength += 512 - (currentFile.Length % 512);
                        header.FileSize = Convert.ToString(currentFile.Length, 8).PadLeft(11, '0');
                        header.LastModificationTime = Convert.ToString(
                            (long)new FileInfo(entries[currentEntry]).LastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                            , 8).PadLeft(11, '0');
                        header.FileType = '0';
                    }
                    else if (header.FileName.EndsWith("/")) // It's a directory...
                    {
                        header.FileMode = "0040755";
                        header.FileSize = "".PadLeft(11, '0');
                        header.LastModificationTime = Convert.ToString(
                            (long)new DirectoryInfo(entries[currentEntry]).LastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                            , 8).PadLeft(11, '0');
                        header.FileType = '5';
                    }
                    header.OwnerID = "0000000";
                    header.GroupID = "0000000";
                    header.UstarMagic = "ustar  ";
                    //header.UstarVersion = new char[] {'0', '0'};
                    header.CalcChecksum();
                    currentHeader = header.getBytes();
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
    }
}
