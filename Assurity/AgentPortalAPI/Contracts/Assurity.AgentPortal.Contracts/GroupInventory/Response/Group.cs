namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class Group
{
    public string? Name { get; set; }

    public string? Number { get; set; }

    public string? Status { get; set; }

    public string? EffectiveDate { get; set; }

    public int? ActivePolicyCount { get; set; }
}