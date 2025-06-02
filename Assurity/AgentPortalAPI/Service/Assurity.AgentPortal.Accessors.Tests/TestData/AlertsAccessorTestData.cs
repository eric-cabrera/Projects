namespace Assurity.AgentPortal.Accessors.Tests.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.AssureLink.Entities;

[ExcludeFromCodeCoverage]
public static class AlertsAccessorTestData
{
    public static List<DistributionList> DistributionLists =>
    [
        new DistributionList
        {
            Id = 1,
            AgentId = "ABC1",
            Email = "ABC1@fake.com"
        },
        new DistributionList
        {
            Id = 2,
            AgentId = "XYZ1",
            Email = "XYZ1@fake.com"
        },
        new DistributionList
        {
            Id = 3,
            AgentId = "ASJ1",
            Email = "ASJ1@fake.com"
        },
        new DistributionList
        {
            Id = 4,
            AgentId = "SKA1",
            Email = "SKA1@fake.com"
        },
        new DistributionList
        {
            Id = 5,
            AgentId = "EUL1",
            Email = "EUL1@fake.com"
        }
    ];

    public static List<DistributionMaster> DistributionMasters =>
    [
        new DistributionMaster
        {
            Id = 1,
            AgentId = "ABC1",
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        },
        new DistributionMaster
        {
            Id = 2,
            AgentId = "XYZ1",
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        },
        new DistributionMaster
        {
            Id = 3,
            AgentId = "ASJ1",
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        },
        new DistributionMaster
        {
            Id = 4,
            AgentId = "SKA1",
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        },
        new DistributionMaster
        {
            Id = 5,
            AgentId = "EUL1",
            DisableAll = true,
            SelfAdd = true,
            SelfMet = true,
            SelfOutstanding = true,
            HierarchyAdd = true,
            HierarchyMet = true,
            HierarchyOutstanding = true
        }
    ];
}