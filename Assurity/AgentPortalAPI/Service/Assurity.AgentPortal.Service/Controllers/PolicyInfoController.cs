namespace Assurity.AgentPortal.Service.Controllers;

using System;
using Assurity.AgentPortal.Contracts.PolicyInfo;
using Assurity.AgentPortal.Managers.PolicyInfo;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Assurity.AgentPortal.Utilities.Logging;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.PendingActiveTerminated)]
public class PolicyInfoController(
    ILogger<PolicyInfoController> logger,
    IHttpRequestMessageValuesProvider httpRequestMessageValuesProvider,
    IPolicyInfoManager policyInfoManager,
    IDocumentVaultManager documentVaultManager,
    IExecute360DocumentManager execute360DocumentManager,
    IConfigurationManager configurationManager) : BaseController(configurationManager, logger)
{
    private IPolicyInfoManager PolicyInfoManager { get; } = policyInfoManager;

    private IDocumentVaultManager DocumentVaultManager { get; } = documentVaultManager;

    private IExecute360DocumentManager Execute360DocumentManager { get; } = execute360DocumentManager;

    private IHttpRequestMessageValuesProvider HttpRequestMessageValuesProvider { get; } = httpRequestMessageValuesProvider;

    [HttpGet("Policies/Summaries/{status}", Name = nameof(GetPoliciesByStatus))]
    [ProducesResponseType(typeof(PolicySummariesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPoliciesByStatus(
        Status status)
    {
        var queryString = Request.QueryString.ToString();

        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var policySummaries = await PolicyInfoManager.GetPolicySummaries(agentId, status, queryString);
            if (policySummaries == null || policySummaries.Policies == null)
            {
                Logger.LogError("GetPoliciesByStatus({Status}) resulted in null for agent {AgentId}.", status, agentId);
                return ErrorResponse();
            }

            return Ok(policySummaries);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Policies/Summaries/{status}/Export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportPoliciesByStatus(Status status)
    {
        var queryString = Request.QueryString.ToString();

        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var document = await PolicyInfoManager.GetPolicySummariesAsExcelDocument(agentId, status, queryString);
            if (document == null)
            {
                Logger.LogError("Unable to get {Status} policy export for {AgentId} with the parameters {QueryString}.", status, agentId, queryString);
                return ErrorResponse();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Policies/PendingPolicyRequirements", Name = nameof(GetPendingPolicyRequirements))]
    [ProducesResponseType(typeof(RequirementSummariesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPendingPolicyRequirements()
    {
        var queryString = Request.QueryString.ToString();

        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var requirementSummaries = await PolicyInfoManager.GetPendingPolicyRequirements(agentId, queryString);
            if (requirementSummaries == null || requirementSummaries.RequirementSummaries == null)
            {
                Logger.LogError("GetPendingPolicyRequirements() resulted in null for agent {AgentId}.", agentId);
                return ErrorResponse();
            }

            return Ok(requirementSummaries);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Policies/{policyNumber}", Name = nameof(GetPolicy))]
    [ProducesResponseType(typeof(PolicyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicy(string policyNumber)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }
        else if (string.IsNullOrWhiteSpace(policyNumber))
        {
            Logger.LogWarning("{AgentId} tried to access a policy, but the given PolicyNumber was null or empty.", agentId);
            return BadRequestResponse("Policy Number cannot be null or empty.");
        }

        try
        {
            var foundPolicy = await PolicyInfoManager.GetPolicyInfo(policyNumber, agentId);
            if (foundPolicy == null)
            {
                Logger.LogError("GetPolicy() resulted in null for agent {AgentId}.", agentId);
                return ErrorResponse();
            }

            return Ok(foundPolicy);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Policies/{policyNumber}/Export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportPolicy(string policyNumber)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token when attempting to download policy {PolicyNumber}.", policyNumber);
            return Unauthorized();
        }
        else if (string.IsNullOrWhiteSpace(policyNumber))
        {
            Logger.LogWarning("{AgentId} tried to access a policy for download, but the given PolicyNumber was null or empty.", agentId);
            return BadRequestResponse("Policy Number cannot be null or empty.");
        }

        try
        {
            var agentHasAccess = await PolicyInfoManager.CheckAgentAccessToPolicy(agentId, policyNumber);
            if (!agentHasAccess)
            {
                Logger.LogWarning("Agent {AgentId} does not have access to policy {PolicyNumber}.", agentId, policyNumber);
                return Forbid();
            }

            var document = await DocumentVaultManager.GetPolicyPages(policyNumber);
            if (document == null)
            {
                Logger.LogError("Unable to export policy {PolicyNumber}.", policyNumber);
                return ErrorResponse();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Policies/StatusCounts", Name = nameof(GetPolicyStatusCounts))]
    [ProducesResponseType(typeof(PolicyStatusCountsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicyStatusCounts()
    {
        var agentId = HttpContext.GetAgentId();
        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var foundPolicy = await PolicyInfoManager.GetPolicyStatusCounts(agentId);
            if (foundPolicy == null)
            {
                Logger.LogError("GetPolicyStatusCounts() resulted in null for agent {AgentId}.", agentId);
                return ErrorResponse();
            }

            return Ok(foundPolicy);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Application/{policyNumber}/Export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportApplication(string policyNumber)
    {
        var agentId = HttpContext.GetAgentId();
        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token when attempting to download application for policy number {PolicyNumber}.", policyNumber);
            return Unauthorized();
        }
        else if (string.IsNullOrWhiteSpace(policyNumber))
        {
            Logger.LogWarning("{AgentId} tried to access a export an application, but the given PolicyNumber was null or empty.", agentId);
            return BadRequestResponse("Policy Number cannot be null or empty.");
        }

        try
        {
            var agentHasAccess = await PolicyInfoManager.CheckAgentAccessToPolicy(agentId, policyNumber);
            if (!agentHasAccess)
            {
                Logger.LogWarning("Agent {AgentId} does not have access to application for policy number {PolicyNumber}.", agentId, policyNumber);
                return Forbid();
            }

            var application = await Execute360DocumentManager.GetApplication(policyNumber);
            if (application == null)
            {
                Logger.LogError("Failed to retrieve application for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(application.FileData, application.FileType, application.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}