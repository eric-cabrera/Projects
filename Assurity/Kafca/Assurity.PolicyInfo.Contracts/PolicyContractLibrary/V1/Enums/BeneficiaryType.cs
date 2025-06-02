namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BeneficiaryType
    {
        Primary,

        Contingent,

        [Display(Name = "Per Stirpes Primary")]
        PerStirpesPrimary,

        [Display(Name = "Per Stirpes Contingent")]
        PerStirpesContingent,

        [Display(Name = "Per Stirpes Tertiary")]
        PerStirpesTertiary,

        [Display(Name = "Joint Insured Primary")]
        JointInsuredPrimary,

        [Display(Name = "Joint Insured Contingent")]
        JointInsuredContingent,

        Tertiary,

        Quaternary
    }
}