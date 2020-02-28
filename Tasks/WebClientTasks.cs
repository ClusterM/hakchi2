using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class WebClientTasks
    {
        public static TaskFunc DownloadFile(string url, string fileName, bool successOnError = false, bool onlyLatest = false, DateTime? comparisonDate = null)
        {
            return (Tasker tasker, Object sync) =>
            {
                Conclusion result = Conclusion.Success;
                try
                {
                    var wc = new HakchiWebClient();
                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                    wc.Method = "HEAD";

                    wc.DownloadData(url);

                    if (comparisonDate == null && File.Exists(fileName))
                    {
                        comparisonDate = File.GetLastWriteTime(fileName);
                    }

                    if (onlyLatest && comparisonDate != null)
                    {
                        var date = DateTime.ParseExact(wc.ResponseHeaders.Get("Last-Modified"),
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);

                        if (comparisonDate >= date)
                        {
                            return Conclusion.Success;
                        }
                    }

                    wc.Method = "GET";

                    Debug.WriteLine($"Downloading: {url} to {fileName}");

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
                        else
                        {
                            try
                            {
                                var date = DateTime.ParseExact(wc.ResponseHeaders.Get("Last-Modified"),
                                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                CultureInfo.InvariantCulture.DateTimeFormat,
                                DateTimeStyles.AssumeUniversal);

                                File.SetLastWriteTimeUtc(fileName, date);
                            }
                            catch (Exception) { }
                        }
                    };
                    var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), fileName);
                    new Thread(() =>
                    {
                        while (true)
                        {
                            if (tasker.TaskConclusion == Conclusion.Abort)
                            {
                                Debug.WriteLine("Download Aborted");
                                wc.CancelAsync();
                                break;
                            }
                            if (downloadTask.IsCanceled || downloadTask.IsCompleted || downloadTask.IsFaulted)
                                break;

                            Thread.Sleep(100);
                        }
                    }).Start();
                    downloadTask.Wait();
                } 
                catch (WebException e)
                {
                    if (!successOnError)
                        throw e;
                }
                return result;
            };
        }
    }
}
