namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.AgentPortal.Managers.Claims.Mapping;
using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[ExcludeFromCodeCoverage]
public class ClaimsMappingProfileTests
{
    public ClaimsMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<ClaimsMappingProfile>();
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
    public void AssureLinkClaimResponse_To_ClaimResponse_ShouldMap()
    {
        // Arrange
        var assureLinkClaimResponse = ClaimsSourceData.ClaimResponse;

        // Act
        var mappedClaimResponse = Mapper.Map<ClaimsResponse>(assureLinkClaimResponse);

        // Assert
        var expectedClaimResponse = ClaimsDestinationData.ClaimsResponse;

        var compareResult = new CompareLogic()
            .Compare(expectedClaimResponse, mappedClaimResponse);

        Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
    }
}