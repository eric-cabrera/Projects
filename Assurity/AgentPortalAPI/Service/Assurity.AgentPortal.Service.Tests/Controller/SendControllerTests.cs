namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.Send;
using Assurity.AgentPortal.Contracts.Send.Enums;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Assurity.AgentPortal.Managers.Send;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Service.Models;
using Assurity.AgentPortal.Service.Validation;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class SendControllerTests
{
    [Fact]
    public async Task TakeAction_MissingAgentClaim_ShouldReturn401Unauthorized()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<SendController>>();
        mockLogger.Setup(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);

        var mockHttpContext = GetMockHttpContext(string.Empty);

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        // Act
        var result = await sendController.TakeAction(null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedResult>(result);

        mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"Invalid Agent Id retrieved from access token."),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TakeAction_NotMultipartContent_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<SendController>>();
        mockLogger.Setup(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(false);

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);

        var mockHttpContext = GetMockHttpContext("1B14", "application/json");

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        // Act
        var result = await sendController.TakeAction(null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.Equal(400, badRequestObjectResult.StatusCode);
        Assert.NotNull(badRequestObjectResult.Value);
        Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        var problemDetails = badRequestObjectResult.Value as ProblemDetails;

        Assert.Equal(400, problemDetails.Status);
        Assert.Equal("BadRequest", problemDetails.Title);
        Assert.Equal("Invalid content type found in the request.", problemDetails.Detail);

        mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"Invalid content type found in the request for agent 1B14."),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TakeAction_InitialModelStateOnRequestIsInvalid_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        var mockLogger = new Mock<ILogger<SendController>>();
        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);

        var mockHttpContext = GetMockHttpContext("1B14");

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        // Arrange the ModelState so that it contains an error and becomes invalid.
        sendController.ModelState.Clear();
        sendController.ModelState.AddModelError("PolicyNumber", "PolicyNumber is required.");

        // Act
        var result = await sendController.TakeAction(null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.Equal(400, badRequestObjectResult.StatusCode);
        Assert.NotNull(badRequestObjectResult.Value);
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = badRequestObjectResult.Value as ValidationProblemDetails;

        Assert.Equal("One or more validation errors occurred.", validationProblemDetails.Title);
        Assert.Equal(1, validationProblemDetails.Errors?.Count ?? 0);

        validationProblemDetails.Errors
            .TryGetValue("PolicyNumber", out var policyNumberErrorMessage);

        Assert.Equal("PolicyNumber is required.", policyNumberErrorMessage[0]);
    }

    [Fact]
    public async Task TakeAction_CheckAgentAccessToPolicy_ThrowsException_ShouldReturn500InternalServerError()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<SendController>>();
        mockLogger.Setup(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        var expectedExceptionMessage = "Manager done goofed.";

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);

        var agentId = "1B14";
        var mockHttpContext = GetMockHttpContext(agentId);

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            UploadData = new UploadData
            {
                Message = "Agent message",
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ObjectResult>(result);

        var objectResult = result as ObjectResult;
        var problemDetails = objectResult.Value as ProblemDetails;

        Assert.Equal(500, problemDetails.Status);
        Assert.Equal("InternalServerError", problemDetails.Title);
    }

    [Fact]
    public async Task TakeAction_CheckAgentAccessToPolicy_False_ShouldReturn403Forbidden()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<SendController>>();
        mockLogger.Setup(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(false);

        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);

        var agentId = "1B14";
        var mockHttpContext = GetMockHttpContext(agentId);

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            UploadData = new UploadData
            {
                Message = "Agent message",
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidResult>(result);

        var expectedMessage = $"Agent {agentId} does not have access to policy {uploadRequest.UploadData.PolicyNumber}.";

        mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == expectedMessage),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TakeAction_MissingBothMessageAndFiles_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SendController>>();
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext("1B14");

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            UploadData = new UploadData
            {
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.Equal(400, badRequestObjectResult.StatusCode);
        Assert.NotNull(badRequestObjectResult.Value);
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = badRequestObjectResult.Value as ValidationProblemDetails;

        Assert.Equal("One or more validation errors occurred.", validationProblemDetails.Title);
        Assert.Equal(1, validationProblemDetails.Errors?.Count ?? 0);

        validationProblemDetails.Errors
            .TryGetValue("TakeAction", out var policyNumberErrorMessage);

        Assert.Equal("Either a message or at least one file is required.", policyNumberErrorMessage[0]);
    }

    [Fact]
    public async Task TakeAction_NumberOfFilesExceedsMaximum_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.TakeActionMaximumFilesPerUpload)
            .Returns(1);

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SendController>>();
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext("1B14");

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            FilesToUpload = new List<IFormFile>
            {
                // Two files are represented here, but it doesn't matter what they are
                // for the sake of this test, just that there are two (greater than the maximum
                // number of files mocked up for the ConfigurationManager).
                null,
                null
            },
            UploadData = new UploadData
            {
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.Equal(400, badRequestObjectResult.StatusCode);
        Assert.NotNull(badRequestObjectResult.Value);
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = badRequestObjectResult.Value as ValidationProblemDetails;

        Assert.Equal("One or more validation errors occurred.", validationProblemDetails.Title);
        Assert.Equal(1, validationProblemDetails.Errors?.Count ?? 0);

        validationProblemDetails.Errors
            .TryGetValue("TakeAction", out var policyNumberErrorMessage);

        Assert.Equal(
            "The number of files requested exceeds the maximum files per upload.",
            policyNumberErrorMessage[0]);
    }

    [Fact]
    public async Task TakeAction_FilesFailedValidation_ShouldReturn400BadRequest()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.TakeActionMaximumFilesPerUpload)
            .Returns(0);

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);

        // Only 2 files are mocked up here (despite 3 being represented in the request) to prove
        // that the controller code breaks early when it fails validation for one of the files.
        mockFileValidator
            .SetupSequence(fileValidator => fileValidator.ProcessFormFile(
                It.IsAny<IFormFile>(),
                It.IsAny<ModelStateDictionary>()))
            .ReturnsAsync(new File
            {
                Bytes = new byte[] { 0x20 },
                FileType = FileType.Pdf,
                Name = "TestFile1"
            })
            .ReturnsAsync(new File
            {
                Bytes = Array.Empty<byte>(),
                FileType = FileType.Jpg,
                Name = "TestFile2"
            });

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SendController>>();
        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext("1B14");

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            FilesToUpload = new List<IFormFile>
            {
                // Three files represented here, but doesn't matter what they are until
                // they're mocked up via the FileValidator.ProcessFormFile method.
                null,
                null,
                null
            },
            UploadData = new UploadData
            {
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);

        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.Equal(400, badRequestObjectResult.StatusCode);
        Assert.NotNull(badRequestObjectResult.Value);
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = badRequestObjectResult.Value as ValidationProblemDetails;

        Assert.Equal("One or more validation errors occurred.", validationProblemDetails.Title);
        Assert.Equal(1, validationProblemDetails.Errors?.Count ?? 0);

        validationProblemDetails.Errors
            .TryGetValue("TakeAction", out var takeActionErrorMessage);

        Assert.Equal("Unable to process files.", takeActionErrorMessage[0]);
    }

    [Fact]
    public async Task TakeAction_SendMessageAndFilesToGlobal_ThrowsException_ShouldReturn500InternalServerError()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.TakeActionMaximumFilesPerUpload)
            .Returns(0);

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);
        mockFileValidator
            .Setup(fileValidator => fileValidator.ProcessFormFile(
                It.IsAny<IFormFile>(),
                It.IsAny<ModelStateDictionary>()))
            .ReturnsAsync(new File
            {
                Bytes = new byte[] { 0x20 },
                FileType = FileType.Jpg,
                Name = "TestFile"
            });

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SendController>>();
        mockLogger.Setup(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()));

        var requestGuid = Guid.NewGuid();

        var expectedExceptionMessage = "Send Manager failed, after all that.";

        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);
        mockSendManager
            .Setup(sendManager => sendManager.SendMessageAndFilesToGlobal(It.IsAny<ActionRequest>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var agentId = "1B14";
        var mockHttpContext = GetMockHttpContext(agentId);

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            FilesToUpload = new List<IFormFile>
            {
                // One file represented here, but doesn't matter what it is until
                // it's mocked up via the FileValidator.ProcessFormFile method.
                null
            },
            UploadData = new UploadData
            {
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ObjectResult>(result);

        var objectResult = result as ObjectResult;
        var problemDetails = objectResult.Value as ProblemDetails;

        Assert.Equal(500, problemDetails.Status);
        Assert.Equal("InternalServerError", problemDetails.Title);
    }

    [Fact]
    public async Task TakeAction_SendMessageAndFilesToGlobal_Success_ShouldReturn200OK()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.AzureAdIssuer)
            .Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager
            .Setup(configurationManager => configurationManager.TakeActionMaximumFilesPerUpload)
            .Returns(0);

        var mockFileValidator = new Mock<IFileValidator>(MockBehavior.Strict);
        mockFileValidator
            .Setup(fileValidator => fileValidator.IsMultipartContentType(It.IsAny<string>()))
            .Returns(true);
        mockFileValidator
            .Setup(fileValidator => fileValidator.ProcessFormFile(
                It.IsAny<IFormFile>(),
                It.IsAny<ModelStateDictionary>()))
            .ReturnsAsync(new File
            {
                Bytes = new byte[] { 0x20 },
                FileType = FileType.Jpg,
                Name = "TestFile"
            });

        var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
        mockPolicyInfoManager
            .Setup(policyInfoManager => policyInfoManager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockHttpContext = GetMockHttpContext("1B14");
        var mockLogger = new Mock<ILogger<SendController>>();

        var mockSendManager = new Mock<ISendManager>(MockBehavior.Strict);
        mockSendManager
            .Setup(sendManager => sendManager.SendMessageAndFilesToGlobal(It.IsAny<ActionRequest>()))
            .Returns(Task.CompletedTask);

        var sendController = GetSendController(
            mockHttpContext,
            mockConfigurationManager,
            mockFileValidator,
            mockLogger,
            mockPolicyInfoManager,
            mockSendManager);

        var uploadRequest = new UploadRequest
        {
            FilesToUpload = new List<IFormFile>
            {
                // One file represented here, but doesn't matter what it is until
                // it's mocked up via the FileValidator.ProcessFormFile method.
                null
            },
            UploadData = new UploadData
            {
                PolicyNumber = "9999999990"
            }
        };

        // Act
        var result = await sendController.TakeAction(uploadRequest);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    private static HttpContext GetMockHttpContext(
        string agentId,
        string contentType = "multipart/form-data")
    {
        var claims = new List<Claim>
        {
            new Claim("agentId", agentId, string.Empty, "Ping"),
        };

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            Items = new Dictionary<object, object>
            {
                { "AgentId", agentId },
                { "IsSubaccount", false }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());
        context.Request.ContentType = contentType;

        return context;
    }

    private static SendController GetSendController(
        HttpContext mockHttpContext,
        Mock<IConfigurationManager> mockConfigurationManager,
        Mock<IFileValidator> mockFileValidator,
        Mock<ILogger<SendController>> mockLogger,
        Mock<IPolicyInfoManager> mockPolicyInfoManager,
        Mock<ISendManager> mockSendManager)
    {
        return new SendController(
            mockConfigurationManager.Object,
            mockFileValidator.Object,
            mockLogger.Object,
            mockPolicyInfoManager.Object,
            mockSendManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            }
        };
    }
}