namespace Assurity.Kafka.Engines.Mapping
{
    using System.Collections.Generic;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    public interface IPolicyMapper
    {
        Employer MapEmployer(EmployerDTO employerDto);

        Policy MapPolicy(PolicyDTO policyDto);

        List<Annuitant> MapAnnuitants(List<ParticipantDTO> relationshipDTOs);

        Assignee MapAssignee(List<ParticipantDTO> relationshipDtos);

        List<Beneficiary> MapBeneficiaries(List<ParticipantDTO> relationshipDtos);

        List<Insured> MapInsureds(List<ParticipantDTO> participantDtos, List<BenefitDTO> benefitDtos);

        List<Owner> MapOwners(List<ParticipantDTO> relationshipDtos);

        Payee MapPayee(List<ParticipantDTO> relationshipDtos);

        List<Payor> MapPayors(List<ParticipantDTO> relationshipDtos);

        Benefit MapBenefit(
            LineOfBusiness lineOfBusiness,
            BenefitDTO benefitDto,
            List<ExtendedKeyLookupResult> extendedKeyData);

        Agent MapAgent(PolicyAgentDTO fullAgentDto);

        Agent MapAgent(JustInTimeAgentDTO jitAgentDto, JustInTimeAgentNameDTO jitNameDto);

        Agent MapAgent(JustInTimeAgentDTO jitAgentDto, PNAME pname);

        List<Requirement> MapRequirements(
            List<PolicyRequirement> policyRequirement,
            List<RequirementMapping> requirementMapping,
            List<ParticipantDTO> participants,
            List<GlobalRequirementLookupResult> globalComments);

        Requirement GenerateHomeOfficeReviewRequirement();
    }
}