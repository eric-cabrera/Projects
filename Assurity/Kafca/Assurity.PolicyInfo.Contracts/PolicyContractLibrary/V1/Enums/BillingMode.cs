namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BillingMode
    {
        Annually,
        Biweekly,
        Calendar,

        [Display(Name = "Fifty-two Pay")]
        FiftyTwoPay,

        Monthly,
        Ninthly,
        None,
        Quarterly,

        [Display(Name = "Semi-Annually")]
        SemiAnnually,
        Tenthly,
        Thirteenthly,

        [Display(Name = "Twenty-six Pay")]
        TwentySixPay,
        Unknown,
        Weekly
    }
}
