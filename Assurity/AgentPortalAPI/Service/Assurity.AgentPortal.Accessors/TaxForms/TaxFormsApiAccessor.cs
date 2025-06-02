namespace Assurity.AgentPortal.Accessors.TaxForms;

using Assurity.TaxForms.Contracts.V1;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class TaxFormsApiAccessor : ITaxFormsApiAccessor
{
    public TaxFormsApiAccessor(HttpClient httpClient, ILogger<TaxFormsApiAccessor> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    private HttpClient HttpClient { get; }

    private ILogger<TaxFormsApiAccessor> Logger { get; }

    public async Task<GetAgentFormsResponse?> GetTaxForms(
        string agentId,
        CancellationToken cancellationToken)
    {
        var url = $"agents/{agentId}/forms";

        var response = await HttpClient
            .GetAsync(url, cancellationToken);

        var stringContent = await response
            .Content
            .ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<GetAgentFormsResponse>(stringContent);
        }

        Logger.LogDebug("{Endpoint} returned response: {httpResponseMessage.StatusCode} | {stringContent}", response.RequestMessage?.RequestUri, response.StatusCode, stringContent);
        return new GetAgentFormsResponse();
    }

    public async Task<Stream?> GetTaxForm(
        string agentId,
        string formId,
        CancellationToken cancellationToken)
    {
        var endpoint = $"agents/{agentId}/forms/{formId}";
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