using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlatformsImagesData: BaseData
    {
        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, PlatformsImage> Images { get; internal set; }
    }
}
