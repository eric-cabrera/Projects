namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public record KeyLookup
    {
        public string Identifier { get; set; }

        public short MaxOrdinal { get; set; }

        public short MaxKeyValue { get; set; }
    }
}
