using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GenreData: BaseData
    {
        [JsonProperty("genres", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Genre> Genres { get; internal set; }
    }
}
