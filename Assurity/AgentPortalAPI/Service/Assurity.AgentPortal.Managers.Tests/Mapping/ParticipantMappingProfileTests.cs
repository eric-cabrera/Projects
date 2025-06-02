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
    public class ParticipantMappingProfileTests
    {
        public ParticipantMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
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
        public void Participant_To_Participant_ShouldMap()
        {
            // Arrange
            var participant = PolicySourceTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant;

            // Act
            var mappedParticipant = Mapper.Map<Participant>(participant);

            // Assert
            var expectedParticipant = PolicyDestinationTestData.ActivePolicy.Insureds
                .Single(insured => insured.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self)
                .Participant;

            var compareResult = new CompareLogic()
                .Compare(expectedParticipant, mappedParticipant);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}