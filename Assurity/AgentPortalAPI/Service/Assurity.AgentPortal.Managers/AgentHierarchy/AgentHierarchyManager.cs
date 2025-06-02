namespace Assurity.AgentPortal.Managers.AgentHierarchy;

using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.Integration;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.AgentContracts.FileExport;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Encryption;
using AutoMapper;
using AgentAPI = Assurity.Agent.Contracts.ActiveHierarchy;

public class AgentHierarchyManager : IAgentHierarchyManager
{
    public AgentHierarchyManager(
        IAgentApiAccessor agentApiAccessor,
        IIntegrationAccessor dataStoreAccessor,
        IConfigurationManager configurationManager,
        IEncryption encryption,
        IMapper mapper,
        IFileExportEngine fileExportEngine)
    {
        AgentApiAccessor = agentApiAccessor;
        FileExportEngine = fileExportEngine;
        Mapper = mapper;
        DataStoreAccessor = dataStoreAccessor;
        ConfigurationManager = configurationManager;
        Encryption = encryption;
    }

    private IAgentApiAccessor AgentApiAccessor { get; }

    private IFileExportEngine FileExportEngine { get; }

    private IMapper Mapper { get; }

    private IIntegrationAccessor DataStoreAccessor { get; }

    private IConfigurationManager ConfigurationManager { get; }

    private IEncryption Encryption { get; }

    public async Task<AgentContractsResponse?> GetAgentContracts(
        string agentNumber,
        bool includeAssociatedAgentNumbers,
        MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken,
        string? agentStatusFilters = null)
    {
        Assurity.Agent.Contracts.MarketCodeFilters mappedMarketCodeFilter =
            Enum.TryParse(marketCodeFilter.ToString(), out Assurity.Agent.Contracts.MarketCodeFilters result)
            ? result
            : Assurity.Agent.Contracts.MarketCodeFilters.None;
        var agentContractsApiResponse = await AgentApiAccessor.GetAgentContracts(agentNumber, includeAssociatedAgentNumbers, mappedMarketCodeFilter, cancellationToken, agentStatusFilters);

        if (agentContractsApiResponse == null)
        {
            return null;
        }

        var response = new AgentContractsResponse
        {
            AgentContracts = ConvertAgentContractsResponseToAgentMarketCodeDictionary(agentContractsApiResponse),
        };

        return response;
    }

    public async Task<VertaforeInformationResponse?> GetVertaforeInformation(
        string agentId,
        string username,
        string email,
        CancellationToken cancellationToken)
    {
        var agentContractsApiResponse = await AgentApiAccessor.GetAgentVertaforeContracts(agentId, cancellationToken);

        if (agentContractsApiResponse == null)
        {
            return null;
        }

        var agentContractsDictionary = ConvertAgentContractsResponseToAgentMarketCodeDictionary(agentContractsApiResponse);

        var hasAccess = agentContractsDictionary.ContainsKey(agentId) && agentContractsDictionary[agentId].Count > 0;

        var redirectUrl = string.Empty;
        if (hasAccess)
        {
            var url = ConfigurationManager.VertaforeUrl;
            var salt = ConfigurationManager.VertaforeSalt;
            var secret = ConfigurationManager.VertaforeSecret;

            var encryptedAgentId = Encryption.EncryptStringAes(agentId, secret, salt);
            var encryptedEmail = Encryption.EncryptStringAes(email, secret, salt);
            var encryptedUsername = Encryption.EncryptStringAes(username, secret, salt);

            var cleanAgentId = ReplaceUrlIncompatibleChars(encryptedAgentId);
            var cleanEmail = ReplaceUrlIncompatibleChars(encryptedEmail);
            var cleanUsername = ReplaceUrlIncompatibleChars(encryptedUsername);

            redirectUrl = new Uri(Path.Combine(url, cleanAgentId, cleanEmail, cleanUsername)).ToString();
        }

        return new VertaforeInformationResponse
        {
            HasAccess = hasAccess,
            RedirectUrl = redirectUrl,
        };
    }

    public async Task<List<DropdownOption>?> GetViewAsDropdownOptions(
        string agentNumber,
        bool includeAssociatedAgentNumbers,
        MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken)
    {
        Assurity.Agent.Contracts.MarketCodeFilters mappedMarketCodeFilter =
            Enum.TryParse(marketCodeFilter.ToString(), out Assurity.Agent.Contracts.MarketCodeFilters result)
            ? result
            : Assurity.Agent.Contracts.MarketCodeFilters.None;
        var agentStatusFilters = "TERMINATED";

        var agentContractsApiResponse = await AgentApiAccessor.GetAgentContracts(agentNumber, includeAssociatedAgentNumbers, mappedMarketCodeFilter, cancellationToken, agentStatusFilters);

        if (agentContractsApiResponse == null)
        {
            return null;
        }

        var dropdownOptions = new List<DropdownOption>();

        foreach (var contract in agentContractsApiResponse)
        {
            var dropdownValue = $"{contract.AgentNumber}-{contract.MarketCode}-{contract.Level}";

            dropdownOptions.Add(new DropdownOption
            {
                DisplayValue = dropdownValue,
                AgentNumber = contract.AgentNumber,
                MarketCode = contract.MarketCode,
                CompanyCode = contract.CompanyCode,
                AgentLevel = contract.Level
            });
        }

        return [.. dropdownOptions.OrderBy(x => x.MarketCode).ThenBy(x => x.AgentNumber)];
    }

    /// <summary>
    /// Returns active hierarchy with basic agent information populated (market code, level, name, type, and so forth).
    /// </summary>
    /// <param name="loggedInAgentNumber">The currently logged in agent number (id) pulled from the HttpContext.</param>
    /// <param name="agentNumber"></param>
    /// <param name="marketCode"></param>
    /// <param name="agentLevel"></param>
    /// <param name="companyCode"></param>
    /// <param name="contractStatus">Removes all agents which do not have the specified contractStatus OR do not have agents in their downline with pending requirements. This means that not all agents returned will necessarily have the specified status.</param>
    /// <param name="includeAgentInformation">Will populate all agents with as much agent information as is available. Warning that this may take a while for large hierarchies, may produce a large payload, and may cause Swagger to become unresponsive.</param>
    /// <param name="includePendingRequirements">Will search Global Data for pending requirements by agent, then map to the output. Pending requirements will populate the PendingRequirements array if they exist.Otherwise, the array will be empty.</param>
    /// <param name="filterAgentsWithoutPendingRequirements">Removes all agents which do not have pending requirements OR do not have agents in their downline with pending requirements. Service will return HTTP 404 if no agents with pending requirements exist in the hierarchy.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ActiveHierarchyResponse?> GetAgentHierarchy(
        string loggedInAgentNumber,
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        ContractStatus? contractStatus,
        bool includeAgentInformation,
        bool includePendingRequirements,
        bool filterAgentsWithoutPendingRequirements,
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {agentNumber}");
        }

        if (!additionalAgentIds.Contains(agentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {agentNumber}");
        }

        var response = await AgentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            agentLevel,
            companyCode,
            contractStatus,
            includeAgentInformation,
            includePendingRequirements,
            filterAgentsWithoutPendingRequirements,
            cancellationToken);

        if (response != null)
        {
            return Mapper.Map<ActiveHierarchyResponse>(response);
        }

        return null;
    }

    public async Task<PendingRequirementsHierarchyResponse?> GetPendingRequirementsHierarchy(
        string loggedInAgentNumber,
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {agentNumber}");
        }

        if (!additionalAgentIds.Contains(agentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {agentNumber}");
        }

        var response = await AgentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            agentLevel,
            companyCode,
            null,
            true,
            true,
            true,
            cancellationToken);

        if (response != null)
        {
            return Mapper.Map<PendingRequirementsHierarchyResponse>(response);
        }

        return null;
    }

    public async Task<AgentContractInformation?> GetAgentInformation(
        string loggedInAgentNumber,
        string viewAsAgentNumber,
        string viewAsMarketCode,
        string viewAsAgentlevel,
        string viewAsCompanyCode,
        string downlineAgentNumber,
        string downlineMarketCode,
        string downlineAgentLevel,
        string downlineCompanyCode,
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {viewAsAgentNumber}");
        }

        if (!additionalAgentIds.Contains(viewAsAgentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {viewAsAgentNumber}");
        }

        var isAgentInDownline = await IsAgentInDownline(
            viewAsAgentNumber,
            viewAsMarketCode,
            viewAsAgentlevel,
            viewAsCompanyCode,
            downlineAgentNumber,
            downlineMarketCode,
            downlineAgentLevel,
            downlineCompanyCode,
            cancellationToken);

        if (!isAgentInDownline)
        {
            throw new Exception($"Agent {viewAsAgentNumber} does not have agent {downlineAgentNumber} in their downline.");
        }

        var agentInfoResponse = await AgentApiAccessor.GetAgentInformation(downlineAgentNumber, downlineMarketCode, downlineAgentLevel, downlineCompanyCode, cancellationToken);

        if (agentInfoResponse != null)
        {
            return Mapper.Map<AgentContractInformation>(agentInfoResponse);
        }

        return null;
    }

    public async Task<AgentAppointmentResponse?> GetActiveAppointments(
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
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {viewAsAgentNumber}");
        }

        if (!additionalAgentIds.Contains(viewAsAgentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {viewAsAgentNumber}");
        }

        var isAgentInDownline = await IsAgentInDownline(
             viewAsAgentNumber,
             viewAsMarketCode,
             viewAsAgentlevel,
             viewAsCompanyCode,
             downlineAgentNumber,
             downlineMarketCode,
             downlineAgentLevel,
             downlineCompanyCode,
             cancellationToken);

        if (!isAgentInDownline)
        {
            throw new Exception($"Agent {viewAsAgentNumber} does not have agent {downlineAgentNumber} in their downline.");
        }

        var activeAppointments = await AgentApiAccessor.GetActiveAppointments(downlineAgentNumber, downlineMarketCode, downlineAgentLevel, downlineCompanyCode, state, page, pageSize, cancellationToken);

        if (activeAppointments != null)
        {
            return Mapper.Map<AgentAppointmentResponse>(activeAppointments);
        }

        return null;
    }

    public async Task<MarketCodeInformationResponse> GetMarketCodeInformation(string marketCode)
    {
        var reverseMarketCodes = await DataStoreAccessor.GetReverseHierarchyMarketCodes();

        var nyMarketCodes = await DataStoreAccessor.GetNewYorkMarketCodes();

        return new MarketCodeInformationResponse
        {
            IsNewYorkMarketCode = nyMarketCodes.Any(mc => mc.Trim().ToLower() == marketCode.ToLower()),
            IsReverseHirearchyMarketCode = reverseMarketCodes.Any(mc => mc.Trim().ToLower() == marketCode.ToLower()),
        };
    }

    public async Task<FileResponse?> GetActiveHierarchyExport(
        string loggedInAgentNumber,
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        ContractStatus? contractStatus,
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {agentNumber}");
        }

        if (!additionalAgentIds.Contains(agentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {agentNumber}");
        }

        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{agentNumber}_ActiveHierarchy_{DateTime.Now:yyyy-MM-dd}.xlsx";
        var fileResponse = new FileResponse(fileName, fileType);

        var response = await AgentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            agentLevel,
            companyCode,
            contractStatus,
            true,
            false,
            false,
            cancellationToken);

        if (response != null)
        {
            var headers = FileExportEngine.CreateHeaders<ActiveHierarchyExport>();
            var mappedActiveHierarchy = new List<ActiveHierarchyExport>();
            var activeHierarchyExport = Mapper.Map<ActiveHierarchyExport>(response.Hierarchy);
            mappedActiveHierarchy.Add(activeHierarchyExport);

            if (response?.Hierarchy?.Branches != null)
            {
                foreach (var branch in response.Hierarchy.Branches)
                {
                    ProcessBranch(branch, mappedActiveHierarchy);
                }
            }

            fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedActiveHierarchy, "Active Hierarchy");
        }

        return fileResponse;
    }

    public async Task<FileResponse?> GetActivePendingRequirementsExport(
       string loggedInAgentNumber,
       string agentNumber,
       string marketCode,
       string agentLevel,
       string companyCode,
       CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {agentNumber}");
        }

        if (!additionalAgentIds.Contains(agentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {agentNumber}");
        }

        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{agentNumber}_ActivePendingRequirements_{DateTime.Now:yyyy-MM-dd}.xlsx";
        var fileResponse = new FileResponse(fileName, fileType);

        var response = await AgentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            agentLevel,
            companyCode,
            null,
            true,
            true,
            true,
            cancellationToken);

        if (response == null)
        {
            throw new Exception($"Get agent hierarchy from Agent API response was null for agent {agentNumber}");
        }
        else
        {
            var headers = FileExportEngine.CreateHeaders<HierarchyPendingRequirementExport>();
            var mappedPendingRequirements = new List<HierarchyPendingRequirementExport>();

            mappedPendingRequirements.AddRange(MapPendingRequirements(response.Hierarchy));

            if (response?.Hierarchy?.Branches != null && response.Hierarchy.Branches.Count > 0)
            {
                foreach (var branch in response.Hierarchy.Branches)
                {
                    mappedPendingRequirements.AddRange(MapPendingRequirements(branch));
                }
            }

            fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedPendingRequirements, "Pending Requirements");
        }

        return fileResponse;
    }

    public async Task<FileResponse?> GetAppointmentsExport(
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
        CancellationToken cancellationToken)
    {
        var additionalAgentIds = await AgentApiAccessor.GetAdditionalAgentIds(loggedInAgentNumber, cancellationToken);
        if (additionalAgentIds == null)
        {
            throw new Exception($"Unable to get additional agent ids for the agent {viewAsAgentNumber}");
        }

        if (!additionalAgentIds.Contains(viewAsAgentNumber))
        {
            throw new Exception($"Agent {loggedInAgentNumber} does not have access to agent {viewAsAgentNumber}");
        }

        var isAgentInDownline = await IsAgentInDownline(
            viewAsAgentNumber,
            viewAsMarketCode,
            viewAsAgentlevel,
            viewAsCompanyCode,
            downlineAgentNumber,
            downlineMarketCode,
            downlineAgentLevel,
            downlineCompanyCode,
            cancellationToken);

        if (!isAgentInDownline)
        {
            throw new Exception($"Agent {viewAsAgentNumber} does not have agent {downlineAgentNumber} in their downline.");
        }

        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"{downlineAgentNumber}_Appointments_{DateTime.Now:yyyy-MM-dd}.xlsx";
        var fileResponse = new FileResponse(fileName, fileType);

        var response = await AgentApiAccessor.GetActiveAppointments(
            downlineAgentNumber,
            downlineMarketCode,
            downlineAgentLevel,
            downlineCompanyCode,
            state,
            null,
            null,
            cancellationToken);

        if (response != null)
        {
            var headers = FileExportEngine.CreateHeaders<AgentAppointmentExport>();
            var mappedAppointments = Mapper.Map<List<AgentAppointmentExport>>(response.Appointments);

            fileResponse.FileData = FileExportEngine.CreateExcelDocument(headers, mappedAppointments, "Appointments");
        }

        return fileResponse;
    }

    public async Task<bool> DoesAgentHaveHierarchyDownline(string loggedInAgentNumber, CancellationToken cancellationToken)
    {
        return await AgentApiAccessor.GetAgentHasHierarchyDownline(loggedInAgentNumber, cancellationToken);
    }

    private void ProcessBranch(AgentAPI.AgentHierarchyBranch branch, List<ActiveHierarchyExport> mappedActiveHierarchy)
    {
        if (branch.AgentInformation != null)
        {
            var activeHierarchyExport = Mapper.Map<ActiveHierarchyExport>(branch);
            mappedActiveHierarchy.Add(activeHierarchyExport);
        }

        if (branch.Branches != null && branch.Branches.Count > 0)
        {
            foreach (var subBranch in branch.Branches)
            {
                ProcessBranch(subBranch, mappedActiveHierarchy);
            }
        }
    }

    private IEnumerable<HierarchyPendingRequirementExport> MapPendingRequirements(AgentAPI.AgentHierarchyBranch? source)
    {
        var mappedExports = new List<HierarchyPendingRequirementExport>();

        if (source?.PendingRequirements != null && source.PendingRequirements.Count != 0)
        {
            foreach (var requirement in source.PendingRequirements)
            {
                var mappedExport = Mapper.Map<HierarchyPendingRequirementExport>(source);
                mappedExport.Requirement = requirement.Description ?? string.Empty;
                mappedExport.Note = requirement.Note ?? string.Empty;

                if (!mappedExports.Any(e => e.AgentId == mappedExport.AgentId && e.Requirement == mappedExport.Requirement))
                {
                    mappedExports.Add(mappedExport);
                }
            }
        }

        if (source?.Branches != null && source.Branches.Count != 0)
        {
            foreach (var branch in source.Branches)
            {
                mappedExports.AddRange(MapPendingRequirements(branch)); // Recursive call for nested branches
            }
        }

        return mappedExports;
    }

    private string ReplaceUrlIncompatibleChars(string val)
    {
        return val.Replace("+", "-").Replace("/", "_").Replace("~", "=");
    }

    private Dictionary<string, Dictionary<string, List<string>>> ConvertAgentContractsResponseToAgentMarketCodeDictionary(List<Assurity.Agent.Contracts.AgentContract> contracts)
    {
        var result = new Dictionary<string, Dictionary<string, List<string>>>();

        if (contracts == null || contracts.Count == 0)
        {
            return result;
        }

        foreach (var contract in contracts)
        {
            if (contract == null)
            {
                continue;
            }

            if (result.ContainsKey(contract.AgentNumber))
            {
                if (result[contract.AgentNumber].ContainsKey(contract.MarketCode))
                {
                    if (!result[contract.AgentNumber][contract.MarketCode].Contains(contract.Level))
                    {
                        result[contract.AgentNumber][contract.MarketCode].Add(contract.Level);
                    }
                }
                else
                {
                    result[contract.AgentNumber].Add(
                        contract.MarketCode,
                        new List<string> { contract.Level });
                }
            }
            else
            {
                result.Add(
                    contract.AgentNumber,
                    new Dictionary<string, List<string>>()
                    {
                            { contract.MarketCode, new List<string> { contract.Level } }
                    });
            }
        }

        return result;
    }

    private async Task<bool> IsAgentInDownline(
        string viewAsAgentNumber,
        string viewAsMarketCode,
        string viewAsAgentLevel,
        string viewAsCompanyCode,
        string downlineAgentNumber,
        string downlineMarketCode,
        string downlineAgentLevel,
        string downlineCompanyCode,
        CancellationToken cancellationToken)
    {
        var downlines = await AgentApiAccessor.GetAgentHierarchyDownline(
            viewAsAgentNumber,
            viewAsMarketCode,
            viewAsAgentLevel,
            viewAsCompanyCode,
            cancellationToken);

        if (downlines == null)
        {
            throw new Exception($"Unable to retrieve downlines for agent {viewAsAgentNumber}");
        }

        if (downlines.Count == 0)
        {
            return false;
        }

        var isAgentInDownline = downlines.Any(a =>
            string.Equals(a.AgentNumber?.Trim(), downlineAgentNumber?.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(a.MarketCode?.Trim(), downlineMarketCode?.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(a.Level?.Trim(), downlineAgentLevel?.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(a.CompanyCode?.Trim(), downlineCompanyCode?.Trim(), StringComparison.OrdinalIgnoreCase));

        return isAgentInDownline;
    }
}