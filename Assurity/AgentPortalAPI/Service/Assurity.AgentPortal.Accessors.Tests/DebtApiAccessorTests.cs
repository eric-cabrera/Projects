namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon.Runtime.Internal.Util;
using Assurity.AgentPortal.Accessors.CommissionsAndDebt;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.Tests.TestData;
using Assurity.Commissions.Debt.Contracts.Advances;
using AutoBogus;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class DebtApiAccessorTests
{
    [Theory]
    [ClassData(typeof(DebtParametersGenerator))]
    public async Task GetUnsecuredAdvances_ClassData_ShouldSucceed(
        string agentId,
        DebtParameters parameters)
    {
        // Arrange
        var mockResponse = new AgentDetailsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var queryParams = GetQueryParams(agentId, parameters);

        var expectedUrl = new Uri(QueryHelpers.AddQueryString("http://localhost/v1/debt/unsecuredAdvances", queryParams));

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetUnsecuredAdvances(agentId, parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(250)]
    [InlineData(482)]
    [InlineData(1023)]
    [InlineData(7592)]
    public async Task GetAllUnsecuredAdvances_ShouldPaginate_ShouldSucceed(int totalRecords)
    {
        // Arrange
        var mockResponse = new AutoFaker<AgentDetailsResponse>()
            .RuleFor(response => response.TotalRecords, fake => totalRecords)
            .RuleFor(response => response.PageSize, fake => 100)
            .RuleFor(response => response.Page, fake => 1)
            .Generate();

        var responses = new Queue<HttpResponseMessage>();

        var totalPages = Math.Ceiling((double)mockResponse.TotalRecords / mockResponse.PageSize);

        for (int i = 0; i <= totalPages; i++)
        {
            responses.Enqueue(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
            });

            mockResponse.Page += 1;
        }

        var mockHttpMessageHandler = GetMockHttpMessageHandler(responses);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var debtParams = new DebtParameters();

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetAllUnsecuredAdvances(agentIds[0], debtParams, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockResponse.Agents.Count * totalPages, result.Count);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly((int)totalPages),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [ClassData(typeof(DebtParametersGenerator))]
    public async Task GetSecuredAdvances_NoFilters_ShouldSucceed(
    string agentId,
    DebtParameters parameters)
    {
        // Arrange
        var mockResponse = new AgentDetailsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var queryParams = GetQueryParams(agentId, parameters);

        var expectedUrl = new Uri(QueryHelpers.AddQueryString("http://localhost/v1/debt/securedAdvances", queryParams));

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetSecuredAdvances(agentId, parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(250)]
    [InlineData(482)]
    [InlineData(1023)]
    [InlineData(7592)]
    public async Task GetAllSecuredAdvances_ShouldPaginate_ShouldSucceed(int totalRecords)
    {
        // Arrange
        var mockResponse = new AutoFaker<AgentDetailsResponse>()
            .RuleFor(response => response.TotalRecords, fake => totalRecords)
            .RuleFor(response => response.PageSize, fake => 100)
            .RuleFor(response => response.Page, fake => 1)
            .Generate();

        var responses = new Queue<HttpResponseMessage>();

        var totalPages = Math.Ceiling((double)mockResponse.TotalRecords / mockResponse.PageSize);

        for (int i = 0; i <= totalPages; i++)
        {
            responses.Enqueue(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
            });

            mockResponse.Page += 1;
        }

        var mockHttpMessageHandler = GetMockHttpMessageHandler(responses);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var debtParams = new DebtParameters();

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetAllSecuredAdvances(agentIds[0], debtParams, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockResponse.Agents.Count * totalPages, result.Count);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly((int)totalPages),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetSecuredAdvances_IncludeFilters_ShouldSucceed(bool includeFilters)
    {
        // Arrange
        var agentId = "abc123";
        var debtParameters = new DebtParameters
        {
            IncludeFilters = includeFilters
        };

        var mockResponse = new AgentDetailsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var queryParams = GetQueryParams(agentId, debtParameters);

        var expectedUrl = new Uri(QueryHelpers.AddQueryString("http://localhost/v1/debt/securedAdvances", queryParams));

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetSecuredAdvances(agentId, debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetUnsecuredAdvances_IncludeFilters_ShouldSucceed(bool includeFilters)
    {
        // Arrange
        var agentId = "abc123";
        var debtParameters = new DebtParameters
        {
            IncludeFilters = includeFilters
        };

        var mockResponse = new AgentDetailsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);
        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var queryParams = GetQueryParams(agentId, debtParameters);

        var expectedUrl = new Uri(QueryHelpers.AddQueryString("http://localhost/v1/debt/unsecuredAdvances", queryParams));

        var mockLogger = new Mock<ILogger<DebtApiAccessor>>();

        var accessor = new DebtApiAccessor(mockhttpClient, mockLogger.Object);

        // Act
        var result = await accessor.GetUnsecuredAdvances(agentId, debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    private static Dictionary<string, string> GetQueryParams(string agentId, DebtParameters parameters)
    {
        var page = parameters?.Page > 0 ? parameters.Page : 1;
        var pageSize = parameters?.PageSize > 0 ? parameters.PageSize : 10;
        var queryParams = new Dictionary<string, string>
        {
            { "AgentId", agentId },
            { "Page", page.ToString() },
            { "PageSize", pageSize.ToString() },
        };

        if (parameters == null)
        {
            return queryParams;
        }

        if (parameters?.AgentId != null)
        {
            queryParams.Add("Filters.ViewAsAgentIds", parameters.AgentId);
        }

        if (parameters.PolicyNumber != null)
        {
            queryParams.Add("Filters.PolicyNumber", parameters.PolicyNumber);
        }

        if (parameters.WritingAgentIds != null)
        {
            queryParams.Add("Filters.AgentIds", string.Join(",", parameters.WritingAgentIds));
        }

        if (parameters.Status != null)
        {
            queryParams.Add("Filters.Status", parameters.Status);
        }

        if (parameters.OrderBy != null && parameters.SortDirection != null)
        {
            queryParams.Add("OrderBy", parameters.OrderBy.Value.ToString("G"));
            queryParams.Add("SortDirection", parameters.SortDirection.Value.ToString("G"));
        }

        if (parameters != null && parameters.IncludeFilters)
        {
            queryParams.Add("IncludeFilterValues", "true");
        }

        return queryParams;
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessage)
           .Verifiable();

        return mockHttpMessageHandler;
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(Queue<HttpResponseMessage> httpResponseMessageQueue)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessageQueue.Dequeue)
           .Verifiable();

        return mockHttpMessageHandler;
    }
}
