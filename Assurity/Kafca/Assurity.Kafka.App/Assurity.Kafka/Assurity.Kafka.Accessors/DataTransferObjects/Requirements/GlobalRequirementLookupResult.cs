namespace Assurity.Kafka.Accessors.DataTransferObjects.Requirements
{
    public record GlobalRequirementLookupResult
    {
        public int Sequence { get; set; }

        public int Ix { get; set; }

        public string Type { get; set; }

        public string Note { get; set; }
    }
}
