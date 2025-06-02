namespace Assurity.AgentPortal.Accessors.ApplicationTracker;

using System.Net.Http;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.ApplicationTracker.Contracts;
using Assurity.ApplicationTracker.Contracts.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class ApplicationTrackerApiAccessor : IApplicationTrackerApiAccessor
{
    public ApplicationTrackerApiAccessor(HttpClient httpClient, ILogger<ApplicationTrackerApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<ApplicationTrackerApiAccessor> Logger { get; }

    public async Task<PagedEvents?> GetCases(
        string agentId,
        CaseManagementParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var endpoint = "/api/v1/Tracker/Event";

        var pageNumber = parameters?.PageNumber > 0 ? (int)parameters.PageNumber : 1;
        var pageSize = parameters?.PageSize > 0 ? (int)parameters.PageSize : 10;

        var queryParams = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("PageNumber", pageNumber.ToString()),
            new KeyValuePair<string, string?>("PageSize", pageSize.ToString())
        };

        if (parameters?.ViewAsAgentId != null)
        {
            queryParams.Add(new KeyValuePair<string, string?>("AgentId", parameters.ViewAsAgentId.ToString()));
        }
        else
        {
            queryParams.Add(new KeyValuePair<string, string?>("AgentId", agentId.ToString()));
        }

        if (parameters?.SortColumn != null)
        {
            queryParams.Add(new KeyValuePair<string, string?>("SortColumn", parameters.SortColumn));
        }

        if (parameters?.SortOrder != null)
        {
            var sortOrder = parameters.SortOrder == SortDirection.ASC ?
                System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending;

            queryParams.Add(new KeyValuePair<string, string?>("SortOrder", sortOrder.ToString()));
        }

        if (parameters?.EventTypes != null)
        {
            parameters.EventTypes.Split(';').ToList().ForEach(eventType =>
            {
                queryParams.Add(new KeyValuePair<string, string?>("EventTypes", eventType));
            });
            queryParams.Add(new KeyValuePair<string, string?>("EventTypes", "ExpiredByDate"));
        }

        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.InterviewStarted.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.InterviewCompleted.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.RecipientSent.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.RecipientDeclined.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.EnvelopeVoided.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.RecipientCompleted.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.ApplicationSubmitted.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.Expired.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.CaseStarted.ToString()));
        queryParams.Add(new KeyValuePair<string, string?>("EventTypesToShow", EventType.ReceivedQuote.ToString()));

        if (parameters?.ProductTypes != null)
        {
            parameters.ProductTypes.Split(';').ToList().ForEach(product =>
            {
                queryParams.Add(new KeyValuePair<string, string?>("Products", product));
            });
        }

        if (parameters?.PrimaryInsuredName != null)
        {
            queryParams.Add(new KeyValuePair<string, string?>("PrimaryInsuredName", parameters.PrimaryInsuredName));
        }

        if (parameters?.CreatedDateBegin != null)
        {
            queryParams.Add(new KeyValuePair<string, string?>("CreatedDateBegin", parameters.CreatedDateBegin?.ToString()));
        }

        if (parameters?.CreatedDateEnd != null)
        {
            queryParams.Add(new KeyValuePair<string, string?>("CreatedDateEnd", parameters.CreatedDateEnd?.ToString()));
        }

        var queryString = string.Join("&", queryParams
            .Select(param => $"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value ?? string.Empty)}"));

        var url = $"{endpoint}?{queryString}";

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<PagedEvents>(stringContent);

            if (data != null)
            {
                return data;
            }
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new PagedEvents();
    }

    public async Task<EventFilterOptions?> GetFilterOptions(
        string agentId,
        CancellationToken cancellationToken = default)
    {
        var endpoint = "/api/v1/Tracker/FilterOptions";

        var queryParams = new Dictionary<string, string?>
        {
            { "AgentId", agentId },
        };

        var url = QueryHelpers.AddQueryString(endpoint, queryParams);

        var response = await HttpClient.GetAsync(url, cancellationToken);

        var stringContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<EventFilterOptions>(stringContent);

            return data;
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new EventFilterOptions();
    }
}
