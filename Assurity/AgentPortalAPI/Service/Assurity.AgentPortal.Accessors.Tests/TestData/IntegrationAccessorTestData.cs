namespace Assurity.AgentPortal.Accessors.Tests.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.DataStore.Entities;

[ExcludeFromCodeCoverage]
public static class IntegrationAccessorTestData
{
    public static List<FiservDistributionChannel> FiservDistributionChannel =>
    [
        new FiservDistributionChannel
        {
            AccessCode = "DI",
            MarketCodes = "DI"
        },
        new FiservDistributionChannel
        {
            AccessCode = "FARM",
            MarketCodes = "FARM"
        },
        new FiservDistributionChannel
        {
            AccessCode = "IS",
            MarketCodes = "IS"
        },
        new FiservDistributionChannel
        {
            AccessCode = "IG",
            MarketCodes = "IG"
        },
        new FiservDistributionChannel
        {
            AccessCode = "TL",
            MarketCodes = "TL"
        },
        new FiservDistributionChannel
        {
            AccessCode = "UAM",
            MarketCodes = "UAM"
        },
        new FiservDistributionChannel
        {
            AccessCode = "WS",
            MarketCodes = "WS"
        }
    ];
}