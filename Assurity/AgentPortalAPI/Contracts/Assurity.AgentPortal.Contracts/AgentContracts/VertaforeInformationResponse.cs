namespace Assurity.AgentPortal.Contracts.AgentContracts;

public class VertaforeInformationResponse
{
    public bool HasAccess { get; set; } = false;

    public string? RedirectUrl { get; set; } = string.Empty;
}
