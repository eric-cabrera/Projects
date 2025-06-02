namespace Assurity.AgentPortal.Accessors.IllustrationPro;

public interface IIllustrationProApiAccessor
{
    Task<string?> GetAccountId(string credentials);
}