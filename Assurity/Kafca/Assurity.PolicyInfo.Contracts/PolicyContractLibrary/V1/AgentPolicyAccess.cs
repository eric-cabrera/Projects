namespace Assurity.PolicyInfo.Contracts.V1
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class AgentPolicyAccess
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CompanyCode { get; set; }

        public string AgentId { get; set; }

        public List<string> PolicyNumbers { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
