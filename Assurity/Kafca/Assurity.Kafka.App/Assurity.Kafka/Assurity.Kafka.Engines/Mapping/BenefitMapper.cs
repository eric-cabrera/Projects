namespace Assurity.Kafka.Engines.Mapping
{
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using NewRelic.Api.Agent;

    public class BenefitMapper : IBenefitMapper
    {
        [Trace]
        public Benefit MapBenefit(LineOfBusiness lineOfBusiness, BenefitDTO benefitDto, List<ExtendedKeyLookupResult> extendedKeyData)
        {
            var coverageDescription = benefitDto.ProductCoverages?.Description;
            var benefit = new Benefit
            {
                BenefitDescription = !string.IsNullOrEmpty(coverageDescription) ? coverageDescription.Trim() : string.Empty,
                BenefitId = benefitDto.PBEN_ID,
                BenefitStatus = benefitDto.StatusCode.ToBenefitStatus(benefitDto.StatusReason),
                BenefitStatusReason = benefitDto.StatusReason.ToBenefitStatusReason(benefitDto.StatusCode),
                CoverageType = benefitDto.BenefitSequence == 1
                    ? CoverageType.Base
                    : CoverageType.Rider,
                PlanCode = benefitDto.PlanCode,
                BenefitAmount = CalculateBenefitAmount(benefitDto)
            };

            if (lineOfBusiness == LineOfBusiness.TraditionalLife
                && benefitDto.BaseOrOtherRider != null
                && (benefitDto.BenefitType == BenefitTypes.Base || benefitDto.BenefitType == BenefitTypes.OtherRider))
            {
                benefit.DividendOption = benefitDto.BaseOrOtherRider.Dividend.Trim().ToDividendOption();
            }
            else if (lineOfBusiness == LineOfBusiness.UniversalLife && benefitDto.BenefitType == BenefitTypes.BaseForUniversalLife)
            {
                benefit.DeathBenefitOption = GetDeathBenefitOption(benefitDto);
            }

            benefit.BenefitOptions = MapBenefitOptions(benefitDto, extendedKeyData);

            return benefit;
        }

        [Trace]
        private static List<BenefitOption> MapBenefitOptions(BenefitDTO benefitDto, List<ExtendedKeyLookupResult> extendedKeyData)
        {
            var allBenefitOptions = new List<BenefitOption>();
            if (benefitDto.BenefitType == BenefitTypes.Base
                || benefitDto.BenefitType == BenefitTypes.OtherRider
                || benefitDto.BenefitType == BenefitTypes.BaseForUniversalLife
                || benefitDto.BenefitType == BenefitTypes.Supplemental
                || benefitDto.BenefitType == BenefitTypes.TableRating
                || benefitDto.BenefitType == BenefitTypes.SpecifiedAmountIncrease)
            {
                var benefitOptions = new List<BenefitOption>();
                var extendedKeyBenefitOptions = MapExtendedKeyBenefitOptions(extendedKeyData, benefitDto.MultipleInsureds);
                if (extendedKeyBenefitOptions != null)
                {
                    benefitOptions.AddRange(extendedKeyBenefitOptions);
                }

                var underwritingClassBenefitOptions = MapUnderwritingClassBenefitOption(benefitDto, benefitDto.MultipleInsureds);
                if (underwritingClassBenefitOptions != null)
                {
                    benefitOptions.AddRange(underwritingClassBenefitOptions);
                }

                allBenefitOptions.AddRange(benefitOptions);
            }

            return allBenefitOptions.Any()
                ? allBenefitOptions
                : null;
        }

        [Trace]
        private static List<BenefitOption> MapUnderwritingClassBenefitOption(BenefitDTO benefitDto, List<MultipleInsuredDTO> multipleInsuredDtos)
        {
            var underwritingClassBenefitOptions = new List<BenefitOption>();
            foreach (var multipleInsuredDto in multipleInsuredDtos ?? new List<MultipleInsuredDTO>())
            {
                var benefitOptionValue = GetUnderwritingClassDescription(benefitDto, multipleInsuredDto);
                if (benefitOptionValue == null)
                {
                    continue;
                }

                var relationshipToPrimaryInsured = multipleInsuredDto.RelationshipToPrimaryInsured.Trim().ToRelationshipToPrimaryInsured();
                var benefitOption = new BenefitOption
                {
                    BenefitOptionName = BenefitOptionName.UnderwritingClass,
                    BenefitOptionValue = benefitOptionValue.Trim().ToBenefitOptionValue(),
                    RelationshipToPrimaryInsured = relationshipToPrimaryInsured,
                    StartDate = multipleInsuredDto.StartDate.ToNullableDateTime(),
                    StopDate = multipleInsuredDto.StopDate.ToNullableDateTime()
                };

                underwritingClassBenefitOptions.Add(benefitOption);
            }

            return underwritingClassBenefitOptions;
        }

        private static List<BenefitOption> MapExtendedKeyBenefitOptions(
            List<ExtendedKeyLookupResult> extendedKeyData,
            List<MultipleInsuredDTO> multipleInsuredDtos)
        {
            var benefitOptions = new List<BenefitOption>();
            foreach (var pmuin in multipleInsuredDtos ?? new List<MultipleInsuredDTO>())
            {
                var extendedKeys = pmuin.KdBenefitExtendedKeys;
                var benefitData = extendedKeyData.FirstOrDefault(x => x.Identifier == pmuin.KdDefSegmentId.Trim())?.Lookups;
                if (benefitData == null)
                {
                    continue;
                }

                var keyValues = ConvertStringToArray(extendedKeys);
                for (var i = 1; i <= keyValues.Length; i++)
                {
                    var benefitOption = new BenefitOption
                    {
                        BenefitOptionName = benefitData.FirstOrDefault(x => x.BenefitOrdinal == 0 && x.Key == i).Value.Trim().ToBenefitOptionName(),
                        BenefitOptionValue = benefitData.FirstOrDefault(x => x.BenefitOrdinal == i && x.Key == keyValues[i - 1]).Value.Trim().ToBenefitOptionValue(),
                        RelationshipToPrimaryInsured = pmuin.RelationshipToPrimaryInsured.Trim().ToRelationshipToPrimaryInsured(),
                        StartDate = pmuin.StartDate.ToNullableDateTime(),
                        StopDate = pmuin.StopDate.ToNullableDateTime()
                    };
                    benefitOptions.Add(benefitOption);
                }
            }

            return benefitOptions;
        }

        /// <summary>
        /// Converts extended key pairs to an array of short.
        /// </summary>
        /// <remarks>
        /// Extended key values look something like this: "04050100".
        /// The output of this function for that input would be { 4, 5, 1, 0 }.
        /// </remarks>
        /// <param name="extendedKeys"></param>
        /// <returns></returns>
        private static short[] ConvertStringToArray(string extendedKeys)
        {
            var duplicatedKey = new string(extendedKeys);
            var arrayLength = extendedKeys.Length / 2;
            var convertedValues = new short[arrayLength];
            var i = 0;

            while (duplicatedKey.Length >= 2)
            {
                var substring = duplicatedKey[..2];
                if (string.IsNullOrWhiteSpace(substring))
                {
                    substring = "0";
                }

                var currentValue = short.Parse(substring);
                convertedValues[i] = currentValue;

                duplicatedKey = duplicatedKey[2..];
                i++;
            }

            return convertedValues;
        }

        private static string? GetUnderwritingClassDescription(BenefitDTO benefit, MultipleInsuredDTO pmuin)
        {
            return
                benefit
                ?.CoverageExpansion
                ?.Details
                ?.Where(detail => detail.UnderwritingClassCode.Trim() == pmuin.UnderwritingClass.Trim())
                ?.FirstOrDefault()
                ?.UnderwritingClassDescription
                ?.Trim();
        }

        [Trace]
        private static decimal? CalculateBenefitAmount(BenefitDTO ppben)
        {
            decimal? numberOfUnits = null;
            decimal? valuePerUnit = null;

            switch (ppben.BenefitType)
            {
                case BenefitTypes.Base:
                case BenefitTypes.OtherRider:
                    numberOfUnits = ppben.BaseOrOtherRider.NumberOfUnits;
                    valuePerUnit = ppben.BaseOrOtherRider.ValuePerUnit;
                    break;

                // UL doesn't have units, so we use current death benefit. This is based on AssureLink, where it is displayed as PolicyFaceAmount.
                case BenefitTypes.BaseForUniversalLife:
                    return ppben.BaseForUniversalLife.BF_CURRENT_DB;

                case BenefitTypes.Supplemental:
                    numberOfUnits = ppben.Supplemental.NumberOfUnits;
                    valuePerUnit = ppben.Supplemental.ValuePerUnit;
                    break;
                case BenefitTypes.TableRating:
                    numberOfUnits = ppben.PolicyBenefitTypeSL.NumberOfUnits;
                    valuePerUnit = ppben.PolicyBenefitTypeSL.ValuePerUnit;
                    break;
                case BenefitTypes.SpecifiedAmountIncrease:
                    numberOfUnits = ppben.SpecifiedAmountIncrease.NumberOfUnits;
                    valuePerUnit = ppben.SpecifiedAmountIncrease.ValuePerUnit;
                    break;
            }

            return numberOfUnits.GetValueOrDefault() * valuePerUnit.GetValueOrDefault();
        }

        [Trace]
        private static DeathBenefitOption? GetDeathBenefitOption(BenefitDTO ppben)
        {
            if (ppben.BenefitType.Trim().Equals(BenefitTypes.BaseForUniversalLife, StringComparison.InvariantCultureIgnoreCase))
            {
                return ppben.BaseForUniversalLife?.BF_DB_OPTION?.ToDeathBenefitOption();
            }

            return null;
        }
    }
}
