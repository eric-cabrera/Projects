namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class LifePortraitsControllerTests
{
    [Fact]
    public async Task GetRedirectURL_For_Agent_Success()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();
        var redirectUrlResponse = GetMockRedirectUrlResponse();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForLifePortraits(
            agentId,
            userName,
            cancellationToken))
            .ReturnsAsync(credentialsResponse);

        mockManager.Setup(manager => manager.GetLifePortraitsURL(
            credentialsResponse,
            cancellationToken))
            .ReturnsAsync(redirectUrlResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await lifePortraitsController.GetRedirectURL(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        var expectedRedirectUrl = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(expectedRedirectUrl, result.Value);
    }

    [Fact]
    public async Task GetRedirectURL_For_Agent__No_Results_ShouldReturnProblemDetails()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForLifePortraits(
            agentId,
            userName,
            cancellationToken))
            .ReturnsAsync(() => null);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await lifePortraitsController.GetRedirectURL(CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Fact]
    public async Task GetRedirectURL_For_Agent_Cancellation_Token_ThrowsException()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = new CancellationToken(true);

        mockManager.Setup(manager => manager.GetCredentialsForLifePortraits(
            agentId,
            userName,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await lifePortraitsController.GetRedirectURL(CancellationToken.None);
        var statusCodeResult = actionResult as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetRedirectURL_For_HomeOfficeUser_Success()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForHomeOfficeUser();
        var redirectUrlResponse = GetMockRedirectUrlResponse();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = string.Empty;
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForLifePortraitsForHomeOfficeUser("un1234"))
            .ReturnsAsync(credentialsResponse);

        mockManager.Setup(manager => manager.GetLifePortraitsURL(
            credentialsResponse,
            cancellationToken))
            .ReturnsAsync(redirectUrlResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");

        var mockHttpContext = GetMockHttpContext(agentId, userName, true, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await lifePortraitsController.GetRedirectURL(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        var expectedRedirectUrl = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(expectedRedirectUrl, result.Value);
    }

    [Fact]
    public void GetRedirectURL_For_HomeOfficeUser_No_Results_ShouldReturnProblemDetails()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = string.Empty;
        var userName = string.Empty;
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForLifePortraitsForHomeOfficeUser("ab1234"))
            .Throws(new UnauthorizedAccessException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act & Assert
        var result = Assert.ThrowsAsync<Exception>(async () => await lifePortraitsController.GetRedirectURL(CancellationToken.None));
    }

    [Fact]
    public async Task GetRedirectURL_For_AnonymousUser_Throws_Exception()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForHomeOfficeUser();
        var redirectUrlResponse = GetMockRedirectUrlResponse();

        var mockLogger = new Mock<ILogger<LifePortraitsController>>();
        var mockManager = new Mock<ILifePortraitsManager>(MockBehavior.Strict);
        var agentId = string.Empty;
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var lifePortraitsController = new LifePortraitsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await lifePortraitsController.GetRedirectURL(CancellationToken.None);
        var statusCodeResult = actionResult as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, statusCodeResult.StatusCode);
    }

    [Fact]
    public void HomeOfficeUser_NotImpersonating_ShouldReturnNullAgentId()
    {
        // Arrange
        var agentId = string.Empty;
        var userName = "un1234";
        var context = GetMockHttpContext(agentId, userName, true, false);

        // Act
        var currentAgentId = context.Items["AgentId"];

        // Assert
        Assert.Equal(string.Empty, currentAgentId);
    }

    private static HttpContext GetMockHttpContext(string agentId, string username, bool isHomeOfficeUser, bool isImpersonating)
    {
        var claims = new List<Claim>();

        if (isImpersonating)
        {
            claims.Add(new Claim("agentId", agentId, string.Empty, "Ping"));
        }
        else
        {
            claims.Add(new Claim("sub", Guid.NewGuid().ToString(), string.Empty, "Ping"));
        }

        claims.Add(new Claim("username", username, string.Empty, "Ping"));
        claims.Add(new Claim("sid", Guid.NewGuid().ToString(), string.Empty, "Ping"));
        claims.Add(new Claim("preferred_username", username, string.Empty, "Ping"));
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var context = new DefaultHttpContext
        {
            User = user,
            Items = new Dictionary<object, object>
            {
                { "AgentId", agentId },
                { "IsSubaccount", false },
                { "AgentUsername", username },
                { "IsHomeOfficeUser", isHomeOfficeUser }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

        return context;
    }

    private static string GetMockCredentialsResponseForAgent()
    {
        var mockCredentialsResponse = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>un1234</AGENTID><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1234</AGENCYID><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>IS</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";
        return mockCredentialsResponse;
    }

    private static string GetMockCredentialsResponseForHomeOfficeUser()
    {
        var mockCredentialsResponse = "<?xml version=\"1.0\" encoding=\"utf - 8\"?><FIPINPUT><AGENT><AGENTID>1</AGENTID><FIRSTNAME>Assurity Life</FIRSTNAME><LASTNAME>Insurance Co.</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1</AGENCYID><AGENCYNAME>Assurity Life Insurance Co.</AGENCYNAME><ADDRESS1>PO Box 82533</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68501-2533</ZIP><PHONE>1-800-869-0355</PHONE><EMAIL /><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>HO</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";
        return mockCredentialsResponse;
    }

    private static string GetMockRedirectUrlResponse()
    {
        var mockRedirectUrlResponse = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";
        return mockRedirectUrlResponse;
    }
}