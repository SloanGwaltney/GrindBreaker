using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public class Certification
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("earnedDate")]
        public int? EarnedDate { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("link")]
        public string? Link { get; set; }
    }
}
