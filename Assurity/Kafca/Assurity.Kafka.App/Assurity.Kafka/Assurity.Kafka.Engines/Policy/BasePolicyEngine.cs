using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assurity.Kafka.Engines.Tests")]

namespace Assurity.Kafka.Engines.Policy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using NewRelic.Api.Agent;

    public abstract class BasePolicyEngine
    {
        public BasePolicyEngine(
            IDataStoreAccessor dataStoreAccessor,
            ILifeProAccessor lifeProAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor,
            IEventsAccessor eventsAccessor,
            IConfigurationManager configurationManager,
            ILogger<BasePolicyEngine> logger,
            IPolicyMapper policyMapper)
        {
            DataStoreAccessor = dataStoreAccessor;
            LifeProAccessor = lifeProAccessor;
            GlobalDataAccessor = globalDataAccessor;
            SupportDataAccessor = supportDataAccessor;
            EventsAccessor = eventsAccessor;
            Config = configurationManager;
            Logger = logger;
            PolicyMapper = policyMapper;
        }

        private ILogger<BasePolicyEngine> Logger { get; }

        private IDataStoreAccessor DataStoreAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        private ISupportDataAccessor SupportDataAccessor { get; }

        private IGlobalDataAccessor GlobalDataAccessor { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConfigurationManager Config { get; set; }

        private IPolicyMapper PolicyMapper { get; set; }

        [Trace]
        internal async Task<List<Agent>> GetAgents(bool isMigrationWorker, string policyNumber, string companyCode, DateTime applicationDate)
        {
            var agentRecords = new List<PolicyAgentDTO>();
            if (isMigrationWorker)
            {
                agentRecords = DataStoreAccessor.GetPolicyAgentDTOs(policyNumber, companyCode);
            }
            else
            {
                agentRecords = LifeProAccessor.GetPolicyAgentDTOs(policyNumber, companyCode);
            }

            var nonJitAgents = agentRecords.Where(agent => !IsAgentJustInTime(agent.AgentNumber)).ToList();
            var agents = nonJitAgents.Select(PolicyMapper.MapAgent).ToList();
            if (agentRecords.Any(agent => IsAgentJustInTime(agent.AgentNumber)))
            {
                await GetJustInTimeAgents(isMigrationWorker, agents, policyNumber, companyCode, applicationDate);
            }

            if (agents == null || agents.Count == 0)
            {
                Logger.LogWarning(
                    "No agents found for PolicyNumber and Company Code {PolicyNumber} - {CompanyCode}",
                    policyNumber,
                    companyCode);
            }

            return agents;
        }

        [Transaction]
        internal async Task DeleteAllPolicyData(string policyNumber, string companyCode)
        {
            Logger.LogInformation("Starting DeletePolicy for PolicyNumber: {policyNumber}", policyNumber);

            try
            {
                var existingHierarchy = await EventsAccessor.GetPolicyHierarchyAsync(policyNumber, companyCode);
                if (existingHierarchy != null)
                {
                    var agentIds = RetrieveAgentIds(existingHierarchy.HierarchyBranches);

                    foreach (var agentId in agentIds)
                    {
                        await EventsAccessor.RemoveAgentPolicyAccessAsync(agentId, policyNumber, companyCode);
                    }

                    await EventsAccessor.DeletePolicyHierarchyAsync(policyNumber, companyCode);
                }

                await EventsAccessor.DeletePolicyAsync(policyNumber, companyCode);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DeletePolicy failed for PolicyNumber: {policyNumber}", policyNumber);
            }
        }

        [Transaction]
        internal async Task<(GetPolicyResult, Policy?)> GetPolicy(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            Logger.LogInformation("Creating policy with policy number {policyNumber} and company code {companyCode}", policyNumber, companyCode);
            var policy = new Policy();

            try
            {
                var policyDTO = new PolicyDTO();
                if (isMigrationWorker)
                {
                    policyDTO = DataStoreAccessor.GetPolicyDTO(policyNumber, companyCode);
                }
                else
                {
                    policyDTO = LifeProAccessor.GetPolicyDTO(policyNumber, companyCode);
                }

                if (policyDTO == null)
                {
                    return (GetPolicyResult.NotFound, null);
                }

                policy = PolicyMapper.MapPolicy(policyDTO);
                var returnPaymentData = GetReturnPaymentData(isMigrationWorker, policy.PolicyNumber, policy.CompanyCode);
                if (HasInitialPaymentDeclinedThatIsBeyondRetentionDuration(returnPaymentData.returnPaymentType, returnPaymentData.returnPaymentDate))
                {
                    Logger.LogWarning(
                        "Return Payment Date for Policy Number: {PolicyNumber} passed the Retention Duration value: {InitialPaymentDeclinedRetentionDays}",
                        policy.PolicyNumber,
                        Config.InitialPaymentDeclinedRetentionDays);

                    return (GetPolicyResult.HasInitialPaymentDeclinedThatIsBeyondRetentionDuration, null);
                }
                else
                {
                    policy.ReturnPaymentType = returnPaymentData.returnPaymentType;
                    policy.ReturnPaymentDate = returnPaymentData.returnPaymentDate;
                }

                policy.PolicyStatusDetail = await GetPolicyStatusDetail(isMigrationWorker, policy.PolicyNumber, policy.CompanyCode);
                var productDescription = await GetBaseProductDescriptionByProductCode(isMigrationWorker, policy.ProductCode);
                if (productDescription != null)
                {
                    policy.ProductCategory = productDescription.ProdCategory;
                    policy.ProductDescription = productDescription.AltProdDesc;
                }
                else
                {
                    Logger.LogWarning(
                        "PolicyNumber: {PolicyNumber} - unable to find base product description with ProductCode: {ProductCode}",
                        policyNumber,
                        policy.ProductCode);
                }

                var ppend = await GetPPEND_NEW_BUSINESS_PENDING(isMigrationWorker, policy.PolicyNumber);
                if (ppend != null)
                {
                    policy.SubmitDate = ppend.REQUIREMENT_DATE.ToNullableDateTime();
                }

                var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = policy.CompanyCode,
                    PolicyNumber = policy.PolicyNumber
                };

                var participantsExcludingBeneficiaries = GetParticipantsExcludingBeneficiaries(isMigrationWorker, policyNumber, companyCode);
                policy.Annuitants = PolicyMapper.MapAnnuitants(participantsExcludingBeneficiaries);
                policy.Assignee = PolicyMapper.MapAssignee(participantsExcludingBeneficiaries);
                policy.Owners = PolicyMapper.MapOwners(participantsExcludingBeneficiaries);
                policy.Payors = PolicyMapper.MapPayors(participantsExcludingBeneficiaries);
                policy.Payee = PolicyMapper.MapPayee(participantsExcludingBeneficiaries);

                var beneficiaryParticipants = GetBeneficiaryParticipants(isMigrationWorker, policyNumber, companyCode);
                policy.Beneficiaries = PolicyMapper.MapBeneficiaries(beneficiaryParticipants);

                var benefitDtos = new List<BenefitDTO>();
                if (isMigrationWorker)
                {
                    benefitDtos = DataStoreAccessor.GetBenefitDTOs(policyNumber, companyCode);
                }
                else
                {
                    benefitDtos = LifeProAccessor.GetBenefitDTOs(policyNumber, companyCode);
                }

                policy.Insureds = PolicyMapper.MapInsureds(participantsExcludingBeneficiaries, benefitDtos);
                policy.Benefits = MapBenefits(isMigrationWorker, policy.LineOfBusiness, benefitDtos);
                policy.Agents = await GetAgents(isMigrationWorker, policy.PolicyNumber, policy.CompanyCode, policy.ApplicationDate.GetValueOrDefault());

                var combinedParticipants = participantsExcludingBeneficiaries.Concat(beneficiaryParticipants).ToList();
                policy.Requirements = await GetRequirements(isMigrationWorker, policy, combinedParticipants);
                policy.PastDue = IsPastDue(policy, benefitDtos);
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    "Policy Number: {PolicyNumber} - Failed to assemble. Exception: {exMessage}:{exStackTrace}",
                    policyNumber,
                    ex.Message,
                    ex.StackTrace);

                return (GetPolicyResult.ExceptionThrown, null);
            }

            return (GetPolicyResult.Found, policy);
        }

        [Trace]
        internal async Task<PolicyStatusDetail> GetPolicyStatusDetail(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            string? policyStatusDetail;
            if (isMigrationWorker)
            {
                policyStatusDetail = await DataStoreAccessor.GetPolicyStatusDetail(policyNumber, companyCode);
            }
            else
            {
                policyStatusDetail = await LifeProAccessor.GetPolicyStatusDetail(policyNumber, companyCode);
            }

            return policyStatusDetail.ToPolicyStatusDetail();
        }

        [Trace]
        internal List<Policy> UpdateRequirementNames(
            List<Policy> policies,
            short reqId,
            string reqDescription)
        {
            foreach (var policy in policies)
            {
                int index = 0;
                foreach (var requirement in policy.Requirements)
                {
                    if (requirement.Id == reqId)
                    {
                        policy.Requirements[index].Name = reqDescription;
                    }

                    index++;
                }
            }

            return policies;
        }

        internal bool StatusSupportSyntheticRequirement(Status policyStatus, ReturnPaymentType returnPaymentType)
        {
            if (policyStatus == Status.Pending ||
               (policyStatus == Status.Terminated &&
               (returnPaymentType == ReturnPaymentType.InitialPaymentCardDeclined ||
               returnPaymentType == ReturnPaymentType.InitialPaymentCheckDraftDeclined)))
            {
                return true;
            }

            return false;
        }

        [Trace]
        internal (ReturnPaymentType returnPaymentType, DateTime? returnPaymentDate) GetReturnPaymentData(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            var paymentData = GetPaymentData(isMigrationWorker, policyNumber, companyCode);
            if (paymentData == null || paymentData.Count == 0)
            {
                return (ReturnPaymentType.None, null);
            }

            var isInitialPaymentDeclined = IsInitialPaymentDeclined(isMigrationWorker, policyNumber, companyCode);
            var mostRecentPaymentDate = paymentData.Max(p => p.EffectiveDate);
            var mostRecentCardDeclinedDate =
                paymentData
                .Where(p =>
                    p.CreditCode == 110
                    && p.DebitCode == 771
                    && p.ReversalCode == "Y")
                ?.Max(p => (int?)p.EffectiveDate);

            if (mostRecentPaymentDate == mostRecentCardDeclinedDate)
            {
                return isInitialPaymentDeclined
                    ? (ReturnPaymentType.InitialPaymentCardDeclined, mostRecentPaymentDate.ToNullableDateTime())
                    : (ReturnPaymentType.CardDeclined, mostRecentPaymentDate.ToNullableDateTime());
            }

            var mostRecentCheckDeclinedDate =
                paymentData
                .Where(p => p.CreditCode == 2)
                ?.Max(p => (int?)p.EffectiveDate);

            if (mostRecentPaymentDate == mostRecentCheckDeclinedDate)
            {
                return isInitialPaymentDeclined
                    ? (ReturnPaymentType.InitialPaymentCheckDraftDeclined, mostRecentPaymentDate.ToNullableDateTime())
                    : (ReturnPaymentType.CheckDraftDeclined, mostRecentPaymentDate.ToNullableDateTime());
            }

            return (ReturnPaymentType.None, null);
        }

        [Trace]
        internal async Task<ProductDescription> GetBaseProductDescriptionByProductCode(bool isMigrationWorker, string productCode)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetBaseProductDescriptionByProductCode(productCode);
            }
            else
            {
                return await LifeProAccessor.GetBaseProductDescriptionByProductCode(productCode);
            }
        }

        [Trace]
        internal async Task<ProductDescription> GetBaseProductDescriptionByPlanCode(bool isMigrationWorker, string planCode)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetBaseProductDescriptionByPlanCode(planCode);
            }
            else
            {
                return await LifeProAccessor.GetBaseProductDescriptionByPlanCode(planCode);
            }
        }

        [Trace]
        internal async Task<List<JustInTimeAgentDTO>> GetJustInTimeAgentIds(bool isMigrationWorker, string policyNumber, DateTime applicationDate)
        {
            var jitAgents = new List<JustInTimeAgentDTO>();
            var queueDescriptions = await SupportDataAccessor.GetQueueDescriptions();
            var nbFolderIds = await GlobalDataAccessor.GetNewBusinessFolderIds(policyNumber);

            if (nbFolderIds == null || !nbFolderIds.Any())
            {
                return jitAgents;
            }

            var justInTimeAgentDTOs = await GlobalDataAccessor.GetJustInTimeAgentIds(nbFolderIds);
            foreach (var justInTimeAgentDTO in justInTimeAgentDTOs)
            {
                if (string.IsNullOrWhiteSpace(justInTimeAgentDTO.AGENT))
                {
                    continue;
                }

                var agentFolderIds = await GlobalDataAccessor.GetAgentFolderIdsFromAttributes(justInTimeAgentDTO.AGENT);
                if (agentFolderIds == null || !agentFolderIds.Any())
                {
                    return new List<JustInTimeAgentDTO>
                    {
                        new JustInTimeAgentDTO
                        {
                            AgentId = justInTimeAgentDTO.AGENT,
                            MarketCode = justInTimeAgentDTO.MARKET_CODE,
                            Level = justInTimeAgentDTO.AGENT_LEVEL
                        }
                    };
                }

                foreach (var agentFolderId in agentFolderIds)
                {
                    if (string.IsNullOrWhiteSpace(agentFolderId))
                    {
                        continue;
                    }

                    var queue = await GlobalDataAccessor.GetQueueFromFolderId(agentFolderId);

                    if (string.IsNullOrWhiteSpace(queue)
                        || !queueDescriptions.Select(q => q.ToLower()).Contains(queue.ToLower()))
                    {
                        continue;
                    }

                    var justInTimeAgent = await GlobalDataAccessor.GetJitAgentInfoFromFolderId(
                        justInTimeAgentDTO.AGENT,
                        agentFolderId,
                        justInTimeAgentDTO.MARKET_CODE,
                        justInTimeAgentDTO.AGENT_LEVEL);

                    if (string.IsNullOrWhiteSpace(justInTimeAgent?.AgentId))
                    {
                        var initialUpline = await GetAgentUpline(
                            isMigrationWorker,
                            justInTimeAgentDTO.AGENT,
                            justInTimeAgentDTO.MARKET_CODE,
                            justInTimeAgentDTO.AGENT_LEVEL,
                            applicationDate);

                        if (!string.IsNullOrWhiteSpace(initialUpline?.AgentId))
                        {
                            justInTimeAgent = new JustInTimeAgentDTO
                            {
                                AgentId = justInTimeAgentDTO.AGENT,
                                MarketCode = justInTimeAgentDTO.MARKET_CODE,
                                Level = justInTimeAgentDTO.AGENT_LEVEL,
                                UplineAgentId = initialUpline?.AgentId,
                                UplineMarketCode = initialUpline?.MarketCode,
                                UplineLevel = initialUpline?.Level
                            };
                        }
                    }

                    jitAgents.Add(justInTimeAgent);
                }
            }

            return jitAgents;
        }

        [Trace]
        internal async Task<PPEND_NEW_BUSINESS_PENDING> GetPPEND_NEW_BUSINESS_PENDING(bool isMigrationWorker, string policyNumber)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetPPEND_NEW_BUSINESS_PENDING(policyNumber);
            }
            else
            {
                return await LifeProAccessor.GetPPEND_NEW_BUSINESS_PENDING(policyNumber);
            }
        }

        [Trace]
        internal bool HasInitialPaymentDeclinedThatIsBeyondRetentionDuration(ReturnPaymentType returnPaymentType, DateTime? returnPaymentDate)
        {
            if (!returnPaymentDate.HasValue)
            {
                return false;
            }

            var initialPaymentDeclineTypes = new List<ReturnPaymentType>
                {
                    ReturnPaymentType.InitialPaymentCheckDraftDeclined,
                    ReturnPaymentType.InitialPaymentCardDeclined
                };

            return initialPaymentDeclineTypes.Contains(returnPaymentType) && DateTime.Now.CompareTo(returnPaymentDate.Value.AddDays(Config.InitialPaymentDeclinedRetentionDays)) > 0;
        }

        [Trace]
        internal async Task<List<PolicyRequirement>> GetPolicyRequirementsForHealth(bool isMigrationWorker, CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetPolicyRequirementsForHealth(companyCodeAndPolicyNumber);
            }
            else
            {
                return await LifeProAccessor.GetPolicyRequirementsForHealth(companyCodeAndPolicyNumber);
            }
        }

        [Trace]
        internal async Task<List<PolicyRequirement>> GetPolicyRequirementsForLife(bool isMigrationWorker, CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetPolicyRequirementsForLife(companyCodeAndPolicyNumber);
            }
            else
            {
                return await LifeProAccessor.GetPolicyRequirementsForLife(companyCodeAndPolicyNumber);
            }
        }

        [Trace]
        internal async Task<List<PAGNT_AGENT_MASTER>> GetPAGNT_AGENT_MASTER(bool isMigrationWorker, string companyCode, string agent)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetPAGNT_AGENT_MASTER(companyCode, agent);
            }
            else
            {
                return await LifeProAccessor.GetPAGNT_AGENT_MASTER(companyCode, agent);
            }
        }

        [Trace]
        internal async Task<Participant> CreateParticipantWithName(bool isMigrationWorker, int nameId)
        {
            var pname = await GetPNAME(isMigrationWorker, nameId);
            Participant participant = new Participant();

            if (pname != null)
            {
                participant.IsBusiness = pname.NAME_FORMAT_CODE == "B";

                if (participant.IsBusiness)
                {
                    participant.Business = new Business
                    {
                        Name = new Name
                        {
                            NameId = nameId,
                            BusinessName = pname.NAME_BUSINESS
                        }
                    };
                }
                else
                {
                    participant.Person = new Person
                    {
                        Name = new Name()
                        {
                            NameId = nameId,
                            IndividualPrefix = pname.INDIVIDUAL_PREFIX,
                            IndividualFirst = pname.INDIVIDUAL_FIRST,
                            IndividualMiddle = pname.INDIVIDUAL_MIDDLE,
                            IndividualLast = pname.INDIVIDUAL_LAST,
                            IndividualSuffix = pname.INDIVIDUAL_SUFFIX,
                        }
                    };
                }
            }

            return participant;
        }

        [Trace]
        internal async Task<List<Requirement>> GetRequirements(
            bool isMigrationWorker,
            Policy policy,
            List<ParticipantDTO> participantDTOs)
        {
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(policy.CompanyCode, policy.PolicyNumber);
            var healthRequirementsTask = GetPolicyRequirementsForHealth(isMigrationWorker, companyCodeAndPolicyNumber);
            var lifeRequirementsTask = GetPolicyRequirementsForLife(isMigrationWorker, companyCodeAndPolicyNumber);
            await Task.WhenAll(healthRequirementsTask, lifeRequirementsTask);
            var policyRequirements = await healthRequirementsTask;
            var lifeRequirements = await lifeRequirementsTask;

            policyRequirements.AddRange(lifeRequirements);
            var policyStatus = policy.PolicyStatus;
            var returnPaymentType = policy.ReturnPaymentType;

            if (policyRequirements == null && !policyRequirements.Any())
            {
                if (StatusSupportSyntheticRequirement(policyStatus, returnPaymentType))
                {
                    return new List<Requirement> { PolicyMapper.GenerateHomeOfficeReviewRequirement() };
                }
            }

            var globalLookupDto = new GlobalRequirementCommentsLookupDTO
            {
                PolicyNumber = policy.PolicyNumber,
                Lookups = policyRequirements.Select(r => new RequirementLookupDTO
                {
                    IX = r.Ix,
                    REQSEQ = r.ReqSequence,
                    REQTYPE = r.ReqType
                })
                .ToList()
            };

            var globalComments = GlobalDataAccessor.GetRequirementComments(globalLookupDto);
            var policyRequirementIds = policyRequirements.Select(req => (int)req.Id).ToList();
            var requirementMappings = EventsAccessor.GetRequirementMappings(policyRequirementIds);

            var mappedRequirements = PolicyMapper.MapRequirements(policyRequirements, requirementMappings, participantDTOs, globalComments);
            if (StatusSupportSyntheticRequirement(policyStatus, returnPaymentType) &&
                !mappedRequirements.Any(requirement => requirement.Display && requirement.Status == RequirementStatus.Unmet))
            {
                mappedRequirements.Add(PolicyMapper.GenerateHomeOfficeReviewRequirement());
            }

            return mappedRequirements.Any()
                ? mappedRequirements
                : null;
        }

        /// <summary>
        /// Generates a DTO used to look up extended key data in LifePro data.
        /// </summary>
        /// <remarks>
        /// The lookup data is unique by benefit identifier. We only want to call once for the benefits.
        /// If multiple insureds have the same benefit identifier, we want to use the longest ordinal length.
        /// </remarks>
        /// <param name="multipleInsuredDtos"></param>
        /// <returns></returns>
        [Trace]
        internal ExtendedKeysLookup GenerateExtendedKeysLookup(List<MultipleInsuredDTO> multipleInsuredDtos)
        {
            var dictionary = new Dictionary<string, KeyLookup>();
            foreach (var insured in multipleInsuredDtos)
            {
                var identifier = insured.KdDefSegmentId;
                var extendedKeys = insured.KdBenefitExtendedKeys;
                if (!dictionary.ContainsKey(insured.KdDefSegmentId))
                {
                    dictionary.Add(
                        insured.KdDefSegmentId,
                        new KeyLookup
                        {
                            Identifier = identifier,
                            MaxOrdinal = (short)(extendedKeys.Length / 2),
                            MaxKeyValue = GetMaxExtendedKeyValue(extendedKeys)
                        });
                }
                else
                {
                    var existingLookup = dictionary[identifier];
                    var incomingOrdinalLength = (short)extendedKeys.Length / 2;
                    var incomingMaxKeyValue = GetMaxExtendedKeyValue(extendedKeys);
                    existingLookup.MaxOrdinal = (short)Math.Max(existingLookup.MaxOrdinal, incomingOrdinalLength);
                    existingLookup.MaxKeyValue = Math.Max(existingLookup.MaxKeyValue, incomingMaxKeyValue);
                }
            }

            return new ExtendedKeysLookup
            {
                Lookups = dictionary.Values.ToList()
            };
        }

        private static bool IsAgentJustInTime(string agentId)
        {
            return agentId.Equals(AgentType.Z9Agent, StringComparison.InvariantCultureIgnoreCase);
        }

        [Trace]
        private async Task<PNAME> GetPNAME(bool isMigrationWorker, int nameId)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetPNAME(nameId);
            }
            else
            {
                return await LifeProAccessor.GetPNAME(nameId);
            }
        }

        [Trace]
        private List<PaymentDTO> GetPaymentData(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            if (isMigrationWorker)
            {
                return DataStoreAccessor.GetPaymentData(policyNumber, companyCode);
            }
            else
            {
                return LifeProAccessor.GetPaymentData(policyNumber, companyCode);
            }
        }

        [Trace]
        private bool IsInitialPaymentDeclined(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            if (isMigrationWorker)
            {
                return DataStoreAccessor.IsInitialPaymentDeclined(policyNumber, companyCode);
            }
            else
            {
                return LifeProAccessor.IsInitialPaymentDeclined(policyNumber, companyCode);
            }
        }

        [Trace]
        private async Task<Agent?> GetAgentUpline(bool isMigrationWorker, string agentId, string marketcode, string level, DateTime applicationDate)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetAgentUpline(
                    agentId,
                    marketcode,
                    level,
                    applicationDate);
            }
            else
            {
                return await LifeProAccessor.GetAgentUpline(
                    agentId,
                    marketcode,
                    level,
                    applicationDate);
            }
        }

        [Trace]
        private List<Benefit> MapBenefits(
            bool isMigrationWorker,
            LineOfBusiness lineOfBusiness,
            List<BenefitDTO> benefitDtos)
        {
            var extendedKeysLookup =
                GenerateExtendedKeysLookup(
                    benefitDtos
                    .SelectMany(benefit => benefit?.MultipleInsureds ?? new List<MultipleInsuredDTO>())
                    ?.ToList());

            var lookupData = new List<ExtendedKeyLookupResult>();
            if (isMigrationWorker)
            {
                lookupData = DataStoreAccessor.GetExtendedKeyData(extendedKeysLookup);
            }
            else
            {
                lookupData = LifeProAccessor.GetExtendedKeyData(extendedKeysLookup);
            }

            return benefitDtos
                .Select(dto => PolicyMapper.MapBenefit(lineOfBusiness, dto, lookupData))
                .ToList();
        }

        [Trace]
        private short GetMaxExtendedKeyValue(string extendedKeys)
        {
            var duplicatedKey = new string(extendedKeys);
            short maxValue = 0;

            while (duplicatedKey.Length >= 2)
            {
                var substring = duplicatedKey[..2];
                if (string.IsNullOrWhiteSpace(substring))
                {
                    substring = "0";
                }

                var currentValue = short.Parse(substring);
                if (currentValue > maxValue)
                {
                    maxValue = currentValue;
                }

                duplicatedKey = duplicatedKey[2..];
            }

            return maxValue;
        }

        [Trace]
        private bool IsPastDue(Policy policy, List<BenefitDTO> ppbens)
        {
            if (policy.PolicyStatus == Status.Pending || policy.PolicyStatus == Status.Terminated)
            {
                return false;
            }

            if (policy.BillingForm == BillingForm.ListBill)
            {
                return false;
            }

            if (policy.BillingReason == BillingReason.PaidUp
                || policy.BillingReason == BillingReason.ReducedPaidUp
                || policy.BillingReason == BillingReason.ExtendedTerm
                || policy.BillingReason == BillingReason.StoppedPremium)
            {
                return false;
            }

            if (policy.LineOfBusiness == LineOfBusiness.Annuity || policy.LineOfBusiness == LineOfBusiness.ImmediateAnnuity)
            {
                return false;
            }

            if (policy.LineOfBusiness == LineOfBusiness.UniversalLife)
            {
                var baseBenefit =
                    policy
                    .Benefits
                    .FirstOrDefault(benefit => benefit.CoverageType == CoverageType.Base);

                if (baseBenefit != null)
                {
                    var pben = ppbens.FirstOrDefault(p => p.PBEN_ID == baseBenefit.BenefitId);

                    return pben != null && pben.BaseForUniversalLife?.BF_DATE_NEGATIVE > 0;
                }
            }

            if (policy.PaidToDate != null && policy.PaidToDate.Value.Date < DateTime.Now.Date)
            {
                return true;
            }

            return false;
        }

        [Trace]
        private async Task GetJustInTimeAgents(
            bool isMigrationWorker,
            List<Agent> agents,
            string policyNumber,
            string companyCode,
            DateTime applicationDate)
        {
            var jitAgentRecords = await GetJustInTimeAgentIds(isMigrationWorker, policyNumber, applicationDate);
            if (jitAgentRecords == null || !jitAgentRecords.Any())
            {
                return;
            }

            foreach (var jitAgentRecord in jitAgentRecords)
            {
                var agentACRecord = await GlobalDataAccessor.GetAgentName(jitAgentRecord.AgentId);
                var agent = PolicyMapper.MapAgent(jitAgentRecord, agentACRecord);
                if (agent != null)
                {
                    agents.Add(agent);
                }
                else
                {
                    var pagnt = await GetPAGNT_AGENT_MASTER(isMigrationWorker, companyCode, jitAgentRecord.AgentId);
                    foreach (var record in pagnt)
                    {
                        var pname = await GetPNAME(isMigrationWorker, record.NAME_ID);
                        agents.Add(PolicyMapper.MapAgent(jitAgentRecord, pname));
                    }
                }
            }
        }

        [Trace]
        private List<string> RetrieveAgentIds(List<AgentHierarchy> agentHierarchyList)
        {
            var currentAgentIds = new List<string>();
            foreach (var agentHierarchy in agentHierarchyList)
            {
                if (!string.IsNullOrEmpty(agentHierarchy.Agent?.AgentId))
                {
                    currentAgentIds.Add(agentHierarchy.Agent.AgentId);
                }

                foreach (var hierarchyAgent in agentHierarchy.HierarchyAgents)
                {
                    if (!string.IsNullOrEmpty(hierarchyAgent?.AgentId))
                    {
                        currentAgentIds.Add(hierarchyAgent.AgentId);
                    }
                }
            }

            return currentAgentIds.Distinct().ToList();
        }

        [Trace]
        private List<ParticipantDTO> GetParticipantsExcludingBeneficiaries(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            var relateCodes = RelateCodes.GetRelateCodesExcludingBeneficiary();
            if (isMigrationWorker)
            {
                return DataStoreAccessor.GetParticipantDTOs(policyNumber, companyCode, relateCodes);
            }
            else
            {
                return LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, relateCodes);
            }
        }

        [Trace]
        private List<ParticipantDTO> GetBeneficiaryParticipants(bool isMigrationWorker, string policyNumber, string companyCode)
        {
            var relateCodes = RelateCodes.BeneficiaryRelateCodes;
            if (isMigrationWorker)
            {
                return DataStoreAccessor.GetParticipantDTOsWithoutAddress(policyNumber, companyCode, relateCodes);
            }
            else
            {
                return LifeProAccessor.GetParticipantDTOsWithoutAddress(policyNumber, companyCode, relateCodes);
            }
        }
    }
}