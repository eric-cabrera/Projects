namespace Assurity.AgentPortal.Accessors.MongoDb.Contracts;

using global::MongoDB.Bson;

public class PendingSubaccount
{
    public ObjectId Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string ParentAgentId { get; set; } = string.Empty;

    public string ParentUsername { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; }

    public Guid LinkGuid { get; set; }

    public int ActivationAttempts { get; set; }

    public DateTime EmailSentAt { get; set; }
}
