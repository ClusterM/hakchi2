using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlatformInclude: PlatformBase
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, PlatformBase> Data { get; internal set; }
    }
}
