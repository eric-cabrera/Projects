namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public record PaymentDTO
    {
        public short CreditCode { get; set; }

        public short DebitCode { get; set; }

        public int EffectiveDate { get; set; }

        public string ReversalCode { get; set; }
    }
}
