namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Managers.CommissionsAndDebt.Mapping;
using AutoMapper;
using Xunit;

[ExcludeFromCodeCoverage]
public class AgentStatementOptionsMappingProfileTests
{
    public AgentStatementOptionsMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<AgentStatementOptionsMappingProfile>();
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
