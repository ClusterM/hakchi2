using System.Net;

namespace com.clusterrr.hakchi_gui
{
    class HakchiWebClient : WebClient
    {
        public HakchiWebClient() {
            this.Headers.Add(HttpRequestHeader.UserAgent, $"Hakchi2 CE/{Shared.AppVersion.ToString()} (https://github.com/TeamShinkansen/Hakchi2-CE)");
        }
    }
}
