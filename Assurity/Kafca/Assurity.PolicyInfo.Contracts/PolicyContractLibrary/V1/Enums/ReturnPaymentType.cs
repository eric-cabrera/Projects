namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ReturnPaymentType
    {
        [Display(Name = "Initial Payment Card Declined")]
        InitialPaymentCardDeclined,

        [Display(Name = "Initial Payment Check Draft Declined")]
        InitialPaymentCheckDraftDeclined,

        [Display(Name = "Card Declined")]
        CardDeclined,

        [Display(Name = "Check Draft Declined")]
        CheckDraftDeclined,

        None
    }
}