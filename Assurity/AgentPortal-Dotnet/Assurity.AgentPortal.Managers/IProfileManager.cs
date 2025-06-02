namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Contracts;

public interface IProfileManager
{
    Task<ChangePasswordResponse> ChangePassword(string userId, string currentPassword, string newPassword, string accessToken, CancellationToken cancellationToken);

    Task<string> ChangeMFADevice(string userId, MFAType type, string value, string accessToken, CancellationToken cancellationToken);

    Task<ConfirmMFADeviceResponse> ConfirmMFADeviceChange(string userId, string deviceId, string oldDeviceId, string oneTimePassword, string accessToken, CancellationToken cancellationToken);

    Task<List<PingOneResponseDevice>> GetActiveMFADevices(string userId, string accessToken, CancellationToken cancellationToken);

    Task<bool> RemoveMFADevice(string userId, string deviceId, string accessToken, CancellationToken cancellationToken);

    Task<bool> ResendMFACode(string userId, string deviceId, string accessToken, CancellationToken cancellationToken);
}
