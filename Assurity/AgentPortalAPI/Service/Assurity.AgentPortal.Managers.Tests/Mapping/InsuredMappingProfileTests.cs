namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
    using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class InsuredMappingProfileTests
    {
        public InsuredMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<InsuredMappingProfile>();
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
        public void Insured_To_Insured_ShouldMap()
        {
            // Arrange
            var insured = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self);

            // Act
            var mappedInsured = Mapper.Map<Insured>(insured);

            // Assert
            var expectedInsured = PolicyDestinationTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self);

            var compareResult = new CompareLogic()
                .Compare(expectedInsured, mappedInsured);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}