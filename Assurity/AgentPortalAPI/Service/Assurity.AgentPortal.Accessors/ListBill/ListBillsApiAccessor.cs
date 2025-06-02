namespace Assurity.AgentPortal.Accessors.ListBill;

using Assurity.ListBill.Service.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class ListBillsApiAccessor : IListBillsApiAccessor
{
    public ListBillsApiAccessor(HttpClient httpClient, ILogger<ListBillsApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<ListBillsApiAccessor> Logger { get; }

    public async Task<GroupsResponse?> GetGroups(
        string agentId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var getGroupsEndpoint = $"v1/groups/{agentId}?page={page}&pageSize={pageSize}";

        var response = await HttpClient
            .GetAsync(getGroupsEndpoint, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<GroupsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new GroupsResponse();
    }

    public async Task<ListBillResponse?> GetListBills(
        string groupId,
        CancellationToken cancellationToken)
    {
        var getListBillsEndpoint = $"v1/groups/{groupId}/listBills";

        var response = await HttpClient
            .GetAsync(getListBillsEndpoint, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<ListBillResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new ListBillResponse();
    }

    public async Task<Stream?> GetListBillData(
        string listBillId,
        CancellationToken cancellationToken)
    {
        var endpoint = $"v1/listBills/{listBillId}?fileFormat=Pdf";
        var response = await HttpClient.GetAsync(endpoint, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, responseBody);
        return null;
    }
}
