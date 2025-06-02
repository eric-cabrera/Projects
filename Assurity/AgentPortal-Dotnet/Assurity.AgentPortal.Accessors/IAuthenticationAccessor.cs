namespace Assurity.AgentPortal.Accessors;

using Assurity.AgentPortal.Contracts;

public interface IAuthenticationAccessor
{
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken, string clientId, string clientSecret, string scopes, string url, bool isAzureAd);
}
