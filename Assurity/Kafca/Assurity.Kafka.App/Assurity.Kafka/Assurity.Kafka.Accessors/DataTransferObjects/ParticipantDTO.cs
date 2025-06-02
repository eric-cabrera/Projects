namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class ParticipantDTO
    {
        public string RelateCode { get; set; }

        public string IdentifyingAlpha { get; set; }

        public short BenefitSequenceNumber { get; set; }

        public string SexCode { get; set; }

        public int DateOfBirth { get; set; }

        public NameDTO Name { get; set; }

        public List<AddressDTO> Addresses { get; set; }
    }
}
