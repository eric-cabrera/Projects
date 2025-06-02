namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Employer
    {
        public Business Business { get; set; }

        public string Number { get; set; }

        [BsonRepresentation(BsonType.String)]
        public EmployerStatus? Status { get; set; }
    }
}
