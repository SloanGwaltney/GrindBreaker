using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public class JobExperience
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("company")]
        public string? Company { get; set; }

        [JsonProperty("location")]
        public string? Location { get; set; }

        [JsonProperty("startDate")]
        public int? StartDate { get; set; }

        [JsonProperty("endDate")]
        public int? EndDate { get; set; }

        [JsonProperty("accomplishments")]
        public List<string>? Accomplishments { get; set; }
    }
}
