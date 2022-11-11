using com.clusterrr.util;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class WebClientTasks
    {
        public static TaskFunc DownloadFile(string url, string fileName, bool successOnError = false, bool onlyLatest = false, DateTime? comparisonDate = null, bool gunzip = false)
        {
            return (Tasker tasker, Object sync) =>
            {
                Conclusion result = Conclusion.Success;

                Debug.WriteLine($"Downloading: {url} to {fileName}");

                if (comparisonDate == null && File.Exists(fileName))
                {
                    comparisonDate = File.GetLastWriteTime(fileName);
                }

                var wr = HttpWebRequest.Create(url) as HttpWebRequest;
                wr.UserAgent = HakchiWebClient.UserAgent;

                try
                {
                    using (var response = wr.GetResponse())
                    {
                        var headers = response.Headers;
                        var contentLength = headers.AllKeys.Contains("Content-Length") ? response.ContentLength : 0;

                        var date = DateTime.Now;

                        if (headers.AllKeys.Contains("Last-Modified"))
                        {
                            date = DateTime.ParseExact(headers["Last-Modified"],
                            "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                            CultureInfo.InvariantCulture.DateTimeFormat,
                            DateTimeStyles.AssumeUniversal);

                            if (onlyLatest && comparisonDate != null && comparisonDate >= date)
                            {
                                response.Close();
                                return Conclusion.Success;
                            }
                        }

                        using (var webStream = response.GetResponseStream())
                        using (var trackableStream = new TrackableStream(webStream))
                        {
                            trackableStream.OnProgress += (progress, max) =>
                            {
                                tasker.SetStatus($"{Shared.SizeSuffix(progress)}{(contentLength > 0 ? $" / {Shared.SizeSuffix(contentLength)}" : "")}");
                                tasker.SetProgress(progress, contentLength);
                            };

                            using (var outputFile = File.Create(fileName))
                            {

                                if (gunzip)
                                {
                                    using (var gzipStream = new GZipStream(trackableStream, CompressionMode.Decompress))
                                    {
                                        gzipStream.CopyTo(outputFile);
                                    }
                                }
                                else
                                {
                                    trackableStream.CopyTo(outputFile);
                                }
                            }
                            File.SetLastWriteTime(fileName, date);
                        }
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exception e)
                {
                    if (!successOnError)
                        throw e;
                }

                return result;
            };
        }
    }
}
