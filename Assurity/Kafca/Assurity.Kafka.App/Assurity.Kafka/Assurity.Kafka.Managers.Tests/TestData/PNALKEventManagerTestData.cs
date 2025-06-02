namespace Assurity.Kafka.Managers.Tests.TestData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    [ExcludeFromCodeCoverage]
    public static class PNALKEventManagerTestData
    {
        public static PNALK ActivePNALK => new ()
        {
            ADDRESS_CODE = string.Empty,
            ADDRESS_ID = 234567,
            CANCEL_DATE = 0,
            NAME_ID = 123456,
            TELE_NUM = "4025551234"
        };

        public static Address AddressToBeAdded => new ()
        {
            AddressId = 234567,
            Line1 = "321 Real Ave",
            Line2 = "Unit A",
            Line3 = "Addr Line 3",
            City = "Carson",
            StateAbbreviation = State.IA,
            ZipCode = "51525",
            ZipExtension = "5678",
            BoxNumber = "51525",
            Country = Country.USA
        };

        public static Insured InsuredToBeAdded => new ()
        {
            RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
            Participant = ParticipantToBeAdded
        };

        public static Owner OwnerToBeAdded => new ()
        {
            OwnerType = OwnerType.Primary,
            Participant = ParticipantToBeAdded
        };

        public static Payor PayorToBeAdded => new ()
        {
            PayorType = PayorType.Primary,
            Participant = ParticipantToBeAdded
        };

        public static List<PolicyRelationship> PolicyRelationships => new ()
        {
            new PolicyRelationship
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",

                // Unit tests may overwrite this list for their specific participant type
                // (insured, owner, or payor) when targeting just one relate code.
                RelateCodes = new List<string>
                {
                    RelateCodes.Insured,
                    RelateCodes.Owner,
                    RelateCodes.Payor
                }
            }
        };

        public static List<Policy> PoliciesWithOnePolicyForNoParticipantOrAddress => new ()
        {
            new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                Insureds = new List<Insured>(),
                Owners = new List<Owner>(),
                Payors = new List<Payor>()
            }
        };

        public static List<Policy> PoliciesWithOnePolicyForExistingParticipantAndAddress => new ()
        {
            new Policy
            {
                CompanyCode = "01",
                PolicyNumber = "1234567890",
                Insureds = new List<Insured>
                {
                    new Insured
                    {
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self,
                        Participant = ParticipantForExistingParticipantAndAddress
                    }
                },
                Owners = new List<Owner>
                {
                    new Owner
                    {
                        OwnerType = OwnerType.Primary,
                        Participant = ParticipantForExistingParticipantAndAddress
                    }
                },
                Payors = new List<Payor>
                {
                    new Payor
                    {
                        PayorType = PayorType.Primary,
                        Participant = ParticipantForExistingParticipantAndAddress
                    }
                }
            }
        };

        private static Participant ParticipantForExistingParticipantAndAddress => new ()
        {
            Address = new Address
            {
                AddressId = 234567
            },
            Person = new Person
            {
                Name = new Name
                {
                    NameId = 123456
                }
            },

            // Purposely different from the ActivePNALK's TELE_NUM so that unit tests can
            // verify that an update was made.
            PhoneNumber = "9995559999"
        };

        private static Participant ParticipantToBeAdded => new ()
        {
            Address = AddressToBeAdded,
            Person = new Person
            {
                DateOfBirth = new DateTime(1970, 10, 15),
                EmailAddress = "bpreston@email.com",
                Gender = Gender.Male,
                Name = new Name
                {
                    NameId = 123456,
                    IndividualPrefix = "Mr.",
                    IndividualFirst = "Bill",
                    IndividualMiddle = "S",
                    IndividualLast = "Preston",
                    IndividualSuffix = "Esq."
                }
            },
            PhoneNumber = "4025551234"
        };
    }
}