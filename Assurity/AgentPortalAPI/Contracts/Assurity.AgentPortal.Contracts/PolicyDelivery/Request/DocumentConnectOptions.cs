namespace Assurity.AgentPortal.Contracts.PolicyDelivery.Request;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class DocumentConnectOptions
{
    [Required]
    public string? ViewAsAgentNumber { get; set; }

    [Required]
    public string? ViewAsMarketCode { get; set; }

    [Required]
    public bool OptOutForEDelivery { get; set; }

    [Required]
    public bool AgentLinkSelected { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    public string? AccessCode { get; set; }

    [Required]
    [DefaultValue(true)]
    public bool IncludeDownline { get; set; }

    [Required]
    public DateTime? LastSavedDate { get; set; }
}
