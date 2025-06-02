namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using AutoBogus;
using AutoMapper;
using Xunit;
using CommissionsAPI = Assurity.Commissions.Debt.Contracts.Advances;

[ExcludeFromCodeCoverage]
public class DebtResponseMappingProfileTests
{
    public DebtResponseMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<DebtResponseMappingProfile>();
            config.AddProfile<AgentMappingProfile>();
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
    public void WritingAgentToDebtAgent_IndividualAgent_ShouldMap()
    {
        // Arrange
        var writingAgent = new AutoFaker<CommissionsAPI.Agent>().Generate();
        writingAgent.Participant.Business = null;

        // Act
        var mappedResult = Mapper.Map<DebtAgent>(writingAgent);

        // Assert
        var expectedName = $"{writingAgent.Participant.Person.IndividualLast}, {writingAgent.Participant.Person.IndividualFirst}";
        Assert.Equal(expectedName, mappedResult.AgentName);
        Assert.Equal(writingAgent.Status.ToString(), mappedResult.AgentStatus);
    }

    [Fact]
    public void WritingAgentToDebtAgent_BusinessAgent_ShouldMap()
    {
        // Arrange
        var writingAgent = new AutoFaker<CommissionsAPI.Agent>().Generate();
        writingAgent.Participant.Person = null;

        // Act
        var mappedResult = Mapper.Map<DebtAgent>(writingAgent);

        // Assert
        var expectedName = $"{writingAgent.Participant.Business.BusinessName}";
        Assert.Equal(expectedName, mappedResult.AgentName);
        Assert.Equal(writingAgent.Status.ToString(), mappedResult.AgentStatus);
    }

    [Fact]
    public void PolicyToDebtPolicy_InsuredName_ShouldMap()
    {
        // Arrange
        var policy = new AutoFaker<CommissionsAPI.Policy>().Generate();
        policy.Insureds.First().Participant.Business = null;

        // Act
        var mappedResult = Mapper.Map<DebtPolicy>(policy);

        // Assert
        var expectedName = $"{policy.Insureds.First().Participant.Person.IndividualLast}, " +
            $"{policy.Insureds.First().Participant.Person.IndividualFirst}";
        Assert.Equal(expectedName, mappedResult.InsuredName);
    }
}
