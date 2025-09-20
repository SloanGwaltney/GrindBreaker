using GrindBreaker.Models;
using GrindBreaker.Repositories;
using Newtonsoft.Json;

namespace GrindBreaker.Tests;

public class ProfileRepositoryTests : IDisposable
{
    private readonly string _testDataPath;
    private readonly ProfileRepository _repository;

    public ProfileRepositoryTests()
    {
        _testDataPath = Path.Combine(Path.GetTempPath(), "GrindBreakerTests", Guid.NewGuid().ToString());
        _repository = new ProfileRepository(_testDataPath);
    }

    [Fact]
    public void Constructor_WithNullDataPath_ShouldUseDefaultPath()
    {
        // Arrange & Act
        var repository = new ProfileRepository(null);

        // Assert
        Assert.NotNull(repository);
        // Note: We can't easily test the default path without exposing it, but we can verify the constructor doesn't throw
    }

    [Fact]
    public void Constructor_WithCustomDataPath_ShouldCreateDirectory()
    {
        // Arrange
        var customPath = Path.Combine(Path.GetTempPath(), "CustomGrindBreaker", Guid.NewGuid().ToString());

        // Act
        var repository = new ProfileRepository(customPath);

        // Assert
        Assert.True(Directory.Exists(customPath));
        Directory.Delete(customPath, true);
    }

    [Fact]
    public void GetProfile_WhenFileDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = _repository.GetProfile();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetProfile_WhenFileExists_ShouldReturnProfile()
    {
        // Arrange
        var profile = CreateTestProfile();
        var filePath = Path.Combine(_testDataPath, "profile.json");
        var json = JsonConvert.SerializeObject(profile, Formatting.Indented);
        File.WriteAllText(filePath, json);

        // Act
        var result = _repository.GetProfile();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.FirstName, result.FirstName);
        Assert.Equal(profile.LastName, result.LastName);
        Assert.Equal(profile.Email, result.Email);
    }

    [Fact]
    public void GetProfile_WhenFileContainsInvalidJson_ShouldReturnNull()
    {
        // Arrange
        var filePath = Path.Combine(_testDataPath, "profile.json");
        File.WriteAllText(filePath, "invalid json content");

        // Act
        var result = _repository.GetProfile();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SaveProfile_WithValidProfile_ShouldReturnTrue()
    {
        // Arrange
        var profile = CreateTestProfile();

        // Act
        var result = _repository.SaveProfile(profile);

        // Assert
        Assert.True(result);
        Assert.True(File.Exists(Path.Combine(_testDataPath, "profile.json")));
    }

    [Fact]
    public void SaveProfile_WithValidProfile_ShouldSaveCorrectData()
    {
        // Arrange
        var profile = CreateTestProfile();

        // Act
        _repository.SaveProfile(profile);

        // Assert
        var filePath = Path.Combine(_testDataPath, "profile.json");
        var json = File.ReadAllText(filePath);
        var savedProfile = JsonConvert.DeserializeObject<Profile>(json);
        
        Assert.NotNull(savedProfile);
        Assert.Equal(profile.FirstName, savedProfile.FirstName);
        Assert.Equal(profile.LastName, savedProfile.LastName);
        Assert.Equal(profile.Email, savedProfile.Email);
    }

    [Fact]
    public void SaveProfile_WithNullProfile_ShouldReturnFalse()
    {
        // Act
        var result = _repository.SaveProfile(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SaveProfile_WithEmptyProfile_ShouldReturnTrue()
    {
        // Arrange
        var profile = new Profile();

        // Act
        var result = _repository.SaveProfile(profile);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DeleteProfile_WhenFileDoesNotExist_ShouldReturnTrue()
    {
        // Act
        var result = _repository.DeleteProfile();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DeleteProfile_WhenFileExists_ShouldReturnTrue()
    {
        // Arrange
        var profile = CreateTestProfile();
        _repository.SaveProfile(profile);

        // Act
        var result = _repository.DeleteProfile();

        // Assert
        Assert.True(result);
        Assert.False(File.Exists(Path.Combine(_testDataPath, "profile.json")));
    }

    [Fact]
    public void SaveProfile_ThenGetProfile_ShouldReturnSameData()
    {
        // Arrange
        var originalProfile = CreateTestProfile();

        // Act
        _repository.SaveProfile(originalProfile);
        var retrievedProfile = _repository.GetProfile();

        // Assert
        Assert.NotNull(retrievedProfile);
        Assert.Equal(originalProfile.FirstName, retrievedProfile.FirstName);
        Assert.Equal(originalProfile.LastName, retrievedProfile.LastName);
        Assert.Equal(originalProfile.Email, retrievedProfile.Email);
        Assert.Equal(originalProfile.PhoneNumber, retrievedProfile.PhoneNumber);
    }

    [Fact]
    public void SaveProfile_OverwritesExistingProfile()
    {
        // Arrange
        var firstProfile = CreateTestProfile();
        firstProfile.FirstName = "First";
        
        var secondProfile = CreateTestProfile();
        secondProfile.FirstName = "Second";

        // Act
        _repository.SaveProfile(firstProfile);
        _repository.SaveProfile(secondProfile);
        var retrievedProfile = _repository.GetProfile();

        // Assert
        Assert.NotNull(retrievedProfile);
        Assert.Equal("Second", retrievedProfile.FirstName);
    }

    private static Profile CreateTestProfile()
    {
        return new Profile
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "123-456-7890",
            Skills = new List<string> { "C#", "JavaScript", "SQL" },
            Certifications = new List<Certification>
            {
                new Certification
                {
                    Name = "Microsoft Certified Developer",
                    Description = "Microsoft certification",
                    EarnedDate = 2023,
                    Link = "https://example.com/cert"
                }
            },
            JobExperiences = new List<JobExperience>
            {
                new JobExperience
                {
                    Company = "Test Company",
                    Title = "Software Developer",
                    Location = "Remote",
                    StartDate = 2022,
                    EndDate = 2023,
                    Accomplishments = new List<string> { "Built API", "Improved performance" }
                }
            },
            OtherExperiences = new List<OtherExperience>
            {
                new OtherExperience
                {
                    Type = ExperienceType.Project,
                    Title = "Open Source Contributor",
                    ProjectOrCompanyName = "GitHub Project",
                    StartDate = 2021,
                    EndDate = 2022,
                    Accomplishments = new List<string> { "Fixed bugs", "Added features" }
                }
            },
            Education = new List<Education>
            {
                new Education
                {
                    AwardedBy = "Test University",
                    CredentialEarned = "Bachelor of Computer Science",
                    EndDate = 2020
                }
            }
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataPath))
        {
            Directory.Delete(_testDataPath, true);
        }
    }
}
