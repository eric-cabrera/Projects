namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class AnnuityPolicyDTO
    {
        public string CompanyCode { get; set; }

        public int IssueDate { get; set; }

        public string PolicyNumber { get; set; }

        public string StatusCode { get; set; }

        public int StatusDate { get; set; }

        public string StatusReason { get; set; }

        public string TaxQualification { get; set; }
    }
}
