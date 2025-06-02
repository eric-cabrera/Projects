namespace Assurity.Kafka.Engines.Hierarchy
{
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.PolicyInfo.Contracts.V1;
    using NewRelic.Api.Agent;

    public abstract class BaseHierarchyEngine
    {
        public BaseHierarchyEngine(
            IDataStoreAccessor dataStoreAccessor,
            ILifeProAccessor lifeProAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor)
        {
            DataStoreAccessor = dataStoreAccessor;
            LifeProAccessor = lifeProAccessor;
            GlobalDataAccessor = globalDataAccessor;
            SupportDataAccessor = supportDataAccessor;
        }

        private IDataStoreAccessor DataStoreAccessor { get; }

        private ILifeProAccessor LifeProAccessor { get; }

        private IGlobalDataAccessor GlobalDataAccessor { get; }

        private ISupportDataAccessor SupportDataAccessor { get; }

        [Trace]
        public async Task<PolicyHierarchy> GetPolicyHierarchy(bool isMigrationWorker, Policy policy)
        {
            var agentHierarchy = await BuildAgentHierarchy(isMigrationWorker, policy.Agents, policy.ApplicationDate.GetValueOrDefault());

            var policyHierarchy = new PolicyHierarchy
            {
                CompanyCode = policy.CompanyCode,
                PolicyNumber = policy.PolicyNumber,
                ApplicationDate = policy.ApplicationDate.GetValueOrDefault(),
                HierarchyBranches = agentHierarchy,
            };

            return policyHierarchy;
        }

        [Trace]
        public HashSet<string> GetDistinctAgentIds(List<AgentHierarchy> hierarchy)
        {
            var distinctAgents = new HashSet<string>();

            hierarchy.ForEach(branch =>
            {
                // Add when the writing agent is not a terminated agent.
                if ((!branch.Agent.IsJustInTimeAgent) ||
                    (branch.Agent.IsJustInTimeAgent && branch.Agent.Participant?.Person?.Name?.NameId == 0))
                {
                    distinctAgents.Add(branch.Agent.AgentId);
                }

                branch.HierarchyAgents.ForEach(agent =>
                {
                    distinctAgents.Add(agent.AgentId);
                });
            });

            return distinctAgents;
        }

        [Trace]
        public AgentHierarchiesDTO CompareAgentHierarchies(string companyCode, string policyNum, List<AgentHierarchy>? oldAgentHierarchies, List<AgentHierarchy> newAgentHierarchies)
        {
            var newAgentIds = RetrieveAgentIds(newAgentHierarchies);

            if (oldAgentHierarchies != null)
            {
                var oldAgentIds = RetrieveAgentIds(oldAgentHierarchies);

                return new AgentHierarchiesDTO
                {
                    CompanyCode = companyCode,
                    PolicyNumber = policyNum,
                    AddedAgents = newAgentIds.Except(oldAgentIds).ToList(),
                    RemovedAgents = oldAgentIds.Except(newAgentIds).ToList()
                };
            }
            else
            {
                return new AgentHierarchiesDTO
                {
                    CompanyCode = companyCode,
                    PolicyNumber = policyNum,
                    AddedAgents = newAgentIds.ToList()
                };
            }
        }

        [Trace]
        public async Task<List<AgentHierarchy>> BuildAgentHierarchy(bool isMigrationWorker, List<Agent> agents, DateTime applicationDate)
        {
            var agentHierarchy = BuildInitialAgentHierarchy(agents);

            foreach (var hierarchyList in agentHierarchy)
            {
                var initialUpline = hierarchyList.Agent.IsJustInTimeAgent
                    ? await GetInitialJITAgentUpline(isMigrationWorker, hierarchyList, applicationDate)
                    : await GetInitialAgentUpline(isMigrationWorker, hierarchyList, applicationDate);

                if (initialUpline != null)
                {
                    hierarchyList.HierarchyAgents.Add(initialUpline);
                }

                if (!hierarchyList.Agent.IsWritingAgent && hierarchyList.Agent.IsServicingAgent)
                {
                    continue;
                }

                if (hierarchyList.HierarchyAgents.Count == 0)
                {
                    continue;
                }

                hierarchyList.HierarchyAgents = await GetAdditionalAgentUplines(isMigrationWorker, hierarchyList.HierarchyAgents, applicationDate);
            }

            return agentHierarchy;
        }

        [Trace]
        public List<string> RetrieveAgentIds(List<AgentHierarchy> agentHierarchyList)
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
        private async Task<HierarchyAgent?> GetInitialAgentUpline(bool isMigrationWorker, AgentHierarchy hierarchy, DateTime applicationDate)
        {
            var initialUpline = await GetAgentUpline(
                isMigrationWorker,
                hierarchy.Agent.AgentId,
                hierarchy.Agent.MarketCode,
                hierarchy.Agent.Level,
                applicationDate);

            if (!string.IsNullOrWhiteSpace(initialUpline?.AgentId))
            {
                return new HierarchyAgent
                {
                    Sequence = 1,
                    AgentId = initialUpline.AgentId.Trim(),
                    MarketCode = initialUpline.MarketCode.Trim(),
                    Level = initialUpline.Level.Trim(),
                };
            }

            return null;
        }

        [Trace]
        private async Task<HierarchyAgent?> GetInitialJITAgentUpline(bool isMigrationWorker, AgentHierarchy hierarchy, DateTime applicationDate)
        {
            var agentFolderIds = await GlobalDataAccessor.GetAgentFolderIdsFromAttributes(hierarchy.Agent.AgentId);

            foreach (var agentFolderId in agentFolderIds)
            {
                if (!string.IsNullOrWhiteSpace(agentFolderId))
                {
                    var queue = await GlobalDataAccessor.GetQueueFromFolderId(agentFolderId);
                    var queueDescriptionTask = SupportDataAccessor.GetQueueDescriptions();
                    var queueDescriptions = await queueDescriptionTask;

                    if (!string.IsNullOrWhiteSpace(queue) && queueDescriptions.Contains(queue.ToLower()))
                    {
                        var justInTimeAgent = await GlobalDataAccessor.GetJitAgentInfoFromFolderId(
                            hierarchy.Agent.AgentId,
                            agentFolderId,
                            hierarchy.Agent.MarketCode,
                            hierarchy.Agent.Level);

                        if (justInTimeAgent != null && !string.IsNullOrWhiteSpace(justInTimeAgent?.AgentId))
                        {
                            return new HierarchyAgent
                            {
                                Sequence = 1,
                                AgentId = justInTimeAgent?.UplineAgentId?.Trim(),
                                MarketCode = justInTimeAgent.UplineMarketCode?.Trim(),
                                Level = justInTimeAgent?.UplineLevel?.Trim(),
                            };
                        }
                        else
                        {
                            var initialUpline = await GetAgentUpline(
                                isMigrationWorker,
                                hierarchy.Agent.AgentId,
                                hierarchy.Agent.MarketCode,
                                hierarchy.Agent.Level,
                                applicationDate);

                            if (!string.IsNullOrWhiteSpace(initialUpline?.AgentId))
                            {
                                return new HierarchyAgent
                                {
                                    Sequence = 1,
                                    AgentId = initialUpline.AgentId.Trim(),
                                    MarketCode = initialUpline.MarketCode.Trim(),
                                    Level = initialUpline.Level.Trim(),
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }

        [Trace]
        private async Task<List<HierarchyAgent>> GetAdditionalAgentUplines(bool isMigrationWorker, List<HierarchyAgent> hierarchyAgents, DateTime applicationDate)
        {
            var i = 0;
            var moreItemsInList = true;

            while (moreItemsInList)
            {
                Agent upline = new Agent();
                bool getUplineFromLifePro = false;

                var agentFolderIds = await GlobalDataAccessor.GetAgentFolderIdsFromAttributes(hierarchyAgents[i].AgentId);
                if (agentFolderIds != null)
                {
                    foreach (var agentFolderId in agentFolderIds)
                    {
                        if (!string.IsNullOrWhiteSpace(agentFolderId))
                        {
                            var queue = await GlobalDataAccessor.GetQueueFromFolderId(agentFolderId);
                            var queueDescriptionTask = SupportDataAccessor.GetQueueDescriptions();
                            var queueDescriptions = await queueDescriptionTask;

                            if (!string.IsNullOrWhiteSpace(queue) && queueDescriptions.Contains(queue.ToLower()))
                            {
                                var justInTimeAgent = await GlobalDataAccessor.GetJitAgentInfoFromFolderId(
                                    hierarchyAgents[i].AgentId,
                                    agentFolderId,
                                    hierarchyAgents[i].MarketCode,
                                    hierarchyAgents[i].Level);

                                if (justInTimeAgent != null && !string.IsNullOrWhiteSpace(justInTimeAgent?.AgentId))
                                {
                                    upline = new Agent
                                    {
                                        AgentId = justInTimeAgent?.UplineAgentId?.Trim(),
                                        MarketCode = justInTimeAgent?.UplineMarketCode?.Trim(),
                                        Level = justInTimeAgent?.UplineLevel?.Trim()
                                    };

                                    getUplineFromLifePro = false;
                                }
                                else
                                {
                                    getUplineFromLifePro = true;
                                }
                            }
                            else
                            {
                                getUplineFromLifePro = true;
                            }
                        }
                    }
                }

                if (agentFolderIds == null || !agentFolderIds.Any() || getUplineFromLifePro)
                {
                    upline = await GetAgentUpline(
                        isMigrationWorker,
                        hierarchyAgents[i].AgentId,
                        hierarchyAgents[i].MarketCode,
                        hierarchyAgents[i].Level,
                        applicationDate);
                }

                i++;

                if (string.IsNullOrWhiteSpace(upline?.AgentId))
                {
                    moreItemsInList = false;
                }
                else
                {
                    hierarchyAgents.Add(new HierarchyAgent
                    {
                        Sequence = i + 1,
                        AgentId = upline.AgentId.Trim(),
                        MarketCode = upline.MarketCode.Trim(),
                        Level = upline.Level.Trim(),
                    });
                }
            }

            return hierarchyAgents;
        }

        [Trace]
        private List<AgentHierarchy> BuildInitialAgentHierarchy(List<Agent> agents)
        {
            var agentHierarchy = new List<AgentHierarchy>();

            for (int i = 0; i < agents.Count; i++)
            {
                agentHierarchy.Add(new AgentHierarchy
                {
                    Agent = agents[i],
                    HierarchyAgents = new List<HierarchyAgent>(),
                });
            }

            return agentHierarchy;
        }

        [Trace]
        private async Task<Agent?> GetAgentUpline(bool isMigrationWorker, string agentId, string marketcode, string level, DateTime issueDate)
        {
            if (isMigrationWorker)
            {
                return await DataStoreAccessor.GetAgentUpline(
                    agentId,
                    marketcode,
                    level,
                    issueDate);
            }
            else
            {
                return await LifeProAccessor.GetAgentUpline(
                    agentId,
                    marketcode,
                    level,
                    issueDate);
            }
        }
    }
}
