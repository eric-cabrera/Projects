namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using Assurity.Commissions.Debt.Contracts.Advances;
using AutoBogus;
using AutoMapper;
using Xunit;

public class SecuredDebtSummaryMappingProfileTests
{
    public SecuredDebtSummaryMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<SecuredDebtSummaryMappingProfile>();
        }).CreateMapper();
    }

    private IMapper Mapper { get; set; }

    [Fact]
    public void AssertConfigurationIsValid()
    {
        // Assert
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void SecuredDebtSummaryMapping_ShouldMap()
    {
        // Arrange
        var writingAgents = new AutoFaker<Agent>().Generate();

        // Act
        var result = Mapper.Map<List<SecuredDebtExport>>(writingAgents);

        // Assert
        Assert.Equal(writingAgents.Policies.Count, result.Count);
    }

    [Fact]
    public void SecuredDebtSummaryMapping_NullInsuredsList_ShouldMap()
    {
        // Arrange
        var writingAgent = new AutoFaker<Agent>().Generate();
        writingAgent.Policies.First().Insureds = null;

        // Act
        var result = Mapper.Map<List<SecuredDebtExport>>(writingAgent);

        // Assert
        Assert.Equal(writingAgent.Policies.Count, result.Count);
    }
}
