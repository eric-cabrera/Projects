namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.AgentPortal.Managers.CaseManagement;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class CaseManagementControllerTests
{
    [Fact]
    public async Task GetCases_Success()
    {
        // Arrange
        var parameters = new CaseManagementParameters();
        var response = new CaseManagementResponse();
        var agentId = "123A";
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<CaseManagementController>>();
        var mockManager = new Mock<ICaseManagementManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetCases(agentId, parameters, cancellationToken))
            .ReturnsAsync(response);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CaseManagementController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = mockHttpContext }
        };

        // Act
        var actionResult = await controller.GetCases(parameters, cancellationToken);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task GetCases_NoResults_ReturnsErrorResponse()
    {
        // Arrange
        var parameters = new CaseManagementParameters();
        var agentId = "123A";
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<CaseManagementController>>();
        var mockManager = new Mock<ICaseManagementManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetCases(agentId, parameters, cancellationToken))
            .ReturnsAsync((CaseManagementResponse)null);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CaseManagementController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = mockHttpContext }
        };

        // Act
        var actionResult = await controller.GetCases(parameters, cancellationToken);
        var result = actionResult as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, details.Status);
    }

    [Fact]
    public async Task GetCases_MissingAgentId_ReturnsUnauthorized()
    {
        // Arrange
        var parameters = new CaseManagementParameters();
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<CaseManagementController>>();
        var mockManager = new Mock<ICaseManagementManager>(MockBehavior.Strict);
        var mockConfigurationManager = new Mock<IConfigurationManager>();

        var controller = new CaseManagementController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        // Act
        var actionResult = await controller.GetCases(parameters, cancellationToken);
        var result = actionResult as UnauthorizedResult;

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetFilterOptions_Success()
    {
        // Arrange
        var response = new CaseManagementFilters();
        var agentId = "123A";
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<CaseManagementController>>();
        var mockManager = new Mock<ICaseManagementManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetFilterOptions(agentId, cancellationToken))
            .ReturnsAsync(response);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CaseManagementController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = mockHttpContext }
        };

        // Act
        var actionResult = await controller.GetFilterOptions(cancellationToken);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task ResendEmail_Success()
    {
        // Arrange
        var envelopeId = "envelope123";
        var agentId = "123A";
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<CaseManagementController>>();
        var mockManager = new Mock<ICaseManagementManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.ResendEmail(envelopeId, cancellationToken))
            .ReturnsAsync(true);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new CaseManagementController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = mockHttpContext }
        };

        // Act
        var actionResult = await controller.ResendEmail(envelopeId, cancellationToken);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    private static HttpContext GetMockHttpContext(string agentId)
    {
        var claims = new List<Claim>
        {
            new Claim("agentId", agentId, string.Empty, "Ping")
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return new DefaultHttpContext
        {
            User = user,
            Items = new Dictionary<object, object>
            {
                { "AgentId", agentId },
                { "IsSubaccount", false }
            }
        };
    }
}
