using GrindBreaker.Models;
using GrindBreaker.Repositories;
using Newtonsoft.Json;

namespace GrindBreaker.Tests;

public class CandidacyRepositoryTests : IDisposable
{
    private readonly string _testDataPath;
    private readonly CandidacyRepository _repository;

    public CandidacyRepositoryTests()
    {
        _testDataPath = Path.Combine(Path.GetTempPath(), "GrindBreakerCandidacyTests", Guid.NewGuid().ToString());
        _repository = new CandidacyRepository(_testDataPath);
    }

    [Fact]
    public void Constructor_WithNullDataPath_ShouldUseDefaultPath()
    {
        // Arrange & Act
        var repository = new CandidacyRepository(null);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Constructor_WithCustomDataPath_ShouldCreateDirectory()
    {
        // Arrange
        var customPath = Path.Combine(Path.GetTempPath(), "CustomGrindBreakerCandidacy", Guid.NewGuid().ToString());

        // Act
        var repository = new CandidacyRepository(customPath);

        // Assert
        Assert.True(Directory.Exists(customPath));
        Directory.Delete(customPath, true);
    }

    [Fact]
    public void GetAllCandidacies_WhenFileDoesNotExist_ShouldReturnEmptyList()
    {
        // Act
        var result = _repository.GetAllCandidacies();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllCandidacies_WhenFileExists_ShouldReturnCandidacies()
    {
        // Arrange
        var candidacies = new List<Candidacy> { CreateTestCandidacy() };
        var filePath = Path.Combine(_testDataPath, "candidacies.json");
        var json = JsonConvert.SerializeObject(candidacies, Formatting.Indented);
        File.WriteAllText(filePath, json);

        // Act
        var result = _repository.GetAllCandidacies();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(candidacies[0].Id, result[0].Id);
        Assert.Equal(candidacies[0].Company, result[0].Company);
    }

    [Fact]
    public void GetAllCandidacies_WhenFileContainsInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var filePath = Path.Combine(_testDataPath, "candidacies.json");
        File.WriteAllText(filePath, "invalid json content");

        // Act
        var result = _repository.GetAllCandidacies();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetCandidacy_WithValidId_ShouldReturnCandidacy()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        _repository.SaveCandidacy(candidacy);

        // Act
        var result = _repository.GetCandidacy(candidacy.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(candidacy.Id, result.Id);
        Assert.Equal(candidacy.Company, result.Company);
        Assert.Equal(candidacy.Title, result.Title);
    }

    [Fact]
    public void GetCandidacy_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = _repository.GetCandidacy("invalid-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCandidacy_WithNullId_ShouldReturnNull()
    {
        // Act
        var result = _repository.GetCandidacy(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCandidacy_WithEmptyId_ShouldReturnNull()
    {
        // Act
        var result = _repository.GetCandidacy("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SaveCandidacy_WithValidCandidacy_ShouldReturnTrue()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();

        // Act
        var result = _repository.SaveCandidacy(candidacy);

        // Assert
        Assert.True(result);
        Assert.True(File.Exists(Path.Combine(_testDataPath, "candidacies.json")));
    }

    [Fact]
    public void SaveCandidacy_WithValidCandidacy_ShouldSaveCorrectData()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();

        // Act
        _repository.SaveCandidacy(candidacy);

        // Assert
        var filePath = Path.Combine(_testDataPath, "candidacies.json");
        var json = File.ReadAllText(filePath);
        var savedCandidacies = JsonConvert.DeserializeObject<List<Candidacy>>(json);
        
        Assert.NotNull(savedCandidacies);
        Assert.Single(savedCandidacies);
        Assert.Equal(candidacy.Id, savedCandidacies[0].Id);
        Assert.Equal(candidacy.Company, savedCandidacies[0].Company);
        Assert.Equal(candidacy.Title, savedCandidacies[0].Title);
    }

    [Fact]
    public void SaveCandidacy_WithNullCandidacy_ShouldReturnFalse()
    {
        // Act
        var result = _repository.SaveCandidacy(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SaveCandidacy_MultipleCandidacies_ShouldSaveAll()
    {
        // Arrange
        var candidacy1 = CreateTestCandidacy();
        var candidacy2 = CreateTestCandidacy();
        candidacy2.Company = "Different Company";

        // Act
        _repository.SaveCandidacy(candidacy1);
        _repository.SaveCandidacy(candidacy2);
        var allCandidacies = _repository.GetAllCandidacies();

        // Assert
        Assert.Equal(2, allCandidacies.Count);
    }

    [Fact]
    public void UpdateCandidacy_WithValidCandidacy_ShouldReturnTrue()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        _repository.SaveCandidacy(candidacy);
        candidacy.Company = "Updated Company";

        // Act
        var result = _repository.UpdateCandidacy(candidacy);

        // Assert
        Assert.True(result);
        var updatedCandidacy = _repository.GetCandidacy(candidacy.Id);
        Assert.Equal("Updated Company", updatedCandidacy!.Company);
    }

    [Fact]
    public void UpdateCandidacy_WithNonExistentCandidacy_ShouldReturnFalse()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();

        // Act
        var result = _repository.UpdateCandidacy(candidacy);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateCandidacy_WithNullCandidacy_ShouldReturnFalse()
    {
        // Act
        var result = _repository.UpdateCandidacy(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateCandidacy_WithEmptyId_ShouldReturnFalse()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        candidacy.Id = "";

        // Act
        var result = _repository.UpdateCandidacy(candidacy);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteCandidacy_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        _repository.SaveCandidacy(candidacy);

        // Act
        var result = _repository.DeleteCandidacy(candidacy.Id);

        // Assert
        Assert.True(result);
        Assert.Null(_repository.GetCandidacy(candidacy.Id));
    }

    [Fact]
    public void DeleteCandidacy_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.DeleteCandidacy("non-existent-id");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteCandidacy_WithNullId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.DeleteCandidacy(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteCandidacy_WithEmptyId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.DeleteCandidacy("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        _repository.SaveCandidacy(candidacy);

        // Act
        var result = _repository.UpdateCandidacyStatus(candidacy.Id, CandidacyStatus.Applied);

        // Assert
        Assert.True(result);
        var updatedCandidacy = _repository.GetCandidacy(candidacy.Id);
        Assert.Equal(CandidacyStatus.Applied, updatedCandidacy!.Status);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.UpdateCandidacyStatus("non-existent-id", CandidacyStatus.Applied);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithNullId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.UpdateCandidacyStatus(null!, CandidacyStatus.Applied);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithEmptyId_ShouldReturnFalse()
    {
        // Act
        var result = _repository.UpdateCandidacyStatus("", CandidacyStatus.Applied);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SaveCandidacy_ThenGetAllCandidacies_ShouldReturnSameData()
    {
        // Arrange
        var originalCandidacy = CreateTestCandidacy();

        // Act
        _repository.SaveCandidacy(originalCandidacy);
        var retrievedCandidacies = _repository.GetAllCandidacies();

        // Assert
        Assert.NotNull(retrievedCandidacies);
        Assert.Single(retrievedCandidacies);
        var retrievedCandidacy = retrievedCandidacies[0];
        Assert.Equal(originalCandidacy.Id, retrievedCandidacy.Id);
        Assert.Equal(originalCandidacy.Company, retrievedCandidacy.Company);
        Assert.Equal(originalCandidacy.Title, retrievedCandidacy.Title);
        Assert.Equal(originalCandidacy.Status, retrievedCandidacy.Status);
    }

    [Fact]
    public void SaveCandidacy_WithApplicationSteps_ShouldPreserveSteps()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        candidacy.ApplicationSteps = new List<CandidacyStep>
        {
            new CandidacyStep
            {
                Type = "Phone Interview",
                Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Notes = "Initial phone screening"
            },
            new CandidacyStep
            {
                Type = "Technical Interview",
                Date = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
                Notes = "Coding challenge"
            }
        };

        // Act
        _repository.SaveCandidacy(candidacy);
        var retrievedCandidacy = _repository.GetCandidacy(candidacy.Id);

        // Assert
        Assert.NotNull(retrievedCandidacy);
        Assert.Equal(2, retrievedCandidacy.ApplicationSteps.Count);
        Assert.Equal("Phone Interview", retrievedCandidacy.ApplicationSteps[0].Type);
        Assert.Equal("Technical Interview", retrievedCandidacy.ApplicationSteps[1].Type);
    }

    private static Candidacy CreateTestCandidacy()
    {
        return new Candidacy
        {
            Id = Guid.NewGuid().ToString(),
            Company = "Test Company",
            Title = "Software Developer",
            JobLink = "https://example.com/job",
            JobDescription = "A great job opportunity",
            DateApplied = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Status = CandidacyStatus.ToApply,
            ApplicationSteps = new List<CandidacyStep>()
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
