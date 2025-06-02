namespace Assurity.Kafka.Engines.Tests.Mapping
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BenefitMapperTests
    {
        private IBenefitMapper BenefitMapper => new BenefitMapper();

        [TestMethod]
        public void MapBenefit_NoOptionData_BenefitSequence1_LineOfBusinessNotTraditionalOrUniversalLife_ShouldMapBenefitWithoutOptions_WithoutDividendOption_WithoutDeathBenefitOption_NoUnderwritingClassInfo_ShouldMapAsBaseBenefit()
        {
            // Arrange
            var benefitDto = GetDefaultTestBenefitDTO();

            // Act
            var result = BenefitMapper.MapBenefit(LineOfBusiness.TrueGroupCensus, benefitDto, new List<ExtendedKeyLookupResult>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.DividendOption);
            Assert.AreEqual(benefitDto.PlanCode, result.PlanCode);
            Assert.AreEqual(benefitDto.ProductCoverages.Description, result.BenefitDescription);
            Assert.AreEqual(benefitDto.PBEN_ID, result.BenefitId);
            Assert.AreEqual(benefitDto.StatusCode.ToBenefitStatus(benefitDto.StatusReason), result.BenefitStatus);
            Assert.AreEqual(benefitDto.StatusReason.ToBenefitStatusReason(benefitDto.StatusCode), result.BenefitStatusReason);
            Assert.AreEqual(CoverageType.Base, result.CoverageType);

            var expectedBenefitAmount = benefitDto.BaseOrOtherRider.NumberOfUnits * benefitDto.BaseOrOtherRider.ValuePerUnit;
            Assert.AreEqual(expectedBenefitAmount, result.BenefitAmount);

            Assert.IsNull(result.BenefitOptions);
        }

        [TestMethod]
        public void MapBenefit_LineOfBusiness_TraditionalLife_CoverageTypeBase_ShouldMapBenefitWithDividendOption()
        {
            // Arrange
            var benefitDto = GetDefaultTestBenefitDTO();

            // Act
            var result = BenefitMapper.MapBenefit(LineOfBusiness.TraditionalLife, benefitDto, new List<ExtendedKeyLookupResult>());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(benefitDto.BaseOrOtherRider.Dividend.ToDividendOption(), result.DividendOption);
        }

        [TestMethod]
        public void MapBenefit_LineOfBusiness_UniversalLife_BenefitTypeBaseForUniversalLife_ShouldMapBenefitWithDeathBenefitOption()
        {
            // Arrange
            var benefitDto = GetDefaultTestBenefitDTO();
            benefitDto.BenefitType = BenefitTypes.BaseForUniversalLife;
            benefitDto.BaseForUniversalLife = new PolicyBenefitTypeBF { BF_DB_OPTION = "2" };

            // Act
            var result = BenefitMapper.MapBenefit(LineOfBusiness.UniversalLife, benefitDto, new List<ExtendedKeyLookupResult>());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(benefitDto.BaseForUniversalLife.BF_DB_OPTION.ToDeathBenefitOption(), result.DeathBenefitOption);
        }

        [TestMethod]
        public void MapBenefit_UnderwritingClassBenefitDataNull_NoExtendedKeyData_ShouldReturnBenefitsNull()
        {
            // Arrange
            var benefitDto = GetDefaultTestBenefitDTO();
            var multipleInsuredUnderwritingClass = "N";
            benefitDto.CoverageExpansion = new CoverageExpansionDTO
            {
                Details = new List<CoverageExpansionDetailDTO>()
            };

            benefitDto.MultipleInsureds = new List<MultipleInsuredDTO>
            {
                new MultipleInsuredDTO
                {
                    StartDate = 20220101,
                    StopDate = 20220909,
                    RelationshipToPrimaryInsured = "BROTHER",
                    UnderwritingClass = multipleInsuredUnderwritingClass,
                    KdBenefitExtendedKeys = "0101",
                    KdDefSegmentId = string.Empty
                }
            };

            var extendedKeyData = new List<ExtendedKeyLookupResult>
            {
                new ExtendedKeyLookupResult
                {
                    Identifier = "taco"
                }
            };

            // Act
            var benefit = BenefitMapper.MapBenefit(LineOfBusiness.TrueGroupCensus, benefitDto, extendedKeyData);

            // Assert
            Assert.IsNotNull(benefit);
            Assert.IsNull(benefit.BenefitOptions);
        }

        [TestMethod]
        public void MapBenefit_UnderwritingClassBenefitData_NoExtendedKeyData_ShouldReturnMappedUnderwritingClassBenefitOption()
        {
            // Arrange
            var benefitDto = GetDefaultTestBenefitDTO();
            var multipleInsuredUnderwritingClass = "N";
            var expectedUnderwritingClassDescription = "Non Tobacco";
            benefitDto.CoverageExpansion = new CoverageExpansionDTO
            {
                Details = new List<CoverageExpansionDetailDTO>
                {
                    new CoverageExpansionDetailDTO
                    {
                        UnderwritingClassCode = multipleInsuredUnderwritingClass,
                        UnderwritingClassDescription = expectedUnderwritingClassDescription
                    },
                    new CoverageExpansionDetailDTO
                    {
                        UnderwritingClassCode = "NotExpected",
                        UnderwritingClassDescription = "taco"
                    }
                }
            };

            benefitDto.MultipleInsureds = new List<MultipleInsuredDTO>
            {
                new MultipleInsuredDTO
                {
                    StartDate = 20220101,
                    StopDate = 20220909,
                    RelationshipToPrimaryInsured = "BROTHER",
                    UnderwritingClass = multipleInsuredUnderwritingClass,
                    KdBenefitExtendedKeys = "0101",
                    KdDefSegmentId = string.Empty
                }
            };

            var extendedKeyData = new List<ExtendedKeyLookupResult>
            {
                new ExtendedKeyLookupResult
                {
                    Identifier = "taco"
                }
            };

            // Act
            var benefit = BenefitMapper.MapBenefit(LineOfBusiness.TrueGroupCensus, benefitDto, extendedKeyData);

            // Assert
            Assert.IsNotNull(benefit.BenefitOptions);
            Assert.AreEqual(1, benefit.BenefitOptions.Count);
            var mappedOption = benefit.BenefitOptions.Single();
            var multipleInsuredDto = benefitDto.MultipleInsureds.Single();
            Assert.AreEqual(multipleInsuredDto.RelationshipToPrimaryInsured.ToRelationshipToPrimaryInsured(), mappedOption.RelationshipToPrimaryInsured);
            Assert.AreEqual(multipleInsuredDto.StartDate.ToNullableDateTime(), mappedOption.StartDate);
            Assert.AreEqual(multipleInsuredDto.StopDate.ToNullableDateTime(), mappedOption.StopDate);
            Assert.AreEqual(BenefitOptionName.UnderwritingClass, mappedOption.BenefitOptionName);
            Assert.AreEqual(expectedUnderwritingClassDescription.ToBenefitOptionValue(), mappedOption.BenefitOptionValue);
            Assert.AreEqual(multipleInsuredDto.RelationshipToPrimaryInsured.ToRelationshipToPrimaryInsured(), mappedOption.RelationshipToPrimaryInsured);
        }

        [TestMethod]
        public void Benefit_WithExtendedKeyData_ShouldMapBenefitOptions()
        {
            // Arrange
            var benefitIdentifier1 = "ident1";
            var benefitIdentifier2 = "ident2";
            var expectedBenefit1Option1Name = "acc benefit period";
            var expectedBenefit1Option2Name = "coverage";
            var expectedBenefit1Option3Name = "family coverage";
            var expectedBenefit1Option1Value = "24 weeks";
            var expectedBenefit1Option2Value = "$25.00";
            var expectedBenefit1Option3Value = "cancer rider";

            var expectedBenefit2Option1Name = "class";
            var expectedBenefit2Option1Value = "lifetime";
            var benefitDto = GetDefaultTestBenefitDTO();
            var selfInsuredStartDate = 20220111;
            var selfInsuredStopDate = 20220919;
            var brotherInsuredStartDate = 19990101;
            var brotherInsuredStopDate = 20200505;
            benefitDto.MultipleInsureds = new List<MultipleInsuredDTO>
            {
                new MultipleInsuredDTO
                {
                    RelationshipToPrimaryInsured = "SELF",
                    KdDefSegmentId = benefitIdentifier1,
                    KdBenefitExtendedKeys = "010302",
                    StartDate = selfInsuredStartDate,
                    StopDate = selfInsuredStopDate
                },
                new MultipleInsuredDTO
                {
                    RelationshipToPrimaryInsured = "BROTHER",
                    KdDefSegmentId = benefitIdentifier2,
                    KdBenefitExtendedKeys = "04",
                    StartDate = brotherInsuredStartDate,
                    StopDate = brotherInsuredStopDate
                }
            };

            var extendedKeyData = new List<ExtendedKeyLookupResult>
            {
                new ExtendedKeyLookupResult
                {
                    Identifier = benefitIdentifier1,
                    Lookups = new List<KeyLookupResult>
                    {
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 0,
                            Key = 1,
                            Value = expectedBenefit1Option1Name
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 0,
                            Key = 2,
                            Value = expectedBenefit1Option2Name
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 0,
                            Key = 3,
                            Value = expectedBenefit1Option3Name
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 1,
                            Key = 1,
                            Value = expectedBenefit1Option1Value
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 2,
                            Key = 3,
                            Value = expectedBenefit1Option2Value
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 3,
                            Key = 2,
                            Value = expectedBenefit1Option3Value
                        }
                    }
                },
                new ExtendedKeyLookupResult
                {
                    Identifier = benefitIdentifier2,
                    Lookups = new List<KeyLookupResult>
                    {
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 0,
                            Key = 1,
                            Value = expectedBenefit2Option1Name
                        },
                        new KeyLookupResult
                        {
                            BenefitOrdinal = 1,
                            Key = 4,
                            Value = expectedBenefit2Option1Value
                        }
                    }
                }
            };

            // Act
            var result = BenefitMapper.MapBenefit(LineOfBusiness.TrueGroupCensus, benefitDto, extendedKeyData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.BenefitOptions);
            Assert.AreEqual(4, result.BenefitOptions.Count);
            var benefit1StartDate = selfInsuredStartDate.ToNullableDateTime();
            var benefit1StopDate = selfInsuredStopDate.ToNullableDateTime();
            Assert.AreEqual(expectedBenefit1Option1Name.ToBenefitOptionName(), result.BenefitOptions[0].BenefitOptionName);
            Assert.AreEqual(expectedBenefit1Option1Value.ToBenefitOptionValue(), result.BenefitOptions[0].BenefitOptionValue);
            Assert.AreEqual(benefit1StartDate, result.BenefitOptions[0].StartDate);
            Assert.AreEqual(benefit1StopDate, result.BenefitOptions[0].StopDate);
            Assert.AreEqual(RelationshipToPrimaryInsured.Self, result.BenefitOptions[0].RelationshipToPrimaryInsured);

            Assert.AreEqual(expectedBenefit1Option2Name.ToBenefitOptionName(), result.BenefitOptions[1].BenefitOptionName);
            Assert.AreEqual(expectedBenefit1Option2Value.ToBenefitOptionValue(), result.BenefitOptions[1].BenefitOptionValue);
            Assert.AreEqual(benefit1StartDate, result.BenefitOptions[1].StartDate);
            Assert.AreEqual(benefit1StopDate, result.BenefitOptions[1].StopDate);
            Assert.AreEqual(RelationshipToPrimaryInsured.Self, result.BenefitOptions[0].RelationshipToPrimaryInsured);

            Assert.AreEqual(expectedBenefit1Option3Name.ToBenefitOptionName(), result.BenefitOptions[2].BenefitOptionName);
            Assert.AreEqual(expectedBenefit1Option3Value.ToBenefitOptionValue(), result.BenefitOptions[2].BenefitOptionValue);
            Assert.AreEqual(benefit1StartDate, result.BenefitOptions[2].StartDate);
            Assert.AreEqual(benefit1StopDate, result.BenefitOptions[2].StopDate);
            Assert.AreEqual(RelationshipToPrimaryInsured.Self, result.BenefitOptions[0].RelationshipToPrimaryInsured);

            var benefit2StartDate = brotherInsuredStartDate.ToNullableDateTime();
            var benefit2StopDate = brotherInsuredStopDate.ToNullableDateTime();
            Assert.AreEqual(expectedBenefit2Option1Name.ToBenefitOptionName(), result.BenefitOptions[3].BenefitOptionName);
            Assert.AreEqual(expectedBenefit2Option1Value.ToBenefitOptionValue(), result.BenefitOptions[3].BenefitOptionValue);
            Assert.AreEqual(benefit2StartDate, result.BenefitOptions[3].StartDate);
            Assert.AreEqual(benefit2StopDate, result.BenefitOptions[3].StopDate);
            Assert.AreEqual(RelationshipToPrimaryInsured.Brother, result.BenefitOptions[3].RelationshipToPrimaryInsured);
        }

        private BenefitDTO GetDefaultTestBenefitDTO()
        {
            return new BenefitDTO
            {
                ProductCoverages = new ProductCoveragesDTO
                {
                    Description = "description",
                },
                BenefitType = BenefitTypes.Base,
                StatusCode = "A",
                StatusReason = "RI",
                BenefitSequence = 1,
                PBEN_ID = 1,
                PlanCode = "planCode",
                BaseOrOtherRider = new PolicyBenefitTypeBA_OR
                {
                    Dividend = "5",
                    NumberOfUnits = 1,
                    ValuePerUnit = 2
                }
            };
        }
    }
}
