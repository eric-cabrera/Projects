namespace Assurity.Kafka.Accessors.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Tests.TestData;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LifeProAccessorTests
    {
        private readonly DbContextOptions<LifeProContext> lifeProInMemoryOptions =
            new DbContextOptionsBuilder<LifeProContext>()
            .UseInMemoryDatabase(databaseName: "LPDEVDatabase")
            .Options;

        private LifeProContext LifeProContext =>
            new LifeProContext(lifeProInMemoryOptions);

        [TestCleanup]
        public void Dispose()
        {
            LifeProContext.Dispose();
            LifeProContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetPolicyRelationships_PolicyFound_ShouldReturnPolicyRelationships()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var policyRelationships = await lifeProAccessor
                .GetPolicyRelationships(500001, 700001);

            // Assert
            Assert.IsNotNull(policyRelationships);
            Assert.AreEqual(1, policyRelationships.Count);
            Assert.AreEqual("01", policyRelationships[0].CompanyCode);
            Assert.AreEqual("5150198401", policyRelationships[0].PolicyNumber);
            Assert.AreEqual(3, policyRelationships[0].RelateCodes.Count);
            Assert.IsTrue(policyRelationships[0].RelateCodes.Contains("IN"));
            Assert.IsTrue(policyRelationships[0].RelateCodes.Contains("PA"));
            Assert.IsTrue(policyRelationships[0].RelateCodes.Contains("PO"));
        }

        [TestMethod]
        public async Task GetPolicyRelationships_PolicyNotFound_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var policyRelationships = await lifeProAccessor
                .GetPolicyRelationships(999999, 888888);

            // Assert
            Assert.IsNotNull(policyRelationships);
            Assert.AreEqual(0, policyRelationships.Count);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_Owner_RecordsFound_ShouldReturnPRELARecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER("018819851955", new List<string> { "PO", "O1", "ZC" });

            // Assert
            Assert.IsNotNull(prelaRecords);
            Assert.AreEqual(2, prelaRecords.Count);
            Assert.AreEqual(880001, prelaRecords[0].NAME_ID);
            Assert.AreEqual(880001, prelaRecords[1].NAME_ID);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_Payor_RecordsFound_ShouldReturnPRELARecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER("018819851955", new List<string> { "PA", "P1", "UL", "U2" });

            // Assert
            Assert.IsNotNull(prelaRecords);
            Assert.AreEqual(1, prelaRecords.Count);
            Assert.AreEqual(880001, prelaRecords[0].NAME_ID);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTERForBenefitsOnly_Insured_RecordsFound_ShouldReturnPRELARecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER("018819851955", new List<string> { "IN", "ML" });

            // Assert
            Assert.IsNotNull(prelaRecords);
            Assert.AreEqual(2, prelaRecords.Count);
            Assert.AreEqual(880001, prelaRecords[0].NAME_ID);
            Assert.AreEqual(880001, prelaRecords[1].NAME_ID);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_Insured_RecordsFound_ShouldReturnPRELARecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER("018819851955", new List<string> { "IN", "ML" });

            // Assert
            Assert.IsNotNull(prelaRecords);
            Assert.AreEqual(2, prelaRecords.Count);
            Assert.AreEqual(880001, prelaRecords[0].NAME_ID);
            Assert.AreEqual(880001, prelaRecords[1].NAME_ID);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS_RecordFound_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var identifyingAlpha = "01025150198450";

            // Act
            var prelaList = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(identifyingAlpha);

            // Assert
            Assert.IsNotNull(prelaList);
            Assert.AreEqual(0, prelaList.Count);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS_RecordNotFound_ShouldReturnPRELARecord()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var identifyingAlpha = "018819851955";

            // Act
            var prelaList = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(identifyingAlpha);

            // Assert
            Assert.IsNotNull(prelaList);
            Assert.AreEqual("018819851955", prelaList[0].IDENTIFYING_ALPHA);
            Assert.AreEqual("IN", prelaList[0].RELATE_CODE);
            Assert.AreEqual(1, prelaList[0].BENEFIT_SEQ_NUMBER);
            Assert.AreEqual(880001, prelaList[0].NAME_ID);

            Assert.AreEqual("018819851955", prelaList[1].IDENTIFYING_ALPHA);
            Assert.AreEqual("ML", prelaList[1].RELATE_CODE);
            Assert.AreEqual(2, prelaList[1].BENEFIT_SEQ_NUMBER);
            Assert.AreEqual(880001, prelaList[1].NAME_ID);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_BENEFIT_RecordFound_ShouldReturnPRELARecord()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819851955",
                    880001,
                    "ML",
                    2);

            // Assert
            Assert.IsNotNull(prela);
            Assert.AreEqual("018819851955", prela.IDENTIFYING_ALPHA);
            Assert.AreEqual(2, prela.BENEFIT_SEQ_NUMBER);
            Assert.AreEqual(880001, prela.NAME_ID);
            Assert.AreEqual("ML", prela.RELATE_CODE);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_BENEFIT_MissingRelatedPPBEN_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "8819852015",
                    880002,
                    "ML",
                    4);

            // Assert
            Assert.IsNull(prela);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_BENEFIT_PolicyNotFound_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await lifeProAccessor
                .GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
                    "9090909090",
                    909090,
                    "ML",
                    9);

            // Assert
            Assert.IsNull(prela);
        }

        [TestMethod]
        public async Task GetPCOMC_COMMISSION_CONTROL_TYPE_S_NoData_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await lifeProAccessor
                .GetAgents(
                    "01",
                    "9879879879");

            // Assert
            Assert.IsNotNull(agentRecords);
            Assert.AreEqual(0, agentRecords.Count);
        }

        [TestMethod]
        public async Task GetAgents_NoCommission_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();
            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await lifeProAccessor
                .GetAgents(
                    "01",
                    "1122334456");

            // Assert
            Assert.IsNotNull(agentRecords);
            Assert.AreEqual(1, agentRecords.Count);
        }

        [TestMethod]
        public async Task GetAgents_ServicingAgent_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await lifeProAccessor
                .GetAgents(
                    "01",
                    "1122334457");

            // Assert
            Assert.IsNotNull(agentRecords);
            Assert.AreEqual(1, agentRecords.Count);

            var agentRecord = agentRecords[0];

            Assert.AreEqual("1114", agentRecord.AGENT);
            Assert.AreEqual("90", agentRecord.AGENT_LEVEL);
            Assert.AreEqual("IS", agentRecord.MARKET_CODE);
            Assert.AreEqual("X", agentRecord.SERVICE_AGENT_IND);
            Assert.AreEqual(0, agentRecord.COMM_PCNT);
        }

        [TestMethod]
        public async Task GetPCOMC_COMMISSION_CONTROL_TYPE_S_HasData_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await lifeProAccessor
                .GetAgents(
                    "01",
                    "1122334455");

            // Assert
            Assert.IsNotNull(agentRecords);
            Assert.AreEqual(1, agentRecords.Count);

            var agentRecord = agentRecords[0];

            Assert.AreEqual("1111", agentRecord.AGENT);
            Assert.AreEqual("90", agentRecord.AGENT_LEVEL);
            Assert.AreEqual("IS", agentRecord.MARKET_CODE);
            Assert.AreEqual("X", agentRecord.SERVICE_AGENT_IND);
            Assert.AreEqual(100m, agentRecord.COMM_PCNT);
        }

        [TestMethod]
        public async Task GetPAGNT_AGENT_MASTER_HasData_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var pagntRecords = await lifeProAccessor.GetPAGNT_AGENT_MASTER("01", "00018");

            // Assert
            Assert.IsNotNull(pagntRecords);
            Assert.AreEqual(1, pagntRecords.Count);
            Assert.AreEqual(882100, pagntRecords[0].NAME_ID);
        }

        [TestMethod]
        public async Task GetPAGNT_AGENT_MASTER_NoData_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var pagntRecords = await lifeProAccessor.GetPAGNT_AGENT_MASTER("01", "00019");

            // Assert
            Assert.IsNotNull(pagntRecords);
            Assert.AreEqual(0, pagntRecords.Count);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByCOMCID_NoData_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                    .GetCompanyCodeAndPolicyNumberByCOMCID(555555);

            // Assert
            Assert.IsNull(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByCOMCID_HasData_ShouldReturnCompanyCodeAndPolicyNumber()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                    .GetCompanyCodeAndPolicyNumberByCOMCID(1234);

            // Assert
            Assert.IsNotNull(companyCodeAndPolicyNumber);
            Assert.AreEqual("01", companyCodeAndPolicyNumber.CompanyCode);
            Assert.AreEqual("1122334455", companyCodeAndPolicyNumber.PolicyNumber);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByPolicyNumber_NoData_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPolicyNumber("9717187219");

            // Assert
            Assert.IsNull(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByPolicyNumber_HasData_ShouldReturnCompanyCodeAndPolicyNumber()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPolicyNumber("5150198401");

            // Assert
            Assert.IsNotNull(companyCodeAndPolicyNumber);
            Assert.AreEqual("01", companyCodeAndPolicyNumber.CompanyCode);
            Assert.AreEqual("5150198401", companyCodeAndPolicyNumber.PolicyNumber);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByPBENID_NoData_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPBENID(818737);

            // Assert
            Assert.IsNull(companyCodeAndPolicyNumber);
        }

        [TestMethod]
        public async Task GetCompanyCodeAndPolicyNumberByPBENID_HasData_ShouldReturnCompanyCodeAndPolicyNumber()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var companyCodeAndPolicyNumber = await lifeProAccessor
                .GetCompanyCodeAndPolicyNumberByPBENID(54321);

            // Assert
            Assert.IsNotNull(companyCodeAndPolicyNumber);
            Assert.AreEqual("01", companyCodeAndPolicyNumber.CompanyCode);
            Assert.AreEqual("8819851955", companyCodeAndPolicyNumber.PolicyNumber);
        }

        [TestMethod]
        public async Task GetPolicyStatusDetail_HasData_ShouldReturnPolicyStatusDetail()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var policyStatusDetail = await lifeProAccessor
                .GetPolicyStatusDetail("5150198401", "01");

            // Assert
            Assert.IsNotNull(policyStatusDetail);
            Assert.AreEqual("LAPSED", policyStatusDetail);
        }

        [TestMethod]
        public async Task GetPolicyRequirementsForHealth_HasData_ShouldReturnPolicyRequirements()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "4150299379"
            };

            // Act
            var policyRequirements = await lifeProAccessor.GetPolicyRequirementsForHealth(companyCodeAndPolicyNumber);

            // Assert
            Assert.IsNotNull(policyRequirements);
            Assert.AreEqual(5, policyRequirements.Count);

            Assert.AreEqual("MEDICAL INFORMATION BUREAU", policyRequirements[0].Description);
            Assert.AreEqual(20081023, policyRequirements[0].AddedDate);
            Assert.AreEqual(20201129, policyRequirements[0].ObtainedDate);
            Assert.AreEqual(922226, policyRequirements[0].NameId);
            Assert.AreEqual(string.Empty, policyRequirements[0].LifeproComment);
            Assert.AreEqual("Y", policyRequirements[0].Status);
            Assert.AreEqual(1, policyRequirements[0].Id);
            Assert.AreEqual(0, policyRequirements[0].Ix);
            Assert.AreEqual("MIB", policyRequirements[0].ReqType);
            Assert.AreEqual(1, policyRequirements[0].ReqSequence);

            Assert.AreEqual("BALANCE OF 1ST PREMIUM", policyRequirements[3].Description);
            Assert.AreEqual(20081023, policyRequirements[3].AddedDate);
            Assert.AreEqual(20201129, policyRequirements[0].ObtainedDate);
            Assert.AreEqual(922226, policyRequirements[3].NameId);
            Assert.AreEqual("$148.62", policyRequirements[3].LifeproComment);
            Assert.AreEqual("Y", policyRequirements[3].Status);
            Assert.AreEqual(48, policyRequirements[3].Id);
            Assert.AreEqual(0, policyRequirements[3].Ix);
            Assert.AreEqual("BAL PREM", policyRequirements[3].ReqType);
            Assert.AreEqual(4, policyRequirements[3].ReqSequence);
        }

        [TestMethod]
        public async Task GetPolicyRequirementsForLife_HasData_ShouldReturnPolicyRequirements()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "2150941812"
            };

            // Act
            var policyRequirements = await lifeProAccessor.GetPolicyRequirementsForLife(companyCodeAndPolicyNumber);

            // Assert
            Assert.IsNotNull(policyRequirements);
            Assert.AreEqual(3, policyRequirements.Count);

            Assert.AreEqual("AGENT CONTRACTING", policyRequirements[0].Description);
            Assert.AreEqual(20190520, policyRequirements[0].AddedDate);
            Assert.AreEqual(20220330, policyRequirements[0].ObtainedDate);
            Assert.AreEqual(2454411, policyRequirements[0].NameId);
            Assert.AreEqual("C J DOLAN   43TD", policyRequirements[0].LifeproComment);
            Assert.AreEqual("N", policyRequirements[0].Status);
            Assert.AreEqual(4, policyRequirements[0].Id);
            Assert.AreEqual("AGT CONT", policyRequirements[0].ReqType);
            Assert.AreEqual(0, policyRequirements[0].ReqSequence);
            Assert.AreEqual(1, policyRequirements[0].Ix);

            Assert.AreEqual("SCRIPT CHECK", policyRequirements[1].Description);
            Assert.AreEqual(20190520, policyRequirements[1].AddedDate);
            Assert.AreEqual(20220330, policyRequirements[0].ObtainedDate);
            Assert.AreEqual(2454411, policyRequirements[1].NameId);
            Assert.AreEqual(string.Empty, policyRequirements[1].LifeproComment);
            Assert.AreEqual("N", policyRequirements[1].Status);
            Assert.AreEqual(2, policyRequirements[1].Id);
            Assert.AreEqual("SCRPTCHK", policyRequirements[1].ReqType);
            Assert.AreEqual(0, policyRequirements[1].ReqSequence);
            Assert.AreEqual(2, policyRequirements[1].Ix);

            Assert.AreEqual("MEDICAL INFORMATION BUREAU", policyRequirements[2].Description);
            Assert.AreEqual(20190520, policyRequirements[2].AddedDate);
            Assert.AreEqual(20220330, policyRequirements[0].ObtainedDate);
            Assert.AreEqual(2454411, policyRequirements[2].NameId);
            Assert.AreEqual(string.Empty, policyRequirements[2].LifeproComment);
            Assert.AreEqual("N", policyRequirements[2].Status);
            Assert.AreEqual(1, policyRequirements[2].Id);
            Assert.AreEqual("MIB", policyRequirements[2].ReqType);
            Assert.AreEqual(0, policyRequirements[2].ReqSequence);
            Assert.AreEqual(3, policyRequirements[2].Ix);
        }

        [TestMethod]
        public async Task GetUnderwritingClass_HasData_ShouldReturnPreferredNonTobacco()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var underwritingClass = await lifeProAccessor.GetUnderwritingClassDescription("RI07622030", "H");

            // Assert
            Assert.IsNotNull(underwritingClass);
            Assert.AreEqual("Preferred Non-Tobacco", underwritingClass);
        }

        [TestMethod]
        public async Task GetCoverageDescription_ShouldReturn_CoverageDescription()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var coverageDesscription = await lifeProAccessor.GetCoverageDescription("030MRI");

            // Assert
            Assert.IsNotNull(coverageDesscription);
            Assert.AreEqual("Mo Readjustment Income Rider on 030", coverageDesscription);
        }

        [TestMethod]
        public async Task GetCoverageDescription_ShouldReturn_Null()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var coverageDesscription = await lifeProAccessor.GetCoverageDescription("030MRIJJJ");

            // Assert
            Assert.IsNull(coverageDesscription);
        }

        [TestMethod]
        public void GetPaymentData_ShouldReturnPaymentData()
        {
            // Arrange
            var policyNumber = "5150198401";
            var companyCode = "01";
            var creditCodes = new List<int> { 2, 110, 121, 122, 123, 124, 127, 128, 129, 131, 132 };
            var expectedEntities = DataStoreAndLifeProTestData.GetPACTGEntities
                .Where(x =>
                    x.POLICY_NUMBER == policyNumber
                    && x.COMPANY_CODE == companyCode
                    && creditCodes.Contains(x.CREDIT_CODE))
                .ToList();

            var expectedDtos =
                expectedEntities
                .Select(x => new PaymentDTO
                {
                    CreditCode = x.CREDIT_CODE,
                    DebitCode = x.DEBIT_CODE,
                    EffectiveDate = x.EFFECTIVE_DATE,
                    ReversalCode = x.REVERSAL_CODE
                })
                .ToList();

            var mockDbContextFactory = GetMockLifeProDbContextFactory();
            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var paymentData = lifeProAccessor.GetPaymentData(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(paymentData);
            Assert.AreEqual(expectedEntities.Count, paymentData.Count);
            CollectionAssert.AreEquivalent(expectedDtos, paymentData);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_ShouldReturn_True()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = lifeProAccessor.IsInitialPaymentDeclined("5150198401", "01");

            // Assert
            Assert.IsTrue(isInitialPaymentDeclined);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = lifeProAccessor.IsInitialPaymentDeclined("6150198401", "01");

            // Assert
            Assert.IsFalse(isInitialPaymentDeclined);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_DifferentCancelReason_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = lifeProAccessor.IsInitialPaymentDeclined("7150198401", "01");

            // Assert
            Assert.IsFalse(isInitialPaymentDeclined);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordFound_ShouldReturn_True()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await lifeProAccessor.IsULBenefitInGracePeriod(277772);

            // Assert
            Assert.IsTrue(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordFound_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await lifeProAccessor.IsULBenefitInGracePeriod(277773);

            // Assert
            Assert.IsFalse(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordNotFound_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await lifeProAccessor.IsULBenefitInGracePeriod(277774);

            // Assert
            Assert.IsFalse(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task GetPastDuePolicies_PaidToDate_LowerThan_ComparisonDate_ShouldReturn_PastDuePolicies()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsTrue(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_PaidToDate_NotLowerThan_ComparisonDate_ShouldReturn_EmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230215;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsFalse(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_ContractCode_S_ShouldReturn_PastDuePolicies()
        {
            // Arrange
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "S",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };
            var mockDbContextFactory = GetMockLifeProDbContextFactoryWithoutSeedData(ppolcEntities);

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsTrue(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_ContractCode_NotAOrS_ShouldReturn_EmptyList()
        {
            // Arrange
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "P",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };

            var mockDbContextFactory = GetMockLifeProDbContextFactoryWithoutSeedData(ppolcEntities);

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsFalse(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_LineOfBusiness_A_ShouldReturn_EmptyList()
        {
            // Arrange
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "A",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };

            var mockDbContextFactory = GetMockLifeProDbContextFactoryWithoutSeedData(ppolcEntities);

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsFalse(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_BillingForm_LST_ShouldReturn_EmptyList()
        {
            // Arrange
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "LST",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };

            var mockDbContextFactory = GetMockLifeProDbContextFactoryWithoutSeedData(ppolcEntities);

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsFalse(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetPastDuePolicies_BillingReason_RU_ShouldReturn_EmptyList()
        {
            // Arrange
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "RU",
                    PAID_TO_DATE = 20230322,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };

            var mockDbContextFactory = GetMockLifeProDbContextFactoryWithoutSeedData(ppolcEntities);

            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);
            var comparisonDate = 20230515;

            // Act
            var pastDuePolicies = await lifeProAccessor.GetPastDuePolicyNumbers(comparisonDate);

            // Assert
            Assert.IsFalse(pastDuePolicies.Any());
        }

        [TestMethod]
        public async Task GetAnnuityPolicy_Terminated_StatusCode_ShouldReturn_AnnuityPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();
            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            var policyNumber = "123456789";
            var companyCode = "01";

            // Act
            var annuityPolicy = await lifeProAccessor.GetAnnuityPolicy(policyNumber, companyCode);

            // Arrange
            Assert.IsNotNull(annuityPolicy);
            Assert.AreEqual("123456789", annuityPolicy.POLICY_NUMBER);
            Assert.AreEqual("01", annuityPolicy.COMPANY_CODE);
            Assert.AreEqual("T", annuityPolicy.STATUS_CODE);
            Assert.AreEqual("DC", annuityPolicy.STATUS_REASON);
            Assert.AreEqual(20151202, annuityPolicy.STATUS_DATE);
            Assert.AreEqual(20011201, annuityPolicy.ISSUE_DATE);
            Assert.AreEqual("2", annuityPolicy.TAX_QUALIFICATION);
        }

        [TestMethod]
        public async Task GetAnnuityPolicy_Active_StatusCode_ShouldReturn_AnnuityPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockLifeProDbContextFactory();
            var lifeProAccessor = new LifeProAccessor(mockDbContextFactory.Object);

            var policyNumber = "345678912";
            var companyCode = "01";

            // Act
            var annuityPolicy = await lifeProAccessor.GetAnnuityPolicy(policyNumber, companyCode);

            // Arrange
            Assert.IsNotNull(annuityPolicy);
            Assert.AreEqual("345678912", annuityPolicy.POLICY_NUMBER);
            Assert.AreEqual("01", annuityPolicy.COMPANY_CODE);
            Assert.AreEqual("A", annuityPolicy.STATUS_CODE);
            Assert.AreEqual(string.Empty, annuityPolicy.STATUS_REASON);
            Assert.AreEqual(20071101, annuityPolicy.STATUS_DATE);
            Assert.AreEqual(20071101, annuityPolicy.ISSUE_DATE);
            Assert.AreEqual("0", annuityPolicy.TAX_QUALIFICATION);
        }

        private static void SeedData(LifeProContext lifeProContext)
        {
            lifeProContext.PPOLC.AddRange(
                DataStoreAndLifeProTestData.GetPPOLCEntities());

            lifeProContext.PPEND_NEW_BUSINESS_PENDING.AddRange(
                DataStoreAndLifeProTestData.GetPPENDNewBusinessPendingEntities);

            lifeProContext.PPEND_NEW_BUS_PEND_UNDERWRITING.AddRange(
                DataStoreAndLifeProTestData.GetPPENDNewBusPendUnderwritingEntities);

            lifeProContext.PNAME.AddRange(
                DataStoreAndLifeProTestData.PNAMEEntities);

            lifeProContext.PGRUP_GROUP_MASTER.AddRange(
                DataStoreAndLifeProTestData.PGRUPGroupMasterEntities);

            lifeProContext.PICDA_WAIVER_DETAILS.AddRange(
                DataStoreAndLifeProTestData.GetPICDAWaiverDetailsEntities);

            lifeProContext.PPOLM_POLICY_BENEFIT_MISC.AddRange(
                DataStoreAndLifeProTestData.GetPPOLMPolicyBenefitMiscEntities);

            lifeProContext.PAGNT_AGENT_MASTER.AddRange(
                DataStoreAndLifeProTestData.PAGNTAgentMasterEntities);

            lifeProContext.PRELA_RELATIONSHIP_MASTER.AddRange(
                DataStoreAndLifeProTestData.PRELARelationshipMasterEntities);

            lifeProContext.PCEXP_COVERAGE_EXPANSION.AddRange(
                DataStoreAndLifeProTestData.GetPCEXPCoverageExpansionEntities);

            lifeProContext.PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS.AddRange(
                DataStoreAndLifeProTestData.GetPCEXPCoverageExpansionUWCLSDetailsEntities);

            lifeProContext.PCOVR_PRODUCT_COVERAGES.AddRange(
                DataStoreAndLifeProTestData.GetPCOVR_PRODUCT_COVERAGEEntities());

            lifeProContext.PNALK.AddRange(
                DataStoreAndLifeProTestData.PNALKEntities);

            lifeProContext.PPBEN_POLICY_BENEFITS_TYPES_BF.AddRange(
                DataStoreAndLifeProTestData.PPBENPolicyBenefitsTypesBFEntities);

            lifeProContext.PPBEN_POLICY_BENEFITS.AddRange(
                DataStoreAndLifeProTestData.PPBENPolicyBenefitsEntities);

            lifeProContext.PCOMC_COMMISSION_CONTROL.AddRange(
                DataStoreAndLifeProTestData.PCOMCCommissionControlEntities);

            lifeProContext.PCOMC_COMMISSION_CONTROL_TYPE_S.AddRange(
                DataStoreAndLifeProTestData.PCOMCCommissionControlTypeSEntities);

            lifeProContext.PRQRM.AddRange(
                DataStoreAndLifeProTestData.GetPRQRMEntities);

            lifeProContext.PRQRMTBL.AddRange(
                DataStoreAndLifeProTestData.GetPRQRMTBLEntities);

            lifeProContext.PMEDR.AddRange(
                DataStoreAndLifeProTestData.GetPMEDREntities);

            lifeProContext.PACTG.AddRange(
                DataStoreAndLifeProTestData.GetPACTGEntities);

            lifeProContext.PBDRV.AddRange(
                DataStoreAndLifeProTestData.GetPBDRVEntities);

            lifeProContext.PACON_ANNUITY_POLICY.AddRange(
                DataStoreAndLifeProTestData.GetPACONAnnuityPolicyEntities);

            lifeProContext.SaveChanges();
        }

        private static void SeedDataPPOLC(LifeProContext lifeProContext, List<PPOLC> ppolcList)
        {
            lifeProContext.PPOLC.AddRange(ppolcList);

            lifeProContext.SaveChanges();
        }

        private IMock<IDbContextFactory<LifeProContext>> GetMockLifeProDbContextFactory()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<LifeProContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(LifeProContext);

            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContext())
                .Returns(LifeProContext);

            SeedData(LifeProContext);

            return mockDbContextFactory;
        }

        private IMock<IDbContextFactory<LifeProContext>> GetMockLifeProDbContextFactoryWithoutSeedData(List<PPOLC> ppolcEntities)
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<LifeProContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(LifeProContext);

            SeedDataPPOLC(LifeProContext, ppolcEntities);

            return mockDbContextFactory;
        }
    }
}