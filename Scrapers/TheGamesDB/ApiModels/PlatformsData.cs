using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlatformsData: BaseData
    {
        [JsonProperty("platforms", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Platform> Platforms { get; internal set; }
    }
}
