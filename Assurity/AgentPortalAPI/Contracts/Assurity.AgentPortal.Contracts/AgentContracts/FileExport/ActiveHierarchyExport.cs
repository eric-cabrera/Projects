namespace Assurity.AgentPortal.Contracts.AgentContracts.FileExport;

using System;
using System.ComponentModel;

public class ActiveHierarchyExport
{
    [DisplayName("Agent Name")]
    public string? AgentName { get; set; }

    [DisplayName("Agent ID")]
    public string? ActiveAgentId { get; set; }

    [DisplayName("Market Code")]
    public string? MarketCode { get; set; }

    [DisplayName("Agent Level")]
    public string? Level { get; set; }

    [DisplayName("Start Date")]
    public DateTime? StartDate { get; set; }

    [DisplayName("Advance Frequency")]
    public string? AdvanceFrequency { get; set; }

    [DisplayName("Advance Rate")]
    public string? AdvanceRate { get; set; }

    [DisplayName("Address Line 1")]
    public string? AddressLine1 { get; set; }

    [DisplayName("Address Line 2")]
    public string? AddressLine2 { get; set; }

    [DisplayName("City")]
    public string? City { get; set; }

    [DisplayName("State")]
    public string? State { get; set; }

    [DisplayName("Zip Code")]
    public string? ZipCode { get; set; }

    [DisplayName("Phone")]
    public string? Phone { get; set; }

    [DisplayName("Fax")]
    public string? Fax { get; set; }

    [DisplayName("Email")]
    public string? Email { get; set; }

    [DisplayName("Direct Deposit")]
    public string? DirectDeposit { get; set; }

    [DisplayName("Contract Status")]
    public string? ContractStatus { get; set; }

    [DisplayName("Anti Money Laundering")]
    public string? AML { get; set; }
}
