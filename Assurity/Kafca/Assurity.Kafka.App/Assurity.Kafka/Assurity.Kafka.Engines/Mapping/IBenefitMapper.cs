namespace Assurity.Kafka.Engines.Mapping
{
    using System.Collections.Generic;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    public interface IBenefitMapper
    {
        Benefit MapBenefit(
            LineOfBusiness lineOfBusiness,
            BenefitDTO benefitDto,
            List<ExtendedKeyLookupResult> extendedKeyData);
    }
}