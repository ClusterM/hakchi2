using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GamesResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public GamesData Data { get; internal set; }

        [JsonProperty("include", NullValueHandling = NullValueHandling.Ignore)]
        public Include Includes { get; internal set; }

        [JsonProperty("pages", NullValueHandling = NullValueHandling.Ignore)]
        public PageData<GamesResponse> Pages { get; internal set; }
    }
}
