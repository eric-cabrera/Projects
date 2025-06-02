namespace Assurity.AgentPortal.Contracts.Claims;

using System;
using System.Collections.Generic;

public class Claim
{
    public string? ClaimNumber { get; set; }

    public Name? Claimant { get; set; }

    public DateTime DateReported { get; set; }

    public decimal PaymentAmount { get; set; }

    public string? PolicyNumber { get; set; }

    public string? PolicyType { get; set; }

    public string? Status { get; set; }

    public string? StatusReason { get; set; }

    public List<ClaimDetail>? Details { get; set; }
}
