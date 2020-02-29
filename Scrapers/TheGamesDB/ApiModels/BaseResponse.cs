using Newtonsoft.Json;
namespace TeamShinkansen.Scrapers.TheGamesDB.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseResponse
    {
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public int Code { get; internal set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; internal set; }

        [JsonProperty("remaining_monthly_allowance", NullValueHandling = NullValueHandling.Ignore)]
        public int RemainingMonthlyAllowance { get; internal set; }

        [JsonProperty("extra_allowance", NullValueHandling = NullValueHandling.Ignore)]
        public int ExtraAllowance { get; internal set; }
    }
}
