using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public enum ExperienceType
    {
        Project,
        VolunteerWork,
        Other
    }

    public class OtherExperience
    {
        [JsonProperty("type")]
        public ExperienceType? Type { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("projectOrCompanyName")]
        public string? ProjectOrCompanyName { get; set; }

        [JsonProperty("startDate")]
        public int? StartDate { get; set; }

        [JsonProperty("endDate")]
        public int? EndDate { get; set; }

        [JsonProperty("accomplishments")]
        public List<string>? Accomplishments { get; set; }
    }
}
