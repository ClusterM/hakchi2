using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GamesImageData : BaseResponse
    {
        [JsonProperty("base_url", NullValueHandling = NullValueHandling.Ignore)]
        public BaseUrl BaseURL { get; internal set; }

        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, BoxartImage[]> Images { get; internal set; }
    }
}
