using GrindBreaker.Models;
using GrindBreaker.Repositories;
using GrindBreaker.RPC;
using GrindBreaker.RPC.Models;
using Moq;
using Newtonsoft.Json;

namespace GrindBreaker.Tests;

public class CandidacyRPCTests
{
    private readonly Mock<ICandidacyRepository> _mockRepository;
    private readonly Mock<IWebviewWrapper> _mockWebview;
    private readonly CandidacyRPC _candidacyRPC;

    public CandidacyRPCTests()
    {
        _mockRepository = new Mock<ICandidacyRepository>();
        _mockWebview = new Mock<IWebviewWrapper>();
        _candidacyRPC = new CandidacyRPC(_mockRepository.Object);
    }

    [Fact]
    public void GetAllCandidacies_ShouldReturnSuccessWithCandidacies()
    {
        // Arrange
        var candidacies = new List<Candidacy> { CreateTestCandidacy() };
        _mockRepository.Setup(r => r.GetAllCandidacies()).Returns(candidacies);

        // Act
        _candidacyRPC.GetAllCandidacies(_mockWebview.Object, "test-id", "[]");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetAllCandidacies_WhenRepositoryThrows_ShouldReturnError()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllCandidacies()).Throws(new Exception("Repository error"));

        // Act
        _candidacyRPC.GetAllCandidacies(_mockWebview.Object, "test-id", "[]");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetCandidacy_WithValidId_ShouldReturnSuccessWithCandidacy()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy.Id });
        _mockRepository.Setup(r => r.GetCandidacy(candidacy.Id)).Returns(candidacy);

        // Act
        _candidacyRPC.GetCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetCandidacy_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var requestJson = JsonConvert.SerializeObject(new[] { "non-existent-id" });
        _mockRepository.Setup(r => r.GetCandidacy("non-existent-id")).Returns((Candidacy?)null);

        // Act
        _candidacyRPC.GetCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetCandidacy_WithInvalidRequest_ShouldReturnError()
    {
        // Act
        _candidacyRPC.GetCandidacy(_mockWebview.Object, "test-id", "invalid-json");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetCandidacy_WithEmptyId_ShouldReturnError()
    {
        // Arrange
        var requestJson = JsonConvert.SerializeObject(new[] { "" });

        // Act
        _candidacyRPC.GetCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });
        _mockRepository.Setup(r => r.SaveCandidacy(It.IsAny<Candidacy>())).Returns(true);

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(r => r.SaveCandidacy(It.IsAny<Candidacy>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithMissingRequiredFields_ShouldReturnError()
    {
        // Arrange
        var candidacy = new Candidacy { Company = "", Title = "" };
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithNullCompany_ShouldReturnError()
    {
        // Arrange
        var candidacy = new Candidacy { Company = null!, Title = "Test Title" };
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithNullTitle_ShouldReturnError()
    {
        // Arrange
        var candidacy = new Candidacy { Company = "Test Company", Title = null! };
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WhenRepositoryFails_ShouldReturnError()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });
        _mockRepository.Setup(r => r.SaveCandidacy(It.IsAny<Candidacy>())).Returns(false);

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithInvalidJson_ShouldReturnError()
    {
        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", "invalid-json");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SaveCandidacy_WithEmptyRequest_ShouldReturnError()
    {
        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", "[]");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacy_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });
        _mockRepository.Setup(r => r.UpdateCandidacy(It.IsAny<Candidacy>())).Returns(true);

        // Act
        _candidacyRPC.UpdateCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateCandidacy(It.IsAny<Candidacy>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacy_WithMissingRequiredFields_ShouldReturnError()
    {
        // Arrange
        var candidacy = new Candidacy { Company = "", Title = "" };
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });

        // Act
        _candidacyRPC.UpdateCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacy_WhenRepositoryFails_ShouldReturnError()
    {
        // Arrange
        var candidacy = CreateTestCandidacy();
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });
        _mockRepository.Setup(r => r.UpdateCandidacy(It.IsAny<Candidacy>())).Returns(false);

        // Act
        _candidacyRPC.UpdateCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void DeleteCandidacy_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var candidacyId = "test-id";
        var requestJson = JsonConvert.SerializeObject(new[] { candidacyId });
        _mockRepository.Setup(r => r.DeleteCandidacy(candidacyId)).Returns(true);

        // Act
        _candidacyRPC.DeleteCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(r => r.DeleteCandidacy(candidacyId), Times.Once);
    }

    [Fact]
    public void DeleteCandidacy_WithNonExistentId_ShouldReturnError()
    {
        // Arrange
        var candidacyId = "non-existent-id";
        var requestJson = JsonConvert.SerializeObject(new[] { candidacyId });
        _mockRepository.Setup(r => r.DeleteCandidacy(candidacyId)).Returns(false);

        // Act
        _candidacyRPC.DeleteCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void DeleteCandidacy_WithEmptyId_ShouldReturnError()
    {
        // Arrange
        var requestJson = JsonConvert.SerializeObject(new[] { "" });

        // Act
        _candidacyRPC.DeleteCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void DeleteCandidacy_WithInvalidJson_ShouldReturnError()
    {
        // Act
        _candidacyRPC.DeleteCandidacy(_mockWebview.Object, "test-id", "invalid-json");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var candidacyId = "test-id";
        var newStatus = CandidacyStatus.Applied;
        var requestJson = JsonConvert.SerializeObject(new object[] { candidacyId, newStatus.ToString() });
        _mockRepository.Setup(r => r.UpdateCandidacyStatus(candidacyId, newStatus)).Returns(true);

        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateCandidacyStatus(candidacyId, newStatus), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithInvalidStatus_ShouldReturnError()
    {
        // Arrange
        var candidacyId = "test-id";
        var invalidStatus = "InvalidStatus";
        var requestJson = JsonConvert.SerializeObject(new object[] { candidacyId, invalidStatus });

        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithEmptyId_ShouldReturnError()
    {
        // Arrange
        var requestJson = JsonConvert.SerializeObject(new object[] { "", CandidacyStatus.Applied.ToString() });

        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithInsufficientData_ShouldReturnError()
    {
        // Arrange
        var requestJson = JsonConvert.SerializeObject(new object[] { "test-id" });

        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WhenRepositoryFails_ShouldReturnError()
    {
        // Arrange
        var candidacyId = "test-id";
        var newStatus = CandidacyStatus.Applied;
        var requestJson = JsonConvert.SerializeObject(new object[] { candidacyId, newStatus.ToString() });
        _mockRepository.Setup(r => r.UpdateCandidacyStatus(candidacyId, newStatus)).Returns(false);

        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void UpdateCandidacyStatus_WithInvalidJson_ShouldReturnError()
    {
        // Act
        _candidacyRPC.UpdateCandidacyStatus(_mockWebview.Object, "test-id", "invalid-json");

        // Assert
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Error, It.IsAny<string>()), Times.Once);
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
                Notes = "Initial screening"
            }
        };
        var requestJson = JsonConvert.SerializeObject(new[] { candidacy });
        _mockRepository.Setup(r => r.SaveCandidacy(It.IsAny<Candidacy>())).Returns(true);

        // Act
        _candidacyRPC.SaveCandidacy(_mockWebview.Object, "test-id", requestJson);

        // Assert
        _mockRepository.Verify(r => r.SaveCandidacy(It.Is<Candidacy>(c => c.ApplicationSteps.Count == 1)), Times.Once);
        _mockWebview.Verify(w => w.Return("test-id", RPCResultType.Success, It.IsAny<string>()), Times.Once);
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
}
