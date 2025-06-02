namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
    using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class AgentMappingProfileTests
    {
        public AgentMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<AgentMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<ParticipantMappingProfile>();
                config.AddProfile<PersonMappingProfile>();
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
        public void Agent_To_Agent_ShouldMap()
        {
            // Arrange
            var agent = PolicySourceTestData.ActivePolicy.Agents.First();

            // Act
            var mappedAgent = Mapper.Map<Agent>(agent);

            // Assert
            var expectedAgent = PolicyDestinationTestData.ActivePolicy.Agents.First();

            var compareResult = new CompareLogic()
                .Compare(expectedAgent, mappedAgent);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}