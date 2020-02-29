using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Include
    {
        [JsonProperty("boxart", NullValueHandling = NullValueHandling.Ignore)]
        public BoxartInclude Boxart { get; internal set; }
        [JsonProperty("platform", NullValueHandling = NullValueHandling.Ignore)]
        public PlatformInclude Platform { get; internal set; }
    }
}
