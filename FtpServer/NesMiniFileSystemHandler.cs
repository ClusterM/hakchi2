using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using com.clusterrr.hakchi_gui;
using System.Globalization;

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
        // shell
        private ISystemShell shell;

        public NesMiniFileSystemHandler(ISystemShell shell, string startPath)
        {
            os = OS.Unix;
            this.currentPath = startPath;
            this.shell = shell;
        }

        public NesMiniFileSystemHandler(ISystemShell shell)
            : this(shell, "/")
        {
        }

        private NesMiniFileSystemHandler(string path, OS os, ISystemShell shell)
        {
            this.currentPath = path;
            this.os = os;
            this.shell = shell;
        }

        public void UpdateShell(ISystemShell shell)
        {
            this.shell = shell;
        }

        public IFileSystemHandler Clone(IPEndPoint peer)
        {
            return new NesMiniFileSystemHandler(currentPath, os, shell);
        }

        public ResultOrError<string> GetCurrentDirectory()
        {
            return MakeResult<string>(currentPath);
        }

        public ResultOrError<string> ChangeDirectory(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                shell.ExecuteSimple("cd \""+newPath+"\"", 1000 ,true);
                currentPath = newPath;
            }
            catch (Exception ex)
            {
                return MakeError<string>(ex.Message);
            }
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
                shell.ExecuteSimple("mkdir \"" + newpath + "\"");
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
                shell.ExecuteSimple("rm -rf \"" + rpath + "\"");
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
                shell.Execute("cat \"" + newPath + "\"", null, data, null, 1000, true);
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
                string directory = "/";
                int p = newPath.LastIndexOf("/");
                if (p > 0)
                    directory = newPath.Substring(0, p);
                shell.Execute("mkdir -p \"" + directory + "\" && cat > \"" + newPath + "\"", str, null, null, 1000, true);
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
                shell.ExecuteSimple("rm -rf \"" + newPath + "\"", 1000, true);
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
                shell.ExecuteSimple("mv \"" + fromPath + "\" \"" + toPath + "\"", 1000, true);
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
                var lines = shell.ExecuteSimple("ls -lAp \"" + newPath + "\"", 1000, true)
                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.StartsWith("total")) continue;
                    FileSystemEntry entry = new FileSystemEntry();
                    entry.Mode = line.Substring(0, 13).Trim();
                    entry.Name = line.Substring(57).Trim();
                    entry.IsDirectory = entry.Name.EndsWith("/");
                    if (entry.IsDirectory) entry.Name = entry.Name.Substring(0, entry.Name.Length - 1);
                    entry.Size = long.Parse(line.Substring(34, 9).Trim());
                    var dt = line.Substring(44, 12).Trim();
                    try
                    {
                        entry.LastModifiedTimeUtc = DateTime.ParseExact(dt, "MMM  d HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite);
                    }
                    catch (FormatException)
                    {
                        entry.LastModifiedTimeUtc = DateTime.ParseExact(dt, "MMM  d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite);
                    }
                    result.Add(entry);
                }
            }
            catch (Exception ex)
            {
                return MakeError<FileSystemEntry[]>(ex.Message);
            }
            return MakeResult<FileSystemEntry[]>(result.ToArray());
        }

        public ResultOrError<string> ListEntriesRaw(string path)
        {
            if (path == null)
                path = "/";
            if (path.StartsWith("-"))
                path = ". " + path;
            string newPath = ResolvePath(path);
            List<string> result = new List<string>();
            try
            {
                var lines = shell.ExecuteSimple("ls " + newPath, 1000, true);
                return MakeResult<string>(lines);
            }
            catch (Exception ex)
            {
                return MakeError<string>(ex.Message);
            }
        }

        public ResultOrError<long> GetFileSize(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                var size = shell.ExecuteSimple("stat -c%s \"" + newPath + "\"", 1000, true);
                return MakeResult<long>(long.Parse(size));
            }
            catch (Exception ex)
            {
                return MakeError<long>(ex.Message);
            }
        }

        public ResultOrError<DateTime> GetLastModifiedTimeUtc(string path)
        {
            string newPath = ResolvePath(path);
            try
            {
                var time = shell.ExecuteSimple("stat -c%Z \"" + newPath + "\"", 1000, true);
                return MakeResult<DateTime>(DateTime.FromFileTime(long.Parse(time)));
            }
            catch (Exception ex)
            {
                return MakeError<DateTime>(ex.Message);
            }
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
            string newPath = ResolvePath(path);
            try
            {
                shell.ExecuteSimple(string.Format("chmod {0} {1}", mode, newPath), 1000, true);
                return ResultOrError<bool>.MakeResult(true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }
        }

        public ResultOrError<bool> SetLastModifiedTimeUtc(string path, DateTime time)
        {
            string newPath = ResolvePath(path);
            try
            {
                shell.ExecuteSimple(string.Format("touch -ct {0:yyyyMMddHHmm.ss} \"{1}\"", time, newPath), 1000, true);
                return ResultOrError<bool>.MakeResult(true);
            }
            catch (Exception ex)
            {
                return MakeError<bool>(ex.Message);
            }
        }
    }
}
