namespace Assurity.AgentPortal.Contracts;

public class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public List<string> States { get; set; } = new();

    public List<string> ContactTypes { get; set; } = new List<string>();
}