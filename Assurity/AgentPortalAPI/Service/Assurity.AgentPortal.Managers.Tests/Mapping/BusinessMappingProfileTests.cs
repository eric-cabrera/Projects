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
    public class BusinessMappingProfileTests
    {
        public BusinessMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<BusinessMappingProfile>();
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
        public void Business_To_Business_ShouldMap()
        {
            // Arrange
            var business = PolicySourceTestData.Employer.Business;

            // Act
            var mappedBusiness = Mapper.Map<Business>(business);

            // Assert
            var expectedBusiness = PolicyDestinationTestData.Employer.Business;

            var compareResult = new CompareLogic()
                .Compare(expectedBusiness, mappedBusiness);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}