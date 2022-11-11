using com.clusterrr.util;
using SharpCompress.Readers;
using System;
using System.IO;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class ArchiveTasks
    {
        public static TaskFunc ExtractArchive(String fileName, string destinationFolder)
        {
            return (Tasker tasker, Object syncObject) =>
            {
                using (var fileStream = File.OpenRead(fileName))
                using (var trackStream = new TrackableStream(fileStream))
                {
                    trackStream.OnProgress += tasker.OnProgress;

                    using (var reader = ReaderFactory.Open(trackStream))
                    {
                        reader.WriteAllToDirectory(destinationFolder, new SharpCompress.Common.ExtractionOptions() { ExtractFullPath = true, Overwrite = true, PreserveFileTime = true });

                        return Conclusion.Success;
                    }
                }
            };
        }
    }
}
