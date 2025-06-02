namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum PayorType
    {
        Primary,

        Additional,

        [Display(Name = "Secondary Addressee")]
        SecondaryAddressee
    }
}