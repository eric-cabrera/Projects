namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public record NameDTO
    {
        public int NameId { get; set; }

        public string NameFormatCode { get; set; }

        public string IndividualPrefix { get; set; }

        public string IndividualFirst { get; set; }

        public string IndividualMiddle { get; set; }

        public string IndividualLast { get; set; }

        public string IndividualSuffix { get; set; }

        public string NameBusiness { get; set; }

        public string BusinessEmailAdress { get; set; }

        public string PersonalEmailAdress { get; set; }
    }
}
