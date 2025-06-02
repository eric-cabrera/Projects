namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum DividendOption
    {
        [Display(Name = "No Dividend")]
        NoDividend,

        [Display(Name = "Cash Dividend")]
        CashDividend,

        [Display(Name = "Reduce Premium")]
        ReducePremium,

        [Display(Name = "Accumulate at Interest/Add to Cash Value")]
        AccumulateAtInterestOrAddToCashValue,

        [Display(Name = "Paid-up Additions")]
        PaidupAdditions,

        [Display(Name = "One Year Term")]
        OneYearTerm,

        [Display(Name = "Reduce Loan")]
        ReduceLoan,

        [Display(Name = "One Year Term Conditional")]
        OneYearTermConditional,

        [Display(Name = "Target is % of Base")]
        TargetIsPercentageOfBase,

        [Display(Name = "Special Code")]
        SpecialCode,

        [Display(Name = "None")]
        None,

        Unknown
    }
}