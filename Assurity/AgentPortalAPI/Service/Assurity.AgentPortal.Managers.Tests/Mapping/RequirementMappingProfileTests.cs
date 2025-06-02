namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
    using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class RequirementMappingProfileTests
    {
        public RequirementMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<ParticipantMappingProfile>();
                config.AddProfile<PersonMappingProfile>();
                config.AddProfile<RequirementMappingProfile>();
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
        public void Requirement_To_Requirement_ShouldMap()
        {
            // Arrange
            var requirement = PolicySourceTestData.ActivePolicy.Requirements.First();

            // Act
            var mappedRequirement = Mapper.Map<RequirementResponse>(requirement);

            // Assert
            var expectedRequirement = PolicyDestinationTestData.ActivePolicy.Requirements.First();

            var compareResult = new CompareLogic()
                .Compare(expectedRequirement, mappedRequirement);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}