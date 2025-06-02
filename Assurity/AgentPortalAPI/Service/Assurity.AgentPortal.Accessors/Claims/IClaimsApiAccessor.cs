namespace Assurity.AgentPortal.Accessors.Claims;

using Assurity.AgentPortal.Contracts.Claims;
using Assurity.Claims.Contracts.AssureLink;

public interface IClaimsApiAccessor
{
    Task<AssureLinkClaimResponse?> GetClaims(string agentId, ClaimsParameters parameters, CancellationToken cancellationToken = default);
}