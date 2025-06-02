namespace Assurity.AgentPortal.Contracts.GroupInventory.Request;

using System.ComponentModel.DataAnnotations;
using Assurity.AgentPortal.Contracts.Enums;

public class GroupDetailsQueryParameters : PageQueryParameters
{
    [Required]
    public string? GroupNumber { get; set; }

    public string? PolicyNumber { get; set; }

    /// <summary>
    /// Multiple product descriptions accepted in a semicolon separated list.
    /// Must be an exact match.
    /// </summary>
    public string? ProductDescription { get; set; }

    /// <summary>
    /// If providing a person name, must be in "Last,First" format.
    /// Both person names and business names bust match exactly.
    /// </summary>
    public string? PolicyOwnerName { get; set; }

    /// <summary>
    /// Will always be treated as a UTC time.
    /// </summary>
    public DateTime? IssueStartDate { get; set; }

    /// <summary>
    /// Will always be treated as a UTC time.
    /// </summary>
    public DateTime? IssueEndDate { get; set; }

    /// <summary>
    /// Multiple policy statuses are accepted in a semicolon separated list.
    /// Must be an exact match (Active, Pending, Terminated).
    /// </summary>
    public string? PolicyStatus { get; set; }

    public DetailsOrderBy? OrderBy { get; set; }

    public SortDirection SortDirection { get; set; }
}
