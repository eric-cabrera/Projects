namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Managers.ProductionCredit;
using AutoMapper;
using Xunit;

[ExcludeFromCodeCoverage]
public class ProductionCreditMappingProfileTests
{
    public ProductionCreditMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<ProductionCreditMappingProfile>();
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