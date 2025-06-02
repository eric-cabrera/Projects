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
    public class OwnerMappingProfileTests
    {
        public OwnerMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<OwnerMappingProfile>();
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
        public void Owner_To_Owner_ShouldMap()
        {
            // Arrange
            var owner = PolicySourceTestData.ActivePolicy.Owners.First();

            // Act
            var mappedOwner = Mapper.Map<Owner>(owner);

            // Assert
            var expectedOwner = PolicyDestinationTestData.ActivePolicy.Owners.First();

            var compareResult = new CompareLogic()
                .Compare(expectedOwner, mappedOwner);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}