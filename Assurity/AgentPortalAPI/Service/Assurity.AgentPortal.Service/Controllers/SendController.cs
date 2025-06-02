namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Contracts.Send;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Assurity.AgentPortal.Managers.Send;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Service.Models;
using Assurity.AgentPortal.Service.Validation;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.PendingActiveTerminated)]
public class SendController(
    IConfigurationManager configurationManager,
    IFileValidator fileValidator,
    ILogger<SendController> logger,
    IPolicyInfoManager policyInfoManager,
    ISendManager sendManager) : BaseController(configurationManager, logger)
{
    private IFileValidator FileValidator { get; } = fileValidator;

    private IPolicyInfoManager PolicyInfoManager { get; } = policyInfoManager;

    private ISendManager SendManager { get; } = sendManager;

    [HttpPost]
    [Route("TakeAction")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TakeAction([FromForm] UploadRequest uploadRequest)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (!FileValidator.IsMultipartContentType(Request.ContentType))
        {
            Logger.LogWarning("Invalid content type found in the request for agent {AgentId}.", agentId);
            return BadRequestResponse("Invalid content type found in the request.");
        }

        // Check initial model validation
        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var policyNumber = uploadRequest.UploadData.PolicyNumber;

        try
        {
            var isAgentAuthorizedToAccessPolicy = await PolicyInfoManager
                .CheckAgentAccessToPolicy(agentId, policyNumber);

            if (!isAgentAuthorizedToAccessPolicy)
            {
                Logger.LogWarning(
                    "Agent {AgentId} does not have access to policy {PolicyNumber}.",
                    agentId,
                    policyNumber);

                return Forbid();
            }
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }

        var actionRequest = new ActionRequest
        {
            AgentId = agentId,
            Message = uploadRequest.UploadData.Message,
            PolicyNumber = policyNumber
        };

        var hasMessage = !string.IsNullOrWhiteSpace(actionRequest.Message);
        var fileCount = uploadRequest.FilesToUpload?.Count ?? 0;
        var hasFiles = fileCount > 0;

        if (!hasMessage && !hasFiles)
        {
            ModelState.AddModelError("TakeAction", "Either a message or at least one file is required.");
        }
        else if (hasFiles)
        {
            var maximumFilesPerUpload = ConfigurationManager.TakeActionMaximumFilesPerUpload;

            if (maximumFilesPerUpload > 0 && fileCount > maximumFilesPerUpload)
            {
                ModelState.AddModelError(
                    "TakeAction",
                    "The number of files requested exceeds the maximum files per upload.");
            }
            else
            {
                var files = new List<File>();

                foreach (var formFile in uploadRequest.FilesToUpload)
                {
                    var file = await FileValidator.ProcessFormFile(formFile, ModelState);

                    if ((file?.Bytes?.Length ?? 0) == 0)
                    {
                        ModelState.AddModelError("TakeAction", "Unable to process files.");
                        break;
                    }

                    files.Add(file);
                }

                actionRequest.Files = files;
            }
        }

        // Additional check for message or file(s) errors.
        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        try
        {
            await SendManager.SendMessageAndFilesToGlobal(actionRequest);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}