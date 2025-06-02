namespace Assurity.AgentPortal.Managers.Integration
{
    public interface IIllustrationProManager
    {
        string GetCredentialsForIllustrationProForHomeOfficeUser(string username);

        Task<string?> GetCredentialsForIllustrationPro(
            string agentId,
            string userName,
            CancellationToken cancellationToken = default);

        Task<string?> GetIllustrationProAccountId(string credentials);
    }
}
