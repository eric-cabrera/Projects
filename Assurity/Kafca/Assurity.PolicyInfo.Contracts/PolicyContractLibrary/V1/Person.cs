namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Person
    {
        public DateTime? DateOfBirth { get; set; }

        public string EmailAddress { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Gender? Gender { get; set; }

        public Name Name { get; set; }
    }
}