namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.UserData;
using Assurity.AgentPortal.Managers.AgentHierarchy;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class UserDataControllerTests
{
    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetBusinessTypes(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockUserDataManager = new Mock<IUserDataManager>();
        bool includeAssociatedAgentNumbers = false;

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HashSet<Market>());
        }

        var mockAgentManager = new Mock<IAgentHierarchyManager>();
        mockAgentManager.Setup(manager => manager.GetAgentContracts(
                It.IsAny<string>(),
                includeAssociatedAgentNumbers,
                MarketCodeFilters.AgentCenter,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()))
            .ReturnsAsync(new AgentContractsResponse());

        var mockLogger = new Mock<ILogger<UserDataController>>();
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new UserDataController(mockLogger.Object, mockConfigurationManager.Object, mockAgentManager.Object, mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetBusinessTypes(CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode);
    }

    [Fact]
    public async Task GetAgentContracts_Success()
    {
        // Arrange
        var agentId = "ABC1";
        bool includeAssociatedAgentNumbers = false;

        var contractsResponse = new AgentContractsResponse();
        var mockLogger = new Mock<ILogger<UserDataController>>();
        var mockAgentManager = new Mock<IAgentHierarchyManager>(MockBehavior.Strict);
        mockAgentManager.Setup(manager => manager.GetAgentContracts(
               agentId,
               false,
               MarketCodeFilters.None,
               It.IsAny<CancellationToken>(),
               It.IsAny<string>()))
           .ReturnsAsync(contractsResponse);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Market>());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var userDataController = new UserDataController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockAgentManager.Object,
            mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await userDataController.GetAgentContracts(
            CancellationToken.None,
            includeAssociatedAgentNumbers,
            MarketCodeFilters.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetAgentContracts_ShouldReturn_ProblemDetails()
    {
        // Arrange
        var agentId = "123A";
        var contractsResponse = new AgentContractsResponse();
        var mockLogger = new Mock<ILogger<UserDataController>>();
        var mockAgentManager = new Mock<IAgentHierarchyManager>(MockBehavior.Strict);
        mockAgentManager.Setup(manager => manager.GetAgentContracts(
             It.IsAny<string>(),
             false,
             It.IsAny<MarketCodeFilters>(),
             It.IsAny<CancellationToken>(),
             It.IsAny<string>()))
         .ReturnsAsync(() => null);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Market>());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);
        bool includeAssociatedAgentNumbers = false;
        MarketCodeFilters marketCodeFilter = MarketCodeFilters.AgentCenter;

        var userDataController = new UserDataController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockAgentManager.Object,
            mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await userDataController.GetAgentContracts(
            CancellationToken.None,
            includeAssociatedAgentNumbers,
            marketCodeFilter);
        var result = actionResult as ObjectResult;

        var details = result.Value as ProblemDetails;

        // Assert
        Assert.Equal("An unexpected error occured.", details.Detail);
        Assert.Equal("InternalServerError", details.Title);
        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetAgentContracts_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "ABC1";
        var mockLogger = new Mock<ILogger<UserDataController>>();
        var mockAgentManager = new Mock<IAgentHierarchyManager>(MockBehavior.Strict);
        mockAgentManager.Setup(manager => manager.GetAgentContracts(
              It.IsAny<string>(),
              false,
              It.IsAny<MarketCodeFilters>(),
              It.IsAny<CancellationToken>(),
              It.IsAny<string>()))
            .ReturnsAsync(() => null);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetBusinessTypesByAgentId(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Market>());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);
        bool includeAssociatedAgentNumbers = false;
        MarketCodeFilters marketCodeFilter = MarketCodeFilters.AgentCenter;

        var userDataController = new UserDataController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockAgentManager.Object,
            mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await userDataController.GetAgentContracts(
           CancellationToken.None,
           includeAssociatedAgentNumbers,
           marketCodeFilter);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK, false)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError, false)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError, true)]
    public async Task GetAgentAccess(
        string agentId,
        int statusCode,
        bool hasNullResponse)
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserDataController>>();
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockAgentManager = new Mock<IAgentHierarchyManager>();
        var mockUserDataManager = new Mock<IUserDataManager>();

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            if (hasNullResponse)
            {
                mockUserDataManager.Setup(manager => manager.GetAgentAccess(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync((AccessLevel?)null);
            }
            else
            {
                mockUserDataManager.Setup(manager => manager.GetAgentAccess(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception());
            }
        }
        else
        {
            mockUserDataManager.Setup(manager => manager.GetAgentAccess(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccessLevel.Full);
        }

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new UserDataController(mockLogger.Object, mockConfigurationManager.Object, mockAgentManager.Object, mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            },
        };

        // Act
        var actionResult = await controller.GetAgentAccess(CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode);
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

    private void Validate(IActionResult actionResult, int statusCode)
    {
        if (statusCode == StatusCodes.Status200OK)
        {
            var result = actionResult as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            var result = actionResult as ObjectResult;
            var details = result.Value as ProblemDetails;

            Assert.NotNull(result);
            Assert.Equal(statusCode, details.Status);
        }
    }
}
