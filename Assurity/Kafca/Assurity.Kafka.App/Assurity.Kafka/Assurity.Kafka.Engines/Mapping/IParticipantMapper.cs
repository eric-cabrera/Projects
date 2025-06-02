namespace Assurity.Kafka.Engines.Mapping
{
    using System.Collections.Generic;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;

    public interface IParticipantMapper
    {
        Agent MapAgent(JustInTimeAgentDTO jitAgentDto, JustInTimeAgentNameDTO jitNameDto);

        Agent MapAgent(PolicyAgentDTO fullAgentDto);

        Agent MapAgent(JustInTimeAgentDTO jitAgentDto, PNAME pname);

        List<Annuitant> MapAnnuitants(List<ParticipantDTO> relationshipDTOs);

        Assignee MapAssignee(List<ParticipantDTO> relationshipDtos);

        List<Beneficiary> MapBeneficiaries(List<ParticipantDTO> relationshipDtos);

        List<Insured> MapInsureds(List<ParticipantDTO> participantDtos, List<BenefitDTO> benefitDtos);

        List<Owner> MapOwners(List<ParticipantDTO> relationshipDtos);

        Participant MapParticipant(ParticipantDTO participantDto);

        Payee MapPayee(List<ParticipantDTO> relationshipDtos);

        List<Payor> MapPayors(List<ParticipantDTO> relationshipDtos);
    }
}