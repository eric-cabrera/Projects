namespace Assurity.AgentPortal.Accessors.MongoDb.Contracts;

using Assurity.AgentPortal.Accessors.Impersonation;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;

public class ImpersonationLog
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string HomeOfficeUserId { get; set; }

    public string HomeOfficeUserEmail { get; set; }

    public UserSearch ImpersonatedUser { get; set; }
}
