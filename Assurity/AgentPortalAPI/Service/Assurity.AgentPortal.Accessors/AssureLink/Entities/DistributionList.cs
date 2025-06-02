namespace Assurity.AgentPortal.Accessors.AssureLink.Entities;

public sealed class DistributionList
{
    public int Id { get; set; }

    public string AgentId { get; set; } = null!;

    public string Email { get; set; } = null!;
}