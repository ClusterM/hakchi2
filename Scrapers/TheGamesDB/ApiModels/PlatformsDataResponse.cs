using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlatformDataResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public PlatformsData Data { get; internal set; }
    }
}
