using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
