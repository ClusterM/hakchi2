using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BoxartImage
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int ID { get; internal set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; internal set; }

        [JsonProperty("side", NullValueHandling = NullValueHandling.Ignore)]
        public string Side { get; internal set; }

        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; internal set; }

        [JsonProperty("resolution", NullValueHandling = NullValueHandling.Ignore)]
        public string Resolution { get; internal set; }
    }
}
