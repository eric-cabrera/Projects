namespace Assurity.AgentPortal.Accessors.Agent;

using Assurity.AgentPortal.Contracts.AgentContracts;
using AgentAPI = Assurity.Agent.Contracts;

public interface IAgentApiAccessor
{
    Task<AgentAPI.AgentAccessResponse?> GetAgentAccess(string agentId, CancellationToken cancellationToken);

    Task<List<AgentAPI.AgentContract>?> GetAgentContracts(
        string agentId,
        bool includeAssociatedAgentNumbers,
        AgentAPI.MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken,
        string? agentStatusFilters = null);

    Task<List<AgentAPI.AgentContract>?> GetAgentVertaforeContracts(string agentId, CancellationToken cancellationToken);

    Task<List<string>?> GetAgentMarketCodes(
        string agentId,
        bool filterMarketCodesForAgentCenter,
        CancellationToken cancellationToken);

    Task<List<string>?> GetAdditionalAgentIds(string agentId, CancellationToken cancellationToken = default);

    Task<AgentAPI.AgentInformation?> GetAgentInformation(string agentId, CancellationToken cancellationToken);

    Task<AgentAPI.ActiveHierarchy.ActiveHierarchyResponse?> GetAgentHierarchy(
      string agentNumber,
      string marketCode,
      string agentLevel,
      string companyCode,
      ContractStatus? contractStatus,
      bool includeAgentInformation,
      bool includePendingRequirements,
      bool filterAgentsWithoutPendingRequirements,
      CancellationToken cancellationToken);

    Task<AgentAPI.ActiveHierarchy.AgentContractInformation?> GetAgentInformation(
      string agentNumber,
      string marketCode,
      string level,
      string companyCode,
      CancellationToken cancellationToken);

    Task<AgentAPI.ActiveHierarchy.AgentAppointmentResponse?> GetActiveAppointments(
      string agentNumber,
      string marketCode,
      string agentLevel,
      string companyCode,
      Contracts.Enums.State? state,
      int? page,
      int? pageSize,
      CancellationToken cancellationToken);

    Task<List<AgentAPI.AgentContract>?> GetAgentHierarchyDownline(
      string agentNumber,
      string marketCode,
      string agentLevel,
      string companyCode,
      CancellationToken cancellationToken);

    Task<bool> GetAgentHasHierarchyDownline(string agentNumber, CancellationToken cancellationToken);
}