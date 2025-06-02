namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BillingReason
    {
        [Display(Name = "Policy Change")]
        PolicyChange,

        [Display(Name = "Paid Up")]
        PaidUp,

        [Display(Name = "Vanish Base")]
        VanishBase,

        [Display(Name = "Vanish")]
        Vanish,

        [Display(Name = "Waiver Disability")]
        WaiverDisability,

        [Display(Name = "Adjustment")]
        Adjustment,

        [Display(Name = "Billing Error")]
        BillingError,

        [Display(Name = "Death Pending")]
        DeathPending,

        [Display(Name = "Early Paid Up")]
        EarlyPaidUp,

        [Display(Name = "Extended Term")]
        ExtendedTerm,

        [Display(Name = "Non Forfeiture")]
        NonForfeiture,

        [Display(Name = "Reduced Paid Up")]
        ReducedPaidUp,

        [Display(Name = "Stopped Premium")]
        StoppedPremium,

        [Display(Name = "Waiver Pending")]
        WaiverPending,

        [Display(Name = "None")]
        None,

        Unknown
    }
}