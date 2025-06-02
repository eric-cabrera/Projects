namespace Assurity.Kafka.Engines.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MigratePolicyEngineTests
    {
        private Mock<ILogger<MigratePolicyEngine>> mockLogger = new Mock<ILogger<MigratePolicyEngine>>();

        [TestMethod]
        public async Task GetPolicy_ReturnsPolicy()
        {
            // Arrange
            var policyNumber = "1234567890";
            var companyCode = "01";
            var expectedPolicyDto = new PolicyDTO();
            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);

            SetupPolicyDataMockReturns(mockDataStoreAccessor);

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyDTO(policyNumber, companyCode))
                .Returns(expectedPolicyDto);

            var participantDTOs = new List<ParticipantDTO>();
            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.GetRelateCodesExcludingBeneficiary()))
                .Returns(participantDTOs);

            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOsWithoutAddress(policyNumber, companyCode, RelateCodes.BeneficiaryRelateCodes))
                .Returns(participantDTOs);

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyAgentDTOs(policyNumber, companyCode))
                .Returns(new List<PolicyAgentDTO>());

            var kdefSegmentId = "024M090D";
            mockDataStoreAccessor
                .Setup(m => m.GetBenefitDTOs(policyNumber, companyCode))
                .Returns(new List<BenefitDTO>()
                {
                    new BenefitDTO
                    {
                        MultipleInsureds = new List<MultipleInsuredDTO>
                        {
                            new MultipleInsuredDTO
                            {
                                KdBenefitExtendedKeys = "0101",
                                KdDefSegmentId = kdefSegmentId,
                            }
                        }
                    }
                });

            var expectedExtendedKeyLookup = new KeyLookup
            {
                Identifier = kdefSegmentId,
                MaxOrdinal = 2,
                MaxKeyValue = 1
            };

            mockDataStoreAccessor
                .Setup(m => m.GetExtendedKeyData(It.Is<ExtendedKeysLookup>(
                    lookup => lookup.Lookups.Count == 1 && lookup.Lookups.Contains(expectedExtendedKeyLookup))))
                .Returns(new List<ExtendedKeyLookupResult>());

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPolicy(expectedPolicyDto))
                .Returns(new Policy
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
                    ReturnPaymentType = ReturnPaymentType.None,
                    ApplicationDate = new DateTime(2022, 01, 01),
                    ResidentState = State.NE,
                    SubmitDate = new DateTime(2022, 01, 01),
                    ApplicationReceivedDate = new DateTime(2022, 01, 01),
                    TerminationDate = null
                });

            mockPolicyMapper
                .Setup(m => m.MapAnnuitants(participantDTOs))
                .Returns(new List<Annuitant>());

            mockPolicyMapper
                .Setup(m => m.MapAssignee(participantDTOs))
                .Returns(new Assignee());

            mockPolicyMapper
                .Setup(m => m.MapBeneficiaries(participantDTOs))
                .Returns(new List<Beneficiary>());

            mockPolicyMapper
                .Setup(m => m.MapInsureds(participantDTOs, It.IsAny<List<BenefitDTO>>()))
                .Returns(new List<Insured>());

            mockPolicyMapper
                .Setup(m => m.MapOwners(participantDTOs))
                .Returns(new List<Owner>());

            mockPolicyMapper
                .Setup(m => m.MapPayee(participantDTOs))
                .Returns(new Payee());

            mockPolicyMapper
                .Setup(m => m.MapPayors(participantDTOs))
                .Returns(new List<Payor>());

            mockPolicyMapper
                .Setup(m => m.MapBenefit(It.IsAny<LineOfBusiness>(), It.IsAny<BenefitDTO>(), It.IsAny<List<ExtendedKeyLookupResult>>()))
                .Returns(new Benefit());

            mockPolicyMapper
                .Setup(m => m.MapAgent(It.IsAny<PolicyAgentDTO>()))
                .Returns(new Agent());

            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            mockEventsAccessor
                .Setup(m => m.GetRequirementMappings(It.IsAny<List<int>>()))
                .Returns(new List<RequirementMapping>());

            mockPolicyMapper
                .Setup(m => m.GenerateHomeOfficeReviewRequirement())
                .Returns(new Requirement());

            var expectedRequirement = new Requirement
            {
                Display = true,
                Status = RequirementStatus.Unmet
            };

            mockPolicyMapper
                .Setup(m => m.MapRequirements(
                    It.IsAny<List<PolicyRequirement>>(),
                    It.IsAny<List<RequirementMapping>>(),
                    It.IsAny<List<ParticipantDTO>>(),
                    It.IsAny<List<GlobalRequirementLookupResult>>()))
                .Returns(new List<Requirement> { expectedRequirement });

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(m => m.GetRequirementComments(It.IsAny<GlobalRequirementCommentsLookupDTO>()))
                .Returns(new List<GlobalRequirementLookupResult>());

            var policyEngine = new MigratePolicyEngine(
                mockDataStoreAccessor.Object,
                mockGlobalDataAccessor.Object,
                null,
                mockEventsAccessor.Object,
                GetMockConfigurationManager().Object,
                mockLogger.Object,
                mockPolicyMapper.Object);

            // Act
            (var result, var actualPolicy) = await policyEngine.GetPolicy(policyNumber, companyCode);

            // Assert
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
                ReturnPaymentType = ReturnPaymentType.None,
                Agents = new List<Agent>(),
                Annuitants = new List<Annuitant>(),
                Assignee = new Assignee(),
                Benefits = new List<Benefit> { new Benefit() },
                Beneficiaries = new List<Beneficiary>(),
                Insureds = new List<Insured>(),
                Owners = new List<Owner>(),
                Payors = new List<Payor>(),
                Payee = new Payee(),
                PolicyStatusDetail = PolicyStatusDetail.InsuredRequested,
                ProductDescription = "10 Y TERM",
                Requirements = new List<Requirement> { expectedRequirement },
                ResidentState = State.NE,
                SubmitDate = new DateTime(2022, 01, 01),
                ApplicationDate = new DateTime(2022, 01, 01),
                ApplicationReceivedDate = new DateTime(2022, 01, 01),
                TerminationDate = null
            };

            Assert.AreEqual(result, GetPolicyResult.Found);
            Assert.IsNotNull(actualPolicy);
            Assert.AreEqual("1234567890", actualPolicy.PolicyNumber);
            Assert.AreEqual(null, actualPolicy.TerminationDate);

            var compareResult = new CompareLogic().Compare(expectedPolicy, actualPolicy);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [TestMethod]
        public async Task GetPolicy_MostRecentCardDeclinedDate_Passed_RetentionDuration_Returns_Null()
        {
            // Arrange
            var policyNumber = "policy";
            var companyCode = "01";
            var expectedPolicyDto = new PolicyDTO();
            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            mockDataStoreAccessor
                .Setup(m => m.GetPolicyDTO(policyNumber, companyCode))
                .Returns(expectedPolicyDto);

            var mostRecentPaymentDate = 20230101;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto
            };

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(true);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPolicy(expectedPolicyDto))
                .Returns(new Policy
                {
                    PolicyNumber = policyNumber,
                    CompanyCode = companyCode
                });

            var policyEngine = new MigratePolicyEngine(
                mockDataStoreAccessor.Object,
                null,
                null,
                null,
                GetMockConfigurationManager().Object,
                mockLogger.Object,
                mockPolicyMapper.Object);

            // Act
            (var result, var actualPolicy) = await policyEngine.GetPolicy(policyNumber, companyCode);

            Assert.AreEqual(result, GetPolicyResult.HasInitialPaymentDeclinedThatIsBeyondRetentionDuration);
            Assert.IsNull(actualPolicy);
        }

        [TestMethod]
        public async Task GetPolicy_MostRecentCheckDraftDeclinedDate_Passed_RetentionDuration_Returns_Null()
        {
            // Arrange
            var policyNumber = "policy";
            var companyCode = "01";
            var expectedPolicyDto = new PolicyDTO();
            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);

            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = 20230101
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = 20220101
                };

            var checkDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 2,
                    EffectiveDate = 20230101
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto,
                checkDeclinedPaymentDto
            };

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(true);

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyDTO(policyNumber, companyCode))
                .Returns(expectedPolicyDto);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPolicy(expectedPolicyDto))
                .Returns(new Policy
                {
                    PolicyNumber = policyNumber,
                    CompanyCode = companyCode
                });

            var policyEngine = new MigratePolicyEngine(
                mockDataStoreAccessor.Object,
                null,
                null,
                null,
                GetMockConfigurationManager().Object,
                mockLogger.Object,
                mockPolicyMapper.Object);

            // Act
            (var result, var actualPolicy) = await policyEngine.GetPolicy(policyNumber, companyCode);

            Assert.AreEqual(result, GetPolicyResult.HasInitialPaymentDeclinedThatIsBeyondRetentionDuration);
            Assert.IsNull(actualPolicy);
        }

        [TestMethod]
        public async Task GetPolicy_MostRecentCardDeclinedDate_NotPassed_RetentionDuration_Returns_Policy()
        {
            // Arrange
            var policyNumber = "1234567890";
            var companyCode = "01";
            var expectedPolicyDto = new PolicyDTO();
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            SetupPolicyDataMockReturns(mockDataStoreAccessor);

            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.GetRelateCodesExcludingBeneficiary()))
                .Returns(new List<ParticipantDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOsWithoutAddress(policyNumber, companyCode, RelateCodes.BeneficiaryRelateCodes))
                .Returns(new List<ParticipantDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyDTO(policyNumber, companyCode))
                .Returns(expectedPolicyDto);

            var mostRecentPaymentDate = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + 10;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto
            };

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(true);

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyAgentDTOs(policyNumber, companyCode))
                .Returns(new List<PolicyAgentDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetBenefitDTOs(policyNumber, companyCode))
                .Returns(new List<BenefitDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetExtendedKeyData(It.IsAny<ExtendedKeysLookup>()))
                .Returns(new List<ExtendedKeyLookupResult>());

            var policyStatusDetail = "status";
            mockDataStoreAccessor
                .Setup(m => m.GetPolicyStatusDetail(policyNumber, companyCode))
                .ReturnsAsync(policyStatusDetail);

            mockDataStoreAccessor
                .Setup(m => m.GetBaseProductDescriptionByProductCode(It.IsAny<string>()))
                .ReturnsAsync(new ProductDescription { AltProdDesc = "10 Y TERM" });

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPolicy(expectedPolicyDto))
                .Returns(new Policy
                {
                    PolicyNumber = policyNumber,
                    CompanyCode = companyCode,
                    ApplicationDate = new DateTime(2022, 01, 01),
                });

            mockPolicyMapper
                .Setup(m => m.MapAnnuitants(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Annuitant>());

            mockPolicyMapper
                .Setup(m => m.MapAssignee(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new Assignee());

            mockPolicyMapper
                .Setup(m => m.MapBeneficiaries(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Beneficiary>());

            mockPolicyMapper
                .Setup(m => m.MapInsureds(It.IsAny<List<ParticipantDTO>>(), It.IsAny<List<BenefitDTO>>()))
                .Returns(new List<Insured>());

            mockPolicyMapper
                .Setup(m => m.MapOwners(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Owner>());

            mockPolicyMapper
                .Setup(m => m.MapPayee(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new Payee());

            mockPolicyMapper
                .Setup(m => m.MapPayors(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Payor>());

            mockPolicyMapper
                .Setup(m => m.MapBenefit(It.IsAny<LineOfBusiness>(), It.IsAny<BenefitDTO>(), It.IsAny<List<ExtendedKeyLookupResult>>()))
                .Returns(new Benefit());

            mockPolicyMapper
                .Setup(m => m.GenerateHomeOfficeReviewRequirement())
                .Returns(new Requirement());

            mockPolicyMapper
                .Setup(m => m.MapRequirements(
                    It.IsAny<List<PolicyRequirement>>(),
                    It.IsAny<List<RequirementMapping>>(),
                    It.IsAny<List<ParticipantDTO>>(),
                    It.IsAny<List<GlobalRequirementLookupResult>>()))
                .Returns(new List<Requirement>());

            mockPolicyMapper
                .Setup(m => m.MapAgent(It.IsAny<PolicyAgentDTO>()))
                .Returns(new Agent());

            mockEventsAccessor
                .Setup(m => m.GetRequirementMappings(It.IsAny<List<int>>()))
                .Returns(new List<RequirementMapping>());

            mockGlobalDataAccessor
                .Setup(m => m.GetRequirementComments(It.IsAny<GlobalRequirementCommentsLookupDTO>()))
                .Returns(new List<GlobalRequirementLookupResult>());

            var policyEngine = new MigratePolicyEngine(
                mockDataStoreAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object,
                mockEventsAccessor.Object,
                GetMockConfigurationManager().Object,
                mockLogger.Object,
                mockPolicyMapper.Object);

            // Act
            (var result, var actualPolicy) = await policyEngine.GetPolicy(policyNumber, companyCode);

            Assert.AreEqual(result, GetPolicyResult.Found);
            Assert.IsNotNull(actualPolicy);
        }

        [TestMethod]
        public async Task GetPolicy_MostRecentCheckDraftDeclinedDate_NotPassed_RetentionDuration_Returns_Policy()
        {
            // Arrange
            var policyNumber = "1234567890";
            var companyCode = "01";
            var expectedPolicyDto = new PolicyDTO();
            var mockDataStoreAccessor = new Mock<IDataStoreAccessor>(MockBehavior.Strict);
            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            var mockEventsAccessor = new Mock<IEventsAccessor>(MockBehavior.Strict);
            SetupPolicyDataMockReturns(mockDataStoreAccessor);

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyDTO(policyNumber, companyCode))
                .Returns(expectedPolicyDto);

            var mostRecentPaymentDate = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + 10;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto
            };

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(true);

            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.GetRelateCodesExcludingBeneficiary()))
                .Returns(new List<ParticipantDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetParticipantDTOsWithoutAddress(policyNumber, companyCode, RelateCodes.BeneficiaryRelateCodes))
                .Returns(new List<ParticipantDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetBenefitDTOs(policyNumber, companyCode))
                .Returns(new List<BenefitDTO>());

            mockDataStoreAccessor
                .Setup(m => m.GetExtendedKeyData(It.IsAny<ExtendedKeysLookup>()))
                .Returns(new List<ExtendedKeyLookupResult>());

            mockDataStoreAccessor
                .Setup(m => m.GetPolicyAgentDTOs(policyNumber, companyCode))
                .Returns(new List<PolicyAgentDTO>());

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPolicy(expectedPolicyDto))
                .Returns(new Policy
                {
                    PolicyNumber = policyNumber,
                    CompanyCode = companyCode,
                    ApplicationDate = new DateTime(2022, 01, 01),
                });

            mockPolicyMapper
                .Setup(m => m.MapAnnuitants(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Annuitant>());

            mockPolicyMapper
                .Setup(m => m.MapAssignee(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new Assignee());

            mockPolicyMapper
                .Setup(m => m.MapBeneficiaries(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Beneficiary>());

            mockPolicyMapper
                .Setup(m => m.MapInsureds(It.IsAny<List<ParticipantDTO>>(), It.IsAny<List<BenefitDTO>>()))
                .Returns(new List<Insured>());

            mockPolicyMapper
                .Setup(m => m.MapOwners(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Owner>());

            mockPolicyMapper
                .Setup(m => m.MapPayee(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new Payee());

            mockPolicyMapper
                .Setup(m => m.MapPayors(It.IsAny<List<ParticipantDTO>>()))
                .Returns(new List<Payor>());

            mockPolicyMapper
                .Setup(m => m.MapBenefit(It.IsAny<LineOfBusiness>(), It.IsAny<BenefitDTO>(), It.IsAny<List<ExtendedKeyLookupResult>>()))
                .Returns(new Benefit());

            mockPolicyMapper
                .Setup(m => m.MapAgent(It.IsAny<PolicyAgentDTO>()))
                .Returns(new Agent());

            mockEventsAccessor
                .Setup(m => m.GetRequirementMappings(It.IsAny<List<int>>()))
                .Returns(new List<RequirementMapping>());

            mockPolicyMapper
                .Setup(m => m.GenerateHomeOfficeReviewRequirement())
                .Returns(new Requirement());

            mockPolicyMapper
                .Setup(m => m.MapRequirements(
                    It.IsAny<List<PolicyRequirement>>(),
                    It.IsAny<List<RequirementMapping>>(),
                    It.IsAny<List<ParticipantDTO>>(),
                    It.IsAny<List<GlobalRequirementLookupResult>>()))
                .Returns(new List<Requirement>());

            mockGlobalDataAccessor
                .Setup(m => m.GetRequirementComments(It.IsAny<GlobalRequirementCommentsLookupDTO>()))
                .Returns(new List<GlobalRequirementLookupResult>());

            var policyEngine = new MigratePolicyEngine(
                mockDataStoreAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object,
                mockEventsAccessor.Object,
                GetMockConfigurationManager().Object,
                mockLogger.Object,
                mockPolicyMapper.Object);

            // Act
            (var result, var actualPolicy) = await policyEngine.GetPolicy(policyNumber, companyCode);

            Assert.AreEqual(result, GetPolicyResult.Found);
            Assert.IsNotNull(actualPolicy);
            Assert.IsInstanceOfType(actualPolicy, typeof(Policy));
        }

        private void SetupPolicyDataMockReturns(Mock<IDataStoreAccessor> mockDataStoreAccessor)
        {
            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPolicyStatusDetail(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("INSURED REQUESTED");

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetBaseProductDescriptionByProductCode(
                    It.IsAny<string>()))
                .ReturnsAsync(new ProductDescription { AltProdDesc = "10 Y TERM" });

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPPEND_NEW_BUSINESS_PENDING(
                    It.IsAny<string>()))
                .ReturnsAsync(new PPEND_NEW_BUSINESS_PENDING { POLICY_NUMBER = "1234567890", FACE_AMOUNT = 1200, REQUIREMENT_DATE = 20220101, PEND_ID = 1 });

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPolicyRequirementsForHealth(
                    It.IsAny<CompanyCodeAndPolicyNumber>()))
                .ReturnsAsync(new List<PolicyRequirement>
                {
                    new PolicyRequirement()
                });

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPolicyRequirementsForLife(
                    It.IsAny<CompanyCodeAndPolicyNumber>()))
                .ReturnsAsync(new List<PolicyRequirement>
                {
                    new PolicyRequirement()
                });

            var mostRecentPaymentDate = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + 10;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = 20220101
                };

            var checkDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 2,
                    EffectiveDate = 20220101
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto,
                checkDeclinedPaymentDto
            };

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(false);

            mockDataStoreAccessor
                .Setup(dataStoreAccessor => dataStoreAccessor.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);
        }

        private Mock<IConfigurationManager> GetMockConfigurationManager()
        {
            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager
                .Setup(m => m.InitialPaymentDeclinedRetentionDays)
                .Returns(45);

            return mockConfigurationManager;
        }
    }
}