namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using Assurity.AgentPortal.Accessors.CommissionsAndDebt;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.Commissions.Internal.Contracts.Cycle;
using Assurity.Commissions.Internal.Contracts.PolicyDetails;
using Assurity.Commissions.Internal.Contracts.Summary;
using Assurity.Commissions.Internal.Contracts.WritingAgent;
using AutoBogus;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class CommissionsApiAccessorTests
{
    [Fact]
    public async Task GetCommissionsCycle_NoAdditionalFilters_Success()
    {
        var mockResponse = new CycleCommissionsResponse
        {
            FirstYearCommission = 1.0m,
            MyCommissions = 1.0m,
            RenewalCommission = 1.0m
        };

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var result = await commissionsAccessor.GetCommissionsCycle(agentIds, cycleBeginDate, null, null);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") }
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/cycle", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetCommissionsCycle_CycleEndDateFilter_Success()
    {
        var mockResponse = new CycleCommissionsResponse
        {
            FirstYearCommission = 1.0m,
            MyCommissions = 1.0m,
            RenewalCommission = 1.0m
        };

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var cycleEndDate = DateTimeOffset.Now.AddDays(-1);

        var result = await commissionsAccessor.GetCommissionsCycle(agentIds, cycleBeginDate, cycleEndDate, null);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "CycleDateEnd", cycleEndDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/cycle", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetCommissionsCycle_CycleEndDateEqualsCycleBeginDate_Success()
    {
        var mockResponse = new CycleCommissionsResponse
        {
            FirstYearCommission = 1.0m,
            MyCommissions = 1.0m,
            RenewalCommission = 1.0m
        };

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var cycleEndDate = cycleBeginDate;

        var result = await commissionsAccessor.GetCommissionsCycle(agentIds, cycleBeginDate, cycleEndDate, null);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/cycle", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetCommissionsCycle_WritingAgentIdFilter_Success()
    {
        var mockResponse = new CycleCommissionsResponse
        {
            FirstYearCommission = 1.0m,
            MyCommissions = 1.0m,
            RenewalCommission = 1.0m
        };

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var cycleEndDate = DateTimeOffset.Now.AddDays(-1);
        var writingAgentIds = new List<string>
        {
            "howdy",
            "partner",
        };

        var result = await commissionsAccessor.GetCommissionsCycle(agentIds, cycleBeginDate, cycleEndDate, writingAgentIds);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "CycleDateEnd", cycleEndDate.ToString("yyyyMMdd") },
            { "WritingAgentIds", string.Join(",", writingAgentIds) }
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/cycle", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetCommissionsCycle_UnsuccessfulStatusCode()
    {
        var mockLogger = new Mock<ILogger<CommissionsApiAccessor>>();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, mockLogger.Object);

        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);

        var result = await commissionsAccessor.GetCommissionsCycle(agentIds, cycleBeginDate, null, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPolicyDetails_NoAdditionalFilters_Success()
    {
        var mockResponse = new AutoFaker<PolicyDetailsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var page = 5;
        var pageSize = 50;

        var filterParams = new CommissionParameters
        {
            CycleBeginDate = cycleBeginDate,
            Page = page,
            PageSize = pageSize,
        };

        var result = await commissionsAccessor.GetPolicyDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", filterParams.Page.ToString() },
            { "PageSize", filterParams.PageSize.ToString() },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/policydetails", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetPolicyDetails_CycleEndDateFilter_Success()
    {
        var mockResponse = new AutoFaker<PolicyDetailsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleEndDate = DateTimeOffset.Now.AddDays(-1);

        var filterParams = new CommissionParameters
        {
            Page = 1,
            PageSize = 10,
            CycleBeginDate = cycleBeginDate,
            CycleEndDate = cycleEndDate,
        };

        var result = await commissionsAccessor.GetPolicyDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", "1" },
            { "PageSize", "10" },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "CycleDateEnd", cycleEndDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/policydetails", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetPolicyDetails_WritingAgentsFilter_Success()
    {
        var mockResponse = new AutoFaker<PolicyDetailsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var writingAgentIds = new List<string>
        {
            "howdy",
            "there",
        };

        var filterParams = new CommissionParameters
        {
            Page = 1,
            PageSize = 10,
            CycleBeginDate = cycleBeginDate,
            WritingAgentIds = writingAgentIds,
        };

        var result = await commissionsAccessor.GetPolicyDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", "1" },
            { "PageSize", "10" },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "WritingAgentIds", string.Join(",", writingAgentIds) },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/policydetails", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetPolicyDetails_PolicyNumberFilter_Success()
    {
        // Arrange
        var mockResponse = new AutoFaker<PolicyDetailsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var policyNumber = "abcdef1234";

        var filterParams = new CommissionParameters
        {
            Page = 1,
            PageSize = 10,
            CycleBeginDate = cycleBeginDate,
            PolicyNumber = policyNumber,
        };

        // Act
        var result = await commissionsAccessor.GetPolicyDetails(agentIds, filterParams);

        // Assert
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", "1" },
            { "PageSize", "10" },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "PolicyNumber", policyNumber },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/policydetails", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData("AgentId", SortDirection.ASC)]
    [InlineData("AgentId", SortDirection.DESC)]
    [InlineData("Commission", SortDirection.ASC)]
    [InlineData("Commission", SortDirection.DESC)]
    [InlineData("PrimaryInsured", SortDirection.ASC)]
    [InlineData("PrimaryInsured", SortDirection.DESC)]
    [InlineData("PaymentDate", SortDirection.ASC)]
    [InlineData("PaymentDate", SortDirection.DESC)]
    [InlineData("WritingAgentId", SortDirection.ASC)]
    [InlineData("WritingAgentId", SortDirection.DESC)]
    [InlineData("WritingAgent", SortDirection.ASC)]
    [InlineData("WritingAgent", SortDirection.DESC)]
    public async Task GetPolicyDetails_OrderBy_Success(string orderBy, SortDirection sortDirection)
    {
        // Arrange
        var mockResponse = new AutoFaker<PolicyDetailsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var filterParams = new CommissionParameters
        {
            Page = 1,
            PageSize = 10,
            CycleBeginDate = cycleBeginDate,
            OrderBy = orderBy,
            SortDirection = sortDirection,
        };

        // Act
        var result = await commissionsAccessor.GetPolicyDetails(agentIds, filterParams);

        // Assert
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", "1" },
            { "PageSize", "10" },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "OrderBy", orderBy },
            { "SortDirection", sortDirection.ToString("G") }
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/policydetails", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWritingAgentDetails_NoFilters_Success()
    {
        // Arrange
        var mockResponse = new AutoFaker<WritingAgentsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var filterParams = new CommissionParameters
        {
            CycleBeginDate = cycleBeginDate,
        };

        // Act
        var result = await commissionsAccessor.GetWritingAgentDetails(agentIds, filterParams);

        // Assert
        var queryParams = new Dictionary<string, string>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/writingagent", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWritingAgentDetails_CycleEndDateFilter_Success()
    {
        var mockResponse = new AutoFaker<WritingAgentsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var cycleEndDate = DateTimeOffset.Now.AddDays(-1);

        var filterParams = new CommissionParameters
        {
            Page = 1,
            PageSize = 10,
            CycleBeginDate = cycleBeginDate,
            CycleEndDate = cycleEndDate,
        };

        var result = await commissionsAccessor.GetWritingAgentDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "Page", "1" },
            { "PageSize", "10" },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "CycleDateEnd", cycleEndDate.ToString("yyyyMMdd") },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/writingagent", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWritingAgentDetails_WritingAgentsFilter_Success()
    {
        var mockResponse = new AutoFaker<WritingAgentsResponse>().Generate();

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

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };
        var writingAgentIds = new List<string>
        {
            "howdy",
            "there",
        };

        var filterParams = new CommissionParameters
        {
            CycleBeginDate = cycleBeginDate,
            WritingAgentIds = writingAgentIds,
        };

        var result = await commissionsAccessor.GetWritingAgentDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "WritingAgentIds", string.Join(",", writingAgentIds) },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/writingagent", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData("AgentId", SortDirection.ASC)]
    [InlineData("AgentId", SortDirection.DESC)]
    [InlineData("YtdPaidRenewal", SortDirection.ASC)]
    [InlineData("YtdPaidRenewal", SortDirection.DESC)]
    [InlineData("CyclePaidRenewal", SortDirection.ASC)]
    [InlineData("CyclePaidRenewal", SortDirection.DESC)]
    [InlineData("YtdPaidFirstYear", SortDirection.ASC)]
    [InlineData("YtdPaidFirstYear", SortDirection.DESC)]
    [InlineData("CyclePaidFirstYear", SortDirection.ASC)]
    [InlineData("CyclePaidFirstYear", SortDirection.DESC)]
    [InlineData("WritingAgent", SortDirection.ASC)]
    [InlineData("WritingAgent", SortDirection.DESC)]
    public async Task GetWritingAgentDetails_OrderBy_Success(string orderBy, SortDirection sortDirection)
    {
        var mockResponse = new AutoFaker<WritingAgentsResponse>().Generate();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(mockResponse)),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var commissionsAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        var cycleBeginDate = DateTimeOffset.Now.AddDays(-7);
        var agentIds = new List<string>
        {
            "abc123",
            "123abc",
        };

        var filterParams = new CommissionParameters
        {
            CycleBeginDate = cycleBeginDate,
            OrderBy = orderBy,
            SortDirection = sortDirection,
        };

        var result = await commissionsAccessor.GetWritingAgentDetails(agentIds, filterParams);

        var queryParams = new Dictionary<string, string>
        {
            { "AgentIds", string.Join(",", agentIds) },
            { "CycleDateBegin", cycleBeginDate.ToString("yyyyMMdd") },
            { "OrderBy", orderBy },
            { "SortDirection", sortDirection.ToString("G") }
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/writingagent", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetCommissionSummary_Success()
    {
        // Arrange
        var agents = new List<string>
        {
            "aaxb",
            "056j",
        };
        var mockResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

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

        var commissionAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        // Act
        var response = await commissionAccessor.GetCommissionsSummary(agents);

        // Assert
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agents) },
        };

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(mockhttpClient.BaseAddress + "v1/commissions/summary", queryParams));
        Assert.NotNull(response);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(AgentStatementType.Summary)]
    [InlineData(AgentStatementType.FirstYearDetail)]
    [InlineData(AgentStatementType.RenewalDetail)]
    public async Task GetAgentStatement_Success(AgentStatementType agentStatementType)
    {
        // Arrange
        var cycleDate = DateTime.Now;
        var agentId = "aaxb";
        var sessionId = "100";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Howdy There!"));
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(stream),
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(httpResponseMessage);

        var mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var commissionAccessor = new CommissionsApiAccessor(mockhttpClient, null);

        // Act
        var result = await commissionAccessor.GetAgentStatement(agentId, cycleDate, agentStatementType, sessionId);

        // Assert
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentId", agentId },
            { "CycleDate", cycleDate.ToString() },
            { "SessionId", sessionId },
        };

        switch (agentStatementType)
        {
            case AgentStatementType.Summary:
                queryParams.Add("ReportType", "Summary");
                break;
            case AgentStatementType.FirstYearDetail:
                queryParams.Add("ReportType", "Details");
                queryParams.Add("CommissionType", "FirstYear");
                break;
            case AgentStatementType.RenewalDetail:
                queryParams.Add("ReportType", "Details");
                queryParams.Add("CommissionType", "Renewal");
                break;
        }

        var expectedUrl = new Uri(QueryHelpers.AddQueryString(
            mockhttpClient.BaseAddress + "v1/commissions/agentstatementsummary", queryParams));

        Assert.NotNull(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUrl),
            ItExpr.IsAny<CancellationToken>());
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
