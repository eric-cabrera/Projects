namespace Assurity.AgentPortal.Managers.Integration
{
    public interface ILifePortraitsManager
    {
        Task<string> GetCredentialsForLifePortraitsForHomeOfficeUser(string username);

        Task<string?> GetCredentialsForLifePortraits(
            string agentId,
            string userName,
            CancellationToken cancellationToken = default);

        Task<string?> GetLifePortraitsURL(
            string credentials,
            CancellationToken cancellationToken = default);
    }
}
