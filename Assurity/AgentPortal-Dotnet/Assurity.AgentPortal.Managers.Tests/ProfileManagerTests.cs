namespace Assurity.AgentPortal.Managers.Tests;

using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;
using Moq;

public class ProfileManagerTests
{
    private readonly Mock<IProfileAccessor> _mockProfileAccessor;
    private readonly Mock<ILogger<ProfileManager>> _mockLogger;

    public ProfileManagerTests()
    {
        _mockProfileAccessor = new Mock<IProfileAccessor>();
        _mockLogger = new Mock<ILogger<ProfileManager>>();
    }

    [Fact]
    public async Task ChangePassword_Success()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Status = "OK",
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(string.Empty, response.Message);
        Assert.True(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_PasswordTooYoung()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "REQUEST_FAILED",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Code = "PASSWORD_TOO_YOUNG",
                    Message = "New password did not satisfy password policy requirements"
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The password cannot be changed because it has not been long enough since the last password change.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_CurrentPasswordWrong()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "The current password provided for the user is invalid"
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The provided old password is incorrect. Please try again.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_NewPasswordLength()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        Length = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockChangePasswordResponse.Details.First().InnerError.Length, response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_MinCharacters()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        MinCharacters = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockChangePasswordResponse.Details.First().InnerError.MinCharacters, response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_MinComplexity()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        MinComplexity = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The proposed password is not acceptable because it is too simple.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_ExcludesCommonlyUsed()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        ExcludesCommonlyUsed = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockChangePasswordResponse.Details.First().InnerError.ExcludesCommonlyUsed, response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_MaxRepeatedCharacters()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        MaxRepeatedCharacters = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockChangePasswordResponse.Details.First().InnerError.MaxRepeatedCharacters, response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_NotSimilarToCurrent()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        NotSimilarToCurrent = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The new password is too similar to the current password.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_History()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError
                    {
                        History = "error message!"
                    }
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The new password cannot be a previously used password.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_Default()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = "INVALID_DATA",
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = "New password did not satisfy password policy requirements",
                    InnerError = new PingOneResponseDetailsInnerError()
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("The new password does not meet the password policy.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Failure_ErrorNotFound()
    {
        // Arrange
        var fakeAccessToken = "fakeAccessToken";
        var userId = Guid.NewGuid().ToString();
        var currentPassword = "A123";
        var newPassword = "password2";

        var mockChangePasswordResponse = new PingOneResponse
        {
            Code = string.Empty,
            Details = new List<PingOneResponseDetails>
            {
                new PingOneResponseDetails
                {
                    Message = string.Empty,
                    InnerError = new PingOneResponseDetailsInnerError()
                }
            }
        };

        _mockProfileAccessor.Setup(accessor => accessor.ChangePassword(
            It.Is<string>(uid => uid == userId),
            It.Is<string>(cpw => cpw == currentPassword),
            It.Is<string>(npw => npw == newPassword),
            It.Is<string>(token => token == fakeAccessToken),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(mockChangePasswordResponse)
        .Verifiable();

        var userDataManager = GetProfileManager();

        // Act
        var response = await userDataManager.ChangePassword(userId, currentPassword, newPassword, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("An unexpected error has occurred. Please try again.", response.Message);
        Assert.False(response.Success);

        _mockProfileAccessor.Verify(
            accessor => accessor.ChangePassword(
                userId,
                currentPassword,
                newPassword,
                fakeAccessToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private ProfileManager GetProfileManager()
    {
        return new ProfileManager(_mockLogger.Object, _mockProfileAccessor.Object);
    }
}
