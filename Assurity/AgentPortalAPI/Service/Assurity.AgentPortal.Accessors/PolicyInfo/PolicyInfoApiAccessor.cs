namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.PolicyInfo.Contracts.V1;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

public class PolicyInfoApiAccessor : IPolicyInfoApiAccessor
{
    public PolicyInfoApiAccessor(HttpClient httpClient, ILogger<PolicyInfoApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<PolicyInfoApiAccessor> Logger { get; }

    public async Task<Policy?> GetPolicyInfo(string policyNumber, string agentId)
    {
        var policyAgentEndpoint = $"agents/{agentId}/downlinePolicies/{policyNumber}";

        var httpResponse = await HttpClient
            .GetAsync(policyAgentEndpoint);

        var stringContent = await httpResponse
            .Content
            .ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<Policy>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", httpResponse.RequestMessage?.RequestUri, httpResponse.StatusCode, stringContent);
        return new Policy();
    }

    public async Task<PolicyInfoAPI.PolicyStatusCounts?> GetPolicyStatusCounts(string agentId)
    {
        var policyStatusCountsEndpoint = $"agents/{agentId}/downlinePolicies/statusCounts";

        var httpResponse = await HttpClient
            .GetAsync(policyStatusCountsEndpoint);

        var stringContent = await httpResponse
            .Content
            .ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<PolicyInfoAPI.PolicyStatusCounts>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", httpResponse.RequestMessage?.RequestUri, httpResponse.StatusCode, stringContent);
        return new PolicyInfoAPI.PolicyStatusCounts();
    }

    public async Task<PolicyInfoAPI.PolicySummariesResponse?> GetPolicySummaries(
        string agentId,
        Status status,
        string? queryString)
    {
        var policySummaryEndpoint =
            $"agents/{agentId}/downlinePolicies/summaries/{status}{queryString ?? string.Empty}";

        var httpResponse = await HttpClient
            .GetAsync(policySummaryEndpoint);

        var stringContent = await httpResponse
            .Content
            .ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<PolicyInfoAPI.PolicySummariesResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", httpResponse.RequestMessage?.RequestUri, httpResponse.StatusCode, stringContent);
        return new PolicyInfoAPI.PolicySummariesResponse();
    }

    public async Task<PolicyInfoAPI.RequirementSummaryResponse?> GetPendingPolicyRequirementSummaries(string agentId, string? queryString)
    {
        var policySummaryEndpoint =
            $"agents/{agentId}/downlinePolicies/pendingPolicyRequirements{queryString ?? string.Empty}";

        var httpResponse = await HttpClient
            .GetAsync(policySummaryEndpoint);

        var stringContent = await httpResponse
            .Content
            .ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<PolicyInfoAPI.RequirementSummaryResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", httpResponse.RequestMessage?.RequestUri, httpResponse.StatusCode, stringContent);
        return new PolicyInfoAPI.RequirementSummaryResponse();
    }

    public async Task<bool> CheckAgentAccessToPolicy(string agentId, string policyNumber)
    {
        var policySummaryEndpoint =
            $"agents/{agentId}/downlinePolicies/{policyNumber}/checkAccess";

        var httpResponse = await HttpClient
            .GetAsync(policySummaryEndpoint);

        var stringContent = await httpResponse
            .Content
            .ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", httpResponse.RequestMessage?.RequestUri, httpResponse.StatusCode, stringContent);
            return false;
        }

        return httpResponse.IsSuccessStatusCode;
    }
}