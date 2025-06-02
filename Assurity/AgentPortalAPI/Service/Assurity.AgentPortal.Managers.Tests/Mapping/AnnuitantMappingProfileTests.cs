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
    public class AnnuitantMappingProfileTests
    {
        public AnnuitantMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<AnnuitantMappingProfile>();
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
        public void Annuitant_To_Annuitant_ShouldMap()
        {
            // Arrange
            var annuitant = PolicySourceTestData.ActivePolicy.Annuitants.First();

            // Act
            var mappedAnnuitant = Mapper.Map<Annuitant>(annuitant);

            // Assert
            var expectedAnnuitant = PolicyDestinationTestData.ActivePolicy.Annuitants.First();

            var compareResult = new CompareLogic()
                .Compare(expectedAnnuitant, mappedAnnuitant);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}