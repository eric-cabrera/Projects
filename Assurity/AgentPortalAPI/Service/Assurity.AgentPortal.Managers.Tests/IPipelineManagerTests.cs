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
public class IPipelineManagerTests
{
    private readonly Mapper mapper;

    public IPipelineManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new IPipelineSsoInfoMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetBrowserPostSamlSignature_Success()
    {
        // Arrange
        var agentId = "1234";
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

        var mockIPipelineEngine = new Mock<IIPipelineEngine>();
        mockIPipelineEngine.Setup(engine => engine.GetBrowserPostSamlSignature(It.IsAny<IPipelineSsoInfo>()))
            .Returns(() => "pd5gds89jdsd94gsadsa57asd0ss2hjls98s");

        var iPipelineManager = new IPipelineManager(mockAgentApiAccessor.Object, mockIPipelineEngine.Object, mapper);

        // Act
        var response = await iPipelineManager.GetBrowserPostSamlSignature(agentId, cancellationToken);

        // Assert
        var expectedCredentialsString = "pd5gds89jdsd94gsadsa57asd0ss2hjls98s";

        Assert.NotNull(response);
        Assert.Equal(expectedCredentialsString, response);

        mockIPipelineEngine.Verify(
            engine => engine.GetBrowserPostSamlSignature(
            It.IsAny<IPipelineSsoInfo>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBrowserPostSamlSignature_AgentInformation_Null_ShouldReturn_NullResponse()
    {
        // Arrange
        var agentId = "1234";
        var cancellationToken = CancellationToken.None;

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAgentInformation(agentId, cancellationToken))
            .ReturnsAsync(() => null);

        var mockIPipelineEngine = new Mock<IIPipelineEngine>();
        mockIPipelineEngine.Setup(engine => engine.GetBrowserPostSamlSignature(
            It.IsAny<IPipelineSsoInfo>()))
            .Returns(() => "pd5gds89jdsd94gsadsa57asd0ss2hjls98s");

        var iPipelineManager = new IPipelineManager(mockAgentApiAccessor.Object, mockIPipelineEngine.Object, mapper);

        // Act
        var response = await iPipelineManager.GetBrowserPostSamlSignature(agentId, cancellationToken);

        // Assert
        Assert.Null(response);

        mockIPipelineEngine.Verify(
            engine => engine.GetBrowserPostSamlSignature(
            It.IsAny<IPipelineSsoInfo>()),
            Times.Never);
    }

    [Fact]
    public void GetBrowserPostSamlSignature_ForHomeOfficeUser_Success()
    {
        // Arrange
        var agentInfo = new IPipelineSsoInfo
        {
            AgentNumber = "HO9999",
            FirstName = "Assurity",
            LastName = "Life",
            Agentname = "Assurity Life",
            Email = "HelpDesk@assurity.com",
            Phone = "0000000000",
            Fax = "0000000000",
            Address1 = "PO Box 82533",
            City = "LINCOLN",
            State = "NE",
            ZipCode = "68501"
        };

        var expectedCredentialsString = "pd5gds89jdsd94gsadsa57asd0ss2hjls98s";

        var mockIPipelineEngine = new Mock<IIPipelineEngine>();
        mockIPipelineEngine.Setup(engine => engine.GetBrowserPostSamlSignature(
            It.IsAny<IPipelineSsoInfo>()))
            .Returns(() => "pd5gds89jdsd94gsadsa57asd0ss2hjls98s");

        var iPipelineManager = new IPipelineManager(null, mockIPipelineEngine.Object, mapper);

        // Act
        var response = iPipelineManager.GetBrowserPostSamlSignature();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedCredentialsString, response);
    }
}