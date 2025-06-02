using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PolicyMigration
{
    public class Policy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CompanyCode { get; set; }

        public string PolicyNumber { get; set; }

        public string ProductCode { get; set; }

        public string ProductCategory { get; set; }

        public string ProductDescription { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]  
        [BsonRepresentation(BsonType.String)]         
        public PolicyStatus PolicyStatus { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]  
        [BsonRepresentation(BsonType.String)]         
        public PolicyStatusReason PolicyStatusReason { get; set; }

        public decimal FaceAmount { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public State IssueState { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public State ResidentState { get; set; }

        public string BillingMode { get; set; } // Consider an Enum

        public string BillingForm { get; set; } // Consider an Enum

        //This is a YYYYMMDD integer in LifePro
        public DateTime IssueDate { get; set; }

        //This is a YYYYMMDD integer in LifePro
        public DateTime PaidToDate { get; set; }

        public decimal ModePremium { get; set; }

        public decimal AnnualPremium { get; set; }

        public short BillingDay { get; set; }

        // Unsure how to derive. See PolicyInfo service.
        public decimal CashValue { get; set; }

        public List<Owner> Owners { get; set; }

        public List<Insured> Insureds { get; set; }

        public List<Payor> Payors { get; set; }

        public List<Agent> Agents { get; set; }

        // Still researching what and how to derive. See PolicyInfo service. PMUIN+ExtendedKeys? 
        public List<Benefit> Benefits { get; set; }

    }

    public class Owner
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public OwnerType OwnerType { get; set; }

        public Participant Participant { get; set; }
    }

    public class Insured
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public InsuredRelationshipToPrimary RelationshipToPrimaryInsured { get; set; }

        public Participant Participant { get; set; }
    }

    public class Payor
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public PayorType PayorType { get; set; }

        public Participant Participant { get; set; }
    }

    public class Agent
    {

        public bool IsServicingAgent { get; set; }

        public bool IsWritingAgent { get; set; }

        public string AgentId { get; set; }

        public string MarketCode { get; set; }

        public string Level { get; set; }
    }

    public class Participant
    {
        public bool IsBusiness { get; set; }

        public Business Business { get; set; }

        public Person Person { get; set; }

        public string PhoneNumber { get; set; }

        public Address Address { get; set; }
    }

    public class Business
    {
        public Name Name { get; set; }

        public string EmailAddress { get; set; }
    }

    public class Person
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Name Name { get; set; }

        public string EmailAddress { get; set; }

        // Consider an occupation object that includes occupation class. Where do we derive occupation class from?
        // Consider a Tobacco enum. What are it's values? Where do we derive a person's tobacco usage from? Extended keys? Is it actually tocbacco usage at time of application?

    }

    public class Name
    {
        public int NameId { get; set; } // Is this necessary? It must be monitored in EDA, but might not need to be stored.

        public string BusinessName { get; set; }

        public string IndividualPrefix { get; set; }

        public string IndividualFirst { get; set; }

        public string IndividualMiddle { get; set; }

        public string IndividualLast { get; set; }

        public string IndividualSuffix { get; set; }
    }

    public class Address
    {

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string City { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public State? StateAbbreviation { get; set; }

        public string ZipCode { get; set; }

        public string ZipExtension { get; set; }

        public string BoxNumber { get; set; }

        public Country? Country { get; set; }
    }

    public class Benefit
    {
        // More work to do here.
        public CoverageType CoverageType { get; set; }

        // must be long, not basic int
        public long BenefitId { get; set; } // Is this necessary? It must be monitored in EDA, but might not need to be stored.

        public string PlanCode { get; set; }

        public string BenefitCategory { get; set; }

        public string BenefitDescription { get; set; }

        public PolicyStatus BenefitStatus { get; set; }

        public PolicyStatusReason? BenefitStatusReason { get; set; }

        public decimal BenefitAmount { get; set; }

        /// <summary>
        /// This is exluded from out model for now until better understood. It appears to come from a LifePro surrender quote API. It should likely be retreived by upstream services on-demand in the same way.
        /// </summary>
        // public decimal CashValue { get; set; }

        public BenefitOptions? SelfBenefitOptions { get; set; }

        public BenefitOptions? SpouseBenefitOptions { get; set; }

        /// <summary>
        /// If children can have coverage options that differ from each other, this will need to be a list.
        /// </summary>
        public BenefitOptions? ChildBenefitOptions { get; set; }


    }

    /// <summary>
    /// Many more items need defined here based on extended key analysis.
    /// </summary>
    public class BenefitOptions
    {
        public List<ExtendedKey> ExtendedKeys { get; set; }
    }

    public class ExtendedKey
    {
        public string KeyOption { get; set; }

        public string KeyValue { get; set; }
    }
}
