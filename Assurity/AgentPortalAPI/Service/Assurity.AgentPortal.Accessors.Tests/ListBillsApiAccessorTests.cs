namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.ListBill;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.ListBill.Service.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class ListBillsApiAccessorTests
{
    private readonly Uri baseAddress = new Uri("http://localhost");

    [Fact]
    public async Task GetGroups_Success()
    {
        // Arrange
        var agentId = "123A";
        var page = 1;
        var pageSize = 10;
        var mockResponse = new GroupsResponse()
        {
            Page = 1,
            PageSize = 10,
            TotalRecords = 47,
            Groups =
            [
                new()
                {
                    Id = "1100234481",
                    Name = "Test Systems Inc - Biweekly",
                    City = "New Park",
                    State = "IL"
                },
            ]
        };
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);

        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };

        var listBillsApiAccessor = GetListBillsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await listBillsApiAccessor.GetGroups(
            agentId,
            mockResponse.Page,
            mockResponse.PageSize,
            CancellationToken.None);

        var expectedUrl = mockHttpClient.BaseAddress + "v1/groups/" + agentId + "?page=" + page + "&pageSize=" + pageSize;

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetListBills_Success()
    {
        // Arrange
        var groupId = "12345678";

        var mockResponse = new ListBillResponse()
        {
            GroupNumber = "1234567890",
            ListBills =
            [
                new()
                {
                    Id = "1000000006",
                    Date = DateTime.Now.Date,
                },
                new()
                {
                    Id = "1000000005",
                    Date = DateTime.Now.Date.AddMonths(-1),
                },
                new()
                {
                    Id = "1000000004",
                    Date = DateTime.Now.Date.AddMonths(-2),
                },
                new()
                {
                    Id = "1000000003",
                    Date = DateTime.Now.Date.AddMonths(-3),
                },
                new()
                {
                    Id = "1000000002",
                    Date = DateTime.Now.Date.AddMonths(-4),
                },
                new()
                {
                    Id = "1000000001",
                    Date = DateTime.Now.Date.AddMonths(-5),
                },
            ]
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };
        var listBillsApiAccessor = GetListBillsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await listBillsApiAccessor.GetListBills(groupId, CancellationToken.None);

        var expectedUrl = mockHttpClient.BaseAddress + "v1/groups/" + groupId + "/listBills";

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetListBills_IfUnsuccessful_ThrowsException()
    {
        // Arrange
        var groupId = "12345678";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.InternalServerError);
        var listBillsApiAccessor = GetListBillsApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await listBillsApiAccessor.GetListBills(groupId, CancellationToken.None));
    }

    [Fact]
    public async Task GetListBillData_Success()
    {
        var listBillId = "18297TEST10000A7";
        var stream = new MemoryStream();

        var mockHttpMessageHandler = GetMockHttpMessageHandler(new StreamContent(stream), HttpStatusCode.OK);
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };
        var accessor = GetListBillsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await accessor.GetListBillData(listBillId, CancellationToken.None);
        var expectedUrl = mockHttpClient.BaseAddress + "v1/listBills/" + listBillId + "?fileFormat=Pdf";

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetListBillData_IfUnsuccessful_ThrowsException()
    {
        // Arrange
        var listBillId = "18297TEST10000A7";
        var stream = new MemoryStream();

        var mockHttpMessageHandler = GetMockHttpMessageHandler(new StreamContent(stream), HttpStatusCode.InternalServerError);

        var listBillsApiAccessor = GetListBillsApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await listBillsApiAccessor.GetListBillData(listBillId, CancellationToken.None));
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(
        object? mockResponse,
        HttpStatusCode statusCode)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage)
            .Verifiable();

        return mockHttpMessageHandler;
    }

    private ListBillsApiAccessor GetListBillsApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockhttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = baseAddress
        };

        var mockLogger = new Mock<ILogger<ListBillsApiAccessor>>(MockBehavior.Loose);

        return new ListBillsApiAccessor(mockhttpClient, mockLogger.Object);
    }
}