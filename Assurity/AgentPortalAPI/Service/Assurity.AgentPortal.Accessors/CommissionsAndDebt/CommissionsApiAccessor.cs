namespace Assurity.AgentPortal.Accessors.CommissionsAndDebt;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.Commissions.Internal.Contracts.AgentStatementOptions;
using Assurity.Commissions.Internal.Contracts.Cycle;
using Assurity.Commissions.Internal.Contracts.PolicyDetails;
using Assurity.Commissions.Internal.Contracts.Summary;
using Assurity.Commissions.Internal.Contracts.WritingAgent;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class CommissionsApiAccessor : ICommissionsApiAccessor
{
    private const string DateFormat = "yyyyMMdd";

    public CommissionsApiAccessor(HttpClient httpClient, ILogger<CommissionsApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<CommissionsApiAccessor> Logger { get; }

    public async Task<CycleCommissionsResponse?> GetCommissionsCycle(
        List<string> agentIds,
        DateTimeOffset? cycleBeginDate,
        DateTimeOffset? cycleEndDate,
        List<string>? writingAgentIds,
        CancellationToken cancellationToken = default)
    {
        var url = "v1/commissions/cycle";

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", agentIds) }
        };

        if (cycleBeginDate != null)
        {
            queryParams.Add("CycleDateBegin", cycleBeginDate.Value.ToString(DateFormat));

            if (cycleEndDate != null && cycleBeginDate.Value != cycleEndDate.Value)
            {
                queryParams.Add("CycleDateEnd", cycleEndDate.Value.ToString(DateFormat));
            }
        }

        if (writingAgentIds != null)
        {
            queryParams.Add("WritingAgentIds", string.Join(",", writingAgentIds));
        }

        var newUrl = QueryHelpers.AddQueryString(url, queryParams);

        var response = await HttpClient.GetAsync(newUrl, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var serializedResponse = JsonConvert.DeserializeObject<CycleCommissionsResponse>(stringContent);

            return serializedResponse;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return null;
    }

    public async Task<PolicyDetailsResponse?> GetPolicyDetails(
        List<string> filteredAgentIds,
        CommissionParameters policyDetailsParameters,
        CancellationToken cancellationToken = default)
    {
        var endpoint = "v1/commissions/policydetails";
        var url = BuildUrlWithQueryString(filteredAgentIds, policyDetailsParameters, endpoint);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<PolicyDetailsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new PolicyDetailsResponse();
    }

    public async Task<SummaryCommissionsResponse?> GetCommissionsSummary(List<string> agentIds, CancellationToken cancellationToken = default)
    {
        var endpoint = "v1/commissions/summary";

        var url = QueryHelpers.AddQueryString(endpoint, "AgentIds", string.Join(",", agentIds));

        var response = await HttpClient.GetAsync(url, cancellationToken);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var commissionSummary = JsonConvert.DeserializeObject<SummaryCommissionsResponse>(stringContent);

            return commissionSummary;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new SummaryCommissionsResponse();
    }

    public async Task<WritingAgentsResponse?> GetWritingAgentDetails(
        List<string> agentIds,
        CommissionParameters filterParameters,
        CancellationToken cancellationToken = default)
    {
        var endpoint = "v1/commissions/writingagent";
        var url = BuildUrlWithQueryString(agentIds, filterParameters, endpoint);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var serializedResponse = JsonConvert.DeserializeObject<WritingAgentsResponse>(stringContent);

            return serializedResponse;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new WritingAgentsResponse();
    }

    public async Task<Stream?> GetAgentStatement(
    string agentId,
    DateTime cycleDate,
    AgentStatementType statementType,
    string sessionId,
    CancellationToken cancellationToken = default)
    {
        var endpoint = "v1/commissions/agentstatementsummary";
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentId", agentId },
            { "CycleDate", cycleDate.ToString() },
            { "SessionId", sessionId },
        };

        switch (statementType)
        {
            case AgentStatementType.Summary:
                queryParams.Add("ReportType", "Summary");
                break;
            case AgentStatementType.FirstYearDetail:
                queryParams.Add("ReportType", "Details");
                queryParams.Add("CommissionType", "FirstYear");
                break;
            case AgentStatementType.RenewalDetail:
                queryParams.Add("ReportType", "Details");
                queryParams.Add("CommissionType", "Renewal");
                break;
        }

        var url = QueryHelpers.AddQueryString(endpoint, queryParams);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);
        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return null;
    }

    public async Task<AgentStatementOptionsResponse?> GetAgentStatementOptions(List<string> agentIds, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "agentIds", string.Join(',', agentIds) }
        };

        var endpoint = $"v1/commissions/AgentStatementOptions";
        var url = QueryHelpers.AddQueryString(endpoint, queryParams);
        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<AgentStatementOptionsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new AgentStatementOptionsResponse();
    }

    private static string BuildUrlWithQueryString(
        List<string> filteredAgentIds,
        CommissionParameters commissionParameters,
        string url)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "AgentIds", string.Join(",", filteredAgentIds) },
        };

        if (commissionParameters.Page != null && commissionParameters.PageSize != null)
        {
            queryParams.Add("Page", commissionParameters.Page.ToString());
            queryParams.Add("PageSize", commissionParameters.PageSize.ToString());
        }

        if (commissionParameters.CycleBeginDate != null)
        {
            queryParams.Add("CycleDateBegin", commissionParameters.CycleBeginDate.Value.ToString(DateFormat));

            if (commissionParameters.CycleEndDate != null && commissionParameters.CycleBeginDate != commissionParameters.CycleEndDate)
            {
                queryParams.Add("CycleDateEnd", commissionParameters.CycleEndDate.Value.ToString(DateFormat));
            }
        }

        if (commissionParameters.WritingAgentIds != null
            && commissionParameters.WritingAgentIds.Count > 0)
        {
            queryParams.Add("WritingAgentIds", string.Join(",", commissionParameters.WritingAgentIds));
        }

        if (commissionParameters.OrderBy != null && commissionParameters.SortDirection != null)
        {
            queryParams.Add("OrderBy", commissionParameters.OrderBy);
            queryParams.Add("SortDirection", commissionParameters.SortDirection.Value.ToString("G"));
        }

        if (commissionParameters.PolicyNumber != null)
        {
            queryParams.Add("PolicyNumber", commissionParameters.PolicyNumber);
        }

        if (commissionParameters.DisablePagination != null)
        {
            queryParams.Add("DisablePagination", commissionParameters.DisablePagination.ToString());
        }

        return QueryHelpers.AddQueryString(url, queryParams);
    }
}
