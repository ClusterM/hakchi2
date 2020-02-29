using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BoxartInclude: BaseData
    {
        [JsonProperty("base_url", NullValueHandling = NullValueHandling.Ignore)]
        public BaseUrl BaseURL { get; internal set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, BoxartImage[]> Data { get; internal set; }
    }
}
