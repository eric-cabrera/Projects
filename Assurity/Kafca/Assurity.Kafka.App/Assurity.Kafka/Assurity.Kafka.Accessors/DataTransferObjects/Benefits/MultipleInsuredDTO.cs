namespace Assurity.Kafka.Accessors.DataTransferObjects.Benefits
{
    public class MultipleInsuredDTO
    {
        public int NameId { get; set; }

        public string RelationshipToPrimaryInsured { get; set; }

        public string KdDefSegmentId { get; set; }

        public string KdBenefitExtendedKeys { get; set; }

        public string UnderwritingClass { get; set; }

        public int StartDate { get; set; }

        public int StopDate { get; set; }
    }
}
