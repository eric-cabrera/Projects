namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.AgentPortal.Managers.Claims;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class ClaimsControllerTests
{
    [Fact]
    public async Task GetClaims_Success()
    {
        // Arrange
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };

        var claimsResponse = GetMockClaimsResponse();

        var mockLogger = new Mock<ILogger<ClaimsController>>();
        var mockManager = new Mock<IClaimsManager>(MockBehavior.Strict);
        var agentId = "123A";
        var cancellationToken = CancellationToken.None;

        mockManager.Setup(manager => manager.GetClaims(
               agentId,
               claimsParameters,
               cancellationToken))
            .ReturnsAsync(claimsResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var claimsController = new ClaimsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await claimsController.GetClaims(claimsParameters, CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetClaims_No_Results_ReturnsErrorResponse()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<ClaimsController>>();
        var mockManager = new Mock<IClaimsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetClaims(
            agentId,
            claimsParameters,
            cancellationToken))
            .ReturnsAsync(() => null);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var claimsController = new ClaimsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await claimsController.GetClaims(claimsParameters, CancellationToken.None);
        var result = actionResult as ObjectResult;
        var details = result.Value as ProblemDetails;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, details.Status);
    }

    [Fact]
    public async Task GetClaims_Missing_Required_Field_ThrowsException()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters { };
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<ClaimsController>>();
        var mockManager = new Mock<IClaimsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetClaims(
            agentId,
            claimsParameters,
            cancellationToken));

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var claimsController = new ClaimsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await claimsController.GetClaims(claimsParameters, CancellationToken.None);
        var okResult = actionResult as ObjectResult;
        var problemDetails = okResult.Value as ProblemDetails;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
        Assert.Equal("One of ClaimNumber, PolicyNumber, ClaimantFirstName or ClaimantLastName is Required.", problemDetails.Detail);
    }

    [Fact]
    public async Task GetClaims_Cancellation_Token_ThrowsException()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };
        var cancellationToken = new CancellationToken(true);

        var mockLogger = new Mock<ILogger<ClaimsController>>();
        var mockManager = new Mock<IClaimsManager>(MockBehavior.Strict);

        mockManager.Setup(manager => manager.GetClaims(
            agentId,
            claimsParameters,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var claimsController = new ClaimsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await claimsController.GetClaims(claimsParameters, CancellationToken.None);
        var statusCodeResult = actionResult as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetClaims_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };
        var cancellationToken = CancellationToken.None;

        var mockLogger = new Mock<ILogger<ClaimsController>>();
        var mockManager = new Mock<IClaimsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetClaims(
            agentId,
            claimsParameters,
            cancellationToken))
            .Throws(new SystemException());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var claimsController = new ClaimsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await claimsController.GetClaims(claimsParameters, CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    private static HttpContext GetMockHttpContext(string agentId)
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
                { "IsSubaccount", false }
            }
        };

        context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

        return context;
    }

    private ClaimsResponse GetMockClaimsResponse()
    {
        var mockClaimsResponse = new ClaimsResponse
        {
            Claims = new List<Contracts.Claims.Claim>
            {
                new Contracts.Claims.Claim
                {
                    ClaimNumber = null,
                    Claimant = new Name
                    {
                        FirstName = "John",
                        LastName = "Smith"
                    },
                    DateReported = DateTime.Parse("12/12/2024"),
                    PaymentAmount = 1,
                    PolicyNumber = "4180078103",
                    PolicyType = "Disability Income",
                    Status = "Received",
                    StatusReason = string.Empty,
                    Details = [
                        new Contracts.Claims.ClaimDetail
                        {
                            DeliveryMethod = "Check",
                            BenefitDate = DateTime.Parse("12/12/2024"),
                            BenefitDescription = "Cancer",
                            PaymentAmount = 1,
                            PaymentDate = DateTime.Parse("12/12/2024"),
                            PolicyNumber = "4180078103",
                            Status = "Received"
                        }
                    ]
                },
            },
            Page = 1,
            PageSize = 10,
            TotalRecords = 1,
        };

        return mockClaimsResponse;
    }
}