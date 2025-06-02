namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Security.Claims;
using Assurity.AgentPortal.Contracts.Impersonation;
using Assurity.AgentPortal.Managers.Impersonation;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Bogus;
using global::MongoDB.Bson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class ImpersonationControllerTests
{
    [Fact]
    public async Task Search_SuccessfulResponse()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>();
        var mockManager = new Mock<IImpersonationManager>();
        mockManager.Setup(x => x.SearchAgents(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockImpersonationResponse());

        var controller = new ImpersonationController(null, mockConfigurationManager.Object, mockManager.Object);

        // Act
        var response = await controller.Search("abc123", CancellationToken.None);

        // Assert
        var result = response as ObjectResult;

        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task Impersonate_SuccessfulResponse()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>();
        var mockManager = new Mock<IImpersonationManager>();
        mockManager.Setup(x => x.ImpersonateAgent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ImpersonationRecord());

        var mockHttpContext = GetMockHttpContext();

        var controller = new ImpersonationController(null, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.Impersonate(new ImpersonationRecord { Id = "123" });

        // Assert
        var result = response as ObjectResult;

        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetRecentImpersonations_Success()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>();
        var mockManager = new Mock<IImpersonationManager>();
        mockManager.Setup(x => x.GetRecentImpersonations(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ImpersonationRecord>());

        var mockHttpContext = GetMockHttpContext();

        var controller = new ImpersonationController(null, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var response = await controller.GetRecentImpersonations(CancellationToken.None);

        // Assert
        var result = response as ObjectResult;

        Assert.Equal(200, result.StatusCode);
    }

    private static List<ImpersonationRecord> GetMockImpersonationResponse()
    {
        var agentFaker = new Faker<AgentRecord>()
            .RuleFor(x => x.AgentIds, f => f.Make(3, () => f.Random.AlphaNumeric(4)))
            .RuleFor(x => x.Name, f => f.Person.FullName);
        var impersonationLogFaker = new Faker<ImpersonationRecord>()
            .RuleFor(x => x.Id, f => ObjectId.GenerateNewId().ToString())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.RegisteredAgentId, f => f.Random.String(4))
            .RuleFor(x => x.Agents, f => f.Make(f.Random.Int(1, 10), () => agentFaker.Generate()))
            .RuleFor(x => x.UserName, f => f.Person.UserName);

        return impersonationLogFaker.Generate(20);
    }

    private static HttpContext GetMockHttpContext()
    {
        var claims = new List<Claim>
        {
            new("sid", Guid.NewGuid().ToString(), string.Empty, "AzureAd"),
            new("sub", Guid.NewGuid().ToString(), string.Empty, "AzureAd"),
            new("preferred_username", "TestUser", string.Empty, "AzureAd")
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var context = new DefaultHttpContext
        {
            User = user,
            Items = new Dictionary<object, object>
            {
                { "AgentId", Guid.NewGuid().ToString() },
                { "IsSubaccount", false }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

        return context;
    }
}
