namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.Claims;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.AgentPortal.Managers.Claims;
using Assurity.AgentPortal.Managers.Claims.Mapping;
using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
using Assurity.Claims.Contracts;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Xunit;

using ClaimsAPI = Assurity.Claims.Contracts.AssureLink;

[ExcludeFromCodeCoverage]
public class ClaimsManagerTests
{
    private readonly Mapper mapper;

    public ClaimsManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new ClaimsMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetClaims_Success()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };
        var cancellationToken = CancellationToken.None;

        var mockClaimsApiAccessor = new Mock<IClaimsApiAccessor>();
        mockClaimsApiAccessor.Setup(accessor => accessor.GetClaims(agentId, claimsParameters, cancellationToken))
            .ReturnsAsync(() => GetMockClaimsResponse());

        var claimsManager = new ClaimsManager(mapper, mockClaimsApiAccessor.Object);

        // Act
        var response = await claimsManager.GetClaims(agentId, claimsParameters);
        var expectedClaimResponse = ClaimsDestinationData.ClaimsResponse;
        var compareResult = new CompareLogic()
            .Compare(expectedClaimResponse, response);

        // Assert
        Assert.NotNull(response);
        Assert.True(compareResult.AreEqual, compareResult.DifferencesString);

        mockClaimsApiAccessor.Verify(
            accessor => accessor.GetClaims(
            agentId,
            claimsParameters,
            cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetClaims_NullResponse_ShouldReturn_Null()
    {
        // Arrange
        var agentId = "123A";
        var claimsParameters = new ClaimsParameters
        {
            ClaimantFirstName = "John",
            ClaimantLastName = "Smith"
        };

        var cancellationToken = CancellationToken.None;

        var mockClaimsApiAccessor = new Mock<IClaimsApiAccessor>();
        mockClaimsApiAccessor.Setup(accessor => accessor.GetClaims(agentId, claimsParameters, cancellationToken))
            .ReturnsAsync(() => null);

        var claimsManager = new ClaimsManager(mapper, mockClaimsApiAccessor.Object);

        // Act
        var response = await claimsManager.GetClaims(agentId, claimsParameters);

        // Assert
        Assert.Null(response);

        mockClaimsApiAccessor.Verify(
            accessor => accessor.GetClaims(
            agentId,
            claimsParameters,
            cancellationToken),
            Times.Once);
    }

    private ClaimsAPI.AssureLinkClaimResponse GetMockClaimsResponse()
    {
        var mockClaimsResponse = new ClaimsAPI.AssureLinkClaimResponse
        {
            Claims = new List<ClaimsAPI.Claim>
            {
                new ClaimsAPI.Claim
                {
                    ClaimNumber = null,
                    Claimant = new ClaimsAPI.Name
                    {
                        FirstName = "John",
                        LastName = "Smith"
                    },
                    DateReported = DateTime.Parse("12/12/2024"),
                    PaymentAmount = 1,
                    PolicyNumber = "4180078103",
                    PolicyType = "Disability Income",
                    Status = ClaimStatus.Received,
                    StatusReason = string.Empty,
                    Details = [
                        new ClaimsAPI.ClaimDetail
                        {
                            DeliveryMethod = "Check",
                            BenefitDate = DateTime.Parse("12/12/2024"),
                            BenefitDescription = "Cancer",
                            PaymentAmount = 1,
                            PaymentDate = DateTime.Parse("12/12/2024"),
                            PolicyNumber = "4180078103",
                            Status = ClaimStatus.Received
                        }
                    ]
                },
            },
            Page = 1,
            PageSize = 10,
            TotalRecords = 1,
        };

        return mockClaimsResponse;
    }
}