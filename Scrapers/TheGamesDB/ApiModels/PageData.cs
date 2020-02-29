using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PageData<ResponseType>
    {
        [JsonProperty("previous", NullValueHandling = NullValueHandling.Ignore)]
        public string Previous { get; internal set; }

        [JsonProperty("current", NullValueHandling = NullValueHandling.Ignore)]
        public string Current { get; internal set; }

        [JsonProperty("next", NullValueHandling = NullValueHandling.Ignore)]
        public string Next { get; internal set; }

        [JsonIgnore]
        public Type Type { get => typeof(ResponseType); }

        public Task<ResponseType> GetPreviousPage() => API.ApiRequest<ResponseType>(Previous);
        public Task<ResponseType> GetNextPage() => API.ApiRequest<ResponseType>(Next);
    }
}
