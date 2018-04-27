using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    /// <summary>
    /// Class to represent any file within the directory structure of the games/applications.
    /// </summary>
    public class ApplicationFileInfo
    {
        public string FilePath
        { get; set; }

        public long FileSize
        { get; set; }

        public DateTime ModifiedTime
        { get; set; }

        public string LocalFilePath
        { get; set; }

        public Stream FileStream
        { get; set; }

        public ApplicationFileInfo()
        { }

        ~ApplicationFileInfo()
        {
            if (FileStream != null)
            {
                FileStream.Dispose();
                FileStream = null;
            }
        }

        public ApplicationFileInfo(string filepath, long filesize, DateTime modifiedTime, string localfilepath = null)
        {
            this.FilePath = filepath;
            this.FileSize = filesize;
            this.ModifiedTime = modifiedTime;
            this.LocalFilePath = localfilepath;
            this.FileStream = null;
        }

        public ApplicationFileInfo(string filepath, DateTime modifiedTime, Stream localfiledata)
        {
            this.FilePath = filepath;
            this.FileSize = localfiledata.Length;
            this.ModifiedTime = modifiedTime;
            this.LocalFilePath = null;
            this.FileStream = localfiledata;
        }

        public override bool Equals(object obj)
        {
            var info = obj as ApplicationFileInfo;
            var preliminaryEqual =
                info != null &&
                FilePath == info.FilePath &&
                FileSize == info.FileSize;
            // check duration and allow 3 seconds leeway (accounting for FAT32 imprecise date/time property)
            return preliminaryEqual && ModifiedTime.Subtract(info.ModifiedTime).Duration() < TimeSpan.FromSeconds(3);
        }

        public override int GetHashCode()
        {
            var hashCode = -1706955063;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + FileSize.GetHashCode();
            //hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedTime.ToString()); // don't count date in the hashcode, because i have to test with imprecision
            return hashCode;
        }

        public static HashSet<ApplicationFileInfo> GetApplicationFileInfoForDirectory(string rootDirectory, string targetDirectory = null, bool recursive = true, string[] skipFiles = null)
        {
            var fileInfoSet = new HashSet<ApplicationFileInfo>();
            var filepaths = Directory.GetFiles(rootDirectory, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            targetDirectory = targetDirectory ?? string.Empty;
            if (!string.IsNullOrEmpty(targetDirectory))
                targetDirectory = "/" + targetDirectory.Trim('/');

            foreach (string path in filepaths)
            {
                if (skipFiles != null && skipFiles.Contains(Path.GetFileName(path)))
                    continue;
                // make the filepath match what we'd get back from the console
                string canonicalPath = "." + targetDirectory + path.Remove(0, rootDirectory.Length).Replace("\\", "/");
                FileInfo f = new FileInfo(path);
                fileInfoSet.Add(new ApplicationFileInfo(canonicalPath, f.Length, f.LastWriteTimeUtc, path));
            }

            return fileInfoSet;
        }

        public static HashSet<ApplicationFileInfo> GetApplicationFileInfoFromConsoleOutput(string output)
        {
            var fileInfoSet = new HashSet<ApplicationFileInfo>();

            foreach (Match infoMatch in Regex.Matches(output, "^(.*?) (\\d+) (\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}(?:\\.\\d+)?)?$", RegexOptions.Multiline))
            {
                long filesize = long.Parse(infoMatch.Groups[2].Value);
                DateTime lastWriteTime = DateTime.Parse(infoMatch.Groups[3].Value);
                fileInfoSet.Add(new ApplicationFileInfo(infoMatch.Groups[1].Value, filesize, lastWriteTime));
            }

            return fileInfoSet;
        }

        public static ApplicationFileInfo GetApplicationFileInfo(string fullPath, string targetDirectory = null)
        {
            targetDirectory = targetDirectory ?? string.Empty;
            if (!string.IsNullOrEmpty(targetDirectory))
                targetDirectory = "/" + targetDirectory.Trim('/');

            string canonicalPath = "." + targetDirectory + "/" + Path.GetFileName(fullPath);
            FileInfo f = new FileInfo(fullPath);
            return new ApplicationFileInfo(canonicalPath, f.Length, f.LastWriteTimeUtc, fullPath);
        }

        public static void DebugListHashSet(IEnumerable<ApplicationFileInfo> localGameSet)
        {
            Trace.WriteLine("HashSet Listing:");
            foreach (var afi in localGameSet)
                Trace.WriteLine($"{afi.FilePath} [{afi.LocalFilePath ?? "-"}]{(afi.FileStream == null ? "" : " [stream]")} {afi.FileSize} bytes {afi.ModifiedTime.ToString()}");
        }
    }

    /// <summary>
    /// Helper class for ApplicationFileInfo operations
    /// </summary>
    public static class ApplicationFileInfoExtensions
    {
        /// <summary>
        /// Allows copying "files" from one set into this one, as if they were files on a drive (overwrite files with same path, disregarding this hashset's conditions of modified date and size)
        /// </summary>
        public static HashSet<ApplicationFileInfo> CopyFilesTo(this HashSet<ApplicationFileInfo> first, HashSet<ApplicationFileInfo> second, bool overwrite = true)
        {
            var result = new HashSet<ApplicationFileInfo>(first);
            foreach (var i in second)
            {
                bool add = true;
                foreach (var j in result)
                {
                    if (j.FilePath.Equals(i.FilePath))
                    {
                        if (overwrite)
                            result.Remove(j);
                        else
                            add = false;
                        break;
                    }
                }
                if (add)
                    result.Add(i);
            }
            return result;
        }

        /// <summary>
        /// Returns cumulative size of files in the set
        /// </summary>
        public static long GetSize(this HashSet<ApplicationFileInfo> set, long padFileSize = -1)
        {
            long size = 0;
            foreach(var afi in set)
            {
                size += Shared.PadFileSize(afi.FileSize, padFileSize);
            }
            return size;
        }
    }
}
