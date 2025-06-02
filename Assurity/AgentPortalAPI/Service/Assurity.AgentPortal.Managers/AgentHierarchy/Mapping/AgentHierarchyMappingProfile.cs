namespace Assurity.AgentPortal.Managers.AgentHierarchy.Mapping;

using Assurity.AgentPortal.Utilities.Formatting;
using AutoMapper;
using AgentAPI = Assurity.Agent.Contracts;
using LocalAgentContracts = Assurity.AgentPortal.Contracts.AgentContracts;

public class AgentHierarchyMappingProfile : Profile
{
    private readonly string hierarchyDateFormat = "MM/dd/yyyy";

    public AgentHierarchyMappingProfile()
    {
        CreateMap<AgentAPI.ActiveHierarchy.ActiveHierarchyFilters, LocalAgentContracts.ActiveHierarchyFilters>();

        CreateMappingForAgentHierarchyBranch();
        CreateMappingForActiveHierarchy();
        CreateMappingForPendingRequirements();
        CreateMappingForAgentAppointment();
    }

    private static string? GetAgentName(AgentAPI.ActiveHierarchy.AgentContractInformation agentContractInformation)
    {
        if (agentContractInformation.Name.IsBusiness)
        {
            return agentContractInformation.Name.BusinessName;
        }
        else
        {
            return $"{agentContractInformation.Name.IndividualLast}, {agentContractInformation.Name.IndividualFirst}";
        }
    }

    private static string? GetDirectDeposit(AgentAPI.ActiveHierarchy.AgentContractInformation agentContractInformation)
    {
        if (agentContractInformation.DirectDeposit.HasValue)
        {
            return agentContractInformation.DirectDeposit.Value ? "Yes" : "No";
        }

        return null;
    }

    private static LocalAgentContracts.ActiveHierarchyFilters GetHierarchyFilters(AgentAPI.ActiveHierarchy.ActiveHierarchyFilters activeHierarchyFilters)
    {
        return new LocalAgentContracts.ActiveHierarchyFilters
        {
            AgentNames = activeHierarchyFilters.AgentNames
                .Select(kvp =>
                    new LocalAgentContracts.Agent
                    {
                        DisplayValue = kvp.Key + " - " + kvp.Value,
                        AgentNumber = kvp.Key,
                        AgentName = kvp.Value
                    })
                .ToList()
        };
    }

    private void CreateMappingForAgentHierarchyBranch()
    {
        CreateMap<AgentAPI.ActiveHierarchy.AgentHierarchyBranch, LocalAgentContracts.AgentHierarchyBranch>()
            .ForMember(dest => dest.ContractStatus, opt => opt.MapFrom(src => DataFormatter.GetEnumDisplayName(src.ContractStatus)));

        CreateMap<AgentAPI.ActiveHierarchy.AgentContractInformation, LocalAgentContracts.AgentContractInformation>()
            .ForMember(dest => dest.DirectDeposit, opt => opt.MapFrom(src => GetDirectDeposit(src)))
            .ForMember(dest => dest.AdvanceRate, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAPercentage(src.AdvanceRate)))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.StartDate, hierarchyDateFormat)));

        CreateMap<AgentAPI.Address, LocalAgentContracts.Address>()
            .ForMember(dest => dest.FaxNumber, opt => opt.MapFrom(src => DataFormatter.FormatPhoneNumber(src.FaxNumber)))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => DataFormatter.FormatPhoneNumber(src.PhoneNumber)))
            .ForMember(dest => dest.Zip, opt => opt.MapFrom(src => DataFormatter.FormatZipCode(src.Zip)));

        CreateMap<AgentAPI.Requirement, LocalAgentContracts.Requirement>();
    }

    private void CreateMappingForActiveHierarchy()
    {
        CreateMap<AgentAPI.ActiveHierarchy.ActiveHierarchyResponse, LocalAgentContracts.ActiveHierarchyResponse>()
            .ForMember(dest => dest.Filters, opt => opt.MapFrom(src => GetHierarchyFilters(src.Filters)));

        CreateMap<AgentAPI.ActiveHierarchy.AgentHierarchy, LocalAgentContracts.AgentHierarchy>();

        CreateMap<AgentAPI.ActiveHierarchy.AgentHierarchyBranch, LocalAgentContracts.FileExport.ActiveHierarchyExport>()
            .ForMember(dest => dest.ActiveAgentId, opt => opt.MapFrom(src => src.AgentNumber ?? string.Empty))
            .ForMember(dest => dest.AgentName, opt => opt.MapFrom(src => GetAgentName(src.AgentInformation)))
            .ForMember(dest => dest.AdvanceRate, opt => opt.MapFrom(src => DataFormatter.FormatDecimalAsAPercentage(src.AgentInformation.AdvanceRate)))
            .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AgentInformation.Address.Line1))
            .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AgentInformation.Address.Line2))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.AgentInformation.Address.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.AgentInformation.Address.StateAbbreviation))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => DataFormatter.FormatZipCode(src.AgentInformation.Address.Zip)))
            .ForMember(dest => dest.AdvanceFrequency, opt => opt.MapFrom(src => src.AgentInformation.AdvanceFrequency))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => DataFormatter.FormatPhoneNumber(src.AgentInformation.Address.PhoneNumber)))
            .ForMember(dest => dest.Fax, opt => opt.MapFrom(src => DataFormatter.FormatPhoneNumber(src.AgentInformation.Address.FaxNumber)))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AgentInformation.Address.EmailAddress))
            .ForMember(dest => dest.ContractStatus, opt => opt.MapFrom(src => DataFormatter.GetEnumDisplayName(src.ContractStatus)))
            .ForMember(dest => dest.DirectDeposit, opt => opt.MapFrom(src => GetDirectDeposit(src.AgentInformation)))
            .ForMember(dest => dest.AML, opt => opt.MapFrom(src => src.AgentInformation.AntiMoneyLaundering ?? "None"));
    }

    private void CreateMappingForPendingRequirements()
    {
        CreateMap<AgentAPI.ActiveHierarchy.ActiveHierarchyResponse, LocalAgentContracts.PendingRequirementsHierarchyResponse>()
            .ForMember(dest => dest.Filters, opt => opt.MapFrom(src => GetHierarchyFilters(src.Filters)));

        CreateMap<AgentAPI.ActiveHierarchy.AgentHierarchyBranch, LocalAgentContracts.PendingRequirementsHierarchyBranch>()
            .ForMember(dest => dest.ContractStatus, opt => opt.MapFrom(src => DataFormatter.GetEnumDisplayName(src.ContractStatus)))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.AgentInformation.Address.EmailAddress))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => DataFormatter.FormatPhoneNumber(src.AgentInformation.Address.PhoneNumber)));

        CreateMap<AgentAPI.ActiveHierarchy.AgentHierarchyBranch, LocalAgentContracts.FileExport.HierarchyPendingRequirementExport>()
            .ForMember(dest => dest.AgentName, opt => opt.MapFrom(src => GetAgentName(src.AgentInformation)))
            .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentNumber))
            .ForMember(dest => dest.MarketCode, opt => opt.MapFrom(src => src.MarketCode))
            .ForMember(dest => dest.AgentLevel, opt => opt.MapFrom(src => src.Level))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.AgentInformation.Address.EmailAddress))
            .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src =>
                src.PendingRequirements.Any()
                ? string
                    .Join(", ", src.PendingRequirements
                    .Where(r => r.Description != null)
                    .Select(r => r.Description))
                : string.Empty))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src =>
                src.PendingRequirements.Any()
                ? string
                    .Join(", ", src.PendingRequirements
                    .Where(r => r.Note != null)
                    .Select(r => r.Note))
                : string.Empty));
    }

    private void CreateMappingForAgentAppointment()
    {
        CreateMap<AgentAPI.ActiveHierarchy.AgentAppointmentResponse, LocalAgentContracts.AgentAppointmentResponse>();

        CreateMap<AgentAPI.AgentAppointment, LocalAgentContracts.AgentAppointment>()
            .ForMember(dest => dest.GrantedDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.GrantedDate, hierarchyDateFormat)))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.ExpirationDate, hierarchyDateFormat)))
            .ForMember(dest => dest.IsResident, opt => opt.MapFrom(src => src.IsResident ? "Yes" : "No"));

        CreateMap<AgentAPI.AgentAppointment, LocalAgentContracts.FileExport.AgentAppointmentExport>()
            .ForMember(dest => dest.GrantedDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.GrantedDate, hierarchyDateFormat)))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => DataFormatter.FormatDate(src.ExpirationDate, hierarchyDateFormat)))
            .ForMember(dest => dest.IsResident, opt => opt.MapFrom(src => src.IsResident ? "R" : "NR"));
    }
}
