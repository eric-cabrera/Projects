namespace Assurity.PolicyInfo.Contracts.V1
{
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Address
    {
        public int AddressId { get; set; }

        public string BoxNumber { get; set; }

        public string City { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Country? Country { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        [BsonRepresentation(BsonType.String)]
        public State? StateAbbreviation { get; set; }

        public string ZipCode { get; set; }

        public string ZipExtension { get; set; }
    }
}