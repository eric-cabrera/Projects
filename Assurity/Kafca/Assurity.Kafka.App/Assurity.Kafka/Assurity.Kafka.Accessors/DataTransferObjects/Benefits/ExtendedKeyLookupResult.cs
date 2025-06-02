namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public record ExtendedKeyLookupResult
    {
        public string Identifier { get; set; }

        public List<KeyLookupResult> Lookups { get; set; }
    }
}
