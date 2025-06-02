namespace Assurity.AgentPortal.Contracts.Directus;

public class TemporaryMessage
{
    public int Id { get; set; }

    public string Heading { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime PublishDate { get; set; }

    public string CtaLabel { get; set; } = string.Empty;

    public string CtaLink { get; set; } = string.Empty;
}
