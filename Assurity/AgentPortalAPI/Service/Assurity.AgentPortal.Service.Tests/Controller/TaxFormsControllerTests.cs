namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Contracts.TaxForms;
using Assurity.AgentPortal.Managers.TaxForms;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class TaxFormsControllerTests
{
    [Fact]
    public async Task GetTaxForms_Success()
    {
        // Arrange
        var agentId = "123A";

        var taxFormsResponse = new List<TaxForm>();

        var mockLogger = new Mock<ILogger<TaxFormsController>>();
        var mockManager = new Mock<ITaxFormsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetTaxForms(
            agentId,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(taxFormsResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new TaxFormsController(
            mockLogger.Object,
            mockManager.Object,
            mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetTaxForms(
            CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetTaxForm_Success()
    {
        // Arrange
        var agentId = "123A";
        var formId = "6test3F8C32650!F1959E857E9D2DE921F847";

        var fileResponse = new FileResponse(
            $"{formId}-{DateTime.Now:MMddyyyy}.pdf",
            "pdf")
        {
            FileData = new byte[0]
        };

        var mockLogger = new Mock<ILogger<TaxFormsController>>();
        var mockManager = new Mock<ITaxFormsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetTaxForm(
                agentId,
                formId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new TaxFormsController(
            mockLogger.Object,
            mockManager.Object,
            mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetTaxForm(
            formId,
            CancellationToken.None);
        var result = actionResult as FileContentResult;

        // Assert
        Assert.IsType<FileContentResult>(actionResult);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal($"{formId}-{DateTime.Now:MMddyyyy}.pdf", result.FileDownloadName);
    }

    [Fact]
    public async Task GetTaxForms_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "ABC1";

        var mockManager = new Mock<ITaxFormsManager>(MockBehavior.Strict);
        mockManager
            .Setup(m => m.GetTaxForms(agentId, CancellationToken.None))
            .ThrowsAsync(new Exception());

        var mockLogger = new Mock<ILogger<TaxFormsController>>();

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new TaxFormsController(
            mockLogger.Object,
            mockManager.Object,
            mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetTaxForms(CancellationToken.None);
        var result = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(result.Value);
    }

    [Fact]
    public async Task GetTaxForm_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "ABC1";
        var formId = "6test3F8C32650!F1959E857E9D2DE921F847";

        var mockManager = new Mock<ITaxFormsManager>(MockBehavior.Strict);
        mockManager
            .Setup(m => m.GetTaxForm(agentId, formId, CancellationToken.None))
            .ThrowsAsync(new Exception());

        var mockLogger = new Mock<ILogger<TaxFormsController>>();

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new TaxFormsController(
            mockLogger.Object,
            mockManager.Object,
            mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetTaxForm(formId, CancellationToken.None);
        var result = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(result.Value);
    }

    [Fact]
    public async Task GetTaxForm_ReturnsBadRequest_WhenFormIdIsNullOrWhiteSpace()
    {
        // Arrange
        var agentId = "ABC1";

        var mockManager = new Mock<ITaxFormsManager>(MockBehavior.Strict);
        mockManager
            .Setup(m => m.GetTaxForm(agentId, null, CancellationToken.None))
            .ThrowsAsync(new Exception());

        var mockLogger = new Mock<ILogger<TaxFormsController>>();

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new TaxFormsController(
            mockLogger.Object,
            mockManager.Object,
            mockConfigurationManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await controller.GetTaxForm(null, CancellationToken.None);
        var result = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(result.Value);
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