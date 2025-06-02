namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum LineOfBusiness
    {
        [Display(Name = "A")]
        Annuity,

        [Display(Name = "G")]
        Group,

        [Display(Name = "H")]
        Health,

        [Display(Name = "I")]
        ImmediateAnnuity,

        [Display(Name = "L")]
        TraditionalLife,

        [Display(Name = "S")]
        TrueGroupCensus,

        [Display(Name = "U")]
        UniversalLife,

        Unknown
    }
}