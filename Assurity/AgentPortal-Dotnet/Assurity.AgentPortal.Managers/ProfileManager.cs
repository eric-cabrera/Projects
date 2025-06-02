namespace Assurity.AgentPortal.Managers;

using System.Collections.Generic;
using System.Data;
using System.Threading;
using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;

public class ProfileManager : BaseManager, IProfileManager
{
    private const string PINGONESUCCESS = "ACTIVE";
    private readonly ILogger<ProfileManager> logger;

    public ProfileManager(
        ILogger<ProfileManager> log,
        IProfileAccessor profileAccessor)
    {
        logger = log;
        ProfileAccessor = profileAccessor;
    }

    private IProfileAccessor ProfileAccessor { get; set; }

    public async Task<ChangePasswordResponse> ChangePassword(string userId, string currentPassword, string newPassword, string accessToken, CancellationToken cancellationToken)
    {
        var result = await ProfileAccessor.ChangePassword(userId, currentPassword, newPassword, accessToken, cancellationToken);

        var changePasswordResponse = new ChangePasswordResponse() { Success = true };

        if (result is null)
        {
            changePasswordResponse.Success = false;
            return changePasswordResponse;
        }

        if (string.IsNullOrEmpty(result.Status) || result.Status != "OK")
        {
            changePasswordResponse.Success = false;
            changePasswordResponse.Message = GetChangePasswordErrorMessage(result);
            logger.LogError("Failed to update password. reasons: {reason}", changePasswordResponse.Message);
        }

        return changePasswordResponse;
    }

    public async Task<string> ChangeMFADevice(string userId, MFAType type, string value, string accessToken, CancellationToken cancellationToken)
    {
        return await ProfileAccessor.CreateMFADevice(userId, type, value, accessToken, cancellationToken);
    }

    public async Task<ConfirmMFADeviceResponse> ConfirmMFADeviceChange(string userId, string deviceId, string oldDeviceId, string oneTimePassword, string accessToken, CancellationToken cancellationToken)
    {
        var deviceList = await DeleteInactiveMFADevices(userId, deviceId, accessToken, cancellationToken);

        var activateResult = await ProfileAccessor.ActivateMFADevice(userId, oneTimePassword, deviceId, accessToken, cancellationToken);

        if (activateResult is null)
        {
            return new ConfirmMFADeviceResponse { Success = false };
        }

        if (activateResult.Status == PINGONESUCCESS)
        {
            var deleteDevice = deviceList.Where(x => x.Id == oldDeviceId).FirstOrDefault();

            var deleteDeviceId = deleteDevice?.Id;

            var updateDevice = deviceList.Where(x => x.Id == deviceId).FirstOrDefault();

            if (deleteDeviceId != null)
            {
                var deleteResult = await ProfileAccessor.DeleteMFADevice(userId, deleteDeviceId, accessToken, cancellationToken);
            }

            if (updateDevice != null)
            {
                var updateResult = await ProfileAccessor.UpdateUserContactInfo(
                    userId,
                    updateDevice.Type,
                    updateDevice.Type == MFAType.EMAIL ? updateDevice.Email : updateDevice.Phone,
                    accessToken,
                    cancellationToken);
            }

            if (deleteDevice != null && updateDevice != null && deleteDevice.Type == MFAType.EMAIL)
            {
                await ProfileAccessor.SendConfirmationEmails(deleteDevice.Email, updateDevice.Email, accessToken, cancellationToken);
            }

            return new ConfirmMFADeviceResponse { Success = true };
        }
        else if (activateResult.Code == "INVALID_DATA")
        {
            return new ConfirmMFADeviceResponse
            {
                Success = false,
                AttemptsLeft = activateResult.Details.FirstOrDefault()?.InnerError?.AttemptsRemaining
            };
        }

        return new ConfirmMFADeviceResponse { Success = false };
    }

    public async Task<List<PingOneResponseDevice>> GetActiveMFADevices(string userId, string accessToken, CancellationToken cancellationToken)
    {
        var result = await ProfileAccessor.GetAllPingUserMFADevices(userId, accessToken, cancellationToken);

        if (result is null || result.Embedded is null || result.Embedded.Devices is null)
        {
            return new List<PingOneResponseDevice>();
        }

        return result.Embedded.Devices.Where(x => x.Status == "ACTIVE").OrderByDescending(x => x.Type).ToList();
    }

    public async Task<bool> RemoveMFADevice(string userId, string deviceId, string accessToken, CancellationToken cancellationToken)
    {
        return await ProfileAccessor.DeleteMFADevice(userId, deviceId, accessToken, cancellationToken);
    }

    public async Task<bool> ResendMFACode(string userId, string deviceId, string accessToken, CancellationToken cancellationToken)
    {
        return await ProfileAccessor.ResendMFACode(userId, deviceId, accessToken, cancellationToken);
    }

    private async Task<List<PingOneResponseDevice>> DeleteInactiveMFADevices(string userId, string newDeviceId, string accessToken, CancellationToken cancellationToken)
    {
        var devicesResponse = await ProfileAccessor.GetAllPingUserMFADevices(userId, accessToken, cancellationToken);

        if (devicesResponse is null)
        {
            return new List<PingOneResponseDevice>();
        }

        var devicesToDelete = devicesResponse.Embedded.Devices.Where(x => x.Status != "ACTIVE" && x.Id != newDeviceId).Select(x => x.Id);

        if (devicesToDelete.Any())
        {
            var deleteTasks = new List<Task>();
            foreach (string deviceId in devicesToDelete)
            {
                deleteTasks.Add(ProfileAccessor.DeleteMFADevice(userId, deviceId, accessToken, cancellationToken));
            }

            await Task.WhenAll(deleteTasks);

            devicesResponse = await ProfileAccessor.GetAllPingUserMFADevices(userId, accessToken, cancellationToken);
        }

        if (devicesResponse is null)
        {
            return new List<PingOneResponseDevice>();
        }

        return devicesResponse.Embedded.Devices;
    }
}
