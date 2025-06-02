namespace Assurity.AgentPortal.Managers.Claims;

using Assurity.AgentPortal.Accessors.Claims;
using Assurity.AgentPortal.Contracts.Claims;
using AutoMapper;

public class ClaimsManager : IClaimsManager
{
    public ClaimsManager(
        IMapper mapper,
        IClaimsApiAccessor claimsApiAccessor)
    {
        Mapper = mapper;
        ClaimsApiAccessor = claimsApiAccessor;
    }

    private IMapper Mapper { get; }

    private IClaimsApiAccessor ClaimsApiAccessor { get; }

    public async Task<ClaimsResponse?> GetClaims(
        string agentId,
        ClaimsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var claimsResponse = await ClaimsApiAccessor.GetClaims(agentId, parameters, cancellationToken);

        if (claimsResponse == null)
        {
            return null;
        }

        return Mapper.Map<ClaimsResponse>(claimsResponse);
    }
}
