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
    /// The class responsible for handling an updated PADDR record.
    /// </summary>
    public class PADDREventManager : IPADDREventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PADDREventManager"/> class.
        /// </summary>
        /// <param name="eventsAccessor">Accesses MongoDB Events database.</param>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="policyEngine">The policyEngine that will be used for various policy related logic.</param>
        /// <param name="mapper">Auto mapper.</param>
        /// <param name="configuration"></param>
        public PADDREventManager(
            IEventsAccessor eventsAccessor,
            ILogger<PADDREventManager> logger,
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

        private ILogger<PADDREventManager> Logger { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private IMapper Mapper { get; }

        private IConfigurationManager Config { get; set; }

        /// <summary>
        /// Process the event from the PADDREvent Topic.
        /// Updates all policies that are dependent on this PADDR.
        /// </summary>
        /// <param name="paddr">A paddr record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PADDR paddr, bool slowConsumer = false)
        {
            try
            {
                // Take the ADDRESS_ID from paddr and look for it in all the policies
                // in the mongo db that contain an address that matches this addressId
                // Update all the address fields where this is found.
                // If none are found, do nothing.
                if (await UpdateExistingAddressInPolicies(paddr, slowConsumer))
                {
                    Logger.LogInformation("Processing PADDR event AddressId: {ADDRESS_ID} has update policies", paddr.ADDRESS_ID);
                }
                else
                {
                    Logger.LogWarning("Processing PADDR event AddressId: {ADDRESS_ID} was not found in an existing policy and will be ignored", paddr.ADDRESS_ID);
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
        /// <param name="paddr">Address record with the given AddressId corresponding to policies to be migrated.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns>True or False depending on if the policy object was updated. </returns>
        [Trace]
        public async Task<bool> UpdateExistingAddressInPolicies(PADDR paddr, bool slowConsumer = false)
        {
            var newAddress = Mapper.Map<Address>(paddr);
            bool updatedFlag = false;
            var policiesToBeUpdated = await GetPoliciesWithAddressId(newAddress.AddressId);

            var policiesToBeUpdatedCount = policiesToBeUpdated.Count;
            if (policiesToBeUpdatedCount > Config.KafkaSlowConsumerUpdateThreshold && !slowConsumer)
            {
                Logger.LogWarning(
                    "Slow Consumer - PADDR event AddressId: {paddr.ADDRESS_ID}, " +
                    "Number of updates to process: {policiesToBeUpdated.Count}. This will be " +
                    "done while disconnected from kafka",
                    paddr.ADDRESS_ID,
                    policiesToBeUpdated.Count);
                throw new SlowConsumerException();
            }

            if (policiesToBeUpdatedCount == 0)
            {
                return updatedFlag;
            }

            foreach (var policy in policiesToBeUpdated)
            {
                var policyDictionary = new Dictionary<string, object>();

                if (policy.Insureds?.Any() ?? false)
                {
                    foreach (var insured in policy.Insureds)
                    {
                        updatedFlag = UpdateAddress(insured.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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
                        updatedFlag = UpdateAddress(owner.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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
                        updatedFlag = UpdateAddress(payor.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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
                        updatedFlag = UpdateAddress(beneficiary.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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
                        updatedFlag = UpdateAddress(annuitant.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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
                        updatedFlag = UpdateAddress(agent.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);
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

                    var modifiedCount = await EventsAccessor.UpdateAddressInPolicyRequirements(policy, newAddress);
                    updatedFlag = true;
                }

                if (policy.Payee?.Participant != null)
                {
                    updatedFlag = UpdateAddress(policy.Payee.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Payee)))
                    {
                        policyDictionary.Add(nameof(Policy.Payee), policy.Payee);
                    }
                }

                if (policy.Assignee?.Participant != null)
                {
                    updatedFlag = UpdateAddress(policy.Assignee.Participant, newAddress, updatedFlag, policy.PolicyNumber, paddr.ADDRESS_ID);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Assignee)))
                    {
                        policyDictionary.Add(nameof(Policy.Assignee), policy.Assignee);
                    }
                }

                if (updatedFlag)
                {
                    await EventsAccessor.UpdatePolicyAsync(policy, policyDictionary);
                    Logger.LogInformation(
                        "Processing PADDR event AddressId: {ADDRESS_ID} with policy number: {PolicyNumber}",
                        paddr.ADDRESS_ID,
                        policy.PolicyNumber);
                }
            }

            return updatedFlag;
        }

        [Trace]
        internal async Task<List<Policy>> GetPoliciesWithAddressId(int addressId)
        {
            var policies = new List<Policy>();

            await GetPoliciesWithInsuredsByAddressId(policies, addressId);
            await GetPoliciesWithOwnersByAddressId(policies, addressId);
            await GetPoliciesWithPayorsByAddressId(policies, addressId);
            await GetPoliciesWithBeneficiariesByAddressId(policies, addressId);
            await GetPoliciesWithPayeeByAddressId(policies, addressId);
            await GetPoliciesWithAnnuitantsByAddressId(policies, addressId);
            await GetPoliciesWithAssigneeByAddressId(policies, addressId);
            await GetPoliciesWithAgentsByAddressId(policies, addressId);
            await GetPoliciesWithRequirementsByAddressId(policies, addressId);

            return policies.DistinctBy(policy => policy.PolicyNumber).ToList();
        }

        [Trace]
        internal async Task GetPoliciesWithInsuredsByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesInsureds = await EventsAccessor.GetPoliciesWithInsuredsByAddressIdAsync(addressId);
            if (mdbPoliciesInsureds?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesInsureds);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithOwnersByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesOwners = await EventsAccessor.GetPoliciesWithOwnersByAddressIdAsync(addressId);
            if (mdbPoliciesOwners?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesOwners);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithBeneficiariesByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesBeneficiaries = await EventsAccessor.GetPoliciesWithBeneficiariesByAddressIdAsync(addressId);
            if (mdbPoliciesBeneficiaries?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesBeneficiaries);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithPayorsByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesPayors = await EventsAccessor.GetPoliciesWithPayorsByAddressIdAsync(addressId);
            if (mdbPoliciesPayors?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesPayors);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAnnuitantsByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesAnnuitants = await EventsAccessor.GetPoliciesWithAnnuitantsByAddressIdAsync(addressId);
            if (mdbPoliciesAnnuitants?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAnnuitants);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAssigneeByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesAssignee = await EventsAccessor.GetPoliciesWithAssigneeByAddressIdAsync(addressId);
            if (mdbPoliciesAssignee?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAssignee);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithPayeeByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesPayee = await EventsAccessor.GetPoliciesWithPayeeByAddressIdAsync(addressId);
            if (mdbPoliciesPayee?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesPayee);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithAgentsByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesAgents = await EventsAccessor.GetPoliciesWithAgentsByAddressIdAsync(addressId);
            if (mdbPoliciesAgents?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesAgents);
            }
        }

        [Trace]
        internal async Task GetPoliciesWithRequirementsByAddressId(List<Policy> policies, int addressId)
        {
            var mdbPoliciesRequirements = await EventsAccessor.GetPoliciesWithRequirementsByAddressIdAsync(addressId);
            if (mdbPoliciesRequirements?.Any() ?? false)
            {
                policies.AddRange(mdbPoliciesRequirements);
            }
        }

        [Trace]
        internal bool UpdateAddress(Participant participant, Address newAddress, bool updatedFlag, string policyNumber, int addressId)
        {
            if (participant != null)
            {
                if (participant.Address?.AddressId == newAddress.AddressId)
                {
                    participant.Address = newAddress;
                    updatedFlag = true;
                }
            }
            else
            {
                Logger.LogWarning("Missing Participant for PADDR event AddressId: {addressId} for PolicyNumber: {policyNumber}.", addressId, policyNumber);
            }

            return updatedFlag;
        }
    }
}