using System.IO;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    static class FileTasks
    {
        public static TaskFunc DeleteFile(string path, bool ignoreMissing = false)
        {
            return (Tasker tasker, object syncObject) =>
            {
                if (File.Exists(path) || ignoreMissing == false)
                {
                    File.Delete(path);
                }
                return Conclusion.Success;
            };
        }
        public static TaskFunc MoveFile(string sourceFileName, string destFileName, bool overwrite = false, bool ignoreMissing = false)
        {
            return (Tasker tasker, object syncObject) =>
            {
                if (!File.Exists(sourceFileName))
                {
                    if (ignoreMissing)
                    {
                        return Conclusion.Success;
                    }

                    throw new FileNotFoundException($"Could not find file {sourceFileName}");
                }

                if (overwrite && File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(destFileName));

                File.Move(sourceFileName, destFileName);

                return Conclusion.Success;
            };
        }
        public static TaskFunc CopyFile(string sourceFileName, string destFileName, bool overwrite = false, bool ignoreMissing = false)
        {
            return (Tasker tasker, object syncObject) =>
            {
                if (!File.Exists(sourceFileName))
                {
                    if (ignoreMissing)
                    {
                        return Conclusion.Success;
                    }

                    throw new FileNotFoundException($"Could not find file {sourceFileName}");
                }

                if (overwrite && File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(destFileName));

                File.Copy(sourceFileName, destFileName);

                return Conclusion.Success;
            };
        }
    }
}
