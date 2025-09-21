using GrindBreaker.Models;
using Newtonsoft.Json;

namespace GrindBreaker.Tests;

public class CandidacyModelTests
{
    [Fact]
    public void Candidacy_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var candidacy = new Candidacy();

        // Assert
        Assert.NotNull(candidacy.Id);
        Assert.NotEmpty(candidacy.Id);
        Assert.Equal(string.Empty, candidacy.Company);
        Assert.Equal(string.Empty, candidacy.Title);
        Assert.Null(candidacy.JobLink);
        Assert.Null(candidacy.JobDescription);
        Assert.Equal(0, candidacy.DateApplied);
        Assert.Equal(CandidacyStatus.ToApply, candidacy.Status);
        Assert.NotNull(candidacy.ApplicationSteps);
        Assert.Empty(candidacy.ApplicationSteps);
    }

    [Fact]
    public void Candidacy_Id_ShouldBeUniqueGuid()
    {
        // Act
        var candidacy1 = new Candidacy();
        var candidacy2 = new Candidacy();

        // Assert
        Assert.NotEqual(candidacy1.Id, candidacy2.Id);
        Assert.True(Guid.TryParse(candidacy1.Id, out _));
        Assert.True(Guid.TryParse(candidacy2.Id, out _));
    }

    [Fact]
    public void Candidacy_JsonSerialization_ShouldWorkCorrectly()
    {
        // Arrange
        var candidacy = new Candidacy
        {
            Company = "Test Company",
            Title = "Software Developer",
            JobLink = "https://example.com/job",
            JobDescription = "A great job opportunity",
            DateApplied = 1640995200, // 2022-01-01
            Status = CandidacyStatus.Applied,
            ApplicationSteps = new List<CandidacyStep>
            {
                new CandidacyStep
                {
                    Type = "Phone Interview",
                    Date = 1640995200,
                    Notes = "Initial screening"
                }
            }
        };

        // Act
        var json = JsonConvert.SerializeObject(candidacy);
        var deserializedCandidacy = JsonConvert.DeserializeObject<Candidacy>(json);

        // Assert
        Assert.NotNull(deserializedCandidacy);
        Assert.Equal(candidacy.Id, deserializedCandidacy.Id);
        Assert.Equal(candidacy.Company, deserializedCandidacy.Company);
        Assert.Equal(candidacy.Title, deserializedCandidacy.Title);
        Assert.Equal(candidacy.JobLink, deserializedCandidacy.JobLink);
        Assert.Equal(candidacy.JobDescription, deserializedCandidacy.JobDescription);
        Assert.Equal(candidacy.DateApplied, deserializedCandidacy.DateApplied);
        Assert.Equal(candidacy.Status, deserializedCandidacy.Status);
        Assert.NotNull(deserializedCandidacy.ApplicationSteps);
        Assert.Single(deserializedCandidacy.ApplicationSteps);
        Assert.Equal(candidacy.ApplicationSteps[0].Type, deserializedCandidacy.ApplicationSteps[0].Type);
        Assert.Equal(candidacy.ApplicationSteps[0].Date, deserializedCandidacy.ApplicationSteps[0].Date);
        Assert.Equal(candidacy.ApplicationSteps[0].Notes, deserializedCandidacy.ApplicationSteps[0].Notes);
    }

    [Fact]
    public void Candidacy_JsonSerialization_WithNullOptionalFields_ShouldWorkCorrectly()
    {
        // Arrange
        var candidacy = new Candidacy
        {
            Company = "Test Company",
            Title = "Software Developer",
            JobLink = null,
            JobDescription = null,
            DateApplied = 1640995200,
            Status = CandidacyStatus.ToApply,
            ApplicationSteps = new List<CandidacyStep>()
        };

        // Act
        var json = JsonConvert.SerializeObject(candidacy);
        var deserializedCandidacy = JsonConvert.DeserializeObject<Candidacy>(json);

        // Assert
        Assert.NotNull(deserializedCandidacy);
        Assert.Equal(candidacy.Company, deserializedCandidacy.Company);
        Assert.Equal(candidacy.Title, deserializedCandidacy.Title);
        Assert.Null(deserializedCandidacy.JobLink);
        Assert.Null(deserializedCandidacy.JobDescription);
        Assert.Equal(candidacy.Status, deserializedCandidacy.Status);
        Assert.NotNull(deserializedCandidacy.ApplicationSteps);
        Assert.Empty(deserializedCandidacy.ApplicationSteps);
    }

    [Fact]
    public void CandidacyStatus_AllValues_ShouldBeDefined()
    {
        // Act & Assert
        Assert.Equal(0, (int)CandidacyStatus.ToApply);
        Assert.Equal(1, (int)CandidacyStatus.Applied);
        Assert.Equal(2, (int)CandidacyStatus.PreInterview);
        Assert.Equal(3, (int)CandidacyStatus.PostInterview);
        Assert.Equal(4, (int)CandidacyStatus.Offered);
        Assert.Equal(5, (int)CandidacyStatus.Rejected);
        Assert.Equal(6, (int)CandidacyStatus.Ghosted);
        Assert.Equal(7, (int)CandidacyStatus.Withdrawn);
    }

    [Fact]
    public void CandidacyStatus_JsonSerialization_ShouldWorkCorrectly()
    {
        // Arrange
        var statuses = new[]
        {
            CandidacyStatus.ToApply,
            CandidacyStatus.Applied,
            CandidacyStatus.PreInterview,
            CandidacyStatus.PostInterview,
            CandidacyStatus.Offered,
            CandidacyStatus.Rejected,
            CandidacyStatus.Ghosted,
            CandidacyStatus.Withdrawn
        };

        foreach (var status in statuses)
        {
            // Act
            var json = JsonConvert.SerializeObject(status);
            var deserializedStatus = JsonConvert.DeserializeObject<CandidacyStatus>(json);

            // Assert
            Assert.Equal(status, deserializedStatus);
        }
    }

    [Fact]
    public void CandidacyStep_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var step = new CandidacyStep();

        // Assert
        Assert.Equal(string.Empty, step.Type);
        Assert.Equal(0, step.Date);
        Assert.Null(step.Notes);
    }

    [Fact]
    public void CandidacyStep_JsonSerialization_ShouldWorkCorrectly()
    {
        // Arrange
        var step = new CandidacyStep
        {
            Type = "Phone Interview",
            Date = 1640995200,
            Notes = "Initial screening call"
        };

        // Act
        var json = JsonConvert.SerializeObject(step);
        var deserializedStep = JsonConvert.DeserializeObject<CandidacyStep>(json);

        // Assert
        Assert.NotNull(deserializedStep);
        Assert.Equal(step.Type, deserializedStep.Type);
        Assert.Equal(step.Date, deserializedStep.Date);
        Assert.Equal(step.Notes, deserializedStep.Notes);
    }

    [Fact]
    public void CandidacyStep_JsonSerialization_WithNullNotes_ShouldWorkCorrectly()
    {
        // Arrange
        var step = new CandidacyStep
        {
            Type = "Technical Interview",
            Date = 1640995200,
            Notes = null
        };

        // Act
        var json = JsonConvert.SerializeObject(step);
        var deserializedStep = JsonConvert.DeserializeObject<CandidacyStep>(json);

        // Assert
        Assert.NotNull(deserializedStep);
        Assert.Equal(step.Type, deserializedStep.Type);
        Assert.Equal(step.Date, deserializedStep.Date);
        Assert.Null(deserializedStep.Notes);
    }

    [Fact]
    public void Candidacy_WithMultipleApplicationSteps_ShouldSerializeCorrectly()
    {
        // Arrange
        var candidacy = new Candidacy
        {
            Company = "Test Company",
            Title = "Software Developer",
            ApplicationSteps = new List<CandidacyStep>
            {
                new CandidacyStep
                {
                    Type = "Application Submitted",
                    Date = 1640995200,
                    Notes = "Applied online"
                },
                new CandidacyStep
                {
                    Type = "Phone Interview",
                    Date = 1641081600,
                    Notes = "Initial screening"
                },
                new CandidacyStep
                {
                    Type = "Technical Interview",
                    Date = 1641168000,
                    Notes = "Coding challenge"
                }
            }
        };

        // Act
        var json = JsonConvert.SerializeObject(candidacy);
        var deserializedCandidacy = JsonConvert.DeserializeObject<Candidacy>(json);

        // Assert
        Assert.NotNull(deserializedCandidacy);
        Assert.Equal(3, deserializedCandidacy.ApplicationSteps.Count);
        Assert.Equal("Application Submitted", deserializedCandidacy.ApplicationSteps[0].Type);
        Assert.Equal("Phone Interview", deserializedCandidacy.ApplicationSteps[1].Type);
        Assert.Equal("Technical Interview", deserializedCandidacy.ApplicationSteps[2].Type);
    }

    [Fact]
    public void Candidacy_DateApplied_ShouldHandleUnixTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

        var candidacy = new Candidacy
        {
            DateApplied = unixTimestamp
        };

        // Act
        var convertedDateTime = DateTimeOffset.FromUnixTimeSeconds(candidacy.DateApplied).UtcDateTime;

        // Assert
        Assert.Equal(dateTime, convertedDateTime);
    }

    [Fact]
    public void CandidacyStep_Date_ShouldHandleUnixTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2022, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

        var step = new CandidacyStep
        {
            Date = unixTimestamp
        };

        // Act
        var convertedDateTime = DateTimeOffset.FromUnixTimeSeconds(step.Date).UtcDateTime;

        // Assert
        Assert.Equal(dateTime, convertedDateTime);
    }
}
