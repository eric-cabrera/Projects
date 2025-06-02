namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;
using Amazon.Runtime.Internal.Transform;
using Assurity.AgentPortal.Contracts.Integration;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using ComponentSpace.SAML2.Protocols;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class IPipelineControllerTests
{
    [Fact]
    public async Task GetIPipelineResponse_WithAgentIdParam_Success()
    {
        // Arrange
        var samlResponse = GetMockSAMLResponse();
        var providedAgentId = "5678";
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<IPipelineController>>();
        var mockManager = new Mock<IIPipelineManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature(
                providedAgentId,
                cancellationToken))
            .ReturnsAsync(samlResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager.Setup(manager => manager.IPipelineTargetString)
            .Returns("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=");
        mockConfigurationManager.Setup(manager => manager.IPipelineConnectionString)
            .Returns("https://federate-uat.ipipeline.com/sp/ACS.saml2");
        mockConfigurationManager.Setup(manager => manager.IPipelineAssurityCompanyId)
            .Returns("3046");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderCompanyId).Returns("58");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderAgentId).Returns("23");

        var mockHttpContext = GetMockHttpContext("1234");

        var iPipelineController = new IPipelineController(
                mockLogger.Object,
                mockConfigurationManager.Object,
                mockManager.Object,
                null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await iPipelineController.GetIPipelineResponse(cancellationToken, providedAgentId);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.IsType<IPipelineResponse>(result.Value);

        var responseObj = (IPipelineResponse)result.Value;
        Assert.Equal(samlResponse, responseObj.SAMLResponse);
        Assert.Equal("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=3046", responseObj.IPipelineTargetString);
        Assert.Equal("https://federate-uat.ipipeline.com/sp/ACS.saml2", responseObj.IPipelineConnectionString);
    }

    [Fact]
    public async Task GetIPipelineResponse_Success()
    {
        // Arrange
        var samlResponse = GetMockSAMLResponse();

        var mockLogger = new Mock<ILogger<IPipelineController>>();
        var mockManager = new Mock<IIPipelineManager>(MockBehavior.Strict);
        var agentId = "1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature(
            agentId,
            cancellationToken))
            .ReturnsAsync(samlResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager.Setup(manager => manager.IPipelineTargetString).Returns("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=");
        mockConfigurationManager.Setup(manager => manager.IPipelineConnectionString).Returns("https://federate-uat.ipipeline.com/sp/ACS.saml2");
        mockConfigurationManager.Setup(manager => manager.IPipelineAssurityCompanyId).Returns("3046");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderCompanyId).Returns("58");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderAgentId).Returns("23");

        var mockHttpContext = GetMockHttpContext(agentId);

        var iPipelineController = new IPipelineController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await iPipelineController.GetIPipelineResponse(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        var expectedSamlResponse = "PHNhbWxwOlJlc3BvbnNlIElEPSJfM2ZhNjhiNDctMmZmYS00M2U2LWFhY2QtOGI1MGVlMjUwN2NmIiBWZXJzaW9";

        Assert.Equal(200, result.StatusCode);
        Assert.IsType<IPipelineResponse>(result.Value);

        var responseObj = (IPipelineResponse)result.Value;
        Assert.Equal(expectedSamlResponse, responseObj.SAMLResponse);
        Assert.Equal("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=3046", responseObj.IPipelineTargetString);
        Assert.Equal("https://federate-uat.ipipeline.com/sp/ACS.saml2", responseObj.IPipelineConnectionString);
    }

    [Fact]
    public async Task GetIPipelineResponse_No_Results_ShouldReturnProblemDetails()
    {
        // Arrange
        var samlResponse = GetMockSAMLResponse();

        var mockLogger = new Mock<ILogger<IPipelineController>>();
        var mockManager = new Mock<IIPipelineManager>(MockBehavior.Strict);
        var agentId = "1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature(
            agentId,
            cancellationToken))
            .ReturnsAsync(() => null);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var iPipelineController = new IPipelineController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await iPipelineController.GetIPipelineResponse(CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Fact]
    public async Task GetIPipelineResponse_Cancellation_Token_ThrowsException()
    {
        // Arrange
        var samlResponse = GetMockSAMLResponse();

        var mockLogger = new Mock<ILogger<IPipelineController>>();
        var mockManager = new Mock<IIPipelineManager>(MockBehavior.Strict);
        var agentId = "1234";
        var cancellationToken = new CancellationToken(true);

        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature(
            agentId,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var iPipelineController = new IPipelineController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await iPipelineController.GetIPipelineResponse(CancellationToken.None);
        var statusCodeResult = actionResult as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetIPipelineHomeOfficeResponse_Success()
    {
        // Arrange
        var homeOfficeId = GetMockCredentialsResponseForHomeOfficeUser();
        var samlResponse = GetMockSAMLResponse();

        var mockLogger = new Mock<ILogger<IPipelineController>>();
        var mockManager = new Mock<IIPipelineManager>(MockBehavior.Strict);
        var cancellationToken = CancellationToken.None;
        var expectedSamlResponse = "PHNhbWxwOlJlc3BvbnNlIElEPSJfM2ZhNjhiNDctMmZmYS00M2U2LWFhY2QtOGI1MGVlMjUwN2NmIiBWZXJzaW9";

        // Setup for parameterless method
        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature())
            .Returns(expectedSamlResponse);

        // Setup for method with parameters
        mockManager.Setup(manager => manager.GetBrowserPostSamlSignature(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSamlResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
        mockConfigurationManager.Setup(manager => manager.IPipelineTargetString).Returns("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=");
        mockConfigurationManager.Setup(manager => manager.IPipelineConnectionString).Returns("https://federate-uat.ipipeline.com/sp/ACS.saml2");
        mockConfigurationManager.Setup(manager => manager.IPipelineAssurityCompanyId).Returns("3046");
        mockConfigurationManager.Setup(manager => manager.IPipelineAssurityCompanyId).Returns("3046");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderCompanyId).Returns("58");
        mockConfigurationManager.Setup(manager => manager.IPipelineZanderAgentId).Returns("23");

        var mockHttpContext = GetMockHttpContext(homeOfficeId, true);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetAdditionalAgentIds("ab1234", cancellationToken))
            .ReturnsAsync(new List<string>());

        var iPipelineController = new IPipelineController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object, mockUserDataManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await iPipelineController.GetIPipelineResponse(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.IsType<IPipelineResponse>(result.Value);

        var responseObj = (IPipelineResponse)result.Value;
        Assert.Equal(expectedSamlResponse, responseObj.SAMLResponse);
        Assert.Equal("https://pipepasstoigo-uat3.ipipeline.com/default.aspx?gaid=3046", responseObj.IPipelineTargetString);
        Assert.Equal("https://federate-uat.ipipeline.com/sp/ACS.saml2", responseObj.IPipelineConnectionString);
    }

    private static HttpContext GetMockHttpContext(string agentId, bool isHomeOffice = false)
    {
        var claims = new List<System.Security.Claims.Claim>
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
                { "IsSubaccount", false },
                { "IsHomeOfficeUser", isHomeOffice }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

        return context;
    }

    private static string GetMockSAMLResponse()
    {
        var mockCredentialsResponse = "PHNhbWxwOlJlc3BvbnNlIElEPSJfM2ZhNjhiNDctMmZmYS00M2U2LWFhY2QtOGI1MGVlMjUwN2NmIiBWZXJzaW9";
        return mockCredentialsResponse;
    }

    private string GetMockCredentialsResponseForHomeOfficeUser()
    {
        return "HO9999";
    }
}
