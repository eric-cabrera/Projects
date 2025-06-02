namespace Assurity.AgentPortal.Engines.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.Integration;
using Assurity.AgentPortal.Engines.Integration;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class IPipelineEngineTests
{
    [Fact]
    public void GetBrowserPostSamlSignature_ShouldReturn_Base64_String()
    {
        // Arrange
        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(configurationManager => configurationManager.IPipelineConnectionString)
            .Returns("https://federate-uat.ipipeline.com/sp/ACS.saml2");

        mockConfigurationManager.Setup(configurationManager => configurationManager.IPipelineDomain)
          .Returns("ipipeline.com");

        var iPipelineEngine = new IPipelineEngine(mockConfigurationManager.Object);
        var agentInfo = new IPipelineSsoInfo
        {
           AgentNumber = "1234",
           Agentname = "Jacob Smith",
           FirstName = "Jacob",
           LastName = "Smith",
           Address1 = "123 ABC St",
           City = "Lincoln",
           State = "NE",
           ZipCode = "68510",
           Email = "abc123@gmail.com",
           Phone = "4021234567",
           Fax = "4023456789"
        };

        // Act
        var signedSaml = iPipelineEngine.GetBrowserPostSamlSignature(agentInfo);

        // Assert
        Assert.NotNull(signedSaml);
        Assert.IsType<string>(signedSaml);
    }
}