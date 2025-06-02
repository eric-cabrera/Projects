namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Agent.Contracts;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Managers.Integration.Mapping;
    using Assurity.Models.UnitedStates;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;
    using DTOs = Assurity.AgentPortal.Accessors.DTOs;

    [ExcludeFromCodeCoverage]
    public class AgentInformationMappingProfileTests
    {
        public AgentInformationMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<IllustrationSsoInfoMappingProfile>();
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
        public void AgentInformationResponse_To_IllustrationSsoInfo_ShouldMap()
        {
           var agentInformationResponse = new AgentInformation()
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

            // Act
           var mappedAgentInfo = Mapper.Map<IllustrationSsoInfo>(agentInformationResponse);

            // Assert
           var expectedAgent = new IllustrationSsoInfo
            {
                ADDRESS1 = "123 ABC St",
                CITY = "Lincoln",
                EMAIL = "abc123@gmail.com",
                FIRSTNAME = "Jacob",
                LASTNAME = "Smith",
                PHONE = "4021234567",
                STATE = "NE",
                ZIP = "68510"
            };

           var compareResult = new CompareLogic()
                .Compare(expectedAgent, mappedAgentInfo);

           Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}