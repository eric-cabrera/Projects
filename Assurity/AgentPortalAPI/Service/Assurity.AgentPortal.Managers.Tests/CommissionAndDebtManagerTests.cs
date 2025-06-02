namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.CommissionsAndDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Managers.CommissionsAndDebt;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using Assurity.Commissions.Internal.Contracts.AgentStatementOptions;
using Assurity.Commissions.Internal.Contracts.Cycle;
using Assurity.Commissions.Internal.Contracts.PolicyDetails;
using Assurity.Commissions.Internal.Contracts.Summary;
using Assurity.Commissions.Internal.Contracts.WritingAgent;
using AutoBogus;
using AutoMapper;
using Moq;
using Xunit;
using AccessorDTO = Assurity.AgentPortal.Accessors.DTOs;
using Debt = Assurity.Commissions.Debt.Contracts.Advances;

[ExcludeFromCodeCoverage]
public class CommissionsAndDebtManagerTests
{
    private readonly Mapper mapper;

    public CommissionsAndDebtManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new CommissionDetailsMappingProfile(),
            new CommissionCycleMappingProfile(),
            new CommissionSummaryMappingProfile(),
            new DebtResponseMappingProfile(),
            new WritingAgentDetailsMappingProfile(),
            new CommissionParametersMappingProfile(),
            new DebtParametersMappingProfile(),
            new AgentStatementOptionsMappingProfile(),
            new AgentMappingProfile(),
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_Success()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        commissionsParams.ViewAsAgentId = null;
        var commissionParamDTO = mapper.Map<Accessors.DTOs.CommissionParameters>(commissionsParams);
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var policyDetailsResponse = new AutoFaker<PolicyDetailsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse)
        .Verifiable();
        mockAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(policyDetailsResponse)
        .Verifiable();
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(commissionSummaryResponse)
        .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData(agentIds[0], commissionsParams);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(policyDetailsResponse.PolicyDetails.First().AgentId, response.CommissionDetails.PolicyDetails.First().AgentId);
        Assert.Equal(policyDetailsResponse.PolicyDetails.First().EmployerId, response.CommissionDetails.PolicyDetails.First().EmployerId);
        Assert.Equal(cycleResponse.FirstYearCommission, response.CommissionCycle.FirstYear);
        Assert.Equal(cycleResponse.RenewalCommission, response.CommissionCycle.Renewal);
        Assert.Equal(cycleResponse.MyCommissions, response.CommissionCycle.MyCommissions);
        Assert.Equal(commissionsParams.CycleBeginDate, response.CommissionCycle.CycleStartDate);
        Assert.Equal(commissionsParams.CycleEndDate, response.CommissionCycle.CycleEndDate);
        Assert.Equal(commissionSummaryResponse.TaxableCommissionsTotal, response.CommissionSummary.TaxableCommissionsTotal);
        Assert.Equal(commissionSummaryResponse.RenewalCommissionsPreviousYear, response.CommissionSummary.RenewalCommissionsPreviousYear);
        Assert.Equal(commissionSummaryResponse.RenewalCommissionsCurrentYear, response.CommissionSummary.RenewalCommissionsCurrentYear);
        Assert.Equal(commissionSummaryResponse.FirstYearCommissionsCurrentYear, response.CommissionSummary.FirstYearCommissionsCurrentYear);
        Assert.Equal(commissionSummaryResponse.FirstYearCommissionsPreviousYear, response.CommissionSummary.FirstYearCommissionsPreviousYear);
        Assert.Equal(policyDetailsResponse.Filters.AgentFilterValues, response.CommissionDetails.Filters.ViewAsAgentIds);

        mockAccessor.Verify(
            accessor => accessor.GetCommissionsCycle(
            agentIds,
            commissionsParams.CycleBeginDate,
            commissionsParams.CycleEndDate,
            commissionsParams.WritingAgentIds,
            CancellationToken.None),
            Times.Once);

        mockAccessor.Verify(
            accessor => accessor.GetCommissionsSummary(
                agentIds,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_CycleEndDateIsFuture_EstimatedShouldBeTrue()
    {
        // Arrange
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        commissionsParams.CycleEndDate = DateTime.UtcNow.AddDays(1);
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var policyDetailsResponse = new AutoFaker<PolicyDetailsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse);
        mockAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(policyDetailsResponse);
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(commissionSummaryResponse);

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData("aaxb", commissionsParams);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.CommissionCycle.Estimated);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_CycleIsNull_ResponseShouldBeNull()
    {
        // Arrange
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        var policyDetailsResponse = new AutoFaker<PolicyDetailsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult<CycleCommissionsResponse>(null));
        mockAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(policyDetailsResponse);
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(commissionSummaryResponse);

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData("aaxb", commissionsParams);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_PolicyDetailsIsNull_ResponseShouldBeNull()
    {
        // Arrange
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse);
        mockAccessor.Setup<Task<PolicyDetailsResponse>>(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<Accessors.DTOs.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult<PolicyDetailsResponse>(null));
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(commissionSummaryResponse);
        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData("aaxb", commissionsParams);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_SummaryIsNull_ResponseShouldBeNull()
    {
        // Arrange
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        var policyDetailsResponse = new AutoFaker<PolicyDetailsResponse>().Generate();
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse);
        mockAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(policyDetailsResponse);
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult<SummaryCommissionsResponse>(null));
        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData("aaxb", commissionsParams);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetCommissionAndSummaryData_FilterOnAgentId_Success()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var commissionsParams = new AutoFaker<PolicyDetailsParameters>().Generate();
        commissionsParams.ViewAsAgentId = agentIds[0];
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var policyDetailsResponse = new AutoFaker<PolicyDetailsResponse>().Generate();
        var commissionSummaryResponse = new AutoFaker<SummaryCommissionsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse)
        .Verifiable();
        mockAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(policyDetailsResponse)
        .Verifiable();
        mockAccessor.Setup(accessor => accessor.GetCommissionsSummary(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(commissionSummaryResponse)
        .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);
        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionAndSummaryData(agentIds[0], commissionsParams);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(policyDetailsResponse.PolicyDetails.First().AgentId, response.CommissionDetails.PolicyDetails.First().AgentId);
        Assert.Equal(policyDetailsResponse.PolicyDetails.First().EmployerId, response.CommissionDetails.PolicyDetails.First().EmployerId);
        Assert.Equal(cycleResponse.FirstYearCommission, response.CommissionCycle.FirstYear);
        Assert.Equal(cycleResponse.RenewalCommission, response.CommissionCycle.Renewal);
        Assert.Equal(cycleResponse.MyCommissions, response.CommissionCycle.MyCommissions);
        Assert.Equal(commissionsParams.CycleBeginDate, response.CommissionCycle.CycleStartDate);
        Assert.Equal(commissionsParams.CycleEndDate, response.CommissionCycle.CycleEndDate);
        Assert.Equal(commissionSummaryResponse.TaxableCommissionsTotal, response.CommissionSummary.TaxableCommissionsTotal);
        Assert.Equal(commissionSummaryResponse.RenewalCommissionsPreviousYear, response.CommissionSummary.RenewalCommissionsPreviousYear);
        Assert.Equal(commissionSummaryResponse.RenewalCommissionsCurrentYear, response.CommissionSummary.RenewalCommissionsCurrentYear);
        Assert.Equal(commissionSummaryResponse.FirstYearCommissionsCurrentYear, response.CommissionSummary.FirstYearCommissionsCurrentYear);
        Assert.Equal(commissionSummaryResponse.FirstYearCommissionsPreviousYear, response.CommissionSummary.FirstYearCommissionsPreviousYear);

        var expectedAgentId = agentIds.Where(agentId => agentId.Equals("aaxb", StringComparison.OrdinalIgnoreCase)).ToList();

        mockAccessor.Verify(
            accessor => accessor.GetCommissionsCycle(
                expectedAgentId,
                commissionsParams.CycleBeginDate,
                commissionsParams.CycleEndDate,
                commissionsParams.WritingAgentIds,
                CancellationToken.None),
            Times.Once);

        mockAccessor.Verify<Task<PolicyDetailsResponse>>(
            accessor => accessor.GetPolicyDetails(
                expectedAgentId,
                It.IsAny<Accessors.DTOs.CommissionParameters>(),
                CancellationToken.None));

        mockAccessor.Verify(
            accessor => accessor.GetCommissionsSummary(
            expectedAgentId,
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetCommissionDateByWritingAgent_Success()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var commissionsParams = new AutoFaker<WritingAgentParameters>().Generate();
        commissionsParams.ViewAsAgentId = null;
        var cycleResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var writingAgentResponse = new AutoFaker<WritingAgentsResponse>().Generate();
        var mappedResponse = new AutoFaker<WritingAgentDetailsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleResponse)
        .Verifiable();

        mockAccessor.Setup(accessor => accessor.GetWritingAgentDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(writingAgentResponse)
        .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(agentIds);

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionDataByWritingAgent(agentIds[0], commissionsParams);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(writingAgentResponse.CyclePaidRenewalTotal, response.CyclePaidRenewalTotal);
        Assert.Equal(writingAgentResponse.CyclePaidFirstYearTotal, response.CyclePaidFirstYearTotal);
        Assert.Equal(writingAgentResponse.YearToDateRenewalTotal, response.YearToDateRenewalTotal);
        Assert.Equal(writingAgentResponse.TotalRecords, response.TotalRecords);
    }

    [Fact]
    public async Task GetCommissionDateByWritingAgent_FilterByAgentId_Success()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var commissionsParams = new AutoFaker<WritingAgentParameters>().Generate();
        commissionsParams.ViewAsAgentId = agentIds[0];
        var writingAgentResponse = new AutoFaker<WritingAgentsResponse>().Generate();
        var cycleCommissionsResponse = new AutoFaker<CycleCommissionsResponse>().Generate();
        var mappedResponse = new AutoFaker<WritingAgentDetailsResponse>().Generate();

        var mockAccessor = new Mock<ICommissionsApiAccessor>();
        mockAccessor.Setup(accessor => accessor.GetCommissionsCycle(
            It.IsAny<List<string>>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<DateTimeOffset>(),
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(cycleCommissionsResponse)
        .Verifiable();

        mockAccessor.Setup(accessor => accessor.GetWritingAgentDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(writingAgentResponse)
        .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(agentIds);

        var commissionsManager = new CommissionAndDebtManager(mapper, mockAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var response = await commissionsManager.GetCommissionDataByWritingAgent(agentIds[0], commissionsParams);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(writingAgentResponse.FilterValues.AgentFilterValues, response.Filters.ViewAsAgentIds);

        var expectedAgentId = agentIds.Where(agentId => agentId.Equals("aaxb", StringComparison.OrdinalIgnoreCase)).ToList();

        mockAccessor.Verify<Task<WritingAgentsResponse>>(
            accessor => accessor.GetWritingAgentDetails(
                expectedAgentId,
                It.IsAny<Accessors.DTOs.CommissionParameters>(),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetUnsecuredDebt_ShouldSucceed()
    {
        // Arrange
        var debtParameters = new UnsecuredAdvanceParameters();
        var accessorResponse = new AutoFaker<Debt.AgentDetailsResponse>().Generate();

        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(
            mockDebtApiAccessor => mockDebtApiAccessor.GetUnsecuredAdvances(
                It.IsAny<string>(),
                It.IsAny<AccessorDTO.DebtParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var manager = new CommissionAndDebtManager(mapper, null, mockDebtApiAccessor.Object, null, null);

        // Act
        var result = await manager.GetUnsecuredAdvances("aaxb", debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessorResponse.Agents.First().AgentId, result.Agents.First().AgentId);
        Assert.Equal(accessorResponse.Agents.First().BalanceOwed, result.Agents.First().BalanceOwed);
        Assert.Equal(accessorResponse.Agents.First().UnsecuredAdvanceOwed, result.Agents.First().UnsecuredAdvanceOwed);
        Assert.Equal(accessorResponse.Agents.First().ReversedCommissionOwed, result.Agents.First().ReversedCommissionOwed);
    }

    [Fact]
    public async Task GetUnsecuredDebt_FilteredOnAgentId_ShouldSucceed()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var debtParameters = new UnsecuredAdvanceParameters
        {
            AgentId = agentIds.Last()
        };
        var accessorResponse = new AutoFaker<Debt.AgentDetailsResponse>().Generate();

        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(
            mockDebtApiAccessor => mockDebtApiAccessor.GetUnsecuredAdvances(
                It.IsAny<string>(),
                It.IsAny<AccessorDTO.DebtParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);

        var manager = new CommissionAndDebtManager(mapper, null, mockDebtApiAccessor.Object, mockAgentApiAccessor.Object, null);

        // Act
        var result = await manager.GetUnsecuredAdvances(agentIds[0], debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessorResponse.Agents.First().AgentId, result.Agents.First().AgentId);
        Assert.Equal(accessorResponse.Agents.First().BalanceOwed, result.Agents.First().BalanceOwed);
        Assert.Equal(accessorResponse.Agents.First().UnsecuredAdvanceOwed, result.Agents.First().UnsecuredAdvanceOwed);
        Assert.Equal(accessorResponse.Agents.First().ReversedCommissionOwed, result.Agents.First().ReversedCommissionOwed);

        var expectedAgentId = agentIds.Where(agentId => agentId == agentIds.Last()).ToList();
    }

    [Fact]
    public async Task GetSecuredDebt_ShouldSucceed()
    {
        // Arrange
        var debtParameters = new SecuredAdvanceParameters();
        var accessorResponse = new AutoFaker<Debt.AgentDetailsResponse>().Generate();

        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(
            mockDebtApiAccessor => mockDebtApiAccessor.GetSecuredAdvances(
                It.IsAny<string>(),
                It.IsAny<AccessorDTO.DebtParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var manager = new CommissionAndDebtManager(mapper, null, mockDebtApiAccessor.Object, null, null);

        // Act
        var result = await manager.GetSecuredAdvances("aaxb", debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessorResponse.Agents.First().AgentId, result.Agents.First().AgentId);
        Assert.Equal(accessorResponse.Agents.First().BalanceOwed, result.Agents.First().BalanceOwed);
        Assert.Equal(accessorResponse.Agents.First().UnsecuredAdvanceOwed, result.Agents.First().UnsecuredAdvanceOwed);
        Assert.Equal(accessorResponse.Agents.First().ReversedCommissionOwed, result.Agents.First().ReversedCommissionOwed);
    }

    [Fact]
    public async Task GetSecuredDebt_FilteredOnAgentId_ShouldSucceed()
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };
        var debtParameters = new SecuredAdvanceParameters
        {
            AgentId = agentIds.Last()
        };
        var accessorResponse = new AutoFaker<Debt.AgentDetailsResponse>().Generate();

        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(
            mockDebtApiAccessor => mockDebtApiAccessor.GetSecuredAdvances(
                It.IsAny<string>(),
                It.IsAny<AccessorDTO.DebtParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessorResponse)
            .Verifiable();

        var manager = new CommissionAndDebtManager(mapper, null, mockDebtApiAccessor.Object, null, null);

        // Act
        var result = await manager.GetSecuredAdvances(agentIds[0], debtParameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessorResponse.Agents.First().AgentId, result.Agents.First().AgentId);
        Assert.Equal(accessorResponse.Agents.First().BalanceOwed, result.Agents.First().BalanceOwed);
        Assert.Equal(accessorResponse.Agents.First().UnsecuredAdvanceOwed, result.Agents.First().UnsecuredAdvanceOwed);
        Assert.Equal(accessorResponse.Agents.First().ReversedCommissionOwed, result.Agents.First().ReversedCommissionOwed);

        var expectedAgentId = agentIds.Where(agentId => agentId == agentIds.Last()).ToList();
    }

    [Fact]
    public async Task GetPolicyDetailsExcel_ShouldSucceed()
    {
        // Arrange
        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var mockCommmissionsApiAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommmissionsApiAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PolicyDetailsResponse());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<AgentPortal.Contracts.CommissionsDebt.PolicyDetail>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<AgentPortal.Contracts.CommissionsDebt.PolicyDetail>>(),
            It.IsAny<string>()))
            .Returns(Array.Empty<byte>());

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            mockCommmissionsApiAccessor.Object,
            null,
            mockAgentApiAccessor.Object,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetPolicyDetailsExcel("aaxb", new PolicyDetailsParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Matches("Commissions_PolicyDetails", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.NotNull(result.FileData);
        Assert.IsType<byte[]>(result.FileData);
    }

    [Fact]
    public async Task GetPolicyDetailsExcel_NullFileContents_ShouldReturnNullFileContents()
    {
        // Arrange
        var parameters = new PolicyDetailsParameters
        {
            CycleBeginDate = DateTime.Now,
        };

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var mockCommmissionsApiAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommmissionsApiAccessor.Setup(accessor => accessor.GetPolicyDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PolicyDetailsResponse());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<PolicyDetailsExport>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<PolicyDetailsExport>>(),
            It.IsAny<string>()))
            .Returns<byte[]>(null);

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            mockCommmissionsApiAccessor.Object,
            null,
            mockAgentApiAccessor.Object,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetPolicyDetailsExcel("aaxb", parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Commissions_PolicyDetails", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.Null(result.FileData);
    }

    [Fact]
    public async Task GetWritingAgentDetailsExcel_ShouldSucceed()
    {
        // Arrange
        var parameters = new WritingAgentParameters
        {
            CycleBeginDate = DateTime.Now,
        };
        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var mockCommmissionsApiAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommmissionsApiAccessor.Setup(accessor => accessor.GetWritingAgentDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WritingAgentsResponse());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<Contracts.CommissionsDebt.WritingAgentDetail>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<Contracts.CommissionsDebt.WritingAgentDetail>>(),
            It.IsAny<string>()))
            .Returns(Array.Empty<byte>());

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            mockCommmissionsApiAccessor.Object,
            null,
            mockAgentApiAccessor.Object,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetWritingAgentDetailsExcel("aaxb", parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Commissions_WritingAgentDetails", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.NotNull(result.FileData);
        Assert.IsType<byte[]>(result.FileData);
    }

    [Fact]
    public async Task GetWritingAgentDetailsExcel_NullFileContents_ShouldReturnNullFileContents()
    {
        // Arrange
        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<string>());

        var mockCommmissionsApiAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommmissionsApiAccessor.Setup(accessor => accessor.GetWritingAgentDetails(
            It.IsAny<List<string>>(),
            It.IsAny<AccessorDTO.CommissionParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WritingAgentsResponse());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<Contracts.CommissionsDebt.WritingAgentDetail>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<Contracts.CommissionsDebt.WritingAgentDetail>>(),
            It.IsAny<string>()))
            .Returns<byte[]>(null);

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            mockCommmissionsApiAccessor.Object,
            null,
            mockAgentApiAccessor.Object,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetWritingAgentDetailsExcel("aaxb", new WritingAgentParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Matches("Commissions_WritingAgentDetails", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.Null(result.FileData);
    }

    [Fact]
    public async Task GetUnsecuredAdvancesExcel_ShouldSucceed()
    {
        // Arrange
        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(accessor => accessor.GetAllUnsecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<AccessorDTO.DebtParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Debt.Agent>());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<UnsecuredDebtExport>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<UnsecuredDebtExport>>(),
            It.IsAny<string>()))
            .Returns(Array.Empty<byte>());

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            null,
            mockDebtApiAccessor.Object,
            null,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetUnsecuredAdvancesExcel("aaxb", new UnsecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UnsecuredAdvances_aaxb", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.NotNull(result.FileData);
        Assert.IsType<byte[]>(result.FileData);
    }

    [Fact]
    public async Task GetUnsecuredAdvancesExcel_NullFileContents_ShouldReturnNullFileContents()
    {
        // Arrange
        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(accessor => accessor.GetAllUnsecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<AccessorDTO.DebtParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Debt.Agent>());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<UnsecuredDebtExport>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<UnsecuredDebtExport>>(),
            It.IsAny<string>()))
            .Returns<byte[]>(null);

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            null,
            mockDebtApiAccessor.Object,
            null,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetUnsecuredAdvancesExcel("aaxb", new UnsecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Matches("UnsecuredAdvances_aaxb", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.Null(result.FileData);
    }

    [Fact]
    public async Task GetSecuredAdvancesExcel_ShouldSucceed()
    {
        // Arrange
        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(accessor => accessor.GetAllSecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<AccessorDTO.DebtParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Debt.Agent>());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<SecuredDebtExport>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<SecuredDebtExport>>(),
            It.IsAny<string>()))
            .Returns(Array.Empty<byte>());

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            null,
            mockDebtApiAccessor.Object,
            null,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetSecuredAdvancesExcel("aaxb", new SecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("SecuredAdvances_aaxb", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.NotNull(result.FileData);
        Assert.IsType<byte[]>(result.FileData);
    }

    [Fact]
    public async Task GetSecuredAdvancesExcel_NullFileContents_ShouldReturnNullFileContents()
    {
        // Arrange
        var mockDebtApiAccessor = new Mock<IDebtApiAccessor>();
        mockDebtApiAccessor.Setup(accessor => accessor.GetAllSecuredAdvances(
            It.IsAny<string>(),
            It.IsAny<AccessorDTO.DebtParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Debt.Agent>());

        var mockFileExportEngine = new Mock<IFileExportEngine>();
        mockFileExportEngine.Setup(engine => engine.CreateHeaders<SecuredDebtExport>()).Returns(new List<string>());
        mockFileExportEngine.Setup(engine => engine.CreateExcelDocument(
            It.IsAny<List<string>>(),
            It.IsAny<List<SecuredDebtExport>>(),
            It.IsAny<string>()))
            .Returns<byte[]>(null);

        var commissionAndDebtManager = new CommissionAndDebtManager(
            mapper,
            null,
            mockDebtApiAccessor.Object,
            null,
            mockFileExportEngine.Object);

        // Act
        var result = await commissionAndDebtManager.GetSecuredAdvancesExcel("aaxb", new SecuredAdvanceParameters(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Matches("SecuredAdvances_aaxb", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileType);
        Assert.Null(result.FileData);
    }

    [Theory]
    [InlineData(AgentStatementType.Summary)]
    [InlineData(AgentStatementType.RenewalDetail)]
    [InlineData(AgentStatementType.FirstYearDetail)]
    public async Task GetAgentStatement_NoAgentIdSpecified_ShouldSucceed(AgentStatementType agentStatementType)
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };

        var request = new AgentStatementRequest
        {
            CycleDate = DateTime.Now,
            AgentStatementType = agentStatementType,
        };

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);

        var mockCommissionsAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommissionsAccessor.Setup(accessor => accessor.GetAgentStatement(
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<AgentStatementType>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream())
            .Verifiable();

        var manager = new CommissionAndDebtManager(mapper, mockCommissionsAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var result = await manager.GetAgentStatement(
            "aaxb",
            "0",
            request.AgentId,
            request.CycleDate.Value,
            request.AgentStatementType.Value,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        mockCommissionsAccessor.Verify(
            accessor => accessor.GetAgentStatement(
                "aaxb",
                request.CycleDate.Value,
                agentStatementType,
                "0",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(AgentStatementType.Summary)]
    [InlineData(AgentStatementType.RenewalDetail)]
    [InlineData(AgentStatementType.FirstYearDetail)]
    public async Task GetAgentStatement_AgentIdSpecified_ShouldSucceed(AgentStatementType agentStatementType)
    {
        // Arrange
        var agentIds = new List<string>
        {
            "aaxb",
            "abc123",
            "321cba",
        };

        var request = new AgentStatementRequest
        {
            AgentId = agentIds[1],
            CycleDate = DateTime.Now,
            AgentStatementType = agentStatementType,
        };

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);

        var mockCommissionsAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommissionsAccessor.Setup(accessor => accessor.GetAgentStatement(
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<AgentStatementType>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream())
            .Verifiable();

        var manager = new CommissionAndDebtManager(mapper, mockCommissionsAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var result = await manager.GetAgentStatement(
            "aaxb",
            "0",
            request.AgentId,
            request.CycleDate.Value,
            request.AgentStatementType.Value,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        mockCommissionsAccessor.Verify(
            accessor => accessor.GetAgentStatement(
                agentIds[1],
                request.CycleDate.Value,
                agentStatementType,
                "0",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAgentStatementOptions_ShouldSucceed()
    {
        // Arrange
        var agentId = "aaxb";
        var agentIds = new List<string> { agentId };
        var agentStatementCycleDates = new AgentStatementOptionsResponse
        {
            Agents = new List<Assurity.Commissions.Internal.Contracts.Shared.Agent>
            {
                new()
                {
                    Name = "Test User",
                    Id = agentId
                }
            },
            CycleDates = new List<int>
            {
                20230101,
                20230115,
            }
        };

        var mockCommissionsAccessor = new Mock<ICommissionsApiAccessor>();
        mockCommissionsAccessor.Setup(accessor => accessor.GetAgentStatementOptions(
            It.IsAny<List<string>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentStatementCycleDates);

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentIds);

        var manager = new CommissionAndDebtManager(mapper, mockCommissionsAccessor.Object, null, mockAgentApiAccessor.Object, null);

        // Act
        var result = await manager.GetAgentStatementOptions(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20230101, result.CycleDates[0]);
        Assert.Equal(20230115, result.CycleDates[1]);
    }
}
