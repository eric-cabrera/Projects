namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum StatusReason
    {
        [Display(Name = "Accum APL Error")]
        AccumAPLError,

        [Display(Name = "Allocation Error")]
        AllocationError,

        Amendment,

        [Display(Name = "Anniversary Processing Error")]
        AnniversaryProcessingError,

        Approved,

        [Display(Name = "Auto Numbered")]
        AutoNumbered,

        [Display(Name = "Billing Error")]
        BillingError,

        Canceled,
        Conversion,

        [Display(Name = "Conversion Pending")]
        ConversionPending,

        [Display(Name = "Death Claim")]
        DeathClaim,

        [Display(Name = "Death Pending")]
        DeathPending,

        Declined,

        [Display(Name = "Declined COC Positive")]
        DeclinedCOCPositive,

        [Display(Name = "Declined Full Und")]
        DeclinedFullUnd,

        [Display(Name = "Declined Nonreapply")]
        DeclinedNonreapply,

        [Display(Name = "Declined ReApply")]
        DeclinedReApply,

        [Display(Name = "Deferred Policy")]
        DeferredPolicy,

        Error,

        [Display(Name = "ETI Conversion")]
        ETIConversion,

        [Display(Name = "Exchange Benefit")]
        ExchangeBenefit,

        Expire,

        [Display(Name = "Home Office Suspended")]
        HomeOfficeSuspended,

        Incomplete,

        [Display(Name = "Incomplete MIB")]
        IncompleteMIB,

        [Display(Name = "Incomplete Rewrite")]
        IncompleteRewrite,

        Ineligible,

        [Display(Name = "Ineligible Age")]
        IneligibleAge,

        [Display(Name = "Ineligible Drug Question")]
        IneligibleDrugQuestion,

        [Display(Name = "Ineligible Hospital")]
        IneligibleHospital,

        [Display(Name = "Ineligible Max CV")]
        IneligibleMaxCV,

        [Display(Name = "Ineligible Not Member")]
        IneligibleNotMember,

        Lapsed,

        [Display(Name = "Lapse Pending")]
        LapsePending,

        [Display(Name = "Loan Process Error")]
        LoanProcessError,

        Matured,

        [Display(Name = "Modeaversary Processing Error")]
        ModeaversaryProcessingError,

        [Display(Name = "Monthaversary Processing Error")]
        MonthaversaryProcessingError,

        [Display(Name = "MPR To MDV Conversion")]
        MPRtoMDVConversion,

        None,

        [Display(Name = "Not Approved")]
        NotApproved,

        [Display(Name = "Not Issued")]
        NotIssued,

        [Display(Name = "Not Taken")]
        NotTaken,

        [Display(Name = "Not Taken Owner")]
        NotTakenOwner,

        [Display(Name = "Not Taken Payor")]
        NotTakenPayor,

        [Display(Name = "Not Taken Rating")]
        NotTakenRating,

        [Display(Name = "Policy Change")]
        PolicyChange,

        Postponed,

        [Display(Name = "Preliminary Term")]
        PreliminaryTerm,

        QuestionableCostBasis,

        [Display(Name = "Ready To Issue")]
        ReadyToIssue,

        Reinstated,

        [Display(Name = "Reissued")]
        Reissued,

        [Display(Name = "Renewal Pending")]
        RenewalPending,

        Replaced,

        [Display(Name = "Replacement Pending")]
        ReplacementPending,

        Reserved,
        Restored,

        [Display(Name = "Returned Check")]
        ReturnedCheck,

        [Display(Name = "Returned Eft")]
        ReturnedEft,

        [Display(Name = "RPI Conversion")]
        RPIConversion,

        Submitted,
        Surrendered,

        [Display(Name = "Surrender Pending")]
        SurrenderPending,

        [Display(Name = "Termination Pending")]
        TerminationPending,

        [Display(Name = "Two Year Lookback")]
        TwoYearLookback,

        [Display(Name = "Unapplied Cash")]
        UnappliedCash,

        Unknown,

        [Display(Name = "Waiver Disability")]
        WaiverDisability,

        [Display(Name = "Waiver Pending")]
        WaiverPending,

        [Display(Name = "Withdrawn")]
        Withdrawn,

        [Display(Name = "Withdrawn Owner")]
        WithdrawnOwner,

        [Display(Name = "Withdrawn Payor")]
        WithdrawnPayor
    }
}