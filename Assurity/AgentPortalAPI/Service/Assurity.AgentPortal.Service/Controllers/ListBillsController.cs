namespace Assurity.AgentPortal.Service.Controllers;

using System.Net.Mime;
using Assurity.AgentPortal.Contracts.ListBill;
using Assurity.AgentPortal.Managers.ListBill;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ListBill)]
public class ListBillsController(
    ILogger<ListBillsController> logger,
    IConfigurationManager configurationManager,
    IListBillsManager listBillsManager) : BaseController(configurationManager, logger)
{
    private IListBillsManager ListBillsManager { get; } = listBillsManager;

    /// <summary>
    /// Get all groups for which a given agent has access.
    /// </summary>
    /// <remarks>
    /// # Overview
    /// - Groups are returned in ascending order by Id (Group Number).
    /// - This endpoint supports pagination. If the page or pageSize is not provided, the
    /// default values are 1 and 1000, respectively. Values must be greater than 0, or
    /// a validation message will be returned.
    /// - Only groups with list bills generated within the last 6 months will be returned.
    ///
    /// # Usage
    /// This is the first endpoint to use when wanting to retrieve a list bill.
    /// 1. Provide an Agent Id.
    /// 2. Provide optional query parameters to narrow your search results.
    /// 3. Provide optional paging parameters.
    /// 4. Assuming the agent ID you provide returns results, select a returned `group.Id` and use it as input for the GET /listbills endpoint.
    ///
    /// # Notes
    ///  If you are reviewing this via the Dev server instance, you will likely have a hard time getting any list bill data to come back.
    /// This is due to the fact that the data in Dev sparse as well as old. Though data does exist, it will be filtered out because the list bills
    /// were generated greater than 6 months ago. Test is little better. Prod can be targeted via HTTP Client (Postman, Insomnia, ARC, etc) with little issue
    /// and without the need for any authentication.
    ///
    /// ## Useful Agent IDs
    ///     - 672M
    ///     - 4U6C
    ///     - 55ZZ.
    /// </remarks>
    /// <param name="page" example="1"></param>
    /// <param name="pageSize" example="1000"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("groups", Name = nameof(GetGroups))]
    [ProducesResponseType(typeof(Contracts.ListBill.GroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroups(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var pageNumber = page ?? null;
            var pageSizeNumber = pageSize ?? null;

            var response = await ListBillsManager.
                GetGroups(agentId, pageNumber, pageSizeNumber, cancellationToken);

            if (response == null || response.Groups == null)
            {
                Logger.LogError("Failed to retrieve groups for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    /// <summary>
    /// Overview
    ///  List bills are returned in descending order by date.
    ///  All groups returned by the GET /groups should produce an HTTP 200 response from this endpoint and include valid list bills.
    ///  Usage: Provide a group ID from the GET /groups endpoint.
    ///  Retrieve a listBill.Id from the response and use it as input from the GET /listBill endpoint.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="cancellationToken"></param>
    /// example groupId = "L31146"
    /// <returns></returns>
    [HttpGet("groups/{groupId}/listBills", Name = nameof(GetListBills))]
    [ProducesResponseType(typeof(ListBillsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetListBills(
        string groupId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            Logger.LogWarning("GroupId cannot be null or empty.");
            return BadRequestResponse("GroupId cannot be null or empty.");
        }

        try
        {
            var response = await ListBillsManager.GetListBills(groupId, cancellationToken);
            if (response == null || response.ListBills == null)
            {
                Logger.LogError("Failed to retrieve list bills for group id {GroupId}", groupId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("listBills/{listBillId}", Name = nameof(GetListBillData))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetListBillData(
        string listBillId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(listBillId))
        {
            Logger.LogWarning("ListBill Id required.");
            return BadRequestResponse("List Bill Id cannot be null or empty.");
        }

        try
        {
            var response = await ListBillsManager.GetListBillData(listBillId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve list bill data for list bill id {ListBillId}", listBillId);
                return ErrorResponse();
            }

            var mimeType = MediaTypeNames.Application.Pdf;

            return File(response.FileData, mimeType, response.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
