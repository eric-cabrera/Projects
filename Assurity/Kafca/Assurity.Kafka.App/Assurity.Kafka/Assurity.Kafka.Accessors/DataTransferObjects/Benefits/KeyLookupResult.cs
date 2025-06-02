namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public record KeyLookupResult
    {
        /// <summary>
        /// Key lookup value.
        /// </summary>
        public short Key { get; set; }

        /// <summary>
        /// Order in which benefit should be accessed.
        /// </summary>
        /// <remarks>
        /// The first benefit is ordinal 1, the second 2, third 3...
        /// </remarks>
        public short BenefitOrdinal { get; set; }

        public string Value { get; set; }
    }
}