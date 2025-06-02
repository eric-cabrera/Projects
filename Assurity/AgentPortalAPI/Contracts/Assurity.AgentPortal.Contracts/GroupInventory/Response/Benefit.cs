namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class Benefit
{
    public string? CoverageType { get; set; }

    public string? Description { get; set; }

    public string? Amount { get; set; }

    public List<string>? CoverageOptions { get; set; }
}
