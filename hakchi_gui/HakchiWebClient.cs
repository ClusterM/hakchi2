using System;
using System.Net;

namespace com.clusterrr.hakchi_gui
{
    class HakchiWebClient : WebClient
    {
        public static readonly string UserAgent = $"Hakchi2 CE/{Shared.AppVersion.ToString()} (https://github.com/TeamShinkansen/Hakchi2-CE)";
        public string Method
        {
            get;
            set;
        }

        public HakchiWebClient() {
            this.Headers.Add(HttpRequestHeader.UserAgent, HakchiWebClient.UserAgent);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);

            if (!string.IsNullOrEmpty(Method))
                webRequest.Method = Method;

            return webRequest;
        }
    }
}
