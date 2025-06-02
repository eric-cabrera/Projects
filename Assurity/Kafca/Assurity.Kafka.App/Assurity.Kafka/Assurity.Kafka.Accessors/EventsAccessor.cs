namespace Assurity.Kafka.Accessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Castle.Core.Internal;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using NewRelic.Api.Agent;
    using Policy = Assurity.PolicyInfo.Contracts.V1.Policy;

    public class EventsAccessor : IEventsAccessor
    {
        public EventsAccessor(
            ILogger<EventsAccessor> logger,
            IConfigurationManager configurationManager,
            IMongoClient mongoClient)
        {
            Logger = logger;
            var database = mongoClient.GetDatabase(configurationManager.MongoDbDatabaseName);
            PolicyCollection = database.GetCollection<Policy>(configurationManager.MongoPolicyCollectionName);
            PolicyHierarchyCollection = database.GetCollection<PolicyHierarchy>(configurationManager.MongoPolicyHierarchyCollectionName);
            AgentPolicyAccessCollection = database.GetCollection<AgentPolicyAccess>(configurationManager.MongoAgentPolicyAccessCollectionName);
            RequirementMappingCollection = database.GetCollection<RequirementMapping>(configurationManager.MongoRequirementMappingCollectionName);
            InitialPaymentDeclinedRetentionDays = configurationManager.InitialPaymentDeclinedRetentionDays;
            TerminationRetentionYears = configurationManager.TerminationRetentionYears;
        }

        private int InitialPaymentDeclinedRetentionDays { get; }

        private int TerminationRetentionYears { get; }

        private ILogger<EventsAccessor> Logger { get; set; }

        private IMongoCollection<Policy> PolicyCollection { get; }

        private IMongoCollection<PolicyHierarchy> PolicyHierarchyCollection { get; }

        private IMongoCollection<AgentPolicyAccess> AgentPolicyAccessCollection { get; }

        private IMongoCollection<RequirementMapping> RequirementMappingCollection { get; }

        [Trace]
        public async Task<List<RequirementMapping>> GetAllRequirementMappingsAsync()
        {
            var filter = Builders<RequirementMapping>.Filter.Empty;
            return await RequirementMappingCollection.Find(filter).ToListAsync();
        }

        [Trace]
        public List<RequirementMapping> GetRequirementMappings(List<int> ids)
        {
            return
                RequirementMappingCollection
                .AsQueryable()
                .Where(r => ids.Contains(r.RequirementId))
                .ToList();
        }

        [Trace]
        public async Task<RequirementMapping?> GetRequirementMappingAsync(int requirementId)
        {
            var filter = Builders<RequirementMapping>.Filter.And(
                Builders<RequirementMapping>.Filter.Eq(req => req.RequirementId, requirementId));
            return await RequirementMappingCollection.Find(filter).SingleOrDefaultAsync();
        }

        [Trace]
        public async Task<bool> CheckIfPolicyExists(string policyNumber, string companyCode)
        {
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, companyCode));

            return await PolicyCollection.Find(filter).CountDocumentsAsync() > 0;
        }

        [Trace]
        public async Task<long> DeletePolicyAsync(string policyNumber, string companyCode)
        {
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, companyCode));

            var result = await PolicyCollection.DeleteOneAsync(filter);

            return result.DeletedCount;
        }

        [Trace]
        public async Task DeletePoliciesAsync(IEnumerable<string> policyNumbers)
        {
            await PolicyCollection.DeleteManyAsync(p => policyNumbers.Contains(p.PolicyNumber));
        }

        [Trace]
        public async Task<long> DeletePoliciesAsync(IClientSessionHandle session, List<string> policyNumbers)
        {
            var policiesFilter = Builders<Policy>.Filter.In(policy => policy.PolicyNumber, policyNumbers);

            var updatedResult = await PolicyCollection.DeleteManyAsync(session, policiesFilter);
            return updatedResult.DeletedCount;
        }

        [Trace]
        public async Task<long> DeletePolicyHierarchiesAsync(IClientSessionHandle session, List<string> policyNumbers)
        {
            var policiesFilter = Builders<PolicyHierarchy>.Filter.In(policy => policy.PolicyNumber, policyNumbers);

            var updatedResult = await PolicyHierarchyCollection.DeleteManyAsync(session, policiesFilter);
            return updatedResult.DeletedCount;
        }

        [Trace]
        public void DeletePolicyHierarchies(List<CompanyCodeAndPolicyNumber> companyCodesAndPolicyNumbers)
        {
            foreach (var record in companyCodesAndPolicyNumbers)
            {
                var filter = Builders<PolicyHierarchy>.Filter.And(
                    Builders<PolicyHierarchy>.Filter.Eq(p => p.PolicyNumber, record.PolicyNumber.Trim()),
                    Builders<PolicyHierarchy>.Filter.Eq(p => p.CompanyCode, record.CompanyCode));

                PolicyHierarchyCollection.DeleteOne(filter);
            }
        }

        [Trace]
        public async Task<long> DeletePolicyHierarchyAsync(string policyNumber, string companyCode)
        {
            var result =
                await PolicyHierarchyCollection.DeleteOneAsync(
                    pol => pol.PolicyNumber == policyNumber && pol.CompanyCode == companyCode);

            return result.DeletedCount;
        }

        [Trace]
        public async Task<string> CreatePolicyAsync(Policy policy)
        {
            try
            {
                policy.CreateDate = DateTime.Now;
                await PolicyCollection.InsertOneAsync(policy);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to insert policy {PolicyNumber} after being created. The policy may already exist.", policy.PolicyNumber);
            }

            return policy.Id;
        }

        [Trace]
        public async Task CreateOrReplacePolicyAsync(Policy policy)
        {
            try
            {
                var filter =
                    Builders<Policy>
                    .Filter
                    .And(
                        Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policy.PolicyNumber.Trim()),
                        Builders<Policy>.Filter.Eq(p => p.CompanyCode, policy.CompanyCode));

                var existingPolicy =
                    await PolicyCollection
                    .Find(filter)
                    .FirstOrDefaultAsync();

                if (existingPolicy != null)
                {
                    policy.Id = existingPolicy.Id;
                    policy.LastModified = DateTime.Now;
                    await PolicyCollection.ReplaceOneAsync(filter, policy);
                }
                else
                {
                    policy.CreateDate = DateTime.Now;
                    await PolicyCollection.InsertOneAsync(policy);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to insert policy {PolicyNumber}.", policy.PolicyNumber);
            }
        }

        [Trace]
        public async Task<long> UpdatePolicyAsync<T>(Policy policyIn, T obj, string objType)
        {
            policyIn.LastModified = DateTime.Now;
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode));

            var updateBuilder = Builders<Policy>.Update;
            var updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                updateBuilder.Set(objType, obj)
            };

            var result = await PolicyCollection.UpdateOneAsync(filter, updateBuilder.Combine(updates));
            return result.MatchedCount;
        }

        [Trace]
        public async Task<long> UpdatePolicyAsync<T>(Policy policyIn, Dictionary<string, T> objDictionary)
        {
            policyIn.LastModified = DateTime.Now;
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode));

            var updateBuilder = Builders<Policy>.Update;
            var updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified)
            };

            foreach (var obj in objDictionary)
            {
                objDictionary.TryGetValue(obj.Key, out var value);
                updates.Add(updateBuilder.Set(obj.Key, value));
            }

            var result = await PolicyCollection.UpdateOneAsync(filter, updateBuilder.Combine(updates));
            return result.MatchedCount;
        }

        [Trace]
        public async Task<long> UpdatePolicyBenefitsAsync<T>(Policy policyIn, Dictionary<string, T> objDictionary, long benefitId)
        {
            policyIn.LastModified = DateTime.Now;
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                Builders<Policy>.Filter.ElemMatch(p => p.Benefits, benefit => benefit.BenefitId == benefitId));

            var updateBuilder = Builders<Policy>.Update;
            var updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified)
            };

            foreach (var obj in objDictionary)
            {
                objDictionary.TryGetValue(obj.Key, out var value);

                switch (obj.Key)
                {
                    case BenefitProperties.BenefitAmount:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitAmount, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitAmount, Convert.ToDecimal(value)));
                        }

                        break;

                    case BenefitProperties.BenefitDescription:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitDescription, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitDescription, Convert.ToString(value)));
                        }

                        break;

                    case BenefitProperties.BenefitStatus:
                        updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitStatus, (Status)Convert.ChangeType(value, typeof(Status))));
                        break;

                    case BenefitProperties.BenefitStatusReason:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitStatusReason, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitStatusReason, (StatusReason)Convert.ChangeType(value, typeof(StatusReason))));
                        }

                        break;

                    case BenefitProperties.CoverageType:
                        updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().CoverageType, (CoverageType)Convert.ChangeType(value, typeof(CoverageType))));
                        break;

                    case BenefitProperties.DeathBenefitOption:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().DeathBenefitOption, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().DeathBenefitOption, (DeathBenefitOption)Convert.ChangeType(value, typeof(DeathBenefitOption))));
                        }

                        break;

                    case BenefitProperties.DividendOption:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().DividendOption, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().DividendOption, (DividendOption)Convert.ChangeType(value, typeof(DividendOption))));
                        }

                        break;

                    case BenefitProperties.PlanCode:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().PlanCode, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().PlanCode, Convert.ToString(value)));
                        }

                        break;

                    case BenefitProperties.BenefitOptions:
                        if (value == null)
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitOptions, null));
                        }
                        else
                        {
                            updates.Add(updateBuilder.Set(policy => policy.Benefits.FirstMatchingElement().BenefitOptions, (List<BenefitOption>)Convert.ChangeType(value, typeof(List<BenefitOption>))));
                        }

                        break;
                }
            }

            var result = await PolicyCollection.UpdateOneAsync(filter, updateBuilder.Combine(updates));
            return result.MatchedCount;
        }

        [Trace]
        public async Task<long> UpdateNameAndEmailAddressInPolicyRequirements(Policy policyIn, Person newPerson, bool isBusiness)
        {
            policyIn.LastModified = DateTime.Now;
            var updateBuilder = Builders<Policy>.Update;

            if (isBusiness)
            {
                var businessNameFilter = Builders<Policy>.Filter.And(
                    Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                    Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                    Builders<Policy>.Filter.ElemMatch(
                        p => p.Requirements,
                        requirement => requirement.AppliesTo != null &&
                        requirement.AppliesTo.IsBusiness == true &&
                        requirement.AppliesTo.Business != null &&
                        requirement.AppliesTo.Business.Name != null &&
                        requirement.AppliesTo.Business.Name.NameId == newPerson.Name.NameId));

                var updates = new List<UpdateDefinition<Policy>>
                {
                    updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                    updateBuilder.Set("Requirements.$[reqElem].AppliesTo.Business.Name", newPerson.Name),
                    updateBuilder.Set("Requirements.$[reqElem].AppliesTo.Business.EmailAddress", newPerson.EmailAddress)
                };

                var arrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("reqElem.AppliesTo.Business.Name.NameId", new BsonDocument("$eq", newPerson.Name.NameId)))
                };

                var businessNameResult = await PolicyCollection.UpdateOneAsync(
                    businessNameFilter,
                    updateBuilder.Combine(updates),
                    new UpdateOptions
                    {
                        ArrayFilters = arrayFilters
                    });

                return businessNameResult.ModifiedCount;
            }
            else
            {
                var personNameFilter = Builders<Policy>.Filter.And(
                    Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                    Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                    Builders<Policy>.Filter.ElemMatch(
                        p => p.Requirements,
                        requirement => requirement.AppliesTo != null &&
                        requirement.AppliesTo.IsBusiness == false &&
                        requirement.AppliesTo.Person != null &&
                        requirement.AppliesTo.Person.Name != null &&
                        requirement.AppliesTo.Person.Name.NameId == newPerson.Name.NameId));

                var updates = new List<UpdateDefinition<Policy>>
                {
                    updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                    updateBuilder.Set("Requirements.$[reqElem].AppliesTo.Person.Name", newPerson.Name),
                    updateBuilder.Set("Requirements.$[reqElem].AppliesTo.Person.EmailAddress", newPerson.EmailAddress)
                };

                var arrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("reqElem.AppliesTo.Person.Name.NameId", new BsonDocument("$eq", newPerson.Name.NameId)))
                };

                var personNameResult = await PolicyCollection.UpdateOneAsync(
                    personNameFilter,
                    updateBuilder.Combine(updates),
                    new UpdateOptions
                    {
                        ArrayFilters = arrayFilters
                    });

                return personNameResult.ModifiedCount;
            }
        }

        [Trace]
        public async Task<long> UpdateAddressInPolicyRequirements(Policy policyIn, Address newAddress)
        {
            policyIn.LastModified = DateTime.Now;
            var addressFilter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                Builders<Policy>.Filter.ElemMatch(
                    p => p.Requirements,
                    requirement => requirement.AppliesTo != null &&
                    requirement.AppliesTo.Address != null &&
                    requirement.AppliesTo.Address.AddressId == newAddress.AddressId));

            var updateBuilder = Builders<Policy>.Update;
            var updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                updateBuilder.Set("Requirements.$[reqElem].AppliesTo.Address", newAddress)
            };

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("reqElem.AppliesTo.Address.AddressId", new BsonDocument("$eq", newAddress.AddressId)))
            };

            var result = await PolicyCollection.UpdateOneAsync(
                addressFilter,
                updateBuilder.Combine(updates),
                new UpdateOptions
                {
                    ArrayFilters = arrayFilters
                });

            return result.ModifiedCount;
        }

        [Trace]
        public async Task<long> UpdatePhoneNumberInPolicyRequirements(Policy policyIn, int nameId, string phoneNumber)
        {
            policyIn.LastModified = DateTime.Now;
            var businessNameFilter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                Builders<Policy>.Filter.ElemMatch(
                    p => p.Requirements,
                    requirement => requirement.AppliesTo != null &&
                    requirement.AppliesTo.IsBusiness == true &&
                    requirement.AppliesTo.Business != null &&
                    requirement.AppliesTo.Business.Name != null &&
                    requirement.AppliesTo.Business.Name.NameId == nameId));

            var updateBuilder = Builders<Policy>.Update;
            var updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                updateBuilder.Set("Requirements.$[reqElem].AppliesTo.PhoneNumber", phoneNumber)
            };

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("reqElem.AppliesTo.Business.Name.NameId", new BsonDocument("$eq", nameId)))
            };

            var businessPhoneNumberResult = await PolicyCollection.UpdateOneAsync(
                businessNameFilter,
                updateBuilder.Combine(updates),
                new UpdateOptions
                {
                    ArrayFilters = arrayFilters
                });

            if (businessPhoneNumberResult.ModifiedCount != 0)
            {
                return businessPhoneNumberResult.ModifiedCount;
            }

            var personNameFilter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyIn.PolicyNumber.Trim()),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, policyIn.CompanyCode),
                Builders<Policy>.Filter.ElemMatch(
                    p => p.Requirements,
                    requirement => requirement.AppliesTo != null &&
                    requirement.AppliesTo.IsBusiness == false &&
                    requirement.AppliesTo.Person != null &&
                    requirement.AppliesTo.Person.Name != null &&
                    requirement.AppliesTo.Person.Name.NameId == nameId));

            updates = new List<UpdateDefinition<Policy>>
            {
                updateBuilder.Set(nameof(policyIn.LastModified), policyIn.LastModified),
                updateBuilder.Set("Requirements.$[reqElem].AppliesTo.PhoneNumber", phoneNumber)
            };

            arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("reqElem.AppliesTo.Person.Name.NameId", new BsonDocument("$eq", nameId)))
            };

            var personPhoneNumberResult = await PolicyCollection.UpdateOneAsync(
                personNameFilter,
                updateBuilder.Combine(updates),
                new UpdateOptions
                {
                    ArrayFilters = arrayFilters
                });

            return personPhoneNumberResult.ModifiedCount;
        }

        [Trace]
        public async Task<string> UpdatePolicyHierarchyAsync(string policyNumber, string companyCode, List<AgentHierarchy> agentHierarchies)
        {
            var filter = Builders<PolicyHierarchy>.Filter.And(
                Builders<PolicyHierarchy>.Filter.Eq(p => p.PolicyNumber, policyNumber),
                Builders<PolicyHierarchy>.Filter.Eq(p => p.CompanyCode, companyCode));

            var update = Builders<PolicyHierarchy>.
                Update.Set(policyHierarchy => policyHierarchy.HierarchyBranches, agentHierarchies)
                    .Set(policyHierarchy => policyHierarchy.LastModified, DateTime.Now);

            var options = new FindOneAndUpdateOptions<PolicyHierarchy>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After,
            };

            var result = await PolicyHierarchyCollection.FindOneAndUpdateAsync(filter, update, options);

            return result.Id.ToString();
        }

        [Trace]
        public async Task<long> UpdatePastDuePoliciesAsync(List<string> policyNumbers)
        {
            var policiesFilter = Builders<Policy>.Filter.In(policy => policy.PolicyNumber, policyNumbers);
            var update = Builders<Policy>.
                Update.Set(policy => policy.PastDue, true)
                    .Set(policy => policy.LastModified, DateTime.Now);

            var updatedResult = await PolicyCollection.UpdateManyAsync(policiesFilter, update);
            return updatedResult.ModifiedCount;
        }

        [Trace]
        public HashSet<CompanyCodeAndPolicyNumber> GetAllCompanyCodesAndPolicyNumbers()
        {
            return
                PolicyCollection
                .AsQueryable()
                .Select(p => new CompanyCodeAndPolicyNumber(p.CompanyCode, p.PolicyNumber))
                .ToHashSet();
        }

        [Trace]
        public async Task<Policy?> GetPolicyAsync(string policyNumber)
        {
            var filter = Builders<Policy>
                .Filter
                .Eq(policy => policy.PolicyNumber, policyNumber);

            return await PolicyCollection
                .Find(filter)
                .SingleOrDefaultAsync();
        }

        [Trace]
        public async Task<Policy?> GetPolicyAsync(string policyNumber, string companyCode)
        {
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyNumber),
                Builders<Policy>.Filter.Eq(p => p.CompanyCode, companyCode));
            return await PolicyCollection.Find(filter).SingleOrDefaultAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesAsync(
            List<PolicyRelationship> policyRelationships)
        {
            if ((policyRelationships?.Count ?? 0) == 0)
            {
                return new List<Policy>();
            }

            var filters = new List<FilterDefinition<Policy>>();

            foreach (var policyRelationship in policyRelationships)
            {
                var companyCodeFilter = Builders<Policy>.Filter
                    .Eq(policy => policy.CompanyCode, policyRelationship.CompanyCode);
                var policyNumberFilter = Builders<Policy>.Filter
                    .Eq(policy => policy.PolicyNumber, policyRelationship.PolicyNumber);

                var andFilter = Builders<Policy>.Filter
                    .And(companyCodeFilter, policyNumberFilter);

                filters.Add(andFilter);
            }

            var orFilter = Builders<Policy>.Filter.Or(filters);

            return await PolicyCollection
                .Find(orFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesAsync(short reqId)
        {
            var reqNumFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Requirements, req => req.Id == reqId);

            var filters = new List<FilterDefinition<Policy>>();

            return await PolicyCollection.Aggregate()
                .Match(reqNumFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesByGroupNumber(string groupNumber)
        {
            var groupNumberFilter = Builders<Policy>
                .Filter
                .Eq(policy => policy.Employer.Number, groupNumber);

            return await PolicyCollection
                .Find(groupNumberFilter)
                .ToListAsync();
        }

        [Trace]
        public List<CompanyCodeAndPolicyNumber> GetCompanyCodeAndPolicyNumberOfFlaggedPolicies()
        {
            return
                PolicyCollection
                .AsQueryable()
                .Where(p => p.Flagged)
                .Select(p => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = p.CompanyCode,
                    PolicyNumber = p.PolicyNumber
                })
                .ToList();
        }

        [Trace]
        public async Task<List<Policy>> GetFlaggedPoliciesAsync(int pageNumber, int nPerPage)
        {
            var flaggedFilter = Builders<Policy>
                .Filter
                .Eq(policy => policy.Flagged, true);

            return await PolicyCollection
                .Find(flaggedFilter)
                .Sort("{ _id: 1 }")
                .Skip(pageNumber > 0 ? ((pageNumber - 1) * nPerPage) : 0)
                .Limit(nPerPage)
                .ToListAsync();
        }

        [Trace]
        public async Task<long> SetChangedPoliciesFlagged(List<string> policyNumbers)
        {
            var policiesFilter = Builders<Policy>.Filter.In(policy => policy.PolicyNumber, policyNumbers);
            var update = Builders<Policy>.
                Update.Set(policy => policy.Flagged, true)
                    .Set(policy => policy.LastModified, DateTime.Now);

            var updatedResult = await PolicyCollection.UpdateManyAsync(policiesFilter, update);
            return updatedResult.ModifiedCount;
        }

        public long FlagPastDuePolicies()
        {
            var pastDueFilter = Builders<Policy>
                .Filter
                .Eq(policy => policy.PastDue, true);

            var updateFilter = Builders<Policy>
                .Update
                .Set(policy => policy.Flagged, true);

            var updateResult = PolicyCollection.UpdateMany(pastDueFilter, updateFilter);

            return updateResult.ModifiedCount;
        }

        public long FlagPendingPolicies()
        {
            var pendingFilter = Builders<Policy>
                .Filter
                .Eq(policy => policy.PolicyStatus, Status.Pending);

            var updateFilter = Builders<Policy>
                .Update
                .Set(policy => policy.Flagged, true);

            var result = PolicyCollection.UpdateMany(pendingFilter, updateFilter);

            return result.ModifiedCount;
        }

        [Trace]
        public IEnumerable<string> GetPolicyNumbersForDeletion(DateTime batchStartDate)
        {
            var retentionThreshold = batchStartDate.AddYears(-TerminationRetentionYears);

            return PolicyCollection
                .AsQueryable()
                .Where(policy => policy.Employer == null && policy.TerminationDate < retentionThreshold)
                .Select(p => p.PolicyNumber)
                .Distinct();
        }

        [Trace]
        public async Task<List<Policy>> GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(DateTime batchStartDate)
        {
            var initialPaymentDeclineTypes = new List<ReturnPaymentType>
            {
                ReturnPaymentType.InitialPaymentCheckDraftDeclined,
                ReturnPaymentType.InitialPaymentCardDeclined
            };

            var pastDate = batchStartDate.AddDays(-InitialPaymentDeclinedRetentionDays);
            return await PolicyCollection
                .Find(policies => policies.ReturnPaymentDate.HasValue &&
                    policies.ReturnPaymentDate.Value < pastDate &&
                    initialPaymentDeclineTypes.Contains(policies.ReturnPaymentType))
                .ToListAsync();
        }

        [Trace]
        public async Task<PolicyHierarchy?> GetPolicyHierarchyAsync(string policyNumber, string companyCode)
        {
            var filter = Builders<PolicyHierarchy>.Filter.And(
                Builders<PolicyHierarchy>.Filter.Eq(p => p.PolicyNumber, policyNumber),
                Builders<PolicyHierarchy>.Filter.Eq(p => p.CompanyCode, companyCode));
            return await PolicyHierarchyCollection.Find(filter).SingleOrDefaultAsync();
        }

        [Trace]
        public async Task UpdateOrCreatePolicyHierarchyAsync(PolicyHierarchy policyHierarchy)
        {
            policyHierarchy.LastModified = DateTime.Now;
            var filter = Builders<PolicyHierarchy>.Filter.And(
                Builders<PolicyHierarchy>.Filter.Eq(p => p.PolicyNumber, policyHierarchy.PolicyNumber),
                Builders<PolicyHierarchy>.Filter.Eq(p => p.CompanyCode, policyHierarchy.CompanyCode));

            var updateOptions = new UpdateOptions { IsUpsert = true };
            var update = Builders<PolicyHierarchy>.Update
                .Set(p => p.HierarchyBranches, policyHierarchy.HierarchyBranches)
                .Set(p => p.LastModified, policyHierarchy.LastModified);

            var options = new ReplaceOptions { IsUpsert = true };

            var updatedPolicyHierarchy = await PolicyHierarchyCollection.FindOneAndUpdateAsync(filter, update);

            if (updatedPolicyHierarchy == null)
            {
                await PolicyHierarchyCollection.InsertOneAsync(policyHierarchy);
            }
        }

        [Trace]
        public async Task<AgentPolicyAccess?> GetAgentPolicyAccessAsync(string agentId, string companyCode)
        {
            var filter = Builders<AgentPolicyAccess>.Filter.And(
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.CompanyCode, companyCode),
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.AgentId, agentId));

            return await AgentPolicyAccessCollection.Find(filter).SingleOrDefaultAsync();
        }

        [Trace]
        public async Task<long> InsertAgentPolicyAccessAsync(string agentId, string policyNumber, string companyCode)
        {
            var filter = Builders<AgentPolicyAccess>.Filter.And(
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.CompanyCode, companyCode),
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.AgentId, agentId));
            var update = Builders<AgentPolicyAccess>
                .Update.AddToSet(a => a.PolicyNumbers, policyNumber)
                .Set(a => a.LastModified, DateTime.Now);

            var result = await AgentPolicyAccessCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                var agentPolicyAccess = new AgentPolicyAccess
                {
                    AgentId = agentId,
                    CompanyCode = companyCode,
                    PolicyNumbers = new List<string> { policyNumber },
                    LastModified = DateTime.Now,
                };

                await AgentPolicyAccessCollection.InsertOneAsync(agentPolicyAccess);
                return 1;
            }

            return result.MatchedCount;
        }

        [Trace]
        public async Task<long> RemoveAgentPolicyAccessAsync(string agentId, string policyNumber, string companyCode)
        {
            var filter = Builders<AgentPolicyAccess>.Filter.And(
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.CompanyCode, companyCode),
                Builders<AgentPolicyAccess>.Filter.Eq(a => a.AgentId, agentId));
            var update = Builders<AgentPolicyAccess>
                .Update.Pull(a => a.PolicyNumbers, policyNumber)
                .Set(a => a.LastModified, DateTime.Now);

            var result = await AgentPolicyAccessCollection.UpdateManyAsync(filter, update);

            return result.MatchedCount;
        }

        [Trace]
        public async Task<long> UpdateAgentPolicyAccessListAsync(IClientSessionHandle session, List<string> removePolicyNumbers)
        {
            var filter = Builders<AgentPolicyAccess>.Filter.Empty;

            var updateBuilder = Builders<AgentPolicyAccess>.Update;
            var updates = new List<UpdateDefinition<AgentPolicyAccess>>
            {
                updateBuilder.Set(a => a.LastModified, DateTime.Now),
            };

            foreach (var policyNumber in removePolicyNumbers)
            {
                updateBuilder.Pull(a => a.PolicyNumbers, policyNumber);
            }

            var result = await AgentPolicyAccessCollection.UpdateManyAsync(filter, updateBuilder.Combine(updates));

            return result.MatchedCount;
        }

        [Trace]
        public async Task<string> InsertPolicyBenefitAsync(Benefit benefit, string policyNumber, string companyCode)
        {
            try
            {
                var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(a => a.CompanyCode, companyCode),
                Builders<Policy>.Filter.Eq(a => a.PolicyNumber, policyNumber),
                Builders<Policy>.Filter.Not(Builders<Policy>.Filter.ElemMatch(a => a.Benefits, b => b.BenefitId.Equals(benefit.BenefitId))));

                var update = Builders<Policy>
                    .Update.AddToSet(a => a.Benefits, benefit)
                    .Set(a => a.LastModified, DateTime.Now);

                var options = new FindOneAndUpdateOptions<Policy>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After,
                };

                var result = await PolicyCollection.FindOneAndUpdateAsync(filter, update, options);

                return result.Id.ToString();
            }
            catch
            {
                // If the BenefitId already exists then it would give duplicate key exception.
                return null;
            }
        }

        [Trace]
        public async Task<string> RemovePolicyBenefitByBenefitIdAsync(string policyNumber, string companyCode, long benefitId)
        {
            var filter = Builders<Policy>.Filter.And(
                Builders<Policy>.Filter.Eq(a => a.CompanyCode, companyCode),
                Builders<Policy>.Filter.Eq(a => a.PolicyNumber, policyNumber));
            var update = Builders<Policy>
                .Update.PullFilter(a => a.Benefits, Builders<Benefit>.Filter.Where(b => b.BenefitId == benefitId))
                .Set(a => a.LastModified, DateTime.Now);

            var options = new FindOneAndUpdateOptions<Policy>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After,
            };

            var result = await PolicyCollection.FindOneAndUpdateAsync(filter, update, options);

            return result.Id.ToString();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithAnnuitantsByAddressIdAsync(int addrId)
        {
            var annuitantsAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Annuitants, annuitant => annuitant.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(annuitantsAddressFilter)
                .ToListAsync();
        }

        public async Task<List<Policy>> GetPoliciesWithAnnuitantsByNameIdAsync(int nameId)
        {
            var annuitantsNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy =>
                    policy.Annuitants,
                    annuitant => (annuitant.Participant.Business != null && annuitant.Participant.Business.Name.NameId == nameId) ||
                    (annuitant.Participant.Person != null && annuitant.Participant.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(annuitantsNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithAssigneeByNameIdAsync(int nameId)
        {
            var assigneeBusinessNameFilter = Builders<Policy>.Filter
                .Where(policy => policy.Assignee.Participant.Business != null && policy.Assignee.Participant.Business.Name.NameId == nameId);

            var assigneePersonNameFilter = Builders<Policy>.Filter
                .Where(policy => policy.Assignee.Participant.Person != null && policy.Assignee.Participant.Person.Name.NameId == nameId);

            var assigneeNameFilter = Builders<Policy>.Filter.Or(assigneeBusinessNameFilter, assigneePersonNameFilter);

            return await PolicyCollection.Aggregate()
                .Match(assigneeNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithAssigneeByAddressIdAsync(int addrId)
        {
            var assigneeAddressFilter = Builders<Policy>.Filter
                .Eq(policy => policy.Assignee.Participant.Address.AddressId, addrId);

            return await PolicyCollection.Aggregate()
                .Match(assigneeAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithBeneficiariesByAddressIdAsync(int addrId)
        {
            var beneficiariesAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Beneficiaries, beneficiary => beneficiary.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(beneficiariesAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithBeneficiariesByNameIdAsync(int nameId)
        {
            var beneficiariesNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy => policy.Beneficiaries,
                    beneficiary =>
                        (beneficiary.Participant.Business != null && beneficiary.Participant.Business.Name.NameId == nameId) ||
                        (beneficiary.Participant.Person != null && beneficiary.Participant.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(beneficiariesNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithAgentsByAddressIdAsync(int addrId)
        {
            var agentsAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Agents, agent => agent.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(agentsAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithAgentsByNameIdAsync(int nameId)
        {
            var agentsNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy => policy.Agents,
                    agent =>
                        (agent.Participant.Business != null && agent.Participant.Business.Name.NameId == nameId) ||
                        (agent.Participant.Person != null && agent.Participant.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(agentsNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithEmployerByNameIdAsync(int nameId)
        {
            var employerNameFilter = Builders<Policy>.Filter.Where(policy =>
                policy.Employer.Business != null && policy.Employer.Business.Name.NameId == nameId);

            return await PolicyCollection.Aggregate()
                .Match(employerNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithInsuredsByAddressIdAsync(int addrId)
        {
            var insuredsAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Insureds, insured => insured.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(insuredsAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithInsuredsByNameIdAsync(int nameId)
        {
            var insuredsNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy => policy.Insureds,
                    insured =>
                        (insured.Participant.Person != null && insured.Participant.Person.Name.NameId == nameId) ||
                        (insured.Participant.Business != null && insured.Participant.Business.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(insuredsNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithPayorsByAddressIdAsync(int addrId)
        {
            var payorsAddressFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy =>
                    policy.Payors,
                    payor => payor.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(payorsAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithPayorsByNameIdAsync(int nameId)
        {
            var payorsNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy =>
                        policy.Payors,
                    payor =>
                        (payor.Participant.Business != null && payor.Participant.Business.Name.NameId == nameId) ||
                        (payor.Participant.Person != null && payor.Participant.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(payorsNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithPayeeByAddressIdAsync(int addrId)
        {
            var payeeAddressFilter = Builders<Policy>.Filter
                .Eq(policy => policy.Payee.Participant.Address.AddressId, addrId);

            return await PolicyCollection.Aggregate()
                .Match(payeeAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithPayeeByNameIdAsync(int nameId)
        {
            var payeeBusinessNameFilter = Builders<Policy>.Filter.Where(policy =>
               policy.Payee.Participant.Business != null && policy.Payee.Participant.Business.Name.NameId == nameId);

            var payeePersonNameFilter = Builders<Policy>.Filter.Where(policy =>
                policy.Payee.Participant.Person != null && policy.Payee.Participant.Person.Name.NameId == nameId);

            var payeeNameFilter = Builders<Policy>.Filter.Or(payeeBusinessNameFilter, payeePersonNameFilter);

            return await PolicyCollection.Aggregate()
                .Match(payeeNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithOwnersByAddressIdAsync(int addrId)
        {
            var ownersAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Owners, owner => owner.Participant.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(ownersAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithOwnersByNameIdAsync(int nameId)
        {
            var ownersNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy =>
                        policy.Owners, owner =>
                        (owner.Participant.Business != null && owner.Participant.Business.Name.NameId == nameId) ||
                        (owner.Participant.Person != null && owner.Participant.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(ownersNameFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithRequirementsByAddressIdAsync(int addrId)
        {
            var requirementsAddressFilter = Builders<Policy>.Filter
                .ElemMatch(policy => policy.Requirements, requirement => requirement.AppliesTo.Address.AddressId == addrId);

            return await PolicyCollection.Aggregate()
                .Match(requirementsAddressFilter)
                .ToListAsync();
        }

        [Trace]
        public async Task<List<Policy>> GetPoliciesWithRequirementsByNameIdAsync(int nameId)
        {
            var requirementsNameFilter = Builders<Policy>.Filter
                .ElemMatch(
                    policy =>
                    policy.Requirements, requirement =>
                        (requirement.AppliesTo.Business != null && requirement.AppliesTo.Business.Name.NameId == nameId) ||
                        (requirement.AppliesTo.Person != null && requirement.AppliesTo.Person.Name.NameId == nameId));

            return await PolicyCollection.Aggregate()
                .Match(requirementsNameFilter)
                .ToListAsync();
        }

        private FilterDefinition<Policy> GetBusinessNameRequirementsFilter(int nameId)
        {
            return Builders<Policy>.Filter.ElemMatch(
                p => p.Requirements,
                requirement => requirement.AppliesTo != null &&
                requirement.AppliesTo.IsBusiness == true &&
                requirement.AppliesTo.Business != null &&
                requirement.AppliesTo.Business.Name != null &&
                requirement.AppliesTo.Business.Name.NameId == nameId);
        }

        private FilterDefinition<Policy> GetPersonNameRequirementsFilter(int nameId)
        {
            return Builders<Policy>.Filter.ElemMatch(
                p => p.Requirements,
                requirement => requirement.AppliesTo != null &&
                requirement.AppliesTo.IsBusiness == false &&
                requirement.AppliesTo.Person != null &&
                requirement.AppliesTo.Person.Name != null &&
                requirement.AppliesTo.Person.Name.NameId == nameId);
        }

        private string GetNameIdFilterTarget(bool isBusiness)
        {
            var variableField = isBusiness
                ? "Business"
                : "Person";

            return $"reqElem.AppliesTo.{variableField}.Name.NameId";
        }

        private FilterDefinition<Policy> GetPolicyNumberFilter(string policyNumber)
        {
            return Builders<Policy>.Filter.Eq(p => p.PolicyNumber, policyNumber.Trim());
        }

        private FilterDefinition<Policy> GetCompanyCodeFilter(string companyCode)
        {
            return Builders<Policy>.Filter.Eq(p => p.CompanyCode, companyCode);
        }
    }
}