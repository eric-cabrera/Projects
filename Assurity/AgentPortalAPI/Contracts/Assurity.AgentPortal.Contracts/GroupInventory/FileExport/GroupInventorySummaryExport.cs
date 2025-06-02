namespace Assurity.AgentPortal.Contracts.GroupInventory.FileExport;

using System;
using System.ComponentModel;

public class GroupInventorySummaryExport
{
    [DisplayName("Group Name")]
    public string? GroupName { get; set; }

    [DisplayName("Group Number")]
    public string? GroupNumber { get; set; }

    [DisplayName("Status")]
    public string? GroupStatus { get; set; }

    [DisplayName("Policy Count")]
    public int? PolicyCount { get; set; }

    [DisplayName("Effective Date")]
    public DateTime? GroupEffectiveDate { get; set; }
}