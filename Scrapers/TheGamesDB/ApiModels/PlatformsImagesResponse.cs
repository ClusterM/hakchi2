using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlatformsImagesResponse : BaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public PlatformsImagesData Data { get; internal set; }

        [JsonProperty("pages", NullValueHandling = NullValueHandling.Ignore)]
        public PageData<PlatformsImagesResponse> Pages { get; internal set; }
    }
}
