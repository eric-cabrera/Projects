namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using Assurity.Commissions.Debt.Contracts.Advances;
using AutoBogus;
using AutoMapper;
using Xunit;

public class UnsecuredDebtSummaryMappingProfileTests
{
    public UnsecuredDebtSummaryMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<UnsecuredDebtSummaryMappingProfile>();
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
    public void UnsecuredDebtSummaryMapping_ShouldMap()
    {
        // Arrange
        var writingAgent = new AutoFaker<Agent>().Generate();

        // Act
        var result = Mapper.Map<List<UnsecuredDebtExport>>(writingAgent);

        // Assert
        Assert.Equal(writingAgent.Policies.Count, result.Count);
    }

    [Fact]
    public void UnsecuredDebtSummaryMapping_NullInsuredsList_ShouldMap()
    {
        // Arrange
        var writingAgent = new AutoFaker<Agent>().Generate();

        writingAgent.Policies.First().Insureds = null;

        // Act
        var result = Mapper.Map<List<UnsecuredDebtExport>>(writingAgent);

        // Assert
        Assert.Equal(writingAgent.Policies.Count, result.Count);
    }
}
