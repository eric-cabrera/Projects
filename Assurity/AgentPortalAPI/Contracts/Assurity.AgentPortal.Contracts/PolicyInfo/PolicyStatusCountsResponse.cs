namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class PolicyStatusCountsResponse
{
    public PendingStatusCounts PendingPolicies { get; set; }

    public ActiveStatusCounts ActivePolicies { get; set; }

    public TerminatedStatusCounts TerminatedPolicies { get; set; }
}
