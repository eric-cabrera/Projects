namespace Assurity.AgentPortal.Service.Models;

using System.ComponentModel.DataAnnotations;

public class UploadData
{
    [MaxLength(2000)]
    public string? Message { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
    [StringLength(12, MinimumLength = 10, ErrorMessage = "PolicyNumber must be between 10 and 12 characters.")]
    public string PolicyNumber { get; set; }
}