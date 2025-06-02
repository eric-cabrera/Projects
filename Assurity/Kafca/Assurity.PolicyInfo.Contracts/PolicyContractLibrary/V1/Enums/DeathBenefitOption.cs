namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum DeathBenefitOption
    {
        [Display(Name = "Option 1 - Face Amount")]
        FaceAmountOption,

        [Display(Name = "Option 2 - Face Amount + Cash Value")]
        FaceAmountPlusCashValueOption,

        [Display(Name = "None")]
        None,

        Unknown
    }
}