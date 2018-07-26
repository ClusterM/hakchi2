using com.clusterrr.hakchi_gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ArxOne.Ftp;

namespace com.clusterrr.util.arxoneftp
{
    class FtpWrapper : IDisposable
    {
        const int transferChunkSize = 256 * 1024;

        public delegate void OnProgressDelegate(long Position, long Length, string filename);
        public delegate void OnFileInputErrorDelegate(string filename);
        public event OnProgressDelegate OnReadProgress = delegate { };
        public event OnFileInputErrorDelegate OnFileInputError = delegate { };

        public long Length
        {
            get { return totalSize; }
        }

        IEnumerable<ApplicationFileInfo> localSet;
        SortedSet<string> directorySet;
        string rootDirectory;
        string[] skipFiles;
        long totalSize;
        ArxOne.Ftp.FtpClient ftp;

        public FtpWrapper(IEnumerable<ApplicationFileInfo> localSet, string rootDirectory = null, string[] skipFiles = null)
        {
            this.localSet = localSet;
            this.rootDirectory = rootDirectory;
            this.skipFiles = skipFiles;
            this.ftp = null;
            scanLocalSet();
        }

        public FtpWrapper(string directory, string rootDirectory = null, string[] skipFiles = null)
        {
            throw new NotImplementedException();
        }

        private void scanLocalSet()
        {
            directorySet = new SortedSet<string>();
            totalSize = 0;
            foreach (var afi in localSet)
            {
                if (skipFiles != null && Array.IndexOf(skipFiles, Path.GetFileName(afi.FilePath)) != -1)
                    continue;

                string targetDir = Path.GetDirectoryName(afi.FilePath).Replace("\\", "/");
                string baseDir = ".";
                foreach(var dir in targetDir.Split('/').Skip(1))
                {
                    baseDir += $"/{dir}";
                    if (!directorySet.Contains(baseDir))
                        directorySet.Add(baseDir);
                }

                directorySet.Add(Path.GetDirectoryName(afi.FilePath).Replace("\\", "/"));
                totalSize += afi.FileSize;
            }
        }

        public bool Connect(string host, int port, string username, string password)
        {
            try
            {
                ftp = new ArxOne.Ftp.FtpClient(new Uri($"ftp://{host}:{port}"), new NetworkCredential(username, password));
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return false;
        }

        public bool Upload(string rootRemoteDirectory = null)
        {
            if (ftp == null)
                return false;

            foreach (var dir in this.directorySet)
            {
                string uri = rootRemoteDirectory + "/" + dir.Substring(2);
#if VERY_DEBUG
                Debug.WriteLine("MKD: " + uri);
#endif
                try
                {
                    ftp.Mkd(uri);
                }
                catch (ArxOne.Ftp.Exceptions.FtpFileException ex)
                {
                    if (ex.Code != 550)
                        Trace.WriteLine(ex.Message);
                }
            }

            using (MemoryStream commandBuilder = new MemoryStream())
            {
                string command = $"#!/bin/sh\ncd \"{rootRemoteDirectory}\"\n";
                commandBuilder.Write(Encoding.UTF8.GetBytes(command), 0, command.Length);

                long currentPosition = 0;
                foreach (var afi in localSet)
                {
                    if (skipFiles != null && Array.IndexOf(skipFiles, Path.GetFileName(afi.FilePath)) != -1)
                        continue;

                    Stream inStream = null;
                    bool owned = true;
                    if (!string.IsNullOrEmpty(afi.LocalFilePath))
                    {
                        inStream = new FileStream(afi.LocalFilePath, FileMode.Open);
                    }
                    else if (afi.FileStream != null)
                    {
                        inStream = afi.FileStream;
                        inStream.Position = 0;
                        owned = false;
                    }
                    else if (this.rootDirectory != null)
                    {
                        string inPath = new Uri(this.rootDirectory + "/" + afi.FilePath).LocalPath;
                        inStream = new FileStream(inPath, FileMode.Open);
                    }
                    if (inStream != null)
                    {
                        string uri = rootRemoteDirectory + "/" + afi.FilePath.Substring(2);
                        using (Stream outStream = ftp.Stor(uri))
                        {
#if VERY_DEBUG
                            Debug.WriteLine("STOR: " + uri);
#endif
                            byte[] buf = new byte[transferChunkSize];
                            int read, totalRead = 0;
                            while((read = inStream.Read(buf, 0, buf.Length)) > 0)
                            {
                                outStream.Write(buf, 0, read);
                                totalRead += read;
                                OnReadProgress(currentPosition + totalRead, totalSize, afi.FilePath);
                            }
                        }
                        if (owned)
                            inStream.Dispose();

                        command = string.Format("touch -m -t {1} '{0}'\n", afi.FilePath.Replace("'", @"'\''"), afi.ModifiedTime.ToString("yyyyMMddHHmm.ss"));
                        commandBuilder.Write(Encoding.UTF8.GetBytes(command), 0, command.Length);
                    }
                    else
                    {
                        Trace.WriteLine($"Error with file \"{afi.FilePath}\", no input data.");
                        OnFileInputError(afi.FilePath);
                    }
                    currentPosition += afi.FileSize;
                    OnReadProgress(currentPosition, totalSize, afi.FilePath);
                }


                // adjust time pointers!
                hakchi.RunTemporaryScript(commandBuilder, "modtime.sh");
            }
            return true;
        }

        public void Dispose()
        {
            if (ftp != null)
            {
                ftp.Dispose();
                ftp = null;
            }
        }
    }
}
