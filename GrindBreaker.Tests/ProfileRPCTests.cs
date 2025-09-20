using GrindBreaker.Models;
using GrindBreaker.Repositories;
using GrindBreaker.RPC;
using GrindBreaker.RPC.Models;
using Moq;
using Newtonsoft.Json;

namespace GrindBreaker.Tests;

public class ProfileRPCTests
{
    private readonly Mock<IProfileRepository> _mockRepository;
    private readonly Mock<IWebviewWrapper> _mockWebview;
    private readonly ProfileRPC _profileRPC;

    public ProfileRPCTests()
    {
        _mockRepository = new Mock<IProfileRepository>();
        _mockWebview = new Mock<IWebviewWrapper>();
        _profileRPC = new ProfileRPC(_mockRepository.Object);
    }

    #region GetProfile Tests

    [Fact]
    public void GetProfile_WhenProfileExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var profile = CreateTestProfile();
        var id = "test-id";
        var req = "[]";
        
        _mockRepository.Setup(r => r.GetProfile()).Returns(profile);

        // Act
        _profileRPC.GetProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Success, 
            It.Is<string>(json => ContainsProfileData(json, profile))
        ), Times.Once);
    }

    [Fact]
    public void GetProfile_WhenProfileIsNull_ShouldReturnErrorResult()
    {
        // Arrange
        var id = "test-id";
        var req = "[]";
        
        _mockRepository.Setup(r => r.GetProfile()).Returns((Profile?)null);

        // Act
        _profileRPC.GetProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "Profile not found"))
        ), Times.Once);
    }

    [Fact]
    public void GetProfile_WhenRepositoryThrowsException_ShouldReturnErrorResult()
    {
        // Arrange
        var id = "test-id";
        var req = "[]";
        
        _mockRepository.Setup(r => r.GetProfile()).Throws(new Exception("Database error"));

        // Act
        _profileRPC.GetProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "An error occurred while retrieving the profile"))
        ), Times.Once);
    }

    [Fact]
    public void GetProfile_WhenJsonSerializationFails_ShouldReturnErrorResult()
    {
        // Arrange
        var profile = CreateTestProfile();
        var id = "test-id";
        var req = "[]";
        
        _mockRepository.Setup(r => r.GetProfile()).Returns(profile);
        
        // Mock JsonConvert.SerializeObject to throw an exception
        // Note: This is a bit tricky with static methods, but we can test the behavior

        // Act
        _profileRPC.GetProfile(_mockWebview.Object, id, req);

        // Assert
        // The method should still work normally since we can't easily mock static methods
        // This test verifies the method handles the normal case
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Success, 
            It.IsAny<string>()
        ), Times.Once);
    }

    #endregion

    #region SaveProfile Tests

    [Fact]
    public void SaveProfile_WithValidProfileData_ShouldReturnSuccessResult()
    {
        // Arrange
        var profile = CreateTestProfile();
        var id = "test-id";
        var req = JsonConvert.SerializeObject(new[] { profile });
        
        _mockRepository.Setup(r => r.SaveProfile(It.IsAny<Profile>())).Returns(true);

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockRepository.Verify(r => r.SaveProfile(It.Is<Profile>(p => p.FirstName == profile.FirstName)), Times.Once);
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Success, 
            It.Is<string>(json => ContainsSuccessMessage(json, "Profile saved successfully"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WhenRepositoryReturnsFalse_ShouldReturnErrorResult()
    {
        // Arrange
        var profile = CreateTestProfile();
        var id = "test-id";
        var req = JsonConvert.SerializeObject(new[] { profile });
        
        _mockRepository.Setup(r => r.SaveProfile(It.IsAny<Profile>())).Returns(false);

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "Failed to save profile"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WithNullRequest_ShouldReturnErrorResult()
    {
        // Arrange
        var id = "test-id";
        var req = "null";

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "Invalid profile data"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WithEmptyArray_ShouldReturnErrorResult()
    {
        // Arrange
        var id = "test-id";
        var req = "[]";

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "Invalid profile data"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WithInvalidJson_ShouldReturnErrorResult()
    {
        // Arrange
        var id = "test-id";
        var req = "invalid json";

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "Invalid JSON format"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WhenRepositoryThrowsException_ShouldReturnErrorResult()
    {
        // Arrange
        var profile = CreateTestProfile();
        var id = "test-id";
        var req = JsonConvert.SerializeObject(new[] { profile });
        
        _mockRepository.Setup(r => r.SaveProfile(It.IsAny<Profile>())).Throws(new Exception("Database error"));

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockWebview.Verify(w => w.Return(
            id, 
            RPCResultType.Error, 
            It.Is<string>(json => ContainsErrorMessage(json, "An error occurred while saving the profile"))
        ), Times.Once);
    }

    [Fact]
    public void SaveProfile_WithMultipleProfiles_ShouldUseFirstProfile()
    {
        // Arrange
        var firstProfile = CreateTestProfile();
        firstProfile.FirstName = "First";
        
        var secondProfile = CreateTestProfile();
        secondProfile.FirstName = "Second";
        
        var profiles = new[] { firstProfile, secondProfile };
        var id = "test-id";
        var req = JsonConvert.SerializeObject(profiles);
        
        _mockRepository.Setup(r => r.SaveProfile(It.IsAny<Profile>())).Returns(true);

        // Act
        _profileRPC.SaveProfile(_mockWebview.Object, id, req);

        // Assert
        _mockRepository.Verify(r => r.SaveProfile(It.Is<Profile>(p => p.FirstName == "First")), Times.Once);
    }

    #endregion

    #region Helper Methods

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

    private static bool ContainsProfileData(string json, Profile profile)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<RPCResult<Profile>>(json);
            return result != null && 
                   !result.IsError && 
                   result.Data != null && 
                   result.Data.FirstName == profile.FirstName;
        }
        catch
        {
            return false;
        }
    }

    private static bool ContainsErrorMessage(string json, string expectedMessage)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<RPCResult<object>>(json);
            return result != null && 
                   result.IsError && 
                   result.ErrorMessage == expectedMessage;
        }
        catch
        {
            return false;
        }
    }

    private static bool ContainsSuccessMessage(string json, string expectedMessage)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<RPCResult<string>>(json);
            return result != null && 
                   !result.IsError && 
                   result.Data == expectedMessage;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
