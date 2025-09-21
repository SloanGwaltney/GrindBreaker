using Newtonsoft.Json;

namespace GrindBreaker.Models
{
    public class Candidacy
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("company")]
        public string Company { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("jobLink")]
        public string? JobLink { get; set; }

        [JsonProperty("jobDescription")]
        public string? JobDescription { get; set; }

        [JsonProperty("dateApplied")]
        public long DateApplied { get; set; } // Unix timestamp

        [JsonProperty("status")]
        public CandidacyStatus Status { get; set; }

        [JsonProperty("applicationSteps")]
        public List<CandidacyStep> ApplicationSteps { get; set; } = new();
    }

    public enum CandidacyStatus
    {
        ToApply,
        Applied,
        PreInterview,
        PostInterview,
        Offered,
        Rejected,
        Ghosted,
        Withdrawn
    }

    public class CandidacyStep
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("date")]
        public long Date { get; set; } // Unix timestamp

        [JsonProperty("notes")]
        public string? Notes { get; set; }
    }
}
