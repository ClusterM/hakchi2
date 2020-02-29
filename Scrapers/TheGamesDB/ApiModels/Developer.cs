using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Developer
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int ID { get; internal set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; internal set; }
    }
}
