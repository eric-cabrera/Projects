namespace Assurity.AgentPortal.Managers.PolicyInfo;

using Assurity.AgentPortal.Contracts.Shared;

public interface IDocumentVaultManager
{
    Task<FileResponse?> GetPolicyPages(string policyNumber);
}
