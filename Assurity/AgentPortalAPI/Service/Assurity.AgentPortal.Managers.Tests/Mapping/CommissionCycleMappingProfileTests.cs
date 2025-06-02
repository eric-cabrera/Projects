namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using AutoMapper;
using Xunit;

[ExcludeFromCodeCoverage]
public class CommissionCycleMappingProfileTests
{
    public CommissionCycleMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<CommissionCycleMappingProfile>();
        }).CreateMapper();
    }

    private IMapper Mapper { get; }

    [Fact]
    public void AssertConfigurationIsValid()
    {
        // Assert
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
