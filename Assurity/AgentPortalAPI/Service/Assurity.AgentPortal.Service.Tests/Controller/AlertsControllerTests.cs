namespace Assurity.AgentPortal.Service.Tests.Controller;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Assurity.AgentPortal.Contracts.Alerts;
using Assurity.AgentPortal.Managers.Alerts;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AlertsControllerTests
{
    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetDistributionEmails(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.GetDistributionEmailsByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.GetDistributionEmailsByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager
                .Setup(manager => manager.GetDistributionEmailsByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DistributionList> { new DistributionList { Email = "ABC1@fake.com", Id = 1337 } });
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetDistributionEmails(CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, true);
    }

    [Theory]
    [InlineData("ABC1", "ABC1@fake.com", StatusCodes.Status200OK, false)]
    [InlineData("ABC1", "", StatusCodes.Status400BadRequest, true)]
    [InlineData("", "ABC1@fake.com", StatusCodes.Status401Unauthorized, false)]
    [InlineData("ABC1", "ABC1@fake.com", StatusCodes.Status409Conflict, false)]
    [InlineData("ABC1", "ABC1@fake.com", StatusCodes.Status500InternalServerError, false)]
    public async Task AddDistributionEmail(
        string agentId,
        string email,
        int statusCode,
        bool expectObjectResult)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.AddDistributionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.AddDistributionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.AddDistributionEmail(email, CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, expectObjectResult);
    }

    [Theory]
    [InlineData("ABC1", 0, StatusCodes.Status200OK, false)]
    [InlineData("FAKE", -1, StatusCodes.Status400BadRequest, false)]
    [InlineData("", 0, StatusCodes.Status401Unauthorized, false)]
    [InlineData("ABC1", 0, StatusCodes.Status409Conflict, false)]
    [InlineData("ABC1", 0, StatusCodes.Status500InternalServerError, false)]
    public async Task DeleteDistributionEmail(
        string agentId,
        int emailId,
        int statusCode,
        bool expectObjectResult)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.DeleteDistributionEmail(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.DeleteDistributionEmail(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else if (statusCode == StatusCodes.Status400BadRequest)
        {
            mockManager
                .Setup(manager => manager.DeleteDistributionEmail(It.Is<int>(x => x == -1), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException());
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.DeleteDistributionEmail(emailId, CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, expectObjectResult);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task GetAlertPreferences(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.GetAlertPreferencesByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.GetAlertPreferencesByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else
        {
            mockManager
                .Setup(manager => manager.GetAlertPreferencesByAgentId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AlertPreferences());
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.GetAlertPreferences(CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, true);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK, true, false)]
    [InlineData("ABC1", StatusCodes.Status400BadRequest, false, true)]
    [InlineData("", StatusCodes.Status401Unauthorized, true, false)]
    [InlineData("ABC1", StatusCodes.Status409Conflict, true, false)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError, true, false)]
    public async Task AddOrUpdateAlertPreferences(
        string agentId,
        int statusCode,
        bool validParameter,
        bool expectObjectResult)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();
        AlertPreferences alertPreferences = null;

        if (validParameter)
        {
            alertPreferences = new AlertPreferences
            {
                DisableAll = true,
                SelfAdd = true,
                SelfMet = true,
                SelfOutstanding = true,
                HierarchyAdd = true,
                HierarchyMet = true,
                HierarchyOutstanding = true
            };
        }

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.AddOrUpdateAlertPreferences(It.IsAny<string>(), It.IsAny<AlertPreferences>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.AddOrUpdateAlertPreferences(It.IsAny<string>(), It.IsAny<AlertPreferences>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.AddOrUpdateAlertPreferences(alertPreferences, CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, expectObjectResult);
    }

    [Theory]
    [InlineData("ABC1", StatusCodes.Status200OK)]
    [InlineData("FAKE", StatusCodes.Status500InternalServerError)]
    [InlineData("", StatusCodes.Status401Unauthorized)]
    [InlineData("ABC1", StatusCodes.Status409Conflict)]
    [InlineData("ABC1", StatusCodes.Status500InternalServerError)]
    public async Task DeleteAlertPreferences(
        string agentId,
        int statusCode)
    {
        // Arrange
        var mockManager = new Mock<IAlertsManager>();

        if (statusCode == StatusCodes.Status409Conflict)
        {
            mockManager
                .Setup(manager => manager.DeleteAlertPreferences(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
        }
        else if (statusCode == StatusCodes.Status500InternalServerError)
        {
            mockManager
                .Setup(manager => manager.DeleteAlertPreferences(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
        }
        else if (statusCode == StatusCodes.Status400BadRequest)
        {
            mockManager
                .Setup(manager => manager.DeleteAlertPreferences(It.Is<string>(x => x == "FAKE"), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException());
        }

        var controller = SetUpMockAlertsController(agentId, mockManager);

        // Act
        var actionResult = await controller.DeleteAlertPreferences(CancellationToken.None);

        // Assert
        Validate(actionResult, statusCode, false);
    }

    private static DefaultHttpContext GetMockHttpContext(string agentId)
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

    private static AlertsController SetUpMockAlertsController(string agentId, Mock<IAlertsManager> mockManager)
    {
        var mockLogger = new Mock<ILogger<AlertsController>>();
        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        var mockHttpContext = GetMockHttpContext(agentId);

        return new AlertsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };
    }

    private static void Validate(IActionResult actionResult, int statusCode, bool expectObjectResult)
    {
        if (statusCode == StatusCodes.Status200OK)
        {
            if (expectObjectResult)
            {
                var result = actionResult as ObjectResult;
                Assert.NotNull(result);
                Assert.Equal(statusCode, result.StatusCode);
            }
            else
            {
                var result = actionResult as OkResult;
                Assert.Equal(statusCode, result.StatusCode);
            }
        }
        else if (statusCode == StatusCodes.Status400BadRequest)
        {
            if (expectObjectResult)
            {
                var result = actionResult as ObjectResult;
                var details = result.Value as ProblemDetails;
                Assert.NotNull(result);
                Assert.Equal(statusCode, details.Status);
            }
            else
            {
                var result = actionResult as StatusCodeResult;
                Assert.NotNull(result);
                Assert.Equal(statusCode, result.StatusCode);
            }
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
