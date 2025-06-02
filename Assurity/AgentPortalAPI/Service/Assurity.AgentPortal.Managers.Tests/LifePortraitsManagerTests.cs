namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.Agent.Contracts;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.Integration;
using Assurity.AgentPortal.Accessors.LifePortraits;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Integration;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Managers.Integration.Mapping;
using AutoMapper;
using Moq;
using Xunit;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;

[ExcludeFromCodeCoverage]
public class LifePortraitsManagerTests
{
    private readonly Mapper mapper;

    public LifePortraitsManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new IllustrationSsoInfoMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetCredentialsForLifePortraits_Success()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;
        var redirectUrl = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";

        var agentInformation = new AgentInformation()
        {
            Name = new Assurity.Agent.Contracts.Name
            {
                IndividualFirst = "Jacob",
                IndividualLast = "Smith",
                IndividualMiddle = "R",
                IndividualSuffix = "Jr",
                IsBusiness = false
            },
            Address = new Assurity.Agent.Contracts.Address
            {
                Line1 = "123 ABC St",
                City = "Lincoln",
                StateAbbreviation = "NE",
                Zip = "68510",
                EmailAddress = "abc123@gmail.com",
                PhoneNumber = "4021234567",
                FaxNumber = "4023456789"
            }
        };

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentInformation(agentId, cancellationToken))
            .ReturnsAsync(() => agentInformation);

        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentMarketCodes(agentId, true, cancellationToken))
            .ReturnsAsync(() => new List<string> { "ISNY", "AGTDTC", "WSR11" });

        var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
        mockDataStoreAccessor.Setup(accessor => accessor.GetFiservDistributionChannelForLifePortraits(It.IsAny<List<string>>()))
            .ReturnsAsync(() => "IS");

        mockDataStoreAccessor.Setup(accessor => accessor.GetLifePortraitsSSOUserId(It.IsAny<string>()))
            .ReturnsAsync(() => 0);

        mockDataStoreAccessor.Setup(accessor => accessor.CreateSSOUserId(It.IsAny<string>()))
            .ReturnsAsync(() => 60004);

        var expectedCredentialsString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>60004</AGENTID><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1234</AGENCYID><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>IS</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";

        var mockLifePortraitsApiAccessor = new Mock<ILifePortraitsApiAccessor>();
        mockLifePortraitsApiAccessor.Setup(accessor => accessor.GetURL(expectedCredentialsString, cancellationToken))
            .ReturnsAsync(() => redirectUrl);

        var lifePortraitsManager = new LifePortraitsManager(mockAgentApiAccessor.Object, mockDataStoreAccessor.Object, mockLifePortraitsApiAccessor.Object, mapper);

        // Act
        var response = await lifePortraitsManager.GetCredentialsForLifePortraits(agentId, userName);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedCredentialsString, response);

        mockDataStoreAccessor.Verify(
            accessor => accessor.GetFiservDistributionChannelForLifePortraits(
            It.IsAny<List<string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCredentialsForLifePortraits_AgentInformation_Null_ShouldReturn_NullResponse()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;
        var redirectUrl = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentInformation(agentId, cancellationToken))
            .ReturnsAsync(() => null);

        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentMarketCodes(agentId, true, cancellationToken))
            .ReturnsAsync(() => new List<string> { "ISNY", "AGTDTC", "WSR11" });

        var expectedCredentialsString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>un1234</AGENTID><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1234</AGENCYID><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>IS</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";

        var mockLifePortraitsApiAccessor = new Mock<ILifePortraitsApiAccessor>();
        mockLifePortraitsApiAccessor.Setup(accessor => accessor.GetURL(expectedCredentialsString, cancellationToken))
            .ReturnsAsync(() => redirectUrl);

        var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();

        var lifePortraitsManager = new LifePortraitsManager(mockAgentApiAccessor.Object, mockDataStoreAccessor.Object, mockLifePortraitsApiAccessor.Object, mapper);

        // Act
        var response = await lifePortraitsManager.GetCredentialsForLifePortraits(agentId, userName);

        // Assert
        Assert.Null(response);

        mockDataStoreAccessor.Verify(
            accessor => accessor.GetFiservDistributionChannelForLifePortraits(
            It.IsAny<List<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCredentialsForLifePortraits_GetAgentMarketCodes_Null_ShouldReturnNull()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;
        var expectedCredentialsString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>un1234</AGENTID><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1234</AGENCYID><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>IS</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";
        var redirectUrl = "https://fiservtest.assurity.com/Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey=16930B333D24444DA61E9A4A207962B9";

        var agentInformation = new AgentInformation()
        {
            Name = new Assurity.Agent.Contracts.Name
            {
                IndividualFirst = "Jacob",
                IndividualLast = "Smith",
                IndividualMiddle = "R",
                IndividualSuffix = "Jr",
                IsBusiness = false
            },
            Address = new Assurity.Agent.Contracts.Address
            {
                Line1 = "123 ABC St",
                City = "Lincoln",
                StateAbbreviation = "NE",
                Zip = "68510",
                EmailAddress = "abc123@gmail.com",
                PhoneNumber = "4021234567",
                FaxNumber = "4023456789"
            }
        };

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentInformation(agentId, cancellationToken))
            .ReturnsAsync(() => agentInformation);

        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentMarketCodes(agentId, true, cancellationToken))
            .ReturnsAsync(() => null);

        var mockLifePortraitsApiAccessor = new Mock<ILifePortraitsApiAccessor>();
        mockLifePortraitsApiAccessor.Setup(accessor => accessor.GetURL(expectedCredentialsString, cancellationToken))
            .ReturnsAsync(() => redirectUrl);

        var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();

        var lifePortraitsManager = new LifePortraitsManager(mockAgentApiAccessor.Object, mockDataStoreAccessor.Object, mockLifePortraitsApiAccessor.Object, mapper);

        // Act
        var response = await lifePortraitsManager.GetCredentialsForLifePortraits(agentId, userName);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetCredentialsForLifePortraitsForHomeOfficeUser_Success()
    {
        // Arrange
        var expectedCredentialsString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>60004</AGENTID><FIRSTNAME>Assurity Life</FIRSTNAME><LASTNAME>Insurance Co.</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID>1</AGENCYID><AGENCYNAME>Assurity Life Insurance Co.</AGENCYNAME><ADDRESS1>PO Box 82533</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68501-2533</ZIP><PHONE>1-800-869-0355</PHONE><EMAIL /><ISBANK>0</ISBANK></AGENCY><DISTRIBUTION>HO</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";

        var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
        mockDataStoreAccessor.Setup(accessor => accessor.GetLifePortraitsSSOUserId(It.IsAny<string>()))
            .ReturnsAsync(60004);

        var lifePortraitsManager = new LifePortraitsManager(null, mockDataStoreAccessor.Object, null, null);

        // Act
        var response = lifePortraitsManager.GetCredentialsForLifePortraitsForHomeOfficeUser("ab1234");

        // Assert
        Assert.NotNull(response?.Result);
        Assert.Equal(expectedCredentialsString, response?.Result);
    }
}