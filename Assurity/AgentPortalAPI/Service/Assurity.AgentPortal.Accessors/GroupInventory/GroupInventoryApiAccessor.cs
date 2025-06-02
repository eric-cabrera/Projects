namespace Assurity.AgentPortal.Accessors.GroupInventory;

using Assurity.Groups.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class GroupInventoryApiAccessor : IGroupInventoryApiAccessor
{
    public GroupInventoryApiAccessor(
        HttpClient httpClient,
        ILogger<GroupInventoryApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<GroupInventoryApiAccessor> Logger { get; }

    public async Task<GroupSummaryResponse?> GetGroupSummary(
        string agentNumber,
        GroupSummaryQueryParameters queryParameters,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"groups/{Uri.EscapeDataString(agentNumber)}/summary";

        var queryParams = new Dictionary<string, string?>
        {
            { "groupNumber", queryParameters.GroupNumber },
            { "groupName", queryParameters.GroupName },
            { "groupEffectiveDateStartDate", queryParameters.GroupEffectiveDateStartDate?.ToString("yyyy-MM-dd") },
            { "groupEffectiveDateEndDate", queryParameters.GroupEffectiveDateEndDate?.ToString("yyyy-MM-dd") },
            { "groupStatus", queryParameters.GroupStatus?.ToString() },
            { "orderBy", queryParameters.OrderBy?.ToString() },
            { "sortDirection", queryParameters.SortDirection.ToString() },
            { "page", queryParameters.Page?.ToString() },
            { "pageSize", queryParameters.PageSize?.ToString() }
        };

        var url = QueryHelpers.AddQueryString(endpoint, queryParams);
        var response = await HttpClient.GetAsync(url, cancellationToken);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<GroupSummaryResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new GroupSummaryResponse();
    }

    public async Task<GroupDetailResponse?> GetGroupDetail(
       string loggedInAgentNumber,
       string groupNumber,
       GroupDetailsQueryParameters queryParameters,
       CancellationToken cancellationToken = default)
    {
        var endpoint = $"groups/{Uri.EscapeDataString(groupNumber)}/details";

        var queryParams = new Dictionary<string, string?>
        {
            { "policyNumber", queryParameters.PolicyNumber },
            { "productDescription", queryParameters.ProductDescription },
            { "policyOwnerName", queryParameters.PolicyOwnerName },
            { "issueDateStartDate", queryParameters.IssueDateStartDate?.ToString("yyyy-MM-dd") },
            { "issueDateEndDate", queryParameters.IssueDateEndDate?.ToString("yyyy-MM-dd") },
            { "policyStatus", queryParameters.PolicyStatus?.ToString() },
            { "orderBy", queryParameters.OrderBy?.ToString() },
            { "sortDirection", queryParameters.SortDirection.ToString() },
            { "page", queryParameters.Page?.ToString() },
            { "pageSize", queryParameters.PageSize?.ToString() }
        };

        var url = QueryHelpers.AddQueryString(endpoint, queryParams);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Add("accept", "text/plain");
        requestMessage.Headers.Add("loggedInAgentId", loggedInAgentNumber);

        var response = await HttpClient.SendAsync(requestMessage, cancellationToken);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<GroupDetailResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new GroupDetailResponse();
    }
}