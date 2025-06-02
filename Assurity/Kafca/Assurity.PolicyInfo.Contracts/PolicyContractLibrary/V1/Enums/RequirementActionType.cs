namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RequirementActionType
    {
        [Display(Name = "Upload File")]
        UploadFile,

        [Display(Name = "Send Message")]
        SendMessage,

        [Display(Name = "Upload File Or Send Message")]
        UploadFileOrSendMessage
    }
}
