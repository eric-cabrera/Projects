namespace Assurity.AgentPortal.Managers.Tests.Mapping.TestData
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    [ExcludeFromCodeCoverage]
    public static class PolicyDestinationTestData
    {
        public static PolicyResponse PendingPolicy => new()
        {
            Annuitants = new List<Annuitant>
            {
                new Annuitant
                {
                    Participant = PrimaryParticipant,
                    AnnuitantType = AnnuitantType.Primary
                }
            },
            Agents = new List<Agent>
            {
                new Agent
                {
                    AgentId = "API1",
                    IsServicingAgent = true,
                    IsWritingAgent = true,
                    Level = "90",
                    MarketCode = "IS",
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 748272,
                            BoxNumber = "7000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "123 Husker Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1973, 10, 16),
                            EmailAddress = "primaryAgent@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Primary",
                                IndividualMiddle = "I",
                                IndividualLast = "Agent",
                                NameId = 123123
                            }
                        },
                        PhoneNumber = "402-555-2222"
                    }
                }
            },
            Assignee = new Assignee
            {
                Participant = PrimaryParticipant
            },
            Id = "68768686863cb1245feda7576a66",
            AnnualPremium = 300.5M,
            Benefits = new List<BenefitResponse>
            {
                new BenefitResponse
                {
                    BenefitAmount = 220.68M,
                    BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                    BenefitId = 2580405,
                    BenefitOptions = new List<BenefitOptionResponse>
                    {
                        new BenefitOptionResponse
                        {
                            BenefitOptionName = MappingExtensions.GetEnumDisplayName(BenefitOptionName.PolicyFee),
                            BenefitOptionValue = MappingExtensions.GetEnumDisplayName(BenefitOptionValue.PolicyFee2000),
                            RelationshipToPrimaryInsured = MappingExtensions.GetEnumDisplayName(RelationshipToPrimaryInsured.Self),
                            StartDate = new DateTime(2023, 2, 1),
                            StopDate = new DateTime(9999, 12, 31)
                        }
                    },
                    BenefitStatus = MappingExtensions.GetEnumDisplayName(Status.Active),
                    BenefitStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
                    CoverageType = MappingExtensions.GetEnumDisplayName(CoverageType.Base),
                    DeathBenefitOption = MappingExtensions.GetEnumDisplayName(DeathBenefitOption.FaceAmountOption),
                    DividendOption = MappingExtensions.GetEnumDisplayName(DividendOption.NoDividend),
                    PlanCode = "W H1101"
                },
                new BenefitResponse
                {
                    BenefitAmount = 0M,
                    BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                    BenefitId = 2580406,
                    BenefitOptions = new List<BenefitOptionResponse>
                    {
                        new BenefitOptionResponse
                        {
                            BenefitOptionName = MappingExtensions.GetEnumDisplayName(BenefitOptionName.PolicyFee),
                            BenefitOptionValue = MappingExtensions.GetEnumDisplayName(BenefitOptionValue.PolicyFee560),
                            RelationshipToPrimaryInsured = MappingExtensions.GetEnumDisplayName(RelationshipToPrimaryInsured.Self),
                            StartDate = new DateTime(2023, 2, 1),
                            StopDate = new DateTime(9999, 12, 31)
                        }
                    },
                    BenefitStatus = MappingExtensions.GetEnumDisplayName(Status.Active),
                    BenefitStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
                    CoverageType = MappingExtensions.GetEnumDisplayName(CoverageType.Rider),
                    DeathBenefitOption = MappingExtensions.GetEnumDisplayName(DeathBenefitOption.FaceAmountOption),
                    DividendOption = MappingExtensions.GetEnumDisplayName(DividendOption.NoDividend),
                    PlanCode = "W H1101CC"
                }
            },
            Beneficiaries = new List<Beneficiary>
            {
                new Beneficiary
                {
                    BeneficiaryType = BeneficiaryType.Primary,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 5882300,
                            BoxNumber = "7000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "383 Arbor Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2009, 4, 18),
                            EmailAddress = "primarybeneficiary@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Primary",
                                IndividualLast = "Beneficiary",
                                IndividualMiddle = "A",
                                NameId = 5643411
                            }
                        },
                        PhoneNumber = "402-555-9889"
                    }
                },
                new Beneficiary
                {
                    BeneficiaryType = BeneficiaryType.Contingent,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 94334,
                            BoxNumber = "8000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "383 Arbor Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "8000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2011, 9, 22),
                            EmailAddress = "contingentbeneficiary@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Contingent",
                                IndividualLast = "Beneficiary",
                                IndividualMiddle = "B",
                                NameId = 239839
                            }
                        },
                        PhoneNumber = "402-555-9889"
                    }
                }
            },
            BillingDay = null,
            BillingForm = MappingExtensions.GetEnumDisplayName(BillingForm.Direct),
            BillingMode = MappingExtensions.GetEnumDisplayName(BillingMode.FiftyTwoPay),
            BillingStatus = MappingExtensions.GetEnumDisplayName(BillingStatus.Suspended),
            BillingReason = MappingExtensions.GetEnumDisplayName(BillingReason.WaiverDisability),
            CompanyCode = "01",
            Insureds = new List<Insured>
            {
                new Insured
                {
                    Participant = PrimaryParticipant,
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1984, 12, 23),
                            EmailAddress = "spouseinsured@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Spouse",
                                IndividualLast = "Insured",
                                IndividualMiddle = "B",
                                NameId = 7845121
                            }
                        },
                        PhoneNumber = "402-555-6233"
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2018, 7, 11),
                            Gender = Gender.Female,
                            Name = new Name
                            {
                                IndividualFirst = "Child1",
                                IndividualLast = "Insured",
                                IndividualMiddle = "C",
                                NameId = 3443211
                            }
                        },
                        PhoneNumber = "402-555-6227"
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2018, 7, 11),
                            Gender = Gender.Female,
                            Name = new Name
                            {
                                IndividualFirst = "Child2",
                                IndividualLast = "Insured",
                                IndividualMiddle = "D",
                                NameId = 6443211
                            }
                        },
                        PhoneNumber = "402-555-6227"
                    },
                    RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Child
                }
            },
            IssueDate = new DateTime(2023, 2, 1),
            IssueState = State.NE,
            LineOfBusiness = MappingExtensions.GetEnumDisplayName(LineOfBusiness.TraditionalLife),
            ModePremium = 66.16M,
            Owners = new List<Owner>
            {
                new Owner
                {
                    OwnerType = OwnerType.Primary,
                    Participant = PrimaryParticipant
                },
                new Owner
                {
                    OwnerType = OwnerType.Additional,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 234234,
                            BoxNumber = "8000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "5150 North Halen",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "8000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1977, 9, 9),
                            EmailAddress = "additionalowner@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Additional",
                                IndividualLast = "Owner",
                                IndividualMiddle = "B",
                                NameId = 898383
                            }
                        },
                        PhoneNumber = "402-555-5150"
                    }
                }
            },
            PaidToDate = null,
            Payors = new List<Payor>
            {
                new Payor
                {
                    Participant = PrimaryParticipant,
                    PayorType = PayorType.Primary
                }
            },
            PolicyNumber = "4531276890",
            PolicyStatus = MappingExtensions.GetEnumDisplayName(Status.Pending),
            PolicyStatusDetail = MappingExtensions.GetEnumDisplayName(PolicyStatusDetail.InsuredRequested),
            PolicyStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
            ProductCategory = "Accident Expense",
            ProductCode = "W H1101",
            ProductDescription = "Ind. PRO 24-hour Accident Expense",
            Requirements = new List<RequirementResponse>
            {
                new RequirementResponse
                {
                    ActionType = MappingExtensions.GetEnumDisplayName(RequirementActionType.SendMessage),
                    AddedDate = new DateTime(2023, 2, 1),
                    AppliesTo = PrimaryParticipant,
                    Display = true,
                    FulfillingParty = MappingExtensions.GetEnumDisplayName(RequirementFulfillingParty.Agent),
                    GlobalComment = "Received on time.",
                    Id = 1,
                    LifeProComment = "Action taken and accepted.",
                    Name = "APS",
                    ObtainedDate = new DateTime(2023, 2, 1),
                    PhoneNumberComment = "Who ya gonna call? 402-555-1234",
                    Status = MappingExtensions.GetEnumDisplayName(RequirementStatus.Met)
                }
            },
            ResidentState = State.NE,
            ReturnPaymentType = MappingExtensions.GetEnumDisplayName(ReturnPaymentType.InitialPaymentCardDeclined),
            SubmitDate = new DateTime(2023, 1, 27),
            ApplicationDate = new DateTime(2023, 1, 27),
            CreateDate = new DateTime(2023, 2, 1),
            LastModified = new DateTime(2023, 2, 1),
            LastStatusChangeDate = new DateTime(2023, 2, 1),
            TaxQualificationStatus = MappingExtensions.GetEnumDisplayName(TaxQualificationStatus.IRA)
        };

        public static PolicyResponse ActivePolicy => new()
        {
            Annuitants = new List<Annuitant>
            {
                new Annuitant
                {
                    Participant = PrimaryParticipant,
                    AnnuitantType = AnnuitantType.Primary
                }
            },
            Agents = new List<Agent>
            {
                new Agent
                {
                    AgentId = "API1",
                    IsServicingAgent = true,
                    IsWritingAgent = true,
                    Level = "90",
                    MarketCode = "IS",
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 748272,
                            BoxNumber = "7000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "123 Husker Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1973, 10, 16),
                            EmailAddress = "primaryAgent@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Primary",
                                IndividualMiddle = "I",
                                IndividualLast = "Agent",
                                NameId = 123123
                            }
                        },
                        PhoneNumber = "402-555-2222"
                    }
                }
            },
            Assignee = new Assignee
            {
                Participant = PrimaryParticipant
            },
            Id = "68768686863cb1245feda7576a66",
            AnnualPremium = 300.5M,
            Benefits = new List<BenefitResponse>
            {
                new BenefitResponse
                {
                    BenefitAmount = 220.68M,
                    BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                    BenefitId = 2580405,
                    BenefitOptions = new List<BenefitOptionResponse>
                    {
                        new BenefitOptionResponse
                        {
                            BenefitOptionName = MappingExtensions.GetEnumDisplayName(BenefitOptionName.PolicyFee),
                            BenefitOptionValue = MappingExtensions.GetEnumDisplayName(BenefitOptionValue.PolicyFee2000),
                            RelationshipToPrimaryInsured = MappingExtensions.GetEnumDisplayName(RelationshipToPrimaryInsured.Self),
                            StartDate = new DateTime(2023, 2, 1),
                            StopDate = new DateTime(9999, 12, 31)
                        }
                    },
                    BenefitStatus = MappingExtensions.GetEnumDisplayName(Status.Active),
                    BenefitStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
                    CoverageType = MappingExtensions.GetEnumDisplayName(CoverageType.Base),
                    DeathBenefitOption = MappingExtensions.GetEnumDisplayName(DeathBenefitOption.FaceAmountOption),
                    DividendOption = MappingExtensions.GetEnumDisplayName(DividendOption.NoDividend),
                    PlanCode = "W H1101"
                },
                new BenefitResponse
                {
                    BenefitAmount = 0M,
                    BenefitDescription = "Ind. PRO 24-hour Accident Expense",
                    BenefitId = 2580406,
                    BenefitOptions = new List<BenefitOptionResponse>
                    {
                        new BenefitOptionResponse
                        {
                            BenefitOptionName = MappingExtensions.GetEnumDisplayName(BenefitOptionName.PolicyFee),
                            BenefitOptionValue = MappingExtensions.GetEnumDisplayName(BenefitOptionValue.PolicyFee560),
                            RelationshipToPrimaryInsured = MappingExtensions.GetEnumDisplayName(RelationshipToPrimaryInsured.Self),
                            StartDate = new DateTime(2023, 2, 1),
                            StopDate = new DateTime(9999, 12, 31)
                        }
                    },
                    BenefitStatus = MappingExtensions.GetEnumDisplayName(Status.Active),
                    BenefitStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
                    CoverageType = MappingExtensions.GetEnumDisplayName(CoverageType.Rider),
                    DeathBenefitOption = MappingExtensions.GetEnumDisplayName(DeathBenefitOption.FaceAmountOption),
                    DividendOption = MappingExtensions.GetEnumDisplayName(DividendOption.NoDividend),
                    PlanCode = "W H1101CC"
                }
            },
            Beneficiaries = new List<Beneficiary>
            {
                new Beneficiary
                {
                    BeneficiaryType = BeneficiaryType.Primary,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 5882300,
                            BoxNumber = "7000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "383 Arbor Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2009, 4, 18),
                            EmailAddress = "primarybeneficiary@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Primary",
                                IndividualLast = "Beneficiary",
                                IndividualMiddle = "A",
                                NameId = 5643411
                            }
                        },
                        PhoneNumber = "402-555-9889"
                    }
                },
                new Beneficiary
                {
                    BeneficiaryType = BeneficiaryType.Contingent,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 94334,
                            BoxNumber = "8000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "383 Arbor Street",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "8000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2011, 9, 22),
                            EmailAddress = "contingentbeneficiary@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Contingent",
                                IndividualLast = "Beneficiary",
                                IndividualMiddle = "B",
                                NameId = 239839
                            }
                        },
                        PhoneNumber = "402-555-9889"
                    }
                }
            },
            BillingDay = null,
            BillingForm = MappingExtensions.GetEnumDisplayName(BillingForm.Direct),
            BillingMode = MappingExtensions.GetEnumDisplayName(BillingMode.FiftyTwoPay),
            BillingStatus = MappingExtensions.GetEnumDisplayName(BillingStatus.Suspended),
            BillingReason = MappingExtensions.GetEnumDisplayName(BillingReason.WaiverDisability),
            CompanyCode = "01",
            Insureds = new List<Insured>
            {
                new Insured
                {
                    Participant = PrimaryParticipant,
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1984, 12, 23),
                            EmailAddress = "spouseinsured@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Spouse",
                                IndividualLast = "Insured",
                                IndividualMiddle = "B",
                                NameId = 7845121
                            }
                        },
                        PhoneNumber = "402-555-6233"
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2018, 7, 11),
                            Gender = Gender.Female,
                            Name = new Name
                            {
                                IndividualFirst = "Child1",
                                IndividualLast = "Insured",
                                IndividualMiddle = "C",
                                NameId = 3443211
                            }
                        },
                        PhoneNumber = "402-555-6227"
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
                            Line1 = "88 Flux Road",
                            StateAbbreviation = State.NE,
                            ZipCode = "68502",
                            ZipExtension = "7000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(2018, 7, 11),
                            Gender = Gender.Female,
                            Name = new Name
                            {
                                IndividualFirst = "Child2",
                                IndividualLast = "Insured",
                                IndividualMiddle = "D",
                                NameId = 6443211
                            }
                        },
                        PhoneNumber = "402-555-6227"
                    },
                    RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Child
                }
            },
            IssueDate = new DateTime(2023, 2, 1),
            IssueState = State.NE,
            LineOfBusiness = MappingExtensions.GetEnumDisplayName(LineOfBusiness.TraditionalLife),
            ModePremium = 66.16M,
            Owners = new List<Owner>
            {
                new Owner
                {
                    OwnerType = OwnerType.Primary,
                    Participant = PrimaryParticipant
                },
                new Owner
                {
                    OwnerType = OwnerType.Additional,
                    Participant = new Participant
                    {
                        Address = new Address
                        {
                            AddressId = 234234,
                            BoxNumber = "8000",
                            City = "Lincoln",
                            Country = Country.USA,
                            Line1 = "5150 North Halen",
                            StateAbbreviation = State.NE,
                            ZipCode = "68501",
                            ZipExtension = "8000"
                        },
                        Person = new Person
                        {
                            DateOfBirth = new DateTime(1977, 9, 9),
                            EmailAddress = "additionalowner@assurity.com",
                            Gender = Gender.Male,
                            Name = new Name
                            {
                                IndividualFirst = "Additional",
                                IndividualLast = "Owner",
                                IndividualMiddle = "B",
                                NameId = 898383
                            }
                        },
                        PhoneNumber = "402-555-5150"
                    }
                }
            },
            PaidToDate = new DateTime(2023, 2, 1),
            Payors = new List<Payor>
            {
                new Payor
                {
                    Participant = PrimaryParticipant,
                    PayorType = PayorType.Primary
                }
            },
            PolicyNumber = "4531276890",
            PolicyStatus = MappingExtensions.GetEnumDisplayName(Status.Active),
            PolicyStatusDetail = MappingExtensions.GetEnumDisplayName(PolicyStatusDetail.InsuredRequested),
            PolicyStatusReason = MappingExtensions.GetEnumDisplayName(StatusReason.Lapsed),
            ProductCategory = "Accident Expense",
            ProductCode = "W H1101",
            ProductDescription = "Ind. PRO 24-hour Accident Expense",
            Requirements = new List<RequirementResponse>
            {
                new RequirementResponse
                {
                    ActionType = MappingExtensions.GetEnumDisplayName(RequirementActionType.SendMessage),
                    AddedDate = new DateTime(2023, 2, 1),
                    AppliesTo = PrimaryParticipant,
                    Display = true,
                    FulfillingParty = MappingExtensions.GetEnumDisplayName(RequirementFulfillingParty.Agent),
                    GlobalComment = "Received on time.",
                    Id = 1,
                    LifeProComment = "Action taken and accepted.",
                    Name = "APS",
                    ObtainedDate = new DateTime(2023, 2, 1),
                    PhoneNumberComment = "Who ya gonna call? 402-555-1234",
                    Status = MappingExtensions.GetEnumDisplayName(RequirementStatus.Met)
                }
            },
            ResidentState = State.NE,
            ReturnPaymentType = MappingExtensions.GetEnumDisplayName(ReturnPaymentType.InitialPaymentCardDeclined),
            SubmitDate = new DateTime(2023, 1, 27),
            ApplicationDate = new DateTime(2023, 1, 27),
            CreateDate = new DateTime(2023, 2, 1),
            LastModified = new DateTime(2023, 2, 1),
            LastStatusChangeDate = new DateTime(2023, 2, 1),
            TaxQualificationStatus = MappingExtensions.GetEnumDisplayName(TaxQualificationStatus.IRA)
        };

        public static Employer Employer => new()
        {
            Business = new()
            {
                Name = new()
                {
                    BusinessName = "Assurity Life Insurance Company",
                    NameId = 111110
                },
                EmailAddress = "assurity@assurity.com"
            },
            Number = "A123001"
        };

        public static Participant PrimaryParticipant => new()
        {
            Address = new Address
            {
                AddressId = 5882300,
                BoxNumber = "7000",
                City = "Lincoln",
                Country = Country.USA,
                Line1 = "88 Flux Road",
                StateAbbreviation = State.NE,
                ZipCode = "68502",
                ZipExtension = "7000"
            },
            Person = new Person
            {
                DateOfBirth = new DateTime(1981, 1, 16),
                EmailAddress = "primaryinsured@assurity.com",
                Gender = Gender.Female,
                Name = new Name
                {
                    IndividualFirst = "Primary",
                    IndividualLast = "Insured",
                    IndividualMiddle = "A",
                    NameId = 5643211
                }
            },
            PhoneNumber = "402-555-6227"
        };
    }
}