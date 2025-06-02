namespace Assurity.AgentPortal.Contracts.Claims;

public class ClaimDetail
{
    public DateTime BenefitDate { get; set; }

    public string BenefitDescription { get; set; } = string.Empty;

    public string DeliveryMethod { get; set; } = string.Empty;

    public decimal PaymentAmount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public string? Status { get; set; }
}
