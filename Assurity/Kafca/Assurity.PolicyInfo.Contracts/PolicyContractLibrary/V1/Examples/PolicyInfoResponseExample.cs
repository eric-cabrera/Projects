namespace Assurity.PolicyInfo.Contracts.V1.Examples
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    [ExcludeFromCodeCoverage]
    public class PolicyInfoResponseExample
    {
        public Dictionary<string, Policy> GetExamples()
        {
            return new Dictionary<string, Policy>
            {
                { nameof(Policy), GetPolicyInfoExample() }
            };
        }

        private Policy GetPolicyInfoExample()
        {
            var participant = new Participant
            {
                Address = new Address
                {
                    AddressId = 5882300,
                    BoxNumber = "7000",
                    City = "Lincoln",
                    Country = Country.USA,
                    Line1 = "123 Main St",
                    StateAbbreviation = State.NE,
                    ZipCode = "68516"
                },
                Business = null,
                IsBusiness = false,
                Person = new Person
                {
                    DateOfBirth = new DateTime(1976, 05, 07),
                    EmailAddress = "jsmith2@gmail.com",
                    Gender = Gender.Male,
                    Name = new Name
                    {
                        IndividualFirst = "John",
                        IndividualLast = "Smith",
                        IndividualMiddle = "A",
                        NameId = 5643211
                    },
                },
                PhoneNumber = "402-908-6789",
            };

            return new Policy
            {
                Annuitants = new List<Annuitant>
                {
                    new Annuitant
                    {
                        Participant = participant,
                        AnnuitantType = AnnuitantType.Primary
                    }
                },
                Agents = new List<Agent>
                {
                    new Agent
                    {
                        AgentId = "1234",
                        IsWritingAgent = true,
                        IsServicingAgent = false,
                        Level = "11",
                        MarketCode = "ABCD"
                    },
                    new Agent
                    {
                        AgentId = "5678",
                        IsWritingAgent = false,
                        IsServicingAgent = true,
                        Level = "12",
                        MarketCode = "AB-1"
                    }
                },
                Assignee = new Assignee
                {
                    Participant = participant
                },
                Id = "68768686863cb1245feda7576a66",
                AnnualPremium = 300.50m,
                Benefits = new List<Benefit>
                {
                    new Benefit
                    {
                        BenefitAmount = 220.68m,
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitId = 2580405,
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Lapsed,
                        CoverageType = CoverageType.Base,
                        PlanCode = "W H1101   ",
                        DeathBenefitOption = DeathBenefitOption.FaceAmountOption,
                        DividendOption = DividendOption.NoDividend,
                        BenefitOptions = new List<BenefitOption>
                        {
                            new BenefitOption
                            {
                                BenefitOptionName = BenefitOptionName.PolicyFee,
                                BenefitOptionValue = BenefitOptionValue.PolicyFee2000,
                                StartDate = new DateTime(2021, 11, 21),
                                StopDate = new DateTime(9999, 12, 31)
                            }
                        }
                    },
                    new Benefit
                    {
                        BenefitAmount = 0,
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitId = 2580406,
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Error,
                        CoverageType = CoverageType.Rider,
                        PlanCode = "W H1101CC ",
                        BenefitOptions = new List<BenefitOption>
                        {
                            new BenefitOption
                            {
                                BenefitOptionName = BenefitOptionName.PolicyFee,
                                BenefitOptionValue = BenefitOptionValue.PolicyFee560,
                                StartDate = new DateTime(2021, 11, 21),
                                StopDate = new DateTime(9999, 12, 31)
                            }
                        }
                    },
                    new Benefit
                    {
                        BenefitAmount = 41,
                        BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                        BenefitId = 2580407,
                        BenefitStatus = Status.Terminated,
                        BenefitStatusReason = StatusReason.Error,
                        CoverageType = CoverageType.Rider,
                        PlanCode = "R W1110   ",
                        BenefitOptions = new List<BenefitOption>
                        {
                            new BenefitOption
                            {
                                BenefitOptionName = BenefitOptionName.EliminationPeriod,
                                BenefitOptionValue = BenefitOptionValue.NinetyDays,
                                StartDate = new DateTime(2021, 11, 21),
                                StopDate = new DateTime(9999, 12, 31)
                            }
                        }
                    }
                },
                Beneficiaries = new List<Beneficiary>
                {
                    new Beneficiary
                    {
                        Participant = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(2015, 02, 02),
                                EmailAddress = "jsmith2@gmail.com",
                                Gender = Gender.Male,
                                Name = new Name
                                {
                                    IndividualFirst = "Jacob",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "E",
                                    NameId = 5643411
                                },
                            },
                            PhoneNumber = "402-908-6789",
                        }
                    }
                },
                BillingDay = 0,
                BillingForm = BillingForm.Direct,
                BillingMode = BillingMode.FiftyTwoPay,
                BillingStatus = BillingStatus.Suspended,
                BillingReason = BillingReason.WaiverDisability,
                CompanyCode = "01",
                Insureds = new List<Insured>
                {
                    new Insured
                    {
                        Participant = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(1976, 05, 07),
                                EmailAddress = "jsmith2@gmail.com",
                                Gender = Gender.Male,
                                Name = new Name
                                {
                                    IndividualFirst = "John",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "A",
                                    NameId = 5643211
                                },
                            },
                            PhoneNumber = "402-908-6789",
                        },
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Self
                    },
                    new Insured
                    {
                        Participant = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(1980, 02, 01),
                                EmailAddress = "jsmith2@gmail.com",
                                Gender = Gender.Male,
                                Name = new Name
                                {
                                    IndividualFirst = "Jill",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "E",
                                    NameId = 7845121
                                },
                            },
                            PhoneNumber = "402-546-1245",
                        },
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Spouse
                    },
                    new Insured
                    {
                        Participant = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(2003, 09, 08),
                                Gender = Gender.Female,
                                Name = new Name
                                {
                                    IndividualFirst = "Kathy",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "A",
                                    NameId = 3443211
                                },
                            },
                            PhoneNumber = "402-238-4512",
                        },
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Child
                    },
                    new Insured
                    {
                        Participant = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(2008, 03, 01),
                                Gender = Gender.Male,
                                Name = new Name
                                {
                                    IndividualFirst = "Matt",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "S",
                                    NameId = 6443211
                                },
                            }
                        },
                        RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Child
                    }
                },
                IssueDate = new DateTime(2021, 11, 21),
                IssueState = State.NE,
                LineOfBusiness = LineOfBusiness.TraditionalLife,
                ModePremium = 66.16m,
                Owners = new List<Owner>
                {
                    new Owner
                    {
                        OwnerType = OwnerType.Primary,
                        Participant = participant
                    }
                },
                PastDue = false,
                PaidToDate = new DateTime(2021, 10, 19),
                Payors = new List<Payor>
                {
                    new Payor
                    {
                        Participant = participant,
                        PayorType = PayorType.Primary
                    },
                },
                Payee = new Payee
                {
                    Participant = participant
                },
                PolicyNumber = "4531276890",
                PolicyStatus = Status.Terminated,
                PolicyStatusDetail = PolicyStatusDetail.InsuredRequested,
                PolicyStatusReason = StatusReason.Error,
                ProductCategory = "Accident Expense",
                ProductCode = "W H1101   ",
                ProductDescription = "Ind. PRO 24-hour Accident Expense",
                Requirements = new List<Requirement>
                {
                    new Requirement
                    {
                        Id = 17,
                        Name = "APS",
                        Status = RequirementStatus.Met,
                        AddedDate = new DateTime(2021, 11, 23),
                        ObtainedDate = null,
                        AppliesTo = new Participant
                        {
                            Address = new Address
                            {
                                AddressId = 5882300,
                                BoxNumber = "7000",
                                City = "Lincoln",
                                Country = Country.USA,
                                Line1 = "123 Main St",
                                StateAbbreviation = State.NE,
                                ZipCode = "68516"
                            },
                            Business = null,
                            IsBusiness = false,
                            Person = new Person
                            {
                                DateOfBirth = new DateTime(2008, 03, 01),
                                Gender = Gender.Male,
                                Name = new Name
                                {
                                    IndividualFirst = "Matt",
                                    IndividualLast = "Smith",
                                    IndividualMiddle = "S",
                                    NameId = 6443211
                                },
                            }
                        },
                        LifeProComment = "Unknown",
                        GlobalComment = null,
                        PhoneNumberComment = null,
                        FulfillingParty = RequirementFulfillingParty.Agent,
                        ActionType = RequirementActionType.UploadFileOrSendMessage,
                        Display = true
                    }
                },
                ResidentState = State.NE,
                ReturnPaymentType = ReturnPaymentType.None,
                SubmitDate = new DateTime(2021, 12, 01),
                ApplicationDate = new DateTime(2021, 11, 23),
                ApplicationReceivedDate = new DateTime(2021, 11, 23),
                CreateDate = new DateTime(2021, 09, 28),
                LastModified = new DateTime(2022, 11, 28),
                LastStatusChangeDate = new DateTime(2022, 11, 28),
                TerminationDate = new DateTime(2022, 11, 28),
                TaxQualificationStatus = TaxQualificationStatus.IRA
            };
        }
    }
}