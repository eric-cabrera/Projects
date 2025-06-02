namespace Assurity.Kafka.Accessors.DataTransferObjects.Requirements
{
    public record RequirementLookupDTO
    {
        public int REQSEQ { get; set; }

        public int IX { get; set; }

        public string REQTYPE { get; set; }
    }
}
