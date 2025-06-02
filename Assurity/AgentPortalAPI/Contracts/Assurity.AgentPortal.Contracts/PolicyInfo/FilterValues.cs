namespace Assurity.AgentPortal.Contracts.PolicyInfo;

using Assurity.PolicyInfo.Contracts.V1.Enums;

public class FilterValues
{
    public List<string> AgentIdFilterValues { get; set; }

    public List<string> AgentLastNameFilterValues { get; set; }

    public List<string> PrimaryInsuredFilterValues { get; set; }

    public List<string> EmployerFilterValues { get; set; }

    public List<StatusReason> ReasonFilterValues { get; set; }

    public List<string> PolicyNumberFilterValues { get; set; }

    public List<string> ViewAsFilterValues { get; set; }
}
