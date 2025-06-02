namespace Assurity.Kafka.Accessors.Tests.TestData
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.PolicyInfo.Contracts.V1;
    using MongoDB.Bson;

    [ExcludeFromCodeCoverage]
    public static class MongoDbTestData
    {
        public static List<Policy> PoliciesForGetPoliciesAsyncTests => new ()
        {
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "01",
                PolicyNumber = "5150198401"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "01",
                PolicyNumber = "5150198402"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "01",
                PolicyNumber = "5150198403"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "01",
                PolicyNumber = "5150198404"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "02",
                PolicyNumber = "5150198450"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "02",
                PolicyNumber = "5150198451"
            },
            new Policy
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CompanyCode = "02",
                PolicyNumber = "5150198452"
            },
        };
    }
}