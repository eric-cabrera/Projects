namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.TaxForms;
using Assurity.AgentPortal.Managers.TaxForms.Mapping;
using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[ExcludeFromCodeCoverage]
public class TaxFormsResponseMappingProfileTests
{
    public TaxFormsResponseMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<TaxFormsResponseMappingProfile>();
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
    public void AgentForm_To_TaxForm_ShouldMap()
    {
        // Arrange
        var taxForm = TaxFormsSourceData.TaxForm;

        // Act
        var mappedTaxForm = Mapper.Map<TaxForm>(taxForm);

        // Assert
        var expectedTaxForm = TaxFormsDestinationData.TaxForm;

        var compareResult = new CompareLogic()
            .Compare(expectedTaxForm, mappedTaxForm);

        Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
    }
}
