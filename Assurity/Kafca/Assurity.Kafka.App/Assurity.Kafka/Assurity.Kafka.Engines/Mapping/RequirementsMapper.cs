namespace Assurity.Kafka.Engines.Mapping
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using NewRelic.Api.Agent;

    public class RequirementsMapper : IRequirementsMapper
    {
        public RequirementsMapper(IParticipantMapper participantMapper)
        {
            ParticipantMapper = participantMapper;
        }

        private IParticipantMapper ParticipantMapper { get; }

        [Trace]
        public List<Requirement> MapRequirements(
            List<PolicyRequirement> policyRequirements,
            List<RequirementMapping> requirementMappings,
            List<ParticipantDTO> participants,
            List<GlobalRequirementLookupResult> globalComments)
        {
            var mappedRequirements = new List<Requirement>();
            foreach (var policyRequirement in policyRequirements)
            {
                var matchingRequirementMapping = requirementMappings.FirstOrDefault(r => r.RequirementId == policyRequirement.Id);
                if (matchingRequirementMapping == null)
                {
                    continue;
                }

                var associatedParticipant = participants.FirstOrDefault(p => p.Name?.NameId == policyRequirement.NameId);
                var mappedParticipant =
                    associatedParticipant != null
                    ? ParticipantMapper.MapParticipant(associatedParticipant)
                    : null;

                var globalComment =
                    globalComments
                    .FirstOrDefault(c => c.Sequence == policyRequirement.ReqSequence
                        && c.Ix == policyRequirement.Ix
                        && c.Type == policyRequirement.ReqType)
                    ?.Note;

                mappedRequirements.Add(new Requirement
                {
                    AppliesTo = mappedParticipant,
                    AddedDate = policyRequirement.AddedDate.ToNullableDateTime(),
                    ObtainedDate = policyRequirement.ObtainedDate.ToNullableDateTime(),
                    Status = policyRequirement.Status.ToRequirementStatus(),
                    Name = policyRequirement.Description,
                    Id = policyRequirement.Id,
                    LifeProComment = policyRequirement.LifeproComment.Trim(),
                    GlobalComment = globalComment,
                    PhoneNumberComment = matchingRequirementMapping.Phone,
                    FulfillingParty = matchingRequirementMapping.FulfillingParty?.ToRequirementFulfillingParty(),
                    ActionType = matchingRequirementMapping.AgentAction?.ToRequirementActionType(),
                    Display = matchingRequirementMapping.Display
                });
            }

            return mappedRequirements;
        }

        public Requirement GenerateHomeOfficeReviewRequirement()
        {
            return new Requirement
            {
                AddedDate = DateTime.Now,
                Status = RequirementStatus.Unmet,
                Name = "Pending Home Office Review",
                Id = 0,
                FulfillingParty = RequirementFulfillingParty.HomeOffice,
                Display = true
            };
        }
    }
}
