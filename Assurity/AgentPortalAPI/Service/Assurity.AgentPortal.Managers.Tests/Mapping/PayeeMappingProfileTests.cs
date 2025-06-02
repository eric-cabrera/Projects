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
    public class PayeeMappingProfileTests
    {
        public PayeeMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<ParticipantMappingProfile>();
                config.AddProfile<PayeeMappingProfile>();
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
        public void Payee_To_Payee_ShouldMap()
        {
            // Arrange
            var payee = PolicySourceTestData.ActivePolicy.Payee;

            // Act
            var mappedPayee = Mapper.Map<Payee>(payee);

            // Assert
            var primaryParticipant = PolicyDestinationTestData.PrimaryParticipant;
            var expectedPayee = new Payee
            {
                Participant = primaryParticipant
            };

            var compareResult = new CompareLogic()
                .Compare(expectedPayee, mappedPayee);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}