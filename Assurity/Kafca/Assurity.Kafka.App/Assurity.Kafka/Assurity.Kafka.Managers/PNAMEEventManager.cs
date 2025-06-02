namespace Assurity.Kafka.Managers
{
    using System.Linq;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// The class responsible for handling an updated PNAME record.
    /// </summary>
    public class PNAMEEventManager : IPNAMEEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PNAMEEventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The policyEngine that will be used for various policy related logic.</param>
        /// <param name="mapper">Auto mapper.</param>
        /// <param name="configuration"></param>
        public PNAMEEventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PNAMEEventManager> logger,
            IConsumerPolicyEngine policyEngine,
            IMapper mapper,
            IConfigurationManager configuration)
        {
            EventsAccessor = eventsAccessor;
            Logger = logger;
            PolicyEngine = policyEngine;
            Mapper = mapper;
            Config = configuration;
        }

        private IEventsAccessor EventsAccessor { get; }

        private ILogger<PNAMEEventManager> Logger { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IMapper Mapper { get; }

        private IConfigurationManager Config { get; set; }

        /// <summary>
        /// Process the event from the PPNAMEEvent Topic.
        /// Updates all policies that are dependant on this PNAME.
        /// </summary>
        /// <param name="pname">A pname record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PNAME pname, bool slowConsumer = false)
        {
            try
            {
                // Take the nameID from this pname and look for it in all the policies
                // in the mongo db that contain a name that matches this nameID.
                // Update all the name fields where this is found.
                // If none are found, do nothing.
                if (await UpdateExistingNameInPolicies(pname, slowConsumer))
                {
                    Logger.LogInformation("Processing PNAME event NameId: {NAME_ID} has update policies", pname.NAME_ID);
                }
                else
                {
                    Logger.LogWarning(
                        "Processing PNAME event NameId: {NAME_ID} was not found in an existing policy and will be ignored", pname.NAME_ID);
                }

                return;
            }
            catch (SlowConsumerException)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a policy object for a given policyNumber.
        /// </summary>
        /// <param name="pname">A list of policyNumbers corresponding to policies to be migrated.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns>The policy object for a given policyNumber. </returns>
        [Trace]
        public async Task<bool> UpdateExistingNameInPolicies(PNAME pname, bool slowConsumer = false)
        {
            var isBusiness = pname.NAME_FORMAT_CODE == "B";
            bool updatedFlag = false;

            var newPerson = Mapper.Map<Person>(pname);
            if (newPerson == null || newPerson.Name == null)
            {
                return updatedFlag;
            }

            var policiesToBeUpdated = await GetPoliciesWithNameId(newPerson.Name.NameId);
            var policiesToBeUpdatedCount = policiesToBeUpdated.Count;
            if (policiesToBeUpdatedCount == 0)
            {
                return updatedFlag;
            }

            if (policiesToBeUpdatedCount > Config.KafkaSlowConsumerUpdateThreshold && !slowConsumer)
            {
                Logger.LogWarning(
                    "Slow Consumer - PNAME event NameId: {pname.NAME_ID}, Number of updates to process: {policiesToBeUpdated.Count}. This will be done while disconnected from kafka",
                    pname.NAME_ID,
                    policiesToBeUpdated.Count);

                throw new SlowConsumerException();
            }

            foreach (var policy in policiesToBeUpdated)
            {
                var policyDictionary = new Dictionary<string, object>();

                if (policy.Insureds?.Any() ?? false)
                {
                    foreach (var insured in policy.Insureds)
                    {
                        updatedFlag = UpdatePerson(insured.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Insureds)))
                    {
                        policyDictionary.Add(nameof(Policy.Insureds), policy.Insureds);
                    }
                }

                if (policy.Owners?.Any() ?? false)
                {
                    foreach (var owner in policy.Owners)
                    {
                        updatedFlag = UpdatePerson(owner.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Owners)))
                    {
                        policyDictionary.Add(nameof(Policy.Owners), policy.Owners);
                    }
                }

                if (policy.Payors?.Any() ?? false)
                {
                    foreach (var payor in policy.Payors)
                    {
                        updatedFlag = UpdatePerson(payor.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Payors)))
                    {
                        policyDictionary.Add(nameof(Policy.Payors), policy.Payors);
                    }
                }

                if (policy.Beneficiaries?.Any() ?? false)
                {
                    foreach (var beneficiary in policy.Beneficiaries)
                    {
                        updatedFlag = UpdatePerson(beneficiary.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Beneficiaries)))
                    {
                        policyDictionary.Add(nameof(Policy.Beneficiaries), policy.Beneficiaries);
                    }
                }

                if (policy.Annuitants?.Any() ?? false)
                {
                    foreach (var annuitant in policy.Annuitants)
                    {
                        updatedFlag = UpdatePerson(annuitant.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Annuitants)))
                    {
                        policyDictionary.Add(nameof(Policy.Annuitants), policy.Annuitants);
                    }
                }

                if (policy.Agents?.Any() ?? false)
                {
                    foreach (var agent in policy.Agents)
                    {
                        updatedFlag = UpdatePerson(agent.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Agents)))
                    {
                        policyDictionary.Add(nameof(Policy.Agents), policy.Agents);
                    }
                }

                var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber
                {
                    PolicyNumber = policy.PolicyNumber,
                    CompanyCode = policy.CompanyCode,
                };

                policy.Requirements = await PolicyEngine.GetRequirements(policy);
                if (policy.Requirements?.Any() ?? false)
                {
                    var policyDictionaryForRequirements = new Dictionary<string, object>
                    {
                        { nameof(Policy.Requirements), policy.Requirements }
                    };

                    await EventsAccessor.UpdatePolicyAsync(policy, policyDictionaryForRequirements);

                    var modifiedCount = await EventsAccessor.UpdateNameAndEmailAddressInPolicyRequirements(policy, newPerson, isBusiness);
                    updatedFlag = true;
                }

                if (policy.Employer?.Business != null)
                {
                    updatedFlag = UpdateEmployerName(policy.Employer.Business, newPerson.Name, updatedFlag, policy.PolicyNumber, pname.NAME_ID);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Employer)))
                    {
                        policyDictionary.Add(nameof(Policy.Employer), policy.Employer);
                    }
                }

                if (policy.Payee?.Participant != null)
                {
                    updatedFlag = UpdatePerson(policy.Payee.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Payee)))
                    {
                        policyDictionary.Add(nameof(Policy.Payee), policy.Payee);
                    }
                }

                if (policy.Assignee?.Participant != null)
                {
                    updatedFlag = UpdatePerson(policy.Assignee.Participant, isBusiness, newPerson, updatedFlag, policy.PolicyNumber, pname.NAME_ID);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Assignee)))
                    {
                        policyDictionary.Add(nameof(Policy.Assignee), policy.Assignee);
                    }
                }

                if (updatedFlag)
                {
                    await EventsAccessor.UpdatePolicyAsync(policy, policyDictionary);
                    Logger.LogInformation("Processing PNAME event NameId: {pname.NAME_ID} with policy number: {policy.PolicyNumber}", pname.NAME_ID, policy.PolicyNumber);
                }
            }

            return updatedFlag;
        }

        [Trace]
        internal async Task<List<Policy>> GetPoliciesWithNameId(int nameId)
        {
            var policies = new List<Policy>();

            await GetPoliciesWithInsuredsByNameId(policies, nameId);
            await GetPoliciesWithOwnersByNameId(policies, nameId);
            await GetPoliciesWithPayorsByNameId(policies, nameId);
            await GetPoliciesWithBeneficiariesByNameId(policies, nameId);
            await GetPoliciesWithPayeeByNameId(policies, nameId);
            await GetPoliciesWithAnnuitantsByNameId(policies, nameId);
            await GetPoliciesWithAssigneeByNameId(policies, nameId);
            await GetPoliciesWithAgentsByNameId(policies, nameId);
            await GetPoliciesWithRequirementsByNameId(policies, nameId);
            await GetPoliciesWithEmployerByNameId(policies, nameId);

            return policies.DistinctBy(policy => policy.PolicyNumber).ToList();
        }

        [Trace]
        internal async Task GetPoliciesWithInsuredsByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesInsureds = await EventsAccessor.GetPoliciesWithInsuredsByNameIdAsync(nameId);
            if (mdbPoliciesInsureds?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesInsureds);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithOwnersByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesOwners = await EventsAccessor.GetPoliciesWithOwnersByNameIdAsync(nameId);
            if (mdbPoliciesOwners?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesOwners);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithBeneficiariesByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesBeneficiaries = await EventsAccessor.GetPoliciesWithBeneficiariesByNameIdAsync(nameId);
            if (mdbPoliciesBeneficiaries?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesBeneficiaries);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithPayorsByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesPayors = await EventsAccessor.GetPoliciesWithPayorsByNameIdAsync(nameId);
            if (mdbPoliciesPayors?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesPayors);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAnnuitantsByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesAnnuitants = await EventsAccessor.GetPoliciesWithAnnuitantsByNameIdAsync(nameId);
            if (mdbPoliciesAnnuitants?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAnnuitants);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAssigneeByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesAssignee = await EventsAccessor.GetPoliciesWithAssigneeByNameIdAsync(nameId);
            if (mdbPoliciesAssignee?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAssignee);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithPayeeByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesPayee = await EventsAccessor.GetPoliciesWithPayeeByNameIdAsync(nameId);
            if (mdbPoliciesPayee?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesPayee);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAgentsByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesAgents = await EventsAccessor.GetPoliciesWithAgentsByNameIdAsync(nameId);
            if (mdbPoliciesAgents?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAgents);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithRequirementsByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesRequirements = await EventsAccessor.GetPoliciesWithRequirementsByNameIdAsync(nameId);
            if (mdbPoliciesRequirements?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesRequirements);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithEmployerByNameId(List<Policy> policies, int nameId)
        {
            var mdbPoliciesEmployer = await EventsAccessor.GetPoliciesWithEmployerByNameIdAsync(nameId);
            if (mdbPoliciesEmployer?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesEmployer);
            }
        }

        [Trace]
        internal bool UpdatePerson(Participant participant, bool isBusiness, Person newPerson, bool updatedFlag, string policyNumber, int nameId)
        {
            if (participant != null)
            {
                if (isBusiness)
                {
                    if (participant.Business?.Name?.NameId == newPerson.Name.NameId)
                    {
                        participant.IsBusiness = isBusiness;
                        participant.Business.Name = newPerson.Name;
                        participant.Business.EmailAddress = newPerson.EmailAddress;
                        updatedFlag = true;
                    }
                }
                else if (participant.Person?.Name?.NameId == newPerson.Name.NameId)
                {
                    participant.Person = newPerson;
                    participant.IsBusiness = isBusiness;
                    updatedFlag = true;
                }
            }
            else
            {
                Logger.LogWarning("Missing Participant for PNAME event NameId: {nameId} for PolicyNumber: {policyNumber}.", nameId, policyNumber);
            }

            return updatedFlag;
        }

        [Trace]
        internal bool UpdateEmployerName(Business business, Name newName, bool updatedFlag, string policyNumber, int nameId)
        {
            if (business != null)
            {
                if (business.Name?.NameId == newName.NameId)
                {
                    business.Name = newName;
                    updatedFlag = true;
                }
            }
            else
            {
                Logger.LogWarning("Missing Business for PNAME event NameId: {nameId} for PolicyNumber: {policyNumber}.", nameId, policyNumber);
            }

            return updatedFlag;
        }
    }
}
