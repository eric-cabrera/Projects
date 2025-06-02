namespace Assurity.Kafka.Accessors.Tests.TestData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;

    [ExcludeFromCodeCoverage]
    public static class DataStoreAndLifeProTestData
    {
        public static string PPOLCWithAllNavigationPropertiesPolicyNumber => "151058190581";

        public static string PPOLCWithAnnuityNull => "151058190582";

        public static string PPOLCWithGroupNull => "151058190583";

        public static string PPOLCWithNewBusinessPendingNull => "151058190585";

        public static string PPOLCWithNavigationPropertiesNull => "151058190584";

        public static string PRELANavigationPropertiesCompanyCode => "01";

        public static string PRELANavigationPropertiesPolicyNumber => "5150198401";

        public static string PPBENNavigationPropertiesPolicyNumber => "84563212222";

        public static List<PNALK> PNALKEntities =>
            new ()
            {
                new PNALK
                {
                    NAME_ID = 500001,
                    ADDRESS_CODE = string.Empty,
                    ADDRESS_ID = 700001,
                    CANCEL_DATE = 0,
                    TELE_NUM = "4025550001"
                },
                new PNALK
                {
                    NAME_ID = 500001,
                    ADDRESS_CODE = "RES",
                    ADDRESS_ID = 800091,
                    CANCEL_DATE = 0,
                    TELE_NUM = "4025559991"
                },
                new PNALK
                {
                    NAME_ID = 500001,
                    ADDRESS_CODE = string.Empty,
                    ADDRESS_ID = 800092,
                    CANCEL_DATE = 20221015,
                    TELE_NUM = "4025559992"
                },
                new PNALK
                {
                    NAME_ID = 500002,
                    ADDRESS_CODE = string.Empty,
                    ADDRESS_ID = 700002,
                    CANCEL_DATE = 0,
                    TELE_NUM = "4025550002"
                },
                new PNALK
                {
                    NAME_ID = 500009,
                    ADDRESS_CODE = string.Empty,
                    ADDRESS_ID = 700003,
                    CANCEL_DATE = 0,
                    TELE_NUM = "4025550003"
                },
                new PNALK
                {
                    NAME_ID = 500009,
                    ADDRESS_CODE = "BUS",
                    ADDRESS_ID = 800093,
                    CANCEL_DATE = 20221102,
                    TELE_NUM = "4025550003"
                }
            };

        public static List<PNAME> PNAMEEntities =>
            new ()
            {
                new PNAME
                {
                    NAME_ID = 822007,
                    NAME_BUSINESS = "ABC Company",
                    BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                    INDIVIDUAL_FIRST = string.Empty,
                    INDIVIDUAL_LAST = string.Empty,
                    INDIVIDUAL_MIDDLE = string.Empty,
                    INDIVIDUAL_PREFIX = string.Empty,
                    INDIVIDUAL_SUFFIX = string.Empty,
                    DATE_OF_BIRTH = 0,
                    NAME_FORMAT_CODE = string.Empty,
                    SEX_CODE = string.Empty,
                    PERSONAL_EMAIL_ADR = string.Empty
                },
                new PNAME
                {
                    NAME_ID = 522007,
                    NAME_BUSINESS = "Lincoln Public Schools",
                    BUSINESS_EMAIL_ADR = "lps@gmail.com",
                    INDIVIDUAL_FIRST = string.Empty,
                    INDIVIDUAL_LAST = string.Empty,
                    INDIVIDUAL_MIDDLE = string.Empty,
                    INDIVIDUAL_PREFIX = string.Empty,
                    INDIVIDUAL_SUFFIX = string.Empty,
                    DATE_OF_BIRTH = 0,
                    NAME_FORMAT_CODE = string.Empty,
                    SEX_CODE = string.Empty,
                    PERSONAL_EMAIL_ADR = string.Empty
                }
            };

        public static List<PGRUP_GROUP_MASTER> PGRUPGroupMasterEntities =>
            new ()
            {
                new PGRUP_GROUP_MASTER
                {
                    COMPANY_CODE = "01",
                    GROUP_NUMBER = "7457234579",
                    NAME_ID = 822007
                },
                new PGRUP_GROUP_MASTER
                {
                    COMPANY_CODE = "01",
                    GROUP_NUMBER = "8907290897",
                    NAME_ID = 522007
                }
            };

        public static List<PPBEN_POLICY_BENEFITS> PPBENPolicyBenefitsEntities =>
            new ()
            {
                new PPBEN_POLICY_BENEFITS
                {
                    PBEN_ID = 54321,
                    POLICY_NUMBER = "8819851955",
                    COMPANY_CODE = "01",
                    BENEFIT_SEQ = 2,
                    BENEFIT_TYPE = BenefitTypes.OtherRider,
                    PLAN_CODE = "ADIR-E",
                    STATUS_CODE = "A",
                    STATUS_REASON = string.Empty,
                    STATUS_DATE = 0
                }
            };

        public static List<PPBEN_POLICY_BENEFITS_TYPES_BF> PPBENPolicyBenefitsTypesBFEntities =>
            new ()
            {
                new PPBEN_POLICY_BENEFITS_TYPES_BF
                {
                    PBEN_ID = 277772,
                    ANN_PREM_PER_UNIT = 100M,
                    NUMBER_OF_UNITS = 9M,
                    BF_DB_OPTION = "1",
                    VALUE_PER_UNIT = 1000.00M,
                    BF_DATE_NEGATIVE = 20180119
                },
                new PPBEN_POLICY_BENEFITS_TYPES_BF
                {
                    PBEN_ID = 277773,
                    ANN_PREM_PER_UNIT = 100M,
                    NUMBER_OF_UNITS = 9M,
                    BF_DB_OPTION = "1",
                    VALUE_PER_UNIT = 1000.00M,
                    BF_DATE_NEGATIVE = 0
                }
            };

        public static List<PCOMC_COMMISSION_CONTROL> PCOMCCommissionControlEntities =>
            new ()
            {
                new PCOMC_COMMISSION_CONTROL
                {
                    COMC_ID = 1234,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "1122334455",
                    RECORD_TYPE = "S"
                },
                new PCOMC_COMMISSION_CONTROL
                {
                    COMC_ID = 2341,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "1122334456",
                    RECORD_TYPE = "S"
                },
                new PCOMC_COMMISSION_CONTROL
                {
                    COMC_ID = 2342,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "1122334457",
                    RECORD_TYPE = "S"
                }
            };

        public static List<PCOMC_COMMISSION_CONTROL_TYPE_S> PCOMCCommissionControlTypeSEntities =>
            new ()
            {
                new PCOMC_COMMISSION_CONTROL_TYPE_S
                {
                    AGENT = "1111",
                    AGENT_LEVEL = "90",
                    COMC_ID = 1234,
                    MARKET_CODE = "IS",
                    SERVICE_AGENT_IND = "X",
                    COMM_PCNT = 100.00m
                },
                new PCOMC_COMMISSION_CONTROL_TYPE_S
                {
                    AGENT = "1112",
                    AGENT_LEVEL = "90",
                    COMC_ID = 2341,
                    MARKET_CODE = "IS",
                    SERVICE_AGENT_IND = string.Empty,
                    COMM_PCNT = 0
                },
                new PCOMC_COMMISSION_CONTROL_TYPE_S
                {
                    AGENT = "1114",
                    AGENT_LEVEL = "90",
                    COMC_ID = 2342,
                    MARKET_CODE = "IS",
                    SERVICE_AGENT_IND = "X",
                    COMM_PCNT = 0
                },
            };

        public static List<PAGNT_AGENT_MASTER> PAGNTAgentMasterEntities =>
             new ()
             {
                new PAGNT_AGENT_MASTER
                {
                    AGNT_ID = 1,
                    COMPANY_CODE = "01",
                    AGENT_NUMBER = "13456",
                    NAME_ID = 885696
                },
                new PAGNT_AGENT_MASTER
                {
                    AGNT_ID = 2,
                    COMPANY_CODE = "01",
                    AGENT_NUMBER = "00018",
                    NAME_ID = 882100
                },
                new PAGNT_AGENT_MASTER
                {
                    AGNT_ID = 3,
                    COMPANY_CODE = "01",
                    AGENT_NUMBER = "00011",
                    NAME_ID = 982100
                }
             };

        public static List<PRELA_RELATIONSHIP_MASTER> PRELARelationshipMasterEntities =>
            new ()
            {
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198401",
                    NAME_ID = 500001,
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198401",
                    NAME_ID = 500001,
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 1
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198401",
                    NAME_ID = 500001,
                    RELATE_CODE = "PA"
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198401",
                    NAME_ID = 500001,
                    RELATE_CODE = "PO"
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198402",
                    NAME_ID = 500002,
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 2
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198402",
                    NAME_ID = 500002,
                    RELATE_CODE = "PA",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198402",
                    NAME_ID = 500002,
                    RELATE_CODE = "PO",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198402",
                    NAME_ID = 500002,
                    RELATE_CODE = "PE",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "015150198402",
                    NAME_ID = 500002,
                    RELATE_CODE = "ML",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "025150198450",
                    NAME_ID = 500009,
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "O1",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "PO",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "PA",
                    BENEFIT_SEQ_NUMBER = 0
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 1
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819851955",
                    NAME_ID = 880001,
                    RELATE_CODE = "ML",
                    BENEFIT_SEQ_NUMBER = 2
                },
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = "018819852015",
                    NAME_ID = 880002,
                    RELATE_CODE = "ML",
                    BENEFIT_SEQ_NUMBER = 4
                }
            };

        public static List<PICDA_WAIVER_DETAILS> GetPICDAWaiverDetailsEntities =>
            new ()
            {
                new PICDA_WAIVER_DETAILS
                {
                    KEY_DATA = "01",
                    TYPE_CODE = "Y",
                    RECORD_SEQUENCE = 0,
                    DESCRIPTION = "INSURED REQUESTED"
                },
                new PICDA_WAIVER_DETAILS
                {
                    KEY_DATA = "11",
                    TYPE_CODE = "Y",
                    RECORD_SEQUENCE = 0,
                    DESCRIPTION = "LAPSED"
                }
            };

        public static List<PPOLM_POLICY_BENEFIT_MISC> GetPPOLMPolicyBenefitMiscEntities =>
            new ()
            {
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 1,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CANCEL_REASON = "11",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 2,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198402",
                    CANCEL_REASON = "01",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 3,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198450",
                    CANCEL_REASON = "15",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 4,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "8819851955",
                    CANCEL_REASON = "17",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 5,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "8819852015",
                    CANCEL_REASON = "18",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 6,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CANCEL_REASON = "17",
                    CANCEL_DESC = "FREE LOOK PERIOD - BAD CHECK OR CREDIT CARD"
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 7,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "6150198401",
                    CANCEL_REASON = "17",
                    CANCEL_DESC = string.Empty
                },
                new PPOLM_POLICY_BENEFIT_MISC
                {
                    POLM_ID = 8,
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "7150198401",
                    CANCEL_REASON = "18",
                    CANCEL_DESC = string.Empty
                },
            };

        public static List<PRQRM> GetPRQRMEntities =>
            new ()
            {
                new PRQRM
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    REQ_SEQUENCE = 0,
                    NAME_ID = 922226,
                    LAST_CHG_DATE = 20081118
                }
            };

        public static List<PRQRMTBL> GetPRQRMTBLEntities =>
            new ()
            {
                new PRQRMTBL
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    NAME_ID = 922226,
                    REQ_SEQUENCE = 0,
                    SEQ = 1,
                    UND_REQ_MET = "Y",
                    UND_DESC_CODE = 1,
                    UND_COMMENTS = string.Empty,
                    UND_REQ_DATE = 20081023,
                    UND_OBTAIN_DATE = 20201129
                },
                new PRQRMTBL
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    NAME_ID = 922226,
                    REQ_SEQUENCE = 0,
                    SEQ = 2,
                    UND_REQ_MET = "Y",
                    UND_DESC_CODE = 98,
                    UND_COMMENTS = string.Empty,
                    UND_REQ_DATE = 20081023,
                    UND_OBTAIN_DATE = 20201129
                },
                new PRQRMTBL
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    NAME_ID = 922226,
                    REQ_SEQUENCE = 0,
                    SEQ = 3,
                    UND_REQ_MET = "Y",
                    UND_DESC_CODE = 44,
                    UND_COMMENTS = "OK AS APPLIED FOR!",
                    UND_REQ_DATE = 20081029,
                    UND_OBTAIN_DATE = 20201129
                },
                new PRQRMTBL
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    NAME_ID = 922226,
                    REQ_SEQUENCE = 0,
                    SEQ = 4,
                    UND_REQ_MET = "Y",
                    UND_DESC_CODE = 48,
                    UND_COMMENTS = "$148.62",
                    UND_REQ_DATE = 20081023,
                    UND_OBTAIN_DATE = 20201129
                },
                new PRQRMTBL
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "4150299379",
                    NAME_ID = 922226,
                    REQ_SEQUENCE = 0,
                    SEQ = 5,
                    UND_REQ_MET = "Y",
                    UND_DESC_CODE = 1,
                    UND_COMMENTS = string.Empty,
                    UND_REQ_DATE = 20081030,
                    UND_OBTAIN_DATE = 20201129
                }
            };

        public static List<PMEDR> GetPMEDREntities =>
            new ()
            {
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 1,
                    REQ_NAME = "MIB",
                    REQ_DESCRIPTION = "MEDICAL INFORMATION BUREAU"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 2,
                    REQ_NAME = "SCRPTCHK",
                    REQ_DESCRIPTION = "SCRIPT CHECK"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 4,
                    REQ_NAME = "AGT CONT",
                    REQ_DESCRIPTION = "AGENT CONTRACTING"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 98,
                    REQ_NAME = "HOINSPE",
                    REQ_DESCRIPTION = "HOME OFFICE INSPECTION"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 44,
                    REQ_NAME = "CASE APP",
                    REQ_DESCRIPTION = "CASE APPROVED"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 48,
                    REQ_NAME = "BAL PREM",
                    REQ_DESCRIPTION = "BALANCE OF 1ST PREMIUM"
                },
                new PMEDR
                {
                    RECORD_TYPE = "R",
                    REQ_NUMBER = 61,
                    REQ_NAME = "STE HLTH",
                    REQ_DESCRIPTION = "DELIVERY ACKNOWLEDGEMENT AND AGREEMENT"
                },
            };

        public static List<PACTG> GetPACTGEntities =>
            new ()
            {
                new PACTG
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    LIFEPRO_ID = 171,
                    DATE_ADDED = 20070409,
                    TIME_ADDED = 8073374,
                    BENEFIT_SEQ = 1,
                    RECORD_SEQUENCE = 0,
                    CREDIT_CODE = 110,
                    DEBIT_CODE = 771,
                    EFFECTIVE_DATE = 20091214,
                    REVERSAL_CODE = "Y"
                },
                new PACTG
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    LIFEPRO_ID = 172,
                    DATE_ADDED = 20070409,
                    TIME_ADDED = 8073374,
                    BENEFIT_SEQ = 1,
                    RECORD_SEQUENCE = 0,
                    CREDIT_CODE = 2,
                    DEBIT_CODE = 10,
                    EFFECTIVE_DATE = 20181214,
                    REVERSAL_CODE = "Y"
                },
                new PACTG
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "6150198401",
                    LIFEPRO_ID = 173,
                    DATE_ADDED = 20070409,
                    TIME_ADDED = 8073374,
                    BENEFIT_SEQ = 1,
                    RECORD_SEQUENCE = 0,
                    CREDIT_CODE = 2,
                    DEBIT_CODE = 10,
                    EFFECTIVE_DATE = 20211214,
                    REVERSAL_CODE = "Y"
                },
                new PACTG
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "6150198401",
                    LIFEPRO_ID = 172,
                    DATE_ADDED = 20070409,
                    TIME_ADDED = 8073374,
                    BENEFIT_SEQ = 1,
                    RECORD_SEQUENCE = 0,
                    CREDIT_CODE = 252,
                    DEBIT_CODE = 123,
                    EFFECTIVE_DATE = 20191214,
                    REVERSAL_CODE = "Y"
                },
                new PACTG
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "7150198401",
                    LIFEPRO_ID = 172,
                    DATE_ADDED = 20070409,
                    TIME_ADDED = 8073374,
                    BENEFIT_SEQ = 1,
                    RECORD_SEQUENCE = 0,
                    DEBIT_CODE = 252,
                    CREDIT_CODE = 131,
                    EFFECTIVE_DATE = 20221214,
                    REVERSAL_CODE = "Y"
                }
            };

        public static List<PBDRV> GetPBDRVEntities =>
            new ()
            {
                new PBDRV
                {
                    DESCRIPTION = "YEAREND",
                    STATUS_CODE = "B",
                    BATCH_START_DATE = 5132023,
                    BATCH_STOP_DATE = 5152023
                },
                new PBDRV
                {
                    DESCRIPTION = "USER DEFINED",
                    STATUS_CODE = "D",
                    BATCH_START_DATE = 4012023,
                    BATCH_STOP_DATE = 4302023
                },
            };

        public static List<PACON_ANNUITY_POLICY> GetPACONAnnuityPolicyEntities =>
            new ()
            {
                new PACON_ANNUITY_POLICY
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "123456789",
                    STATUS_CODE = "T",
                    STATUS_REASON = "DC",
                    STATUS_DATE = 20151202,
                    ISSUE_DATE = 20011201,
                    TAX_QUALIFICATION = "2"
                },
                new PACON_ANNUITY_POLICY
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "345678912",
                    STATUS_CODE = "A",
                    STATUS_REASON = string.Empty,
                    STATUS_DATE = 20071101,
                    ISSUE_DATE = 20071101,
                    TAX_QUALIFICATION = "0"
                },
                new PACON_ANNUITY_POLICY
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "453216789",
                    STATUS_CODE = "T",
                    STATUS_REASON = "SR",
                    STATUS_DATE = 20080929,
                    ISSUE_DATE = 20071023,
                    TAX_QUALIFICATION = "0"
                },
                new PACON_ANNUITY_POLICY
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "553216789",
                    STATUS_CODE = "T",
                    STATUS_REASON = "EX",
                    STATUS_DATE = 20121106,
                    ISSUE_DATE = 20121106,
                    TAX_QUALIFICATION = "3"
                }
            };

        public static List<PPEND_NEW_BUSINESS_PENDING> GetPPENDNewBusinessPendingEntities =>
           new ()
           {
               new PPEND_NEW_BUSINESS_PENDING
               {
                   POLICY_NUMBER = "2150941812",
                   COMPANY_CODE = "01",
                   REDEF_TYPE = RedefTypes.Underwriting,
                   LAST_CHANGE_DATE = 20190520,
                   UND_NAME_ID = 2454411,
                   PEND_ID = 562595
               }
           };

        public static List<PPEND_NEW_BUS_PEND_UNDERWRITING> GetPPENDNewBusPendUnderwritingEntities =>
            new ()
            {
                new PPEND_NEW_BUS_PEND_UNDERWRITING
                {
                    PEND_ID = 562595,
                    IDX = 1,
                    UND_FLAG = "N",
                    UND_DATE = 20190520,
                    UND_O_DATE = 20220330,
                    UND_CODE = 4,
                    COMMENTS = "C J DOLAN   43TD"
                },
                new PPEND_NEW_BUS_PEND_UNDERWRITING
                {
                    PEND_ID = 562595,
                    IDX = 2,
                    UND_FLAG = "N",
                    UND_DATE = 20190520,
                    UND_O_DATE = 20220330,
                    UND_CODE = 2,
                    COMMENTS = string.Empty
                },
                new PPEND_NEW_BUS_PEND_UNDERWRITING
                {
                    PEND_ID = 562595,
                    IDX = 3,
                    UND_FLAG = "N",
                    UND_DATE = 20190520,
                    UND_O_DATE = 20220330,
                    UND_CODE = 1,
                    COMMENTS = string.Empty
                }
            };

        public static List<PCEXP_COVERAGE_EXPANSION> GetPCEXPCoverageExpansionEntities =>
            new ()
            {
                new PCEXP_COVERAGE_EXPANSION
                {
                    CEXP_ID = 1,
                    COVERAGE_ID = "RI07622030"
                },
                new PCEXP_COVERAGE_EXPANSION
                {
                    CEXP_ID = 2,
                    COVERAGE_ID = "042MRI"
                }
            };

        public static List<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS> GetPCEXPCoverageExpansionUWCLSDetailsEntities =>
            new ()
            {
                new PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
                {
                    CEXP_ID = 1,
                    IDX = 1,
                    UWCLS_CODE = "H",
                    UWCLS_DESC = "Preferred Non-Tobacco"
                },
                new PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
                {
                    CEXP_ID = 2,
                    IDX = 1,
                    UWCLS_CODE = "H",
                    UWCLS_DESC = "Non-Tobacco"
                }
            };

        public static List<PKDEF_KEY_DEFINITION> PKDEF_KEY_DEFINITIONs => new List<PKDEF_KEY_DEFINITION>
        {
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 0,
                KEY_NUM = 0,
                KDEF_DESC = string.Empty,
                KDEF_ID = 1
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 1,
                KEY_NUM = 0,
                KDEF_DESC = "Benefit Period  ",
                KDEF_ID = 2
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 1,
                KEY_NUM = 1,
                KDEF_DESC = "1 Years  ",
                KDEF_ID = 3
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 1,
                KEY_NUM = 2,
                KDEF_DESC = "30 Days  ",
                KDEF_ID = 4
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 2,
                KEY_NUM = 0,
                KDEF_DESC = "Elimination Period    ",
                KDEF_ID = 5
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 2,
                KEY_NUM = 1,
                KDEF_DESC = "Two Years    ",
                KDEF_ID = 6
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 3,
                KEY_NUM = 0,
                KDEF_DESC = "Occupation Class    ",
                KDEF_ID = 7
            },
            new PKDEF_KEY_DEFINITION
            {
                IDENT = "DI 100",
                DESC_NUM = 3,
                KEY_NUM = 1,
                KDEF_DESC = "5 Years  ",
                KDEF_ID = 8
            },
        };

        public static List<PCOVR_PRODUCT_COVERAGES> GetPCOVR_PRODUCT_COVERAGEEntities() =>
            new ()
            {
                new PCOVR_PRODUCT_COVERAGES
                {
                    COMPANY_CODE = "01",
                    COVR_ID = 1,
                    COVERAGE_ID = "030ADD",
                    DESCRIPTION = "Acc Death and Dism on 030"
                },
                new PCOVR_PRODUCT_COVERAGES
                {
                    COMPANY_CODE = "01",
                    COVR_ID = 2,
                    COVERAGE_ID = "030AME",
                    DESCRIPTION = "Accidental Medical Expense on 030"
                },
                new PCOVR_PRODUCT_COVERAGES
                {
                    COMPANY_CODE = "01",
                    COVR_ID = 3,
                    COVERAGE_ID = "030MRI",
                    DESCRIPTION = "Mo Readjustment Income Rider on 030"
                }
            };

        public static List<PPOLC> GetPPOLCEntities()
        {
            var ppolcEntities = new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198401",
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198402",
                    CONTRACT_CODE = "S",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "5150198403",
                    CONTRACT_CODE = "S",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                },
                new PPOLC
                {
                    COMPANY_CODE = "02",
                    POLICY_NUMBER = "5150198450",
                    CONTRACT_CODE = "P",
                    GROUP_NUMBER = string.Empty,
                    LINE_OF_BUSINESS = "U",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "8819851955",
                    CONTRACT_CODE = "T",
                    CONTRACT_DATE = 20130916,
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20150225,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "8819852015",
                    CONTRACT_CODE = "T",
                    CONTRACT_DATE = int.Parse(DateTimeUtility.DateTimeNowCentral().AddYears(-1).ToString("yyyyMMdd")),
                    GROUP_NUMBER = "  ",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "G00604123456",
                    CONTRACT_CODE = "A",
                    CONTRACT_DATE = 19800101,
                    GROUP_NUMBER = "  ",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "8887776666",
                    CONTRACT_CODE = "T",
                    CONTRACT_DATE = DateTime.Now.AddYears(-1).ToLifeProDate(),
                    GROUP_NUMBER = "L337GROUP",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "6666888777",
                    CONTRACT_CODE = "T",
                    CONTRACT_DATE = DateTime.Now.AddYears(-4).ToLifeProDate(),
                    GROUP_NUMBER = "H4X0RGROUP",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = "77776666888",
                    CONTRACT_CODE = "T",
                    CONTRACT_DATE = DateTime.Now.AddYears(-4).ToLifeProDate(),
                    GROUP_NUMBER = "   ",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101
                }
            };

            // Provide all PPOLC entities with values for other fields that aren't
            // necessarily relevant to any tests but are required nonetheless.
            foreach (var ppolc in ppolcEntities)
            {
                ppolc.CONTRACT_REASON = string.Empty;
                ppolc.ISSUE_STATE = "NE";
                ppolc.PRODUCT_CODE = "DI 100";
                ppolc.RES_STATE = "NE";
                ppolc.POLC_SPECIAL_MODE = " ";
                ppolc.BILLING_CODE = "A";
                ppolc.PAYMENT_REASON = "NM";
                ppolc.TAX_QUALIFY_CODE = "3";
            }

            return ppolcEntities;
        }

        public static List<PRELA_RELATIONSHIP_MASTER> GetPRELAWithNavigationProperties()
        {
            return new List<PRELA_RELATIONSHIP_MASTER>
            {
                new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = $"{PRELANavigationPropertiesCompanyCode}{PRELANavigationPropertiesPolicyNumber}",
                    NAME_ID = 500001,
                    PNAME = new PNAME
                    {
                        NAME_ID = 500001,
                        NAME_BUSINESS = "ABC Company",
                        BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                        INDIVIDUAL_FIRST = "First",
                        INDIVIDUAL_LAST = "Last",
                        INDIVIDUAL_MIDDLE = "M",
                        INDIVIDUAL_PREFIX = "Mr",
                        INDIVIDUAL_SUFFIX = "ESQ",
                        DATE_OF_BIRTH = 19860202,
                        NAME_FORMAT_CODE = "I",
                        SEX_CODE = "F",
                        PERSONAL_EMAIL_ADR = "email@email.com",
                        PNALKs = new List<PNALK>
                        {
                            new PNALK
                            {
                                ADDRESS_CODE = "M",
                                ADDRESS_ID = 1,
                                TELE_NUM = "4021238888"
                            },
                            new PNALK
                            {
                                ADDRESS_CODE = string.Empty,
                                ADDRESS_ID = 2,
                                CANCEL_DATE = 20240101,
                                TELE_NUM = "4021239999"
                            },
                            new PNALK
                            {
                                ADDRESS_CODE = string.Empty,
                                ADDRESS_ID = 3,
                                CANCEL_DATE = 0,
                                NAME_ID = 500001,
                                TELE_NUM = "4021235678",
                                PADDR = new PADDR
                                {
                                    ADDRESS_ID = 3,
                                    ADDR_LINE_1 = "Line1",
                                    ADDR_LINE_2 = "Line2",
                                    ADDR_LINE_3 = "Line3",
                                    BOX_NUMBER = "Box",
                                    CITY = "City",
                                    COUNTRY = "USA",
                                    STATE = "NE",
                                    ZIP = "68506",
                                    ZIP_EXTENSION = "1234"
                                }
                            }
                        }
                    },
                    RELATE_CODE = "IN",
                    BENEFIT_SEQ_NUMBER = 0
                }
            };
        }

        public static List<PPOLC> GetPPOLCWithNavigationProperties()
        {
            return new List<PPOLC>
            {
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = PPOLCWithAllNavigationPropertiesPolicyNumber,
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457234579",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                    PPEND_NEW_BUSINESS_PENDING = GetDefaultPPEND_NEW_BUSINESS_PENDING(),
                    PGRUP_GROUP_MASTER = new PGRUP_GROUP_MASTER
                    {
                        GROUP_NUMBER = "7457234579",
                        COMPANY_CODE = "01",
                        STATUS_CODE = 'A',
                        PNAME = new PNAME
                        {
                            NAME_ID = 822007,
                            NAME_BUSINESS = "ABC Company",
                            BUSINESS_EMAIL_ADR = "abc_company@gmail.com",
                            INDIVIDUAL_FIRST = string.Empty,
                            INDIVIDUAL_LAST = string.Empty,
                            INDIVIDUAL_MIDDLE = string.Empty,
                            INDIVIDUAL_PREFIX = string.Empty,
                            INDIVIDUAL_SUFFIX = string.Empty,
                            DATE_OF_BIRTH = 0,
                            NAME_FORMAT_CODE = string.Empty,
                            SEX_CODE = string.Empty,
                            PERSONAL_EMAIL_ADR = string.Empty
                        }
                    },
                    PACON_ANNUITY_POLICY = new PACON_ANNUITY_POLICY
                    {
                        COMPANY_CODE = "01",
                        POLICY_NUMBER = PPOLCWithAllNavigationPropertiesPolicyNumber,
                        STATUS_CODE = "T",
                        STATUS_REASON = "DC",
                        STATUS_DATE = 20151202,
                        ISSUE_DATE = 20011201,
                        TAX_QUALIFICATION = "2"
                    }
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = PPOLCWithGroupNull,
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = string.Empty,
                    LINE_OF_BUSINESS = "L",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20210322,
                    APPLICATION_DATE = 20220101,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "OH",
                    PRODUCT_CODE = "DI 200",
                    RES_STATE = "MI",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                    PGRUP_GROUP_MASTER = null,
                    PPEND_NEW_BUSINESS_PENDING = GetDefaultPPEND_NEW_BUSINESS_PENDING(),
                    PACON_ANNUITY_POLICY = new PACON_ANNUITY_POLICY
                    {
                        COMPANY_CODE = "01",
                        POLICY_NUMBER = "123456789",
                        STATUS_CODE = "A",
                        STATUS_REASON = "DC",
                        STATUS_DATE = 20161202,
                        ISSUE_DATE = 20031201,
                        TAX_QUALIFICATION = "3"
                    }
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = PPOLCWithAnnuityNull,
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457299999",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                    PPEND_NEW_BUSINESS_PENDING = GetDefaultPPEND_NEW_BUSINESS_PENDING(),
                    PGRUP_GROUP_MASTER = new PGRUP_GROUP_MASTER
                    {
                        GROUP_NUMBER = "7457299999",
                        COMPANY_CODE = "01",
                        STATUS_CODE = 'A',
                        PNAME = new PNAME
                        {
                            NAME_ID = 822997,
                            NAME_BUSINESS = "XYZ Company",
                            BUSINESS_EMAIL_ADR = "xyzCompany@gmail.com",
                            INDIVIDUAL_FIRST = string.Empty,
                            INDIVIDUAL_LAST = string.Empty,
                            INDIVIDUAL_MIDDLE = string.Empty,
                            INDIVIDUAL_PREFIX = string.Empty,
                            INDIVIDUAL_SUFFIX = string.Empty,
                            DATE_OF_BIRTH = 0,
                            NAME_FORMAT_CODE = string.Empty,
                            SEX_CODE = string.Empty,
                            PERSONAL_EMAIL_ADR = string.Empty
                        }
                    }
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = PPOLCWithNewBusinessPendingNull,
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = "7457999999",
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                    PACON_ANNUITY_POLICY = new PACON_ANNUITY_POLICY
                    {
                        COMPANY_CODE = "01",
                        POLICY_NUMBER = "PPOLCWithNewBusinessPendingNull",
                        STATUS_CODE = "A",
                        STATUS_REASON = "DC",
                        STATUS_DATE = 20161202,
                        ISSUE_DATE = 20031201,
                        TAX_QUALIFICATION = "3"
                    },
                    PGRUP_GROUP_MASTER = new PGRUP_GROUP_MASTER
                    {
                        GROUP_NUMBER = "7457999999",
                        COMPANY_CODE = "01",
                        STATUS_CODE = 'A',
                        PNAME = new PNAME
                        {
                            NAME_ID = 829997,
                            NAME_BUSINESS = "XYZ Company",
                            BUSINESS_EMAIL_ADR = "xyzCompany@gmail.com",
                            INDIVIDUAL_FIRST = string.Empty,
                            INDIVIDUAL_LAST = string.Empty,
                            INDIVIDUAL_MIDDLE = string.Empty,
                            INDIVIDUAL_PREFIX = string.Empty,
                            INDIVIDUAL_SUFFIX = string.Empty,
                            DATE_OF_BIRTH = 0,
                            NAME_FORMAT_CODE = string.Empty,
                            SEX_CODE = string.Empty,
                            PERSONAL_EMAIL_ADR = string.Empty
                        }
                    }
                },
                new PPOLC
                {
                    COMPANY_CODE = "01",
                    POLICY_NUMBER = PPOLCWithNavigationPropertiesNull,
                    CONTRACT_CODE = "A",
                    GROUP_NUMBER = string.Empty,
                    LINE_OF_BUSINESS = "H",
                    BILLING_FORM = "CRD",
                    BILLING_REASON = "ST",
                    PAID_TO_DATE = 20230322,
                    APPLICATION_DATE = 20230101,
                    CONTRACT_REASON = string.Empty,
                    ISSUE_STATE = "NE",
                    PRODUCT_CODE = "DI 100",
                    RES_STATE = "NE",
                    POLC_SPECIAL_MODE = " ",
                    BILLING_CODE = "A",
                    PAYMENT_REASON = "NM",
                    TAX_QUALIFY_CODE = "3",
                }
            };
        }

        public static List<PPBEN_POLICY_BENEFITS> GetPPBEN_WithNavigationProperties()
        {
            return new List<PPBEN_POLICY_BENEFITS>
            {
                new PPBEN_POLICY_BENEFITS
                {
                    PBEN_ID = 54321,
                    POLICY_NUMBER = PPBENNavigationPropertiesPolicyNumber,
                    COMPANY_CODE = "01",
                    BENEFIT_SEQ = 2,
                    BENEFIT_TYPE = BenefitTypes.OtherRider,
                    PLAN_CODE = "ADIR-E",
                    STATUS_CODE = "A",
                    STATUS_REASON = string.Empty,
                    STATUS_DATE = 0,
                    PMUIN_MULTIPLE_INSUREDs = new List<PMUIN_MULTIPLE_INSUREDS>
                    {
                        new PMUIN_MULTIPLE_INSUREDS
                        {
                            COMPANY_CODE = "01",
                            POLICY_NUMBER = "PPBENNavigationPropertiesPolicyNumber",
                            NAME_ID = 880001,
                            RELATIONSHIP_CODE = "JE",
                            BENEFIT_SEQ = 2,
                            MULT_RELATE = "SELF",
                            KD_BEN_EXTEND_KEYS = "0102                                              ",
                            KD_DEF_SEGT_ID = "W A200",
                            UWCLS = "E",
                            STOP_DATE = 99999999,
                            START_DATE = 20180101,
                        }
                    },
                    PPBEN_POLICY_BENEFITS_TYPES_BF = new PPBEN_POLICY_BENEFITS_TYPES_BF
                    {
                        PBEN_ID = 54321,
                        ANN_PREM_PER_UNIT = 100M,
                        NUMBER_OF_UNITS = 9M,
                        BF_DB_OPTION = "1",
                        VALUE_PER_UNIT = 1000.00M,
                        BF_DATE_NEGATIVE = 20180119,
                        BF_CURRENT_DB = 9990.00M,
                    },
                    PPBEN_POLICY_BENEFITS_TYPES_BA_OR = new PPBEN_POLICY_BENEFITS_TYPES_BA_OR
                    {
                        PBEN_ID = 54321,
                        ANN_PREM_PER_UNIT = 200M,
                        NUMBER_OF_UNITS = 1M,
                        DIVIDEND = "300",
                        VALUE_PER_UNIT = 2001.00M,
                    },
                    PPBEN_POLICY_BENEFITS_TYPES_SL = new PPBEN_POLICY_BENEFITS_TYPES_SL
                    {
                        PBEN_ID = 54321,
                        ANN_PREM_PER_UNIT = 300M,
                        NUMBER_OF_UNITS = 3M,
                        VALUE_PER_UNIT = 3001.00M,
                    },
                    PPBEN_POLICY_BENEFITS_TYPES_SP = new PPBEN_POLICY_BENEFITS_TYPES_SP
                    {
                        PBEN_ID = 54321,
                        ANN_PREM_PER_UNIT = 400M,
                        NUMBER_OF_UNITS = 4M,
                        VALUE_PER_UNIT = 4001.00M,
                    },
                    PPBEN_POLICY_BENEFITS_TYPES_SU = new PPBEN_POLICY_BENEFITS_TYPES_SU
                    {
                        PBEN_ID = 54321,
                        ANN_PREM_PER_UNIT = 500M,
                        NUMBER_OF_UNITS = 5M,
                        VALUE_PER_UNIT = 5001.00M,
                    },
                    PCEXP_COVERAGE_EXPANSION = new PCEXP_COVERAGE_EXPANSION
                    {
                        CEXP_ID = 1,
                        COVERAGE_ID = "RI07622030",
                        PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS = new List<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS>
                        {
                            new PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
                            {
                                CEXP_ID = 1,
                                IDX = 1,
                                UWCLS_CODE = "H",
                                UWCLS_DESC = "Preferred Non-Tobacco"
                            },
                            new PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
                            {
                                CEXP_ID = 1,
                                IDX = 2,
                                UWCLS_CODE = "H",
                                UWCLS_DESC = "Non-Tobacco"
                            }
                        }
                    },
                    PCOVR_PRODUCT_COVERAGES = new PCOVR_PRODUCT_COVERAGES
                    {
                        COMPANY_CODE = "01",
                        COVR_ID = 1,
                        COVERAGE_ID = "ADIR-E",
                        DESCRIPTION = "Acc Death and Dism on 030"
                    }
                }
            };
        }

        public static List<PCOMC_COMMISSION_CONTROL> GetPCOMC_COMMISSION_CONTROLsWithNavigationProperties() => new List<PCOMC_COMMISSION_CONTROL>
        {
            new PCOMC_COMMISSION_CONTROL
            {
                COMC_ID = 2341,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334456",
                RECORD_TYPE = "S",
                PCOMC_COMMISSION_CONTROL_TYPE_S = new PCOMC_COMMISSION_CONTROL_TYPE_S
                {
                    AGENT = "1112",
                    AGENT_LEVEL = "90",
                    COMC_ID = 2341,
                    MARKET_CODE = "IS",
                    SERVICE_AGENT_IND = string.Empty,
                    COMM_PCNT = 0,
                    PAGNT_AGENT_MASTERs = new List<PAGNT_AGENT_MASTER>
                    {
                        new PAGNT_AGENT_MASTER
                        {
                            AGNT_ID = 2,
                            COMPANY_CODE = "01",
                            AGENT_NUMBER = "1112",
                            NAME_ID = 885697,
                            PNAME = new PNAME
                            {
                                NAME_ID = 885697,
                                INDIVIDUAL_FIRST = "First",
                                INDIVIDUAL_LAST = "Last",
                                INDIVIDUAL_MIDDLE = string.Empty,
                                INDIVIDUAL_PREFIX = string.Empty,
                                INDIVIDUAL_SUFFIX = string.Empty,
                                NAME_FORMAT_CODE = "I",
                                BUSINESS_EMAIL_ADR = string.Empty,
                                NAME_BUSINESS = string.Empty,
                                PERSONAL_EMAIL_ADR = "email@email.com",
                                SEX_CODE = "F",
                                DATE_OF_BIRTH = 19860202,
                            }
                        }
                    }
                }
            },
            new PCOMC_COMMISSION_CONTROL
            {
                COMC_ID = 2342,
                COMPANY_CODE = "01",
                POLICY_NUMBER = "1122334457",
                RECORD_TYPE = "S",
                PCOMC_COMMISSION_CONTROL_TYPE_S = new PCOMC_COMMISSION_CONTROL_TYPE_S
                {
                    AGENT = "1114",
                    AGENT_LEVEL = "90",
                    COMC_ID = 2342,
                    MARKET_CODE = "IS",
                    SERVICE_AGENT_IND = "X",
                    COMM_PCNT = 0,
                    PAGNT_AGENT_MASTERs = new List<PAGNT_AGENT_MASTER>
                    {
                        new PAGNT_AGENT_MASTER
                        {
                            AGNT_ID = 3,
                            COMPANY_CODE = "01",
                            AGENT_NUMBER = "1114",
                            NAME_ID = 885698,
                            PNAME = new PNAME
                            {
                                NAME_ID = 885698,
                                NAME_BUSINESS = "ABC Company",
                                BUSINESS_EMAIL_ADR = "biz@biz.com",
                                NAME_FORMAT_CODE = "B",
                                INDIVIDUAL_FIRST = string.Empty,
                                INDIVIDUAL_LAST = string.Empty,
                                INDIVIDUAL_MIDDLE = string.Empty,
                                INDIVIDUAL_PREFIX = string.Empty,
                                INDIVIDUAL_SUFFIX = string.Empty,
                                PERSONAL_EMAIL_ADR = string.Empty,
                                SEX_CODE = "M"
                            }
                        }
                    }
                }
            }
        };

        private static PPEND_NEW_BUSINESS_PENDING GetDefaultPPEND_NEW_BUSINESS_PENDING() => new PPEND_NEW_BUSINESS_PENDING
        {
            REQUIREMENT_DATE = 20230101,
            COMPANY_CODE = "01",
            REDEF_TYPE = RedefTypes.Underwriting,
        };
    }
}