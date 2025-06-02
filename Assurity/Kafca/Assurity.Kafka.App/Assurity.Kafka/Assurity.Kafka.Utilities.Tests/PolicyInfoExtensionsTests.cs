namespace Assurity.Kafka.Utilities.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PolicyInfoExtensionsTests
    {
        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeIsNull_ShouldDefaultToTerminatedBenefitStatus()
        {
            // Arrange
            string? statusCode = null;

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(null);

            // Assert
            Assert.AreEqual(Status.Terminated, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeA_ShouldReturnActiveBenefitStatus()
        {
            // Arrange
            var statusCode = "A";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(string.Empty);

            // Assert
            Assert.AreEqual(Status.Active, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeT_LifeProStatusReasonLP_ShouldReturnLapsedBenefitStatus()
        {
            // Arrange
            var statusCode = "T";
            var statusReason = "LP";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(statusReason);

            // Assert
            Assert.AreEqual(Status.Terminated, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeT_LifeProStatusReasonNotLP_ShouldReturnTerminatedBenefitStatus()
        {
            // Arrange
            var statusCode = "T";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(string.Empty);

            // Assert
            Assert.AreEqual(Status.Terminated, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeP_ShouldReturnPendingBenefitStatus()
        {
            // Arrange
            var statusCode = "P";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(string.Empty);

            // Assert
            Assert.AreEqual(Status.Pending, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeS_LifeProStatusReasonLP_ShouldReturnLapsedBenefitStatus()
        {
            // Arrange
            var statusCode = "S";
            var statusReason = "LP";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(statusReason);

            // Assert
            Assert.AreEqual(Status.Active, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeS_LifeProStatusReasonNotLP_ShouldReturnActiveBenefitStatus()
        {
            // Arrange
            var statusCode = "S";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(string.Empty);

            // Assert
            Assert.AreEqual(Status.Active, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatus_LifeProStatusCodeRandomText_ShouldDefaultToTerminatedBenefitStatus()
        {
            // Arrange
            var statusCode = "blargh";

            // Act
            var benefitStatus = statusCode.ToBenefitStatus(string.Empty);

            // Assert
            Assert.AreEqual(Status.Terminated, benefitStatus);
        }

        [TestMethod]
        public void ToBenefitStatusReason_Null_ShouldReturnNone()
        {
            // Arrange
            string? statusReason = null;

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_Empty_ShouldReturnNone()
        {
            // Arrange
            var statusReason = string.Empty;

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_BlankSpace_ShouldReturnNone()
        {
            // Arrange
            var statusReason = " ";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_BlankSpaces_ShouldReturnNone()
        {
            // Arrange
            var statusReason = "  ";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsRI_ShouldReturnReissued()
        {
            // Arrange
            var statusReason = "RI";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Reissued, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsRI_StatusCodeIsA_ShouldReturnReinstated()
        {
            // Arrange
            var statusReason = "RI";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason("A");

            // Assert
            Assert.AreEqual(StatusReason.Reinstated, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsRL_ShouldReturnReplacementPending()
        {
            // Arrange
            var statusReason = "RL";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.ReplacementPending, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsNN_ShouldReturnNotTaken()
        {
            // Arrange
            var statusReason = "NN";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.NotTaken, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsMB_ShouldReturnIncomplete()
        {
            // Arrange
            var statusReason = "MB";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Incomplete, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsDR_ShouldReturnDeclined()
        {
            // Arrange
            var statusReason = "DR";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Declined, benefitStatusReason);
        }

        [TestMethod]
        public void ToBenefitStatusReason_StatusReasonIsRandomText_ShouldReturnUnknown()
        {
            // Arrange
            var statusReason = "blargh";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Unknown, benefitStatusReason);
        }

        [TestMethod]
        public void ToCountry_USA_ShouldReturnUSA()
        {
            // Arrange
            var countryString = "USA";

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.AreEqual(Country.USA, country);
        }

        [TestMethod]
        public void ToCountry_USA_TrailingSpaces_ShouldReturnUSA()
        {
            // Arrange
            var countryString = "USA    ";

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.AreEqual(Country.USA, country);
        }

        [TestMethod]
        public void ToCountry_EmptyString_ShouldReturnNull()
        {
            // Arrange
            var countryString = string.Empty;

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.IsNull(country);
        }

        [TestMethod]
        public void ToCountry_NullString_ShouldReturnNull()
        {
            // Arrange
            string countryString = null;

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.IsNull(country);
        }

        [TestMethod]
        public void ToCountry_China_ShouldReturnChina()
        {
            // Arrange
            string countryString = "China";

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.IsNotNull(country);
            Assert.AreEqual(Country.CHINA, country);
        }

        [TestMethod]
        public void ToCountry_US_ShouldReturnUSA()
        {
            // Arrange
            string countryString = "US";

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.IsNotNull(country);
            Assert.AreEqual(Country.USA, country);
        }

        [TestMethod]
        public void ToCountry_US1234_ShouldReturnNull()
        {
            // Arrange
            string countryString = "US1234";

            // Act
            var country = countryString.ToCountry();

            // Assert
            Assert.IsNotNull(country);
            Assert.AreEqual(Country.UNKNOWN, country);
        }

        [TestMethod]
        public void ToDateTime_IntLength8_ShouldReturnDateTime()
        {
            // Arrange
            var lifeProDate = 20221121;

            // Act
            var dateTime = lifeProDate.ToNullableDateTime();

            // Assert
            Assert.AreEqual(new DateTime(2022, 11, 21), dateTime);
        }

        [TestMethod]
        public void ToDateTime_IntLengthLessThan8_ShouldReturnNull()
        {
            // Arrange
            var lifeProDate = 2022112;

            // Act
            var dateTime = lifeProDate.ToNullableDateTime();

            // Assert
            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToDateTime_IntLengthGreaterThan8_ShouldReturnNull()
        {
            // Arrange
            var lifeProDate = 202211211;

            // Act
            var dateTime = lifeProDate.ToNullableDateTime();

            // Assert
            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToIntDateMMDDYYYY_IntDateLength8_ShouldReturn_ConvertedIntDate()
        {
            // Arrange
            var lifeProDate = 5132023;

            // Act
            var convertedDate = lifeProDate.ToLifeProDateInteger();

            // Assert
            Assert.AreEqual(20230513, convertedDate);
        }

        [TestMethod]
        public void ToIntDateMMDDYYYY_IntDateLength7_ShouldReturn_ConvertedIntDate()
        {
            // Arrange
            var lifeProDate = 11272023;

            // Act
            var convertedDate = lifeProDate.ToLifeProDateInteger();

            // Assert
            Assert.AreEqual(20231127, convertedDate);
        }

        [TestMethod]
        public void ToIntDateMMDDYYYY_IntDateLengthLowerThan7_ShouldThrowException()
        {
            // Arrange
            var lifeProDate = 272023;

            // Act & Assert
            var exception =
                Assert.ThrowsException<Exception>(() => lifeProDate.ToLifeProDateInteger());

            Assert.AreEqual("Invalid LifePro Date.", exception.Message);
        }

        [TestMethod]
        public void ToNullableDateTime_ShouldReturnDateTime()
        {
            // Arrange
            var lifeProDate = 20221121;

            // Act
            var dateTime = lifeProDate.ToNullablePaidToDate(LineOfBusiness.Health);

            // Assert
            Assert.AreEqual(new DateTime(2022, 11, 21), dateTime);
        }

        [TestMethod]
        public void ToNullableDateTime_UniversalLife_LineOfBusiness_ShouldReturnNull()
        {
            // Arrange
            var lifeProDate = 20221121;

            // Act
            var dateTime = lifeProDate.ToNullablePaidToDate(LineOfBusiness.UniversalLife);

            // Assert
            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToNullableDateTime_Annuity_LineOfBusiness_ShouldReturnNull()
        {
            // Arrange
            var lifeProDate = 20221121;

            // Act
            var dateTime = lifeProDate.ToNullablePaidToDate(LineOfBusiness.Annuity);

            // Assert
            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToNullableDateTime_ImmediateAnnuity_LineOfBusiness_ShouldReturnNull()
        {
            // Arrange
            var lifeProDate = 20221121;

            // Act
            var dateTime = lifeProDate.ToNullablePaidToDate(LineOfBusiness.ImmediateAnnuity);

            // Assert
            Assert.IsNull(dateTime);
        }

        [TestMethod]
        public void ToGender_LifeProSexCodeF_ShouldReturnFemale()
        {
            // Arrange
            var lifeProSexCode = "F";

            // Act
            var gender = lifeProSexCode.ToGender();

            // Assert
            Assert.AreEqual(Gender.Female, gender);
        }

        [TestMethod]
        public void ToGender_LifeProSexCodeM_ShouldReturnMale()
        {
            // Arrange
            var lifeProSexCode = "M";

            // Act
            var gender = lifeProSexCode.ToGender();

            // Assert
            Assert.AreEqual(Gender.Male, gender);
        }

        [TestMethod]
        public void ToGender_RandomString_ShouldReturnUnknown()
        {
            // Arrange
            var lifeProSexCode = "blargh";

            // Act
            var gender = lifeProSexCode.ToGender();

            // Assert
            Assert.AreEqual(Gender.Unknown, gender);
        }

        [TestMethod]
        public void ToRelationshipToPrimaryInsured_Self_ShouldReturnPrimary()
        {
            // Arrange
            var insured = "SELF";

            // Act
            var relationshipToPrimaryInsured = insured.ToRelationshipToPrimaryInsured();

            // Assert
            Assert.AreEqual(RelationshipToPrimaryInsured.Self, relationshipToPrimaryInsured);
        }

        [TestMethod]
        public void ToRelationshipToPrimaryInsured_Spouse_ShouldReturnSpouse()
        {
            // Arrange
            var insured = "SPOUSE";

            // Act
            var relationshipToPrimaryInsured = insured.ToRelationshipToPrimaryInsured();

            // Assert
            Assert.AreEqual(RelationshipToPrimaryInsured.Spouse, relationshipToPrimaryInsured);
        }

        [TestMethod]
        public void ToRelationshipToPrimaryInsured_Child_ShouldReturnChild()
        {
            // Arrange
            var insured = "CHILD";

            // Act
            var relationshipToPrimaryInsured = insured.ToRelationshipToPrimaryInsured();

            // Assert
            Assert.AreEqual(RelationshipToPrimaryInsured.Child, relationshipToPrimaryInsured);
        }

        [TestMethod]
        public void ToRelationshipToPrimaryInsured_Null_ShouldDefaultToPrimary()
        {
            // Arrange
            string? insured = null;

            // Act
            var relationshipToPrimaryInsured = insured.ToRelationshipToPrimaryInsured();

            // Assert
            Assert.AreEqual(RelationshipToPrimaryInsured.Unknown, relationshipToPrimaryInsured);
        }

        [TestMethod]
        public void ToPayorType_RelateCodeIsPA_ShouldReturnPrimaryPayorType()
        {
            // Arrange
            var relateCode = "PA";

            // Act
            var payorType = relateCode.ToPayorType();

            // Assert
            Assert.AreEqual(PayorType.Primary, payorType);
        }

        [TestMethod]
        public void ToPayorType_RelateCodeIsP1_ShouldReturnAdditionalPayorType()
        {
            // Arrange
            var relateCode = "P1";

            // Act
            var payorType = relateCode.ToPayorType();

            // Assert
            Assert.AreEqual(PayorType.Additional, payorType);
        }

        [TestMethod]
        public void ToRelateCode_RelateCodeIsP0_ShouldReturnPrimary()
        {
            // Arrange
            var relateCode = "PO";

            // Act
            var ownerType = relateCode.ToRelateCode();

            // Assert
            Assert.AreEqual(OwnerType.Primary, ownerType);
        }

        [TestMethod]
        public void ToRelateCode_RelateCodeIsO1_ShouldReturnAdditional()
        {
            // Arrange
            var relateCode = "O1";

            // Act
            var ownerType = relateCode.ToRelateCode();

            // Assert
            Assert.AreEqual(OwnerType.Additional, ownerType);
        }

        [TestMethod]
        public void ToRelateCode_RelateCodeIsZC_ShouldReturnContingent()
        {
            // Arrange
            var relateCode = "ZC";

            // Act
            var ownerType = relateCode.ToRelateCode();

            // Assert
            Assert.AreEqual(OwnerType.Contingent, ownerType);
        }

        [TestMethod]
        public void ToAnnuitantType_A1_ShouldReturnPrimary()
        {
            // Arrange
            var relateCode = "A1";

            // Act
            var annuitantType = relateCode.ToAnnuitantType();

            // Assert
            Assert.AreEqual(AnnuitantType.Primary, annuitantType);
        }

        [TestMethod]
        public void ToAnnuitantType_A3_ShouldReturnPrimary()
        {
            // Arrange
            var relateCode = "A3";

            // Act
            var annuitantType = relateCode.ToAnnuitantType();

            // Assert
            Assert.AreEqual(AnnuitantType.Tertiary, annuitantType);
        }

        [TestMethod]
        public void ToBeneficiaryType_B1_ShouldReturnPrimary()
        {
            // Arrange
            var relateCode = "B1";

            // Act
            var beneficiaryType = relateCode.ToBeneficiaryType();

            // Assert
            Assert.AreEqual(BeneficiaryType.Primary, beneficiaryType);
        }

        [TestMethod]
        public void ToBeneficiaryType_J2_ShouldReturn_JointInsuredContingent()
        {
            // Arrange
            var relateCode = "J2";

            // Act
            var beneficiaryType = relateCode.ToBeneficiaryType();

            // Assert
            Assert.AreEqual(BeneficiaryType.JointInsuredContingent, beneficiaryType);
        }

        [TestMethod]
        public void ToBeneficiaryType_J2_ShouldReturnPrimary()
        {
            // Arrange
            var relateCode = "XT";

            // Act
            var beneficiaryType = relateCode.ToBeneficiaryType();

            // Assert
            Assert.AreEqual(BeneficiaryType.PerStirpesTertiary, beneficiaryType);
        }

        [TestMethod]
        public void ToOwnerType_PO_ShouldReturn_Primary()
        {
            // Arrange
            var relateCode = "PO";

            // Act
            var ownerType = relateCode.ToOwnerType();

            // Assert
            Assert.AreEqual(OwnerType.Primary, ownerType);
        }

        [TestMethod]
        public void ToOwnerType_O1_ShouldReturn_Additional()
        {
            // Arrange
            var relateCode = "O1";

            // Act
            var ownerType = relateCode.ToOwnerType();

            // Assert
            Assert.AreEqual(OwnerType.Additional, ownerType);
        }

        [TestMethod]
        public void ToOwnerType_ZC_ShouldReturn_Additional()
        {
            // Arrange
            var relateCode = "ZC";

            // Act
            var ownerType = relateCode.ToOwnerType();

            // Assert
            Assert.AreEqual(OwnerType.Contingent, ownerType);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeIsNull_ShouldDefaultToTerminatedPolicyStatus()
        {
            // Arrange
            string? contractCode = null;

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Terminated, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeA_ShouldReturnActivePolicyStatus()
        {
            // Arrange
            var contractCode = "A";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Active, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeT_LifeProContractReasonLP_ShouldReturnLapsedPolicyStatus()
        {
            // Arrange
            var contractCode = "T";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Terminated, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeT_LifeProContractReasonNotLP_ShouldReturnTerminatedPolicyStatus()
        {
            // Arrange
            var contractCode = "T";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Terminated, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeP_ShouldReturnPendingPolicyStatus()
        {
            // Arrange
            var contractCode = "P";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Pending, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeS_LifeProContractReasonLP_ShouldReturnLapsedPolicyStatus()
        {
            // Arrange
            var contractCode = "S";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Active, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeS_LifeProContractReasonNotLP_ShouldReturnActivePolicyStatus()
        {
            // Arrange
            var contractCode = "S";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Active, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatus_LifeProContractCodeRandomText_ShouldDefaultToTerminatedPolicyStatus()
        {
            // Arrange
            var contractCode = "blargh";

            // Act
            var policyStatus = contractCode.ToPolicyStatus();

            // Assert
            Assert.AreEqual(Status.Terminated, policyStatus);
        }

        [TestMethod]
        public void ToPolicyStatusReason_Null_ShouldReturnNone()
        {
            // Arrange
            string? contractReason = null;

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_Empty_ShouldReturnNone()
        {
            // Arrange
            var contractReason = string.Empty;

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_BlankSpace_ShouldReturnNone()
        {
            // Arrange
            var contractReason = " ";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_BlankSpaces_ShouldReturnNone()
        {
            // Arrange
            var contractReason = "  ";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.None, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsAE_ShouldReturnAccumAPLError()
        {
            // Arrange
            var contractReason = "AE";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.AccumAPLError, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsAL_ShouldReturnAllocationError()
        {
            // Arrange
            var contractReason = "AL";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.AllocationError, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsAM_ShouldReturnAmendment()
        {
            // Arrange
            var contractReason = "AM";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Amendment, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsAP_ShouldReturnApproved()
        {
            // Arrange
            var contractReason = "AP";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Approved, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsAU_ShouldReturnAutoNumbered()
        {
            // Arrange
            var contractReason = "AU";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.AutoNumbered, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsDF_ShouldReturnDeferredPolicy()
        {
            // Arrange
            var contractReason = "DF";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.DeferredPolicy, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsIB_ShouldReturnIneligibleNotMember()
        {
            // Arrange
            var contractReason = "IB";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.IneligibleNotMember, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsLN_ShouldReturnLoanProcessError()
        {
            // Arrange
            var contractReason = "LN";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.LoanProcessError, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsNA_ShouldReturnNotApproved()
        {
            // Arrange
            var contractReason = "NA";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.NotApproved, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsNQ_ShouldReturnQuestionableCostBasis()
        {
            // Arrange
            var contractReason = "NQ";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.QuestionableCostBasis, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsPT_ShouldReturnPreliminaryTerm()
        {
            // Arrange
            var contractReason = "PT";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.PreliminaryTerm, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsTP_ShouldReturnTerminationPending()
        {
            // Arrange
            var contractReason = "TP";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.TerminationPending, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsWD_ShouldReturnWaiverDisability()
        {
            // Arrange
            var contractReason = "WD";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.WaiverDisability, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIs2L_ShouldReturnTwoYearLookback()
        {
            // Arrange
            var contractReason = "2L";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.TwoYearLookback, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsRI_ShouldReturnReissued()
        {
            // Arrange
            var contractReason = "RI";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Reissued, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsRI_ContractCodeIsA_ShouldReturnReinstated()
        {
            // Arrange
            var contractReason = "RI";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason("A");

            // Assert
            Assert.AreEqual(StatusReason.Reinstated, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_StatusReasonIsRL_ShouldReturnReplacementPending()
        {
            // Arrange
            var statusReason = "RL";

            // Act
            var benefitStatusReason = statusReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.ReplacementPending, benefitStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_ContractReasonIsRandomText_ShouldReturnUnknown()
        {
            // Arrange
            var contractReason = "blargh";

            // Act
            var policyStatusReason = contractReason.ToPolicyStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Unknown, policyStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_StatusReasonIsNN_ShouldReturnNotTaken()
        {
            // Arrange
            var statusReason = "NN";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.NotTaken, benefitStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_StatusReasonIsMB_ShouldReturnIncomplete()
        {
            // Arrange
            var statusReason = "MB";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Incomplete, benefitStatusReason);
        }

        [TestMethod]
        public void ToPolicyStatusReason_StatusReasonIsDR_ShouldReturnDeclined()
        {
            // Arrange
            var statusReason = "DR";

            // Act
            var benefitStatusReason = statusReason.ToBenefitStatusReason(null);

            // Assert
            Assert.AreEqual(StatusReason.Declined, benefitStatusReason);
        }

        [TestMethod]
        public void ToState_StateAbbreviationIsAK_ShouldReturnAlaska()
        {
            // Arrange
            var stateAbbreviation = "AK";

            // Act
            var state = stateAbbreviation.ToState();

            // Assert
            Assert.AreEqual(State.AK, state);
        }

        [TestMethod]
        public void ToState_StateAbbreviationIsMN_ShouldReturnMinnesota()
        {
            // Arrange
            var stateAbbreviation = "MN";

            // Act
            var state = stateAbbreviation.ToState();

            // Assert
            Assert.AreEqual(State.MN, state);
        }

        [TestMethod]
        public void ToState_StateAbbreviationIsRandomText_ShouldReturnNull()
        {
            // Arrange
            var stateAbbreviation = "blargh";

            // Act
            var state = stateAbbreviation.ToState();

            // Assert
            Assert.IsNull(state);
        }

        [TestMethod]
        public void ToBillingForm_ShouldReturn_CreditCard()
        {
            // Arrange
            var billingFormStr = "CRD";

            // Act
            var billingForm = billingFormStr.ToBillingForm();

            // Assert
            Assert.AreEqual(BillingForm.CreditCard, billingForm);
        }

        [TestMethod]
        public void ToBillingForm_ShouldReturn_None()
        {
            // Arrange
            var billingFormStr = string.Empty;

            // Act
            var billingForm = billingFormStr.ToBillingForm();

            // Assert
            Assert.AreEqual(BillingForm.None, billingForm);
        }

        [TestMethod]
        public void ToBillingCode_ShouldReturn_HoldBilling()
        {
            // Arrange
            var billingCodeStr = "H";

            // Act
            var billingCode = billingCodeStr.ToBillingCode();

            // Assert
            Assert.AreEqual(BillingStatus.HoldBilling, billingCode);
        }

        [TestMethod]
        public void ToBillingCode_ShouldReturn_None()
        {
            // Arrange
            var billingCodeStr = string.Empty;

            // Act
            var billingCode = billingCodeStr.ToBillingCode();

            // Assert
            Assert.AreEqual(BillingStatus.None, billingCode);
        }

        [TestMethod]
        public void ToBillingReason_ShouldReturn_PaidUp()
        {
            // Arrange
            var billingReasonStr = "PU";

            // Act
            var billingReason = billingReasonStr.ToBillingReason();

            // Assert
            Assert.AreEqual(BillingReason.PaidUp, billingReason);
        }

        [TestMethod]
        public void ToBillingReason_ShouldReturn_None()
        {
            // Arrange
            var billingReasonStr = "MA";

            // Act
            var billingReason = billingReasonStr.ToBillingReason();

            // Assert
            Assert.AreEqual(BillingReason.None, billingReason);
        }

        [TestMethod]
        public void ToLineOfBusiness_ShouldReturn_TraditionalLife()
        {
            // Arrange
            var lineOfBusinessStr = "L";

            // Act
            var lineOfBusiness = lineOfBusinessStr.ToLineOfBusiness();

            // Assert
            Assert.AreEqual(LineOfBusiness.TraditionalLife, lineOfBusiness);
        }

        [TestMethod]
        public void ToTaxQualificationStatus_ShouldReturn_Sec457Plan()
        {
            // Arrange
            var taxQualificationStr = "7";

            // Act
            var taxQualificationStatus = taxQualificationStr.ToTaxQualificationStatus("L");

            // Assert
            Assert.AreEqual(TaxQualificationStatus.Sec457Plan, taxQualificationStatus);
        }

        [TestMethod]
        public void ToTaxQualificationStatus_ShouldReturn_Qualified()
        {
            // Arrange
            var taxQualificationStr = "Y";

            // Act
            var taxQualificationStatus = taxQualificationStr.ToTaxQualificationStatus("I");

            // Assert
            Assert.AreEqual(TaxQualificationStatus.Qualified, taxQualificationStatus);
        }

        [TestMethod]
        public void ToTaxQualificationStatus_ShouldReturn_Unknown()
        {
            // Arrange
            var taxQualificationStr = "Y";

            // Act
            var taxQualificationStatus = taxQualificationStr.ToTaxQualificationStatus("L");

            // Assert
            Assert.AreEqual(TaxQualificationStatus.Unknown, taxQualificationStatus);
        }

        [TestMethod]
        public void ToBillingMode_ShouldReturn_SemiAnnually()
        {
            // Arrange
            short billingModeVal = 6;

            // Act
            var billingMode = billingModeVal.ToBillingMode(null);

            // Assert
            Assert.AreEqual(BillingMode.SemiAnnually, billingMode);
        }

        [TestMethod]
        public void ToBillingMode_ShouldReturn_Ninthly()
        {
            // Arrange
            short billingModeVal = 1;

            // Act
            var billingMode = billingModeVal.ToBillingMode("G");

            // Assert
            Assert.AreEqual(BillingMode.Ninthly, billingMode);
        }

        [TestMethod]
        public void ToBillingMode_ShouldReturn_Tenthly()
        {
            // Arrange
            short billingModeVal = 1;

            // Act
            var billingMode = billingModeVal.ToBillingMode("U");

            // Assert
            Assert.AreEqual(BillingMode.Tenthly, billingMode);
        }

        [TestMethod]
        public void ToBillingMode_ShouldReturn_Monthly()
        {
            // Arrange
            short billingModeVal = 1;

            // Act
            var billingMode = billingModeVal.ToBillingMode(null);

            // Assert
            Assert.AreEqual(BillingMode.Monthly, billingMode);
        }

        [TestMethod]
        public void ToBenefitOptionName_ShouldReturn_AccidentExpenseNonEmployer()
        {
            // Arrange
            string benefitOptionNameStr = "AEHALFCOMP";

            // Act
            var benefitOptionName = benefitOptionNameStr.ToBenefitOptionName();

            // Assert
            Assert.AreEqual(BenefitOptionName.AccidentExpenseHalfCompensation, benefitOptionName);
        }

        [TestMethod]
        public void ToBenefitOptionName_ShouldReturn_Unknown()
        {
            // Arrange
            string benefitOptionNameStr = "Some Unknown Value";

            // Act
            var benefitOptionName = benefitOptionNameStr.ToBenefitOptionName();

            // Assert
            Assert.AreEqual(BenefitOptionName.Unknown, benefitOptionName);
        }

        [TestMethod]
        public void ToBenefitOptionName_ShouldReturn_HospitalConfinement()
        {
            // Arrange
            string benefitOptionNameStr = "Hospital Confinement";

            // Act
            var benefitOptionName = benefitOptionNameStr.ToBenefitOptionName();

            // Assert
            Assert.AreEqual(BenefitOptionName.HospitalConfinement, benefitOptionName);
        }

        [TestMethod]
        public void ToBenefitOptionName_ShouldReturn_CoverageType()
        {
            // Arrange
            string benefitOptionNameStr = "COVRAGE TYPE";

            // Act
            var benefitOptionName = benefitOptionNameStr.ToBenefitOptionName();

            // Assert
            Assert.AreEqual(BenefitOptionName.CoverageType, benefitOptionName);
        }

        [TestMethod]
        public void ToBenefitOptionName_ShouldReturn_BenefitAmount()
        {
            // Arrange
            string benefitOptionNameStr = "Own Occupation";

            // Act
            var benefitOptionName = benefitOptionNameStr.ToBenefitOptionName();

            // Assert
            Assert.AreEqual(BenefitOptionName.BenefitPeriod, benefitOptionName);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_HospitalIndemnityZeroCompensation()
        {
            // Arrange
            string benefitOptionValueStr = "HIZEROCOMP";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.HospitalIndemnityZeroCompensation, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_2Unit_100k40k10K()
        {
            // Arrange
            string benefitOptionValueStr = "2 Unit 100K/40K/10K";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.TwoUnits100K40K10K, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_CancerAndSpecifiedDisease()
        {
            // Arrange
            string benefitOptionValueStr = "Cancer & S/D";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.CancerAndSpecifiedDisease, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_EmployeeSpouse()
        {
            // Arrange
            string benefitOptionValueStr = "Employee / Spouse";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.EmployeeSpouse, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_Unknown()
        {
            // Arrange
            string benefitOptionValueStr = "5 y";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.Unknown, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_JointStandardNonTobacco()
        {
            // Arrange
            string benefitOptionValueStr = "Joint -S & N";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.JointStandardNonTobacco, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_MaleTobaccoFemaleNonTobacco()
        {
            // Arrange
            string benefitOptionValueStr = "MALE-T, FEMALE-N";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.MaleTobaccoFemaleNonTobacco, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_SelectNonTobaccoVegetarian()
        {
            // Arrange
            string benefitOptionValueStr = "HIQ Select+ Non-Tob Vege";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.SelectNonTobaccoVegetarian, benefitOptionValue);
        }

        [TestMethod]
        public void ToBenefitOptionValue_ShouldReturn_StandardNonTobacco()
        {
            // Arrange
            string benefitOptionValueStr = "S / NT";

            // Act
            var benefitOptionValue = benefitOptionValueStr.ToBenefitOptionValue();

            // Assert
            Assert.AreEqual(BenefitOptionValue.StandardNonTobacco, benefitOptionValue);
        }

        [TestMethod]
        public void ToDeathBenefitOptionValue_ShouldReturn_FaceAmountOption()
        {
            // Arrange
            string deathbenefitOptionValueStr = "1";

            // Act
            var deathBenefitOptionValue = deathbenefitOptionValueStr.ToDeathBenefitOption();

            // Assert
            Assert.AreEqual(DeathBenefitOption.FaceAmountOption, deathBenefitOptionValue);
        }

        [TestMethod]
        public void ToDeathBenefitOptionValue_ShouldReturn_None()
        {
            // Arrange
            string deathbenefitOptionValueStr = string.Empty;

            // Act
            var deathBenefitOptionValue = deathbenefitOptionValueStr.ToDeathBenefitOption();

            // Assert
            Assert.AreEqual(DeathBenefitOption.None, deathBenefitOptionValue);
        }

        [TestMethod]
        public void ToDeathBenefitOptionValue_ShouldReturn_Unknown()
        {
            // Arrange
            string deathbenefitOptionValueStr = " ";

            // Act
            var deathBenefitOptionValue = deathbenefitOptionValueStr.ToDeathBenefitOption();

            // Assert
            Assert.AreEqual(DeathBenefitOption.Unknown, deathBenefitOptionValue);
        }

        [TestMethod]
        public void ToDividendOptionValue_ShouldReturn_ReducePremium()
        {
            // Arrange
            string dividendOptionValueStr = "2";

            // Act
            var dividendOptionValue = dividendOptionValueStr.ToDividendOption();

            // Assert
            Assert.AreEqual(DividendOption.ReducePremium, dividendOptionValue);
        }

        [TestMethod]
        public void ToDividendOptionValue_ShouldReturn_None()
        {
            // Arrange
            string dividendOptionValueStr = string.Empty;

            // Act
            var dividendOptionValue = dividendOptionValueStr.ToDividendOption();

            // Assert
            Assert.AreEqual(DividendOption.None, dividendOptionValue);
        }

        [TestMethod]
        public void ToDividendOptionValue_ShouldReturn_Unknown()
        {
            // Arrange
            string dividendOptionValueStr = " ";

            // Act
            var dividendOptionValue = dividendOptionValueStr.ToDividendOption();

            // Assert
            Assert.AreEqual(DividendOption.Unknown, dividendOptionValue);
        }

        [TestMethod]
        public void ToDividendOptionValue_ShouldReturn_ReduceLoan()
        {
            // Arrange
            string dividendOptionValueStr = "6";

            // Act
            var dividendOptionValue = dividendOptionValueStr.ToDividendOption();

            // Assert
            Assert.AreEqual(DividendOption.ReduceLoan, dividendOptionValue);
        }

        [TestMethod]
        public void ToRequirementFulfillingParty_ShouldReturn_HomeOffice()
        {
            // Arrange
            string fulfillingPartyStr = "HomeOffice";

            // Act
            var fulfillingPartyValue = fulfillingPartyStr.ToRequirementFulfillingParty();

            // Assert
            Assert.AreEqual(RequirementFulfillingParty.HomeOffice, fulfillingPartyValue);
        }

        [TestMethod]
        public void ToRequirementFulfillingParty_ShouldReturn_Agent()
        {
            // Arrange
            string fulfillingPartyStr = "Agent";

            // Act
            var fulfillingPartyValue = fulfillingPartyStr.ToRequirementFulfillingParty();

            // Assert
            Assert.AreEqual(RequirementFulfillingParty.Agent, fulfillingPartyValue);
        }

        [TestMethod]
        public void ToRequirementAgentAction_ShouldReturn_UploadFile()
        {
            // Arrange
            string actionTypeStr = "UploadFile";

            // Act
            var actionTypeValue = actionTypeStr.ToRequirementActionType();

            // Assert
            Assert.AreEqual(RequirementActionType.UploadFile, actionTypeValue);
        }

        [TestMethod]
        public void ToRequirementAgentAction_ShouldReturn_UploadFileOrSendMessage()
        {
            // Arrange
            string actionTypeStr = "UploadFileOrSendMessage";

            // Act
            var actionTypeValue = actionTypeStr.ToRequirementActionType();

            // Assert
            Assert.AreEqual(RequirementActionType.UploadFileOrSendMessage, actionTypeValue);
        }

        [TestMethod]
        public void ToPhoneNumber_ShouldReturn_PhoneNumber()
        {
            // Arrange
            string phoneNumber = "1234567890";

            // Act
            var phoneNumberValue = phoneNumber.ToPhoneNumber();

            // Assert
            Assert.AreEqual("1234567890", phoneNumberValue);
        }

        [TestMethod]
        public void ToPhoneNumber_IsNotNumericShouldReturn_Null()
        {
            // Arrange
            string phoneNumber = "abc1234567";

            // Act
            var phoneNumberValue = phoneNumber.ToPhoneNumber();

            // Assert
            Assert.IsNull(phoneNumberValue);
        }

        [TestMethod]
        public void ToPhoneNumber_AllZerosShouldReturn_Null()
        {
            // Arrange
            string phoneNumber = "0000000000";

            // Act
            var phoneNumberValue = phoneNumber.ToPhoneNumber();

            // Assert
            Assert.IsNull(phoneNumberValue);
        }

        [TestMethod]
        public void ToPhoneNumber_InvalidLengthShouldReturn_Null()
        {
            // Arrange
            string phoneNumber = "12345678910";

            // Act
            var phoneNumberValue = phoneNumber.ToPhoneNumber();

            // Assert
            Assert.IsNull(phoneNumberValue);
        }

        [TestMethod]
        public void ToPhoneNumber_EmptyStringShouldReturn_Null()
        {
            // Arrange
            string phoneNumber = string.Empty;

            // Act
            var phoneNumberValue = phoneNumber.ToPhoneNumber();

            // Assert
            Assert.IsNull(phoneNumberValue);
        }

        [TestMethod]
        public void TrimStringProperties_ShouldTrimStringPropertiesOfObjectsOnly()
        {
            // Arrange
            var list = new List<PNALK>()
            {
                new PNALK
                {
                    ADDRESS_CODE = "123. CodeString    ",
                    ADDRESS_ID = 100,
                    CANCEL_DATE = 000,
                    TELE_NUM = "(312) 588-2300 ",
                    NAME_ID = 1000,
                },
                new PNALK
                {
                    ADDRESS_CODE = "123. CodeString2",
                    ADDRESS_ID = 1001,
                    CANCEL_DATE = 0001,
                    TELE_NUM = string.Empty,
                    NAME_ID = 1001,
                },
            };

            // Act
            PolicyInfoExtensions.TrimStringProperties(list);

            // Assert
            Assert.AreEqual(1000, list[0].NAME_ID);
            Assert.AreEqual("123. CodeString", list[0].ADDRESS_CODE);
            Assert.AreEqual(100, list[0].ADDRESS_ID);
            Assert.AreEqual(000, list[0].CANCEL_DATE);
            Assert.AreEqual("(312) 588-2300", list[0].TELE_NUM);

            Assert.AreEqual(1001, list[1].NAME_ID);
            Assert.AreEqual("123. CodeString2", list[1].ADDRESS_CODE);
            Assert.AreEqual(1001, list[1].ADDRESS_ID);
            Assert.AreEqual(0001, list[1].CANCEL_DATE);
            Assert.AreEqual(string.Empty, list[1].TELE_NUM);
        }

        [TestMethod]
        public void TrimStringProperties_ShouldTrimStringPropertiesOfObjectOnly()
        {
            // Arrange
            var obj = new PNAME
            {
                NAME_ID = 100,
                INDIVIDUAL_FIRST = "Thomas      ",
                INDIVIDUAL_MIDDLE = "U",
                INDIVIDUAL_LAST = "Ruth Ruth",
                INDIVIDUAL_PREFIX = string.Empty,
                INDIVIDUAL_SUFFIX = "3",
                PERSONAL_EMAIL_ADR = "email@email.com    ",
                SEX_CODE = "F"
            };

            // Act
            PolicyInfoExtensions.TrimStringProperties(obj);

            // Assert
            Assert.AreEqual(100, obj.NAME_ID);
            Assert.AreEqual("Thomas", obj.INDIVIDUAL_FIRST);
            Assert.AreEqual("U", obj.INDIVIDUAL_MIDDLE);
            Assert.AreEqual("Ruth Ruth", obj.INDIVIDUAL_LAST);
            Assert.AreEqual(string.Empty, obj.INDIVIDUAL_PREFIX);
            Assert.AreEqual("3", obj.INDIVIDUAL_SUFFIX);
            Assert.AreEqual("email@email.com", obj.PERSONAL_EMAIL_ADR);
        }

        [TestMethod]
        public void ToLifeProDate_ShouldConvertDateToInteger()
        {
            // Arrange
            var testDate = new DateTime(1986, 8, 2);

            // Act
            var lifeProDate = testDate.ToLifeProDate();

            // Assert
            Assert.AreEqual(19860802, lifeProDate);
        }
    }
}