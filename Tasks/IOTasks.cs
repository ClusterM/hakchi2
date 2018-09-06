using System;
using System.IO;
using System.Security.AccessControl;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public static class IOTasks
    {
        public static TaskFunc FileDelete(string path)
        {
            return (Tasker tasker, Object sync) =>
            {
                File.Delete(path);
                return Conclusion.Success;
            };
        }
        public static TaskFunc FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            return (Tasker tasker, Object sync) =>
            {
                File.Copy(sourceFileName, destFileName, overwrite);
                return Conclusion.Success;
            };
        }
        public static TaskFunc FileMove(string sourceFileName, string destFileName)
        {
            return (Tasker tasker, Object sync) =>
            {
                File.Move(sourceFileName, destFileName);
                return Conclusion.Success;
            };
        }

        public static TaskFunc DirectoryCreate(string path)
        {
            return (Tasker tasker, Object sync) =>
            {
                Directory.CreateDirectory(path);
                return Conclusion.Success;
            };
        }
        public static TaskFunc DirectoryCreate(string path, DirectorySecurity directorySecurity)
        {
            return (Tasker tasker, Object sync) =>
            {
                Directory.CreateDirectory(path, directorySecurity);
                return Conclusion.Success;
            };
        }
        public static TaskFunc DirectoryDelete(string path, bool recursive = false)
        {
            return (Tasker tasker, Object sync) =>
            {
                Directory.Delete(path, recursive);
                return Conclusion.Success;
            };
        }
        public static TaskFunc DirectoryMove(string sourceDirName, string destDirName)
        {
            return (Tasker tasker, Object sync) =>
            {
                Directory.Move(sourceDirName, destDirName);
                return Conclusion.Success;
            };
        }
    }
}
