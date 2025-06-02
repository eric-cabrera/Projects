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
    public class BenefitOptionMappingProfileTests
    {
        public BenefitOptionMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
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
        public void BenefitOption_To_BenefitOption_ShouldMap()
        {
            // Arrange
            var benefitOption = PolicySourceTestData.ActivePolicy.Benefits.First()
                .BenefitOptions.First();

            // Act
            var mappedBenefitOption = Mapper.Map<BenefitOptionResponse>(benefitOption);

            // Assert
            var expectedBenefitOption = PolicyDestinationTestData.ActivePolicy.Benefits.First()
                .BenefitOptions.First();

            var compareResult = new CompareLogic()
                .Compare(expectedBenefitOption, mappedBenefitOption);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}