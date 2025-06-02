namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is just really the PRELA, PNAME, PNALK, and PADDR entities stiched together to read the results from the owners query
    /// </summary>
    [Table("OwnerQueryEntity")]
    public class OwnerQueryEntity
    {
        [Required]
        [StringLength(12)]
        public string PolicyNumber { get; set; }

        [Key]
        public int NameId { get; set; }

        public string OwnerType { get; set; }

        public string ParticipantIsBusiness { get; set; }

        public string ParticipantBusinessName { get; set; }

        public string ParticipantBusinessEmailAddress { get; set; }

        public string ParticipantPersonGender { get; set; }

        public string ParticipantPersonDateOfBirth { get; set; }

        [StringLength(30)]
        public string ParticipantPersonIndividualPrefix { get; set; }

        [StringLength(40)]
        public string ParticipantPersonIndividualLast { get; set; }

        [StringLength(20)]
        public string ParticipantPersonIndividualFirst { get; set; }

        [StringLength(10)]
        public string ParticipantPersonIndividualMiddle { get; set; }

        [StringLength(6)]
        public string ParticipantPersonIndividualSuffix { get; set; }

        public string ParticipantPersonEmailAddress { get; set; }

        [StringLength(10)]
        public string ParticipantPhoneNumber { get; set; }

        [StringLength(35)]
        public string ParticipantAddressLine1 { get; set; }

        [StringLength(35)]
        public string ParticipantAddressLine2 { get; set; }

        [StringLength(35)]
        public string ParticipantAddressLine3 { get; set; }

        [StringLength(24)]
        public string ParticipantAddressCity { get; set; }

        [StringLength(2)]
        public string ParticipantAddressState { get; set; }

        [StringLength(5)]
        public string ParticipantAddressZip { get; set; }

        public string ParticipantAddressZipExtension { get; set; }

        public string ParticipantAddressBoxNumber { get; set; }

        public string ParticipantAddressCountry { get; set; }

        public int AddressId { get; set; }

        public string AddressCode { get; set; }

        public int CancelDate { get; set; }

        public string RelateCode { get; set; }
    }
}