using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GenreResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public GenreData Data { get; internal set; }
    }
}
