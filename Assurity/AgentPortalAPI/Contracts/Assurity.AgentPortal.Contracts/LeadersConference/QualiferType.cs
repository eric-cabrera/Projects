namespace Assurity.AgentPortal.Contracts.LeadersConference;

using System.ComponentModel.DataAnnotations;

public enum QualiferType
{
    [Display(Name= "WS Personal Detail")]
    WorksitePersonal,
    [Display(Name = "WS Hierarchy Detail")]
    WorksiteHierarchy,
    [Display(Name = "IPD Personal Detail")]
    IndividualPersonal,
    [Display(Name = "IPD Hierarchy Detail")]
    IndividualHierarchy
}
