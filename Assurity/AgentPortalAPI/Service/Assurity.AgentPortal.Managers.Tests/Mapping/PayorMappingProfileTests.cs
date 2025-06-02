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
    public class PayorMappingProfileTests
    {
        public PayorMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<ParticipantMappingProfile>();
                config.AddProfile<PayorMappingProfile>();
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
        public void Payor_To_Payor_ShouldMap()
        {
            // Arrange
            var payor = PolicySourceTestData.ActivePolicy.Payors.First();

            // Act
            var mappedPayor = Mapper.Map<Payor>(payor);

            // Assert
            var expectedPayor = PolicyDestinationTestData.ActivePolicy.Payors.First();

            var compareResult = new CompareLogic()
                .Compare(expectedPayor, mappedPayor);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}