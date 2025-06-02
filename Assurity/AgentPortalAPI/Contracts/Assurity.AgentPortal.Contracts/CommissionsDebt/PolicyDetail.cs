namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class PolicyDetail
{
    public string? AgentId { get; set; }

    public decimal? AdvanceRecovery { get; set; }

    public decimal? Commission { get; set; }

    public decimal? CommissionRate { get; set; }

    public string? CommissionType { get; set; }

    public string? Contract { get; set; }

    public string? EmployerId { get; set; }

    public string? EmployerName { get; set; }

    public string? LineOfBusiness { get; set; }

    public string? Mode { get; set; }

    public decimal? ModePremium { get; set; }

    public decimal? NetCommission { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PolicyNumber { get; set; }

    public DateTime? PremiumDueDate { get; set; }

    public string? PrimaryInsured { get; set; }

    public string? ProductDescription { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? WritingAgent { get; set; }

    public string? WritingAgentId { get; set; }

    public string? ChargebackReason { get; set; }
}
