using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using com.clusterrr.clovershell;

namespace mooftpserv
{
    public class NesMiniFileSystemHandler : IFileSystemHandler
    {
        // list of supported operating systems
        private enum OS { WinNT, WinCE, Unix };

        // currently used operating system
        private OS os;
        // current path as TVFS or unix-like
        private string currentPath;
        // clovershell
        private ClovershellConnection clovershell;

        public NesMiniFileSystemHandler(ClovershellConnection clovershell, string startPath)
        {
            os = OS.Unix;
            this.currentPath = startPath;
            this.clovershell = clovershell;
        }

        public NesMiniFileSystemHandler(ClovershellConnection clovershell)
            : this(clovershell, "/")
        {
        }

        private NesMiniFileSystemHandler(string path, OS os, ClovershellConnection clovershell)
        {
            this.currentPath = path;
            this.os = os;
            this.clovershell = clovershell;
        }

        public IFileSystemHandler Clone(IPEndPoint peer)
        {
            return new NesMiniFileSystemHandler(currentPath, os, clovershell);
        }

        public ResultOrError<string> GetCurrentDirectory()
        {
            return MakeResult<string>(currentPath);
        }

        public ResultOrError<string> ChangeDirectory(string path)
        {
            string newPath = ResolvePath(path);
            currentPath = newPath;
            return MakeResult<string>(newPath);
        }

        public ResultOrError<string> ChangeToParentDirectory()
        {
            return ChangeDirectory("..");
        }

        public ResultOrError<string> CreateDirectory(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                foreach (var c in newPath)
                    if ((int)c > 255) throw new Exception("Invalid characters in directory name");
                var newpath = DecodePath(newPath);
                clovershell.ExecuteSimple("mkdir \"" + newpath + "\"");
            }
            catch (Exception ex)
            {
                return MakeError<string>(ex.Message);
            }

            return MakeResult<string>(newPath);
        }

        public ResultOrError<bool> RemoveDirectory(string path)
        {
            string newPath = ResolvePath(path);

            try
            {
                var rpath = DecodePath(newPath);
                clovershell.ExecuteSimple("rm -rf \"" + rpath + "\"");
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }

            return MakeResult<bool>(true);
        }

        public ResultOrError<Stream> ReadFile(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                var data = new MemoryStream();
                clovershell.Execute("cat \"" + newPath + "\"", null, data, null, 1000, true);
                data.Seek(0, SeekOrigin.Begin);
                return MakeResult<Stream>(data);
            }
            catch (Exception ex)
            {
                return MakeError<Stream>(ex.Message);
            }
        }

        public ResultOrError<Stream> WriteFile(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                foreach (var c in newPath)
                    if ((int)c > 255) throw new Exception("Invalid characters in directory name");
                return MakeResult<Stream>(new MemoryStream());
            }
            catch (Exception ex)
            {
                return MakeError<Stream>(ex.Message);
            }
        }

        public ResultOrError<bool> WriteFileFinalize(string path, Stream str)
        {
            string newPath = ResolvePath(path);
            try
            {
                str.Seek(0, SeekOrigin.Begin);
                clovershell.Execute("cat > \"" + newPath + "\"", str, null, null, 1000, true);
                str.Dispose();
                return MakeResult<bool>(true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }
        }

        public ResultOrError<bool> RemoveFile(string path)
        {
            string newPath = ResolvePath(path);

            try
            {
                clovershell.ExecuteSimple("rm -rf \"" + newPath + "\"", 1000, true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }

            return MakeResult<bool>(true);
        }

        public ResultOrError<bool> RenameFile(string fromPath, string toPath)
        {
            fromPath = ResolvePath(fromPath);
            toPath = ResolvePath(toPath);
            try
            {
                clovershell.ExecuteSimple("mv \"" + fromPath + "\" \"" + toPath + "\"", 1000, true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }

            return MakeResult<bool>(true);
        }

        public ResultOrError<FileSystemEntry[]> ListEntries(string path)
        {
            string newPath = ResolvePath(path);
            List<FileSystemEntry> result = new List<FileSystemEntry>();
            try
            {
                var lines = clovershell.ExecuteSimple("ls -lep \"" + newPath + "\"", 1000, true)
                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.StartsWith("total")) continue;
                    FileSystemEntry entry = new FileSystemEntry();
                    entry.Mode = line.Substring(1, 12).Trim();
                    entry.Name = line.Substring(69).Trim();
                    entry.IsDirectory = entry.Name.EndsWith("/");
                    if (entry.IsDirectory) entry.Name = entry.Name.Substring(0, entry.Name.Length - 1);
                    entry.Size = long.Parse(line.Substring(29, 15).Trim());
                    // Who cares? There is no time source on NES Mini
                    //DateTime.Parse(line.Substring(44, 25).Trim());
                    entry.LastModifiedTimeUtc = DateTime.MinValue;
                    result.Add(entry);
                }
            }
            catch (Exception ex)
            {
                return MakeError<FileSystemEntry[]>(ex.Message);
            }

            return MakeResult<FileSystemEntry[]>(result.ToArray());
        }

        public ResultOrError<long> GetFileSize(string path)
        {
            string newPath = ResolvePath(path);
            List<FileSystemEntry> result = new List<FileSystemEntry>();
            try
            {
                var lines = clovershell.ExecuteSimple("ls -le \"" + newPath + "\"", 1000, true)
                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    return MakeResult<long>(long.Parse(line.Substring(29, 15).Trim()));
                }
            }
            catch (Exception ex)
            {
                return MakeError<long>(ex.Message);
            }
            return MakeResult<long>(0);
        }

        public ResultOrError<DateTime> GetLastModifiedTimeUtc(string path)
        {
            /*
            string newPath = ResolvePath(path);
            List<FileSystemEntry> result = new List<FileSystemEntry>();
            try
            {
                var lines = clovershell.ExecuteSimple("ls -le \"" + newPath + "\"", 1000, true)
                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    MakeResult<DateTime>(DateTime.Parse(line.Substring(45, 25).Trim()));
                }
            }
            catch (Exception ex)
            {
                return MakeError<DateTime>(ex.Message);
            }
             */
            return MakeResult<DateTime>(DateTime.MinValue);
        }

        private string ResolvePath(string path)
        {
            if (path == null) return currentPath;
            if (path.Contains(" -> "))
                path = path.Substring(path.IndexOf(" -> ") + 4);
            return FileSystemHelper.ResolvePath(currentPath, path);
        }

        private string EncodePath(string path)
        {
            if (os == OS.WinNT)
                return "/" + path[0] + (path.Length > 2 ? path.Substring(2).Replace(@"\", "/") : "");
            else if (os == OS.WinCE)
                return path.Replace(@"\", "/");
            else
                return path;
        }

        private string DecodePath(string path)
        {
            if (path == null || path == "" || path[0] != '/')
                return null;

            if (os == OS.WinNT)
            {
                // some error checking for the drive layer
                if (path == "/")
                    return null; // should have been caught elsewhere

                if (path.Length > 1 && path[1] == '/')
                    return null;

                if (path.Length > 2 && path[2] != '/')
                    return null;

                if (path.Length < 4) // e.g. "/C/"
                    return path[1] + @":\";
                else
                    return path[1] + @":\" + path.Substring(3).Replace("/", @"\");
            }
            else if (os == OS.WinCE)
            {
                return path.Replace("/", @"\");
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Shortcut for ResultOrError<T>.MakeResult()
        /// </summary>
        private ResultOrError<T> MakeResult<T>(T result)
        {
            return ResultOrError<T>.MakeResult(result);
        }

        /// <summary>
        /// Shortcut for ResultOrError<T>.MakeError()
        /// </summary>
        private ResultOrError<T> MakeError<T>(string error)
        {
            return ResultOrError<T>.MakeError(error);
        }

        public ResultOrError<bool> ChmodFile(string mode, string path)
        {
            try
            {
                clovershell.ExecuteSimple(string.Format("chmod {0} {1}", mode, path), 1000, true);
                return ResultOrError<bool>.MakeResult(true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }
        }
    }
}
