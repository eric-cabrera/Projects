namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.Agent.Contracts;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Contracts.Integration;
using Assurity.AgentPortal.Engines.Integration;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Managers.Integration.Mapping;
using Assurity.AgentPortal.Utilities.Integration;
using AutoMapper;
using Moq;
using Xunit;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;

[ExcludeFromCodeCoverage]
public class IllustrationProManagerTests
{
    private readonly Mapper mapper;

    public IllustrationProManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new IllustrationSsoInfoMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetCredentialsForIllustrationPro_Success()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

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

        var mockIllustrationProEngine = new Mock<IIllustrationProEngine>();
        mockIllustrationProEngine.Setup(engine => engine.GetFiservDistributionChannelForIllustrationPro(It.IsAny<List<string>>()))
            .Returns(() => "IS");

        var illustrationProManager = new IllustrationProManager(mockAgentApiAccessor.Object, mockIllustrationProEngine.Object, null, mapper);

        // Act
        var response = await illustrationProManager.GetCredentialsForIllustrationPro(agentId, userName);

        // Assert
        var expectedCredentialsString = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><agentSetup xmlns=\"http://wwww/stoneriver.com/services/AgentAccountService\"><requestObject><a:accountSetupRequestXML xmlns:a=\"http://schemas.datacontract.org/2004/07/LifeServer.DataModel.request\"><AgentAccountSetupData xmlns=\"\"><AGENT><UNIQUEID>un1234</UNIQUEID><UNIQUEPASS>TEST</UNIQUEPASS><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><EMAILTO></EMAILTO><ROLECODE>Default_Agent</ROLECODE><LANGUAGE>en-us</LANGUAGE><PROFILES><PROFILE><DISTRIBUTION>DIST_ASR_IS</DISTRIBUTION><AGENTID>1234</AGENTID><AGENCYID>1234</AGENCYID><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL></PROFILE></PROFILES></AGENT></AgentAccountSetupData></a:accountSetupRequestXML></requestObject></agentSetup></s:Body></s:Envelope>";
        Assert.NotNull(response);
        Assert.Equal(expectedCredentialsString, response);

        mockIllustrationProEngine.Verify(
            engine => engine.GetFiservDistributionChannelForIllustrationPro(
            It.IsAny<List<string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCredentialsForIllustrationPro_AgentInformation_Null_ShouldReturn_NullResponse()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentInformation(agentId, cancellationToken))
            .ReturnsAsync(() => null);

        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentMarketCodes(agentId, true, cancellationToken))
            .ReturnsAsync(() => new List<string> { "ISNY", "AGTDTC", "WSR11" });

        var mockIllustrationProEngine = new Mock<IIllustrationProEngine>();
        mockIllustrationProEngine.Setup(engine => engine.GetFiservDistributionChannelForIllustrationPro(It.IsAny<List<string>>()))
            .Returns(() => "IS");

        var illustrationProManager = new IllustrationProManager(mockAgentApiAccessor.Object, mockIllustrationProEngine.Object, null, mapper);

        // Act
        var response = await illustrationProManager.GetCredentialsForIllustrationPro(agentId, userName);

        // Assert
        Assert.Null(response);

        mockIllustrationProEngine.Verify(
            engine => engine.GetFiservDistributionChannelForIllustrationPro(
            It.IsAny<List<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCredentialsForIllustrationPro_GetAgentMarketCodes_Null_ShouldReturnNull()
    {
        // Arrange
        var agentId = "1234";
        var userName = "un1234";
        var cancellationToken = CancellationToken.None;

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

        var illustrationProEngine = new IllustrationProEngine();

        var illustrationProManager = new IllustrationProManager(mockAgentApiAccessor.Object, illustrationProEngine, null, mapper);

        // Act
        var response = await illustrationProManager.GetCredentialsForIllustrationPro(agentId, userName);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void GetCredentialsForIllustrationProForHomeOfficeUser_Success()
    {
        // Arrange
        var agentInfo = new IllustrationSsoInfo
        {
            AGENCYID = "1",
            FIRSTNAME = "Assurity Life",
            LASTNAME = "Insurance Co.",
            AGENCYNAME = "Assurity Life Insurance Co.",
            ADDRESS1 = "PO Box 82533",
            CITY = "Lincoln",
            STATE = "NE",
            ZIP = "68501-2533",
            PHONE = "800-869-0355",
            ISBANK = "0"
        };

        var expectedOutput = XmlHelper.SerializeIllustrationProXml(agentInfo, "HO", "ab1234").InnerXml;

        var illustrationProManager = new IllustrationProManager(null, null, null, null);

        // Act
        var response = illustrationProManager.GetCredentialsForIllustrationProForHomeOfficeUser("ab1234");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedOutput, response);
    }
}