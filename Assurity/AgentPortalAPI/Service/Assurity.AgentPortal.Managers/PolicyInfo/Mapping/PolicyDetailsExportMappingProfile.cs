namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping;

using System.Text;
using Assurity.AgentPortal.Contracts.PolicyInfo.FileExport;
using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoMapper;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

public class PolicyDetailsExportMappingProfile : Profile
{
    public PolicyDetailsExportMappingProfile()
    {
        MapPendingPolicySumamry();
        MapActivePolicySummary();
        MapTerminatedPolicySummary();
    }

    public static string GetRequirementComment(Requirement requirement)
    {
        var comment = new StringBuilder();
        if (requirement.GlobalComment != null)
        {
            comment.AppendLine(requirement.GlobalComment);
        }

        if (requirement.LifeProComment != null)
        {
            comment.AppendLine(requirement.LifeProComment);
        }

        if (requirement.PhoneNumberComment != null)
        {
            comment.AppendLine(requirement.PhoneNumberComment);
        }

        return comment.ToString();
    }

    public static string GetActionNeeded(Requirement requirement)
    {
        if (requirement.FulfillingParty != RequirementFulfillingParty.Agent || requirement.ActionType == null)
        {
            return string.Empty;
        }

        return requirement.ActionType switch
        {
            RequirementActionType.SendMessage => "Send Info",
            RequirementActionType.UploadFile => "Send File",
            RequirementActionType.UploadFileOrSendMessage => "Send Info/File",
            _ => string.Empty,
        };
    }

    public static string GetParticipantName(Participant? participant)
    {
        var firstName = participant?.Person?.Name?.IndividualFirst;
        var lastName = participant?.Person?.Name?.IndividualLast;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName;
        }

        return $"{firstName} {lastName}".Trim();
    }

    public static string GetParticipantState(Participant? participant)
    {
        var stateAbbreviation = participant?.Address.StateAbbreviation;
        return stateAbbreviation.HasValue
            ? MappingExtensions.GetEnumDisplayName(stateAbbreviation)
            : stateAbbreviation.ToString() ?? string.Empty;
    }

    public static string GetParticipantZip(Participant? participant)
    {
        var zipCode = new StringBuilder();
        var primaryZipCode = participant?.Address?.ZipCode ?? string.Empty;
        if (string.IsNullOrEmpty(primaryZipCode))
        {
            return string.Empty;
        }
        else
        {
            zipCode.Append(primaryZipCode);
        }

        var zipCodeExtension = participant?.Address?.ZipExtension?.Trim() ?? string.Empty;
        var boxNumber = participant?.Address?.BoxNumber?.Trim() ?? string.Empty;

        if (!string.IsNullOrEmpty(zipCodeExtension) && zipCodeExtension.Length == 4)
        {
            zipCode.Append("-" + zipCodeExtension);
        }
        else if (!string.IsNullOrEmpty(boxNumber) && boxNumber.Length == 4)
        {
            zipCode.Append("-" + boxNumber);
        }

        return zipCode.ToString();
    }

    private static string GetServicingAgentId(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            var servicingAgent = agents.Where(agent => agent.IsServicingAgent == true).FirstOrDefault();
            return servicingAgent?.AgentId ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetServicingAgentLastName(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            var servicingAgent = agents.Where(agent => agent.IsServicingAgent == true).FirstOrDefault();

            if (servicingAgent?.Participant?.IsBusiness ?? false)
            {
                return servicingAgent.Participant.Business?.Name.BusinessName ?? string.Empty;
            }

            return servicingAgent?.Participant?.Person?.Name.IndividualLast ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetServicingAgentFirstName(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            var servicingAgent = agents.Where(agent => agent.IsServicingAgent == true).FirstOrDefault();
            return servicingAgent?.Participant?.Person?.Name.IndividualFirst ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetServicingOrJITAgentId(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            return agents?.FirstOrDefault(agent => agent.IsServicingAgent || agent.IsJustInTimeAgent)?.AgentId ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetServicingOrJITAgentLastName(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            var servicingOrJITAgent = agents?.FirstOrDefault(agent => agent.IsServicingAgent || agent.IsJustInTimeAgent);

            if (servicingOrJITAgent?.Participant?.IsBusiness ?? false)
            {
                return servicingOrJITAgent.Participant.Business?.Name.BusinessName ?? string.Empty;
            }

            return servicingOrJITAgent?.Participant?.Person?.Name.IndividualLast ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetServicingOrJITAgentFirstName(List<Agent> agents)
    {
        if (agents?.Any() ?? false)
        {
            return agents?.FirstOrDefault(agent => agent.IsServicingAgent || agent.IsJustInTimeAgent)?.
               Participant?.Person?.Name.IndividualFirst ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetParticipantLastName(Participant? participant)
    {
        if (participant?.IsBusiness ?? false)
        {
            return participant.Business?.Name.BusinessName ?? string.Empty;
        }

        return participant?.Person?.Name.IndividualLast ?? string.Empty;
    }

    private static string GetParticipantFirstName(Participant? participant)
    {
        return participant?.Person?.Name.IndividualFirst ?? string.Empty;
    }

    private static string GetParticipantPhoneNumber(Participant? participant)
    {
        return participant?.PhoneNumber ?? string.Empty;
    }

    private static string GetParticipantEmail(Participant? participant)
    {
        return participant?.Person?.EmailAddress ?? string.Empty;
    }

    private static string GetParticipantAddressLine1(Participant? participant)
    {
        return participant?.Address.Line1 ?? string.Empty;
    }

    private static string GetParticipantAddressLine2(Participant? participant)
    {
        return participant?.Address.Line2 ?? string.Empty;
    }

    private static string GetParticipantCity(Participant? participant)
    {
        return participant?.Address.City ?? string.Empty;
    }

    private static string? GetBillingReason(BillingReason? billingReason)
    {
        // (WP23-2118) To avoid confusion for our agents, only return one of these billing reasons: Paid Up, ETI, or RPU
        var billingReasonsToDisplay = new List<BillingReason> { BillingReason.PaidUp, BillingReason.ExtendedTerm, BillingReason.ReducedPaidUp };
        if (billingReason != null && billingReasonsToDisplay.Contains((BillingReason)billingReason))
        {
            return MappingExtensions.GetEnumDisplayName(billingReason);
        }

        return string.Empty;
    }

    private void MapPendingPolicySumamry()
    {
        CreateMap<PolicyInfoAPI.RequirementSummary, PendingPolicyExport>()
            .ForMember(
                summary => summary.EntryDate,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? summary.Requirement.AddedDate
                    : null))
            .ForMember(
                summary => summary.AgentLastName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingOrJITAgentLastName(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.AgentFirstName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingOrJITAgentFirstName(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.AgentId,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingOrJITAgentId(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantLastName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantFirstName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.Requirement,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? summary.Requirement.Name
                    : string.Empty))
            .ForMember(
                summary => summary.Met,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? MappingExtensions.GetEnumDisplayName(summary.Requirement.Status)
                    : string.Empty))
            .ForMember(
                summary => summary.RequirementAppliesTo,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? GetParticipantName(summary.Requirement.AppliesTo)
                    : string.Empty))
            .ForMember(
                summary => summary.RequirementComment,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? GetRequirementComment(summary.Requirement)
                    : string.Empty))
            .ForMember(
                summary => summary.ActionNeeded,
                opt => opt.MapFrom(
                    summary => summary.Requirement != null
                    ? GetActionNeeded(summary.Requirement)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantLastName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantFirstName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerPhoneNumber,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantPhoneNumber(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerEmail,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantEmail(summary.PrimaryOwner.Participant)
                    : string.Empty));
    }

    private void MapActivePolicySummary()
    {
        CreateMap<PolicyInfoAPI.PolicySummary, ActivePolicyExport>()
            .ForMember(
                summary => summary.AgentLastName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentLastName(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.AgentFirstName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentFirstName(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.AgentId,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentId(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantLastName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantFirstName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.Status,
                opt => opt.MapFrom(summary => MappingExtensions.GetEnumDisplayName(summary.PolicyStatus)))
            .ForMember(
                summary => summary.PastDue,
                opt => opt.MapFrom(summary => summary.PastDue ? "Past Due" : string.Empty))
            .ForMember(
                summary => summary.BillingReason,
                opt => opt.MapFrom(summary => GetBillingReason(summary.BillingReason)))
            .ForMember(
                summary => summary.BillingMode,
                opt => opt.MapFrom(
                    summary => summary.BillingMode != null
                    ? MappingExtensions.GetEnumDisplayName(summary.BillingMode)
                    : string.Empty))
            .ForMember(
                summary => summary.IssueState,
                opt => opt.MapFrom(
                    summary => summary.IssueState != null
                    ? MappingExtensions.GetEnumDisplayName(summary.IssueState)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantLastName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantFirstName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerPhoneNumber,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantPhoneNumber(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerEmail,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantEmail(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerAddressLine1,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantAddressLine1(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerAddressLine2,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantAddressLine2(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerCity,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantCity(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerState,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantState(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerZip,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantZip(summary.PrimaryOwner.Participant)
                    : string.Empty));
    }

    private void MapTerminatedPolicySummary()
    {
        CreateMap<PolicyInfoAPI.PolicySummary, TerminatedPolicyExport>()
            .ForMember(
                summary => summary.AgentLastName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentLastName(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.AgentFirstName,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentFirstName(summary.AssignedAgents)
                    : string.Empty))
             .ForMember(
                summary => summary.AgentId,
                opt => opt.MapFrom(
                    summary => summary.AssignedAgents != null
                    ? GetServicingAgentId(summary.AssignedAgents)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantLastName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryInsuredFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryInsured != null
                    ? GetParticipantFirstName(summary.PrimaryInsured.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.Status,
                opt => opt.MapFrom(summary => MappingExtensions.GetEnumDisplayName(summary.PolicyStatus)))
            .ForMember(
                summary => summary.TerminationReason,
                opt => opt.MapFrom(
                    summary => summary.PolicyStatusReason != null
                    ? MappingExtensions.GetEnumDisplayName(summary.PolicyStatusReason)
                    : string.Empty))
            .ForMember(
                summary => summary.TerminationDetail,
                opt => opt.MapFrom(
                    summary => summary.PolicyStatusDetail != null
                    ? MappingExtensions.GetEnumDisplayName(summary.PolicyStatusDetail)
                    : string.Empty))
            .ForMember(
                summary => summary.BillingReason,
                opt => opt.MapFrom(summary => GetBillingReason(summary.BillingReason)))
            .ForMember(
                summary => summary.BillingMode,
                opt => opt.MapFrom(
                    summary => summary.BillingMode != null
                    ? MappingExtensions.GetEnumDisplayName(summary.BillingMode)
                    : string.Empty))
            .ForMember(
                summary => summary.IssueState,
                opt => opt.MapFrom(
                    summary => summary.IssueState != null
                    ? MappingExtensions.GetEnumDisplayName(summary.IssueState)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerLastName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantLastName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerFirstName,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantFirstName(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerPhoneNumber,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantPhoneNumber(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerEmail,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantEmail(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerAddressLine1,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantAddressLine1(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerAddressLine2,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantAddressLine2(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerCity,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantCity(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerState,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantState(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.PrimaryOwnerZip,
                opt => opt.MapFrom(
                    summary => summary.PrimaryOwner != null
                    ? GetParticipantZip(summary.PrimaryOwner.Participant)
                    : string.Empty))
            .ForMember(
                summary => summary.FirstPaymentFail,
                opt => opt.MapFrom(
                    summary => summary.ReturnPaymentType == ReturnPaymentType.InitialPaymentCardDeclined
                    || summary.ReturnPaymentType == ReturnPaymentType.InitialPaymentCheckDraftDeclined
                    ? "First Payment Fail"
                    : string.Empty));
    }
}