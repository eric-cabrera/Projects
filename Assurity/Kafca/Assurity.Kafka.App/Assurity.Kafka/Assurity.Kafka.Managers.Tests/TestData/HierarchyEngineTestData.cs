namespace Assurity.Kafka.Managers.Tests.TestData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Assurity.PolicyInfo.Contracts.V1;

    [ExcludeFromCodeCoverage]
    public static class HierarchyEngineTestData
    {
        public static PolicyHierarchy PolicyHierarchy => new PolicyHierarchy
        {
            CompanyCode = "01",
            ApplicationDate = DateTime.Now,
            HierarchyBranches = new List<AgentHierarchy>
                {
                    new AgentHierarchy
                    {
                        Agent = new Agent
                        {
                            AgentId = "1234",
                            IsServicingAgent = true,
                            IsWritingAgent = false,
                            Level = "13",
                            MarketCode = "WA",
                        },
                        HierarchyAgents = new List<HierarchyAgent>
                        {
                            new HierarchyAgent
                            {
                                Sequence = 1,
                                AgentId = "4321",
                                Level = "14",
                                MarketCode = "WA",
                            }
                        }
                    }
                }
        };
    }
}
