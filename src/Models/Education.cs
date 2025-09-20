using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public class Education
    {
        [JsonProperty("awardedBy")]
        public string? AwardedBy { get; set; }

        [JsonProperty("credentialEarned")]
        public string? CredentialEarned { get; set; }

        [JsonProperty("endDate")]
        public int? EndDate { get; set; }
    }
}
