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
    public class NameMappingProfileTests
    {
        public NameMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<NameMappingProfile>();
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
        public void Name_To_Name_ShouldMap()
        {
            // Arrange
            var name = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Person.Name;

            // Act
            var mappedName = Mapper.Map<Name>(name);

            // Assert
            var expectedName = PolicyDestinationTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Person.Name;

            var compareResult = new CompareLogic()
                .Compare(expectedName, mappedName);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}