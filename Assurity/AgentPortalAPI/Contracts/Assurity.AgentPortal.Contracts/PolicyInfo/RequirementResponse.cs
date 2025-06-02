namespace Assurity.AgentPortal.Contracts.PolicyInfo;

using Assurity.PolicyInfo.Contracts.V1;

public class RequirementResponse
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Status { get; set; }

    public DateTime? AddedDate { get; set; }

    public DateTime? ObtainedDate { get; set; }

    public Participant AppliesTo { get; set; }

    public string LifeProComment { get; set; }

    public string GlobalComment { get; set; }

    public string PhoneNumberComment { get; set; }

    public string? FulfillingParty { get; set; }

    public string? ActionType { get; set; }

    public bool Display { get; set; }
}
