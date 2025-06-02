namespace Assurity.Kafka.Accessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.EntityFrameworkCore;
    using NewRelic.Api.Agent;

    /// <summary>
    /// The accessor to the Life Pro Real Time Data we need to build a policy object.
    /// </summary>
    public abstract class BaseAccessor
    {
        [Trace]
        public PolicyDTO GetPolicyDTO(DbContext context, string policyNumber, string companyCode)
        {
            const string error = "ER";
            const string terminated = "T";
            var threeYearsAgo = DateTime.Now.AddYears(-3).ToLifeProDate();

            var record =
                context.Set<PPOLC>()
                .Include(ppolc => ppolc.PACON_ANNUITY_POLICY)
                .Include(ppolc => ppolc.PGRUP_GROUP_MASTER)
                    .ThenInclude(pgrup => pgrup.PNAME)
                .Include(ppolc => ppolc.PPEND_NEW_BUSINESS_PENDING)
                .Where(ppolc =>
                    ppolc.POLICY_NUMBER == policyNumber
                    && ppolc.COMPANY_CODE == companyCode
                    && !ppolc.POLICY_NUMBER.StartsWith("G")
                    && ppolc.CONTRACT_REASON != error
                    && !(ppolc.CONTRACT_CODE == terminated
                        && ppolc.CONTRACT_DATE <= threeYearsAgo
                        && string.IsNullOrWhiteSpace(ppolc.GROUP_NUMBER))
                    && ppolc.APPLICATION_DATE.ToString().Length == 8)
                .AsNoTracking()
                .FirstOrDefault();

            if (record == null)
            {
                return null;
            }

            PolicyInfoExtensions.TrimStringProperties(record);

            var policyDto = new PolicyDTO
            {
                AnnualPremium = record.ANNUAL_PREMIUM,
                AppReceivedDate = record.APP_RECEIVED_DATE,
                ApplicationDate = record.APPLICATION_DATE,
                BillingCode = record.BILLING_CODE,
                BillingDate = record.BILLING_DATE,
                BillingForm = record.BILLING_FORM,
                BillingMode = record.BILLING_MODE,
                BillingReason = record.BILLING_REASON,
                CompanyCode = record.COMPANY_CODE,
                ContractCode = record.CONTRACT_CODE,
                ContractDate = record.CONTRACT_DATE,
                ContractReason = record.CONTRACT_REASON,
                GroupNumber = record.GROUP_NUMBER,
                IssueDate = record.ISSUE_DATE,
                IssueState = record.ISSUE_STATE,
                LineOfBusiness = record.LINE_OF_BUSINESS,
                ModePremium = record.MODE_PREMIUM,
                PaidToDate = record.PAID_TO_DATE,
                PolcSpecialMode = record.POLC_SPECIAL_MODE,
                PolicyBillDay = record.POLICY_BILL_DAY,
                PolicyNumber = record.POLICY_NUMBER,
                ProductCode = record.PRODUCT_CODE,
                ResidenceState = record.RES_STATE,
                TaxQualifyCode = record.TAX_QUALIFY_CODE
            };

            if (record.PACON_ANNUITY_POLICY != null)
            {
                policyDto.AnnuityPolicy = new AnnuityPolicyDTO
                {
                    CompanyCode = record.PACON_ANNUITY_POLICY.COMPANY_CODE,
                    IssueDate = record.PACON_ANNUITY_POLICY.ISSUE_DATE,
                    PolicyNumber = record.PACON_ANNUITY_POLICY.POLICY_NUMBER,
                    StatusCode = record.PACON_ANNUITY_POLICY.STATUS_CODE,
                    StatusReason = record.PACON_ANNUITY_POLICY.STATUS_REASON,
                    StatusDate = record.PACON_ANNUITY_POLICY.STATUS_DATE,
                    TaxQualification = record.PACON_ANNUITY_POLICY.TAX_QUALIFICATION
                };
            }

            if (record.PPEND_NEW_BUSINESS_PENDING != null)
            {
                policyDto.NewBusinessPending = new NewBusinessPendingDTO
                {
                    RequirementDate = record.PPEND_NEW_BUSINESS_PENDING.REQUIREMENT_DATE
                };
            }

            if (record.PGRUP_GROUP_MASTER != null)
            {
                policyDto.Employer = new EmployerDTO
                {
                    BusinessEmailAddress = record.PGRUP_GROUP_MASTER.PNAME?.BUSINESS_EMAIL_ADR,
                    EmployerName = record.PGRUP_GROUP_MASTER.PNAME?.NAME_BUSINESS,
                    GroupNumber = record.GROUP_NUMBER,
                    NameId = record.PGRUP_GROUP_MASTER.NAME_ID,
                    StatusCode = record.PGRUP_GROUP_MASTER.STATUS_CODE
                };
            }

            return policyDto;
        }

        [Trace]
        public async Task<PPOLC> GetEligiblePPOLC(DbContext context, string policyNumber, string companyCode)
        {
            var record = new PPOLC();
            const string error = "ER";
            const string terminated = "T";
            var threeYearsAgo = DateTime.Now.AddYears(-3).ToLifeProDate();

            record = await context.Set<PPOLC>()
                .Where(ppolc => ppolc.POLICY_NUMBER == policyNumber
                && ppolc.COMPANY_CODE == companyCode
                && !ppolc.POLICY_NUMBER.StartsWith("G")
                && ppolc.CONTRACT_REASON != error
                && !(ppolc.CONTRACT_CODE == terminated
                        && ppolc.CONTRACT_DATE <= threeYearsAgo
                        && string.IsNullOrWhiteSpace(ppolc.GROUP_NUMBER))
                && ppolc.APPLICATION_DATE.ToString().Length == 8)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<ProductDescription> GetBaseProductDescriptionByProductCode(DbContext context, string productCode)
        {
            var record = await context.Set<ProductDescription>()
                .Where(x => x.ProdNumber == productCode && x.BaseRider == "BASE")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<ProductDescription> GetBaseProductDescriptionByPlanCode(DbContext context, string planCode)
        {
            var record = await context.Set<ProductDescription>()
                .Where(x => x.CovID == planCode && x.BaseRider == "BASE")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PPEND_NEW_BUSINESS_PENDING> GetPPEND_NEW_BUSINESS_PENDING(DbContext context, string policyNumber)
        {
            var record = new PPEND_NEW_BUSINESS_PENDING();

            record = await context.Set<PPEND_NEW_BUSINESS_PENDING>()
                .Where(x => x.POLICY_NUMBER == policyNumber)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER(DbContext context, string identifyingAlpha, List<string> relateCodeList)
        {
            var records = new List<PRELA_RELATIONSHIP_MASTER>();

            records = await context.Set<PRELA_RELATIONSHIP_MASTER>()
                .Where(x => x.IDENTIFYING_ALPHA == identifyingAlpha && relateCodeList.Contains(x.RELATE_CODE))
                .AsNoTracking()
                .Select(prela => new PRELA_RELATIONSHIP_MASTER
                {
                    IDENTIFYING_ALPHA = identifyingAlpha,
                    NAME_ID = prela.NAME_ID,
                    RELATE_CODE = prela.RELATE_CODE,
                    BENEFIT_SEQ_NUMBER = prela.BENEFIT_SEQ_NUMBER
                })
                .Distinct()
                .ToListAsync();

            records.TrimStringProperties();
            return records;
        }

        [Trace]
        public async Task<List<PAGNT_AGENT_MASTER>> GetPAGNT_AGENT_MASTER(DbContext context, string companyCode, string agentNumber)
        {
            var record = new List<PAGNT_AGENT_MASTER>();
            record = await context.Set<PAGNT_AGENT_MASTER>()
                .Where(x => x.COMPANY_CODE == companyCode && (x.AGENT_NUMBER == agentNumber))
                .AsNoTracking()
            .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(DbContext context, string identifyingAlpha)
        {
            var record = new List<PRELA_RELATIONSHIP_MASTER>();

            var insuredRelateCodes = new string[] { "IN", "ML", "JE" };

            record = await context.Set<PRELA_RELATIONSHIP_MASTER>()
                .Where(x => x.IDENTIFYING_ALPHA == identifyingAlpha && x.BENEFIT_SEQ_NUMBER > 0 && insuredRelateCodes.Contains(x.RELATE_CODE))
                .AsNoTracking()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PRELA_RELATIONSHIP_MASTER?> GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
            DbContext context,
            string policyNumber,
            int nameId,
            string relateCode,
            short benefitSeqNumber)
        {
            var prelaQuery = context.Set<PPOLC>()
                .Join(
                    context.Set<PRELA_RELATIONSHIP_MASTER>(),
                    ppolc => ppolc.COMPANY_CODE + ppolc.POLICY_NUMBER,
                    prela => prela.IDENTIFYING_ALPHA,
                    (ppolc, prela) => new { PolicyNumber = ppolc.POLICY_NUMBER, prela })
                .Join(
                    context.Set<PPBEN_POLICY_BENEFITS>(),
                    policyNumberAndPrela => new
                    {
                        POLICY_NUMBER = policyNumberAndPrela.PolicyNumber,
                        BENEFIT_SEQ = policyNumberAndPrela.prela.BENEFIT_SEQ_NUMBER
                    },
                    ppben => new
                    {
                        ppben.POLICY_NUMBER,
                        ppben.BENEFIT_SEQ
                    },
                    (policyNumberAndPrela, ppben) => new { policyNumberAndPrela.PolicyNumber, policyNumberAndPrela.prela, ppben });

            prelaQuery = prelaQuery
                .Where(o => o.PolicyNumber == policyNumber
                    && o.prela.NAME_ID == nameId
                    && o.prela.RELATE_CODE == relateCode
                    && o.prela.BENEFIT_SEQ_NUMBER == benefitSeqNumber
                    && (o.ppben.BENEFIT_TYPE == BenefitTypes.Base
                     || o.ppben.BENEFIT_TYPE == BenefitTypes.OtherRider
                     || o.ppben.BENEFIT_TYPE == BenefitTypes.BaseForUniversalLife))
                .AsNoTracking();

            var record = await prelaQuery
                .Select(o => o.prela)
                .Distinct()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PNAME> GetPNAME(DbContext context, int nameId)
        {
            var record = new PNAME();
            record = await context.Set<PNAME>()
                .Where(x => x.NAME_ID == nameId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<List<PNALK>> GetPNALK(DbContext context, int nameId)
        {
            var record = await context.Set<PNALK>()
                .Where(x => x.NAME_ID == nameId)
                .AsNoTracking()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PADDR> GetPADDR(DbContext context, int addressId)
        {
            var record = new PADDR();
            record = await context.Set<PADDR>()
                .Where(x => x.ADDRESS_ID == addressId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        public List<BenefitDTO> GetBenefitDTOs(
            DbContext context,
            string policyNumber,
            string companyCode)
        {
            var baseQuery = GetBenefitsBaseQuery(context, policyNumber, companyCode);
            var records = baseQuery.ToList();

            PolicyInfoExtensions.TrimStringProperties(records);

            return records.Select(MapToBenefitDTO).ToList();
        }

        public BenefitDTO GetBenefitDTO(
            DbContext context,
            string policyNumber,
            string companyCode,
            long pbenId)
        {
            var baseQuery = GetBenefitsBaseQuery(context, policyNumber, companyCode);
            baseQuery = baseQuery.Where(pben => pben.PBEN_ID == pbenId);
            var record = baseQuery.FirstOrDefault();

            return MapToBenefitDTO(record);
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS?> GetPPBEN_POLICY_BENEFITS(
            DbContext context,
            string policyNumber,
            short benefitSeq,
            long benefitId,
            string companyCode)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS>()
                .Where(ppben => ppben.POLICY_NUMBER == policyNumber
                    && ppben.BENEFIT_SEQ == benefitSeq
                    && ppben.PBEN_ID == benefitId
                    && ppben.COMPANY_CODE == companyCode)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS?> GetPPBEN_POLICY_BENEFITS(
            DbContext context,
            string policyNumber,
            short benefitSeq,
            string companyCode)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS>()
                .Where(ppben => ppben.POLICY_NUMBER == policyNumber
                    && (ppben.COMPANY_CODE == companyCode)
                    && ppben.BENEFIT_SEQ == benefitSeq)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<List<PPBEN_POLICY_BENEFITS>> GetPPBEN_POLICY_BENEFITS(
            DbContext context,
            string policyNumber,
            string companyCode)
        {
            var records = await context.Set<PPBEN_POLICY_BENEFITS>()
                .Where(ppben => ppben.POLICY_NUMBER == policyNumber
                   && (ppben.COMPANY_CODE == companyCode)
                   && (ppben.BENEFIT_TYPE == BenefitTypes.Base
                    || ppben.BENEFIT_TYPE == BenefitTypes.OtherRider
                    || ppben.BENEFIT_TYPE == BenefitTypes.BaseForUniversalLife
                    || ppben.BENEFIT_TYPE == BenefitTypes.Supplemental
                    || ppben.BENEFIT_TYPE == BenefitTypes.SpecifiedAmountIncrease
                    || ppben.BENEFIT_TYPE == BenefitTypes.TableRating))
                .AsNoTracking()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_BA_OR?> GetPPBEN_POLICY_BENEFIT_TYPES_BA_OR(
            DbContext context,
            long pbenID)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS_TYPES_BA_OR>()
                .Where(x => x.PBEN_ID == pbenID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_BF?> GetPPBEN_POLICY_BENEFIT_TYPES_BF(
            DbContext context,
            long pbenID)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS_TYPES_BF>()
                .Where(x => x.PBEN_ID == pbenID)
                .AsNoTracking()
            .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SU?> GetPPBEN_POLICY_BENEFIT_TYPES_SU(
            DbContext context,
            long pbenID)
        {
            return await context.Set<PPBEN_POLICY_BENEFITS_TYPES_SU>()
                .Where(x => x.PBEN_ID == pbenID)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SL?> GetPPBEN_POLICY_BENEFIT_TYPES_SL(
            DbContext context,
            long pbenID)
        {
            return await context.Set<PPBEN_POLICY_BENEFITS_TYPES_SL>()
                .Where(x => x.PBEN_ID == pbenID)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SP?> GetPPBEN_POLICY_BENEFIT_TYPES_SP(
            DbContext context,
            long pbenID)
        {
            return await context.Set<PPBEN_POLICY_BENEFITS_TYPES_SP>()
                .Where(x => x.PBEN_ID == pbenID)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        [Trace]
        public async Task<List<PMUIN_MULTIPLE_INSUREDS>> GetPMUIN_MULTIPLE_INSUREDS(
            DbContext context,
            string policyNumber,
            int benefitSeq,
            int nameId,
            string relateCode,
            string companyCode)
        {
            var record = await context.Set<PMUIN_MULTIPLE_INSUREDS>()
                .Where(pmuin => pmuin.POLICY_NUMBER == policyNumber
                    && pmuin.COMPANY_CODE == companyCode
                    && pmuin.BENEFIT_SEQ == benefitSeq
                    && pmuin.NAME_ID == nameId
                    && pmuin.RELATIONSHIP_CODE == relateCode)
                .AsNoTracking()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public List<ExtendedKeyLookupResult> GetExtendedKeyData(DbContext context, ExtendedKeysLookup lookups)
        {
            return
                lookups
                .Lookups
                .Join(
                    context.Set<PKDEF_KEY_DEFINITION>(),
                    lookup => lookup.Identifier.Trim(),
                    keyDef => keyDef.IDENT.Trim(),
                    (lookup, keyDef) => new { lookup, keyDef })
                .Where(join =>
                    join.keyDef.DESC_NUM >= 0
                    && (join.keyDef.DESC_NUM <= join.lookup.MaxKeyValue
                    || join.keyDef.KEY_NUM <= join.lookup.MaxOrdinal))
                .GroupBy(result => result.keyDef.IDENT.Trim())
                .Select(grouping => new ExtendedKeyLookupResult
                {
                    Identifier = grouping.Key,
                    Lookups =
                        grouping
                        .Select(keyDef => new KeyLookupResult
                        {
                            BenefitOrdinal = keyDef.keyDef.KEY_NUM,
                            Key = keyDef.keyDef.DESC_NUM,
                            Value = keyDef.keyDef.KDEF_DESC.Trim()
                        })
                        .ToList()
                })
                .ToList();
        }

        [Trace]
        public async Task<ExtendedKeyQueryEntity?> GetExtendedKey(DbContext context, string keyIdent, short iter, short ky)
        {
            var record = await context.Set<PKDEF_KEY_DEFINITION>()
                .Join(
                    context.Set<PKDEF_KEY_DEFINITION>(),
                    key0 => new { Ident = key0.IDENT, Num = key0.DESC_NUM },
                    key1 => new { Ident = key1.IDENT, Num = key1.KEY_NUM },
                    (key0, key1) => new ExtendedKeyQueryEntity
                    {
                        KEY_IDENTIFIER = key0.IDENT,
                        KEY_CATEGORY = key0.KDEF_DESC,
                        KEY_CATEGORY_VALUE = key0.DESC_NUM,
                        KEY_OPTION = key1.KDEF_DESC,
                        KEY_OPTION_VALUE = key1.DESC_NUM,
                        KEY0_NUM = key0.KEY_NUM,
                    })
                .Where(x => x.KEY_IDENTIFIER == keyIdent && x.KEY_CATEGORY_VALUE == iter && x.KEY_OPTION_VALUE == ky && x.KEY0_NUM == 0 && x.KEY_CATEGORY_VALUE > 0)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<string?> GetUnderwritingClassDescription(DbContext context, string planCode, string underwritingClass)
        {
            return await context.Set<PCEXP_COVERAGE_EXPANSION>()
                .Join(
                    context.Set<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS>(),
                    pcexp => pcexp.CEXP_ID,
                    pcexpd => pcexpd.CEXP_ID,
                    (pcexp, pcexpd) => new { pcexp, pcexpd })
                .Where(joinedPcexp => joinedPcexp.pcexp.COVERAGE_ID == planCode
                    && joinedPcexp.pcexpd.UWCLS_CODE == underwritingClass)
                .Select(joinedPcexp => joinedPcexp.pcexpd.UWCLS_DESC.Trim())
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        [Trace]
        public List<PolicyAgentDTO> GetPolicyAgentDTOs(
            DbContext context,
            string companyCode,
            string policyNumber)
        {
            var records =
                context.Set<PCOMC_COMMISSION_CONTROL>()
                    .Include(pcomc => pcomc.PCOMC_COMMISSION_CONTROL_TYPE_S)
                    .ThenInclude(pcomcs => pcomcs.PAGNT_AGENT_MASTERs)
                    .ThenInclude(agent => agent.PNAME)
                .Where(pcomc =>
                    pcomc.RECORD_TYPE == "S"
                    && pcomc.POLICY_NUMBER == policyNumber
                    && pcomc.COMPANY_CODE == companyCode)
                .AsNoTracking()
                .Distinct()
                .ToList();

            var mappedAgents = new List<PolicyAgentDTO>();
            foreach (var record in records)
            {
                foreach (var agent in record.PCOMC_COMMISSION_CONTROL_TYPE_S?.PAGNT_AGENT_MASTERs ?? new List<PAGNT_AGENT_MASTER>())
                {
                    var agentDto = new PolicyAgentDTO
                    {
                        AgentNumber = agent.AGENT_NUMBER.Trim(),
                        ServiceAgentIndicator = record.PCOMC_COMMISSION_CONTROL_TYPE_S.SERVICE_AGENT_IND.Trim(),
                        MarketCode = record.PCOMC_COMMISSION_CONTROL_TYPE_S.MARKET_CODE.Trim(),
                        Level = record.PCOMC_COMMISSION_CONTROL_TYPE_S.AGENT_LEVEL.Trim(),
                        CommissionPercent = record.PCOMC_COMMISSION_CONTROL_TYPE_S.COMM_PCNT,
                        Name = MapPNAMEToNameDto(agent.PNAME)
                    };

                    mappedAgents.Add(agentDto);
                }
            }

            return
                mappedAgents
                .Distinct()
                .OrderBy(a => a.AgentNumber)
                .ToList();
        }

        [Trace]
        public async Task<List<AgentDTO>> GetAgents(
            DbContext context,
            string companyCode,
            string policyNumber)
        {
            var records = await context.Set<PCOMC_COMMISSION_CONTROL>()
                .Join(
                    context.Set<PCOMC_COMMISSION_CONTROL_TYPE_S>(),
                    pcomc => pcomc.COMC_ID,
                    pcomc_s => pcomc_s.COMC_ID,
                    (pcomc, pcomc_s) => new { pcomc, pcomc_s })
                .Where(joinedPcomcs => joinedPcomcs.pcomc.RECORD_TYPE == "S"
                    && joinedPcomcs.pcomc.COMPANY_CODE == companyCode
                    && joinedPcomcs.pcomc.POLICY_NUMBER == policyNumber)
                .AsNoTracking()
                .Select(joinedPcomcs => new AgentDTO
                {
                    SERVICE_AGENT_IND = joinedPcomcs.pcomc_s.SERVICE_AGENT_IND,
                    MARKET_CODE = joinedPcomcs.pcomc_s.MARKET_CODE,
                    AGENT = joinedPcomcs.pcomc_s.AGENT,
                    AGENT_LEVEL = joinedPcomcs.pcomc_s.AGENT_LEVEL,
                    COMM_PCNT = joinedPcomcs.pcomc_s.COMM_PCNT
                })
                .Distinct()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<CompanyCodeAndPolicyNumber?> GetCompanyCodeAndPolicyNumberByPolicyNumber(
            DbContext context,
            string policyNumber)
        {
            var record = await context.Set<PPOLC>()
                .Where(ppolc => ppolc.POLICY_NUMBER == policyNumber)
                .AsNoTracking()
                .Select(ppolc => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = ppolc.COMPANY_CODE,
                    PolicyNumber = ppolc.POLICY_NUMBER
                })
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<CompanyCodeAndPolicyNumber?> GetCompanyCodeAndPolicyNumberByCOMCID(
            DbContext context,
            long comcId)
        {
            var record = await context.Set<PCOMC_COMMISSION_CONTROL>()
                .Join(
                    context.Set<PCOMC_COMMISSION_CONTROL_TYPE_S>(),
                    pcomc => pcomc.COMC_ID,
                    pcomc_s => pcomc_s.COMC_ID,
                    (pcomc, pcomc_s) => new { pcomc, pcomc_s })
                .Where(joinedPcomcs => joinedPcomcs.pcomc.RECORD_TYPE == "S"
                    && joinedPcomcs.pcomc.COMC_ID == comcId)
                .AsNoTracking()
                .Select(joinedPcomcs => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = joinedPcomcs.pcomc.COMPANY_CODE,
                    PolicyNumber = joinedPcomcs.pcomc.POLICY_NUMBER
                })
                .Distinct()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<CompanyCodeAndPolicyNumber?> GetCompanyCodeAndPolicyNumberByPBENID(
            DbContext context,
            long pbenId)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS>()
                .Join(
                    context.Set<PPOLC>(),
                    ppben => ppben.POLICY_NUMBER,
                    ppolc => ppolc.POLICY_NUMBER,
                    (ppben, ppolc) => new { ppben, ppolc })
                .Where(joinedQuery => joinedQuery.ppben.PBEN_ID == pbenId)
                .AsNoTracking()
                .Select(joinedQuery => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = joinedQuery.ppolc.COMPANY_CODE,
                    PolicyNumber = joinedQuery.ppolc.POLICY_NUMBER
                })
                .Distinct()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<CompanyCodeAndPolicyNumber?> GetCompanyCodeAndPolicyNumberByPENDID(
            DbContext context,
            long pendId)
        {
            var record = await context.Set<PPEND_NEW_BUSINESS_PENDING>()
                .Join(
                    context.Set<PPEND_NEW_BUS_PEND_UNDERWRITING>(),
                    ppend => ppend.PEND_ID,
                    pendu => pendu.PEND_ID,
                    (ppend, pendu) => new { ppend, pendu })
                .Where(joinedQuery => joinedQuery.ppend.PEND_ID == pendId)
                .AsNoTracking()
                .Select(joinedQuery => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = joinedQuery.ppend.COMPANY_CODE,
                    PolicyNumber = joinedQuery.ppend.POLICY_NUMBER
                })
                .Distinct()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<Agent> GetAgentUpline(DbContext context, string agentNum, string marketCode, string agentLevel, DateTime applicationDate)
        {
            var formattedDate = int.Parse(applicationDate.ToString("yyyyMMdd"));

            var hierarchyAgent = await context.Set<PHIER_AGENT_HIERARCHY>()
                .Where(x => x.AGENT_NUM == agentNum && x.MARKET_CODE == marketCode && x.AGENT_LEVEL == agentLevel &&
                        x.START_DATE <= formattedDate && x.STOP_DATE >= formattedDate)
                .Select(x => new Agent
                {
                    AgentId = x.HIERARCHY_AGENT,
                    MarketCode = x.HIER_MARKET_CODE,
                    Level = x.HIER_AGENT_LEVEL,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(hierarchyAgent);
            return hierarchyAgent;
        }

        [Trace]
        public List<ParticipantDTO> GetParticipantDTOs(DbContext context, string policyNumber, string companyCode, List<string>? relateCodes)
        {
            var baseQuery = GetParticipantBaseQuery(context, policyNumber, companyCode, relateCodes);
            baseQuery = baseQuery
                .Include(prela => prela.PNAME.PNALKs)
                    .ThenInclude(pnalk => pnalk.PADDR);

            return baseQuery
                .Select(prela => new ParticipantDTO
                {
                    RelateCode = prela.RELATE_CODE,
                    IdentifyingAlpha = prela.IDENTIFYING_ALPHA.Trim(),
                    BenefitSequenceNumber = prela.BENEFIT_SEQ_NUMBER,
                    SexCode = prela.PNAME.SEX_CODE,
                    DateOfBirth = prela.PNAME.DATE_OF_BIRTH,
                    Addresses =
                        prela
                        .PNAME
                        .PNALKs
                        .Where(pnalk => pnalk.ADDRESS_CODE == string.Empty && pnalk.CANCEL_DATE == 0)
                        .Select(pnalk => new AddressDTO
                        {
                            AddressId = pnalk.ADDRESS_ID,
                            TelephoneNumber = pnalk.TELE_NUM.Trim(),
                            Line1 = pnalk.PADDR.ADDR_LINE_1.Trim(),
                            Line2 = pnalk.PADDR.ADDR_LINE_2.Trim(),
                            Line3 = pnalk.PADDR.ADDR_LINE_3.Trim(),
                            City = pnalk.PADDR.CITY.Trim(),
                            State = pnalk.PADDR.STATE.Trim(),
                            Zip = pnalk.PADDR.ZIP.Trim(),
                            ZipExtension = pnalk.PADDR.ZIP_EXTENSION.Trim(),
                            BoxNumber = pnalk.PADDR.BOX_NUMBER.Trim(),
                            Country = pnalk.PADDR.COUNTRY.Trim()
                        })
                        .ToList(),
                    Name = MapPNAMEToNameDto(prela.PNAME)
                })
                .ToList();
        }

        [Trace]
        public List<ParticipantDTO> GetParticipantDTOsWithoutAddress(DbContext context, string policyNumber, string companyCode, List<string>? relateCodes)
        {
            var baseQuery = GetParticipantBaseQuery(context, policyNumber, companyCode, relateCodes);
            baseQuery = baseQuery.Include(q => q.PNAME);

            return baseQuery
                .Select(prela => new ParticipantDTO
                {
                    RelateCode = prela.RELATE_CODE,
                    IdentifyingAlpha = prela.IDENTIFYING_ALPHA,
                    BenefitSequenceNumber = prela.BENEFIT_SEQ_NUMBER,
                    SexCode = prela.PNAME.SEX_CODE,
                    DateOfBirth = prela.PNAME.DATE_OF_BIRTH,
                    Name = MapPNAMEToNameDto(prela.PNAME)
                })
                .ToList();
        }

        [Trace]
        public async Task<List<PolicyRelationship>> GetPolicyRelationships(
            DbContext context,
            int nameId,
            int addressId)
        {
            // Get an IQueryable for a join from PPOLC to PRELA to PNALK
            var pnalkQuery = context.Set<PPOLC>()
                .Join(
                    context.Set<PRELA_RELATIONSHIP_MASTER>(),
                    ppolc => ppolc.COMPANY_CODE + ppolc.POLICY_NUMBER,
                    prela => prela.IDENTIFYING_ALPHA,
                    (ppolc, prela) => new { ppolc, prela })
                .Join(
                    context.Set<PNALK>(),
                    ppolcAndPrela => ppolcAndPrela.prela.NAME_ID,
                    pnalk => pnalk.NAME_ID,
                    (ppolcAndPrela, pnalk) => new { ppolcAndPrela, pnalk });

            // Apply a where clause to the query for the given nameId and addressId,
            // also filtering for only PNALK records that are "active".
            pnalkQuery = pnalkQuery
                .Where(o =>
                    o.pnalk.CANCEL_DATE == 0
                    && o.pnalk.ADDRESS_CODE == string.Empty
                    && o.pnalk.NAME_ID == nameId
                    && o.pnalk.ADDRESS_ID == addressId)
                .AsNoTracking();

            var pnalkQuerySelection = pnalkQuery
                .Select(o => new
                {
                    o.ppolcAndPrela.ppolc.COMPANY_CODE,
                    o.ppolcAndPrela.ppolc.POLICY_NUMBER,
                    o.ppolcAndPrela.prela.RELATE_CODE
                })
                .Distinct();

            // Performs the query execution against the database
            var pnalkQueryResults = await pnalkQuerySelection
                .ToListAsync();

            var records = pnalkQueryResults.GroupBy(
                o => new { o.COMPANY_CODE, o.POLICY_NUMBER },
                (key, group) => new PolicyRelationship
                {
                    CompanyCode = key.COMPANY_CODE,
                    PolicyNumber = key.POLICY_NUMBER,
                    RelateCodes = group.Select(o => o.RELATE_CODE).Distinct().ToList()
                }).ToList();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<string?> GetPolicyStatusDetail(DbContext context, string policyNumber, string companyCode)
        {
            var record = await context.Set<PPOLM_POLICY_BENEFIT_MISC>()
                .Join(
                    context.Set<PICDA_WAIVER_DETAILS>(),
                    ppolm => ppolm.CANCEL_REASON,
                    pwaiv => pwaiv.KEY_DATA,
                    (ppolm, pwaiv) => new { ppolm, pwaiv })
                .Where(joinedPpolm => joinedPpolm.pwaiv.TYPE_CODE == "Y"
                    && joinedPpolm.ppolm.POLICY_NUMBER == policyNumber
                    && joinedPpolm.ppolm.COMPANY_CODE == companyCode)
                .AsNoTracking()
                .Select(joinedPpolm => joinedPpolm.pwaiv.DESCRIPTION.Trim())
                .Distinct()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<EmployerDTO?> GetEmployerDetail(DbContext context, string policyNumber, string companyCode)
        {
            var record = await context.Set<PPOLC>()
                .Join(
                    context.Set<PGRUP_GROUP_MASTER>(),
                    ppolc => ppolc.GROUP_NUMBER,
                    pgrup => pgrup.GROUP_NUMBER,
                    (ppolc, pgrup) => new { ppolc, pgrup })
                .Join(
                    context.Set<PNAME>(),
                    ppolcAndPgrup => ppolcAndPgrup.pgrup.NAME_ID,
                    pname => pname.NAME_ID,
                    (ppolcAndPgrup, pname) => new { ppolcAndPgrup, pname })
                .Where(joinedQuery => joinedQuery.ppolcAndPgrup.ppolc.GROUP_NUMBER == joinedQuery.ppolcAndPgrup.pgrup.GROUP_NUMBER
                    && joinedQuery.ppolcAndPgrup.pgrup.NAME_ID == joinedQuery.pname.NAME_ID
                    && joinedQuery.ppolcAndPgrup.ppolc.POLICY_NUMBER == policyNumber
                    && joinedQuery.ppolcAndPgrup.ppolc.COMPANY_CODE == companyCode)
                .AsNoTracking()
                .Select(o => new EmployerDTO
                {
                    GroupNumber = o.ppolcAndPgrup.ppolc.GROUP_NUMBER,
                    EmployerName = o.pname.NAME_BUSINESS,
                    BusinessEmailAddress = o.pname.BUSINESS_EMAIL_ADR,
                    NameId = o.pname.NAME_ID,
                    StatusCode = o.ppolcAndPgrup.pgrup.STATUS_CODE
                })
                .Distinct()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public async Task<List<PolicyRequirement>> GetPolicyRequirementsForHealth(
            DbContext context,
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            // Get an IQueryable for a join from PRQRM to PRQRMTBL to PMEDR
            var reqQuery = context.Set<PRQRM>()
                .Join(
                    context.Set<PRQRMTBL>(),
                    prqrm => new { prqrm.COMPANY_CODE, prqrm.POLICY_NUMBER, prqrm.NAME_ID, prqrm.REQ_SEQUENCE },
                    prqrmtbl => new { prqrmtbl.COMPANY_CODE, prqrmtbl.POLICY_NUMBER, prqrmtbl.NAME_ID, prqrmtbl.REQ_SEQUENCE },
                    (prqrm, prqrmtbl) => new { prqrm, prqrmtbl })
                .Join(
                    context.Set<PMEDR>(),
                    reqTables => reqTables.prqrmtbl.UND_DESC_CODE,
                    pmedr => pmedr.REQ_NUMBER,
                    (reqTables, pmedr) => new { reqTables.prqrm, reqTables.prqrmtbl, pmedr });

            // Apply a where clause to the query.
            // The REQ_DESCRIPTION can sometimes be empty (which is actually filled with spaces
            // in the LifePro SQL table), so those records are filtered out.
            reqQuery = reqQuery
                .Where(o =>
                    o.prqrm.COMPANY_CODE == companyCodeAndPolicyNumber.CompanyCode
                    && o.prqrm.POLICY_NUMBER == companyCodeAndPolicyNumber.PolicyNumber
                    && o.pmedr.REQ_DESCRIPTION != string.Empty
                    && o.pmedr.RECORD_TYPE == "R")
                .AsNoTracking();

            // Apply a Select clause, only selecting what's needed for PolicyRequirement.
            var reqQuerySelection = reqQuery
                .Select(o => new PolicyRequirement
                {
                    Description = o.pmedr.REQ_DESCRIPTION,
                    NameId = o.prqrm.NAME_ID,
                    Id = o.pmedr.REQ_NUMBER,
                    AddedDate = o.prqrmtbl.UND_REQ_DATE,
                    ObtainedDate = o.prqrmtbl.UND_OBTAIN_DATE,
                    Status = o.prqrmtbl.UND_REQ_MET,
                    LifeproComment = o.prqrmtbl.UND_COMMENTS,
                    ReqSequence = o.prqrmtbl.SEQ,
                    Ix = (short)o.prqrmtbl.UND_REQ_NOTE_SEQ,
                    ReqType = o.pmedr.REQ_NAME
                })
                .Distinct();

            // Performs the query execution against the database.
            var records = await reqQuerySelection
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<List<PolicyRequirement>> GetPolicyRequirementsForLife(
            DbContext context,
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            var reqQuery = context.Set<PPEND_NEW_BUSINESS_PENDING>()
                .Join(
                    context.Set<PPEND_NEW_BUS_PEND_UNDERWRITING>(),
                    ppend => new { ppend.PEND_ID },
                    pendu => new { pendu.PEND_ID },
                    (ppend, pendu) => new { ppend, pendu })
                .Join(
                    context.Set<PMEDR>(),
                    reqTables => reqTables.pendu.UND_CODE,
                    pmedr => pmedr.REQ_NUMBER,
                    (reqTables, pmedr) => new { reqTables.ppend, reqTables.pendu, pmedr });

            // Apply a where clause to the query.
            // The REQ_DESCRIPTION can sometimes be empty (which is actually filled with spaces
            // in the LifePro SQL table), so those records are filtered out.
            reqQuery = reqQuery
                .Where(o =>
                    o.ppend.COMPANY_CODE == companyCodeAndPolicyNumber.CompanyCode
                    && o.ppend.POLICY_NUMBER == companyCodeAndPolicyNumber.PolicyNumber
                    && o.ppend.REDEF_TYPE == RedefTypes.Underwriting
                    && o.pmedr.REQ_DESCRIPTION != string.Empty
                    && o.pmedr.RECORD_TYPE == "R")
                .AsNoTracking();

            // Apply a Select clause, only selecting what's needed for PolicyRequirement.
            var reqQuerySelection = reqQuery
                .Select(o => new PolicyRequirement
                {
                    Description = o.pmedr.REQ_DESCRIPTION,
                    NameId = o.ppend.UND_NAME_ID,
                    Id = o.pmedr.REQ_NUMBER,
                    AddedDate = o.pendu.UND_DATE,
                    ObtainedDate = o.pendu.UND_O_DATE,
                    Status = o.pendu.UND_FLAG,
                    LifeproComment = o.pendu.COMMENTS,
                    ReqSequence = o.pendu.NOTE_SEQ,
                    Ix = o.pendu.IDX,
                    ReqType = o.pmedr.REQ_NAME
                })
                .Distinct();

            // Performs the query execution against the database.
            var records = await reqQuerySelection
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<string?> GetCoverageDescription(DbContext context, string planCode)
        {
            var record = await context.Set<PCOVR_PRODUCT_COVERAGES>()
                .Where(pcovr => pcovr.COVERAGE_ID.Trim() == planCode.Trim())
                .AsNoTracking()
                .Select(pcovr => pcovr.DESCRIPTION.Trim())
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        public List<PaymentDTO> GetPaymentData(DbContext context, string policyNumber, string companyCode)
        {
            var creditCodes = new List<int> { 2, 110, 121, 122, 123, 124, 127, 128, 129, 131, 132 };

            return
                context.Set<PACTG>()
                .Where(ppolm =>
                    ppolm.POLICY_NUMBER == policyNumber.Trim()
                    && ppolm.COMPANY_CODE == companyCode.Trim()
                    && creditCodes.Contains(ppolm.CREDIT_CODE))
                .AsNoTracking()
                .Select(p => new PaymentDTO
                {
                    CreditCode = p.CREDIT_CODE,
                    DebitCode = p.DEBIT_CODE,
                    EffectiveDate = p.EFFECTIVE_DATE,
                    ReversalCode = p.REVERSAL_CODE
                })
                .ToList();
        }

        [Trace]
        public bool IsInitialPaymentDeclined(DbContext context, string policyNumber, string companyCode)
        {
            return
                context.Set<PPOLM_POLICY_BENEFIT_MISC>()
                .Where(ppolm => ppolm.POLICY_NUMBER == policyNumber.Trim() &&
                    ppolm.COMPANY_CODE == companyCode && ppolm.CANCEL_REASON == "17" &&
                    ppolm.CANCEL_DESC == "FREE LOOK PERIOD - BAD CHECK OR CREDIT CARD")
                .AsNoTracking()
                .Any();
        }

        [Trace]
        public async Task<int?> GetMostRecentPaymentDate(DbContext context, string policyNumber, string companyCode)
        {
            var creditCodesList = new List<int> { 110, 121, 122, 123, 124, 127, 128, 129, 131, 132 };
            var records = await context.Set<PACTG>()
                .Where(pactg => pactg.POLICY_NUMBER == policyNumber.Trim() &&
                    pactg.COMPANY_CODE == companyCode.Trim() &&
                    creditCodesList.Contains(pactg.CREDIT_CODE))
                .AsNoTracking()
                .Select(pactg => pactg.EFFECTIVE_DATE)
                .ToListAsync();

            return records.Count() > 0 ? records.Max() : null;
        }

        [Trace]
        public async Task<int?> GetMostRecentCardDeclinedDate(DbContext context, string policyNumber, string companyCode)
        {
            var records = await context.Set<PACTG>()
                .Where(pactg => pactg.POLICY_NUMBER == policyNumber.Trim() &&
                    pactg.COMPANY_CODE == companyCode.Trim() && pactg.CREDIT_CODE == 110 &&
                    pactg.DEBIT_CODE == 771 && pactg.REVERSAL_CODE == "Y")
                .AsNoTracking()
                .Select(pactg => pactg.EFFECTIVE_DATE)
                .ToListAsync();

            return records.Count() > 0 ? records.Max() : null;
        }

        [Trace]
        public async Task<int?> GetMostRecentCheckDraftDeclinedDate(DbContext context, string policyNumber, string companyCode)
        {
            var records = await context.Set<PACTG>()
                .Where(pactg => pactg.POLICY_NUMBER == policyNumber.Trim() &&
                    pactg.COMPANY_CODE == companyCode.Trim() &&
                    pactg.CREDIT_CODE == 2)
                .AsNoTracking()
                .Select(pactg => pactg.EFFECTIVE_DATE)
                .ToListAsync();

            return records.Count() > 0 ? records.Max() : null;
        }

        [Trace]
        public async Task<bool> IsULBenefitInGracePeriod(DbContext context, long benefitId)
        {
            var record = await context.Set<PPBEN_POLICY_BENEFITS_TYPES_BF>()
                .Where(ppben => ppben.PBEN_ID == benefitId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return record != null && record.BF_DATE_NEGATIVE > 0;
        }

        [Trace]
        public async Task<List<string>> GetPastDuePolicyNumbers(DbContext context, int comparisonDate)
        {
            var records = await context.Set<PPOLC>()
                .Where(ppolc => ppolc.PAID_TO_DATE < comparisonDate &&
                    PastDueCodes.IncludedForPastDueContractCodes.Contains(ppolc.CONTRACT_CODE) &&
                    !PastDueCodes.ExcludedFromPastDueLinesOfBusiness.Contains(ppolc.LINE_OF_BUSINESS) &&
                    ppolc.BILLING_FORM != "LST" &&
                    !PastDueCodes.ExcludedFromPastDueBillingReasons.Contains(ppolc.BILLING_REASON))
                .AsNoTracking()
                .Select(ppolc => ppolc.POLICY_NUMBER.Trim())
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(records);
            return records;
        }

        [Trace]
        public async Task<PACON_ANNUITY_POLICY?> GetAnnuityPolicy(DbContext context, string policyNumber, string companyCode)
        {
            var record = await context.Set<PACON_ANNUITY_POLICY>()
                .Where(pacon => pacon.COMPANY_CODE == companyCode
                    && pacon.POLICY_NUMBER == policyNumber)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(record);
            return record;
        }

        [Trace]
        private static NameDTO MapPNAMEToNameDto(PNAME pname)
        {
            if (pname == null)
            {
                return null;
            }

            return new NameDTO
            {
                BusinessEmailAdress = pname.BUSINESS_EMAIL_ADR.Trim(),
                NameBusiness = pname.NAME_BUSINESS.Trim(),
                NameId = pname.NAME_ID,
                IndividualFirst = pname.INDIVIDUAL_FIRST.Trim(),
                IndividualLast = pname.INDIVIDUAL_LAST.Trim(),
                IndividualMiddle = pname.INDIVIDUAL_MIDDLE.Trim(),
                IndividualSuffix = pname.INDIVIDUAL_SUFFIX.Trim(),
                IndividualPrefix = pname.INDIVIDUAL_PREFIX.Trim(),
                PersonalEmailAdress = pname.PERSONAL_EMAIL_ADR.Trim(),
                NameFormatCode = pname.NAME_FORMAT_CODE
            };
        }

        [Trace]
        private static BenefitDTO MapToBenefitDTO(PPBEN_POLICY_BENEFITS ppben)
        {
            if (ppben == null)
            {
                return null;
            }

            var benefitDto = new BenefitDTO
            {
                BenefitSequence = ppben.BENEFIT_SEQ,
                BenefitType = ppben.BENEFIT_TYPE,
                CompanyCode = ppben.COMPANY_CODE,
                PolicyNumber = ppben.POLICY_NUMBER,
                PBEN_ID = ppben.PBEN_ID,
                PlanCode = ppben.PLAN_CODE,
                StatusCode = ppben.STATUS_CODE,
                StatusDate = ppben.STATUS_DATE,
                StatusReason = ppben.STATUS_REASON
            };

            benefitDto.TrimStringProperties();

            if (ppben.PCOVR_PRODUCT_COVERAGES != null)
            {
                benefitDto.ProductCoverages = new ProductCoveragesDTO
                {
                    Description = ppben.PCOVR_PRODUCT_COVERAGES.DESCRIPTION.Trim()
                };
            }

            if (ppben.PMUIN_MULTIPLE_INSUREDs?.Any() ?? false)
            {
                benefitDto.MultipleInsureds =
                    ppben.PMUIN_MULTIPLE_INSUREDs
                    .Select(pmuin => new MultipleInsuredDTO
                    {
                        NameId = pmuin.NAME_ID,
                        KdDefSegmentId = pmuin.KD_DEF_SEGT_ID.Trim(),
                        KdBenefitExtendedKeys = pmuin.KD_BEN_EXTEND_KEYS.Replace("\0", string.Empty).TrimEnd('0').Trim(),
                        RelationshipToPrimaryInsured = pmuin.MULT_RELATE.Trim(),
                        StartDate = pmuin.START_DATE,
                        StopDate = pmuin.STOP_DATE,
                        UnderwritingClass = pmuin.UWCLS.Trim(),
                    })
                    .ToList();
            }

            if (ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR != null)
            {
                benefitDto.BaseOrOtherRider = new PolicyBenefitTypeBA_OR
                {
                    AnnualPremiumPerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR.ANN_PREM_PER_UNIT,
                    NumberOfUnits = ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR.NUMBER_OF_UNITS,
                    Dividend = ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR.DIVIDEND,
                    PBEN_ID = ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR.PBEN_ID,
                    ValuePerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR.VALUE_PER_UNIT
                };
                benefitDto.BaseOrOtherRider.TrimStringProperties();
            }

            if (ppben.PPBEN_POLICY_BENEFITS_TYPES_BF != null)
            {
                benefitDto.BaseForUniversalLife = new PolicyBenefitTypeBF
                {
                    AnnualPremiumPerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.ANN_PREM_PER_UNIT,
                    NumberOfUnits = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.NUMBER_OF_UNITS,
                    PBEN_ID = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.PBEN_ID,
                    ValuePerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.VALUE_PER_UNIT,
                    BF_CURRENT_DB = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.BF_CURRENT_DB,
                    BF_DATE_NEGATIVE = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.BF_DATE_NEGATIVE,
                    BF_DB_OPTION = ppben.PPBEN_POLICY_BENEFITS_TYPES_BF.BF_DB_OPTION
                };
                benefitDto.BaseForUniversalLife.TrimStringProperties();
            }

            if (ppben.PPBEN_POLICY_BENEFITS_TYPES_SL != null)
            {
                benefitDto.PolicyBenefitTypeSL = new SubBenefitDTO
                {
                    AnnualPremiumPerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SL.ANN_PREM_PER_UNIT,
                    NumberOfUnits = ppben.PPBEN_POLICY_BENEFITS_TYPES_SL.NUMBER_OF_UNITS,
                    PBEN_ID = ppben.PPBEN_POLICY_BENEFITS_TYPES_SL.PBEN_ID,
                    ValuePerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SL.VALUE_PER_UNIT
                };
                benefitDto.PolicyBenefitTypeSL.TrimStringProperties();
            }

            if (ppben.PPBEN_POLICY_BENEFITS_TYPES_SP != null)
            {
                benefitDto.SpecifiedAmountIncrease = new SubBenefitDTO
                {
                    AnnualPremiumPerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SP.ANN_PREM_PER_UNIT,
                    NumberOfUnits = ppben.PPBEN_POLICY_BENEFITS_TYPES_SP.NUMBER_OF_UNITS,
                    PBEN_ID = ppben.PPBEN_POLICY_BENEFITS_TYPES_SP.PBEN_ID,
                    ValuePerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SP.VALUE_PER_UNIT
                };
                benefitDto.SpecifiedAmountIncrease.TrimStringProperties();
            }

            if (ppben.PPBEN_POLICY_BENEFITS_TYPES_SU != null)
            {
                benefitDto.Supplemental = new SubBenefitDTO
                {
                    AnnualPremiumPerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SU.ANN_PREM_PER_UNIT,
                    NumberOfUnits = ppben.PPBEN_POLICY_BENEFITS_TYPES_SU.NUMBER_OF_UNITS,
                    PBEN_ID = ppben.PPBEN_POLICY_BENEFITS_TYPES_SU.PBEN_ID,
                    ValuePerUnit = ppben.PPBEN_POLICY_BENEFITS_TYPES_SU.VALUE_PER_UNIT
                };
                benefitDto.Supplemental.TrimStringProperties();
            }

            if (ppben.PCEXP_COVERAGE_EXPANSION != null)
            {
                benefitDto.CoverageExpansion = new CoverageExpansionDTO
                {
                    Details =
                        ppben
                        .PCEXP_COVERAGE_EXPANSION
                        .PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS
                        ?.Select(exp => new CoverageExpansionDetailDTO
                        {
                            UnderwritingClassCode = exp.UWCLS_CODE.Trim(),
                            UnderwritingClassDescription = exp.UWCLS_DESC.Trim()
                        })
                        .ToList()
                };
            }

            return benefitDto;
        }

        [Trace]
        private static IQueryable<PPBEN_POLICY_BENEFITS> GetBenefitsBaseQuery(
            DbContext context,
            string policyNumber,
            string companyCode)
        {
            return context.Set<PPBEN_POLICY_BENEFITS>()
                .Where(ppben =>
                    ppben.POLICY_NUMBER == policyNumber && ppben.COMPANY_CODE == companyCode
                    &&
                    (ppben.BENEFIT_TYPE == BenefitTypes.Base
                        || ppben.BENEFIT_TYPE == BenefitTypes.OtherRider
                        || ppben.BENEFIT_TYPE == BenefitTypes.BaseForUniversalLife
                        || ppben.BENEFIT_TYPE == BenefitTypes.Supplemental
                        || ppben.BENEFIT_TYPE == BenefitTypes.SpecifiedAmountIncrease
                        || ppben.BENEFIT_TYPE == BenefitTypes.TableRating))
                .Include(ppben => ppben.PCOVR_PRODUCT_COVERAGES)
                .Include(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR)
                .Include(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_BF)
                .Include(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SL)
                .Include(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SP)
                .Include(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SU)
                .Include(ppben => ppben.PMUIN_MULTIPLE_INSUREDs)
                .Include(ppben => ppben.PCEXP_COVERAGE_EXPANSION)
                    .ThenInclude(pcexp => pcexp.PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS)
                .AsNoTracking();
        }

        [Trace]
        private static IQueryable<PRELA_RELATIONSHIP_MASTER> GetParticipantBaseQuery(DbContext context, string policyNumber, string companyCode, List<string> relateCodes = null)
        {
            var identifyingAlpha = companyCode + policyNumber;

            var baseQuery =
                context.Set<PRELA_RELATIONSHIP_MASTER>()
                .Where(prela => prela.IDENTIFYING_ALPHA == identifyingAlpha)
                .AsNoTracking();

            relateCodes ??= new List<string>();

            if (relateCodes.Any())
            {
                baseQuery = baseQuery.Where(prela => relateCodes.Contains(prela.RELATE_CODE));
            }

            return baseQuery;
        }
    }
}