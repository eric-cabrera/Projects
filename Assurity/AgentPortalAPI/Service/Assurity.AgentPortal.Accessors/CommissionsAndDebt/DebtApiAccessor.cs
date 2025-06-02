namespace Assurity.AgentPortal.Accessors.CommissionsAndDebt;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.Commissions.Debt.Contracts.Advances;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class DebtApiAccessor : IDebtApiAccessor
{
    public DebtApiAccessor(HttpClient client, ILogger<DebtApiAccessor> logger)
    {
        HttpClient = client;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<DebtApiAccessor> Logger { get; }

    public async Task<AgentDetailsResponse?> GetUnsecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var url = BuildRequestUrl(agentId, "v1/debt/unsecuredAdvances", parameters);

        var response = await HttpClient.GetAsync(url, cancellationToken);
        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentDetailsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new AgentDetailsResponse();
    }

    public async Task<List<Agent>> GetAllUnsecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Page = 1;
        parameters.PageSize = 100;
        long pagesRemaining;

        var writingAgentDebt = new List<Agent>();

        do
        {
            var response = await GetUnsecuredAdvances(agentId, parameters, cancellationToken);

            if (response == null || !(response.Agents?.Any() ?? false))
            {
                return writingAgentDebt;
            }

            writingAgentDebt.AddRange(response.Agents);

            pagesRemaining = GetRemainingPages(response.TotalRecords, response.PageSize, response.Page);
            parameters.Page += 1;
        }
        while (pagesRemaining > 0);

        return writingAgentDebt;
    }

    public async Task<AgentDetailsResponse?> GetSecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var url = BuildRequestUrl(agentId, "v1/debt/securedAdvances", parameters);

        var response = await HttpClient.GetAsync(url, cancellationToken);
        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentDetailsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new AgentDetailsResponse();
    }

    public async Task<List<Agent>> GetAllSecuredAdvances(
        string agentId,
        DebtParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Page = 1;
        parameters.PageSize = 100;
        long pagesRemaining;

        var writingAgentDebt = new List<Agent>();

        do
        {
            var response = await GetSecuredAdvances(agentId, parameters, cancellationToken);

            if (response == null || !(response.Agents?.Any() ?? false))
            {
                return writingAgentDebt;
            }

            writingAgentDebt.AddRange(response.Agents);

            pagesRemaining = GetRemainingPages(response.TotalRecords, response.PageSize, response.Page);
            parameters.Page += 1;
        }
        while (pagesRemaining > 0);

        return writingAgentDebt;
    }

    private static long GetRemainingPages(long totalRecords, int pageSize, int page)
    {
        if (pageSize == 0)
        {
            return 0;
        }

        return Convert.ToInt64(Math.Ceiling((double)totalRecords / pageSize) - page);
    }

    private static string BuildRequestUrl(string agentId, string url, DebtParameters parameters)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentId", agentId },
            { "Page", parameters?.Page > 0 ? parameters.Page.ToString() : "1" },
            { "PageSize", parameters?.PageSize > 0 ? parameters.PageSize.ToString() : "10" },
        };

        if (parameters?.AgentId != null)
        {
            queryParams.Add("Filters.ViewAsAgentIds", parameters.AgentId);
        }

        if (parameters?.OrderBy != null && parameters?.SortDirection != null)
        {
            queryParams.Add("OrderBy", parameters.OrderBy.Value.ToString("G"));
            queryParams.Add("SortDirection", parameters.SortDirection.Value.ToString("G"));
        }

        if (!string.IsNullOrEmpty(parameters?.PolicyNumber))
        {
            queryParams.Add("Filters.PolicyNumber", parameters.PolicyNumber);
        }

        if (parameters?.WritingAgentIds?.Any() ?? false)
        {
            queryParams.Add("Filters.AgentIds", string.Join(",", parameters.WritingAgentIds));
        }

        // This is an enum.
        if (!string.IsNullOrEmpty(parameters?.Status))
        {
            queryParams.Add("Filters.Status", parameters.Status);
        }

        if (!string.IsNullOrEmpty(parameters?.HierarchyAgentId)
            && !string.IsNullOrEmpty(parameters?.HierarchyCompanyCode)
            && !string.IsNullOrEmpty(parameters?.HierarchyMarketCode)
            && !string.IsNullOrEmpty(parameters?.HierarchyLevel))
        {
            queryParams.Add("Filters.Hierarchy.AgentId", parameters.HierarchyAgentId);
            queryParams.Add("Filters.Hierarchy.CompanyCode", parameters.HierarchyCompanyCode);
            queryParams.Add("Filters.Hierarchy.MarketCode", parameters.HierarchyMarketCode);
            queryParams.Add("Filters.Hierarchy.AgentLevel", parameters.HierarchyLevel);
        }

        if (parameters != null && parameters.IncludeFilters)
        {
            queryParams.Add("IncludeFilterValues", "true");
        }

        return QueryHelpers.AddQueryString(url, queryParams);
    }
}
