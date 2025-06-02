namespace Assurity.Kafka.Engines.Tests.TestData
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    [ExcludeFromCodeCoverage]
    public static class PNALKEngineTestData
    {
        public static Address Address => new ()
        {
            AddressId = 234567,
            Line1 = "123 Fake St",
            Line2 = "APT 22",
            Line3 = "Line 3",
            City = "Lincoln",
            StateAbbreviation = State.NE,
            ZipCode = "68501",
            ZipExtension = "1234",
            BoxNumber = "68501",
            Country = Country.USA
        };

        public static PNALK PNALK => new ()
        {
            ADDRESS_CODE = string.Empty,
            ADDRESS_ID = 234567,
            CANCEL_DATE = 0,
            NAME_ID = 123456,
            TELE_NUM = "4025551234"
        };

        public static PNAME IndividualPNAME => new ()
        {
            NAME_ID = 123456,
            DATE_OF_BIRTH = 19700114,
            NAME_FORMAT_CODE = "I",
            INDIVIDUAL_PREFIX = "Mr.",
            INDIVIDUAL_FIRST = "John",
            INDIVIDUAL_MIDDLE = "A",
            INDIVIDUAL_LAST = "Wick",
            INDIVIDUAL_SUFFIX = "Sr.",
            PERSONAL_EMAIL_ADR = "jwick@email.com",
            SEX_CODE = "M"
        };

        public static PNAME AdditionalPNAME => new ()
        {
            NAME_ID = 500004,
            DATE_OF_BIRTH = 20001109,
            NAME_FORMAT_CODE = "I",
            INDIVIDUAL_PREFIX = "Mr.",
            INDIVIDUAL_FIRST = "Jacob",
            INDIVIDUAL_MIDDLE = "E",
            INDIVIDUAL_LAST = "Wick",
            INDIVIDUAL_SUFFIX = "Jr.",
            PERSONAL_EMAIL_ADR = "jacob@email.com",
            SEX_CODE = "M"
        };

        public static PNAME BusinessPNAME => new ()
        {
            NAME_ID = 123456,
            DATE_OF_BIRTH = 0,
            NAME_FORMAT_CODE = "B",
            NAME_BUSINESS = "Wick Enterprises",
            BUSINESS_EMAIL_ADR = "wickenterprises@email.com",
            INDIVIDUAL_PREFIX = string.Empty,
            INDIVIDUAL_FIRST = string.Empty,
            INDIVIDUAL_MIDDLE = string.Empty,
            INDIVIDUAL_LAST = string.Empty,
            INDIVIDUAL_SUFFIX = string.Empty,
            PERSONAL_EMAIL_ADR = string.Empty,
            SEX_CODE = string.Empty
        };

        public static List<PolicyRelationship> PolicyRelationships => new ()
        {
            new PolicyRelationship
            {
                CompanyCode = "01",
                PolicyNumber = "5150198400",
                RelateCodes = new List<string> { "IN" }
            },
            new PolicyRelationship
            {
                CompanyCode = "01",
                PolicyNumber = "5150198401",
                RelateCodes = new List<string> { "IN", "PA", "PE", "PO" }
            },
            new PolicyRelationship
            {
                CompanyCode = "02",
                PolicyNumber = "5150198450",
                RelateCodes = new List<string>()
            },
            new PolicyRelationship
            {
                CompanyCode = "01",
                PolicyNumber = "5150198409"
            }
        };
    }
}