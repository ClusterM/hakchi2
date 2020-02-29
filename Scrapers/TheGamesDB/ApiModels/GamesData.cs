using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GamesData: BaseData
    {
        [JsonProperty("games", NullValueHandling = NullValueHandling.Ignore)]
        public Game[] Games { get; internal set; }
    }
}
