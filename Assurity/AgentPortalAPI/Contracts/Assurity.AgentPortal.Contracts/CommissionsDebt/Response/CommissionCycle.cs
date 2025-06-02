namespace Assurity.AgentPortal.Contracts.CommissionsDebt.Response;

public class CommissionCycle
{
    public DateTimeOffset? CycleStartDate { get; set; }

    public DateTimeOffset? CycleEndDate { get; set; }

    public bool Estimated { get; set; } = false;

    public decimal MyCommissions { get; set; }

    public decimal FirstYear { get; set; }

    public decimal Renewal { get; set; }
}
