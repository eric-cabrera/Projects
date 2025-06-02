namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Contracts;

public interface IAuthenticationManager
{
    Task<TokenResponse?> RefreshTokensAsync(string refreshToken, string clientId, string clientSecret, string scopes, string url, bool isAzureAd);
}
