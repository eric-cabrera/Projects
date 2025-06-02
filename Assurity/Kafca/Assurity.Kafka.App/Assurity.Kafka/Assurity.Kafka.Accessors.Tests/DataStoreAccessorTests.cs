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
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Tests.TestData;
    using Assurity.Kafka.Utilities;
    using Assurity.Kafka.Utilities.Constants;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DataStoreAccessorTests
    {
        private readonly DbContextOptions<DataStoreContext> dataStoreInMemoryOptions =
            new DbContextOptionsBuilder<DataStoreContext>()
            .UseInMemoryDatabase(databaseName: "DataStoreDatabase")
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        private DataStoreContext DataStoreContext => new DataStoreContext(dataStoreInMemoryOptions);

        [TestCleanup]
        public void Dispose()
        {
            DataStoreContext.Dispose();
            DataStoreContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetPolicyDTO_AllNavigationPropertiesPopulated_ShouldReturnPolicyDTO()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var policyNumber = DataStoreAndLifeProTestData.PPOLCWithAllNavigationPropertiesPolicyNumber;

            // Act
            var policyDTO = dataStoreAccessor.GetPolicyDTO(policyNumber, "01");

            // Assert
            Assert.IsNotNull(policyDTO);

            var expectedPPOLC =
                DataStoreAndLifeProTestData
                .GetPPOLCWithNavigationProperties()
                .Single(p => p.POLICY_NUMBER == policyNumber);

            Assert.AreEqual(expectedPPOLC.COMPANY_CODE, policyDTO.CompanyCode);
            Assert.AreEqual(expectedPPOLC.POLICY_NUMBER, policyDTO.PolicyNumber);
            Assert.AreEqual(expectedPPOLC.GROUP_NUMBER, policyDTO.GroupNumber);
            Assert.AreEqual(expectedPPOLC.LINE_OF_BUSINESS, policyDTO.LineOfBusiness);
            Assert.AreEqual(expectedPPOLC.PRODUCT_CODE, policyDTO.ProductCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_CODE, policyDTO.ContractCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_REASON, policyDTO.ContractReason);
            Assert.AreEqual(expectedPPOLC.CONTRACT_DATE, policyDTO.ContractDate);
            Assert.AreEqual(expectedPPOLC.BILLING_DATE, policyDTO.BillingDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_DATE, policyDTO.IssueDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_STATE, policyDTO.IssueState);
            Assert.AreEqual(expectedPPOLC.RES_STATE, policyDTO.ResidenceState);
            Assert.AreEqual(expectedPPOLC.BILLING_CODE, policyDTO.BillingCode);
            Assert.AreEqual(expectedPPOLC.BILLING_MODE, policyDTO.BillingMode);
            Assert.AreEqual(expectedPPOLC.BILLING_FORM, policyDTO.BillingForm);
            Assert.AreEqual(expectedPPOLC.BILLING_REASON, policyDTO.BillingReason);
            Assert.AreEqual(expectedPPOLC.PAID_TO_DATE, policyDTO.PaidToDate);
            Assert.AreEqual(expectedPPOLC.POLICY_BILL_DAY, policyDTO.PolicyBillDay);
            Assert.AreEqual(expectedPPOLC.MODE_PREMIUM, policyDTO.ModePremium);
            Assert.AreEqual(expectedPPOLC.ANNUAL_PREMIUM, policyDTO.AnnualPremium);
            Assert.AreEqual(expectedPPOLC.APPLICATION_DATE, policyDTO.ApplicationDate);

            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_CODE, policyDTO.AnnuityPolicy.StatusCode);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_REASON, policyDTO.AnnuityPolicy.StatusReason);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_DATE, policyDTO.AnnuityPolicy.StatusDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.ISSUE_DATE, policyDTO.AnnuityPolicy.IssueDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.TAX_QUALIFICATION, policyDTO.AnnuityPolicy.TaxQualification);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.GROUP_NUMBER, policyDTO.Employer.GroupNumber);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_ID, policyDTO.Employer.NameId);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.STATUS_CODE, policyDTO.Employer.StatusCode);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_BUSINESS, policyDTO.Employer.EmployerName);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.BUSINESS_EMAIL_ADR, policyDTO.Employer.BusinessEmailAddress);

            Assert.IsNotNull(policyDTO.NewBusinessPending);
            Assert.AreEqual(expectedPPOLC.PPEND_NEW_BUSINESS_PENDING.REQUIREMENT_DATE, policyDTO.NewBusinessPending.RequirementDate);
        }

        [TestMethod]
        public void GetPolicyDTO_AllPropertiesPopulatedExceptGroup_ShouldReturnPolicyDTO()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var policyNumber = DataStoreAndLifeProTestData.PPOLCWithGroupNull;

            // Act
            var policyDTO = dataStoreAccessor.GetPolicyDTO(policyNumber, "01");

            // Assert
            Assert.IsNotNull(policyDTO);

            var expectedPPOLC =
                DataStoreAndLifeProTestData
                .GetPPOLCWithNavigationProperties()
                .Single(p => p.POLICY_NUMBER == policyNumber);

            Assert.AreEqual(expectedPPOLC.COMPANY_CODE, policyDTO.CompanyCode);
            Assert.AreEqual(expectedPPOLC.POLICY_NUMBER, policyDTO.PolicyNumber);
            Assert.AreEqual(expectedPPOLC.GROUP_NUMBER, policyDTO.GroupNumber);
            Assert.AreEqual(expectedPPOLC.LINE_OF_BUSINESS, policyDTO.LineOfBusiness);
            Assert.AreEqual(expectedPPOLC.PRODUCT_CODE, policyDTO.ProductCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_CODE, policyDTO.ContractCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_REASON, policyDTO.ContractReason);
            Assert.AreEqual(expectedPPOLC.CONTRACT_DATE, policyDTO.ContractDate);
            Assert.AreEqual(expectedPPOLC.BILLING_DATE, policyDTO.BillingDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_DATE, policyDTO.IssueDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_STATE, policyDTO.IssueState);
            Assert.AreEqual(expectedPPOLC.RES_STATE, policyDTO.ResidenceState);
            Assert.AreEqual(expectedPPOLC.BILLING_CODE, policyDTO.BillingCode);
            Assert.AreEqual(expectedPPOLC.BILLING_MODE, policyDTO.BillingMode);
            Assert.AreEqual(expectedPPOLC.BILLING_FORM, policyDTO.BillingForm);
            Assert.AreEqual(expectedPPOLC.BILLING_REASON, policyDTO.BillingReason);
            Assert.AreEqual(expectedPPOLC.PAID_TO_DATE, policyDTO.PaidToDate);
            Assert.AreEqual(expectedPPOLC.POLICY_BILL_DAY, policyDTO.PolicyBillDay);
            Assert.AreEqual(expectedPPOLC.MODE_PREMIUM, policyDTO.ModePremium);
            Assert.AreEqual(expectedPPOLC.ANNUAL_PREMIUM, policyDTO.AnnualPremium);
            Assert.AreEqual(expectedPPOLC.APPLICATION_DATE, policyDTO.ApplicationDate);

            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_CODE, policyDTO.AnnuityPolicy.StatusCode);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_REASON, policyDTO.AnnuityPolicy.StatusReason);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_DATE, policyDTO.AnnuityPolicy.StatusDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.ISSUE_DATE, policyDTO.AnnuityPolicy.IssueDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.TAX_QUALIFICATION, policyDTO.AnnuityPolicy.TaxQualification);

            Assert.IsNull(policyDTO.Employer);
        }

        [TestMethod]
        public void GetPolicyDTO_AllPropertiesPopulatedExceptAnnuity_ShouldReturnPolicyDTO()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var policyNumber = DataStoreAndLifeProTestData.PPOLCWithAnnuityNull;

            // Act
            var policyDTO = dataStoreAccessor.GetPolicyDTO(policyNumber, "01");

            // Assert
            Assert.IsNotNull(policyDTO);

            var expectedPPOLC =
                DataStoreAndLifeProTestData
                .GetPPOLCWithNavigationProperties()
                .Single(p => p.POLICY_NUMBER == policyNumber);

            Assert.AreEqual(expectedPPOLC.COMPANY_CODE, policyDTO.CompanyCode);
            Assert.AreEqual(expectedPPOLC.POLICY_NUMBER, policyDTO.PolicyNumber);
            Assert.AreEqual(expectedPPOLC.GROUP_NUMBER, policyDTO.GroupNumber);
            Assert.AreEqual(expectedPPOLC.LINE_OF_BUSINESS, policyDTO.LineOfBusiness);
            Assert.AreEqual(expectedPPOLC.PRODUCT_CODE, policyDTO.ProductCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_CODE, policyDTO.ContractCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_REASON, policyDTO.ContractReason);
            Assert.AreEqual(expectedPPOLC.CONTRACT_DATE, policyDTO.ContractDate);
            Assert.AreEqual(expectedPPOLC.BILLING_DATE, policyDTO.BillingDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_DATE, policyDTO.IssueDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_STATE, policyDTO.IssueState);
            Assert.AreEqual(expectedPPOLC.RES_STATE, policyDTO.ResidenceState);
            Assert.AreEqual(expectedPPOLC.BILLING_CODE, policyDTO.BillingCode);
            Assert.AreEqual(expectedPPOLC.BILLING_MODE, policyDTO.BillingMode);
            Assert.AreEqual(expectedPPOLC.BILLING_FORM, policyDTO.BillingForm);
            Assert.AreEqual(expectedPPOLC.BILLING_REASON, policyDTO.BillingReason);
            Assert.AreEqual(expectedPPOLC.PAID_TO_DATE, policyDTO.PaidToDate);
            Assert.AreEqual(expectedPPOLC.POLICY_BILL_DAY, policyDTO.PolicyBillDay);
            Assert.AreEqual(expectedPPOLC.MODE_PREMIUM, policyDTO.ModePremium);
            Assert.AreEqual(expectedPPOLC.ANNUAL_PREMIUM, policyDTO.AnnualPremium);
            Assert.AreEqual(expectedPPOLC.APPLICATION_DATE, policyDTO.ApplicationDate);

            Assert.IsNull(policyDTO.AnnuityPolicy);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.GROUP_NUMBER, policyDTO.Employer.GroupNumber);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_ID, policyDTO.Employer.NameId);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.STATUS_CODE, policyDTO.Employer.StatusCode);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_BUSINESS, policyDTO.Employer.EmployerName);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.BUSINESS_EMAIL_ADR, policyDTO.Employer.BusinessEmailAddress);
        }

        [TestMethod]
        public void GetPolicyDTO_AllPropertiesPopulatedExceptNewBusinessPending_ShouldReturnPolicyDTO()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var policyNumber = DataStoreAndLifeProTestData.PPOLCWithNewBusinessPendingNull;

            // Act
            var policyDTO = dataStoreAccessor.GetPolicyDTO(policyNumber, "01");

            Assert.IsNotNull(policyDTO);

            var expectedPPOLC =
                DataStoreAndLifeProTestData
                .GetPPOLCWithNavigationProperties()
                .Single(p => p.POLICY_NUMBER == policyNumber);

            Assert.AreEqual(expectedPPOLC.COMPANY_CODE, policyDTO.CompanyCode);
            Assert.AreEqual(expectedPPOLC.POLICY_NUMBER, policyDTO.PolicyNumber);
            Assert.AreEqual(expectedPPOLC.GROUP_NUMBER, policyDTO.GroupNumber);
            Assert.AreEqual(expectedPPOLC.LINE_OF_BUSINESS, policyDTO.LineOfBusiness);
            Assert.AreEqual(expectedPPOLC.PRODUCT_CODE, policyDTO.ProductCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_CODE, policyDTO.ContractCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_REASON, policyDTO.ContractReason);
            Assert.AreEqual(expectedPPOLC.CONTRACT_DATE, policyDTO.ContractDate);
            Assert.AreEqual(expectedPPOLC.BILLING_DATE, policyDTO.BillingDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_DATE, policyDTO.IssueDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_STATE, policyDTO.IssueState);
            Assert.AreEqual(expectedPPOLC.RES_STATE, policyDTO.ResidenceState);
            Assert.AreEqual(expectedPPOLC.BILLING_CODE, policyDTO.BillingCode);
            Assert.AreEqual(expectedPPOLC.BILLING_MODE, policyDTO.BillingMode);
            Assert.AreEqual(expectedPPOLC.BILLING_FORM, policyDTO.BillingForm);
            Assert.AreEqual(expectedPPOLC.BILLING_REASON, policyDTO.BillingReason);
            Assert.AreEqual(expectedPPOLC.PAID_TO_DATE, policyDTO.PaidToDate);
            Assert.AreEqual(expectedPPOLC.POLICY_BILL_DAY, policyDTO.PolicyBillDay);
            Assert.AreEqual(expectedPPOLC.MODE_PREMIUM, policyDTO.ModePremium);
            Assert.AreEqual(expectedPPOLC.ANNUAL_PREMIUM, policyDTO.AnnualPremium);
            Assert.AreEqual(expectedPPOLC.APPLICATION_DATE, policyDTO.ApplicationDate);

            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_CODE, policyDTO.AnnuityPolicy.StatusCode);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_REASON, policyDTO.AnnuityPolicy.StatusReason);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.STATUS_DATE, policyDTO.AnnuityPolicy.StatusDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.ISSUE_DATE, policyDTO.AnnuityPolicy.IssueDate);
            Assert.AreEqual(expectedPPOLC.PACON_ANNUITY_POLICY.TAX_QUALIFICATION, policyDTO.AnnuityPolicy.TaxQualification);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.GROUP_NUMBER, policyDTO.Employer.GroupNumber);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_ID, policyDTO.Employer.NameId);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.STATUS_CODE, policyDTO.Employer.StatusCode);

            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.NAME_BUSINESS, policyDTO.Employer.EmployerName);
            Assert.AreEqual(expectedPPOLC.PGRUP_GROUP_MASTER.PNAME.BUSINESS_EMAIL_ADR, policyDTO.Employer.BusinessEmailAddress);

            Assert.IsNull(policyDTO.NewBusinessPending);
        }

        [TestMethod]
        public void GetPolicyDTO_AllNavigationPropertiesNull_ShouldReturnPolicyDTO()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var policyNumber = DataStoreAndLifeProTestData.PPOLCWithNavigationPropertiesNull;

            // Act
            var policyDTO = dataStoreAccessor.GetPolicyDTO(policyNumber, "01");

            // Assert
            Assert.IsNotNull(policyDTO);

            var expectedPPOLC =
                DataStoreAndLifeProTestData
                .GetPPOLCWithNavigationProperties()
                .Single(p => p.POLICY_NUMBER == policyNumber);

            Assert.AreEqual(expectedPPOLC.COMPANY_CODE, policyDTO.CompanyCode);
            Assert.AreEqual(expectedPPOLC.POLICY_NUMBER, policyDTO.PolicyNumber);
            Assert.AreEqual(expectedPPOLC.GROUP_NUMBER, policyDTO.GroupNumber);
            Assert.AreEqual(expectedPPOLC.LINE_OF_BUSINESS, policyDTO.LineOfBusiness);
            Assert.AreEqual(expectedPPOLC.PRODUCT_CODE, policyDTO.ProductCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_CODE, policyDTO.ContractCode);
            Assert.AreEqual(expectedPPOLC.CONTRACT_REASON, policyDTO.ContractReason);
            Assert.AreEqual(expectedPPOLC.CONTRACT_DATE, policyDTO.ContractDate);
            Assert.AreEqual(expectedPPOLC.BILLING_DATE, policyDTO.BillingDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_DATE, policyDTO.IssueDate);
            Assert.AreEqual(expectedPPOLC.ISSUE_STATE, policyDTO.IssueState);
            Assert.AreEqual(expectedPPOLC.RES_STATE, policyDTO.ResidenceState);
            Assert.AreEqual(expectedPPOLC.BILLING_CODE, policyDTO.BillingCode);
            Assert.AreEqual(expectedPPOLC.BILLING_MODE, policyDTO.BillingMode);
            Assert.AreEqual(expectedPPOLC.BILLING_FORM, policyDTO.BillingForm);
            Assert.AreEqual(expectedPPOLC.BILLING_REASON, policyDTO.BillingReason);
            Assert.AreEqual(expectedPPOLC.PAID_TO_DATE, policyDTO.PaidToDate);
            Assert.AreEqual(expectedPPOLC.POLICY_BILL_DAY, policyDTO.PolicyBillDay);
            Assert.AreEqual(expectedPPOLC.MODE_PREMIUM, policyDTO.ModePremium);
            Assert.AreEqual(expectedPPOLC.ANNUAL_PREMIUM, policyDTO.AnnualPremium);
            Assert.AreEqual(expectedPPOLC.APPLICATION_DATE, policyDTO.ApplicationDate);

            Assert.IsNull(policyDTO.AnnuityPolicy);
            Assert.IsNull(policyDTO.Employer);
            Assert.IsNull(policyDTO.NewBusinessPending);
        }

        [TestMethod]
        public void GetRelationshipDto_ShouldReturnExpectedData()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var relationshipDto = dataStoreAccessor.GetParticipantDTOs(
                DataStoreAndLifeProTestData.PRELANavigationPropertiesPolicyNumber,
                DataStoreAndLifeProTestData.PRELANavigationPropertiesCompanyCode,
                RelateCodes.GetRelateCodesExcludingBeneficiary());

            // Assert
            Assert.IsNotNull(relationshipDto);
            var mappedRelationship = relationshipDto.Single();
            var expectedResult = DataStoreAndLifeProTestData.GetPRELAWithNavigationProperties().Single();
            Assert.AreEqual(expectedResult.RELATE_CODE, mappedRelationship.RelateCode);
            Assert.AreEqual(expectedResult.IDENTIFYING_ALPHA, mappedRelationship.IdentifyingAlpha);
            Assert.AreEqual(expectedResult.BENEFIT_SEQ_NUMBER, mappedRelationship.BenefitSequenceNumber);

            var expectedPnalk = expectedResult.PNAME.PNALKs.Single(pnalk => pnalk.CANCEL_DATE == 0 && pnalk.ADDRESS_CODE == string.Empty);
            var mappedAddress = mappedRelationship.Addresses.Single();
            Assert.AreEqual(expectedPnalk.PADDR.ADDRESS_ID, mappedAddress.AddressId);
            Assert.AreEqual(expectedPnalk.PADDR.ADDR_LINE_1, mappedAddress.Line1);
            Assert.AreEqual(expectedPnalk.PADDR.ADDR_LINE_2, mappedAddress.Line2);
            Assert.AreEqual(expectedPnalk.PADDR.ADDR_LINE_3, mappedAddress.Line3);
            Assert.AreEqual(expectedPnalk.TELE_NUM, mappedAddress.TelephoneNumber);
            Assert.AreEqual(expectedPnalk.PADDR.ZIP, mappedAddress.Zip);
            Assert.AreEqual(expectedPnalk.PADDR.ZIP_EXTENSION, mappedAddress.ZipExtension);
            Assert.AreEqual(expectedPnalk.PADDR.BOX_NUMBER, mappedAddress.BoxNumber);
            Assert.AreEqual(expectedPnalk.PADDR.CITY, mappedAddress.City);
            Assert.AreEqual(expectedPnalk.PADDR.COUNTRY, mappedAddress.Country);
            Assert.AreEqual(expectedPnalk.PADDR.STATE, mappedAddress.State);

            Assert.AreEqual(expectedResult.RELATE_CODE, mappedRelationship.RelateCode);
        }

        [TestMethod]
        public async Task GetPPOLC_NonTerminatedPolicies_ShouldReturn_Policy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policy = await dataStoreAccessor.GetPPOLC("5150198401", "01");

            // Assert
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public async Task GetPPOLC_PolicyTerminated1YearAgo_GroupNumberWhitespace_PolicyRecentlyTerminated_ShouldReturnPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policy = await dataStoreAccessor.GetPPOLC("8819852015", "01");

            // Assert
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public async Task GetPPOLC_PolicyTerminated1YearAgo_GroupNumberNotWhitespace_ShouldReturnPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policy = await dataStoreAccessor.GetPPOLC("8887776666", "01");

            // Assert
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public async Task GetPPOLC_PolicyTerminated4YearsAgo_GroupNumberWhitespace_ShouldNotReturnPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policy = await dataStoreAccessor.GetPPOLC("77776666888", "01");

            // Assert
            Assert.IsNull(policy);
        }

        [TestMethod]
        public async Task GetPPOLC_PolicyTerminated4YearsAgo_GroupNumberNotNull_ShouldReturnPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policy = await dataStoreAccessor.GetPPOLC("6666888777", "01");

            // Assert
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public void GetMigratablePPOLCRecordsAsync_ShouldReturnExpectedPolicies()
        {
            // Arrange
            var expectedPolicyNumbers = new List<string>
            {
                "8887776666",
                "8819852015",
                "8819851955",
                "5150198402",
                "5150198401",
                "6666888777",
            };

            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var results = dataStoreAccessor.GetMigratablePPOLCRecords();

            // Assert
            CollectionAssert.AreEquivalent(expectedPolicyNumbers, results.Select(r => r.PolicyNumber).ToList());
        }

        [TestMethod]
        public async Task GetPolicyRelationships_PolicyFound_ShouldReturnPolicyRelationships()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policyRelationships = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policyRelationships = await dataStoreAccessor
                .GetPolicyRelationships(999999, 888888);

            // Assert
            Assert.IsNotNull(policyRelationships);
            Assert.AreEqual(0, policyRelationships.Count);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_Owner_RecordsFound_ShouldReturnPRELARecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prelaRecords = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var identifyingAlpha = "01025150198450";

            // Act
            var prelaList = await dataStoreAccessor
                .GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(identifyingAlpha);

            // Assert
            Assert.IsNotNull(prelaList);
            Assert.AreEqual(0, prelaList.Count);
        }

        [TestMethod]
        public async Task GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS_RecordNotFound_ShouldReturnPRELARecord()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var identifyingAlpha = "018819851955";

            // Act
            var prelaList = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var prela = await dataStoreAccessor
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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await dataStoreAccessor
                .GetAgents(
                    "01",
                    "9879879879");

            // Assert
            Assert.IsNotNull(agentRecords);
            Assert.AreEqual(0, agentRecords.Count);
        }

        [TestMethod]
        public async Task GetAgents_ServicingAgent_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await dataStoreAccessor
                .GetAgents("1122334457", "01");

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
        public async Task GetAgents_ShouldReturnRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var agentRecords = await dataStoreAccessor
                .GetAgents("1122334455", "01");

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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var pagntRecords = await dataStoreAccessor.GetPAGNT_AGENT_MASTER("01", "00018");

            // Assert
            Assert.IsNotNull(pagntRecords);
            Assert.AreEqual(1, pagntRecords.Count);
            Assert.AreEqual(882100, pagntRecords[0].NAME_ID);
        }

        [TestMethod]
        public async Task GetPAGNT_AGENT_MASTER_NoData_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var pagntRecords = await dataStoreAccessor.GetPAGNT_AGENT_MASTER("01", "00019");

            // Assert
            Assert.IsNotNull(pagntRecords);
            Assert.AreEqual(0, pagntRecords.Count);
        }

        [TestMethod]
        public async Task GetPolicyStatusDetail_HasData_ShouldReturnPolicyStatusDetail()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var policyStatusDetail = await dataStoreAccessor
                .GetPolicyStatusDetail("5150198401", "01");

            // Assert
            Assert.IsNotNull(policyStatusDetail);
            Assert.AreEqual("LAPSED", policyStatusDetail);
        }

        [TestMethod]
        public async Task GetPolicyRequirementsForHealth_HasData_ShouldReturnPolicyRequirements()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "4150299379"
            };

            // Act
            var policyRequirements = await dataStoreAccessor.GetPolicyRequirementsForHealth(companyCodeAndPolicyNumber);

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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
            {
                CompanyCode = "01",
                PolicyNumber = "2150941812"
            };

            // Act
            var policyRequirements = await dataStoreAccessor.GetPolicyRequirementsForLife(companyCodeAndPolicyNumber);

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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var underwritingClass = await dataStoreAccessor.GetUnderwritingClassDescription("RI07622030", "H");

            // Assert
            Assert.IsNotNull(underwritingClass);
            Assert.AreEqual("Preferred Non-Tobacco", underwritingClass);
        }

        [TestMethod]
        public async Task GetCoverageDescription_ShouldReturn_CoverageDescription()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var coverageDesscription = await dataStoreAccessor.GetCoverageDescription("030MRI");

            // Assert
            Assert.IsNotNull(coverageDesscription);
            Assert.AreEqual("Mo Readjustment Income Rider on 030", coverageDesscription);
        }

        [TestMethod]
        public async Task GetCoverageDescription_ShouldReturn_Null()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var coverageDesscription = await dataStoreAccessor.GetCoverageDescription("030MRIJJJ");

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

            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var paymentData = dataStoreAccessor.GetPaymentData(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(paymentData);
            Assert.AreEqual(expectedEntities.Count, paymentData.Count);
            CollectionAssert.AreEquivalent(expectedDtos, paymentData);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_ShouldReturn_True()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = dataStoreAccessor.IsInitialPaymentDeclined("5150198401", "01");

            // Assert
            Assert.IsTrue(isInitialPaymentDeclined);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = dataStoreAccessor.IsInitialPaymentDeclined("6150198401", "01");

            // Assert
            Assert.IsFalse(isInitialPaymentDeclined);
        }

        [TestMethod]
        public void IsInitialPaymentDeclined_DifferentCancelReason_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isInitialPaymentDeclined = dataStoreAccessor.IsInitialPaymentDeclined("7150198401", "01");

            // Assert
            Assert.IsFalse(isInitialPaymentDeclined);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordFound_ShouldReturn_True()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await dataStoreAccessor.IsULBenefitInGracePeriod(277772);

            // Assert
            Assert.IsTrue(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordFound_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await dataStoreAccessor.IsULBenefitInGracePeriod(277773);

            // Assert
            Assert.IsFalse(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task IsULBenefitInGracePeriod_RecordNotFound_ShouldReturn_False()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();

            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var isULBenefitInGracePeriod = await dataStoreAccessor.IsULBenefitInGracePeriod(277774);

            // Assert
            Assert.IsFalse(isULBenefitInGracePeriod);
        }

        [TestMethod]
        public async Task GetAnnuityPolicy_Terminated_StatusCode_ShouldReturn_AnnuityPolicy()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            var policyNumber = "123456789";
            var companyCode = "01";

            // Act
            var annuityPolicy = await dataStoreAccessor.GetAnnuityPolicy(policyNumber, companyCode);

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
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            var policyNumber = "345678912";
            var companyCode = "01";

            // Act
            var annuityPolicy = await dataStoreAccessor.GetAnnuityPolicy(policyNumber, companyCode);

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

        [TestMethod]
        public async Task AddSystemDataLoadWithAgentHierarchyChange_ShouldAddRecords()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            var phier = new PHIER_AGENT_HIERARCHY
            {
                COMPANY_CODE = "01",
                AGENT_NUM = "BBC2",
                MARKET_CODE = "IS",
                AGENT_LEVEL = "60",
                STOP_DATE = 20230920,
                START_DATE = 20230120,
                HIERARCHY_AGENT = "BBC1",
                HIER_MARKET_CODE = "IS",
                HIER_AGENT_LEVEL = "90"
            };

            var changeType = ChangeType.Create;
            var beforeAgentId = "BBC2";

            // Act
            await dataStoreAccessor.AddSystemDataLoadWithAgentHierarchyChange(
                phier,
                changeType,
                beforeAgentId);

            // Assert
            var systemDataLoad = DataStoreContext.SystemDataLoads
                .Include(o => o.AgentHierarchyChange)
                .SingleOrDefault();

            Assert.IsNotNull(systemDataLoad);
            Assert.IsNotNull(systemDataLoad.InitiatedDate);

            // The InitiatedDate should've been set some minutes ahead
            Assert.IsTrue(systemDataLoad.InitiatedDate > DateTimeUtility.DateTimeNowCentral());

            Assert.IsNull(systemDataLoad.StartedDate);
            Assert.IsNull(systemDataLoad.FinishedDate);
            Assert.AreEqual(LoadType.Change, systemDataLoad.LoadType);
            Assert.AreEqual(SystemDataLoadStatus.Initiated, systemDataLoad.Status);

            var agentHierarchyChange = systemDataLoad.AgentHierarchyChange;

            Assert.IsNotNull(agentHierarchyChange);

            Assert.AreEqual(ChangeType.Create, agentHierarchyChange.ChangeType);
            Assert.AreEqual(phier.COMPANY_CODE, agentHierarchyChange.CompanyCode);
            Assert.AreEqual(phier.AGENT_NUM, agentHierarchyChange.AgentId);
            Assert.AreEqual(phier.MARKET_CODE, agentHierarchyChange.MarketCode);
            Assert.AreEqual(phier.AGENT_LEVEL, agentHierarchyChange.AgentLevel);
            Assert.AreEqual(phier.HIERARCHY_AGENT, agentHierarchyChange.UplineAgentId);
            Assert.AreEqual(phier.HIER_MARKET_CODE, agentHierarchyChange.UplineMarketCode);
            Assert.AreEqual(phier.HIER_AGENT_LEVEL, agentHierarchyChange.UplineAgentLevel);
            Assert.AreEqual(phier.START_DATE, agentHierarchyChange.StartDate);
            Assert.AreEqual(phier.STOP_DATE, agentHierarchyChange.StopDate);
            Assert.AreEqual(beforeAgentId, agentHierarchyChange.BeforeAgentId);
        }

        [TestMethod]
        public void GetBenefitDTOs_ShouldReturnResults()
        {
            // Arrange
            var testBenefits = DataStoreAndLifeProTestData.GetPPBEN_WithNavigationProperties();
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);

            // Act
            var benefitDTOs = dataStoreAccessor.GetBenefitDTOs(DataStoreAndLifeProTestData.PPBENNavigationPropertiesPolicyNumber, "01");

            // Assert
            Assert.IsNotNull(benefitDTOs);
            Assert.AreEqual(testBenefits.Count, benefitDTOs.Count);
            var ppbenRecord = testBenefits.Single();
            var benefitDTO = benefitDTOs.Single();
            Assert.AreEqual(ppbenRecord.POLICY_NUMBER, benefitDTO.PolicyNumber);
            Assert.AreEqual(ppbenRecord.COMPANY_CODE, benefitDTO.CompanyCode);
            Assert.AreEqual(ppbenRecord.BENEFIT_SEQ, benefitDTO.BenefitSequence);
            Assert.AreEqual(ppbenRecord.BENEFIT_TYPE, benefitDTO.BenefitType);
            Assert.AreEqual(ppbenRecord.STATUS_CODE, benefitDTO.StatusCode);
            Assert.AreEqual(ppbenRecord.STATUS_REASON, benefitDTO.StatusReason);
            Assert.AreEqual(ppbenRecord.STATUS_DATE, benefitDTO.StatusDate);
            Assert.AreEqual(ppbenRecord.STATUS_REASON, benefitDTO.StatusReason);
            Assert.AreEqual(ppbenRecord.PBEN_ID, benefitDTO.PBEN_ID);

            var specificAmountIncreaseData = ppbenRecord.PPBEN_POLICY_BENEFITS_TYPES_SP;
            Assert.IsNotNull(benefitDTO.SpecifiedAmountIncrease);
            Assert.AreEqual(specificAmountIncreaseData.NUMBER_OF_UNITS, benefitDTO.SpecifiedAmountIncrease.NumberOfUnits);
            Assert.AreEqual(specificAmountIncreaseData.ANN_PREM_PER_UNIT, benefitDTO.SpecifiedAmountIncrease.AnnualPremiumPerUnit);
            Assert.AreEqual(specificAmountIncreaseData.PBEN_ID, benefitDTO.SpecifiedAmountIncrease.PBEN_ID);
            Assert.AreEqual(specificAmountIncreaseData.VALUE_PER_UNIT, benefitDTO.SpecifiedAmountIncrease.ValuePerUnit);

            var supplementalData = ppbenRecord.PPBEN_POLICY_BENEFITS_TYPES_SU;
            Assert.IsNotNull(benefitDTO.Supplemental);
            Assert.AreEqual(supplementalData.NUMBER_OF_UNITS, benefitDTO.Supplemental.NumberOfUnits);
            Assert.AreEqual(supplementalData.ANN_PREM_PER_UNIT, benefitDTO.Supplemental.AnnualPremiumPerUnit);
            Assert.AreEqual(supplementalData.PBEN_ID, benefitDTO.Supplemental.PBEN_ID);
            Assert.AreEqual(supplementalData.VALUE_PER_UNIT, benefitDTO.Supplemental.ValuePerUnit);

            var slData = ppbenRecord.PPBEN_POLICY_BENEFITS_TYPES_SL;
            Assert.IsNotNull(benefitDTO.PolicyBenefitTypeSL);
            Assert.AreEqual(slData.NUMBER_OF_UNITS, benefitDTO.PolicyBenefitTypeSL.NumberOfUnits);
            Assert.AreEqual(slData.ANN_PREM_PER_UNIT, benefitDTO.PolicyBenefitTypeSL.AnnualPremiumPerUnit);
            Assert.AreEqual(slData.PBEN_ID, benefitDTO.PolicyBenefitTypeSL.PBEN_ID);
            Assert.AreEqual(slData.VALUE_PER_UNIT, benefitDTO.PolicyBenefitTypeSL.ValuePerUnit);

            var universalLifeData = ppbenRecord.PPBEN_POLICY_BENEFITS_TYPES_BF;
            Assert.IsNotNull(benefitDTO.BaseForUniversalLife);
            Assert.AreEqual(universalLifeData.NUMBER_OF_UNITS, benefitDTO.BaseForUniversalLife.NumberOfUnits);
            Assert.AreEqual(universalLifeData.ANN_PREM_PER_UNIT, benefitDTO.BaseForUniversalLife.AnnualPremiumPerUnit);
            Assert.AreEqual(universalLifeData.PBEN_ID, benefitDTO.BaseForUniversalLife.PBEN_ID);
            Assert.AreEqual(universalLifeData.VALUE_PER_UNIT, benefitDTO.BaseForUniversalLife.ValuePerUnit);
            Assert.AreEqual(universalLifeData.BF_CURRENT_DB, benefitDTO.BaseForUniversalLife.BF_CURRENT_DB);
            Assert.AreEqual(universalLifeData.BF_DATE_NEGATIVE, benefitDTO.BaseForUniversalLife.BF_DATE_NEGATIVE);
            Assert.AreEqual(universalLifeData.BF_DB_OPTION, benefitDTO.BaseForUniversalLife.BF_DB_OPTION);

            var baseOrRiderData = ppbenRecord.PPBEN_POLICY_BENEFITS_TYPES_BA_OR;
            Assert.IsNotNull(benefitDTO.BaseOrOtherRider);
            Assert.AreEqual(baseOrRiderData.NUMBER_OF_UNITS, benefitDTO.BaseOrOtherRider.NumberOfUnits);
            Assert.AreEqual(baseOrRiderData.ANN_PREM_PER_UNIT, benefitDTO.BaseOrOtherRider.AnnualPremiumPerUnit);
            Assert.AreEqual(baseOrRiderData.PBEN_ID, benefitDTO.BaseOrOtherRider.PBEN_ID);
            Assert.AreEqual(baseOrRiderData.VALUE_PER_UNIT, benefitDTO.BaseOrOtherRider.ValuePerUnit);
            Assert.AreEqual(baseOrRiderData.DIVIDEND, benefitDTO.BaseOrOtherRider.Dividend);

            var multipleInsuredData = ppbenRecord.PMUIN_MULTIPLE_INSUREDs.Single();
            Assert.IsNotNull(benefitDTO.MultipleInsureds);
            var multipleInsuredDto = benefitDTO.MultipleInsureds.Single();
            Assert.AreEqual(multipleInsuredData.NAME_ID, multipleInsuredDto.NameId);
            Assert.AreEqual(multipleInsuredData.MULT_RELATE, multipleInsuredDto.RelationshipToPrimaryInsured);
            Assert.AreEqual(multipleInsuredData.KD_BEN_EXTEND_KEYS.Trim(), multipleInsuredDto.KdBenefitExtendedKeys);
            Assert.AreEqual(multipleInsuredData.KD_DEF_SEGT_ID, multipleInsuredDto.KdDefSegmentId);
            Assert.AreEqual(multipleInsuredData.START_DATE, multipleInsuredDto.StartDate);
            Assert.AreEqual(multipleInsuredData.STOP_DATE, multipleInsuredDto.StopDate);
            Assert.AreEqual(multipleInsuredData.UWCLS, multipleInsuredDto.UnderwritingClass);

            var productCoverageData = ppbenRecord.PCOVR_PRODUCT_COVERAGES;
            Assert.IsNotNull(benefitDTO.ProductCoverages);
            Assert.AreEqual(productCoverageData.DESCRIPTION, benefitDTO.ProductCoverages.Description);

            var productExpansionData = ppbenRecord.PCEXP_COVERAGE_EXPANSION;
            Assert.IsNotNull(benefitDTO.CoverageExpansion);
            var iterator = 0;
            foreach (var detail in productExpansionData.PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS)
            {
                Assert.AreEqual(detail.UWCLS_CODE, benefitDTO.CoverageExpansion.Details[iterator].UnderwritingClassCode);
                Assert.AreEqual(detail.UWCLS_DESC, benefitDTO.CoverageExpansion.Details[iterator].UnderwritingClassDescription);
                iterator++;
            }
        }

        [TestMethod]
        public void GetExtendedKeyData_ShouldSucceed()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactory();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var identifier = "DI 100";
            short maxKey = 5;
            short maxOrdinal = 1;

            var extendedKeysLookup = new ExtendedKeysLookup
            {
                Lookups = new List<KeyLookup>
                {
                    new KeyLookup
                    {
                        Identifier = identifier,
                        MaxKeyValue = maxKey,
                        MaxOrdinal = maxOrdinal
                    }
                }
            };

            // Act
            var result = dataStoreAccessor.GetExtendedKeyData(extendedKeysLookup);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var singleLookup = result.Single();
            Assert.AreEqual(identifier, singleLookup.Identifier);
            Assert.IsTrue(singleLookup.Lookups.All(lookup => lookup.Key <= maxKey || lookup.Key <= maxOrdinal));
            Assert.IsTrue(singleLookup.Lookups.All(lookup => lookup.BenefitOrdinal <= maxOrdinal || lookup.BenefitOrdinal <= maxKey));
            Assert.IsTrue(singleLookup.Lookups.All(lookup => lookup.Value != null));
        }

        [TestMethod]
        public void GetPolicyAgentDTOs_ShouldSucceed()
        {
            // Arrange
            var mockDbContextFactory = GetMockDataStoreDbContextFactoryWithNavigationProperties();
            var dataStoreAccessor = new DataStoreAccessor(mockDbContextFactory.Object);
            var companyCode = "01";
            var policyNumber = "1122334456";

            // Act
            var agentDtos = dataStoreAccessor.GetPolicyAgentDTOs(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(agentDtos);

            var testData =
                DataStoreAndLifeProTestData.GetPCOMC_COMMISSION_CONTROLsWithNavigationProperties()
                .Where(data => data.POLICY_NUMBER == policyNumber)
                .ToList();

            Assert.AreEqual(testData.Count, agentDtos.Count);
            foreach (var testRecord in testData)
            {
                var matchingAgentDto = agentDtos.Single(a => a.AgentNumber == testRecord.PCOMC_COMMISSION_CONTROL_TYPE_S.AGENT);
                var pcomc_type_s = testRecord.PCOMC_COMMISSION_CONTROL_TYPE_S;
                foreach (var pagnt in testRecord.PCOMC_COMMISSION_CONTROL_TYPE_S.PAGNT_AGENT_MASTERs)
                {
                    Assert.AreEqual(pagnt.AGENT_NUMBER, matchingAgentDto.AgentNumber);
                    Assert.AreEqual(pcomc_type_s.SERVICE_AGENT_IND, matchingAgentDto.ServiceAgentIndicator);
                    Assert.AreEqual(pcomc_type_s.MARKET_CODE, matchingAgentDto.MarketCode);
                    Assert.AreEqual(pcomc_type_s.AGENT_LEVEL, matchingAgentDto.Level);
                    Assert.AreEqual(pcomc_type_s.COMM_PCNT, matchingAgentDto.CommissionPercent);

                    var pname = pagnt.PNAME;
                    Assert.AreEqual(pname.NAME_FORMAT_CODE, matchingAgentDto.Name.NameFormatCode);
                    Assert.AreEqual(pname.INDIVIDUAL_FIRST, matchingAgentDto.Name.IndividualFirst);
                    Assert.AreEqual(pname.INDIVIDUAL_MIDDLE, matchingAgentDto.Name.IndividualMiddle);
                    Assert.AreEqual(pname.INDIVIDUAL_LAST, matchingAgentDto.Name.IndividualLast);
                    Assert.AreEqual(pname.INDIVIDUAL_SUFFIX, matchingAgentDto.Name.IndividualSuffix);
                    Assert.AreEqual(pname.INDIVIDUAL_PREFIX, matchingAgentDto.Name.IndividualPrefix);
                    Assert.AreEqual(pname.PERSONAL_EMAIL_ADR, matchingAgentDto.Name.PersonalEmailAdress);
                    Assert.AreEqual(pname.BUSINESS_EMAIL_ADR, matchingAgentDto.Name.BusinessEmailAdress);
                    Assert.AreEqual(pname.NAME_BUSINESS, matchingAgentDto.Name.NameBusiness);
                }
            }
        }

        private static void SeedData(DataStoreContext dataStoreContext)
        {
            dataStoreContext.PPOLC.AddRange(
                DataStoreAndLifeProTestData.GetPPOLCEntities());

            dataStoreContext.PPEND_NEW_BUSINESS_PENDING.AddRange(
                DataStoreAndLifeProTestData.GetPPENDNewBusinessPendingEntities);

            dataStoreContext.PPEND_NEW_BUS_PEND_UNDERWRITING.AddRange(
                DataStoreAndLifeProTestData.GetPPENDNewBusPendUnderwritingEntities);

            dataStoreContext.PNAME.AddRange(
                DataStoreAndLifeProTestData.PNAMEEntities);

            dataStoreContext.PGRUP_GROUP_MASTER.AddRange(
                DataStoreAndLifeProTestData.PGRUPGroupMasterEntities);

            dataStoreContext.PICDA_WAIVER_DETAILS.AddRange(
                DataStoreAndLifeProTestData.GetPICDAWaiverDetailsEntities);

            dataStoreContext.PPOLM_POLICY_BENEFIT_MISC.AddRange(
                DataStoreAndLifeProTestData.GetPPOLMPolicyBenefitMiscEntities);

            dataStoreContext.PAGNT_AGENT_MASTER.AddRange(
                DataStoreAndLifeProTestData.PAGNTAgentMasterEntities);

            dataStoreContext.PRELA_RELATIONSHIP_MASTER.AddRange(
                DataStoreAndLifeProTestData.PRELARelationshipMasterEntities);

            dataStoreContext.PCEXP_COVERAGE_EXPANSION.AddRange(
                DataStoreAndLifeProTestData.GetPCEXPCoverageExpansionEntities);

            dataStoreContext.PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS.AddRange(
                DataStoreAndLifeProTestData.GetPCEXPCoverageExpansionUWCLSDetailsEntities);

            dataStoreContext.PCOVR_PRODUCT_COVERAGES.AddRange(
                DataStoreAndLifeProTestData.GetPCOVR_PRODUCT_COVERAGEEntities());

            dataStoreContext.PNALK.AddRange(
                DataStoreAndLifeProTestData.PNALKEntities);

            dataStoreContext.PPBEN_POLICY_BENEFITS_TYPES_BF.AddRange(
                DataStoreAndLifeProTestData.PPBENPolicyBenefitsTypesBFEntities);

            dataStoreContext.PPBEN_POLICY_BENEFITS.AddRange(
                DataStoreAndLifeProTestData.PPBENPolicyBenefitsEntities);

            dataStoreContext.PCOMC_COMMISSION_CONTROL.AddRange(
                DataStoreAndLifeProTestData.PCOMCCommissionControlEntities);

            dataStoreContext.PCOMC_COMMISSION_CONTROL_TYPE_S.AddRange(
                DataStoreAndLifeProTestData.PCOMCCommissionControlTypeSEntities);

            dataStoreContext.PRQRM.AddRange(
                DataStoreAndLifeProTestData.GetPRQRMEntities);

            dataStoreContext.PRQRMTBL.AddRange(
                DataStoreAndLifeProTestData.GetPRQRMTBLEntities);

            dataStoreContext.PMEDR.AddRange(
                DataStoreAndLifeProTestData.GetPMEDREntities);

            dataStoreContext.PACTG.AddRange(
                DataStoreAndLifeProTestData.GetPACTGEntities);

            dataStoreContext.PBDRV.AddRange(
                DataStoreAndLifeProTestData.GetPBDRVEntities);

            dataStoreContext.PACON_ANNUITY_POLICY.AddRange(
                DataStoreAndLifeProTestData.GetPACONAnnuityPolicyEntities);

            dataStoreContext.PKDEF_KEY_DEFINITION.AddRange(
                DataStoreAndLifeProTestData.PKDEF_KEY_DEFINITIONs);

            dataStoreContext.SaveChanges();
        }

        private static void SeedDataWithNavigationProperties(DataStoreContext dataStoreContext)
        {
            dataStoreContext.PPOLC.AddRange(
                DataStoreAndLifeProTestData.GetPPOLCWithNavigationProperties());

            dataStoreContext.PRELA_RELATIONSHIP_MASTER.AddRange(
                DataStoreAndLifeProTestData.GetPRELAWithNavigationProperties());

            dataStoreContext.PPBEN_POLICY_BENEFITS.AddRange(
                DataStoreAndLifeProTestData.GetPPBEN_WithNavigationProperties());

            dataStoreContext.PCOMC_COMMISSION_CONTROL.AddRange(
                DataStoreAndLifeProTestData.GetPCOMC_COMMISSION_CONTROLsWithNavigationProperties());

            dataStoreContext.SaveChanges();
        }

        private IMock<IDbContextFactory<DataStoreContext>> GetMockDataStoreDbContextFactory()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<DataStoreContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(DataStoreContext);

            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContext())
                .Returns(DataStoreContext);

            SeedData(DataStoreContext);

            return mockDbContextFactory;
        }

        private IMock<IDbContextFactory<DataStoreContext>> GetMockDataStoreDbContextFactoryWithNavigationProperties()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<DataStoreContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(DataStoreContext);

            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContext())
                .Returns(DataStoreContext);

            SeedDataWithNavigationProperties(DataStoreContext);

            return mockDbContextFactory;
        }
    }
}