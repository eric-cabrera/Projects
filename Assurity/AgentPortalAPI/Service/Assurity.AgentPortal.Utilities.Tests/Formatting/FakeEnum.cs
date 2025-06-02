namespace Assurity.AgentPortal.Utilities.Tests.Formatting;

using System.ComponentModel.DataAnnotations;

public enum FakeEnum
{
    [Display(Name = "First Fake Enum")]
    First,
    [Display(Name = "Second Fake Enum")]
    Second,
    [Display(Name = "Third Fake Enum")]
    Third
}
