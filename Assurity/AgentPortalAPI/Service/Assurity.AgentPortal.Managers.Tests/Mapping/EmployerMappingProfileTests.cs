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
    public class EmployerMappingProfileTests
    {
        public EmployerMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<EmployerMappingProfile>();
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
        public void Employer_To_Employer_ShouldMap()
        {
            // Arrange
            var employer = PolicySourceTestData.Employer;

            // Act
            var mappedEmployer = Mapper.Map<Employer>(employer);

            // Assert
            var expectedEmployer = PolicyDestinationTestData.Employer;

            var compareResult = new CompareLogic()
                .Compare(expectedEmployer, mappedEmployer);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}