using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PublishersData: BaseData
    {
        [JsonProperty("publishers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Publisher> Publishers { get; internal set; }
    }
}
