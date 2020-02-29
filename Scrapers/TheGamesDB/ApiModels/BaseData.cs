using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseData
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int Count { get; internal set; }
    }
}
