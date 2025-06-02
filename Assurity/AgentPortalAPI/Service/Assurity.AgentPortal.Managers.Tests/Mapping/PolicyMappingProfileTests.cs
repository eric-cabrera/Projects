namespace Assurity.AgentPortal.Managers.Tests.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
    using Assurity.AgentPortal.Managers.Tests.Mapping.TestData;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using KellermanSoftware.CompareNetObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class PolicyMappingProfileTests
    {
        public PolicyMappingProfileTests()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.AddProfile<AddressMappingProfile>();
                config.AddProfile<AgentMappingProfile>();
                config.AddProfile<AnnuitantMappingProfile>();
                config.AddProfile<AssigneeMappingProfile>();
                config.AddProfile<BeneficiaryMappingProfile>();
                config.AddProfile<BenefitMappingProfile>();
                config.AddProfile<BenefitOptionMappingProfile>();
                config.AddProfile<BusinessMappingProfile>();
                config.AddProfile<EmployerMappingProfile>();
                config.AddProfile<InsuredMappingProfile>();
                config.AddProfile<NameMappingProfile>();
                config.AddProfile<OwnerMappingProfile>();
                config.AddProfile<ParticipantMappingProfile>();
                config.AddProfile<PayeeMappingProfile>();
                config.AddProfile<PayorMappingProfile>();
                config.AddProfile<PersonMappingProfile>();
                config.AddProfile<PolicyMappingProfile>();
                config.AddProfile<RequirementMappingProfile>();
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
        public void Policy_To_Policy_ShouldMap()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.ActivePolicy;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [Fact]
        public void Policy_To_Policy_ActivePolicy_ShouldOmitTerminatedBenefits()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            Assert.NotNull(mappedPolicy);
            Assert.IsType<PolicyResponse>(mappedPolicy);
            Assert.Equal(2, mappedPolicy.Benefits.Count);
        }

        [Fact]
        public void Policy_To_Policy_TerminatedPolicy_ShouldIncludeTerminatedBenefits()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;
            policy.PolicyStatus = Status.Terminated;
            policy.TerminationDate = new DateTime(2023, 2, 1);

            // Act
            var mappedPolicy = Mapper.Map<Policy>(policy);

            // Assert
            Assert.NotNull(mappedPolicy);
            Assert.IsType<Policy>(mappedPolicy);
            Assert.Equal(3, mappedPolicy.Benefits.Count);
        }

        [Fact]
        public void Policy_To_Policy_ActivePolicy_ShouldOmitPastAndFutureBenefitOptions()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            Assert.NotNull(mappedPolicy);
            Assert.IsType<PolicyResponse>(mappedPolicy);
            Assert.Single(mappedPolicy.Benefits[1].BenefitOptions);
        }

        [Fact]
        public void Policy_With_LineOfBusiness_ImmediateAnnuity_To_Policy_ShouldMap_With_Payee()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;
            policy.LineOfBusiness = LineOfBusiness.ImmediateAnnuity;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.ActivePolicy;
            var primaryParticipant = PolicyDestinationTestData.PrimaryParticipant;
            expectedPolicy.LineOfBusiness = MappingExtensions.GetEnumDisplayName(LineOfBusiness.ImmediateAnnuity);
            expectedPolicy.Payee = new Payee
            {
                Participant = primaryParticipant
            };

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.NotNull(mappedPolicy.Payee);
            Assert.Equal(expectedPolicy.Payee.Participant.Person.Name.IndividualFirst, mappedPolicy.Payee.Participant.Person.Name.IndividualFirst);
            Assert.Equal(expectedPolicy.Payee.Participant.Person.Name.IndividualLast, mappedPolicy.Payee.Participant.Person.Name.IndividualLast);
            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [Fact]
        public void Policy_With_LineOfBusiness_Non_ImmediateAnnuity_To_Policy_ShouldMap_With_Null_Payee()
        {
            // Arrange
            var policy = PolicySourceTestData.ActivePolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.ActivePolicy;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
            Assert.Null(mappedPolicy.Payee);
        }

        [Fact]
        public void PendingPolicy_ShouldNotMapPaidToDate()
        {
            // Arrange
            var policy = PolicySourceTestData.PendingPolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.PendingPolicy;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
            Assert.Null(mappedPolicy.PaidToDate);
        }

        [Fact]
        public void PendingPolicy_StatusReasonNone_ShouldNotMap()
        {
            // Arrange
            var policy = PolicySourceTestData.PendingPolicy;
            policy.PolicyStatusReason = StatusReason.None;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.PendingPolicy;
            expectedPolicy.PolicyStatusReason = null;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
            Assert.Null(mappedPolicy.PolicyStatusReason);
        }

        [Fact]
        public void BillingDay_BillingFormDirect_ShouldMapNull()
        {
            // Arrange
            var policy = PolicySourceTestData.PendingPolicy;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.PendingPolicy;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
            Assert.Null(mappedPolicy.BillingDay);
        }

        [Fact]
        public void BillingDay_BillingFormDirect_ShouldMap()
        {
            // Arrange
            var policy = PolicySourceTestData.PendingPolicy;
            policy.BillingForm = BillingForm.CreditCard;

            // Act
            var mappedPolicy = Mapper.Map<PolicyResponse>(policy);

            // Assert
            var expectedPolicy = PolicyDestinationTestData.PendingPolicy;
            expectedPolicy.BillingForm = MappingExtensions.GetEnumDisplayName(BillingForm.CreditCard);
            expectedPolicy.BillingDay = 5;

            var compareResult = new CompareLogic()
                .Compare(expectedPolicy, mappedPolicy);

            Assert.True(compareResult.AreEqual, compareResult.DifferencesString);
            Assert.True(mappedPolicy.BillingDay == 5);
        }
    }
}