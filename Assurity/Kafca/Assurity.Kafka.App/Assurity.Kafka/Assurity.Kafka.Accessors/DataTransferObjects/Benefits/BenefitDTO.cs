namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public class BenefitDTO
    {
        public long PBEN_ID { get; set; }

        public string PolicyNumber { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// Sequence number to control the order of the benefits in LifePro.
        /// Among other things, this comes into play when one benefit is dependent on another benefit.
        /// The base benefit is always 1.
        /// </summary>
        public short BenefitSequence { get; set; }

        public string BenefitType { get; set; }

        public string StatusCode { get; set; }

        public string StatusReason { get; set; }

        public string PlanCode { get; set; }

        public int StatusDate { get; set; }

        public CoverageExpansionDTO CoverageExpansion { get; set; }

        public List<MultipleInsuredDTO> MultipleInsureds { get; set; }

        public PolicyBenefitTypeBA_OR BaseOrOtherRider { get; set; }

        public PolicyBenefitTypeBF BaseForUniversalLife { get; set; }

        public SubBenefitDTO PolicyBenefitTypeSL { get; set; }

        public SubBenefitDTO SpecifiedAmountIncrease { get; set; }

        public SubBenefitDTO Supplemental { get; set; }

        public ProductCoveragesDTO ProductCoverages { get; set; }
    }
}
