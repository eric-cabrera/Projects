namespace Assurity.AgentPortal.Managers.Claims;

using Assurity.AgentPortal.Contracts.Claims;

public interface IClaimsManager
{
    Task<ClaimsResponse?> GetClaims(string agentId, ClaimsParameters parameters, CancellationToken cancellationToken = default);
}
