using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeveloperData: BaseData
    {
        [JsonProperty("developers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Developer> Developers { get; internal set; }
    }
}
