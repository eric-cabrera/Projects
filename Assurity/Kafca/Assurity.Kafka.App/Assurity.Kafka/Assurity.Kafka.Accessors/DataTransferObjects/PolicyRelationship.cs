namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class PolicyRelationship
    {
        public string CompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public List<string> RelateCodes { get; set; }
    }
}