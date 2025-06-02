namespace Assurity.Kafka.Accessors.DataTransferObjects.Requirements
{
    using System.Collections.Generic;

    public record GlobalRequirementCommentsLookupDTO
    {
        public string PolicyNumber { get; set; }

        public List<RequirementLookupDTO> Lookups { get; set; }
    }
}
