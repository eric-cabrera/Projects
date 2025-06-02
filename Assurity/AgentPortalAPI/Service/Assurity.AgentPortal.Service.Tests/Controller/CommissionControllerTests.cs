namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Managers.CommissionsAndDebt;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class CommissionControllerTests
{
    [Fact]
    public async Task GetCommissionsData_Success_ShouldReturn200()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new CommissionResponse();
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetCommissionAndSummaryData(
            It.IsAny<string>(),
            It.IsAny<PolicyDetailsParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetPolicyDetails(new PolicyDetailsParameters());
        var result = actionResult as ObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetWritingAgentDetails_Success_ShouldReturn200()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new WritingAgentDetailsResponse();
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetCommissionDataByWritingAgent(
            It.IsAny<string>(),
            It.IsAny<WritingAgentParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetWritingAgents(new WritingAgentParameters());
        var result = actionResult as ObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetWritingAgentDetails_InvalidAgentId_ShouldReturn401()
    {
        // Arrange
        var agentId = string.Empty;

        var mockResponse = new WritingAgentDetailsResponse();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetCommissionDataByWritingAgent(
            It.IsAny<string>(),
            It.IsAny<WritingAgentParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockIlogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockIlogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetWritingAgents(new WritingAgentParameters());
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetUnsecuredAdvances_Success_ShouldReturn200()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new DebtResponse();
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetUnsecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<UnsecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetUnsecuredAdvances(new UnsecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as ObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetUnsecuredAdvances_ManagerThrows_ShouldReturn500()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new DebtResponse();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetUnsecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<UnsecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetUnsecuredAdvances(new UnsecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetUnsecuredAdvances_InvalidAgentId_ShouldReturn401()
    {
        // Arrange
        var agentId = string.Empty;

        var mockResponse = new DebtResponse();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetUnsecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<UnsecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetUnsecuredAdvances(new UnsecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetSecuredAdvances_ManagerThrows_ShouldReturn500()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new DebtResponse();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetSecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<SecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetSecuredAdvances(new SecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetSecuredAdvances_InvalidAgentId_ShouldReturn401()
    {
        // Arrange
        var agentId = string.Empty;

        var mockResponse = new DebtResponse();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetSecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<SecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetSecuredAdvances(new SecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetSecuredAdvances_Success_ShouldReturn200()
    {
        // Arrange
        var agentId = "AAXB";

        var mockResponse = new DebtResponse();
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetSecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<SecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetSecuredAdvances(new SecuredAdvanceParameters(), CancellationToken.None);
        var result = actionResult as ObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ExportPolicyDetails_Success_ShouldReturnFile()
    {
        // Arrange
        var mockResponse = new FileResponse("Howdy", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileData = Array.Empty<byte>()
        };
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetPolicyDetailsExcel(
            It.IsAny<string>(),
            It.IsAny<PolicyDetailsParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportPolicyDetails(new PolicyDetailsParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileContentResult>(response);
    }

    [Fact]
    public async Task ExportPolicyDetails_RequestCancelled_ShouldReturn409()
    {
        // Arrange
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetPolicyDetailsExcel(
            It.IsAny<string>(),
            It.IsAny<PolicyDetailsParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportPolicyDetails(new PolicyDetailsParameters(), CancellationToken.None);
        var result = response as StatusCodeResult;

        // Assert
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task ExportWritingAgentDetails_Success_ShouldReturnFile()
    {
        // Arrange
        var mockResponse = new FileResponse("Howdy", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileData = Array.Empty<byte>()
        };
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetWritingAgentDetailsExcel(
            It.IsAny<string>(),
            It.IsAny<WritingAgentParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportWritingAgentDetails(new WritingAgentParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileContentResult>(response);
    }

    [Fact]
    public async Task ExportWritingAgentDetails_RequestCancelled_ShouldReturn409()
    {
        // Arrange
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetWritingAgentDetailsExcel(
            It.IsAny<string>(),
            It.IsAny<WritingAgentParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportWritingAgentDetails(new WritingAgentParameters(), CancellationToken.None);
        var result = response as StatusCodeResult;

        // Assert
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task ExportUnsecuredAdvances_Success_ShouldReturnFile()
    {
        // Arrange
        var mockResponse = new FileResponse("Howdy", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileData = Array.Empty<byte>()
        };
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetUnsecuredAdvancesExcel(
            It.IsAny<string>(),
            It.IsAny<UnsecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportUnsecuredAdvances(new UnsecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileContentResult>(response);
    }

    [Fact]
    public async Task ExportUnsecuredAdvances_RequestCancelled_ShouldReturn409()
    {
        // Arrange
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetUnsecuredAdvancesExcel(
            It.IsAny<string>(),
            It.IsAny<UnsecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportUnsecuredAdvances(new UnsecuredAdvanceParameters(), CancellationToken.None);
        var result = response as StatusCodeResult;

        // Assert
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task ExportSecuredAdvances_Success_ShouldReturnFile()
    {
        // Arrange
        var mockResponse = new FileResponse("Howdy", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileData = Array.Empty<byte>()
        };
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetSecuredAdvancesExcel(
            It.IsAny<string>(),
            It.IsAny<SecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportSecuredAdvances(new SecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileContentResult>(response);
    }

    [Fact]
    public async Task ExportSecuredAdvances_RequestCancelled_ShouldReturn409()
    {
        // Arrange
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetSecuredAdvancesExcel(
            It.IsAny<string>(),
            It.IsAny<SecuredAdvanceParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.ExportSecuredAdvances(new SecuredAdvanceParameters(), CancellationToken.None);
        var result = response as StatusCodeResult;

        // Assert
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task GetAgentStatement_Success_ShouldReturnFile()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetAgentStatement(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<AgentStatementType>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileStream);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        var request = new AgentStatementRequest
        {
            AgentId = "aaxb",
            CycleDate = DateTime.Now,
            AgentStatementType = AgentStatementType.Summary,
        };

        // Act
        var response = await controller.GetAgentStatement(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<FileStreamResult>(response);
    }

    [Fact]
    public async Task GetAgentStatement_NullFileStream_ShouldReturn500()
    {
        // Arrange
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetAgentStatement(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<AgentStatementType>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Stream>(null));

        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        var request = new AgentStatementRequest
        {
            AgentId = "aaxb",
            CycleDate = DateTime.Now,
            AgentStatementType = AgentStatementType.Summary,
        };

        // Act
        var response = await controller.GetAgentStatement(request, CancellationToken.None);

        var result = response as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetAgentStatement_NullAgentIdParameter_ShouldReturn400()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetAgentStatement(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<AgentStatementType>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Stream>(null));

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        var request = new AgentStatementRequest
        {
            CycleDate = DateTime.Now,
            AgentStatementType = AgentStatementType.Summary,
        };

        // Act
        var response = await controller.GetAgentStatement(request, CancellationToken.None);

        var result = response as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(400, details.Status);
    }

    [Fact]
    public async Task GetAgentStatementCycleDates_Success()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CommissionsAndDebtController>>();
        var mockManager = new Mock<ICommissionAndDebtManager>();
        mockManager.Setup(manager => manager.GetAgentStatementOptions(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AgentStatementOptions());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext("aaxb");

        var controller = new CommissionsAndDebtController(mockLogger.Object, mockManager.Object, mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.GetAgentStatementOptions(CancellationToken.None);
        var details = response as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, details.StatusCode);
    }

    private static HttpContext GetMockHttpContext(string agentId)
    {
        var claims = new List<Claim>
        {
            new("agentId", agentId, string.Empty, "Ping"),
            new("sid", Guid.NewGuid().ToString(), string.Empty, "Ping"),
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var context = new DefaultHttpContext
        {
            User = user,
            Items = new Dictionary<object, object>
            {
                { "AgentId", agentId },
                { "IsSubaccount", false }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

        return context;
    }
}
