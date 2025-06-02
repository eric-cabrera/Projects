namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RequirementFulfillingParty
    {
        Agent,

        [Display(Name = "Home Office")]
        HomeOffice
    }
}
