namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum PolicyStatusDetail
    {
        [Display(Name = "AGENT RECOMMENDED")]
        AgentRecommended,

        [Display(Name = "CANCELLED - INITIAL PREMIUM NOT PAID")]
        CancelledInitialPremiumNotPaid,

        [Display(Name = "CANCELLED DUE TO NO RESPONSE")]
        CancelledDueToNoResponse,

        [Display(Name = "CIRCUMSTANCES CHANGED")]
        CircumstancesChanged,

        [Display(Name = "CONVERSION")]
        Conversion,

        [Display(Name = "COVERAGE THROUGH WORK")]
        CoverageThroughWork,

        [Display(Name = "DEATH")]
        Death,

        [Display(Name = "ETI")]
        ETI,

        [Display(Name = "FREE LOOK PERIOD - BAD CHECK OR CREDIT CARD")]
        FreeLookPeriodBadCheckOrCreditCard,

        [Display(Name = "FREE LOOK PERIOD - INSURED REQUESTED")]
        FreeLookPeriodInsuredRequested,

        [Display(Name = "HOME OFFICE CANCELLED")]
        HomeOfficeCancelled,

        [Display(Name = "INSURED CANCELLED - LUMP SUM PAID")]
        InsuredCancelledLumpSumPaid,

        [Display(Name = "INSURED REQUESTED")]
        InsuredRequested,

        [Display(Name = "LAPSED")]
        Lapsed,

        [Display(Name = "MATURITY")]
        Maturity,

        [Display(Name = "NO LONGER NEED")]
        NoLongerNeed,

        None,

        [Display(Name = "NON -PAYMENT")]
        NonPayment,

        [Display(Name = "NOT SPECIFIED")]
        NotSpecified,

        [Display(Name = "OTHER COVERAGE")]
        OtherCoverage,

        [Display(Name = "REPLACEMENT - EXTERNAL")]
        ReplacementExternal,

        [Display(Name = "REPLACEMENT - INTERNAL")]
        ReplacementInternal,

        [Display(Name = "RESCIND")]
        Rescind,

        [Display(Name = "RETIRED")]
        Retired,

        [Display(Name = "RPU")]
        RPU,

        [Display(Name = "SUPPLEMENTAL CONTRACT")]
        SupplementalContract,

        [Display(Name = "UNABLE TO AFFORD")]
        UnableToAfford,

        Unknown
    }
}
