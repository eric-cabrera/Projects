namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class GroupSummary
{
    public string? Number { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public int PolicyCount { get; set; }

    public string? GroupEffectiveDate { get; set; }
}
