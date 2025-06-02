namespace Assurity.AgentPortal.Service.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Assurity.AgentPortal.Contracts.GroupInventory.Request;
    using Assurity.AgentPortal.Contracts.GroupInventory.Response;
    using Assurity.AgentPortal.Managers.GroupInventory;
    using Assurity.AgentPortal.Service.Authorization;
    using Assurity.AgentPortal.Service.IOCConfig.Extensions;
    using Assurity.AgentPortal.Utilities.Configs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("/API/[controller]")]
    [ApiController]
    [Authorize(AuthorizationPolicyConstants.WorksiteParticipation)]
    public class GroupInventoryController(
        ILogger<GroupInventoryController> logger,
        IConfigurationManager configurationManager,
        IGroupInventoryManager groupInventoryManager) : BaseController(configurationManager, logger)
    {
        private IGroupInventoryManager GroupInventoryManager { get; } = groupInventoryManager;

        [HttpGet("GroupSummary", Name = nameof(GetGroupSummary))]
        [ProducesResponseType(typeof(GroupSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupSummary(
            [FromQuery] GroupSummaryQueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            var loggedInAgentNumber = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            try
            {
                var response = await GroupInventoryManager.GetGroupSummary(
                    loggedInAgentNumber,
                    queryParameters,
                    cancellationToken);

                if (response == null)
                {
                    Logger.LogError("Group summary not found for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                    return ErrorResponse();
                }

                return Ok(response);
            }
            catch (Exception exception)
            {
                return HandleException(exception, Logger);
            }
        }

        [HttpGet("GroupDetails", Name = nameof(GetGroupDetail))]
        [ProducesResponseType(typeof(GroupDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupDetail(
            [FromQuery] GroupDetailsQueryParameters queryParameters,
            CancellationToken cancellationToken = default)
        {
            var loggedInAgentNumber = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(queryParameters.GroupNumber))
            {
                Logger.LogWarning("Group number is required but was not provided for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                return BadRequestResponse("Group number is required.");
            }

            // If the start date is passed in, then there should also be an end date, and vice versa
            if ((queryParameters.IssueStartDate.HasValue && !queryParameters.IssueEndDate.HasValue) ||
                (queryParameters.IssueEndDate.HasValue && !queryParameters.IssueStartDate.HasValue))
            {
                Logger.LogWarning("Both an issue start and end date is required if one is provided but one was not provided for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                return BadRequestResponse("If using issue start or end date to filter, then both issue start and end date are required.");
            }

            try
            {
                var response = await GroupInventoryManager.GetGroupDetail(
                    loggedInAgentNumber,
                    queryParameters,
                    cancellationToken);
                if (response == null)
                {
                    Logger.LogWarning("No group details found for Agent ID: {AgentNumber} and Group Number: {GroupNumber}.", loggedInAgentNumber, queryParameters.GroupNumber);
                    return ErrorResponse();
                }

                return Ok(response);
            }
            catch (Exception exception)
            {
                return HandleException(exception, Logger);
            }
        }

        [HttpGet("GroupSummary/Export", Name = nameof(ExportGroupSummary))]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportGroupSummary(
          [FromQuery] GroupSummaryQueryParameters queryParameters,
          CancellationToken cancellationToken = default)
        {
            var loggedInAgentNumber = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            try
            {
                var document = await GroupInventoryManager.GetGroupSummaryExport(loggedInAgentNumber, queryParameters, cancellationToken);

                if (document == null || document.FileData == null)
                {
                    Logger.LogError("Failed to retrieve group summary export document for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                    return ErrorResponse();
                }

                return File(document.FileData, document.FileType, document.FileName);
            }
            catch (Exception exception)
            {
                return HandleException(exception, Logger);
            }
        }

        [HttpGet("GroupDetails/Export", Name = nameof(ExportGroupDetails))]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportGroupDetails(
           [FromQuery] GroupDetailsQueryParameters queryParameters,
           CancellationToken cancellationToken = default)
        {
            var loggedInAgentNumber = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(queryParameters.GroupNumber))
            {
                Logger.LogWarning("Group number is required but was not provided for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                return BadRequestResponse("Group number is required.");
            }

            // If the start date is passed in, then there should also be an end date, and vice versa
            if ((queryParameters.IssueStartDate.HasValue && !queryParameters.IssueEndDate.HasValue) ||
                (queryParameters.IssueEndDate.HasValue && !queryParameters.IssueStartDate.HasValue))
            {
                Logger.LogWarning("Both an issue start and end date is required if one is provided but one was not provided for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                return BadRequestResponse("If using issue start or end date to filter, then both issue start and end date are required.");
            }

            try
            {
                var document = await GroupInventoryManager.GetGroupDetailsExport(
                    loggedInAgentNumber,
                    queryParameters,
                    cancellationToken);

                if (document == null || document.FileData == null)
                {
                    Logger.LogError("Failed to retrieve group details export document for Agent ID: {AgentNumber}.", loggedInAgentNumber);
                    return ErrorResponse();
                }

                return File(document.FileData, document.FileType, document.FileName);
            }
            catch (Exception exception)
            {
                return HandleException(exception, Logger);
            }
        }
    }
}
