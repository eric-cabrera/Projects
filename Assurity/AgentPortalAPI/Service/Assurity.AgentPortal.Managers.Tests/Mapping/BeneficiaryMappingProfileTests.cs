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
    public class BeneficiaryMappingProfileTests
    {
        public BeneficiaryMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<BeneficiaryMappingProfile>();
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
        public void Beneficiary_To_Beneficiary_ShouldMap()
        {
            // Arrange
            var beneficiary = PolicySourceTestData.ActivePolicy.Beneficiaries.First();

            // Act
            var mappedBeneficiary = Mapper.Map<Beneficiary>(beneficiary);

            // Assert
            var expectedBeneficiary = PolicyDestinationTestData.ActivePolicy.Beneficiaries.First();

            var compareResult = new CompareLogic()
                .Compare(expectedBeneficiary, mappedBeneficiary);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}