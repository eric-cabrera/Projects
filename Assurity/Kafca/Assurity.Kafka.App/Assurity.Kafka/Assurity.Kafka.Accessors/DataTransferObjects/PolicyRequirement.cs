namespace Assurity.Kafka.Accessors.DataTransferObjects
{
    public class PolicyRequirement
    {
        public string Description { get; set; }

        public int NameId { get; set; }

        public short Id { get; set; }

        public int AddedDate { get; set; }

        public int ObtainedDate { get; set; }

        public string Status { get; set; }

        public string LifeproComment { get; set; }

        public int ReqSequence { get; set; }

        /// <summary>
        /// Requirement Index.
        /// </summary>
        public short Ix { get; set; }

        public string ReqType { get; set; }
    }
}