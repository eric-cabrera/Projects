namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoBogus;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

[ExcludeFromCodeCoverage]
public class PolicyInfoApiAccessorTests
{
    [Fact]
    public async Task GetPolicyInfoTest_InvalidAgentId_ReturnsBlankObject()
    {
        // Arrange
        Policy testPolicy = null;

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(JsonConvert.SerializeObject(testPolicy))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var policyNumber = "4750612114";
        var agentId = "1234";

        // Act
        var foundPolicy = await policyInfoApiAccessor.GetPolicyInfo(policyNumber, agentId);

        // Assert
        Assert.NotNull(foundPolicy);
    }

    [Fact]
    public async Task GetPolicyInfoTest_InvalidPolicyNumberAsync()
    {
        // Arrange
        Policy testPolicy = null;

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(JsonConvert.SerializeObject(testPolicy))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var policyNumber = "1234567890";
        var agentId = "7H80";

        // Act
        var foundPolicy = await policyInfoApiAccessor.GetPolicyInfo(policyNumber, agentId);

        // Assert
        Assert.NotNull(foundPolicy);
    }

    [Fact]
    public async Task GetPolicyInfoTest_404FromApiWithBlankResponseBody_ThrowsException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = null
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var policyNumber = "4750612114";
        var agentId = "1234";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await policyInfoApiAccessor.GetPolicyInfo(policyNumber, agentId));
    }

    [Fact]
    public async Task GetPolicyInfoTest_ValidInputsAsync()
    {
        // Arrange
        var testPolicy = new Policy
        {
            Agents = new List<Agent>
            {
                new Agent
                {
                    AgentId = "7H80",
                    IsServicingAgent = false,
                    Level = string.Empty,
                    MarketCode = string.Empty
                }
            },
            Id = string.Empty,
            AnnualPremium = new decimal(264.63),
            Benefits = new List<Benefit>(),
            BillingDay = 0,
            BillingForm = BillingForm.Unknown,
            BillingMode = 0,
            CompanyCode = string.Empty,
            Insureds = new List<Insured>(),
            IssueDate = default,
            IssueState = 0,
            ModePremium = new decimal(66.16),
            Owners = new List<Owner>(),
            PaidToDate = default,
            Payors = new List<Payor>(),
            PolicyNumber = "4750612114",
            PolicyStatus = Status.Pending,
            PolicyStatusReason = null,
            ProductCategory = string.Empty,
            ProductCode = string.Empty,
            ProductDescription = string.Empty,
            ResidentState = 0,
            SubmitDate = default,
            CreateDate = default,
            LastModified = default
        };

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(testPolicy))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act
        var result = await policyInfoApiAccessor.GetPolicyInfo(testPolicy.PolicyNumber, testPolicy.Agents[0].AgentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(testPolicy, result, true);
    }

    [Fact]
    public async Task GetPolicyStatusCountsByAgentIdTest_InvalidAgentId_ReturnsEmptyObject()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Couldn't find agent")
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "1234";

        // Act
        var policyStatusCounts = await policyInfoApiAccessor.GetPolicyStatusCounts(agentId);

        // Assert
        Assert.NotNull(policyStatusCounts);
    }

    [Fact]
    public async Task GetPolicyStatusCountsByAgentIdTest_404FromApiWithoutResponseBody_ThrowsException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = null
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "1234";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await policyInfoApiAccessor.GetPolicyStatusCounts(agentId));
    }

    [Fact]
    public async Task GetPolicyStatusCountsByAgentIdTest_ValidAgentIdAsync()
    {
        // Arrange
        var testPolicyStatusCounts = GetFakePolicyStatusCountsResponse();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(testPolicyStatusCounts))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "7H80";

        // Act
        var result = await policyInfoApiAccessor.GetPolicyStatusCounts(agentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testPolicyStatusCounts.PendingPolicies.UnderwritingRequirements, result.PendingPolicies.UnderwritingRequirements);
        Assert.Equal(testPolicyStatusCounts.ActivePolicies.TotalPolicies, result.ActivePolicies.TotalPolicies);
        Assert.Equal(testPolicyStatusCounts.TerminatedPolicies.DeclinedInitialPremiumPayments, result.TerminatedPolicies.DeclinedInitialPremiumPayments);
    }

    [Fact]
    public async Task GetPolicySummariesByAgentIdTest_InvalidAgentIdAsync()
    {
        // Arrange
        PolicySummary testPolicySummaries = null;

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new List<PolicySummary>
                    {
                        testPolicySummaries
                    }))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "1234";
        var status = Status.Terminated;

        // Act
        var foundPolicySummaries = await policyInfoApiAccessor
            .GetPolicySummaries(agentId, status, string.Empty);

        Assert.NotNull(foundPolicySummaries);
        Assert.Null(foundPolicySummaries.Policies);
    }

    [Fact]
    public async Task GetPolicySummariesByAgentIdTest_InvalidPolicyStatusAsync()
    {
        // Arrange
        PolicySummary testPolicySummaries = null;

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new List<PolicySummary>
                    {
                        testPolicySummaries
                    }))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "7H80";
        var status = Status.Terminated;

        // Act
        var policiesSummaries = await policyInfoApiAccessor
            .GetPolicySummaries(agentId, status, string.Empty);

        Assert.NotNull(policiesSummaries);
        Assert.Null(policiesSummaries.Policies);
    }

    [Fact]
    public async Task GetPolicySummariesByAgentIdTest_ValidAgentIdAndPolicyStatusAsync()
    {
        // Arrange
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var testPolicySummaries = policySummaryFaker.Generate();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new PolicyInfoAPI.PolicySummariesResponse
                    {
                        Policies = new List<PolicyInfoAPI.PolicySummary>
                        {
                            testPolicySummaries
                        }
                    }))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act
        var result = await policyInfoApiAccessor
            .GetPolicySummaries(testPolicySummaries.AssignedAgents[0].AgentId, testPolicySummaries.PolicyStatus, string.Empty);

        // Assert
        Assert.Single(result.Policies);
        Assert.Equal(testPolicySummaries.PolicyNumber, result.Policies.First().PolicyNumber);
        Assert.Equal(testPolicySummaries.AssignedAgents[0].AgentId, result.Policies.First().AssignedAgents[0].AgentId);
        Assert.Equal(testPolicySummaries.FaceAmount, result.Policies.First().FaceAmount);
        Assert.Equal(testPolicySummaries.IssueDate, result.Policies.First().IssueDate);
        Assert.Equal(testPolicySummaries.PaidToDate, result.Policies.First().PaidToDate);
        Assert.Equal(testPolicySummaries.PolicyStatus, result.Policies.First().PolicyStatus);
        Assert.Equal(testPolicySummaries.PolicyStatusReason, result.Policies.First().PolicyStatusReason);
        Assert.Equal(
            testPolicySummaries.PrimaryInsured.Participant.Person.Name.IndividualLast,
            result.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualLast);
        Assert.Equal(testPolicySummaries.ProductCategory, result.Policies.First().ProductCategory);
        Assert.Equal(testPolicySummaries.ProductCode, result.Policies.First().ProductCode);
        Assert.Equal(testPolicySummaries.ProductDescription, result.Policies.First().ProductDescription);
        Assert.Equal(testPolicySummaries.SubmitDate, result.Policies.First().SubmitDate);
    }

    [Fact]
    public async Task GetPendingPolicyRequirementSummaries_InvalidAgentId()
    {
        // Arrange
        PolicySummary testPolicySummaries = null;

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Couldn't find your agent")
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);
        var agentId = "1234";

        // Act
        var requirementSummaries = await policyInfoApiAccessor
            .GetPendingPolicyRequirementSummaries(agentId, string.Empty);

        Assert.NotNull(requirementSummaries);
        Assert.Null(requirementSummaries.RequirementSummaries);
    }

    [Fact]
    public async Task GetPendingPolicyRequirementSummaries_ValidAgentId()
    {
        // Arrange
        var requirementSummaryResponseFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var testRequirementSummaries = requirementSummaryResponseFaker.Generate();

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonConvert.SerializeObject(testRequirementSummaries))
        };

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act
        var result = await policyInfoApiAccessor
            .GetPendingPolicyRequirementSummaries(
            testRequirementSummaries.RequirementSummaries[0].AssignedAgents[0].AgentId,
            string.Empty);

        // Assert
        Assert.Equal(testRequirementSummaries.RequirementSummaries.Count, result.RequirementSummaries.Count);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().PolicyNumber,
            result.RequirementSummaries.First().PolicyNumber);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().AssignedAgents[0].AgentId,
            result.RequirementSummaries.First().AssignedAgents[0].AgentId);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualLast,
            result.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualLast);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().ProductCategory,
            result.RequirementSummaries.First().ProductCategory);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().EmployerName,
            result.RequirementSummaries.First().EmployerName);
        Assert.Equal(
            testRequirementSummaries.RequirementSummaries.First().Requirement.Id,
            result.RequirementSummaries.First().Requirement.Id);
    }

    [Fact]
    public async Task CheckAgentAccessToPolicy_OkResponse_ShouldReturnTrue()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
        };

        var agentId = "1234";
        var policyNumber = "A123456789";

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act
        var result = await policyInfoApiAccessor
            .CheckAgentAccessToPolicy(agentId, policyNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckAgentAccessToPolicy_NotFoundResponseWithResponseBody_ShouldReturnFalse()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Couldn't find your agent")
        };

        var agentId = "1234";
        var policyNumber = "A123456789";

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act
        var result = await policyInfoApiAccessor
            .CheckAgentAccessToPolicy(agentId, policyNumber);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckAgentAccessToPolicy_NotFoundResponseWithoutResponseBody_ThrowsException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = null
        };

        var agentId = "1234";
        var policyNumber = "A123456789";

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await policyInfoApiAccessor.CheckAgentAccessToPolicy(agentId, policyNumber));
    }

    [Fact]
    public async Task CheckAgentAccessToPolicy_ForbiddenResponse_ThrowsException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Forbidden,
        };

        var agentId = "1234";
        var policyNumber = "A123456789";

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await policyInfoApiAccessor.CheckAgentAccessToPolicy(agentId, policyNumber));
    }

    [Fact]
    public async Task CheckAgentAccessToPolicy_InternalServerErrorResponse_ThrowsException()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
        };

        var agentId = "1234";
        var policyNumber = "A123456789";

        var policyInfoApiAccessor = GetPolicyInfoApiAccessor(httpResponseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await policyInfoApiAccessor.CheckAgentAccessToPolicy(agentId, policyNumber));
    }

    private static PolicyInfoApiAccessor GetPolicyInfoApiAccessor(HttpResponseMessage httpResponseMessage)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponseMessage);

        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockhttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var mockLogger = new Mock<ILogger<PolicyInfoApiAccessor>>();

        return new PolicyInfoApiAccessor(mockhttpClient, mockLogger.Object);
    }

    private PolicyInfoAPI.PolicyStatusCounts GetFakePolicyStatusCountsResponse()
    {
        var fakePending = new Faker<PolicyInfoAPI.PendingStatusCounts>()
            .RuleFor(x => x.UnderwritingRequirements, f => f.Random.Number(0, 100))
            .RuleFor(x => x.PendingPremium, f => Math.Round(f.Random.Decimal(0, 100000), 2));
        var fakeActive = new Faker<PolicyInfoAPI.ActiveStatusCounts>()
            .RuleFor(x => x.TotalPolicies, f => f.Random.Number(0, 100))
            .RuleFor(x => x.AnnualizedPremium, f => Math.Round(f.Random.Decimal(0, 100000), 2))
            .RuleFor(x => x.PastDuePremium, f => Math.Round(f.Random.Decimal(0, 100000), 2));
        var fakeTerminated = new Faker<PolicyInfoAPI.TerminatedStatusCounts>()
            .RuleFor(x => x.DeclinedInitialPremiumPayments, f => f.Random.Number(0, 100));

        var fakePolicyStatusCounts = new Faker<PolicyInfoAPI.PolicyStatusCounts>()
            .RuleFor(x => x.PendingPolicies, fakePending.Generate())
            .RuleFor(x => x.ActivePolicies, fakeActive.Generate())
            .RuleFor(x => x.TerminatedPolicies, fakeTerminated.Generate());

        return fakePolicyStatusCounts.Generate();
    }
}