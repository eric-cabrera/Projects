namespace Assurity.Kafka.Managers
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Responsible for updating policies associated with the changes in the PNALK event.
    /// </summary>
    public class PNALKEventManager : IPNALKEventManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PNALKEventManager"/> class.
        /// </summary>
        /// <param name="logger">The logger that will write to the New Relic cloud service.</param>
        /// <param name="eventsAccessor">The EventsAccessor is the MongoDB.</param>
        /// <param name="policyEngine">The policyEngine that will be used for various policy related logic.</param>
        /// <param name="lifeProAccessor"></param>
        /// <param name="configuration"></param>
        public PNALKEventManager(
            ILogger<PNALKEventManager> logger,
            IEventsAccessor eventsAccessor,
            IConsumerPolicyEngine policyEngine,
            ILifeProAccessor lifeProAccessor,
            IConfigurationManager configuration)
        {
            Logger = logger;
            EventsAccessor = eventsAccessor;
            PolicyEngine = policyEngine;
            LifeProAccessor = lifeProAccessor;
            Config = configuration;
        }

        private ILogger<PNALKEventManager> Logger { get; }

        private IEventsAccessor EventsAccessor { get; }

        private IConsumerPolicyEngine PolicyEngine { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        private IConfigurationManager Config { get; set; }

        /// <summary>
        /// Process the event from the PNALKEvent Topic.
        /// Updates all policies that are dependent on this PNALK.
        /// </summary>
        /// <param name="pnalk">A pnalk record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        [Transaction]
        public async Task ProcessEvent(PNALK pnalk, bool slowConsumer = false)
        {
            // "BaseAccessor.GetPolicyRelationships() and BasePolicyEngine.CreateParticipant()
            // only consider PNALK records that are active (CANCEL_DATE = 0) and are a
            // primary address type (ADDRESS_CODE is blank). This same filter was applied
            // in Kafka KSQL to prevent irrelevant Topics from being passed into this PNALK
            // Event Manager. Changes to this logic in any one of those places requires a
            // corresponding change in the others."
            try
            {
                if (await UpdateAddressAndTelephoneInPolicies(pnalk, slowConsumer))
                {
                    Logger.LogInformation(
                        "Processing PNALK event NameId: {NAME_ID}, AddressId {ADDRESS_ID}",
                        pnalk?.NAME_ID,
                        pnalk?.ADDRESS_ID);
                }
                else
                {
                    Logger.LogWarning(
                        "Processing PNALK event NameId: {NAME_ID}, AddressId {ADDRESS_ID}, was not found in an existing policy and will be ignored",
                        pnalk?.NAME_ID,
                        pnalk?.ADDRESS_ID);
                }

                return;
            }
            catch (SlowConsumerException)
            {
                throw;
            }
        }

        [Trace]
        internal async Task<bool> UpdateAddressAndTelephoneInPolicies(PNALK pnalk, bool slowConsumer = false)
        {
            bool updatedFlag = false;

            var policiesToBeUpdated = await GetPoliciesWithNameId(pnalk.NAME_ID);
            var policiesToBeUpdatedCount = policiesToBeUpdated.Count;

            if (policiesToBeUpdatedCount > Config.KafkaSlowConsumerUpdateThreshold && !slowConsumer)
            {
                Logger.LogWarning(
                    "Slow Consumer - PNALK event: NameId: {nameId}, AddressId {addressId}, Number of updates to process: {retryCount}. This will be done while disconnected from kafka",
                    pnalk.NAME_ID,
                    pnalk.ADDRESS_ID,
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
                        updatedFlag = await UpdateParticipant(insured.Participant, pnalk, updatedFlag, policy.PolicyNumber);
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
                        updatedFlag = await UpdateParticipant(owner.Participant, pnalk, updatedFlag, policy.PolicyNumber);
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
                        updatedFlag = await UpdateParticipant(payor.Participant, pnalk, updatedFlag, policy.PolicyNumber);
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
                        updatedFlag = await UpdateParticipant(beneficiary.Participant, pnalk, updatedFlag, policy.PolicyNumber);
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
                        updatedFlag = await UpdateParticipant(annuitant.Participant, pnalk, updatedFlag, policy.PolicyNumber);
                    }

                    if (!policyDictionary.ContainsKey(nameof(Policy.Annuitants)))
                    {
                        policyDictionary.Add(nameof(Policy.Annuitants), policy.Annuitants);
                    }
                }

                policy.Requirements = await PolicyEngine.GetRequirements(policy);
                if (policy.Requirements?.Any() ?? false)
                {
                    var policyDictionaryForRequirements = new Dictionary<string, object>
                        {
                            { nameof(Policy.Requirements), policy.Requirements }
                        };

                    await EventsAccessor.UpdatePolicyAsync(policy, policyDictionaryForRequirements);

                    var address = await GetAddress(pnalk.ADDRESS_ID);
                    var modifiedAddressCount = await EventsAccessor.UpdateAddressInPolicyRequirements(policy, address);

                    var telephoneNumber = pnalk.TELE_NUM.ToPhoneNumber();
                    var modifiedPhoneNumberCount = await EventsAccessor.UpdatePhoneNumberInPolicyRequirements(policy, pnalk.NAME_ID, telephoneNumber);

                    if (modifiedAddressCount > 0 || modifiedPhoneNumberCount > 0)
                    {
                        updatedFlag = true;
                    }
                }

                if (policy.Payee?.Participant != null)
                {
                    updatedFlag = await UpdateParticipant(policy.Payee.Participant, pnalk, updatedFlag, policy.PolicyNumber);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Payee)))
                    {
                        policyDictionary.Add(nameof(Policy.Payee), policy.Payee);
                    }
                }

                if (policy.Assignee?.Participant != null)
                {
                    updatedFlag = await UpdateParticipant(policy.Assignee.Participant, pnalk, updatedFlag, policy.PolicyNumber);

                    if (!policyDictionary.ContainsKey(nameof(Policy.Assignee)))
                    {
                        policyDictionary.Add(nameof(Policy.Assignee), policy.Assignee);
                    }
                }

                if (updatedFlag)
                {
                    await EventsAccessor.UpdatePolicyAsync(policy, policyDictionary);
                    Logger.LogInformation(
                        "Processing PNALK event - Address and/or Phone Number have been updated. " +
                        "NameId: {NAME_ID} " +
                        "AddressId {ADDRESS_ID} " +
                        "PolicyNumber: {PolicyNumber}",
                        pnalk.NAME_ID,
                        pnalk.ADDRESS_ID,
                        policy.PolicyNumber);
                }
            }

            return updatedFlag;
        }

        // NameId and PolicyNumber are only needed for logging purposes.
        [Trace]
        internal async Task<bool> UpdateParticipant(Participant participant, PNALK pnalk, bool updatedFlag, string policyNumber)
        {
            if (participant != null)
            {
                updatedFlag = UpdatePhoneNumber(participant, pnalk, updatedFlag, policyNumber);
                updatedFlag = await UpdateAddress(participant, pnalk, updatedFlag, policyNumber);
            }
            else
            {
                Logger.LogWarning(
                    "PNALK event - Missing Participant for PNALK event NameId: {NAME_ID} for PolicyNumber: {policyNumber}.",
                    pnalk.NAME_ID,
                    policyNumber);
            }

            return updatedFlag;
        }

        [Trace]
        internal async Task<bool> UpdateAddress(Participant participant, PNALK pnalk, bool updatedFlag, string policyNumber)
        {
            if (participant.Address == null)
            {
                Logger.LogInformation(
                    "PNALK event - Participant has no address. Attempted to update " +
                    "AddressId: {ADDRESS_ID} " +
                    "NameId: {NAME_ID} " +
                    "for PolicyNumber: {policyNumber}.",
                    pnalk.ADDRESS_ID,
                    pnalk.NAME_ID,
                    policyNumber);
                return updatedFlag;
            }

            if (participant.Address.AddressId == pnalk.ADDRESS_ID)
            {
                return updatedFlag;
            }

            if ((participant.Person != null && participant.Person.Name.NameId == pnalk.NAME_ID) ||
                (participant.Business != null && participant.Business.Name.NameId == pnalk.NAME_ID))
            {
                participant.Address = await GetAddress(pnalk.ADDRESS_ID);
                updatedFlag = true;
            }

            return updatedFlag;
        }

        [Trace]
        internal bool UpdatePhoneNumber(Participant participant, PNALK pnalk, bool updatedFlag, string policyNumber)
        {
            if (participant.PhoneNumber == pnalk.TELE_NUM.ToPhoneNumber())
            {
                return updatedFlag;
            }

            if ((participant.Person != null && participant.Person.Name.NameId == pnalk.NAME_ID) ||
               (participant.Business != null && participant.Business.Name.NameId == pnalk.NAME_ID))
            {
                participant.PhoneNumber = pnalk.TELE_NUM.ToPhoneNumber();
                updatedFlag = true;
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

        /// <summary>
        /// Gets the PolicyInfo Address contact populated from LifePro for the given addressId.
        /// Only retrieves the data from LifePro for the first call and returns the stored value
        /// from the global field for all other calls within the scope of this manager.
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [Trace]
        private async Task<Address> GetAddress(int addressId)
        {
            var paddr = await LifeProAccessor.GetPADDR(addressId);

            return new Address
            {
                AddressId = addressId,
                Line1 = paddr.ADDR_LINE_1.Trim(),
                Line2 = paddr.ADDR_LINE_2.Trim(),
                Line3 = paddr.ADDR_LINE_3.Trim(),
                City = paddr.CITY.Trim(),
                StateAbbreviation = paddr.STATE.ToState(),
                ZipCode = paddr.ZIP,
                ZipExtension = paddr.ZIP_EXTENSION.Trim(),
                BoxNumber = paddr.BOX_NUMBER,
                Country = paddr.COUNTRY.ToCountry()
            };
        }
    }
}