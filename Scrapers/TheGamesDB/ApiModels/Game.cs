using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? ID { get; internal set; }

        [JsonProperty("game_title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; internal set; }

        [JsonProperty("release_date", NullValueHandling = NullValueHandling.Ignore)]
        public string ReleaseDate { get; internal set; }

        [JsonProperty("platform", NullValueHandling = NullValueHandling.Ignore)]
        public int? Platform { get; internal set; }

        [JsonProperty("players", NullValueHandling = NullValueHandling.Ignore)]
        public int? Players { get; internal set; }

        [JsonProperty("overview", NullValueHandling = NullValueHandling.Ignore)]
        public string Overview { get; internal set; }

        [JsonProperty("last_updated", NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdated { get; internal set; }

        [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
        public string Rating { get; internal set; }

        [JsonProperty("coop", NullValueHandling = NullValueHandling.Ignore)]
        public string Cooperative { get; internal set; }

        [JsonProperty("youtube", NullValueHandling = NullValueHandling.Ignore)]
        public string YouTube { get; internal set; }

        [JsonProperty("os", NullValueHandling = NullValueHandling.Ignore)]
        public string OperatingSystem { get; internal set; }

        [JsonProperty("processor", NullValueHandling = NullValueHandling.Ignore)]
        public string Processor { get; internal set; }

        [JsonProperty("hdd", NullValueHandling = NullValueHandling.Ignore)]
        public string HardDrive { get; internal set; }

        [JsonProperty("ram", NullValueHandling = NullValueHandling.Ignore)]
        public string RAM { get; internal set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public string Video { get; internal set; }

        [JsonProperty("sound", NullValueHandling = NullValueHandling.Ignore)]
        public string Sound { get; internal set; }

        [JsonProperty("developers", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Developers { get; internal set; }

        [JsonProperty("genres", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Genres { get; internal set; }

        [JsonProperty("publishers", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Publishers { get; internal set; }

        [JsonProperty("alternates", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Alternates { get; internal set; }
    }
}
