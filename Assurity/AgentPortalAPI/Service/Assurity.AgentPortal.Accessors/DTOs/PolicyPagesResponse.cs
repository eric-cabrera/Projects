namespace Assurity.AgentPortal.Accessors.DTOs;

public class PolicyPagesResponse
{
    public string PolicyNumber { get; set; }

    public bool? IsSigned { get; set; }

    public string EncodedFile { get; set; }

    public string DocumentExtension { get; set; }
}
