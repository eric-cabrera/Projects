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
    public class BenefitMappingProfileTests
    {
        public BenefitMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<BenefitMappingProfile>();
                config.AddProfile<BenefitOptionMappingProfile>();
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
        public void Benefit_To_Benefit_ShouldMap()
        {
            // Arrange
            var benefit = PolicySourceTestData.ActivePolicy.Benefits.First();

            // Act
            var mappedBenefit = Mapper.Map<BenefitResponse>(benefit);

            // Assert
            var expectedBenefit = PolicyDestinationTestData.ActivePolicy.Benefits.First();

            var compareResult = new CompareLogic()
                .Compare(expectedBenefit, mappedBenefit);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}