namespace Assurity.AgentPortal.Service.Tests.Controller;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.ListBill;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Managers.ListBill;
using Assurity.AgentPortal.Service.Controllers;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class ListBillsControllerTests
{
    [Fact]
    public async Task GetGroups_Success()
    {
        // Arrange
        var agentId = "ABC1";
        var page = 1;
        var pageSize = 10;

        var groupsResponse = new GroupsResponse()
        {
            Page = 1,
            PageSize = 10,
            TotalRecords = 1,
            Groups = new List<Group>() { }
        };
        var mockLogger = new Mock<ILogger<ListBillsController>>();
        var mockManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetGroups(
                agentId,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupsResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var listBillController = new ListBillsController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await listBillController.GetGroups(
            1,
            100,
            CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetGroups_ShouldReturn_ProblemDetails()
    {
        // Arrange
        var agentId = "123A";
        var groupsResponse = new GroupsResponse { TotalRecords = 0 };
        var mockListBillManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockListBillManager
            .Setup(m => m.GetGroups(agentId, 1, 1000, CancellationToken.None))
            .ReturnsAsync(groupsResponse);

        var listBillController = SetUpMockListBillController(agentId);

        // Act
        var actionResult = await listBillController.GetGroups(
            1,
            100,
            CancellationToken.None);
        var result = actionResult as ObjectResult;

        var details = result.Value as ProblemDetails;

        Assert.Equal("An unexpected error occured.", details.Detail);
        Assert.Equal("InternalServerError", details.Title);
        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetGroups_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "ABC1";
        var page = 1;
        var pageSize = 10;

        var mockListBillManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockListBillManager
            .Setup(m => m.GetGroups(agentId, 1, 1000, CancellationToken.None))
            .ThrowsAsync(new Exception());

        var listBillController = SetUpMockListBillController(agentId);

        // Act
        var actionResult = await listBillController.GetGroups(
            1,
            1000,
            CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Fact]
    public async Task GetListBills_Success()
    {
        // Arrange
        var groupId = "12345678";
        var agentId = "ABC1";

        var listBillResponse = new ListBillsResponse()
        {
            GroupNumber = groupId,
            ListBills = new List<ListBill>()
        };
        var mockLogger = new Mock<ILogger<ListBillsController>>();
        var mockManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetListBills(
               groupId, CancellationToken.None))
            .ReturnsAsync(listBillResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var listBillController = new ListBillsController(mockLogger.Object, mockConfigurationManager.Object, mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await listBillController.GetListBills(groupId, CancellationToken.None);
        var result = actionResult as OkObjectResult;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task GetListBills_No_Results_ReturnsErrorResponse()
    {
        // Arrange
        var groupId = "12345678";
        var agentId = "ABC1";

        var mockLogger = new Mock<ILogger<ListBillsController>>();
        var mockManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetListBills(
                groupId, CancellationToken.None))
            .ReturnsAsync(() => new ListBillsResponse());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var listBillController = new ListBillsController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await listBillController.GetListBills(groupId, CancellationToken.None);
        var result = actionResult as ObjectResult;
        var details = result.Value as ProblemDetails;

        Assert.Equal(500, details.Status);
    }

    [Fact]
    public async Task GetListBills_ManagerThrowsException_ShouldReturnProblemDetails()
    {
        // Arrange
        var agentId = "ABC1";
        var page = 1;
        var pageSize = 10;
        var groupId = "12345678";

        var mockListBillManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockListBillManager
            .Setup(m => m.GetListBills(groupId, CancellationToken.None))
            .ThrowsAsync(new Exception());

        var listBillController = SetUpMockListBillController(agentId);

        // Act
        var actionResult = await listBillController.GetListBills(groupId, CancellationToken.None);
        var okResult = actionResult as ObjectResult;

        // Assert
        Assert.IsType<ProblemDetails>(okResult.Value);
    }

    [Fact]
    public async Task GetListBillData_ReturnsBadRequest_WhenListBillIdIsNullOrWhiteSpace()
    {
        // Arrange
        var agentId = "ABC1";
        var listBillId = " ";

        var listBillController = SetUpMockListBillController(agentId);

        // Act
        var actionResult = await listBillController.GetListBillData(listBillId, CancellationToken.None);
        var result = actionResult as ObjectResult;

        var details = result.Value as ProblemDetails;

        // Assert
        Assert.Equal("List Bill Id cannot be null or empty.", details.Detail);
        Assert.Equal("BadRequest", details.Title);
        Assert.Equal(400, details.Status);
    }

    [Fact]
    public async Task GetListBillData_Success_ShouldReturnPDFFile()
    {
        // Arrange
        var agentId = "ABC1";
        var listBillId = "18297TEST10000A7";

        var fileResponse = new FileResponse(
            $"{listBillId}-{DateTime.Now:MMddyyyy}.pdf",
            "pdf")
        {
            FileData = new byte[0]
        };

        var mockLogger = new Mock<ILogger<ListBillsController>>();
        var mockManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockManager.Setup(manager => manager.GetListBillData(
                listBillId, CancellationToken.None))
            .ReturnsAsync(fileResponse);

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var listBillController = new ListBillsController(
            mockLogger.Object,
            mockConfigurationManager.Object,
            mockManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        // Act
        var actionResult = await listBillController.GetListBillData(listBillId, CancellationToken.None);
        var result = actionResult as FileContentResult;

        // Assert
        Assert.IsType<FileContentResult>(actionResult);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal($"{listBillId}-{DateTime.Now:MMddyyyy}.pdf", result.FileDownloadName);
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

    private ListBillsController SetUpMockListBillController(string agentId)
    {
        var mockLogger = new Mock<ILogger<ListBillsController>>();

        var mockListBillManager = new Mock<IListBillsManager>(MockBehavior.Strict);
        mockListBillManager.Setup(manager => manager.GetGroups(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new GroupsResponse());

        var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
        mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
        mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

        var mockHttpContext = GetMockHttpContext(agentId);

        var controller = new ListBillsController(mockLogger.Object, mockConfigurationManager.Object, mockListBillManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext,
            },
        };

        return controller;
    }
}