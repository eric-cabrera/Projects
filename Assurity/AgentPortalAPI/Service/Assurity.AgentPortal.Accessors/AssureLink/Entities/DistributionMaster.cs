namespace Assurity.AgentPortal.Accessors.AssureLink.Entities;

public sealed class DistributionMaster
{
    public int Id { get; set; }

    public string AgentId { get; set; } = null!;

    public bool DisableAll { get; set; }

    public bool SelfAdd { get; set; }

    public bool SelfMet { get; set; }

    public bool SelfOutstanding { get; set; }

    public bool HierarchyAdd { get; set; }

    public bool HierarchyMet { get; set; }

    public bool HierarchyOutstanding { get; set; }
}
