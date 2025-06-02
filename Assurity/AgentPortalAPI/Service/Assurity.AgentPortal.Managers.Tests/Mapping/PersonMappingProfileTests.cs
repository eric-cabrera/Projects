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
    public class PersonMappingProfileTests
    {
        public PersonMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<NameMappingProfile>();
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
        public void Person_To_Person_ShouldMap()
        {
            // Arrange
            var person = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Person;

            // Act
            var mappedPerson = Mapper.Map<Person>(person);

            // Assert
            var expectedPerson = PolicyDestinationTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant.Person;

            var compareResult = new CompareLogic()
                .Compare(expectedPerson, mappedPerson);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}