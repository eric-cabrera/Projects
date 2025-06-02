namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.ApplicationTracker;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.ApplicationTracker.Contracts;
using Assurity.ApplicationTracker.Contracts.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

using AppTrackerDTO = Assurity.ApplicationTracker.Contracts.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class ApplicationTrackerApiAccessorTests
{
    [Fact]
    public async Task GetEvents_Success()
    {
        var mockResponse = GetMockPagedEventsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var accessor = GetApplicationTrackerApiAccessor(mockHttpMessageHandler);

        var parameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 2,
            ViewAsAgentId = "agent456",
            EventTypes = $"{EventType.InterviewStarted};{EventType.ApplicationSubmitted}"
        };

        var response = await accessor.GetCases("agent123", parameters);

        var expectedBaseUrl = "http://localhost/api/v1/Tracker/Event";
        var expectedQueryParams = new List<KeyValuePair<string, string>>
        {
            new("PageNumber", "1"),
            new("PageSize", "2"),
            new("AgentId", "agent456"),
            new("SortOrder", "Ascending"),
            new("EventTypes", EventType.InterviewStarted.ToString()),
            new("EventTypes", EventType.ApplicationSubmitted.ToString())
        };

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri != null &&
                req.RequestUri.ToString().StartsWith(expectedBaseUrl) &&
                QueryParamsMatch(req.RequestUri.Query, expectedQueryParams)),
            ItExpr.IsAny<CancellationToken>());

        Assert.NotNull(response);
        Assert.Equal(JsonConvert.SerializeObject(mockResponse.Data), JsonConvert.SerializeObject(response.Data));
    }

    [Fact]
    public async Task GetEvents_FailsWith404WithResponseBody_ReturnsBlankObject()
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Error")
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var accessor = GetApplicationTrackerApiAccessor(mockHttpMessageHandler);

        var parameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10,
            EventTypes = $"{EventType.InterviewStarted};{EventType.ApplicationSubmitted}"
        };

        var response = await accessor.GetCases("agent123", parameters);

        var expectedBaseUrl = "http://localhost/api/v1/Tracker/Event";
        var expectedQueryParams = new List<KeyValuePair<string, string>>
        {
            new("PageNumber", "1"),
            new("PageSize", "10"),
            new("AgentId", "agent123"),
            new("SortOrder", "Ascending"),
            new("EventTypes", EventType.InterviewStarted.ToString()),
            new("EventTypes", EventType.ApplicationSubmitted.ToString())
        };

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri != null &&
                req.RequestUri.ToString().StartsWith(expectedBaseUrl) &&
                QueryParamsMatch(req.RequestUri.Query, expectedQueryParams)),
            ItExpr.IsAny<CancellationToken>());

        Assert.NotNull(response);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task GetEvents_FailsWith404WithoutResponseBody_ThrowsException()
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = null
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var accessor = GetApplicationTrackerApiAccessor(mockHttpMessageHandler);

        var parameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10,
            EventTypes = $"{EventType.InterviewStarted};{EventType.ApplicationSubmitted}"
        };

        await Assert.ThrowsAsync<Exception>(async () => await accessor.GetCases("agent123", parameters));

        var expectedBaseUrl = "http://localhost/api/v1/Tracker/Event";
        var expectedQueryParams = new List<KeyValuePair<string, string>>
        {
            new("PageNumber", "1"),
            new("PageSize", "10"),
            new("AgentId", "agent123"),
            new("SortOrder", "Ascending"),
            new("EventTypes", EventType.InterviewStarted.ToString()),
            new("EventTypes", EventType.ApplicationSubmitted.ToString())
        };

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri != null &&
                req.RequestUri.ToString().StartsWith(expectedBaseUrl) &&
                QueryParamsMatch(req.RequestUri.Query, expectedQueryParams)),
            ItExpr.IsAny<CancellationToken>());
    }

    private ApplicationTrackerApiAccessor GetApplicationTrackerApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockHttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var mockLogger = new Mock<ILogger<ApplicationTrackerApiAccessor>>();

        return new ApplicationTrackerApiAccessor(mockHttpClient, mockLogger.Object);
    }

    private Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
    {
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

    private PagedEvents GetMockPagedEventsResponse()
    {
        return new PagedEvents
        {
            Data = new List<AppTrackerDTO.ApplicationTracker>
            {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow.AddDays(-1),
                            Event = EventType.InterviewStarted
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.ApplicationSubmitted
                        }
                    }
                }
            },
            NumberOfPages = 2
        };
    }

    private bool QueryParamsMatch(string actualQuery, List<KeyValuePair<string, string>> expectedParams)
    {
        var actualParams = QueryHelpers.ParseQuery(actualQuery);

        foreach (var param in expectedParams)
        {
            if (!actualParams.TryGetValue(param.Key, out var values) || !values.Contains(param.Value))
            {
                return false;
            }
        }

        return true;
    }
}
