namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ApplicationStatus
    {
        [Display(Name = "Application Created")]
        ApplicationCreated,

        [Display(Name = "Interview Started")]
        InterviewStarted,

        [Display(Name = "Interview Completed")]
        InterviewCompleted,

        [Display(Name = "Awaiting Signature")]
        AwaitingSignature,

        [Display(Name = "Application Submitted")]
        ApplicationSubmitted
    }
}