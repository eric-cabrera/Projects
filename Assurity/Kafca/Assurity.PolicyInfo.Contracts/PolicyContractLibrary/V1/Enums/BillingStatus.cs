namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BillingStatus
    {
        Active,

        [Display(Name = "Hold Billing")]
        HoldBilling,

        Suspended,

        [Display(Name = "None")]
        None,

        Unknown
    }
}