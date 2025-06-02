namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Contracts;

public class AuthenticationManager : IAuthenticationManager
{
    public AuthenticationManager(IAuthenticationAccessor authenticationAccessor)
    {
        AuthenticationAccessor = authenticationAccessor;
    }

    private IAuthenticationAccessor AuthenticationAccessor { get; set; }

    public async Task<TokenResponse?> RefreshTokensAsync(string refreshToken, string clientId, string clientSecret, string scopes, string url, bool isAzureAd)
    {
        return await AuthenticationAccessor.RefreshTokenAsync(refreshToken, clientId, clientSecret, scopes, url, isAzureAd);
    }
}
