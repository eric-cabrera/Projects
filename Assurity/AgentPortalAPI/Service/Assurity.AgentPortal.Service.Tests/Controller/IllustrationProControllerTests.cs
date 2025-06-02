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
public class IllustrationProControllerTests
{
    [Fact]
    public async Task GetAccountId_Success()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForHomeOfficeUser();

        var mockLogger = new Mock<ILogger<IllustrationProController>>();
        var mockManager = new Mock<IIllustrationProManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;
        var accountId = "accountId123";

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationProForHomeOfficeUser("ab1234"))
            .Returns(string.Empty);

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationPro(
            agentId,
            userName,
            cancellationToken))
            .ReturnsAsync(credentialsResponse);

        mockManager.Setup(manager => manager.GetIllustrationProAccountId(credentialsResponse))
            .ReturnsAsync(accountId);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var illustrationProController = new IllustrationProController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await illustrationProController.GetAccountId(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(accountId, result.Value);
    }

    [Fact]
    public async Task GetCredentails_No_Results_ShouldReturnProblemDetails()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<IllustrationProController>>();
        var mockManager = new Mock<IIllustrationProManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationPro(
            agentId,
            userName,
            cancellationToken))
            .ReturnsAsync(() => null);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var illustrationProController = new IllustrationProController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await illustrationProController.GetAccountId(CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Fact]
    public async Task GetCredentails_Cancellation_Token_ThrowsException()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<IllustrationProController>>();
        var mockManager = new Mock<IIllustrationProManager>(MockBehavior.Strict);
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = new CancellationToken(true);

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationPro(
            agentId,
            userName,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var illustrationProController = new IllustrationProController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await illustrationProController.GetAccountId(CancellationToken.None);
        var statusCodeResult = actionResult as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetAccountId_For_HomeOfficeUser_Success()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForHomeOfficeUser();
        var mockLogger = new Mock<ILogger<IllustrationProController>>();
        var mockManager = new Mock<IIllustrationProManager>(MockBehavior.Strict);
        var agentId = string.Empty;
        var accountId = "1111";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationProForHomeOfficeUser("un1234"))
            .Returns(credentialsResponse);

        mockManager.Setup(manager => manager.GetIllustrationProAccountId(credentialsResponse))
            .ReturnsAsync(accountId);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId, userName, true, true);

        var illustrationProController = new IllustrationProController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await illustrationProController.GetAccountId(CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(accountId, result.Value);
    }

    [Fact]
    public void GetRedirectURL_For_HomeOfficeUser_No_Results_ShouldReturnProblemDetails()
    {
        // Arrange
        var credentialsResponse = GetMockCredentialsResponseForAgent();

        var mockLogger = new Mock<ILogger<IllustrationProController>>();
        var mockManager = new Mock<IIllustrationProManager>(MockBehavior.Strict);
        var agentId = string.Empty;
        var userName = string.Empty;
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetCredentialsForIllustrationProForHomeOfficeUser("ab1234"))
            .Throws(new UnauthorizedAccessException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        var mockHttpContext = GetMockHttpContext(agentId, userName, false, false);

        var illustrationProController = new IllustrationProController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act & Assert
        var result = Assert.ThrowsAsync<Exception>(async () => await illustrationProController.GetAccountId(CancellationToken.None));
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
        var mockCredentialsResponse = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><agentSetup xmlns=\"http://wwww/stoneriver.com/services/AgentAccountService\"><requestObject><a:accountSetupRequestXML xmlns:a=\"http://schemas.datacontract.org/2004/07/LifeServer.DataModel.request\"><AgentAccountSetupData xmlns=\"\"><AGENT><UNIQUEID>user1234</UNIQUEID><UNIQUEPASS>TEST</UNIQUEPASS><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><EMAILTO></EMAILTO><ROLECODE>Default_Agent</ROLECODE><LANGUAGE>en-us</LANGUAGE><PROFILES><PROFILE><DISTRIBUTION>DIST_ASR_IS</DISTRIBUTION><AGENTID>1234</AGENTID><AGENCYID>1234</AGENCYID><AGENCYNAME/><ADDRESS1>122 ABC St</ADDRESS1><ADDRESS2/><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68512</ZIP><PHONE>3303261232</PHONE><EMAIL>abc123@gmail.com</EMAIL></PROFILE></PROFILES></AGENT></AgentAccountSetupData></a:accountSetupRequestXML></requestObject></agentSetup></s:Body></s:Envelope>";

        return mockCredentialsResponse;
    }

    private static string GetMockCredentialsResponseForHomeOfficeUser()
    {
        var mockCredentialsResponse = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><agentSetup xmlns=\"http://wwww/stoneriver.com/services/AgentAccountService\"><requestObject><a:accountSetupRequestXML xmlns:a=\"http://schemas.datacontract.org/2004/07/LifeServer.DataModel.request\"><AgentAccountSetupData xmlns=\"\"><AGENT><UNIQUEID>1</UNIQUEID><UNIQUEPASS>TEST</UNIQUEPASS><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><EMAILTO></EMAILTO><ROLECODE>Default_Agent</ROLECODE><LANGUAGE>en-us</LANGUAGE><PROFILES><PROFILE><DISTRIBUTION>DIST_ASR_IS</DISTRIBUTION><AGENTID>1234</AGENTID><AGENCYID>1234</AGENCYID><AGENCYNAME/><ADDRESS1>122 ABC St</ADDRESS1><ADDRESS2/><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68512</ZIP><PHONE>3303261232</PHONE><EMAIL>abc123@gmail.com</EMAIL></PROFILE></PROFILES></AGENT></AgentAccountSetupData></a:accountSetupRequestXML></requestObject></agentSetup></s:Body></s:Envelope>";
        return mockCredentialsResponse;
    }
}