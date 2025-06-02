namespace Assurity.AgentPortal.Accessors.PolicyInfo;

public interface IIdentityServerAccessor
{
    Task<string?> GetAuthToken(string[] scopes);
}
