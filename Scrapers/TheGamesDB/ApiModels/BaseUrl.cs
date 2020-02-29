using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseUrl
    {
        [JsonProperty("original", NullValueHandling = NullValueHandling.Ignore)]
        public string Original { get; internal set; }
        [JsonProperty("small", NullValueHandling = NullValueHandling.Ignore)]
        public string Small { get; internal set; }
        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumbnail { get; internal set; }
        [JsonProperty("cropped_center_thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string CroppedCenterThumbnail { get; internal set; }
        [JsonProperty("medium", NullValueHandling = NullValueHandling.Ignore)]
        public string Medium { get; internal set; }
        [JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
        public string Large { get; internal set; }
    }
}
