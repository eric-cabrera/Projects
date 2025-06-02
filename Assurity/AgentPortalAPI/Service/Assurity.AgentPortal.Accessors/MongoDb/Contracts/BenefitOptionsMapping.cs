namespace Assurity.AgentPortal.Accessors.MongoDb.Contracts;

using Assurity.PolicyInfo.Contracts.V1.Enums;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;

public class BenefitOptionsMapping
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public BenefitOptionName Category { get; set; }

    [BsonRepresentation(BsonType.String)]
    public BenefitOptionValue Option { get; set; }

    public bool HideBenefitOption { get; set; }
}
