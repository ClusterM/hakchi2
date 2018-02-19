using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    /// <summary>
    /// Class to represent any file within the directory structure of the games/applications.
    /// </summary>
    class ApplicationFileInfo
    {
        public string Filepath
        { get; set; }

        public long Filesize
        { get; set; }

        public DateTime ModifiedTime
        { get; set; }

        public bool IsTarStreamRefFile
        { get; set; }

        public ApplicationFileInfo()
        { }

        public ApplicationFileInfo(string filepath, long filesize, DateTime modifiedTime, bool isTarStreamRefFile)
        {
            this.Filepath = filepath;
            this.Filesize = filesize;
            this.ModifiedTime = modifiedTime;
            this.IsTarStreamRefFile = isTarStreamRefFile;
        }

        public override bool Equals(object obj)
        {
            var info = obj as ApplicationFileInfo;
            return info != null &&
                   Filepath == info.Filepath &&
                   Filesize == info.Filesize &&
                   ModifiedTime.ToString().Equals(info.ModifiedTime.ToString());
        }

        public override int GetHashCode()
        {
            var hashCode = -1706955063;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Filepath);
            hashCode = hashCode * -1521134295 + Filesize.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedTime.ToString());
            return hashCode;
        }

        public static HashSet<ApplicationFileInfo> GetApplicationFileInfoForDirectory(string rootDirectory, bool recursive = true)
        {
            var fileInfoSet = new HashSet<ApplicationFileInfo>();
            var filepaths = Directory.GetFiles(rootDirectory, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (string path in filepaths)
            {
                // follow through on tarstreamref files
                string pathToRead = path;
                bool isRefFile = false;

                if (TarStream.refRegex.IsMatch(path))
                {
                    pathToRead = File.ReadAllText(path);
                    isRefFile = true;
                }

                // make the filepath match what we'd get back from the console
                string canonicalPath = "." + path.Remove(0, rootDirectory.Length).Replace("\\", "/").Replace(".tarstreamref", "");
                FileInfo f = new FileInfo(pathToRead);
                fileInfoSet.Add(new ApplicationFileInfo(canonicalPath, f.Length, f.LastWriteTimeUtc, isRefFile));
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
                fileInfoSet.Add(new ApplicationFileInfo(infoMatch.Groups[1].Value, filesize, lastWriteTime, false));
            }

            return fileInfoSet;
        }

    }
}
