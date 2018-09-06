using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.hakchi_gui
{
    class HTTPHelpers
    {
        public struct StatusStream
        {
            public HttpStatusCode Status { get; private set; }
            public Stream Stream { get; private set; }
            public long Length { get; private set; }
            public StatusStream(HttpStatusCode status, Stream stream = null, long length = 0)
            {
                this.Status = status;
                this.Stream = stream;
                this.Length = length;
            }
        }
        public async static Task<HttpStatusCode> GetHTTPStatusCodeAsync(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.AllowAutoRedirect = true;
                HttpWebResponse response = (HttpWebResponse)(request.GetResponse());
                return response.StatusCode;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return ((HttpWebResponse)e.Response).StatusCode;
                }
                throw e;
            }
        }

        public async static Task<StatusStream> GetHTTPResponseStreamAsync(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.AllowAutoRedirect = true;
                HttpWebResponse response = (HttpWebResponse)(request.GetResponse());
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    return new StatusStream(response.StatusCode, response.GetResponseStream(), response.ContentLength);
                }
                return new StatusStream(response.StatusCode);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return new StatusStream(((HttpWebResponse)e.Response).StatusCode);
                }
                throw e;
            }

        }

        public async static Task<string> GetHTTPResponseStringAsync(string url, Encoding encoding = null)
        {
            var response = await GetHTTPResponseStreamAsync(url);

            if (response.Status == HttpStatusCode.OK)
            {
                using (var sr = new StreamReader(response.Stream, encoding ?? Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }

            return null;
        }
    }
}
