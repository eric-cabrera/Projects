namespace Assurity.AgentPortal.Accessors;

using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Contracts;

public interface IProfileAccessor
{
    Task<PingOneResponse?> ChangePassword(string currentPassword, string newPassword, string confirmPassword, string accessToken, CancellationToken cancellationToken);

    Task<string> CreateMFADevice(string userId, MFAType mfaType, string value, string accessToken, CancellationToken cancellationToken = default);

    Task<PingOneResponse?> ActivateMFADevice(string userId, string oneTimePassword, string deviceId, string accessToken, CancellationToken cancellationToken = default);

    Task<PingOneResponse?> GetAllPingUserMFADevices(string userId, string accessToken, CancellationToken cancellationToken = default);

    Task<bool> DeleteMFADevice(string userId, string deviceId, string accessToken, CancellationToken cancellationToken = default);

    Task<bool> ResendMFACode(string userId, string deviceId, string accessToken, CancellationToken cancellationToken = default);

    Task<bool> UpdateUserContactInfo(string userId, MFAType mfaType, string value, string accessToken, CancellationToken cancellationToken = default);

    Task SendConfirmationEmails(string originalEmailAddress, string newEmailAddress, string accessToken, CancellationToken cancellationToken = default);
}
