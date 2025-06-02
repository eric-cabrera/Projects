namespace Assurity.Kafka.Engines.Mapping
{
    using System.Collections.Generic;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;

    public interface IRequirementsMapper
    {
        Requirement GenerateHomeOfficeReviewRequirement();

        List<Requirement> MapRequirements(
            List<PolicyRequirement> policyRequirements,
            List<RequirementMapping> requirementMappings,
            List<ParticipantDTO> participants,
            List<GlobalRequirementLookupResult> globalComments);
    }
}