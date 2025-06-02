namespace Assurity.AgentPortal.Accessors.Agent;

using Assurity.AgentPortal.Accessors.Polly;
using Assurity.AgentPortal.Contracts.AgentContracts;
using global::Polly;
using global::Polly.Registry;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AgentAPI = Assurity.Agent.Contracts;

public class AgentApiAccessor : IAgentApiAccessor
{
    public AgentApiAccessor(
        HttpClient httpClient,
        ResiliencePipelineProvider<string> pipelineProvider,
        ILogger<AgentApiAccessor> logger)
    {
        HttpClient = httpClient;
        Pipeline = pipelineProvider.GetPipeline<HttpResponseMessage>(PollyPipelineKeys.HttpRetryPipeline);
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ResiliencePipeline<HttpResponseMessage> Pipeline { get; }

    private ILogger<AgentApiAccessor> Logger { get; }

    /// <summary>
    /// This endpoint will check the agent and all additional agent ids for their contract status - active, pending, terminated, JIT - and return a pre-defined Agent Center report access level.
    /// Documentation for what that means exists in Sharepoint here: https://assuritylife.sharepoint.com/:u:/s/AssurityWebProperties/ES3D9CTB34NPgAoMwlgyizIBLQPlMEnJamODpYwxffGZxw?e=Y0upQw.
    /// </summary>
    /// <param name="agentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>AgentAccessResponse containing the access level.</returns>
    public async Task<AgentAPI.AgentAccessResponse?> GetAgentAccess(string agentId, CancellationToken cancellationToken)
    {
        var endpoint = $"agents/{agentId}/access";

        var response = await Pipeline.ExecuteAsync(
            async token => await HttpClient.GetAsync(endpoint, token), cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentAPI.AgentAccessResponse>(stringContent);
        }

        Logger.LogError("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return null;
    }

    public async Task<List<string>> GetAdditionalAgentIds(string agentId, CancellationToken cancellationToken = default)
    {
        var endpoint = $"agents/{agentId}/additionalAgentIds";

        var response = await Pipeline.ExecuteAsync(
            async token => await HttpClient.GetAsync(endpoint, token), cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<List<string>>(stringContent);

            if (result != null)
            {
                return result;
            }
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return [];
    }

    public async Task<List<AgentAPI.AgentContract>?> GetAgentContracts(
        string agentId,
        bool includeAssociatedAgentNumbers,
        AgentAPI.MarketCodeFilters marketCodeFilter,
        CancellationToken cancellationToken,
        string? agentStatusFilters = null)
    {
        var marketCodeFilterQuery = $"&marketCodeFilter={marketCodeFilter}";
        var agentStatusFiltersQuery = agentStatusFilters != null ? $"&agentStatusFilters={agentStatusFilters}" : string.Empty;

        var getAgentContractsEndpoint = $"agents/{agentId}/activeContracts?includeAssociatedAgentNumbers={includeAssociatedAgentNumbers}{marketCodeFilterQuery}{agentStatusFiltersQuery}";
        var response = await HttpClient.GetAsync(getAgentContractsEndpoint, cancellationToken);
        var agentContractContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var agentContractsList = JsonConvert.DeserializeObject<List<AgentAPI.AgentContract>>(agentContractContent);

            return agentContractsList;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, agentContractContent);
        return [];
    }

    public async Task<List<AgentAPI.AgentContract>?> GetAgentVertaforeContracts(
        string agentId,
        CancellationToken cancellationToken)
    {
        var getAgentContractsEndpoint = $"agents/{agentId}/activeContracts";

        var queryParams = new Dictionary<string, string?>
        {
            { "includeAssociatedAgentNumbers", "true" },
            { "marketCodeFilter", "vertafore" }
        };

        var url = QueryHelpers.AddQueryString(getAgentContractsEndpoint, queryParams);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<List<AgentAPI.AgentContract>>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return [];
    }

    public async Task<List<string>?> GetAgentMarketCodes(
       string agentId,
       bool includeAssociatedAgentNumbers,
       CancellationToken cancellationToken)
    {
        var getAgentContractsEndpoint = $"agents/{agentId}/activeContracts";

        var queryParams = new Dictionary<string, string?>
        {
            { "IncludeAssociatedAgentNumbers", includeAssociatedAgentNumbers.ToString() },
            { "MarketCodeFilter", "none" }
        };

        var url = QueryHelpers.AddQueryString(getAgentContractsEndpoint, queryParams);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var agentContracts = JsonConvert.DeserializeObject<List<AgentAPI.AgentContract>>(stringContent);
            return agentContracts?.Select(agentContract => agentContract.MarketCode).ToList();
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return [];
    }

    public async Task<AgentAPI.AgentInformation?> GetAgentInformation(
        string agentId,
        CancellationToken cancellationToken)
    {
        var getAgentInformationEndpoint = $"agents/{agentId}/information";
        var response = await HttpClient.GetAsync(getAgentInformationEndpoint, cancellationToken);
        var agentInformationContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentAPI.AgentInformation>(agentInformationContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, agentInformationContent);
        return new AgentAPI.AgentInformation();
    }

    public async Task<AgentAPI.ActiveHierarchy.AgentContractInformation?> GetAgentInformation(
        string agentNumber,
        string marketCode,
        string level,
        string companyCode,
        CancellationToken cancellationToken)
    {
        var endpoint = $"agents/{agentNumber}/marketCode/{marketCode}/level/{level}/companyCode/{companyCode}/information";
        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentAPI.ActiveHierarchy.AgentContractInformation>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new AgentAPI.ActiveHierarchy.AgentContractInformation();
    }

    public async Task<AgentAPI.ActiveHierarchy.ActiveHierarchyResponse?> GetAgentHierarchy(
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
        var hierarchyEndpoint = $"agents/{Uri.EscapeDataString(agentNumber)}/marketCode/{Uri.EscapeDataString(marketCode)}/level/{Uri.EscapeDataString(agentLevel)}/companyCode/{Uri.EscapeDataString(companyCode)}/activeHierarchy" +
                                $"?includeAgentInformation={includeAgentInformation}" +
                                $"&includePendingRequirements={includePendingRequirements}" +
                                $"&filterAgentsWithoutPendingRequirements={filterAgentsWithoutPendingRequirements}";

        if (contractStatus.HasValue)
        {
            hierarchyEndpoint += $"&contractStatus={contractStatus.Value}";
        }

        var hierarchyResponse = await HttpClient.GetAsync(hierarchyEndpoint, cancellationToken);
        var hierarchyContent = await hierarchyResponse.Content.ReadAsStringAsync(cancellationToken);

        if (hierarchyResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentAPI.ActiveHierarchy.ActiveHierarchyResponse>(hierarchyContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", hierarchyResponse.RequestMessage?.RequestUri, hierarchyResponse.StatusCode, hierarchyContent);
        return new AgentAPI.ActiveHierarchy.ActiveHierarchyResponse();
    }

    public async Task<AgentAPI.ActiveHierarchy.AgentAppointmentResponse?> GetActiveAppointments(
        string agentNumber,
        string marketCode,
        string agentLevel,
        string companyCode,
        Contracts.Enums.State? state,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var appointmentsEndpoint = $"agents/{agentNumber}/marketCode/{marketCode}/level/{agentLevel}/companyCode/{companyCode}/activeHierarchy/appointments";
        var queryParameters = new List<string>();

        if (state.HasValue && state.Value != 0)
        {
            queryParameters.Add($"state={Uri.EscapeDataString(state.Value.ToString())}");
        }

        if (page.HasValue)
        {
            queryParameters.Add($"page={page.Value}");
        }

        if (pageSize.HasValue)
        {
            queryParameters.Add($"pageSize={pageSize.Value}");
        }

        if (queryParameters.Count > 0)
        {
            appointmentsEndpoint += "?" + string.Join("&", queryParameters);
        }

        var appointmentsResponse = await HttpClient.GetAsync(appointmentsEndpoint, cancellationToken);
        var appointmentsContent = await appointmentsResponse.Content.ReadAsStringAsync(cancellationToken);

        if (appointmentsResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentAPI.ActiveHierarchy.AgentAppointmentResponse>(appointmentsContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", appointmentsResponse.RequestMessage?.RequestUri, appointmentsResponse.StatusCode, appointmentsContent);
        return new AgentAPI.ActiveHierarchy.AgentAppointmentResponse();
    }

    public async Task<List<AgentAPI.AgentContract>?> GetAgentHierarchyDownline(
       string agentNumber,
       string marketCode,
       string agentLevel,
       string companyCode,
       CancellationToken cancellationToken)
    {
        var endpoint = $"agents/{Uri.EscapeDataString(agentNumber)}/marketCode/{Uri.EscapeDataString(marketCode)}/level/{Uri.EscapeDataString(agentLevel)}/companyCode/{Uri.EscapeDataString(companyCode)}/activeHierarchy/contracts/downline";

        HttpClient.DefaultRequestHeaders.Clear();
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<List<AgentAPI.AgentContract>>(responseContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, responseContent);
        return [];
    }

    public async Task<bool> GetAgentHasHierarchyDownline(string agentNumber, CancellationToken cancellationToken)
    {
        var endpoint = $"agent/{Uri.EscapeDataString(agentNumber)}/Downlines?includeDownlinesInResponse=false";

        HttpClient.DefaultRequestHeaders.Clear();
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<bool>(responseContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, responseContent);
        return false;
    }
}