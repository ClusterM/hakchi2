using com.clusterrr.hakchi_gui.Properties;
using System;
using System.IO;
using System.Net;
using System.Threading;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class WebClientTasks
    {
        public static TaskFunc DownloadFile(string url, string fileName)
        {
            return (Tasker tasker, Object sync) =>
            {
                Conclusion result = Conclusion.Success;
                var wc = new WebClient();
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(async (object sender, DownloadProgressChangedEventArgs e) =>
                {
                    tasker.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
                    tasker.SetStatus(String.Format(Resources.DownloadingProgress, Shared.SizeSuffix(e.BytesReceived), Shared.SizeSuffix(e.TotalBytesToReceive)));
                });
                wc.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        File.Delete(fileName);
                        result = Conclusion.Error;
                    }
                };
                var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), fileName);
                new Thread(() =>
                {
                    while (true)
                    {
                        if (tasker.TaskConclusion == Conclusion.Abort)
                        {
                            wc.CancelAsync();
                            break;
                        }
                        if (downloadTask.IsCanceled || downloadTask.IsCompleted || downloadTask.IsFaulted)
                            break;

                        Thread.Sleep(100);
                    }
                }).Start();
                downloadTask.Wait();
                return result;
            };
        }
    }
}
