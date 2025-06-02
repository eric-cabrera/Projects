namespace Assurity.AgentPortal.Managers;

using System.Collections.Generic;
using System.Data;
using System.Threading;
using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Contracts;
using Microsoft.Extensions.Logging;

public abstract class BaseManager
{
    protected string GetChangePasswordErrorMessage(PingOneResponse passwordResponse)
    {
        if (passwordResponse is null)
        {
            // Error response was null, return generic error message.
            return "An unexpected error has occurred. Please try again.";
        }

        if (passwordResponse.Code == "REQUEST_FAILED")
        {
            bool passwordTooYoung = passwordResponse.Details.Any(d => d.Code.Contains("PASSWORD_TOO_YOUNG"));
            if (passwordTooYoung)
            {
                return "The password cannot be changed because it has not been long enough since the last password change.";
            }
        }
        else if (passwordResponse.Code == "INVALID_DATA")
        {
            bool oldPasswordIsWrong = passwordResponse.Details.Any(d => d.Message.Contains("The current password provided for the user is invalid"));

            if (oldPasswordIsWrong)
            {
                return "The provided old password is incorrect. Please try again.";
            }

            // If old password is correct then the problem is likely a password policy failing, need to parse which policy is failing.
            PingOneResponseDetails passwordPolicyErrorDetail;

            passwordPolicyErrorDetail = passwordResponse.Details.FirstOrDefault(d => d.Message == "New password did not satisfy password policy requirements");

            if (passwordPolicyErrorDetail == null)
            {
                passwordPolicyErrorDetail = passwordResponse.Details.FirstOrDefault(d => d.Message == "User password did not satisfy password policy requirements");
            }

            if (passwordPolicyErrorDetail != null)
            {
                // TODO: We may need to consult marketing on the wording of these messages. For now they are verbatim from PING.
                if (passwordPolicyErrorDetail.InnerError.Length != null)
                {
                    return passwordPolicyErrorDetail.InnerError.Length;
                }
                else if (passwordPolicyErrorDetail.InnerError.MinCharacters != null)
                {
                    return passwordPolicyErrorDetail.InnerError.MinCharacters;
                }
                else if (passwordPolicyErrorDetail.InnerError.MinComplexity != null)
                {
                    return "The proposed password is not acceptable because it is too simple.";
                }
                else if (passwordPolicyErrorDetail.InnerError.ExcludesCommonlyUsed != null)
                {
                    return passwordPolicyErrorDetail.InnerError.ExcludesCommonlyUsed;
                }
                else if (passwordPolicyErrorDetail.InnerError.MaxRepeatedCharacters != null)
                {
                    return passwordPolicyErrorDetail.InnerError.MaxRepeatedCharacters;
                }
                else if (passwordPolicyErrorDetail.InnerError.NotSimilarToCurrent != null)
                {
                    return "The new password is too similar to the current password.";
                }
                else if (passwordPolicyErrorDetail.InnerError.History != null)
                {
                    return "The new password cannot be a previously used password.";
                }
                else
                {
                    return "The new password does not meet the password policy.";
                }
            }
        }

        // Error is unhandled, return generic error message.
        return "An unexpected error has occurred. Please try again.";
    }
}
