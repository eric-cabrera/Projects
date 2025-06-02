namespace Assurity.AgentPortal.Contracts.PolicyInfo;

public class BenefitOptionResponse
{
    public string BenefitOptionName { get; set; }

    public string BenefitOptionValue { get; set; }

    public string RelationshipToPrimaryInsured { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? StopDate { get; set; }
}
