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
    public class AssigneeMappingProfileTests
    {
        public AssigneeMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<AssigneeMappingProfile>();
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
        public void Assignee_To_Assignee_ShouldMap()
        {
            // Arrange
            var assignee = PolicySourceTestData.ActivePolicy.Assignee;

            // Act
            var mappedAssignee = Mapper.Map<Assignee>(assignee);

            // Assert
            var expectedAssignee = PolicyDestinationTestData.ActivePolicy.Assignee;

            var compareResult = new CompareLogic()
                .Compare(expectedAssignee, mappedAssignee);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }
    }
}