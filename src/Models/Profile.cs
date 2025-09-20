using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public class Profile
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("skills")]
        public List<string>? Skills { get; set; }

        [JsonProperty("certifications")]
        public List<Certification>? Certifications { get; set; }

        [JsonProperty("jobExperiences")]
        public List<JobExperience>? JobExperiences { get; set; }

        [JsonProperty("otherExperiences")]
        public List<OtherExperience>? OtherExperiences { get; set; }

        [JsonProperty("education")]
        public List<Education>? Education { get; set; }
    }
}
