namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BillingForm
    {
        [Display(Name = "Automatic Bank Withdrawal")]
        AutomaticBankWithdrawal,

        [Display(Name = "Credit Card")]
        CreditCard,

        [Display(Name = "Direct")]
        Direct,

        [Display(Name = "Government Allotment")]
        GovernmentAllotment,

        [Display(Name = "List Bill")]
        ListBill,

        [Display(Name = "None")]
        None,

        Unknown
    }
}
