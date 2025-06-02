namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using Assurity.AgentPortal.Accessors.GroupInventory;
using Assurity.Groups.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

[ExcludeFromCodeCoverage]
public class GroupInventoryApiAccessorTests
{
    private readonly Uri baseAddress = new Uri("http://localhost");

    [Fact]
    public async Task GetGroupSummary_Success()
    {
        // Arrange
        var agentNumber = "12345";
        var queryParams = new GroupSummaryQueryParameters
        {
            GroupNumber = "1001",
            GroupName = "Group A",
            GroupEffectiveDateStartDate = new DateTime(2022, 01, 01),
            GroupEffectiveDateEndDate = new DateTime(2022, 12, 31),
            GroupStatus = "Active",
            OrderBy = SummaryOrderBy.GroupStatus,
            SortDirection = SortDirection.Desc,
            Page = 1,
            PageSize = 10
        };

        var mockGroupSummaryResponse = new GroupSummaryResponse
        {
            GroupSummaries = new List<GroupSummary>
                {
                    new GroupSummary
                    {
                        GroupNumber = "1001",
                        GroupName = "Group A",
                        GroupStatus = "Active",
                        PolicyCount = 5,
                        GroupEffectiveDate = new DateTime(2022, 01, 01)
                    }
                },
            Filters = new GroupSummaryFilters()
        };

        var jsonResponse = JsonConvert.SerializeObject(mockGroupSummaryResponse);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var groupsApiAccessor = GetGroupsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await groupsApiAccessor.GetGroupSummary(agentNumber, queryParams, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var group = result.GroupSummaries[0];
        Assert.Equal("1001", group.GroupNumber);
        Assert.Equal("Group A", group.GroupName);
        Assert.Equal("Active", group.GroupStatus);
        Assert.Equal(new DateTime(2022, 01, 01), group.GroupEffectiveDate);
        Assert.Equal(5, group.PolicyCount);
    }

    [Fact]
    public async Task GetGroupSummary_Failure_ReturnsEmptyGroupSummaryResponse_WhenNotFound()
    {
        // Arrange
        var agentNumber = "12345";
        var queryParams = new GroupSummaryQueryParameters
        {
            GroupNumber = "1001",
            GroupName = "Group A",
            GroupEffectiveDateStartDate = new DateTime(2022, 01, 01),
            GroupEffectiveDateEndDate = new DateTime(2022, 12, 31),
            GroupStatus = "Active",
            OrderBy = SummaryOrderBy.GroupStatus,
            SortDirection = SortDirection.Desc,
            Page = 1,
            PageSize = 10
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound, // Simulate 404 Not Found
                Content = new StringContent("Not Found")
            });

        var groupsApiAccessor = GetGroupsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await groupsApiAccessor.GetGroupSummary(agentNumber, queryParams, CancellationToken.None);

        // Assert
        Assert.IsType<GroupSummaryResponse>(result);
    }

    [Fact]
    public async Task GetGroupDetail_Success()
    {
        // Arrange
        var agentNumber = "12345";
        var groupNumber = "1001";
        var queryParams = new GroupDetailsQueryParameters
        {
            PolicyNumber = "P12345",
            ProductDescription = "Life Insurance",
            PolicyOwnerName = "John Doe",
            IssueDateStartDate = new DateTime(2020, 01, 01),
            IssueDateEndDate = new DateTime(2022, 12, 31),
            PolicyStatus = "Active",
            SortDirection = SortDirection.Desc,
            Page = 1,
            PageSize = 10
        };

        var mockGroupDetailResponse = new GroupDetailResponse
        {
            Group = new Group
            {
                Name = "Group A",
                Number = "1001",
                Status = "Active",
                EffectiveDate = new DateTime(2020, 01, 01),
                ActivePolicyCount = 5
            },
            Policies = new List<Assurity.Groups.Contracts.Policy>
        {
            new Assurity.Groups.Contracts.Policy
            {
                Number = "P12345",
                Status = "Active",
                IssueDate = new DateTime(2021, 06, 15),
                AnnualPremium = 500.00m,
                ModePremium = 50.00m,
                ProductDescription = "Life Insurance"
            }
        },
            Filters = new GroupDetailFilters()
        };

        var jsonResponse = JsonConvert.SerializeObject(mockGroupDetailResponse);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var groupsApiAccessor = GetGroupsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await groupsApiAccessor.GetGroupDetail(agentNumber, groupNumber, queryParams, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1001", result.Group.Number);
        Assert.Equal("Group A", result.Group.Name);
        Assert.Equal("Active", result.Group.Status);
        Assert.Equal(new DateTime(2020, 01, 01), result.Group.EffectiveDate);
        Assert.Equal(5, result.Group.ActivePolicyCount);
        Assert.Single(result.Policies);
        var policy = result.Policies[0];
        Assert.Equal("P12345", policy.Number);
        Assert.Equal("Active", policy.Status);
        Assert.Equal(new DateTime(2021, 06, 15), policy.IssueDate);
        Assert.Equal(500.00m, policy.AnnualPremium);
    }

    [Fact]
    public async Task GetGroupDetail_Failure_ReturnsEmptyObject_NotFound()
    {
        // Arrange
        var agentNumber = "12345";
        var groupNumber = "1001";
        var queryParams = new GroupDetailsQueryParameters
        {
            PolicyNumber = "P12345",
            ProductDescription = "Life Insurance",
            PolicyOwnerName = "John Doe",
            IssueDateStartDate = new DateTime(2020, 01, 01),
            IssueDateEndDate = new DateTime(2022, 12, 31),
            PolicyStatus = "Active",
            SortDirection = SortDirection.Desc,
            Page = 1,
            PageSize = 10
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            });

        var groupsApiAccessor = GetGroupsApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await groupsApiAccessor.GetGroupDetail(agentNumber, groupNumber, queryParams, CancellationToken.None);

        // Assert
        Assert.IsType<GroupDetailResponse>(result);
    }

    private GroupInventoryApiAccessor GetGroupsApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };

        return new GroupInventoryApiAccessor(httpClient, new Mock<ILogger<GroupInventoryApiAccessor>>().Object);
    }
}
