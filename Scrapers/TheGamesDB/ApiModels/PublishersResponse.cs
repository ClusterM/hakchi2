using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PublishersResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public PublishersData Data { get; internal set; }
    }
}
