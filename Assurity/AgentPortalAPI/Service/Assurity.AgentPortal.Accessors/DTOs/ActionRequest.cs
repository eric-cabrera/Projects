namespace Assurity.AgentPortal.Accessors.DTOs;

public class ActionRequest
{
    public string AgentId { get; set; }

    public List<File>? Files { get; set; }

    public string ObjectIdForNewBusinessTransaction { get; set; }

    public string PolicyNumber { get; set; }
}