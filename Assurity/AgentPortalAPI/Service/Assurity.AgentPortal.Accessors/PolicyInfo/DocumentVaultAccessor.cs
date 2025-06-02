namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.DocumentVault.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class DocumentVaultAccessor : IDocumentVaultAccessor
{
    public DocumentVaultAccessor(
        IDocumentVaultClient documentVaultClient,
        ILogger<DocumentVaultAccessor> logger)
    {
        DocumentVaultClient = documentVaultClient;
        Logger = logger;
    }

    private IDocumentVaultClient DocumentVaultClient { get; }

    private ILogger<DocumentVaultAccessor> Logger { get; }

    public async Task<PolicyPagesResponse?> GetPolicyAsync(string accessToken, string policyNumber)
    {
        var response = await DocumentVaultClient.GetPolicyPagesAsync(accessToken, policyNumber);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<PolicyPagesResponse>(responseString);
        }
        else
        {
            Logger.LogError("Endpoint: {Endpoint} | Returned response: {StatusCode} | {Content}", response.RequestMessage?.RequestUri, response.StatusCode, responseString);
            return null;
        }
    }
}