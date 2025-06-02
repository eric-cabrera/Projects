namespace Assurity.AgentPortal.Contracts.Integration;

public class IPipelineSsoInfo
{
    public string AgentNumber { get; set; }

    public string? Agentname { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address1 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? ShowAgency { get; set; }

    public string? AIAgency { get; set; }

    public string AgentPilot
    {
        get { return "N"; }
    }

    public string? Initial_DisconnectedLogonID { get; set; }

    public string? Initial_DisconnectedPassword { get; set; }
}
