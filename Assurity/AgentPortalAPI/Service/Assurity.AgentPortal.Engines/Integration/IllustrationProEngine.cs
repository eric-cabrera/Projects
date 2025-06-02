namespace Assurity.AgentPortal.Engines.Integration
{
    public class IllustrationProEngine : IIllustrationProEngine
    {
        private static readonly Dictionary<string, string> IllustrationProDistributionChannelMapping = new Dictionary<string, string>()
        {
            { "IS", "IS" },
            { "ISNY", "IS" },
            { "IG", "IG" },
            { "IGNY", "IG" },
            { "UAM", "UAM" },
            { "TL", "TL" },
            { "*", "DEFAULT" }
        };

        public string GetFiservDistributionChannelForIllustrationPro(List<string>? marketCodes)
        {
            var key = marketCodes?.FirstOrDefault(marketCode => IllustrationProDistributionChannelMapping.ContainsKey(marketCode));
            return string.IsNullOrEmpty(key)
                ? IllustrationProDistributionChannelMapping.Values.Last()
                : IllustrationProDistributionChannelMapping[key];
        }
    }
}
