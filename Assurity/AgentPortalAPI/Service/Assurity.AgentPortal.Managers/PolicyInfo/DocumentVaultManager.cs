namespace Assurity.AgentPortal.Managers.PolicyInfo;

using System.Net.Mime;
using Assurity.AgentPortal.Accessors.PolicyInfo;
using Assurity.AgentPortal.Contracts.Shared;
using Microsoft.Extensions.Logging;

public class DocumentVaultManager : IDocumentVaultManager
{
    private readonly ILogger logger;

    public DocumentVaultManager(
        ILogger<DocumentVaultManager> logger,
        IIdentityServerAccessor identityServerAccessor,
        IDocumentVaultAccessor documentVaultAccessor)
    {
        this.logger = logger;
        IdentityServerAccessor = identityServerAccessor;
        DocumentVaultAccessor = documentVaultAccessor;
    }

    private IIdentityServerAccessor IdentityServerAccessor { get; }

    private IDocumentVaultAccessor DocumentVaultAccessor { get; }

    public async Task<FileResponse?> GetPolicyPages(string policyNumber)
    {
        var scopes = new string[] { "documentVault" };
        var accessToken = await IdentityServerAccessor.GetAuthToken(scopes);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            logger.LogError("IdentityServer returned empty access token.");
            return null;
        }

        var documentResponse = await DocumentVaultAccessor.GetPolicyAsync(accessToken, policyNumber);
        if (documentResponse == null)
        {
            return null;
        }

        return new FileResponse($"policy-{policyNumber}.pdf", MediaTypeNames.Application.Pdf)
        {
            FileData = Convert.FromBase64String(documentResponse.EncodedFile)
        };
    }
}
