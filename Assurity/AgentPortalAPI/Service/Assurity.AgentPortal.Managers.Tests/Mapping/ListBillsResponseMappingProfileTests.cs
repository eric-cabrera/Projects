namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Managers.ListBill.Mapping;
using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
using Assurity.ListBill.Service.Contracts;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[ExcludeFromCodeCoverage]
public class ListBillsResponseMappingProfileTests
{
    public ListBillsResponseMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<ListBillsResponseMappingProfile>();
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
    public void ListBill_To_ListBill_ShouldMap()
    {
        // Arrange
        var listBill = ListBillsSourceData.ListBill;

        // Act
        var mappedListBill = Mapper.Map<ListBill>(listBill);

        // Assert
        var expectedListBill = ListBillsDestinationData.ListBill;

        var compareResult = new CompareLogic()
            .Compare(expectedListBill, mappedListBill);

        Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
    }
}