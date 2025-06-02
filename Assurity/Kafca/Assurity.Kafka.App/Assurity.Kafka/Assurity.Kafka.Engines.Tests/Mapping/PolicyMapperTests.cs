namespace Assurity.Kafka.Engines.Tests.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PolicyMapperTests
    {
        private static PolicyMapper PolicyMapper => new PolicyMapper(null, null, null);

        [TestMethod]
        public void MapPolicy_AnnuityNull_EmployerNull_ShouldMap()
        {
            // Arrange
            var policyDto = GetDefaultTestPolicyDTO();
            var expectedPolicy = new Policy
            {
                PolicyNumber = "1234567890",
                CompanyCode = "01",
                BillingForm = BillingForm.Unknown,
                ProductCode = "AEPUWLNP",
                PolicyStatus = Status.Active,
                BillingMode = BillingMode.Monthly,
                BillingDay = 1,
                PaidToDate = new DateTime(2022, 01, 01),
                ModePremium = 1,
                AnnualPremium = 1,
                LineOfBusiness = LineOfBusiness.TraditionalLife,
                BillingStatus = BillingStatus.Active,
                BillingReason = BillingReason.PaidUp,
                ResidentState = State.NE,
                ApplicationDate = new DateTime(2022, 01, 01),
                ApplicationReceivedDate = new DateTime(2022, 01, 01)
            };

            // Act
            var mappedPolicy = PolicyMapper.MapPolicy(policyDto);

            // Assert
            Assert.AreEqual(policyDto.PolicyNumber, mappedPolicy.PolicyNumber);
            Assert.AreEqual(policyDto.CompanyCode, mappedPolicy.CompanyCode);
            Assert.AreEqual(expectedPolicy.BillingForm, mappedPolicy.BillingForm);
            Assert.AreEqual(policyDto.ProductCode, mappedPolicy.ProductCode);
            Assert.AreEqual(expectedPolicy.PolicyStatus, mappedPolicy.PolicyStatus);
            Assert.AreEqual(expectedPolicy.BillingMode, mappedPolicy.BillingMode);
            Assert.AreEqual(expectedPolicy.BillingDay, mappedPolicy.BillingDay);
            Assert.AreEqual(expectedPolicy.PaidToDate, mappedPolicy.PaidToDate);
            Assert.AreEqual(policyDto.ModePremium, mappedPolicy.ModePremium);
            Assert.AreEqual(policyDto.AnnualPremium, mappedPolicy.AnnualPremium);
            Assert.AreEqual(expectedPolicy.LineOfBusiness, mappedPolicy.LineOfBusiness);
            Assert.AreEqual(expectedPolicy.BillingStatus, mappedPolicy.BillingStatus);
            Assert.AreEqual(expectedPolicy.BillingReason, mappedPolicy.BillingReason);
            Assert.AreEqual(expectedPolicy.ReturnPaymentType, mappedPolicy.ReturnPaymentType);
            Assert.AreEqual(expectedPolicy.ResidentState, mappedPolicy.ResidentState);
            Assert.AreEqual(expectedPolicy.SubmitDate, mappedPolicy.SubmitDate);
            Assert.AreEqual(expectedPolicy.ApplicationDate, mappedPolicy.ApplicationDate);
            Assert.AreEqual(expectedPolicy.ApplicationReceivedDate, mappedPolicy.ApplicationReceivedDate);
        }

        [TestMethod]
        public void MapPolicy_AnnuityNull_EmployerPopulated_ShouldMap()
        {
            // Arrange
            var policyDto = GetDefaultTestPolicyDTO();
            policyDto.Employer = new EmployerDTO
            {
                GroupNumber = "123456",
                EmployerName = "Employer Name",
                BusinessEmailAddress = "just@biz.com",
                NameId = 51,
                StatusCode = 'A'
            };

            // Act
            var mappedPolicy = PolicyMapper.MapPolicy(policyDto);

            // Assert
            Assert.AreEqual(policyDto.Employer.GroupNumber, mappedPolicy.Employer.Number);
            Assert.AreEqual(policyDto.Employer.BusinessEmailAddress, mappedPolicy.Employer.Business.EmailAddress);
            Assert.AreEqual(policyDto.Employer.EmployerName, mappedPolicy.Employer.Business.Name.BusinessName);
            Assert.AreEqual(policyDto.Employer.NameId, mappedPolicy.Employer.Business.Name.NameId);
            Assert.IsNotNull(mappedPolicy.Employer.Status);
        }

        [TestMethod]
        public void MapPolicy_AnnuityNotNull_ShouldMap()
        {
            // Arrange
            var policyDto = GetDefaultTestPolicyDTO();
            policyDto.LineOfBusiness = "I";
            policyDto.AnnuityPolicy = new AnnuityPolicyDTO
            {
                IssueDate = 19990101,
                StatusCode = "T",
                StatusDate = 20001212,
                StatusReason = string.Empty,
                TaxQualification = "3"
            };

            // Act
            var mappedPolicy = PolicyMapper.MapPolicy(policyDto);

            // Assert
            Assert.AreEqual(StatusReason.None, mappedPolicy.PolicyStatusReason);
            Assert.AreEqual(policyDto.AnnuityPolicy.IssueDate.ToNullableDateTime(), mappedPolicy.IssueDate);
            Assert.AreEqual(policyDto.AnnuityPolicy.StatusCode.ToPolicyStatus(), mappedPolicy.PolicyStatus);
            Assert.AreEqual(
                policyDto.AnnuityPolicy.TaxQualification.ToTaxQualificationStatus("I"),
                mappedPolicy.TaxQualificationStatus);

            Assert.AreEqual(policyDto.AnnuityPolicy.StatusDate.ToNullableDateTime(), mappedPolicy.TerminationDate);
        }

        [TestMethod]
        public void MapPolicy_NewBusinessPendingNotNull_ShouldMap()
        {
            // Arrange
            var policyDto = GetDefaultTestPolicyDTO();
            policyDto.NewBusinessPending = new NewBusinessPendingDTO
            {
                RequirementDate = 19970505
            };

            // Act
            var mappedPolicy = PolicyMapper.MapPolicy(policyDto);

            // Assert
            Assert.AreEqual(policyDto.NewBusinessPending.RequirementDate.ToNullableDateTime(), mappedPolicy.SubmitDate);
        }

        [TestMethod]
        public void MapEmployer_ShouldMapEmployer()
        {
            // Arrange
            var employerDto = new EmployerDTO
            {
                GroupNumber = "123456",
                EmployerName = "Employer Name",
                BusinessEmailAddress = "just@biz.com",
                NameId = 51,
                StatusCode = 'A'
            };

            // Act
            var employer = PolicyMapper.MapEmployer(employerDto);

            // Assert
            Assert.AreEqual(employerDto.GroupNumber, employer.Number);
            Assert.AreEqual(employerDto.BusinessEmailAddress, employer.Business.EmailAddress);
            Assert.AreEqual(employerDto.EmployerName, employer.Business.Name.BusinessName);
            Assert.AreEqual(employerDto.NameId, employer.Business.Name.NameId);
            Assert.IsNotNull(employer.Status);
        }

        [TestMethod]
        public void MapEmployer_EmployerStatus_ShouldMap()
        {
            // Arrange
            var statusCodeToEmployerStatusDictionary = new Dictionary<char, EmployerStatus>
            {
                { 'A', EmployerStatus.Active },
                { 'S', EmployerStatus.Suspended },
                { 'T', EmployerStatus.Terminated }
            };

            // Act
            foreach (var (statusCode, employerStatus) in statusCodeToEmployerStatusDictionary)
            {
                var employerDto = new EmployerDTO
                {
                    StatusCode = statusCode
                };

                var employer = PolicyMapper.MapEmployer(employerDto);

                // Assert
                Assert.AreEqual(employerStatus, employer.Status);
            }
        }

        [TestMethod]
        public void MapBenefit_ShouldInvokeBenefitMapper()
        {
            // Arrange
            var lineOfBusiness = LineOfBusiness.TraditionalLife;
            var benefitDto = new BenefitDTO();
            var extendedKeyData = new List<ExtendedKeyLookupResult>();

            var mockBenefitMapper = new Mock<IBenefitMapper>();
            mockBenefitMapper
                .Setup(m => m.MapBenefit(lineOfBusiness, benefitDto, extendedKeyData))
                .Returns(new Benefit());

            var policyMapper = new PolicyMapper(mockBenefitMapper.Object, null, null);

            // Act
            var benefit = policyMapper.MapBenefit(lineOfBusiness, benefitDto, extendedKeyData);

            // Assert
            Assert.IsNotNull(benefit);
        }

        [TestMethod]
        public void GenerateHomeOfficeReviewRequirement_ShouldInvokeRequirementMapper()
        {
            // Arrange
            var mockRequirementMapper = new Mock<IRequirementsMapper>();
            mockRequirementMapper
                .Setup(m => m.GenerateHomeOfficeReviewRequirement())
                .Returns(new Requirement());

            var policyMapper = new PolicyMapper(null, null, mockRequirementMapper.Object);

            // Act
            var requirement = policyMapper.GenerateHomeOfficeReviewRequirement();

            // Assert
            Assert.IsNotNull(requirement);
        }

        [TestMethod]
        public void MapRequirements_ShouldInvokeRequirementMapper()
        {
            // Arrange
            var policyRequirements = new List<PolicyRequirement>();
            var requirementMappings = new List<RequirementMapping>();
            var participants = new List<ParticipantDTO>();
            var globalCommentData = new List<GlobalRequirementLookupResult>();
            var mockRequirementMapper = new Mock<IRequirementsMapper>();
            mockRequirementMapper
                .Setup(m => m.MapRequirements(policyRequirements, requirementMappings, participants, globalCommentData))
                .Returns(new List<Requirement>());

            var policyMapper = new PolicyMapper(null, null, mockRequirementMapper.Object);

            // Act
            var requirement = policyMapper.MapRequirements(
                policyRequirements,
                requirementMappings,
                participants,
                globalCommentData);

            // Assert
            Assert.IsNotNull(requirement);
        }

        [TestMethod]
        public void MapAgent_PolicyAgentDTO_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var policyAgentDto = new PolicyAgentDTO();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapAgent(policyAgentDto))
                .Returns(new Agent());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapAgent(policyAgentDto);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapAgent_JitAgent_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO();
            var nameDto = new JustInTimeAgentNameDTO();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapAgent(jitAgentDto, nameDto))
                .Returns(new Agent());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapAgent(jitAgentDto, nameDto);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapAgent_JitAgent_Pname_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO();
            var pname = new PNAME();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapAgent(jitAgentDto, pname))
                .Returns(new Agent());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapAgent(jitAgentDto, pname);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapAnnuitants_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapAnnuitants(participants))
                .Returns(new List<Annuitant>());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapAnnuitants(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapAssignee_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapAssignee(participants))
                .Returns(new Assignee());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapAssignee(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapBeneficiaries_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapBeneficiaries(participants))
                .Returns(new List<Beneficiary>());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapBeneficiaries(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapInsureds_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var benefits = new List<BenefitDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapInsureds(participants, benefits))
                .Returns(new List<Insured>());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapInsureds(participants, benefits);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapOwners_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapOwners(participants))
                .Returns(new List<Owner>());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapOwners(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapPayors_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapPayors(participants))
                .Returns(new List<Payor>());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapPayors(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MapPayee_ShouldInvokeParticipantMapper()
        {
            // Arrange
            var participants = new List<ParticipantDTO>();
            var mockParticipantMapper = new Mock<IParticipantMapper>();
            mockParticipantMapper
                .Setup(m => m.MapPayee(participants))
                .Returns(new Payee());

            var policyMapper = new PolicyMapper(null, mockParticipantMapper.Object, null);

            // Act
            var result = policyMapper.MapPayee(participants);

            // Assert
            Assert.IsNotNull(result);
        }

        private static PolicyDTO GetDefaultTestPolicyDTO()
        {
            return new PolicyDTO
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                ProductCode = "AEPUWLNP",
                ContractCode = "A",
                ContractDate = 20230808,
                ContractReason = "DC",
                BillingDate = 20220101,
                IssueDate = 20220101,
                ApplicationDate = 20220101,
                AppReceivedDate = 20220101,
                IssueState = "NE",
                ResidenceState = "NE",
                BillingForm = "NO",
                BillingMode = 1,
                PaidToDate = 20220101,
                ModePremium = 1,
                AnnualPremium = 1,
                LineOfBusiness = "L",
                BillingCode = "A",
                BillingReason = "PU",
                TaxQualifyCode = "3",
                GroupNumber = "3412467890",
            };
        }
    }
}
