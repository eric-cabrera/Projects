namespace Assurity.PolicyInfo.Contracts.V1
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class PolicyHierarchy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public DateTime ApplicationDate { get; set; }

        public List<AgentHierarchy> HierarchyBranches { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
