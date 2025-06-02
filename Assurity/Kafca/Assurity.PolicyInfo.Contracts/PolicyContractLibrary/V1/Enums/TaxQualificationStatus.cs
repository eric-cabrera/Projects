namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum TaxQualificationStatus
    {
        [Display(Name = "Not Qualified")]
        NotQualified,

        [Display(Name = "HR10")]
        HR10,

        [Display(Name = "TSA")]
        TSA,

        [Display(Name = "IRA")]
        IRA,

        [Display(Name = "SEP")]
        SEP,

        [Display(Name = "Deferred Compensation")]
        DeferredCompensation,

        [Display(Name = "Non Qualified Deferred Compensation")]
        NonQualifiedDeferredCompensation,

        [Display(Name = "401k Plan")]
        RetirementPlan,

        [Display(Name = "Sec. 457 Plan")]
        Sec457Plan,

        [Display(Name = "Defined Benefit")]
        DefinedBenefit,

        [Display(Name = "Defined Contribution")]
        DefinedContribution,

        [Display(Name = "Simple IRA")]
        SimpleIRA,

        [Display(Name = "Health Cafeteria - Section 125")]
        HealthCafeteria,

        [Display(Name = "Roth IRA Contribution")]
        RothIRAContribution,

        [Display(Name = "Roth IRA Conversion")]
        RothIRAConversion,

        [Display(Name = "OLAB")]
        OLAB,

        [Display(Name = "Non-OLAB")]
        NonOLAB,

        [Display(Name = "Qualified")]
        Qualified,

        [Display(Name = "NonQualified")]
        NonQualified,

        [Display(Name = "Health Qualified")]
        HealthQualified,

        [Display(Name = "Health Grand-fathered")]
        HealthGrandfathered,

        [Display(Name = "Roth TSA")]
        RothTSA,

        [Display(Name = "Roth 401(k)")]
        Roth401k,

        [Display(Name = "Roth Govt 457")]
        RothGovt457,

        Unknown
    }
}