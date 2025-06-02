namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.AgentPortal.Accessors.DTOs;

public interface IDocumentVaultAccessor
{
    Task<PolicyPagesResponse?> GetPolicyAsync(string accessToken, string policyNumber);
}