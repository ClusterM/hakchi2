using com.clusterrr.hakchi_gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FluentFTP;

namespace com.clusterrr.util.fluentftp
{
    public class FtpWrapper : IDisposable
    {
        const int transferChunkSize = 256 * 1024;

        public delegate void OnProgressDelegate(long Position, long Length, string filename);
        public delegate void OnFileInputErrorDelegate(string filename);
        public event OnProgressDelegate OnReadProgress = delegate { };
        public event OnFileInputErrorDelegate OnFileInputError = delegate {};

        public long Length
        {
            get { return totalSize; }
        }

        IEnumerable<ApplicationFileInfo> localSet;
        SortedSet<string> directorySet;
        string rootDirectory;
        string[] skipFiles;
        long totalSize;
        FluentFTP.FtpClient ftp;

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
                directorySet.Add(Path.GetDirectoryName(afi.FilePath).Replace("\\", "/"));
                totalSize += afi.FileSize;
            }
        }

        public bool Connect(string host, int port, string username, string password)
        {
            try
            {
                FluentFTP.FtpTrace.EnableTracing = false;
                ftp = new FluentFTP.FtpClient(host, port, username, password);
                ftp.EnableThreadSafeDataConnections = false;
                ftp.TransferChunkSize = transferChunkSize;
                ftp.Connect();
                return ftp.IsConnected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public bool Upload(string rootRemoteDirectory = null)
        {
            if (ftp == null || !ftp.IsConnected)
                return false;

            if (!string.IsNullOrEmpty(rootRemoteDirectory))
                ftp.SetWorkingDirectory(rootRemoteDirectory);

            foreach (var dir in this.directorySet)
            {
                ftp.CreateDirectory(dir);
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
                        ftp.Upload(inStream, afi.FilePath, FtpExists.NoCheck, false, new Progress<double>(p =>
                        {
                            if (p != -1)
                                OnReadProgress(currentPosition + (long)(p * afi.FileSize / 100), totalSize, afi.FilePath);
                        }));
                        if (owned)
                            inStream.Dispose();

                        command = string.Format("touch -m -t {1} '{0}'\n", afi.FilePath.Replace("'", @"'\''"), afi.ModifiedTime.ToString("yyyyMMddHHmm.ss"));
                        commandBuilder.Write(Encoding.UTF8.GetBytes(command), 0, command.Length);
                    }
                    else
                    {
                        Debug.WriteLine($"Error with file \"{afi.FilePath}\", no input data.");
                        OnFileInputError(afi.FilePath);
                    }
                    currentPosition += afi.FileSize;
                    OnReadProgress(currentPosition, totalSize, afi.FilePath);
                }
                
                // adjust file modified times after the fact (ftpd doesn't seem to support setting dates)
                try
                {
                    hakchi.Shell.Execute("cat > /tmp/modtime.sh", commandBuilder, null, null, 5000, true);
                    hakchi.Shell.ExecuteSimple("chmod +x /tmp/modtime.sh && /tmp/modtime.sh", 0, true);
                }
                finally
                {
                    hakchi.Shell.ExecuteSimple("rm /tmp/modtime.sh", 0, true);
                }

            }
            return true;
        }

        public void Dispose()
        {
            if(ftp != null)
            {
                if (ftp.IsConnected)
                    ftp.Disconnect();
                ftp.Dispose();
                ftp = null;
            }
        }
    }
}

