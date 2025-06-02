namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using Assurity.AgentPortal.Managers.ProductionCredit;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class ProductionCreditControllerTests
{
    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetIndividualProductionCreditSummary(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IProductionCreditManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager.Setup(manager => manager.GetIndividualProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager.Setup(manager => manager.GetIndividualProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager.Setup(manager => manager.GetIndividualProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IndividualProductionCreditSummary());
        }

        var controller = SetUpMockProductionCreditController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetIndividualProductionCreditSummary(new ProductionCreditParameters(), CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetIndividualPolicyDetailsSummary(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IProductionCreditManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager.Setup(manager => manager.GetIndividualPolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager.Setup(manager => manager.GetIndividualPolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager.Setup(manager => manager.GetIndividualPolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductionCreditPolicyDetailsSummary());
        }

        var controller = SetUpMockProductionCreditController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetIndividualPolicyDetailsSummary(new ProductionCreditPolicyDetailsParameters(), CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetWorksiteProductionCreditSummary(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IProductionCreditManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager.Setup(manager => manager.GetWorksiteProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<WorksiteProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager.Setup(manager => manager.GetWorksiteProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<WorksiteProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager.Setup(manager => manager.GetWorksiteProductionCreditSummary(
                It.IsAny<string>(),
                It.IsAny<WorksiteProductionCreditParameters>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WorksiteProductionCreditSummary());
        }

        var controller = SetUpMockProductionCreditController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetWorksiteProductionCreditSummary(new WorksiteProductionCreditParameters(), CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetWorksitePolicyDetailsSummary(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IProductionCreditManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager.Setup(manager => manager.GetWorksitePolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager.Setup(manager => manager.GetWorksitePolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager.Setup(manager => manager.GetWorksitePolicyDetailsSummary(
                It.IsAny<string>(),
                It.IsAny<ProductionCreditPolicyDetailsParameters>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductionCreditPolicyDetailsSummary());
        }

        var controller = SetUpMockProductionCreditController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetWorksitePolicyDetailsSummary(new ProductionCreditPolicyDetailsParameters(), CancellationToken.None);

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

    private ProductionCreditController SetUpMockProductionCreditController(string agentId, Mock<IProductionCreditManager> mockManager)
    {
        var mockLogger = new Mock<ILogger<ProductionCreditController>>();
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        return new ProductionCreditController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };
    }

    private void Validate(IActionResult actionResult, int statusCode)
    {
        if (statusCode == StatusCodes.Status200OK)
        {
            var result = actionResult as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
        }
        else if (statusCode == StatusCodes.Status401Unauthorized)
        {
            var result = actionResult as UnauthorizedResult;

            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);
        }
        else if (statusCode == StatusCodes.Status409Conflict)
        {
            var result = actionResult as StatusCodeResult;

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
