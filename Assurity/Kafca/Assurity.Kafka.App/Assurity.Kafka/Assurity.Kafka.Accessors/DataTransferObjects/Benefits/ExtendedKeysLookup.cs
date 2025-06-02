namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public record ExtendedKeysLookup
    {
        public List<KeyLookup> Lookups { get; set; }
    }
}