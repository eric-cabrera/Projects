namespace Assurity.AgentPortal.Managers.AgentHierarchy;

using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Shared;

public interface IAgentHierarchyManager
{
    public Task<AgentContractsResponse?> GetAgentContracts(
        string agentId,
        bool includeAssociatedAgentNumbers,
        MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken,
        string? agentStatusFilters = null);

    public Task<MarketCodeInformationResponse> GetMarketCodeInformation(string marketCode);

    public Task<VertaforeInformationResponse?> GetVertaforeInformation(string agentId, string username, string email, CancellationToken cancellationToken);

    Task<List<DropdownOption>?> GetViewAsDropdownOptions(
        string agentId,
        bool includeAssociatedAgentNumbers,
        MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken);

    Task<ActiveHierarchyResponse?> GetAgentHierarchy(
        string loggedInAgentNumber,
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        ContractStatus? contractStatus,
        bool includeAgentInformation,
        bool includePendingRequirements,
        bool filterAgentsWithoutPendingRequirements,
        CancellationToken cancellationToken);

    Task<PendingRequirementsHierarchyResponse?> GetPendingRequirementsHierarchy(
        string loggedInAgentNumber,
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        CancellationToken cancellationToken);

    Task<AgentContractInformation?> GetAgentInformation(
        string loggedInAgentNumber,
        string viewAsAgentNumber,
        string viewAsMarketCode,
        string viewAsAgentlevel,
        string viewAsCompanyCode,
        string downlineAgentNumber,
        string downlineMarketCode,
        string downlineAgentLevel,
        string downlineCompanyCode,
        CancellationToken cancellationToken);

    Task<AgentAppointmentResponse?> GetActiveAppointments(
         string loggedInAgentNumber,
         string viewAsAgentNumber,
         string viewAsMarketCode,
         string viewAsAgentlevel,
         string viewAsCompanyCode,
         string downlineAgentNumber,
         string downlineMarketCode,
         string downlineAgentLevel,
         string downlineCompanyCode,
         State? state,
         int? page,
         int? pageSize,
         CancellationToken cancellationToken);

    Task<FileResponse?> GetActiveHierarchyExport(
       string loggedInAgentNumber,
       string agentNumber,
       string marketCode,
       string agentLevel,
       string companyCode,
       ContractStatus? contractStatus,
       CancellationToken cancellationToken);

    Task<FileResponse?> GetActivePendingRequirementsExport(
       string loggedInAgentNumber,
       string agentNumber,
       string marketCode,
       string agentLevel,
       string companyCode,
       CancellationToken cancellationToken);

    Task<FileResponse?> GetAppointmentsExport(
       string loggedInAgentNumber,
       string viewAsAgentNumber,
       string viewAsMarketCode,
       string viewAsAgentlevel,
       string viewAsCompanyCode,
       string downlineAgentNumber,
       string downlineMarketCode,
       string downlineAgentLevel,
       string downlineCompanyCode,
       State? state,
       CancellationToken cancellationToken);

    Task<bool> DoesAgentHaveHierarchyDownline(string loggedInAgentNumber, CancellationToken cancellationToken);
}