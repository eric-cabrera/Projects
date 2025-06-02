namespace Assurity.AgentPortal.Accessors.LifePortraits;

public interface ILifePortraitsApiAccessor
{
    Task<string?> GetURL(string credentials, CancellationToken cancellationToken);
}