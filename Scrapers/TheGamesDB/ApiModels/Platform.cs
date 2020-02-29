using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Platform: PlatformBase
    {
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; internal set; }

        [JsonProperty("console", NullValueHandling = NullValueHandling.Ignore)]
        public string Console { get; internal set; }

        [JsonProperty("controller", NullValueHandling = NullValueHandling.Ignore)]
        public string Controller { get; internal set; }

        [JsonProperty("developer", NullValueHandling = NullValueHandling.Ignore)]
        public string Developer { get; internal set; }
    }
}
