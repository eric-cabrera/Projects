namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
using AutoBogus;
using AutoMapper;
using Xunit;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

[ExcludeFromCodeCoverage]
public class SummaryMappingProfileTests
{
    public SummaryMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<AddressMappingProfile>();
            config.AddProfile<BusinessMappingProfile>();
            config.AddProfile<NameMappingProfile>();
            config.AddProfile<ParticipantMappingProfile>();
            config.AddProfile<PersonMappingProfile>();
            config.AddProfile<RequirementMappingProfile>();
            config.AddProfile<SummaryMappingProfile>();
            config.AddProfile<PolicyDetailsExportMappingProfile>();
        }).CreateMapper();
    }

    private IMapper Mapper { get; }

    [Fact]
    public void AssertConfigurationIsValid()
    {
        // Assert
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void PolicySummaryResponse_IndividualNames_ShouldMap()
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummariesResponse>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.Policies.First().AssignedAgents[0].IsServicingAgent = true;
        policySummary.Policies.First().AssignedAgents[0].Participant.IsBusiness = false;
        policySummary.Policies.First().PrimaryInsured.Participant.IsBusiness = false;

        // Act
        var mappedResponse = Mapper.Map<PolicySummariesResponse>(policySummary);

        // Assert
        var agentName =
            policySummary.Policies.First().AssignedAgents[0].Participant.Person.Name.IndividualLast + ", " +
            policySummary.Policies.First().AssignedAgents[0].Participant.Person.Name.IndividualFirst;
        var primaryInsuredName =
            policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualLast + ", " +
            policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualFirst;

        Assert.Equal(agentName, mappedResponse.Policies.First().AssignedAgents[0].AssignedAgentName);
        Assert.True(mappedResponse.Policies.First().AssignedAgents[0].IsServicingAgent);
        Assert.Equal(primaryInsuredName, mappedResponse.Policies.First().PrimaryInsuredName);
        Assert.Equal(
            policySummary.Policies.First().EmployerName,
            mappedResponse.Policies.First().EmployerName);
        Assert.Equal(policySummary.Filters.ReasonFilterValues, mappedResponse.Filters.ReasonFilterValues);
        Assert.Equal(policySummary.Filters.AgentLastNameFilterValues, mappedResponse.Filters.AgentLastNameFilterValues);
        Assert.Equal(policySummary.Filters.PrimaryInsuredFilterValues, mappedResponse.Filters.PrimaryInsuredFilterValues);
        Assert.Equal(policySummary.Filters.PolicyNumberFilterValues, mappedResponse.Filters.PolicyNumberFilterValues);
        Assert.Equal(policySummary.Filters.AgentIdFilterValues, mappedResponse.Filters.AgentIdFilterValues);
        Assert.Equal(policySummary.Filters.EmployerFilterValues, mappedResponse.Filters.EmployerFilterValues);
        Assert.Equal(policySummary.Filters.ViewAsFilterValues, mappedResponse.Filters.ViewAsFilterValues);
    }

    [Fact]
    public void PolicySummaryResponse_BusinessName_ShouldMap()
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummariesResponse>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.Policies.First().AssignedAgents[0].IsServicingAgent = true;
        policySummary.Policies.First().AssignedAgents[0].Participant.IsBusiness = true;
        policySummary.Policies.First().PrimaryInsured.Participant.IsBusiness = false;

        // Act
        var mappedResponse = Mapper.Map<PolicySummariesResponse>(policySummary);

        // Assert
        var agentName =
            policySummary.Policies.First().AssignedAgents[0].Participant.Business.Name.BusinessName;
        var primaryInsuredName =
            policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualLast + ", " +
            policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualFirst;

        Assert.Equal(agentName, mappedResponse.Policies.First().AssignedAgents[0].AssignedAgentName);
        Assert.True(mappedResponse.Policies.First().AssignedAgents[0].IsServicingAgent);
        Assert.Equal(primaryInsuredName, mappedResponse.Policies.First().PrimaryInsuredName);
        Assert.Equal(
            policySummary.Policies.First().EmployerName,
            mappedResponse.Policies.First().EmployerName);
        Assert.Equal(policySummary.Filters.ReasonFilterValues, mappedResponse.Filters.ReasonFilterValues);
        Assert.Equal(policySummary.Filters.AgentLastNameFilterValues, mappedResponse.Filters.AgentLastNameFilterValues);
        Assert.Equal(policySummary.Filters.PrimaryInsuredFilterValues, mappedResponse.Filters.PrimaryInsuredFilterValues);
        Assert.Equal(policySummary.Filters.PolicyNumberFilterValues, mappedResponse.Filters.PolicyNumberFilterValues);
        Assert.Equal(policySummary.Filters.AgentIdFilterValues, mappedResponse.Filters.AgentIdFilterValues);
        Assert.Equal(policySummary.Filters.EmployerFilterValues, mappedResponse.Filters.EmployerFilterValues);
        Assert.Equal(policySummary.Filters.ViewAsFilterValues, mappedResponse.Filters.ViewAsFilterValues);
    }

    [Fact]
    public void RequirementSummaryResponse_IndividualNames_ShouldMap()
    {
        // Arrangement
        var requirementSummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var requirementResponse = requirementSummaryFaker.Generate();
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].IsServicingAgent = true;
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.IsBusiness = false;
        requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.IsBusiness = false;

        // Act
        var mappedResponse = Mapper.Map<RequirementSummariesResponse>(requirementResponse);

        // Assert
        var agentName =
            requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.Person.Name.IndividualLast + ", " +
            requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.Person.Name.IndividualFirst;
        var primaryInsuredName =
            requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualLast + ", " +
            requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualFirst;

        Assert.True(mappedResponse.RequirementSummaries.First().AssignedAgents[0].IsServicingAgent);
        Assert.Equal(agentName, mappedResponse.RequirementSummaries.First().AssignedAgents[0].AssignedAgentName);
        Assert.Equal(primaryInsuredName, mappedResponse.RequirementSummaries.First().PrimaryInsuredName);
        Assert.Equal(
            requirementResponse.RequirementSummaries.First().EmployerName,
            mappedResponse.RequirementSummaries.First().EmployerName);
        Assert.Equal(requirementResponse.Filters.ReasonFilterValues, mappedResponse.Filters.ReasonFilterValues);
        Assert.Equal(requirementResponse.Filters.AgentLastNameFilterValues, mappedResponse.Filters.AgentLastNameFilterValues);
        Assert.Equal(requirementResponse.Filters.PrimaryInsuredFilterValues, mappedResponse.Filters.PrimaryInsuredFilterValues);
        Assert.Equal(requirementResponse.Filters.PolicyNumberFilterValues, mappedResponse.Filters.PolicyNumberFilterValues);
        Assert.Equal(requirementResponse.Filters.AgentIdFilterValues, mappedResponse.Filters.AgentIdFilterValues);
        Assert.Equal(requirementResponse.Filters.EmployerFilterValues, mappedResponse.Filters.EmployerFilterValues);
        Assert.Equal(requirementResponse.Filters.ViewAsFilterValues, mappedResponse.Filters.ViewAsFilterValues);
    }

    [Fact]
    public void RequirementSummaryResponse_BusinessNames_ShouldMap()
    {
        // Arrangement
        var requirementSummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var requirementResponse = requirementSummaryFaker.Generate();
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].IsServicingAgent = true;
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.IsBusiness = true;
        requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.IsBusiness = false;

        // Act
        var mappedResponse = Mapper.Map<RequirementSummariesResponse>(requirementResponse);

        // Assert
        var agentName =
            requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.Business.Name.BusinessName;
        var primaryInsuredName =
            requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualLast + ", " +
            requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualFirst;

        Assert.True(mappedResponse.RequirementSummaries.First().AssignedAgents[0].IsServicingAgent);
        Assert.Equal(agentName, mappedResponse.RequirementSummaries.First().AssignedAgents[0].AssignedAgentName);
        Assert.Equal(primaryInsuredName, mappedResponse.RequirementSummaries.First().PrimaryInsuredName);
        Assert.Equal(
            requirementResponse.RequirementSummaries.First().EmployerName,
            mappedResponse.RequirementSummaries.First().EmployerName);
        Assert.Equal(requirementResponse.Filters.ReasonFilterValues, mappedResponse.Filters.ReasonFilterValues);
        Assert.Equal(requirementResponse.Filters.AgentLastNameFilterValues, mappedResponse.Filters.AgentLastNameFilterValues);
        Assert.Equal(requirementResponse.Filters.PrimaryInsuredFilterValues, mappedResponse.Filters.PrimaryInsuredFilterValues);
        Assert.Equal(requirementResponse.Filters.PolicyNumberFilterValues, mappedResponse.Filters.PolicyNumberFilterValues);
        Assert.Equal(requirementResponse.Filters.AgentIdFilterValues, mappedResponse.Filters.AgentIdFilterValues);
        Assert.Equal(requirementResponse.Filters.EmployerFilterValues, mappedResponse.Filters.EmployerFilterValues);
        Assert.Equal(requirementResponse.Filters.ViewAsFilterValues, mappedResponse.Filters.ViewAsFilterValues);
    }

    [Fact]
    public void RequirementSummaryResponse_NoName_ShouldMap()
    {
        // Arrangement
        var requirementSummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var requirementResponse = requirementSummaryFaker.Generate();
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.IsBusiness = false;
        requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.IsBusiness = false;
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.Person.Name.IndividualFirst = null;
        requirementResponse.RequirementSummaries.First().AssignedAgents[0].Participant.Person.Name.IndividualLast = null;
        requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualFirst = null;
        requirementResponse.RequirementSummaries.First().PrimaryInsured.Participant.Person.Name.IndividualLast = null;

        // Act
        var mappedResponse = Mapper.Map<RequirementSummariesResponse>(requirementResponse);

        // Assert
        Assert.Equal(string.Empty, mappedResponse.RequirementSummaries.First().AssignedAgents[0].AssignedAgentName);
        Assert.Equal(string.Empty, mappedResponse.RequirementSummaries.First().PrimaryInsuredName);
    }

    [Fact]
    public void PolicySummaryResponse_NoName_ShouldMap()
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummariesResponse>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.Policies.First().AssignedAgents[0].Participant.IsBusiness = false;
        policySummary.Policies.First().AssignedAgents[0].Participant.Person.Name.IndividualFirst = null;
        policySummary.Policies.First().AssignedAgents[0].Participant.Person.Name.IndividualLast = null;
        policySummary.Policies.First().PrimaryInsured.Participant.IsBusiness = false;
        policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualFirst = null;
        policySummary.Policies.First().PrimaryInsured.Participant.Person.Name.IndividualLast = null;

        // Act
        var mappedResponse = Mapper.Map<PolicySummariesResponse>(policySummary);

        // Assert
        Assert.Equal(string.Empty, mappedResponse.Policies.First().AssignedAgents[0].AssignedAgentName);
        Assert.Equal(string.Empty, mappedResponse.Policies.First().PrimaryInsuredName);
    }

    [Fact]
    public void PolicySummaryResponse_NullAgentAndInsured_ShouldMap()
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummariesResponse>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.Policies.First().AssignedAgents = null;
        policySummary.Policies.First().PrimaryInsured = null;

        // Act
        var mappedResponse = Mapper.Map<PolicySummariesResponse>(policySummary);

        // Assert
        Assert.False(mappedResponse.Policies.First().AssignedAgents.Any());
        Assert.Equal(string.Empty, mappedResponse.Policies.First().PrimaryInsuredName);
    }

    [Fact]
    public void RequirementSummaryResponse_NullAgentAndInsured_ShouldMap()
    {
        // Arrangement
        var requirementSummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var requirementResponse = requirementSummaryFaker.Generate();
        requirementResponse.RequirementSummaries.First().AssignedAgents = null;
        requirementResponse.RequirementSummaries.First().PrimaryInsured = null;

        // Act
        var mappedResponse = Mapper.Map<RequirementSummariesResponse>(requirementResponse);

        // Assert
        Assert.False(mappedResponse.RequirementSummaries.First().AssignedAgents.Any());
        Assert.Equal(string.Empty, mappedResponse.RequirementSummaries.First().PrimaryInsuredName);
    }

    [Fact]
    public void PolicySummaryResponse_EmptySummaries_ShouldMap()
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummariesResponse>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.Policies = new List<PolicyInfoAPI.PolicySummary>();

        // Act
        var mappedResponse = Mapper.Map<PolicySummariesResponse>(policySummary);

        // Assert
        Assert.Empty(mappedResponse.Policies);
    }

    [Fact]
    public void RequirementSummaryResponse_EmptySummaries_ShouldMap()
    {
        // Arrangement
        var requirementSummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummaryResponse>();
        var requirementResponse = requirementSummaryFaker.Generate();
        requirementResponse.RequirementSummaries = new List<PolicyInfoAPI.RequirementSummary>();

        // Act
        var mappedResponse = Mapper.Map<RequirementSummariesResponse>(requirementResponse);

        // Assert
        Assert.Empty(mappedResponse.RequirementSummaries);
    }
}