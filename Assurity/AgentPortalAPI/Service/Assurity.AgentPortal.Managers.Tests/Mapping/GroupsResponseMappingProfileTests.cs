namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Managers.ListBill.Mapping;
using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
using Assurity.ListBill.Service.Contracts;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[ExcludeFromCodeCoverage]
public class GroupsResponseMappingProfileTests
{
    public GroupsResponseMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<GroupsResponseMappingProfile>();
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
    public void Group_To_Group_ShouldMap()
    {
        // Arrange
        var group = ListBillsSourceData.Group;

        // Act
        var mappedGroup = Mapper.Map<Group>(group);

        // Assert
        var expectedGroup = ListBillsDestinationData.Group;

        var compareResult = new CompareLogic()
            .Compare(expectedGroup, mappedGroup);

        Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
    }
}