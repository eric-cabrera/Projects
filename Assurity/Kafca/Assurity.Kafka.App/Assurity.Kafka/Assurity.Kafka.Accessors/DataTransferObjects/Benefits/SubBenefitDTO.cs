namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public class SubBenefitDTO
    {
        public long PBEN_ID { get; set; }

        public decimal NumberOfUnits { get; set; }

        public decimal ValuePerUnit { get; set; }

        public decimal AnnualPremiumPerUnit { get; set; }
    }
}
