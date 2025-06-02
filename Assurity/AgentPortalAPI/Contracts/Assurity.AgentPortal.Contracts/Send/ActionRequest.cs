namespace Assurity.AgentPortal.Contracts.Send;

public class ActionRequest
{
    public string AgentId { get; set; }

    public List<File>? Files { get; set; }

    public string? Message { get; set; }

    public string PolicyNumber { get; set; }
}