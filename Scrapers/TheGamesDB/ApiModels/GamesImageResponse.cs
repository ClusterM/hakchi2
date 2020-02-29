using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GamesImageResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public GamesImageData Data { get; internal set; }

        [JsonProperty("pages", NullValueHandling = NullValueHandling.Ignore)]
        public PageData<GamesImageResponse> Pages { get; internal set; }
    }
}
