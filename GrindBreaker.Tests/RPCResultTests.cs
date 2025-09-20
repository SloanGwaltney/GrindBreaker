using GrindBreaker.RPC.Models;

namespace GrindBreaker.Tests;

public class RPCResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Arrange
        var testData = "test data";

        // Act
        var result = RPCResult<string>.Success(testData);

        // Assert
        Assert.False(result.IsError);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(testData, result.Data);
    }

    [Fact]
    public void Success_WithNullData_ShouldCreateSuccessResult()
    {
        // Arrange & Act
        var result = RPCResult<string?>.Success(null);

        // Assert
        Assert.False(result.IsError);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Error_ShouldCreateErrorResult()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var result = RPCResult<string>.Error(errorMessage);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Error_WithNullErrorMessage_ShouldCreateErrorResult()
    {
        // Arrange & Act
        var result = RPCResult<string>.Error(null!);

        // Assert
        Assert.True(result.IsError);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Error_WithEmptyErrorMessage_ShouldCreateErrorResult()
    {
        // Arrange
        var errorMessage = "";

        // Act
        var result = RPCResult<string>.Error(errorMessage);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Success_WithComplexObject_ShouldCreateSuccessResult()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 123 };

        // Act
        var result = RPCResult<object>.Success(testObject);

        // Assert
        Assert.False(result.IsError);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(testObject, result.Data);
    }

    [Fact]
    public void Error_WithComplexObject_ShouldCreateErrorResult()
    {
        // Arrange
        var errorMessage = "Complex error message";

        // Act
        var result = RPCResult<object>.Error(errorMessage);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}
